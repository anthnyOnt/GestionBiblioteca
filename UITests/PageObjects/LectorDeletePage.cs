using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UITests.PageObjects
{
    public class LectorDeletePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public LectorDeletePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Page elements
        private IWebElement PageTitle => _driver.FindElement(By.TagName("h1"));
        private IWebElement DeleteButton => _driver.FindElement(By.CssSelector("button.btn-danger"));
        private IWebElement CancelButton => _driver.FindElement(By.CssSelector("a.btn-outline-primary"));
        private IWebElement ConfirmationMessage => _driver.FindElement(By.CssSelector(".card-body p"));

        public bool IsOnDeletePage()
        {
            try
            {
                return PageTitle.Text.Contains("Eliminar") && _driver.Url.Contains("/Usuario/Delete");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ConfirmDelete()
        {
            try
            {
                Console.WriteLine("Confirming deletion by clicking Delete button");
                DeleteButton.Click();
                

                _wait.Until(driver => driver.Url.Contains("/Usuario") && !driver.Url.Contains("/Delete"));
                Console.WriteLine("Successfully confirmed deletion and returned to Index page");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error confirming delete: {ex.Message}");
                Console.WriteLine($"Current URL: {_driver.Url}");
                throw;
            }
        }

        public void CancelDelete()
        {
            try
            {
                Console.WriteLine("Canceling deletion by clicking Cancel button");
                CancelButton.Click();
                

                _wait.Until(driver => driver.Url.Contains("/Usuario") && !driver.Url.Contains("/Delete"));
                Console.WriteLine("Successfully canceled deletion and returned to Index page");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error canceling delete: {ex.Message}");
                throw;
            }
        }

        public string GetConfirmationMessage()
        {
            try
            {
                return ConfirmationMessage.Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting confirmation message: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
