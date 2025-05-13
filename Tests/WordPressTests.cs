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

namespace WordPressApiTests

{
    [TestFixture]

    public class WordPressPostLifecycleTests

    {
        [SetUp]

        public void Setup()

        {

            WordPressPostLifecycle.client = new HttpClient();

            WordPressPostLifecycle.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", WordPressPostLifecycle.Credentials);

        }
 
        [TearDown]

        public void TearDown()

        {

            WordPressPostLifecycle.client.Dispose();

        }
 
        [OneTimeSetUp]
        public async Task BeforeAllTests()

        {

            using var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", WordPressPostLifecycle.Credentials);

            var response = await UniversalMethods.SendGetRequest($"{WordPressPostLifecycle.BaseUrl}/posts",client);

            Assert.That(UniversalMethods.IsStatusCodeOk(response), "API should be accessible");

        }
        [Test]
        public async Task Smoke_Post_Put_Check()
        {
            var fastPostResponse = await UniversalMethods.SendPostRequest($"{WordPressPostLifecycle.BaseUrl}/posts",WordPressPostLifecycle.client);
            Assert.That(UniversalMethods.IsStatusCodeCreated(fastPostResponse), "Post should be created");
            var fastCreatedPost = await UniversalMethods.DeserializeResponse<WordPressPost>(fastPostResponse);
            Assert.That(fastCreatedPost.Id, Is.GreaterThan(0), "Created post should have an ID");
            var fastEditResponse = await UniversalMethods.SendPutRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{fastCreatedPost.Id}", WordPressPostLifecycle.client);
            Assert.That(UniversalMethods.IsStatusCodeOk(fastEditResponse));
            var fastUpdatedPost = await UniversalMethods.DeserializeResponse<WordPressPost>(fastEditResponse);
            Assert.That(fastUpdatedPost.Id, Is.EqualTo(fastCreatedPost.Id), "Post ID should remain the same");
            Assert.That(fastUpdatedPost.Title.Rendered,Is.Not.EqualTo(fastCreatedPost.Title.Rendered), "Titles shoud be different");
            Assert.That(fastUpdatedPost.Content.Rendered,Is.Not.EqualTo(fastCreatedPost.Content.Rendered), "Cocntent should be different");
            var fastThrashResponse = await UniversalMethods.SendDeleteRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{fastCreatedPost.Id}?",WordPressPostLifecycle.client);
            Assert.That(UniversalMethods.IsStatusCodeOk(fastThrashResponse));
            var getThrashedResponse = await UniversalMethods.SendGetRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{fastCreatedPost.Id}",WordPressPostLifecycle.client);
            Assert.That(UniversalMethods.IsStatusCodeOk(getThrashedResponse));
            var fastThrashedPost = await UniversalMethods.DeserializeResponse<WordPressPost>(getThrashedResponse);
            Assert.That(fastThrashedPost.Status,Is.EqualTo("trash"));

        }
 
        [Test]

        public async Task ShouldHandleFullPostLifecycle_Create_Edit_Delete()

        {

            var createStartTime = DateTime.Now;

            var createData = new

            {

                title = "Test Lifecycle Post",

                content = "Initial content for lifecycle testing",

                status = "publish"

            };
 
           
 
            var createResponse = await UniversalMethods.SendPostRequest($"{WordPressPostLifecycle.BaseUrl}/posts",WordPressPostLifecycle.client,createData);

            var createTime = (DateTime.Now - createStartTime).TotalMilliseconds;


            Assert.That(createTime, Is.LessThan(WordPressPostLifecycle.PerformanceTimeout), "Create operation should be fast");

            Assert.That(UniversalMethods.IsStatusCodeCreated(createResponse), "Post should be created");
 
            var createdPost = await UniversalMethods.DeserializeResponse<WordPressPost>(createResponse);

            Assert.That(createdPost.Id, Is.GreaterThan(0), "Created post should have an ID");

            Assert.That(createdPost.Title.Rendered, Is.EqualTo(createData.title));

            Assert.That(createdPost.Content.Rendered, Does.Contain(createData.content));
 
            

            await Task.Delay(1000);
 
            

            var editStartTime = DateTime.Now;

            var updateData = new

            {

                title = "Updated Lifecycle Post",

                content = "Updated content for lifecycle testing"

            };
 
            var editResponse = await UniversalMethods.SendPutRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{createdPost.Id}", WordPressPostLifecycle.client,updateData);

            var editTime = (DateTime.Now - editStartTime).TotalMilliseconds;

            Assert.That(editTime, Is.LessThan(WordPressPostLifecycle.PerformanceTimeout), "Edit operation should be fast");

            Assert.That(UniversalMethods.IsStatusCodeOk(editResponse), "Post should be updated");
 
            var updatedPost = await UniversalMethods.DeserializeResponse<WordPressPost>(editResponse);

            Assert.That(updatedPost.Id, Is.EqualTo(createdPost.Id), "Post ID should remain the same");

            Assert.That(updatedPost.Title.Rendered, Is.EqualTo(updateData.title));

            Assert.That(updatedPost.Content.Rendered, Does.Contain(updateData.content));

            Assert.That(updatedPost.Modified, Is.Not.EqualTo(createdPost.Modified), "Modified date should be updated");
 
            

            var getResponse = await UniversalMethods.SendGetRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{createdPost.Id}",WordPressPostLifecycle.client);

            Assert.That(UniversalMethods.IsStatusCodeOk(getResponse), "Should be able to get updated post");

            var retrievedPost = await UniversalMethods.DeserializeResponse<WordPressPost>(getResponse);

            Assert.That(retrievedPost.Title.Rendered, Is.EqualTo(updateData.title));

            Assert.That(retrievedPost.Content.Rendered, Does.Contain(updateData.content));
 
            

            await Task.Delay(1000);
 
            

            var deleteStartTime = DateTime.Now;

            var deleteResponse = await UniversalMethods.SendDeleteRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{createdPost.Id}?force=true",WordPressPostLifecycle.client);

            var deleteTime = (DateTime.Now - deleteStartTime).TotalMilliseconds;

            Assert.That(deleteTime, Is.LessThan(WordPressPostLifecycle.PerformanceTimeout), "Delete operation should be fast");

            Assert.That(UniversalMethods.IsStatusCodeOk(deleteResponse), "Post should be deleted");
 
            

            var checkDeletedResponse = await UniversalMethods.SendGetRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{createdPost.Id}",WordPressPostLifecycle.client);

            Assert.That(UniversalMethods.IsStatusCodeNotFound(checkDeletedResponse), "Deleted post should not be accessible");
 
            UniversalMethods.LogOperationTimes(createTime,editTime,deleteTime);

        }
 
        [Test]

        public async Task ShouldHandleErrorsAppropriately()

        {

            var nonExistentId = 9999999999999999;

            var errorResponse = await UniversalMethods.SendPutRequest($"{WordPressPostLifecycle.BaseUrl}/posts/{nonExistentId}",WordPressPostLifecycle.client);

            Assert.That(UniversalMethods.IsStatusCodeNotFound(errorResponse));
 
            var invalidResponse = await UniversalMethods.SendInvalidPostRequest($"{WordPressPostLifecycle.BaseUrl}/posts",WordPressPostLifecycle.client);

            Assert.That(UniversalMethods.IsStatusCodeBadRequest(invalidResponse));

        }

    }

}
 