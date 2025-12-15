using System;
using System.Linq;
using System.Threading;
using System.Dynamic;
using System.Collections.Generic;
using OpenQA.Selenium;
using NUnit.Framework;
using Reqnroll;
using UITests.PageObjects;

namespace UITests.StepDefinitions.Libros
{
    [Binding]
    public class LibroUpdateSteps
    {
        private readonly LibroEditPage _libroEditPage;
        private readonly ScenarioContext _scenarioContext;
        private int _testLibroId;
        private string _uniqueIsbn;

        public LibroUpdateSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _libroEditPage = new LibroEditPage(WebDriverManager.GetDriver());
        }

        [AfterScenario]
        public void CleanupAfterScenario()
        {
            // Clean up test libro if created
            if (_testLibroId > 0)
            {
                DeleteTestLibro(_testLibroId);
            }
            WebDriverManager.CloseDriver();
        }

        [Given(@"que existe un libro con los siguientes datos iniciales:")]
        public void DadoQueExisteUnLibroConLosSiguientesDatosIniciales(Table table)
        {
            var data = new Dictionary<string, string>();
            
            foreach (var row in table.Rows)
            {
                data[row["Campo"]] = row["Valor"];
            }

            // Generate unique ISBN using timestamp (max 13 chars)
            _uniqueIsbn = $"UPD{DateTime.Now.Ticks % 1000000000}";

            // Create test libro with unique ISBN
            _testLibroId = CreateTestLibro(
                titulo: data["Titulo"],
                isbn: _uniqueIsbn,  // Use unique ISBN
                sinopsis: data.ContainsKey("Sinopsis") ? data["Sinopsis"] : "",
                fechaPub: data.ContainsKey("FechaPub") ? data["FechaPub"] : "",
                idioma: data.ContainsKey("Idioma") ? data["Idioma"] : "",
                edicion: data.ContainsKey("Edicion") ? data["Edicion"] : ""
            );

            Assert.That(_testLibroId, Is.GreaterThan(0), 
                "No se pudo crear el libro de prueba inicial");
        }

        [Given(@"estoy en la página de edición de ese libro")]
        public void DadoQueEstoyEnLaPaginaDeEdicionDeEseLibro()
        {
            _libroEditPage.NavigateToLibroEditPage(_testLibroId);
            Assert.That(_libroEditPage.IsOnEditPage(), Is.True, 
                "No se pudo navegar a la página de edición del libro");
        }

        [When(@"actualizo el formulario de libro con los siguientes datos:")]
        public void CuandoActualizoElFormularioDeLibroConLosSiguientesDatos(Table table)
        {
            var data = new ExpandoObject() as IDictionary<string, object>;
            
            foreach (var row in table.Rows)
            {
                data[row["Campo"]] = row["Valor"];
            }
            
            // Get ISBN value, make all non-empty ISBNs unique to avoid conflicts
            string isbnValue = data.ContainsKey("ISBN") ? data["ISBN"]?.ToString() ?? "" : "";
            if (!string.IsNullOrEmpty(isbnValue))
            {
                // Make ISBN unique but keep it under 13 chars
                string suffix = (DateTime.Now.Ticks % 100000).ToString();
                // Take first chars to leave room for suffix
                int maxPrefixLength = 13 - suffix.Length;
                string prefix = isbnValue.Length > maxPrefixLength ? isbnValue.Substring(0, maxPrefixLength) : isbnValue;
                isbnValue = $"{prefix}{suffix}";
            }
            
            _libroEditPage.FillLibroForm(
                titulo: data.ContainsKey("Titulo") ? data["Titulo"]?.ToString() ?? "" : "",
                isbn: isbnValue,
                sinopsis: data.ContainsKey("Sinopsis") ? data["Sinopsis"]?.ToString() ?? "" : "",
                fechaPub: data.ContainsKey("FechaPub") ? data["FechaPub"]?.ToString() ?? "" : "",
                idioma: data.ContainsKey("Idioma") ? data["Idioma"]?.ToString() ?? "" : "",
                edicion: data.ContainsKey("Edicion") ? data["Edicion"]?.ToString() ?? "" : ""
            );
        }

        [When(@"envío el formulario de actualización de libro")]
        public void CuandoEnvioElFormularioDeActualizacionDeLibro()
        {
            _libroEditPage.SubmitForm();
            Thread.Sleep(300); // Wait for form submission
        }

        [Then(@"debería ver el resultado de actualización de libro ""(.*)""")]
        public void EntoncesDeberiaVerElResultadoDeActualizacionDeLibro(string resultadoEsperado)
        {
            switch (resultadoEsperado)
            {
                case "Aceptado":
                    Assert.That(_libroEditPage.IsOnSuccessPage(), Is.True, 
                        "Se esperaba la actualización exitosa del libro pero se encontraron errores de validación o aún está en la página de edición");
                    break;
                case "Rechazado":
                    Assert.That(_libroEditPage.HasValidationErrors(), Is.True, 
                        "Se esperaban errores de validación pero el libro fue actualizado exitosamente");
                    break;
                default:
                    Assert.Fail($"Resultado esperado desconocido: {resultadoEsperado}");
                    break;
            }
        }

        private int CreateTestLibro(string titulo, string isbn, string sinopsis, string fechaPub, string idioma, string edicion)
        {
            var driver = WebDriverManager.GetDriver();
            var createPage = new LibroCreatePage(driver);
            
            // Use the unique ISBN that was generated in the Given step
            var uniqueIsbn = _uniqueIsbn;
            
            // Navigate to create page
            createPage.NavigateToLibroCreatePage();
            
            // Fill and submit form with unique ISBN
            createPage.FillLibroForm(titulo, uniqueIsbn, sinopsis, fechaPub, idioma, edicion);
            createPage.SubmitForm();
            
            Thread.Sleep(300);
            
            // Check if creation was successful (should redirect to Index)
            if (!createPage.IsOnSuccessPage())
            {
                Console.WriteLine($"ERROR: Failed to create libro. Still on create page or has validation errors.");
                return -1;
            }
            
            // Already on index page after successful creation
            Thread.Sleep(200);
            
            // Find the libro row with matching ISBN and extract the ID from the Edit button href
            try
            {
                var rows = driver.FindElements(By.CssSelector("table.table tbody tr"));
                
                foreach (var row in rows)
                {
                    var cells = row.FindElements(By.TagName("td"));
                    if (cells.Count >= 2)
                    {
                        // ISBN is in column 1 (second column)
                        var isbnElement = cells[1].FindElement(By.CssSelector("span.badge"));
                        if (isbnElement.Text == uniqueIsbn)
                        {
                            // Found the row, now get the Edit button href
                            var editButton = row.FindElement(By.CssSelector("a.btn-outline-primary"));
                            var href = editButton.GetAttribute("href");
                            // Extract ID from href like "/Libro/Edit/123"
                            var idString = href.Split('/').Last();
                            if (int.TryParse(idString, out int libroId))
                            {
                                return libroId;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting libro ID: {ex.Message}");
            }
            
            return -1;
        }

        private void DeleteTestLibro(int libroId)
        {
            try
            {
                var driver = WebDriverManager.GetDriver();
                // Navigate to delete page and confirm deletion
                driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Libro/Delete/{libroId}");
                Thread.Sleep(200);
                
                // Find and click delete confirmation button if exists
                try
                {
                    var deleteButton = driver.FindElement(By.CssSelector("[data-testid='delete-confirm-button']"));
                    deleteButton.Click();
                    Thread.Sleep(200);
                }
                catch
                {
                    // Ignore if delete button not found
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
