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

        public LectorIndexSteps()
        {
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
            // This step represents viewing/loading the list (GET request)
            // Verification happens in the Then step
        }

        [When(@"hago clic en eliminar para un lector")]
        public void CuandoHagoClicEnEliminarParaUnLector()
        {
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
            Assert.That(_lectorIndexPage.HasLectores(), Is.True,
                "La página debe mostrar lectores (GET request exitoso)");
            Assert.That(_lectorIndexPage.GetLectorCount(), Is.GreaterThan(0),
                "Debe haber al menos un lector en la lista");
        }

        [Then(@"el lector debería ser eliminado exitosamente")]
        public void EntoncesElLectorDeberiaSerEliminadoExitosamente()
        {
            Assert.That(_lectorIndexPage.IsOnIndexPage(), Is.True,
                "Debería regresar a la página de índice de lectores después de eliminar");
        }

        [Then(@"debería redirigir a la página de índice")]
        public void EntoncesDeberiaRedirigirALaPaginaDeIndice()
        {
            Assert.That(_lectorIndexPage.IsOnIndexPage(), Is.True,
                "Debería estar en la página de índice de lectores");
        }
    }
}
