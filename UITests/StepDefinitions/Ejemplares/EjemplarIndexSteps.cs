using NUnit.Framework;
using Reqnroll;
using UITests.PageObjects;

namespace UITests.StepDefinitions.Ejemplares
{
    [Binding]
    [Scope(Feature = "Operaciones en la página de índice de ejemplares")]
    public class EjemplarIndexSteps
    {
        private readonly EjemplarIndexPage _ejemplarIndexPage;
        private readonly EjemplarDeletePage _ejemplarDeletePage;
        private readonly ScenarioContext _scenarioContext;

        public EjemplarIndexSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _ejemplarIndexPage = new EjemplarIndexPage(WebDriverManager.GetDriver());
            _ejemplarDeletePage = new EjemplarDeletePage(WebDriverManager.GetDriver());
        }

        [AfterScenario]
        public static void CleanupAfterScenario()
        {
            WebDriverManager.CloseDriver();
        }

        [Given(@"que estoy en la página de índice de ejemplares")]
        public void DadoQueEstoyEnLaPaginaDeIndiceDeEjemplares()
        {
            _ejemplarIndexPage.NavigateToEjemplarIndexPage();
            Assert.That(_ejemplarIndexPage.IsOnIndexPage(), Is.True, 
                "No se pudo navegar a la página de índice de ejemplares");
        }

        [When(@"selecciono todos los ejemplares de la lista")]
        public void CuandoSeleccionoTodosLosEjemplaresDeLaLista()
        {
            // Store the original count for verification
            _scenarioContext["OriginalEjemplarCount"] = _ejemplarIndexPage.GetEjemplarCount();
            
            _ejemplarIndexPage.SelectAllEjemplares();
        }

        [When(@"hago clic en eliminar para un ejemplar")]
        public void CuandoHagoClicEnEliminarParaUnEjemplar()
        {
            // Store the original count for verification
            _scenarioContext["OriginalEjemplarCount"] = _ejemplarIndexPage.GetEjemplarCount();
            _scenarioContext["EjemplarToDeleteName"] = _ejemplarIndexPage.GetFirstEjemplarName();
            
            _ejemplarIndexPage.ClickDeleteForFirstEjemplar();
        }

        [When(@"confirmo la eliminación")]
        public void CuandoConfirmoLaEliminacion()
        {
            Assert.That(_ejemplarDeletePage.IsOnDeletePage(), Is.True,
                "Debería estar en la página de confirmación de eliminación");
                
            _ejemplarDeletePage.ConfirmDelete();
        }

        [Then(@"todos los ejemplares deberían estar seleccionados")]
        public void EntoncesTodosLosEjemplaresDeberianEstarSeleccionados()
        {
            Assert.That(_ejemplarIndexPage.AreAllEjemplaresSelected(), Is.True,
                "Todos los ejemplares deberían estar seleccionados");
                
            // Verify we're still on the index page
            Assert.That(_ejemplarIndexPage.IsOnIndexPage(), Is.True,
                "Debería permanecer en la página de índice de ejemplares");
        }

        [Then(@"el ejemplar debería ser eliminado exitosamente")]
        public void EntoncesElEjemplarDeberiaSerEliminadoExitosamente()
        {
            // Verify we're back on the index page
            Assert.That(_ejemplarIndexPage.IsOnIndexPage(), Is.True,
                "Debería regresar a la página de índice de ejemplares después de eliminar");
        }

        [Then(@"debería redirigir a la página de índice")]
        public void EntoncesDeberiaRedirigirALaPaginaDeIndice()
        {
            // Verify we're on the index page
            Assert.That(_ejemplarIndexPage.IsOnIndexPage(), Is.True,
                "Debería estar en la página de índice de ejemplares");
                
            // Additional verification that we're at the base index URL
            var currentUrl = WebDriverManager.GetDriver().Url;
            Assert.That(currentUrl, Does.EndWith("/Ejemplar") | Does.EndWith("/Ejemplar/"),
                "Debería estar en la página principal del índice de ejemplares");
        }
    }
}
