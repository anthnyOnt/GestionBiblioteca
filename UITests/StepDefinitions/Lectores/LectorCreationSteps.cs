using System.Threading;
using System.Dynamic;
using System.Collections.Generic;
using NUnit.Framework;
using Reqnroll;
using UITests.PageObjects;

namespace UITests.StepDefinitions.Lectores
{
    [Binding]
    public class LectorCreationSteps
    {
        private readonly LectorCreatePage _lectorCreatePage;
        private readonly ScenarioContext _scenarioContext;

        public LectorCreationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _lectorCreatePage = new LectorCreatePage(WebDriverManager.GetDriver());
        }

        [AfterScenario]
        public static void CleanupAfterScenario()
        {
            WebDriverManager.CloseDriver();
        }

        [Given(@"que estoy en la página de creación de lectores")]
        public void DadoQueEstoyEnLaPaginaDeCreacionDeLectores()
        {
            _lectorCreatePage.NavigateToLectorCreatePage();
            Assert.That(_lectorCreatePage.IsOnCreatePage(), Is.True, 
                "No se pudo navegar a la página de creación de lectores");
        }

        [When(@"lleno el formulario de lector con los siguientes datos:")]
        public void CuandoLlenoElFormularioDeLectorConLosSiguientesDatos(Table table)
        {
            var data = new ExpandoObject() as IDictionary<string, object>;
            
            foreach (var row in table.Rows)
            {
                data[row["Campo"]] = row["Valor"];
            }
            
            _lectorCreatePage.FillLectorForm(
                ci: data.ContainsKey("CI") ? data["CI"]?.ToString() ?? "" : "",
                primerNombre: data.ContainsKey("PrimerNombre") ? data["PrimerNombre"]?.ToString() ?? "" : "",
                segundoNombre: data.ContainsKey("SegundoNombre") ? data["SegundoNombre"]?.ToString() ?? "" : "",
                primerApellido: data.ContainsKey("PrimerApellido") ? data["PrimerApellido"]?.ToString() ?? "" : "",
                segundoApellido: data.ContainsKey("SegundoApellido") ? data["SegundoApellido"]?.ToString() ?? "" : "",
                telefono: data.ContainsKey("Telefono") ? data["Telefono"]?.ToString() ?? "" : "",
                correo: data.ContainsKey("Correo") ? data["Correo"]?.ToString() ?? "" : ""
            );
        }

        [When(@"envío el formulario")]
        public void CuandoEnvioElFormulario()
        {
            _lectorCreatePage.SubmitForm();
            Thread.Sleep(400); 
        }

        [Then(@"debería ver el resultado ""(.*)""")]
        public void EntoncesDeberiaVerElResultado(string resultadoEsperado)
        {
            switch (resultadoEsperado)
            {
                case "Aceptado":
                    Assert.That(_lectorCreatePage.IsOnSuccessPage(), Is.True, 
                        "Se esperaba la creación exitosa del lector pero se encontraron errores de validación o aún está en la página de creación");
                    break;
                case "Rechazado":
                    Assert.That(_lectorCreatePage.HasValidationErrors(), Is.True, 
                        "Se esperaban errores de validación pero el lector fue creado exitosamente");
                    break;
                default:
                    Assert.Fail($"Resultado esperado desconocido: {resultadoEsperado}");
                    break;
            }
        }
    }
}
