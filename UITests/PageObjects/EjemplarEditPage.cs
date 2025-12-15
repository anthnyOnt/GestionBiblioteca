using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace UITests.PageObjects
{
    public class EjemplarEditPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public EjemplarEditPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
        }

        private IWebElement IdLibroSelect => _driver.FindElement(By.CssSelector("[data-testid='idlibro']"));
        private IWebElement FechaAdquisicionInput => _driver.FindElement(By.CssSelector("[data-testid='fechaadquisicion']"));
        private IWebElement DescripcionInput => _driver.FindElement(By.CssSelector("[data-testid='descripcion']"));
        private IWebElement ObservacionesInput => _driver.FindElement(By.CssSelector("[data-testid='observaciones']"));
        private IWebElement DisponibleSelect => _driver.FindElement(By.CssSelector("[data-testid='disponible']"));
        private IWebElement SubmitButton => _driver.FindElement(By.CssSelector("[data-testid='submit-button']"));

        private IWebElement IdLibroError => _driver.FindElement(By.CssSelector("[data-testid='idlibro-error']"));
        private IWebElement FechaAdquisicionError => _driver.FindElement(By.CssSelector("[data-testid='fechaadquisicion-error']"));
        private IWebElement DescripcionError => _driver.FindElement(By.CssSelector("[data-testid='descripcion-error']"));
        private IWebElement ObservacionesError => _driver.FindElement(By.CssSelector("[data-testid='observaciones-error']"));

        public void NavigateToEjemplarEditPage(int ejemplarId)
        {
            _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Ejemplar/Edit/{ejemplarId}");
            _wait.Until(driver => driver.FindElement(By.CssSelector("[data-testid='descripcion']")).Displayed);
            
            // Log the initial state of the form
            Console.WriteLine($"Navigated to Edit page for Ejemplar {ejemplarId}");
            try
            {
                var idLibroSelect = new SelectElement(IdLibroSelect);
                Console.WriteLine($"  Initial IdLibro selected: {idLibroSelect.SelectedOption.Text} (value: {idLibroSelect.SelectedOption.GetAttribute("value")})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Warning: Could not get IdLibro selection: {ex.Message}");
            }
        }

        public void FillEjemplarForm(string descripcion, string observaciones, string fechaAdquisicion, string disponible)
        {
            ClearAndType(DescripcionInput, descripcion);
            ClearAndType(ObservacionesInput, observaciones);

            if (!string.IsNullOrEmpty(fechaAdquisicion))
            {
                FechaAdquisicionInput.Clear();
                var jsExecutor = (IJavaScriptExecutor)_driver;
                jsExecutor.ExecuteScript($"arguments[0].value = '{fechaAdquisicion}';", FechaAdquisicionInput);
            }

            if (!string.IsNullOrEmpty(disponible))
            {
                var selectElement = new SelectElement(DisponibleSelect);
                if (disponible.Equals("Disponible", StringComparison.OrdinalIgnoreCase) || disponible.Equals("Sí", StringComparison.OrdinalIgnoreCase))
                {
                    selectElement.SelectByValue("true");
                }
                else if (disponible.Equals("No Disponible", StringComparison.OrdinalIgnoreCase) || disponible.Equals("No", StringComparison.OrdinalIgnoreCase))
                {
                    selectElement.SelectByValue("false");
                }
            }

            Thread.Sleep(200);
        }

        public void SubmitForm()
        {
            Console.WriteLine("Clicking submit button...");
            try
            {
                // Log current form values before submit
                Console.WriteLine($"  Descripcion: {DescripcionInput.GetAttribute("value")}");
                Console.WriteLine($"  Observaciones: {ObservacionesInput.GetAttribute("value")}");
                Console.WriteLine($"  FechaAdquisicion: {FechaAdquisicionInput.GetAttribute("value")}");
                var disponibleSelect = new SelectElement(DisponibleSelect);
                Console.WriteLine($"  Disponible: {disponibleSelect.SelectedOption.Text} (value: {disponibleSelect.SelectedOption.GetAttribute("value")})");
                
                var idLibroSelect = new SelectElement(IdLibroSelect);
                Console.WriteLine($"  IdLibro: {idLibroSelect.SelectedOption.Text} (value: {idLibroSelect.SelectedOption.GetAttribute("value")})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging form values: {ex.Message}");
            }
            
            // Try using JavaScript to submit the form directly
            var jsExecutor = (IJavaScriptExecutor)_driver;
            jsExecutor.ExecuteScript("document.querySelector('form').submit();");
            Console.WriteLine("Form submitted via JavaScript");
            Thread.Sleep(500);
        }

        public bool IsOnSuccessPage()
        {
            try
            {
                var result = _wait.Until(driver =>
                {
                    var currentUrl = driver.Url;
                    return (currentUrl.Contains("/Ejemplar/Index") ||
                           currentUrl.Contains("/Ejemplar") && !currentUrl.Contains("/Edit")) &&
                           !currentUrl.Contains("/Edit");
                });

                Thread.Sleep(200);
                return !HasValidationErrors();
            }
            catch (WebDriverTimeoutException)
            {
                var currentUrl = _driver.Url;
                Console.WriteLine($"Navigation timeout. Current URL: {currentUrl}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Navigation check failed: {ex.Message}");
                return false;
            }
        }

        public bool HasValidationErrors()
        {
            try
            {
                Thread.Sleep(200);

                var idLibroError = HasErrorMessage(IdLibroError);
                var descripcionError = HasErrorMessage(DescripcionError);
                var observacionesError = HasErrorMessage(ObservacionesError);
                var fechaError = HasErrorMessage(FechaAdquisicionError);

                var hasErrors = idLibroError || descripcionError || observacionesError || fechaError;
                var stillOnEditPage = _driver.Url.Contains("/Edit");

                Console.WriteLine($"Validation check - URL: {_driver.Url}");
                Console.WriteLine($"  IdLibroError: {idLibroError}, DescripcionError: {descripcionError}");
                Console.WriteLine($"  ObservacionesError: {observacionesError}, FechaError: {fechaError}");
                Console.WriteLine($"  HasErrors: {hasErrors}, StillOnEditPage: {stillOnEditPage}");

                if (hasErrors)
                {
                    try { if (idLibroError) Console.WriteLine($"  IdLibro error: {IdLibroError.Text}"); } catch { }
                    try { if (descripcionError) Console.WriteLine($"  Descripcion error: {DescripcionError.Text}"); } catch { }
                    try { if (observacionesError) Console.WriteLine($"  Observaciones error: {ObservacionesError.Text}"); } catch { }
                    try { if (fechaError) Console.WriteLine($"  Fecha error: {FechaAdquisicionError.Text}"); } catch { }
                }

                return hasErrors || stillOnEditPage;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool IsOnEditPage()
        {
            return _driver.Url.Contains("/Ejemplar/Edit");
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
                return !string.IsNullOrEmpty(errorElement.Text) && errorElement.Displayed;
            }
            catch
            {
                return false;
            }
        }
    }
}
