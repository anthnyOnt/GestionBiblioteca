using System.Threading;
using System.Dynamic;
using System.Collections.Generic;
using NUnit.Framework;
using Reqnroll;
using UITests.PageObjects;

namespace UITests.StepDefinitions.Libros
{
    [Binding]
    public class LibroCreationSteps
    {
        private readonly LibroCreatePage _libroCreatePage;
        private readonly ScenarioContext _scenarioContext;

        public LibroCreationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _libroCreatePage = new LibroCreatePage(WebDriverManager.GetDriver());
        }

        [AfterScenario]
        public static void CleanupAfterScenario()
        {
            WebDriverManager.CloseDriver();
        }

        [Given(@"que estoy en la página de creación de libros")]
        public void DadoQueEstoyEnLaPaginaDeCreacionDeLibros()
        {
            _libroCreatePage.NavigateToLibroCreatePage();
            Assert.That(_libroCreatePage.IsOnCreatePage(), Is.True, 
                "No se pudo navegar a la página de creación de libros");
        }

        [When(@"lleno el formulario de libro con los siguientes datos:")]
        public void CuandoLlenoElFormularioDeLibroConLosSiguientesDatos(Table table)
        {
            var data = new ExpandoObject() as IDictionary<string, object>;
            
            foreach (var row in table.Rows)
            {
                data[row["Campo"]] = row["Valor"];
            }
            
            _libroCreatePage.FillLibroForm(
                titulo: data.ContainsKey("Titulo") ? data["Titulo"]?.ToString() ?? "" : "",
                isbn: data.ContainsKey("ISBN") ? data["ISBN"]?.ToString() ?? "" : "",
                sinopsis: data.ContainsKey("Sinopsis") ? data["Sinopsis"]?.ToString() ?? "" : "",
                fechaPub: data.ContainsKey("FechaPub") ? data["FechaPub"]?.ToString() ?? "" : "",
                idioma: data.ContainsKey("Idioma") ? data["Idioma"]?.ToString() ?? "" : "",
                edicion: data.ContainsKey("Edicion") ? data["Edicion"]?.ToString() ?? "" : ""
            );
        }

        [When(@"envío el formulario de libro")]
        public void CuandoEnvioElFormularioDeLibro()
        {
            _libroCreatePage.SubmitForm();
            Thread.Sleep(1000); // Wait for form processing
        }

        [Then(@"debería ver el resultado del libro ""(.*)""")]
        public void EntoncesDeberiaVerElResultadoDelLibro(string resultadoEsperado)
        {
            switch (resultadoEsperado)
            {
                case "Aceptado":
                    Assert.That(_libroCreatePage.IsOnSuccessPage(), Is.True, 
                        "Se esperaba la creación exitosa del libro pero se encontraron errores de validación o aún está en la página de creación");
                    break;
                case "Rechazado":
                    Assert.That(_libroCreatePage.HasValidationErrors(), Is.True, 
                        "Se esperaban errores de validación pero el libro fue creado exitosamente");
                    break;
                default:
                    Assert.Fail($"Resultado esperado desconocido: {resultadoEsperado}");
                    break;
            }
        }
    }
}
