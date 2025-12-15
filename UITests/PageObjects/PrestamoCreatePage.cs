using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;
using System.Threading;

namespace UITests.PageObjects
{
    public class PrestamoCreatePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly string _url = "http://localhost:5183/Prestamo/Create";

        public PrestamoCreatePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(3));
        }

        public void NavigateToPrestamoCreate()
        {
            _driver.Navigate().GoToUrl(_url);
        }

        public void SearchLector(string ci)
        {
            var ciInput = _wait.Until(d => d.FindElement(By.CssSelector("[data-testid='ci']")));
            ciInput.Clear();
            ciInput.SendKeys(ci);

            var buscarButton = _driver.FindElement(By.CssSelector("[data-testid='buscar-lector']"));
            buscarButton.Click();

            // Wait for page reload
            Thread.Sleep(300);
        }

        public bool IsLectorFound()
        {
            try
            {
                var alert = _driver.FindElement(By.ClassName("alert-success"));
                return alert.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void SearchLibro(string titulo)
        {
            var tituloInput = _wait.Until(d => d.FindElement(By.CssSelector("[data-testid='titulo']")));
            tituloInput.Clear();
            tituloInput.SendKeys(titulo);

            var buscarButton = _driver.FindElement(By.CssSelector("[data-testid='buscar-libro']"));
            buscarButton.Click();


            Thread.Sleep(500);
            
            Console.WriteLine($"Searched for libro: {titulo}");
            Console.WriteLine($"Current URL after search: {_driver.Url}");
        }

        public void AddEjemplar(int ejemplarId)
        {
            Console.WriteLine($"Attempting to add ejemplar {ejemplarId}...");
            

            try
            {
                var allButtons = _driver.FindElements(By.CssSelector("button[data-testid*='ejemplar']"));
                Console.WriteLine($"Found {allButtons.Count} ejemplar-related buttons:");
                foreach (var btn in allButtons)
                {
                    Console.WriteLine($"  - {btn.GetAttribute("data-testid")} : {btn.Text}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing buttons: {ex.Message}");
            }
            
            var addButton = _wait.Until(d => d.FindElement(By.CssSelector($"[data-testid='agregar-ejemplar-{ejemplarId}']")));
            addButton.Click();


            Thread.Sleep(300);
        }

        public bool IsEjemplarAdded(int ejemplarId)
        {
            try
            {
                var quitarButton = _driver.FindElement(By.CssSelector($"[data-testid='quitar-ejemplar-{ejemplarId}']"));
                return quitarButton.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void SetFechaLimite(int ejemplarIndex, string fecha)
        {
            // Find the date input for the given ejemplar index in the configuration section
            var dateInput = _wait.Until(d => d.FindElement(By.Name($"Lineas[{ejemplarIndex}].FechaLimite")));
            
            // Use JavaScript to set the date value
            var js = (IJavaScriptExecutor)_driver;
            js.ExecuteScript($"arguments[0].value = '{fecha}';", dateInput);
            
            Console.WriteLine($"Set FechaLimite for ejemplar at index {ejemplarIndex} to {fecha}");
        }

        public void SubmitForm()
        {
            // Find the submit button in the final form
            var submitButton = _wait.Until(d => 
                d.FindElements(By.CssSelector("button[type='submit']"))
                .FirstOrDefault(b => b.Text.Contains("Confirmar préstamo")));

            if (submitButton == null)
            {
                throw new Exception("Submit button 'Confirmar préstamo' not found");
            }

            Console.WriteLine("Submitting prestamo form...");
            submitButton.Click();

            // Wait for redirect to Index page
            Thread.Sleep(500);
        }

        public bool IsOnIndexPage()
        {
            var currentUrl = _driver.Url;
            return currentUrl.Contains("/Prestamo") && !currentUrl.Contains("/Create");
        }

        public string GetValidationError(string fieldTestId)
        {
            try
            {
                var errorElement = _driver.FindElement(By.CssSelector($"[data-testid='{fieldTestId}-error']"));
                return errorElement.Text;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }
    }
}
