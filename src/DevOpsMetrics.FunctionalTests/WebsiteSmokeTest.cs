using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace DevOpsMetrics.FunctionalTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [TestClass]
    public class WebsiteSmokeTest
    {
        private ChromeDriver _driver;
        private TestContext _testContextInstance;
        private string _webUrl = null;

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoWebHomeIndexPageTest()
        {
            //Arrange
            bool webLoaded;

            //Act
            string webURL = _webUrl + "home";
            Console.WriteLine("webURL:" + webURL);
            _driver.Navigate().GoToUrl(webURL);
            webLoaded = (_driver.Url == webURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/main/h2");
            Console.WriteLine("data:" + data.Text);

            //Assert
            Assert.IsTrue(webLoaded);
            Assert.IsTrue(data != null);
            Assert.AreEqual(data.Text, "High performing DevOps metrics");
        }

        //[TestMethod]
        //[TestCategory("SkipWhenLiveUnitTesting")]
        //[TestCategory("SmokeTest")]
        //public void GotoWebHomeDeploymentFrequencyPageTest()
        //{
        //    //Arrange
        //    bool webLoaded;

        //    //Act
        //    string webURL = _webUrl + "Home/DeploymentFrequency";
        //    Console.WriteLine("webURL:" + webURL);
        //    _driver.Navigate().GoToUrl(webURL);
        //    webLoaded = (_driver.Url == webURL);
        //    OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/main/h2");
        //    Console.WriteLine("data:" + data.Text);

        //    //Assert
        //    Assert.IsTrue(webLoaded);
        //    Assert.IsTrue(data != null);
        //    Assert.AreEqual(data.Text, "Deployment frequency");
        //}

        //[TestMethod]
        //[TestCategory("SkipWhenLiveUnitTesting")]
        //[TestCategory("SmokeTest")]
        //public void GotoWebHomeLeadTimeForChangesPageTest()
        //{
        //    //Arrange
        //    bool webLoaded;

        //    //Act
        //    string webURL = _webUrl + "Home/LeadTimeForChanges";
        //    Console.WriteLine("webURL:" + webURL);
        //    _driver.Navigate().GoToUrl(webURL);
        //    webLoaded = (_driver.Url == webURL);
        //    OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/main/h2");
        //    Console.WriteLine("data:" + data.Text);

        //    //Assert
        //    Assert.IsTrue(webLoaded);
        //    Assert.IsTrue(data != null);
        //    Assert.AreEqual(data.Text, "Lead time for changes");
        //}

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoWebHomeMTTRPageTest()
        {
            //Arrange
            bool webLoaded;

            //Act
            string webURL = _webUrl + "Home/MeanTimeToRestore";
            Console.WriteLine("webURL:" + webURL);
            _driver.Navigate().GoToUrl(webURL);
            webLoaded = (_driver.Url == webURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/main/h2");
            Console.WriteLine("data:" + data.Text);

            //Assert
            Assert.IsTrue(webLoaded);
            Assert.IsTrue(data != null);
            Assert.AreEqual(data.Text, "Time to restore service");
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoWebHomeChangeFailureRatePageTest()
        {
            //Arrange
            bool webLoaded;

            //Act
            string webURL = _webUrl + "Home/ChangeFailureRate";
            Console.WriteLine("webURL:" + webURL);
            _driver.Navigate().GoToUrl(webURL);
            webLoaded = (_driver.Url == webURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/main/h2");
            Console.WriteLine("data:" + data.Text);

            //Assert
            Assert.IsTrue(webLoaded);
            Assert.IsTrue(data != null);
            Assert.AreEqual(data.Text, "Change failure rate");
        }

        [TestMethod]
        [TestCategory("SkipWhenLiveUnitTesting")]
        [TestCategory("SmokeTest")]
        public void GotoWebHomeProjectDevOpsMetricsPageTest()
        {
            //Arrange
            bool webLoaded;

            //Act
            string webURL = _webUrl + "Home//Project?rowKey=samsmithnz_DevOpsMetrics";
            Console.WriteLine("webURL:" + webURL);
            _driver.Navigate().GoToUrl(webURL);
            webLoaded = (_driver.Url == webURL);
            OpenQA.Selenium.IWebElement data = _driver.FindElementByXPath(@"/html/body/div/main/form/h2");
            Debug.WriteLine("data:" + data.Text);

            //Assert
            Assert.IsTrue(webLoaded);
            Assert.IsTrue(data != null);
            Assert.AreEqual(data.Text, "  GitHub - DevOpsMetrics high performing DevOps metrics");
        }

        [TestInitialize]
        public void SetupTests()
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            _driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions);

            if (TestContext.Properties == null || TestContext.Properties.Count == 0)
            {
                throw new Exception("Select test settings file to continue");
            }
            else
            {
                _webUrl = TestContext.Properties["WebsiteUrl"].ToString();
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
