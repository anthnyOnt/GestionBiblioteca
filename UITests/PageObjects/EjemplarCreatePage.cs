using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Linq;

namespace UITests.PageObjects
{
    public class EjemplarCreatePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public EjemplarCreatePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
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


        private IWebElement PageTitle => _driver.FindElement(By.TagName("h1"));


        public void NavigateToEjemplarCreatePage()
        {
            try
            {
                Console.WriteLine($"Navigating to: {TestConfig.BaseUrl}/Ejemplar/Create");
                _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Ejemplar/Create");
                

                _wait.Until(driver => driver.FindElement(By.CssSelector("[data-testid='descripcion']")).Displayed);
                Console.WriteLine("Successfully navigated to Ejemplar Create page");
                

                try
                {
                    var libroSelect = new SelectElement(IdLibroSelect);
                    var options = libroSelect.Options;
                    Console.WriteLine($"Books loaded in dropdown: {options.Count} options");
                    for (int i = 0; i < Math.Min(options.Count, 3); i++)
                    {
                        Console.WriteLine($"  Book option {i}: '{options[i].Text}' (value: '{options[i].GetAttribute("value")}')");
                    }
                    
                    if (options.Count <= 1)
                    {
                        Console.WriteLine("WARNING: No books available in dropdown - this may cause test failures!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking books dropdown: {ex.Message}");
                    throw; 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to navigate to Ejemplar Create page: {ex.Message}");
                Console.WriteLine($"Current URL: {_driver.Url}");
                throw;
            }
        }

        public void FillEjemplarForm(string descripcion, string observaciones, string fechaAdquisicion, string disponible)
        {
            try
            {
                Console.WriteLine($"Filling form with: Descripcion='{descripcion}', Observaciones='{observaciones}', Fecha='{fechaAdquisicion}', Disponible='{disponible}'");
                

                ClearAndType(DescripcionInput, descripcion);


                ClearAndType(ObservacionesInput, observaciones);


                if (!string.IsNullOrEmpty(fechaAdquisicion))
                {
                    Console.WriteLine($"Setting date: {fechaAdquisicion}");

                    FechaAdquisicionInput.Clear();
                    
                    // Use JavaScript to set the date value to avoid browser-specific issues
                    var jsExecutor = (IJavaScriptExecutor)_driver;
                    jsExecutor.ExecuteScript($"arguments[0].value = '{fechaAdquisicion}';", FechaAdquisicionInput);
                    

                    jsExecutor.ExecuteScript("arguments[0].dispatchEvent(new Event('change'));", FechaAdquisicionInput);
                    
                    Console.WriteLine($"Date set successfully");
                }


                var selectElement = new SelectElement(DisponibleSelect);
                if (disponible == "Disponible")
                {
                    selectElement.SelectByValue("true");
                    Console.WriteLine("Selected: Disponible = true");
                }
                else if (disponible == "No Disponible")
                {
                    selectElement.SelectByValue("false");
                    Console.WriteLine("Selected: Disponible = false");
                }


                try
                {
                    var libroSelectElement = new SelectElement(IdLibroSelect);
                    var options = libroSelectElement.Options;
                    Console.WriteLine($"Available book options: {options.Count}");
                    for (int i = 0; i < Math.Min(options.Count, 5); i++)
                    {
                        Console.WriteLine($"  Option {i}: '{options[i].Text}' (value: '{options[i].GetAttribute("value")}')");
                    }
                    
                    if (options.Count > 1) 
                    {
                        libroSelectElement.SelectByIndex(1); 
                        Console.WriteLine($"Selected book: '{options[1].Text}'");
                    }
                    else
                    {
                        Console.WriteLine("ERROR: No books available to select!");
                        throw new InvalidOperationException("No books available in dropdown - cannot create ejemplar without selecting a book");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error selecting book: {ex.Message}");
                    throw;
                }
                
                Console.WriteLine("Form filled successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error filling form: {ex.Message}");
                throw;
            }
        }

        public void SubmitForm()
        {
            Console.WriteLine("Submitting form...");
            try
            {
                SubmitButton.Click();
                Console.WriteLine("Form submitted successfully");
                Thread.Sleep(1000); // Wait for form processing
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error submitting form: {ex.Message}");
                throw;
            }
        }

        public bool IsOnSuccessPage()
        {
            try
            {

                _wait.Until(driver => 
                {
                    var currentUrl = driver.Url;
                    Console.WriteLine($"Checking URL: {currentUrl}");

                    return !currentUrl.Contains("/Ejemplar/Create");
                });
                

                var finalUrl = _driver.Url;
                Console.WriteLine($"Final URL: {finalUrl}");
                

                if (finalUrl.Contains("/Ejemplar/Index") || finalUrl.EndsWith("/Ejemplar"))
                {
                    return true;
                }
                
                return false;
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

                Thread.Sleep(1000);
                
                var idLibroError = HasErrorMessage(IdLibroError);
                var fechaError = HasErrorMessage(FechaAdquisicionError);
                var descripcionError = HasErrorMessage(DescripcionError);
                var observacionesError = HasErrorMessage(ObservacionesError);
                
                var hasErrors = idLibroError || fechaError || descripcionError || observacionesError;
                

                var stillOnCreatePage = _driver.Url.Contains("/Create");
                
                Console.WriteLine($"Validation check - URL: {_driver.Url}");
                Console.WriteLine($"  IdLibroError: {idLibroError}, FechaError: {fechaError}");
                Console.WriteLine($"  DescripcionError: {descripcionError}, ObservacionesError: {observacionesError}");
                Console.WriteLine($"  HasErrors: {hasErrors}, StillOnCreatePage: {stillOnCreatePage}");
                
                if (hasErrors)
                {

                    try { if (idLibroError) Console.WriteLine($"  IdLibro error: {IdLibroError.Text}"); } catch { }
                    try { if (fechaError) Console.WriteLine($"  Fecha error: {FechaAdquisicionError.Text}"); } catch { }
                    try { if (descripcionError) Console.WriteLine($"  Descripcion error: {DescripcionError.Text}"); } catch { }
                    try { if (observacionesError) Console.WriteLine($"  Observaciones error: {ObservacionesError.Text}"); } catch { }
                }
                

                if (!hasErrors && stillOnCreatePage)
                {
                    Console.WriteLine("  No visible errors but still on Create page - checking for hidden validation errors");
                    try
                    {

                        var validationSummary = _driver.FindElements(By.CssSelector(".validation-summary-errors, .text-danger"));
                        if (validationSummary.Any(vs => !string.IsNullOrWhiteSpace(vs.Text)))
                        {
                            Console.WriteLine($"  Found validation summary errors: {string.Join(", ", validationSummary.Select(vs => vs.Text))}");
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"  Error checking for validation summary: {ex.Message}");
                    }
                }
                
                return hasErrors || stillOnCreatePage;
            }
            catch (NoSuchElementException ex)
            {
                Console.WriteLine($"Element not found during validation check: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during validation check: {ex.Message}");
                return false;
            }
        }

        public bool IsOnCreatePage()
        {
            try
            {
                return _driver.Url.Contains("/Ejemplar/Create") && 
                       PageTitle.Text.Contains("Nuevo ejemplar");
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
