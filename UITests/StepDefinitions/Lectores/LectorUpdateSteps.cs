using System;
using System.Linq;
using System.Threading;
using System.Dynamic;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using OpenQA.Selenium;
using NUnit.Framework;
using Reqnroll;
using UITests.PageObjects;

namespace UITests.StepDefinitions.Lectores
{
    [Binding]
    public class LectorUpdateSteps
    {
        private readonly LectorEditPage _lectorEditPage;
        private readonly ScenarioContext _scenarioContext;
        private int _testUserId;

        public LectorUpdateSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _lectorEditPage = new LectorEditPage(WebDriverManager.GetDriver());
        }

        [AfterScenario]
        public void CleanupAfterScenario()
        {
            // Clean up test user if created
            if (_testUserId > 0)
            {
                DeleteTestUser(_testUserId);
            }
            WebDriverManager.CloseDriver();
        }

        [Given(@"que existe un lector con los siguientes datos iniciales:")]
        public void DadoQueExisteUnLectorConLosSiguientesDatosIniciales(Table table)
        {
            var data = new Dictionary<string, string>();
            
            foreach (var row in table.Rows)
            {
                data[row["Campo"]] = row["Valor"];
            }

            // Create test user via API or direct navigation
            _testUserId = CreateTestUser(
                ci: data["CI"],
                primerNombre: data["PrimerNombre"],
                segundoNombre: data["SegundoNombre"],
                primerApellido: data["PrimerApellido"],
                segundoApellido: data["SegundoApellido"],
                telefono: data["Telefono"],
                correo: data["Correo"]
            );

            Assert.That(_testUserId, Is.GreaterThan(0), 
                "No se pudo crear el lector de prueba inicial");
        }

        [Given(@"estoy en la página de edición de ese lector")]
        public void DadoQueEstoyEnLaPaginaDeEdicionDeEseLector()
        {
            _lectorEditPage.NavigateToLectorEditPage(_testUserId);
            Assert.That(_lectorEditPage.IsOnEditPage(), Is.True, 
                "No se pudo navegar a la página de edición del lector");
        }

        [When(@"actualizo el formulario de lector con los siguientes datos:")]
        public void CuandoActualizoElFormularioDeLectorConLosSiguientesDatos(Table table)
        {
            var data = new ExpandoObject() as IDictionary<string, object>;
            
            foreach (var row in table.Rows)
            {
                data[row["Campo"]] = row["Valor"];
            }
            
            _lectorEditPage.FillLectorForm(
                ci: data.ContainsKey("CI") ? data["CI"]?.ToString() ?? "" : "",
                primerNombre: data.ContainsKey("PrimerNombre") ? data["PrimerNombre"]?.ToString() ?? "" : "",
                segundoNombre: data.ContainsKey("SegundoNombre") ? data["SegundoNombre"]?.ToString() ?? "" : "",
                primerApellido: data.ContainsKey("PrimerApellido") ? data["PrimerApellido"]?.ToString() ?? "" : "",
                segundoApellido: data.ContainsKey("SegundoApellido") ? data["SegundoApellido"]?.ToString() ?? "" : "",
                telefono: data.ContainsKey("Telefono") ? data["Telefono"]?.ToString() ?? "" : "",
                correo: data.ContainsKey("Correo") ? data["Correo"]?.ToString() ?? "" : ""
            );
        }

        [When(@"envío el formulario de actualización")]
        public void CuandoEnvioElFormularioDeActualizacion()
        {
            _lectorEditPage.SubmitForm();
            Thread.Sleep(300); // Wait for form submission
        }

        [Then(@"debería ver el resultado de actualización ""(.*)""")]
        public void EntoncesDeberiaVerElResultadoDeActualizacion(string resultadoEsperado)
        {
            switch (resultadoEsperado)
            {
                case "Aceptado":
                    Assert.That(_lectorEditPage.IsOnSuccessPage(), Is.True, 
                        "Se esperaba la actualización exitosa del lector pero se encontraron errores de validación o aún está en la página de edición");
                    break;
                case "Rechazado":
                    Assert.That(_lectorEditPage.HasValidationErrors(), Is.True, 
                        "Se esperaban errores de validación pero el lector fue actualizado exitosamente");
                    break;
                default:
                    Assert.Fail($"Resultado esperado desconocido: {resultadoEsperado}");
                    break;
            }
        }

        private int CreateTestUser(string ci, string primerNombre, string segundoNombre, 
            string primerApellido, string segundoApellido, string telefono, string correo)
        {
            var driver = WebDriverManager.GetDriver();
            var createPage = new LectorCreatePage(driver);
            
            // Navigate to create page
            createPage.NavigateToLectorCreatePage();
            
            // Fill and submit form
            createPage.FillLectorForm(ci, primerNombre, segundoNombre, primerApellido, 
                segundoApellido, telefono, correo);
            createPage.SubmitForm();
            
            Thread.Sleep(300);
            
            // After successful creation, navigate to index to find the user
            driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Usuario/Index");
            Thread.Sleep(200);
            
            // Find the user row with matching CI and extract the ID from the Edit button href
            try
            {
                var rows = driver.FindElements(By.CssSelector("table.table tbody tr"));
                foreach (var row in rows)
                {
                    var cells = row.FindElements(By.TagName("td"));
                    if (cells.Count >= 2)
                    {
                        var ciElement = cells[1].FindElement(By.CssSelector("span.badge"));
                        if (ciElement.Text == ci)
                        {
                            // Found the row, now get the Edit button href
                            var editButton = row.FindElement(By.CssSelector("a.btn-outline-primary"));
                            var href = editButton.GetAttribute("href");
                            // Extract ID from href like "/Usuario/Edit/123"
                            var idString = href.Split('/').Last();
                            if (int.TryParse(idString, out int userId))
                            {
                                return userId;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting user ID: {ex.Message}");
            }
            
            return -1;
        }

        private void DeleteTestUser(int userId)
        {
            try
            {
                var driver = WebDriverManager.GetDriver();
                // Navigate to delete page and confirm deletion
                driver.Navigate().GoToUrl($"{TestConfig.BaseUrl}/Usuario/Delete/{userId}");
                Thread.Sleep(200);
                
                // Find and click delete confirmation button if exists
                try
                {
                    var deleteButton = driver.FindElement(OpenQA.Selenium.By.CssSelector("[data-testid='delete-confirm-button']"));
                    deleteButton.Click();
                    Thread.Sleep(500);
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
