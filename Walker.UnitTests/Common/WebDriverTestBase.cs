using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;

namespace WalkerScript.UnitTests.Common
{
    public class WebDriverTestBase
    {
        protected IWebDriver Driver { get; private set; }

        protected WebDriverWait Wait { get; private set; }

        [SetUp]
        public void Initialize()
        {
            Driver = new ChromeDriver();
            Wait = new WebDriverWait(Driver, new TimeSpan(0, 0, 5));
        }

        [TearDown]
        public void EndTest()
        {

            var ctx = TestContext.CurrentContext;
            if (ctx.Result.Outcome.Status == TestStatus.Failed)
            {
                var screenShot = Driver.TakeScreenshot();
                var path = Path.Combine(ctx.TestDirectory, string.Format("ex_{1:yyyy-MM-dd_HH.MM.ss}_{0}.png", ctx.Test.FullName, DateTime.Now));
                screenShot.SaveAsFile(path, ScreenshotImageFormat.Png);
            }
            else
            {
                var screenShot = Driver.TakeScreenshot();
                var path = Path.Combine(ctx.TestDirectory, string.Format("ok_{1:yyyy-MM-dd_HH.MM.ss}_{0}.png", ctx.Test.FullName, DateTime.Now));
                screenShot.SaveAsFile(path, ScreenshotImageFormat.Png);
            }
            Driver.Dispose();
        }
    }
}
