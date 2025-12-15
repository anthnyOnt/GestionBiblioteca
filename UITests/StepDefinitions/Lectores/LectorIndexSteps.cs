using NUnit.Framework;
using Reqnroll;
using UITests.PageObjects;

namespace UITests.StepDefinitions.Lectores
{
    [Binding]
    [Scope(Feature = "Operaciones en la página de índice de lectores")]
    public class LectorIndexSteps
    {
        private readonly LectorIndexPage _lectorIndexPage;
        private readonly LectorDeletePage _lectorDeletePage;
        private readonly ScenarioContext _scenarioContext;

        public LectorIndexSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _lectorIndexPage = new LectorIndexPage(WebDriverManager.GetDriver());
            _lectorDeletePage = new LectorDeletePage(WebDriverManager.GetDriver());
        }

        [AfterScenario]
        public static void CleanupAfterScenario()
        {
            WebDriverManager.CloseDriver();
        }

        [Given(@"que estoy en la página de índice de lectores")]
        public void DadoQueEstoyEnLaPaginaDeIndiceDeLectores()
        {
            _lectorIndexPage.NavigateToLectorIndexPage();
            Assert.That(_lectorIndexPage.IsOnIndexPage(), Is.True, 
                "No se pudo navegar a la página de índice de lectores");
        }

        [When(@"selecciono todos los lectores de la lista")]
        public void CuandoSeleccionoTodosLosLectoresDeLaLista()
        {
            // Store the original count for verification
            _scenarioContext["OriginalLectorCount"] = _lectorIndexPage.GetLectorCount();
            
            _lectorIndexPage.SelectAllLectores();
        }

        [When(@"hago clic en eliminar para un lector")]
        public void CuandoHagoClicEnEliminarParaUnLector()
        {
            // Store the original count for verification
            _scenarioContext["OriginalLectorCount"] = _lectorIndexPage.GetLectorCount();
            _scenarioContext["LectorToDeleteName"] = _lectorIndexPage.GetFirstLectorName();
            
            _lectorIndexPage.ClickDeleteForFirstLector();
        }

        [When(@"confirmo la eliminación")]
        public void CuandoConfirmoLaEliminacion()
        {
            Assert.That(_lectorDeletePage.IsOnDeletePage(), Is.True,
                "Debería estar en la página de confirmación de eliminación");
                
            _lectorDeletePage.ConfirmDelete();
        }

        [Then(@"todos los lectores deberían estar seleccionados")]
        public void EntoncesTodosLosLectoresDeberianEstarSeleccionados()
        {
            Assert.That(_lectorIndexPage.AreAllLectoresSelected(), Is.True,
                "Todos los lectores deberían estar seleccionados");
                
            // Verify we're still on the index page
            Assert.That(_lectorIndexPage.IsOnIndexPage(), Is.True,
                "Debería permanecer en la página de índice de lectores");
        }

        [Then(@"el lector debería ser eliminado exitosamente")]
        public void EntoncesElLectorDeberiaSerEliminadoExitosamente()
        {
            // Verify we're back on the index page
            Assert.That(_lectorIndexPage.IsOnIndexPage(), Is.True,
                "Debería regresar a la página de índice de lectores después de eliminar");
        }

        [Then(@"debería redirigir a la página de índice")]
        public void EntoncesDeberiaRedirigirALaPaginaDeIndice()
        {
            // Verify we're on the index page
            Assert.That(_lectorIndexPage.IsOnIndexPage(), Is.True,
                "Debería estar en la página de índice de lectores");
                
            // Additional verification that we're at the base index URL
            var currentUrl = WebDriverManager.GetDriver().Url;
            Assert.That(currentUrl, Does.EndWith("/Usuario") | Does.EndWith("/Usuario/"),
                "Debería estar en la página principal del índice de lectores");
        }
    }
}
