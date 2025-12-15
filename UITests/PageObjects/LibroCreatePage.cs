using System;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using UITests;

namespace UITests.PageObjects
{
    public class LibroCreatePage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public LibroCreatePage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }
        
        private IWebElement TituloInput => _driver.FindElement(By.CssSelector("[data-testid='titulo']"));
        private IWebElement IsbnInput => _driver.FindElement(By.CssSelector("[data-testid='isbn']"));
        private IWebElement SinopsisInput => _driver.FindElement(By.CssSelector("[data-testid='sinopsis']"));
        private IWebElement FechaPublicacionInput => _driver.FindElement(By.CssSelector("[data-testid='fechapublicacion']"));
        private IWebElement IdiomaInput => _driver.FindElement(By.CssSelector("[data-testid='idioma']"));
        private IWebElement EdicionInput => _driver.FindElement(By.CssSelector("[data-testid='edicion']"));
        private IWebElement SubmitButton => _driver.FindElement(By.CssSelector("[data-testid='submit-button']"));

        // Error message elements
        private IWebElement TituloError => _driver.FindElement(By.CssSelector("[data-testid='titulo-error']"));
        private IWebElement IsbnError => _driver.FindElement(By.CssSelector("[data-testid='isbn-error']"));
        private IWebElement SinopsisError => _driver.FindElement(By.CssSelector("[data-testid='sinopsis-error']"));
        private IWebElement FechaPublicacionError => _driver.FindElement(By.CssSelector("[data-testid='fechapublicacion-error']"));
        private IWebElement IdiomaError => _driver.FindElement(By.CssSelector("[data-testid='idioma-error']"));
        private IWebElement EdicionError => _driver.FindElement(By.CssSelector("[data-testid='edicion-error']"));

        // Page title element
        private IWebElement PageTitle => _driver.FindElement(By.TagName("h1"));

        // Page actions
        public void NavigateToLibroCreatePage()
        {
            _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Libro/Create");
            _wait.Until(driver => driver.FindElement(By.CssSelector("[data-testid='titulo']")).Displayed);
        }

        public void FillLibroForm(string titulo, string isbn, string sinopsis, string fechaPub, string idioma, string edicion)
        {
            ClearAndType(TituloInput, titulo);
            ClearAndType(IsbnInput, isbn);
            ClearAndType(SinopsisInput, sinopsis);
            

            if (!string.IsNullOrEmpty(fechaPub))
            {

                if (DateTime.TryParse(fechaPub, out DateTime date))
                {
                    ClearAndType(FechaPublicacionInput, date.ToString("yyyy-MM-dd"));
                }
                else
                {
                    ClearAndType(FechaPublicacionInput, fechaPub);
                }
            }
            
            ClearAndType(IdiomaInput, idioma);
            ClearAndType(EdicionInput, edicion);
        }

        public void SubmitForm()
        {
            SubmitButton.Click();
        }

        public bool IsOnSuccessPage()
        {
            try
            {

                var result = _wait.Until(driver => 
                {
                    var currentUrl = driver.Url;

                    return (currentUrl.Contains("/Libro/Index") || 
                           currentUrl.Contains("/Libro") && !currentUrl.Contains("/Create")) &&
                           !currentUrl.Contains("/Create");
                });
                

                Thread.Sleep(500); 
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
                Thread.Sleep(500);
                
                var tituloError = HasErrorMessage(TituloError);
                var isbnError = HasErrorMessage(IsbnError);
                var sinopsisError = HasErrorMessage(SinopsisError);
                var fechaError = HasErrorMessage(FechaPublicacionError);
                var idiomaError = HasErrorMessage(IdiomaError);
                var edicionError = HasErrorMessage(EdicionError);
                
                var hasErrors = tituloError || isbnError || sinopsisError || fechaError || idiomaError || edicionError;
                

                var stillOnCreatePage = _driver.Url.Contains("/Create");
                
                Console.WriteLine($"Validation check - URL: {_driver.Url}");
                Console.WriteLine($"  TituloError: {tituloError}, IsbnError: {isbnError}, SinopsisError: {sinopsisError}");
                Console.WriteLine($"  FechaError: {fechaError}, IdiomaError: {idiomaError}, EdicionError: {edicionError}");
                Console.WriteLine($"  HasErrors: {hasErrors}, StillOnCreatePage: {stillOnCreatePage}");
                
                if (hasErrors)
                {

                    try { if (tituloError) Console.WriteLine($"  Titulo error: {TituloError.Text}"); } catch { }
                    try { if (isbnError) Console.WriteLine($"  ISBN error: {IsbnError.Text}"); } catch { }
                    try { if (sinopsisError) Console.WriteLine($"  Sinopsis error: {SinopsisError.Text}"); } catch { }
                    try { if (fechaError) Console.WriteLine($"  Fecha error: {FechaPublicacionError.Text}"); } catch { }
                    try { if (idiomaError) Console.WriteLine($"  Idioma error: {IdiomaError.Text}"); } catch { }
                    try { if (edicionError) Console.WriteLine($"  Edicion error: {EdicionError.Text}"); } catch { }
                }
                
                return hasErrors || stillOnCreatePage;
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
                return _driver.Url.Contains("/Libro/Create") && 
                       PageTitle.Text.Contains("Nuevo libro");
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
