using NUnit.Framework;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WordPress.Drivers.DriverFactory;
using WordPress.Posts;
using WordPress.Configs.UniversalMethods;
using System.Net;
using AventStack.ExtentReports;
using WordPress.Reporting;

[assembly: Parallelizable(ParallelScope.Fixtures)]
namespace WordPressApiTests

{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]

    public class WordPressPostLifecycleTests: BaseTestWithReporting
    {
         private static ExtentReports _extent;
        private static ExtentTest _test;
        [SetUp]

        public void Setup(){}

        [TearDown]

        public void TearDown()

        {
                WordPressPostLifecycle.client.Value = null;

        }

        [OneTimeSetUp]
        public async Task BeforeAllTests()

        {
         
            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", WordPressPostLifecycle.Credentials);

            var response = await UniversalMethods.SendGetRequest($"{WordPressPostLifecycle.BaseUrl}/posts", client);

            Assert.That(UniversalMethods.IsStatusCodeOk(response), "API should be accessible");

        }
        [Test]
        [Parallelizable]
        [Retry(3)] 
        public async Task Smoke_Post_Put_Check()
        {
            test.Info("Starting smoke test for post creation, update, and deletion");
            var fastPostResponse = await UniversalMethods.SendPostRequest($"{WordPressPostLifecycle.BaseUrl}/posts", WordPressPostLifecycle.client.Value);
            Assert.That(UniversalMethods.IsStatusCodeCreated(fastPostResponse), "Post should be created");
            var fastCreatedPost = await UniversalMethods.DeserializeResponse<WordPressPost>(fastPostResponse);
            Assert.That(fastCreatedPost.Id, Is.GreaterThan(0), "Created post should have an ID");
            await Task.Delay(100);
            var fastEditResponse = await UniversalMethods.SendPutRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{fastCreatedPost.Id}", WordPressPostLifecycle.client.Value);
            Assert.That(UniversalMethods.IsStatusCodeOk(fastEditResponse));
            var fastUpdatedPost = await UniversalMethods.DeserializeResponse<WordPressPost>(fastEditResponse);
            Assert.That(fastUpdatedPost.Id, Is.EqualTo(fastCreatedPost.Id), "Post ID should remain the same");
            Assert.That(fastUpdatedPost.Title.Rendered, Is.Not.EqualTo(fastCreatedPost.Title.Rendered), "Titles shoud be different");
            Assert.That(fastUpdatedPost.Content.Rendered, Is.Not.EqualTo(fastCreatedPost.Content.Rendered), "Cocntent should be different");
            await Task.Delay(100);
            var fastThrashResponse = await UniversalMethods.SendDeleteRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{fastCreatedPost.Id}?", WordPressPostLifecycle.client.Value);
            Assert.That(UniversalMethods.IsStatusCodeOk(fastThrashResponse));
            await Task.Delay(100);
            var getThrashedResponse = await UniversalMethods.SendGetRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{fastCreatedPost.Id}", WordPressPostLifecycle.client.Value);
            Assert.That(UniversalMethods.IsStatusCodeOk(getThrashedResponse));
            var fastThrashedPost = await UniversalMethods.DeserializeResponse<WordPressPost>(getThrashedResponse);
            Assert.That(fastThrashedPost.Status, Is.EqualTo("trash"));

        }

        [Test]
        [Parallelizable]
         [Retry(3)] 

        public async Task ShouldHandleFullPostLifecycle_Create_Edit_Delete()

        {
            test.Info("Full Post Lifecycle Test: Create, Edit, Delete");

            var createStartTime = DateTime.Now;

            var createData = new

            {

                title = "Test Lifecycle Post",

                content = "Initial content for lifecycle testing",

                status = "publish"

            };



            var createResponse = await UniversalMethods.SendPostRequest($"{WordPressPostLifecycle.BaseUrl}/posts", WordPressPostLifecycle.client.Value, createData);

            var createTime = (DateTime.Now - createStartTime).TotalMilliseconds;


            Assert.That(createTime, Is.LessThan(WordPressPostLifecycle.PerformanceTimeout), "Create operation should be fast");

            Assert.That(UniversalMethods.IsStatusCodeCreated(createResponse), "Post should be created");

            var createdPost = await UniversalMethods.DeserializeResponse<WordPressPost>(createResponse);

            Assert.That(createdPost.Id, Is.GreaterThan(0), "Created post should have an ID");

            Assert.That(createdPost.Title.Rendered, Is.EqualTo(createData.title));

            Assert.That(createdPost.Content.Rendered, Does.Contain(createData.content));



            await Task.Delay(200);



            var editStartTime = DateTime.Now;

            var updateData = new

            {

                title = "Updated Lifecycle Post",

                content = "Updated content for lifecycle testing"

            };

            var editResponse = await UniversalMethods.SendPutRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{createdPost.Id}", WordPressPostLifecycle.client.Value, updateData);

            var editTime = (DateTime.Now - editStartTime).TotalMilliseconds;

            Assert.That(editTime, Is.LessThan(WordPressPostLifecycle.PerformanceTimeout), "Edit operation should be fast");

            Assert.That(UniversalMethods.IsStatusCodeOk(editResponse), "Post should be updated");

            var updatedPost = await UniversalMethods.DeserializeResponse<WordPressPost>(editResponse);

            Assert.That(updatedPost.Id, Is.EqualTo(createdPost.Id), "Post ID should remain the same");

            Assert.That(updatedPost.Title.Rendered, Is.EqualTo(updateData.title));

            Assert.That(updatedPost.Content.Rendered, Does.Contain(updateData.content));

            Assert.That(updatedPost.Modified, Is.Not.EqualTo(createdPost.Modified), "Modified date should be updated");
            await Task.Delay(200);

            var getResponse = await UniversalMethods.SendGetRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{createdPost.Id}", WordPressPostLifecycle.client.Value);

            Assert.That(UniversalMethods.IsStatusCodeOk(getResponse), "Should be able to get updated post");

            var retrievedPost = await UniversalMethods.DeserializeResponse<WordPressPost>(getResponse);

            Assert.That(retrievedPost.Title.Rendered, Is.EqualTo(updateData.title));

            Assert.That(retrievedPost.Content.Rendered, Does.Contain(updateData.content));



            await Task.Delay(200);



            var deleteStartTime = DateTime.Now;

            var deleteResponse = await UniversalMethods.SendDeleteRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{createdPost.Id}?force=true", WordPressPostLifecycle.client.Value);

            var deleteTime = (DateTime.Now - deleteStartTime).TotalMilliseconds;

            Assert.That(deleteTime, Is.LessThan(WordPressPostLifecycle.PerformanceTimeout), "Delete operation should be fast");

            Assert.That(UniversalMethods.IsStatusCodeOk(deleteResponse), "Post should be deleted");
            await Task.Delay(200);

            var checkDeletedResponse = await UniversalMethods.SendGetRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{createdPost.Id}", WordPressPostLifecycle.client.Value);

            Assert.That(UniversalMethods.IsStatusCodeNotFound(checkDeletedResponse), "Deleted post should not be accessible");

            UniversalMethods.LogOperationTimes(createTime, editTime, deleteTime);

        }

        [Test]
        [Parallelizable]
        [Retry(3)] 


        public async Task ShouldHandleErrorsAppropriately()

        {
            test.Info("Negative Test: Handle Errors Appropriately");

            var nonExistentId = 9999999999999999;

            var errorResponse = await UniversalMethods.SendPutRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{nonExistentId}", WordPressPostLifecycle.client.Value);

            Assert.That(UniversalMethods.IsStatusCodeNotFound(errorResponse));

            var invalidResponse = await UniversalMethods.SendInvalidPostRequest($"{WordPressPostLifecycle.BaseUrl}/posts", WordPressPostLifecycle.client.Value);

            Assert.That(UniversalMethods.IsStatusCodeBadRequest(invalidResponse));

        }

        // [OneTimeTearDown]
        // public void OneTimeTearDown()
        // {
        //     WordPressPostLifecycle.client?.Value?.Dispose();
        //     WordPressPostLifecycle.client?.Dispose();

        // }

    }

}
 