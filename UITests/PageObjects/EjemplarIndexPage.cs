using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace UITests.PageObjects
{
    public class EjemplarIndexPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public EjemplarIndexPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        private IWebElement PageTitle => _driver.FindElement(By.TagName("h1"));
        private IList<IWebElement> EjemplarRows => _driver.FindElements(By.CssSelector("table.table tbody tr"));
        private IWebElement GetDeleteButton(IWebElement ejemplarRow) => ejemplarRow.FindElement(By.CssSelector("a.btn-outline-danger"));

        public void NavigateToEjemplarIndexPage()
        {
            _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Ejemplar");
            _wait.Until(driver => driver.FindElement(By.TagName("h1")).Displayed);
        }

        public bool IsOnIndexPage()
        {
            try
            {
                return PageTitle.Text.Contains("Ejemplares") && _driver.Url.Contains("/Ejemplar");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HasEjemplares()
        {
            try
            {
                return EjemplarRows.Count > 0;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public int GetEjemplarCount()
        {
            try
            {
                return EjemplarRows.Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public void ClickDeleteForFirstEjemplar()
        {
            if (!HasEjemplares())
            {
                throw new InvalidOperationException("No hay ejemplares disponibles para eliminar");
            }

            var firstEjemplarRow = EjemplarRows.First();
            var deleteButton = GetDeleteButton(firstEjemplarRow);
            deleteButton.Click();
            
            _wait.Until(driver => driver.Url.Contains("/Delete"));
        }
    }
}
