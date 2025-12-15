using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace UITests.PageObjects
{
    public class LectorIndexPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public LectorIndexPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        private IWebElement PageTitle => _driver.FindElement(By.TagName("h1"));
        private IList<IWebElement> LectorRows => _driver.FindElements(By.CssSelector("table.table tbody tr"));
        private IWebElement GetDeleteButton(IWebElement lectorRow) => lectorRow.FindElement(By.CssSelector("a.btn-outline-danger"));

        public void NavigateToLectorIndexPage()
        {
            _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Usuario");
            _wait.Until(driver => driver.FindElement(By.TagName("h1")).Displayed);
        }

        public bool IsOnIndexPage()
        {
            try
            {
                return PageTitle.Text.Contains("Lectores") && _driver.Url.Contains("/Usuario");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HasLectores()
        {
            try
            {
                return LectorRows.Count > 0;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public int GetLectorCount()
        {
            try
            {
                return LectorRows.Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public void ClickDeleteForFirstLector()
        {
            if (!HasLectores())
            {
                throw new InvalidOperationException("No hay lectores disponibles para eliminar");
            }

            var firstLectorRow = LectorRows.First();
            var deleteButton = GetDeleteButton(firstLectorRow);
            deleteButton.Click();
            
            _wait.Until(driver => driver.Url.Contains("/Delete"));
        }
    }
}
