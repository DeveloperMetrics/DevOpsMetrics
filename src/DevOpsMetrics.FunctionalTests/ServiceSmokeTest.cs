using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DevOpsMetrics.Core.Models.AzureDevOps;
using DevOpsMetrics.Core.Models.GitHub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DevOpsMetrics.FunctionalTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestCategory("L2Test")]
    [TestCategory("SkipWhenLiveUnitTesting")]
    [TestClass]
    public class ServiceSmokeTest
    {
        private ChromeDriver _driver;
        private TestContext _testContextInstance;
        private string _serviceUrl = null;

        [TestMethod]
        public void GetAzureDevOpsSettingsTest()
        {
            //Arrange
            bool serviceLoaded;

            //Act
            string serviceUrl = _serviceUrl + "/api/Settings/GetAzureDevOpsSettings";
            Console.WriteLine("serviceUrl:" + serviceUrl);
            _driver.Navigate().GoToUrl(serviceUrl);
            serviceLoaded = (_driver.Url == serviceUrl);
            OpenQA.Selenium.IWebElement data = _driver.FindElement(By.XPath(@"/html/body/pre"));
            Console.WriteLine("data:" + data.Text);
            List<AzureDevOpsSettings> settings = new();
            if (string.IsNullOrEmpty(data?.Text) == false)
            {
                settings = JsonConvert.DeserializeObject<List<AzureDevOpsSettings>>(data.Text);
            }

            //Assert
            Assert.IsTrue(serviceLoaded);
            Assert.IsTrue(data != null);
            Assert.IsTrue(settings.Count >= 0);
            Assert.IsTrue(settings[0].Organization != null);

        }

        [TestMethod]
        public void GetGitGubSettingsTest()
        {
            //Arrange
            bool serviceLoaded;

            //Act
            string serviceUrl = _serviceUrl + "/api/Settings/GetGitHubSettings";
            Console.WriteLine("serviceUrl:" + serviceUrl);
            _driver.Navigate().GoToUrl(serviceUrl);
            serviceLoaded = (_driver.Url == serviceUrl);
            OpenQA.Selenium.IWebElement data = _driver.FindElement(By.XPath(@"/html/body/pre"));
            Console.WriteLine("data:" + data.Text);
            List<GitHubSettings> settings = new();
            if (string.IsNullOrEmpty(data?.Text) == false)
            {
                settings = JsonConvert.DeserializeObject<List<GitHubSettings>>(data.Text);
            }

            //Assert
            Assert.IsTrue(serviceLoaded);
            Assert.IsTrue(data != null);
            Assert.IsTrue(settings.Count >= 0);
            Assert.IsTrue(settings[0].Owner != null);
        }

        [TestInitialize]
        public void SetupTests()
        {
            ChromeOptions chromeOptions = new();
            chromeOptions.AddArguments("headless");
            _driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions);

            if (TestContext.Properties == null || TestContext.Properties.Count == 0)
            {
                throw new Exception("Select test settings file to continue");
            }
            else
            {
                _serviceUrl = TestContext.Properties["ServiceUrl"].ToString();
            }
        }

        [TestCleanup()]
        public void CleanupTests()
        {
            _driver.Quit();
        }

        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
            }
        }
    }
}
