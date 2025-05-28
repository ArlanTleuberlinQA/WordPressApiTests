using AventStack.ExtentReports;

using AventStack.ExtentReports.Reporter;

using NUnit.Framework;

using System;

using System.IO;
 
namespace WordPress.Reporting

{

    public class BaseTestWithReporting

    {

        protected static ExtentReports extent;

        protected ExtentTest test;
 
        [OneTimeSetUp]

        public void ReportSetup()

        {

            var reportDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");

            Directory.CreateDirectory(reportDir);

            var reportPath = Path.Combine(reportDir, $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");
 
            var htmlReporter = new ExtentSparkReporter(reportPath);

            extent = new ExtentReports();

            extent.AttachReporter(htmlReporter);

        }
 
        [SetUp]

        public void StartTest()

        {

            test = extent.CreateTest(TestContext.CurrentContext.Test.Name);

        }
 
        [TearDown]

        public void LogTestResult()

        {

            var result = TestContext.CurrentContext.Result;
 
            switch (result.Outcome.Status)

            {

                case NUnit.Framework.Interfaces.TestStatus.Passed:

                    test.Pass("Test passed");

                    break;

                case NUnit.Framework.Interfaces.TestStatus.Failed:

                    test.Fail($"Test failed: {result.Message}\n{result.StackTrace}");

                    break;

                default:

                    test.Warning("Test completed with unexpected status");

                    break;

            }
 
            extent.Flush();

        }
 
        [OneTimeTearDown]

        public void ReportCleanup()

        {

            extent.Flush();

        }

    }

}