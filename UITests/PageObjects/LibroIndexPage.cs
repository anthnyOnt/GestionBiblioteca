using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace UITests.PageObjects
{
    public class LibroIndexPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public LibroIndexPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        private IWebElement PageTitle => _driver.FindElement(By.TagName("h1"));
        private IList<IWebElement> LibroRows => _driver.FindElements(By.CssSelector("table.table tbody tr"));
        private IWebElement GetDeleteButton(IWebElement libroRow) => libroRow.FindElement(By.CssSelector("a.btn-outline-danger"));

        public void NavigateToLibroIndexPage()
        {
            _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Libro");
            _wait.Until(driver => driver.FindElement(By.TagName("h1")).Displayed);
        }

        public bool IsOnIndexPage()
        {
            try
            {
                return PageTitle.Text.Contains("Libros") && _driver.Url.Contains("/Libro");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HasLibros()
        {
            try
            {
                return LibroRows.Count > 0;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public int GetLibroCount()
        {
            try
            {
                return LibroRows.Count;
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public void ClickDeleteForFirstLibro()
        {
            if (!HasLibros())
            {
                throw new InvalidOperationException("No hay libros disponibles para eliminar");
            }

            var firstLibroRow = LibroRows.First();
            var deleteButton = GetDeleteButton(firstLibroRow);
            deleteButton.Click();
            
            _wait.Until(driver => driver.Url.Contains("/Delete"));
        }
    }
}
