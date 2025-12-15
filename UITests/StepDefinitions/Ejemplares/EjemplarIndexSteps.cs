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

        public EjemplarIndexSteps()
        {
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
            // This step represents viewing/loading the list (GET request)
            // Verification happens in the Then step
        }

        [When(@"hago clic en eliminar para un ejemplar")]
        public void CuandoHagoClicEnEliminarParaUnEjemplar()
        {
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
            Assert.That(_ejemplarIndexPage.HasEjemplares(), Is.True,
                "La página debe mostrar ejemplares (GET request exitoso)");
            Assert.That(_ejemplarIndexPage.GetEjemplarCount(), Is.GreaterThan(0),
                "Debe haber al menos un ejemplar en la lista");
        }

        [Then(@"el ejemplar debería ser eliminado exitosamente")]
        public void EntoncesElEjemplarDeberiaSerEliminadoExitosamente()
        {
            Assert.That(_ejemplarIndexPage.IsOnIndexPage(), Is.True,
                "Debería regresar a la página de índice de ejemplares después de eliminar");
        }

        [Then(@"debería redirigir a la página de índice")]
        public void EntoncesDeberiaRedirigirALaPaginaDeIndice()
        {
            Assert.That(_ejemplarIndexPage.IsOnIndexPage(), Is.True,
                "Debería estar en la página de índice de ejemplares");
        }
    }
}
