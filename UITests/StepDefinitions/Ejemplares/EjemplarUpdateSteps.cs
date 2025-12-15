using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;
using UITests.PageObjects;

namespace UITests.StepDefinitions.Ejemplares
{
    [Binding]
    public class EjemplarUpdateSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly EjemplarEditPage _ejemplarEditPage;
        private int _testEjemplarId = -1;

        public EjemplarUpdateSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _ejemplarEditPage = new EjemplarEditPage(WebDriverManager.GetDriver());
        }

        [AfterScenario]
        public void CleanupAfterScenario()
        {
            // Clean up test ejemplar if created
            if (_testEjemplarId > 0)
            {
                DeleteTestEjemplar(_testEjemplarId);
            }
            WebDriverManager.CloseDriver();
        }

        [Given(@"que existe un ejemplar con los siguientes datos iniciales:")]
        public void DadoQueExisteUnEjemplarConLosSiguientesDatosIniciales(Table table)
        {
            var data = new Dictionary<string, string>();
            
            foreach (var row in table.Rows)
            {
                data[row["Campo"]] = row["Valor"];
            }

            // Create test ejemplar
            _testEjemplarId = CreateTestEjemplar(
                descripcion: data["Descripcion"],
                observaciones: data.ContainsKey("Observaciones") ? data["Observaciones"] : "",
                fechaAdquisicion: data.ContainsKey("FechaAdquisicion") ? data["FechaAdquisicion"] : "",
                disponible: data.ContainsKey("Disponible") ? data["Disponible"] : "Disponible"
            );

            Assert.That(_testEjemplarId, Is.GreaterThan(0), 
                "No se pudo crear el ejemplar de prueba inicial");
        }

        [Given(@"estoy en la página de edición de ese ejemplar")]
        public void DadoQueEstoyEnLaPaginaDeEdicionDeEseEjemplar()
        {
            _ejemplarEditPage.NavigateToEjemplarEditPage(_testEjemplarId);
            Assert.That(_ejemplarEditPage.IsOnEditPage(), Is.True, 
                "No se pudo navegar a la página de edición del ejemplar");
        }

        [When(@"actualizo el formulario de ejemplar con los siguientes datos:")]
        public void CuandoActualizoElFormularioDeEjemplarConLosSiguientesDatos(Table table)
        {
            var data = new ExpandoObject() as IDictionary<string, object>;
            
            foreach (var row in table.Rows)
            {
                data[row["Campo"]] = row["Valor"];
            }
            
            _ejemplarEditPage.FillEjemplarForm(
                descripcion: data.ContainsKey("Descripcion") ? data["Descripcion"]?.ToString() ?? "" : "",
                observaciones: data.ContainsKey("Observaciones") ? data["Observaciones"]?.ToString() ?? "" : "",
                fechaAdquisicion: data.ContainsKey("FechaAdquisicion") ? data["FechaAdquisicion"]?.ToString() ?? "" : "",
                disponible: data.ContainsKey("Disponible") ? data["Disponible"]?.ToString() ?? "" : ""
            );
        }

        [When(@"envío el formulario de actualización de ejemplar")]
        public void CuandoEnvioElFormularioDeActualizacionDeEjemplar()
        {
            _ejemplarEditPage.SubmitForm();
        }

        [Then(@"debería ver el resultado de actualización de ejemplar ""(.*)""")]
        public void EntoncesDeberiaVerElResultadoDeActualizacionDeEjemplar(string resultadoEsperado)
        {
            switch (resultadoEsperado)
            {
                case "Aceptado":
                    Assert.That(_ejemplarEditPage.IsOnSuccessPage(), Is.True, 
                        "Se esperaba la actualización exitosa del ejemplar pero se encontraron errores de validación o aún está en la página de edición");
                    break;
                case "Rechazado":
                    Assert.That(_ejemplarEditPage.HasValidationErrors(), Is.True, 
                        "Se esperaban errores de validación pero el ejemplar fue actualizado exitosamente");
                    break;
                default:
                    Assert.Fail($"Resultado esperado desconocido: {resultadoEsperado}");
                    break;
            }
        }

        private int CreateTestEjemplar(string descripcion, string observaciones, string fechaAdquisicion, string disponible)
        {
            var driver = WebDriverManager.GetDriver();
            var createPage = new EjemplarCreatePage(driver);
            
            // Navigate to create page
            createPage.NavigateToEjemplarCreatePage();
            
            // Fill and submit form
            createPage.FillEjemplarForm(descripcion, observaciones, fechaAdquisicion, disponible);
            createPage.SubmitForm();
            
            Thread.Sleep(300);
            
            // Check if creation was successful (should redirect to Index)
            if (!createPage.IsOnSuccessPage())
            {
                Console.WriteLine($"ERROR: Failed to create ejemplar. Still on create page or has validation errors.");
                return -1;
            }
            
            // Already on index page after successful creation
            Thread.Sleep(200);
            
            // Find the ejemplar row and extract the ID from the Edit button href
            try
            {
                var rows = driver.FindElements(By.CssSelector("table.table tbody tr"));
                Console.WriteLine($"Found {rows.Count} rows in the ejemplar table");
                
                // Get the first row (most recent ejemplar)
                if (rows.Count > 0)
                {
                    var firstRow = rows[0];
                    var editButton = firstRow.FindElement(By.CssSelector("a.btn-outline-primary"));
                    var href = editButton.GetAttribute("href");
                    // Extract ID from href like "/Ejemplar/Edit/123"
                    var idString = href.Split('/').Last();
                    if (int.TryParse(idString, out int ejemplarId))
                    {
                        Console.WriteLine($"Successfully extracted ejemplar ID: {ejemplarId}");
                        return ejemplarId;
                    }
                }
                Console.WriteLine($"ERROR: Could not find ejemplar in the index table");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting ejemplar ID: {ex.Message}");
            }
            
            return -1;
        }

        private void DeleteTestEjemplar(int ejemplarId)
        {
            try
            {
                var driver = WebDriverManager.GetDriver();
                // Navigate to delete page and confirm deletion
                driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Ejemplar/Delete/{ejemplarId}");
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
