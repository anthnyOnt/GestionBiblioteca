using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace UITests
{
    public static class WebDriverManager
    {
        private static IWebDriver _driver;

        public static IWebDriver GetDriver()
        {
            if (_driver == null)
            {
                var options = new ChromeOptions();
                options.AddArguments("--headless");
                options.AddArguments("--no-sandbox");
                options.AddArguments("--disable-dev-shm-usage");
                options.AddArguments("--window-size=1920,1080");
                options.AddArguments("--disable-gpu");
                options.AddArguments("--disable-extensions");

                _driver = new ChromeDriver(options);
                _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3); 
            }
            return _driver;
        }

        public static void CloseDriver()
        {
            _driver?.Quit();
            _driver = null;
        }
    }
}
