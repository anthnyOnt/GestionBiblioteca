using System.Threading;
using System.Dynamic;
using System.Collections.Generic;
using NUnit.Framework;
using Reqnroll;
using UITests.PageObjects;

namespace UITests.StepDefinitions.Ejemplares
{
    [Binding]
    public class EjemplarCreationSteps
    {
        private readonly EjemplarCreatePage _ejemplarCreatePage;
        private readonly ScenarioContext _scenarioContext;

        public EjemplarCreationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _ejemplarCreatePage = new EjemplarCreatePage(WebDriverManager.GetDriver());
        }

        [AfterScenario]
        public static void CleanupAfterEjemplarScenario()
        {
            WebDriverManager.CloseDriver();
        }

        [Given(@"que estoy en la página de creación de ejemplares")]
        public void DadoQueEstoyEnLaPaginaDeCreacionDeEjemplares()
        {
            _ejemplarCreatePage.NavigateToEjemplarCreatePage();
            Assert.That(_ejemplarCreatePage.IsOnCreatePage(), Is.True, 
                "No se pudo navegar a la página de creación de ejemplares");
        }

        [When(@"lleno el formulario de ejemplar con los siguientes datos:")]
        public void CuandoLlenoElFormularioDeEjemplarConLosSiguientesDatos(Table table)
        {
            var data = new ExpandoObject() as IDictionary<string, object>;
            
            foreach (var row in table.Rows)
            {
                data[row["Campo"]] = row["Valor"];
            }
            
            _ejemplarCreatePage.FillEjemplarForm(
                descripcion: data.ContainsKey("Descripcion") ? data["Descripcion"]?.ToString() ?? "" : "",
                observaciones: data.ContainsKey("Observaciones") ? data["Observaciones"]?.ToString() ?? "" : "",
                fechaAdquisicion: data.ContainsKey("FechaAdquisicion") ? data["FechaAdquisicion"]?.ToString() ?? "" : "",
                disponible: data.ContainsKey("Disponible") ? data["Disponible"]?.ToString() ?? "" : ""
            );
        }

        [When(@"envío el formulario de ejemplar")]
        public void CuandoEnvioElFormularioDeEjemplar()
        {
            _ejemplarCreatePage.SubmitForm();
            Thread.Sleep(1000); // Wait for form processing
        }

        [Then(@"debería ver el resultado del ejemplar ""(.*)""")]
        public void EntoncesDeberiaVerElResultadoDelEjemplar(string resultadoEsperado)
        {
            switch (resultadoEsperado)
            {
                case "Aceptado":
                    Assert.That(_ejemplarCreatePage.IsOnSuccessPage(), Is.True, 
                        "Se esperaba la creación exitosa del ejemplar pero se encontraron errores de validación o aún está en la página de creación");
                    break;
                case "Rechazado":
                    Assert.That(_ejemplarCreatePage.HasValidationErrors(), Is.True, 
                        "Se esperaban errores de validación pero el ejemplar fue creado exitosamente");
                    break;
                default:
                    Assert.Fail($"Resultado esperado desconocido: {resultadoEsperado}");
                    break;
            }
        }
    }
}
