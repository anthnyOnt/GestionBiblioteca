using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using UITests;

namespace UITests.PageObjects
{
    public class LectorCreatePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public LectorCreatePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }


        private IWebElement CiInput => _driver.FindElement(By.CssSelector("[data-testid='ci']"));
        private IWebElement PrimerNombreInput => _driver.FindElement(By.CssSelector("[data-testid='primernombre']"));
        private IWebElement SegundoNombreInput => _driver.FindElement(By.CssSelector("[data-testid='segundonombre']"));
        private IWebElement PrimerApellidoInput => _driver.FindElement(By.CssSelector("[data-testid='primerapellido']"));
        private IWebElement SegundoApellidoInput => _driver.FindElement(By.CssSelector("[data-testid='segundoapellido']"));
        private IWebElement TelefonoInput => _driver.FindElement(By.CssSelector("[data-testid='telefono']"));
        private IWebElement CorreoInput => _driver.FindElement(By.CssSelector("[data-testid='correo']"));
        private IWebElement SubmitButton => _driver.FindElement(By.CssSelector("[data-testid='submit-button']"));


        private IWebElement CiError => _driver.FindElement(By.CssSelector("[data-testid='ci-error']"));
        private IWebElement PrimerNombreError => _driver.FindElement(By.CssSelector("[data-testid='primernombre-error']"));
        private IWebElement SegundoNombreError => _driver.FindElement(By.CssSelector("[data-testid='segundonombre-error']"));
        private IWebElement PrimerApellidoError => _driver.FindElement(By.CssSelector("[data-testid='primerapellido-error']"));
        private IWebElement SegundoApellidoError => _driver.FindElement(By.CssSelector("[data-testid='segundoapellido-error']"));
        private IWebElement TelefonoError => _driver.FindElement(By.CssSelector("[data-testid='telefono-error']"));
        private IWebElement CorreoError => _driver.FindElement(By.CssSelector("[data-testid='correo-error']"));


        private IWebElement PageTitle => _driver.FindElement(By.TagName("h1"));


        public void NavigateToLectorCreatePage()
        {
            _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Usuario/Create");
            _wait.Until(driver => driver.FindElement(By.CssSelector("[data-testid='ci']")).Displayed);
        }

        public void FillLectorForm(string ci, string primerNombre, string segundoNombre, 
            string primerApellido, string segundoApellido, string telefono, string correo)
        {
            ClearAndType(CiInput, ci);
            ClearAndType(PrimerNombreInput, primerNombre);
            ClearAndType(SegundoNombreInput, segundoNombre);
            ClearAndType(PrimerApellidoInput, primerApellido);
            ClearAndType(SegundoApellidoInput, segundoApellido);
            ClearAndType(TelefonoInput, telefono);
            ClearAndType(CorreoInput, correo);
        }

        public void SubmitForm()
        {
            SubmitButton.Click();
        }

        public bool IsOnSuccessPage()
        {
            try
            {

                _wait.Until(driver => 
                {
                    var currentUrl = driver.Url;
                    return currentUrl.Contains("/Usuario") && !currentUrl.Contains("/Create");
                });
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
        }

        public bool HasValidationErrors()
        {
            try
            {

                Thread.Sleep(500);
                
                return HasErrorMessage(CiError) || 
                       HasErrorMessage(PrimerNombreError) ||
                       HasErrorMessage(SegundoNombreError) || 
                       HasErrorMessage(PrimerApellidoError) ||
                       HasErrorMessage(SegundoApellidoError) || 
                       HasErrorMessage(TelefonoError) ||
                       HasErrorMessage(CorreoError);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsOnCreatePage()
        {
            try
            {
                return _driver.Url.Contains("/Usuario/Create") && 
                       PageTitle.Text.Contains("Nuevo lector");
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private void ClearAndType(IWebElement element, string text)
        {
            element.Clear();
            if (!string.IsNullOrEmpty(text))
            {
                element.SendKeys(text);
            }
        }

        private bool HasErrorMessage(IWebElement errorElement)
        {
            try
            {
                return errorElement.Displayed && !string.IsNullOrWhiteSpace(errorElement.Text);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch (StaleElementReferenceException)
            {
                return false;
            }
        }
    }
}
