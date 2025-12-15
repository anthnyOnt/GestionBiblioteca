using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;
using UITests.PageObjects;

namespace UITests.StepDefinitions.Prestamos
{
    [Binding]
    public class PrestamoCreationSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly PrestamoCreatePage _prestamoCreatePage;
        private readonly IWebDriver _driver;
        
        private string _testUsuarioCi = "1234567";
        private string _testLibroTitulo = "";
        private int _testLibroId = -1;
        private List<int> _testEjemplarIds = new List<int>();
        private int _testPrestamoId = -1;

        public PrestamoCreationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _driver = WebDriverManager.GetDriver();
            _prestamoCreatePage = new PrestamoCreatePage(_driver);
        }

        [AfterScenario]
        public void CleanupAfterScenario()
        {
            // Clean up test data in reverse order of creation
            if (_testPrestamoId > 0)
            {
                DeleteTestPrestamo(_testPrestamoId);
            }
            
            foreach (var ejemplarId in _testEjemplarIds)
            {
                DeleteTestEjemplar(ejemplarId);
            }
            
            if (_testLibroId > 0)
            {
                DeleteTestLibro(_testLibroId);
            }
            
            if (!string.IsNullOrEmpty(_testUsuarioCi))
            {
                DeleteTestUsuario(_testUsuarioCi);
            }
            
            WebDriverManager.CloseDriver();
        }

        [Given(@"el sistema tiene un usuario con CI ""(.*)"" registrado")]
        public void DadoElSistemaTieneUnUsuarioConCIRegistrado(string ci)
        {
            _testUsuarioCi = ci;
            
            // Create test usuario (lector)
            var lectorPage = new LectorCreatePage(_driver);
            lectorPage.NavigateToLectorCreatePage();
            
            lectorPage.FillLectorForm(
                ci: ci,
                primerNombre: "Test",
                segundoNombre: "",
                primerApellido: "Lector",
                segundoApellido: "Prestamo",
                telefono: "12345678",
                correo: $"test{ci}@example.com"
            );
            
            lectorPage.SubmitForm();
            Thread.Sleep(500);
            
            Assert.That(lectorPage.IsOnSuccessPage(), Is.True, 
                "No se pudo crear el usuario de prueba");
        }

        [Given(@"el sistema tiene un libro con (\d+) ejemplares disponibles")]
        public void DadoElSistemaTieneUnLibroConEjemplaresDisponibles(int cantidadEjemplares)
        {
            // Create test libro
            var libroPage = new LibroCreatePage(_driver);
            libroPage.NavigateToLibroCreatePage();
            
            _testLibroTitulo = $"Test Libro Prestamo {DateTime.Now.Ticks}";
            
            libroPage.FillLibroForm(
                titulo: _testLibroTitulo,
                isbn: $"978{DateTime.Now.Ticks % 1000000000}",
                sinopsis: "Libro de prueba para préstamos",
                fechaPub: "2024-01-01",
                idioma: "Español",
                edicion: "1"
            );
            
            libroPage.SubmitForm();
            Thread.Sleep(500);
            
            Assert.That(libroPage.IsOnSuccessPage(), Is.True, 
                "No se pudo crear el libro de prueba");
            
            // Extract libro ID by searching for our specific libro title
            // Need to search through all rows since table may contain libros from previous tests
            Thread.Sleep(300);
            var libroRows = _driver.FindElements(By.CssSelector("table.table tbody tr"));
            
            // Search through all rows to find our libro by exact title match
            foreach (var row in libroRows)
            {
                var titleCell = row.FindElement(By.TagName("td")).Text;
                // Use full title for exact match (titles share same prefix "Test Libro Prestamo")
                if (titleCell == _testLibroTitulo)
                {
                    var editLink = row.FindElement(By.CssSelector("a[href*='/Libro/Edit/']"));
                    var href = editLink.GetAttribute("href");
                    _testLibroId = int.Parse(href.Split('/').Last());
                    Console.WriteLine($"Found test libro with ID: {_testLibroId} by exact title match: '{_testLibroTitulo}'");
                    break;
                }
            }
            
            if (_testLibroId <= 0)
            {
                throw new Exception("Failed to extract libro ID after creation");
            }
            
            Console.WriteLine($"Using libro ID: {_testLibroId}");
            
            // Create ejemplares for the libro
            var ejemplarPage = new EjemplarCreatePage(_driver);
            
            for (int i = 0; i < cantidadEjemplares; i++)
            {
                ejemplarPage.NavigateToEjemplarCreatePage();
                
                // Fill form WITHOUT selecting libro (we'll set it manually after)
                var js = (IJavaScriptExecutor)_driver;
                
                // Set descripcion
                js.ExecuteScript($"document.querySelector('[data-testid=\"descripcion\"]').value = 'Ejemplar {i + 1} para préstamo';");
                
                // Set fecha
                js.ExecuteScript($"document.querySelector('[data-testid=\"fechaadquisicion\"]').value = '{DateTime.Now.AddDays(-10):yyyy-MM-dd}';");
                
                // Set disponible
                js.ExecuteScript("document.querySelector('[data-testid=\"disponible\"]').value = 'true';");
                
                // CRITICAL: Set the libro ID to the one we just created
                js.ExecuteScript($"document.querySelector('[data-testid=\"idlibro\"]').value = '{_testLibroId}';");
                
                Console.WriteLine($"Filled form for ejemplar {i + 1}, libro ID set to {_testLibroId}");
                Thread.Sleep(200);
                
                ejemplarPage.SubmitForm();
                Thread.Sleep(300);
                
                Assert.That(ejemplarPage.IsOnSuccessPage(), Is.True, 
                    $"No se pudo crear el ejemplar {i + 1}");
                
                // Extract ejemplar ID from the most recent row (our just-created ejemplar)
                int ejemplarId = ExtractMostRecentEjemplarId();
                _testEjemplarIds.Add(ejemplarId);
                
                Console.WriteLine($"Created ejemplar {i + 1} with ID: {ejemplarId} for libro {_testLibroId}");
            }
            
            _scenarioContext["TestLibroTitulo"] = _testLibroTitulo;
            _scenarioContext["TestEjemplarIds"] = _testEjemplarIds;
        }

        [When(@"navego a la página de crear préstamo")]
        public void CuandoNavegoALaPaginaDeCrearPrestamo()
        {
            _prestamoCreatePage.NavigateToPrestamoCreate();
        }

        [When(@"busco el lector con CI ""(.*)""")]
        public void CuandoBuscoElLectorConCI(string ci)
        {
            _prestamoCreatePage.SearchLector(ci);
            Thread.Sleep(300);
            
            Assert.That(_prestamoCreatePage.IsLectorFound(), Is.True, 
                $"No se encontró el lector con CI {ci}");
        }

        [When(@"busco el libro por título")]
        public void CuandoBuscoElLibroPorTitulo()
        {
            var titulo = _scenarioContext["TestLibroTitulo"].ToString();
            _prestamoCreatePage.SearchLibro(titulo);
            Thread.Sleep(300);
        }

        [When(@"agrego el ejemplar (\d+) al préstamo")]
        public void CuandoAgregoElEjemplarAlPrestamo(int ejemplarIndex)
        {
            var ejemplarIds = (List<int>)_scenarioContext["TestEjemplarIds"];
            var ejemplarId = ejemplarIds[ejemplarIndex - 1]; // Convert 1-based to 0-based
            
            _prestamoCreatePage.AddEjemplar(ejemplarId);
            Thread.Sleep(300);
            
            Assert.That(_prestamoCreatePage.IsEjemplarAdded(ejemplarId), Is.True, 
                $"No se pudo agregar el ejemplar {ejemplarId}");
        }

        [When(@"establezco la fecha límite del ejemplar (\d+) para (\d+) días después")]
        public void CuandoEstablezcoLaFechaLimiteDelEjemplarParaDiasDespues(int ejemplarIndex, int dias)
        {
            var fecha = DateTime.Today.AddDays(dias).ToString("yyyy-MM-dd");
            
            // ejemplarIndex is 1-based, but the form uses 0-based index
            _prestamoCreatePage.SetFechaLimite(ejemplarIndex - 1, fecha);
            Thread.Sleep(200);
        }

        [When(@"confirmo el préstamo")]
        public void CuandoConfirmoElPrestamo()
        {
            _prestamoCreatePage.SubmitForm();
            Thread.Sleep(500);
        }

        [Then(@"el préstamo debe ser creado exitosamente")]
        public void EntoncesElPrestamoDebeSerCreadoExitosamente()
        {
            Assert.That(_prestamoCreatePage.IsOnIndexPage(), Is.True, 
                "Se esperaba la creación exitosa del préstamo pero no se redirigió a la página de índice");
            
            // Extract prestamo ID from the index page table
            _testPrestamoId = ExtractIdFromTable("prestamo");
            
            Console.WriteLine($"Prestamo created successfully with ID: {_testPrestamoId}");
        }

        [Then(@"debo ser redirigido a la página de préstamos")]
        public void EntoncesDebSerRedirigidoALaPaginaDePrestamos()
        {
            Assert.That(_prestamoCreatePage.IsOnIndexPage(), Is.True, 
                "No se redirigió correctamente a la página de préstamos");
        }

        private int ExtractIdFromTable(string entityType)
        {
            try
            {
                Thread.Sleep(200);
                var rows = _driver.FindElements(By.CssSelector("table.table tbody tr"));
                
                if (rows.Count > 0)
                {
                    var firstRow = rows[0];
                    
                    // Try to find Edit or Details button
                    IWebElement linkButton = null;
                    try
                    {
                        linkButton = firstRow.FindElement(By.CssSelector("a.btn-outline-primary"));
                    }
                    catch
                    {
                        try
                        {
                            linkButton = firstRow.FindElement(By.CssSelector("a[href*='/Edit/']"));
                        }
                        catch
                        {
                            linkButton = firstRow.FindElement(By.CssSelector("a[href*='/Details/']"));
                        }
                    }
                    
                    var href = linkButton.GetAttribute("href");
                    var idString = href.Split('/').Last();
                    
                    if (int.TryParse(idString, out int id))
                    {
                        Console.WriteLine($"Successfully extracted {entityType} ID: {id}");
                        return id;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting {entityType} ID: {ex.Message}");
            }
            
            return -1;
        }

        private int ExtractEjemplarIdForLibro(int libroId, int ejemplarIndex)
        {
            try
            {
                Thread.Sleep(200);
                
                // Navigate to Ejemplar index to see all ejemplares
                _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Ejemplar");
                Thread.Sleep(300);
                
                var rows = _driver.FindElements(By.CssSelector("table.table tbody tr"));
                Console.WriteLine($"Found {rows.Count} ejemplar rows total");
                
                // Find all ejemplares that match our libro by checking the row text
                var matchingEjemplares = new List<int>();
                
                foreach (var row in rows)
                {
                    try
                    {
                        // Get the edit link to extract ID
                        var editLink = row.FindElement(By.CssSelector("a[href*='/Ejemplar/Edit/']"));
                        var href = editLink.GetAttribute("href");
                        var idString = href.Split('/').Last();
                        
                        if (int.TryParse(idString, out int ejemplarId))
                        {
                            // Check if this ejemplar belongs to our libro by looking at IdLibro column
                            // The table should have IdLibro information
                            var cells = row.FindElements(By.TagName("td"));
                            if (cells.Count > 0)
                            {
                                var rowText = row.Text;
                                // Check if the row contains reference to our libro
                                // Since we can't easily get IdLibro from the display, we'll use a simpler approach:
                                // Just get the most recent ejemplares (they should be at the top)
                                matchingEjemplares.Add(ejemplarId);
                            }
                        }
                    }
                    catch
                    {
                        // Skip rows that don't have the expected structure
                        continue;
                    }
                }
                
                // Return the ejemplar at the requested index (most recent first)
                if (matchingEjemplares.Count > ejemplarIndex)
                {
                    return matchingEjemplares[ejemplarIndex];
                }
                
                Console.WriteLine($"Could not find ejemplar at index {ejemplarIndex}, returning most recent");
                return matchingEjemplares.Count > 0 ? matchingEjemplares[0] : -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting ejemplar ID for libro {libroId}: {ex.Message}");
                return -1;
            }
        }

        private int ExtractMostRecentEjemplarId()
        {
            try
            {
                // We're already on the Ejemplar index page after successful creation
                // Get ALL ejemplar IDs and return the highest one (most recently created)
                Thread.Sleep(200);
                var rows = _driver.FindElements(By.CssSelector("table.table tbody tr"));
                
                var allEjemplarIds = new List<int>();
                foreach (var row in rows)
                {
                    try
                    {
                        var editLink = row.FindElement(By.CssSelector("a[href*='/Ejemplar/Edit/']"));
                        var href = editLink.GetAttribute("href");
                        var idString = href.Split('/').Last();
                        
                        if (int.TryParse(idString, out int ejemplarId))
                        {
                            allEjemplarIds.Add(ejemplarId);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
                
                if (allEjemplarIds.Count > 0)
                {
                    var maxId = allEjemplarIds.Max();
                    Console.WriteLine($"Extracted most recent ejemplar ID: {maxId} (from {allEjemplarIds.Count} total ejemplares)");
                    return maxId;
                }
                
                Console.WriteLine("Could not extract most recent ejemplar ID - no ejemplares found");
                return -1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting most recent ejemplar ID: {ex.Message}");
                return -1;
            }
        }

        private void DeleteTestPrestamo(int prestamoId)
        {
            try
            {
                _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Prestamo/Delete/{prestamoId}");
                Thread.Sleep(200);
                
                try
                {
                    var deleteButton = _driver.FindElement(By.CssSelector("[data-testid='delete-confirm-button']"));
                    deleteButton.Click();
                    Thread.Sleep(200);
                }
                catch { }
            }
            catch { }
        }

        private void DeleteTestEjemplar(int ejemplarId)
        {
            try
            {
                _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Ejemplar/Delete/{ejemplarId}");
                Thread.Sleep(200);
                
                try
                {
                    var deleteButton = _driver.FindElement(By.CssSelector("[data-testid='delete-confirm-button']"));
                    deleteButton.Click();
                    Thread.Sleep(200);
                }
                catch { }
            }
            catch { }
        }

        private void DeleteTestLibro(int libroId)
        {
            try
            {
                _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Libro/Delete/{libroId}");
                Thread.Sleep(200);
                
                try
                {
                    var deleteButton = _driver.FindElement(By.CssSelector("[data-testid='delete-confirm-button']"));
                    deleteButton.Click();
                    Thread.Sleep(200);
                }
                catch { }
            }
            catch { }
        }

        private void DeleteTestUsuario(string ci)
        {
            try
            {
                // Navigate to Usuario index, find by CI, delete
                _driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Usuario");
                Thread.Sleep(200);
                
                // Search for the CI in the table
                var rows = _driver.FindElements(By.CssSelector("table.table tbody tr"));
                foreach (var row in rows)
                {
                    if (row.Text.Contains(ci))
                    {
                        var deleteButton = row.FindElement(By.CssSelector("a[href*='/Delete/']"));
                        var deleteUrl = deleteButton.GetAttribute("href");
                        
                        _driver.Navigate().GoToUrl(deleteUrl);
                        Thread.Sleep(200);
                        
                        try
                        {
                            var confirmButton = _driver.FindElement(By.CssSelector("[data-testid='delete-confirm-button']"));
                            confirmButton.Click();
                            Thread.Sleep(200);
                        }
                        catch { }
                        break;
                    }
                }
            }
            catch { }
        }
    }
}
