using NUnit.Framework;
using Reqnroll;
using UITests.PageObjects;

namespace UITests.StepDefinitions.Libros
{
    [Binding]
    [Scope(Feature = "Operaciones en la página de índice de libros")]
    public class LibroIndexSteps
    {
        private readonly LibroIndexPage _libroIndexPage;
        private readonly LibroDeletePage _libroDeletePage;

        public LibroIndexSteps()
        {
            _libroIndexPage = new LibroIndexPage(WebDriverManager.GetDriver());
            _libroDeletePage = new LibroDeletePage(WebDriverManager.GetDriver());
        }

        [AfterScenario]
        public static void CleanupAfterScenario()
        {
            WebDriverManager.CloseDriver();
        }

        [Given(@"que estoy en la página de índice de libros")]
        public void DadoQueEstoyEnLaPaginaDeIndiceDeLibros()
        {
            _libroIndexPage.NavigateToLibroIndexPage();
            Assert.That(_libroIndexPage.IsOnIndexPage(), Is.True, 
                "No se pudo navegar a la página de índice de libros");
        }

        [When(@"selecciono todos los libros de la lista")]
        public void CuandoSeleccionoTodosLosLibrosDeLaLista()
        {
            // This step represents viewing/loading the list (GET request)
            // Verification happens in the Then step
        }

        [When(@"hago clic en eliminar para un libro")]
        public void CuandoHagoClicEnEliminarParaUnLibro()
        {
            _libroIndexPage.ClickDeleteForFirstLibro();
        }

        [When(@"confirmo la eliminación")]
        public void CuandoConfirmoLaEliminacion()
        {
            Assert.That(_libroDeletePage.IsOnDeletePage(), Is.True,
                "Debería estar en la página de confirmación de eliminación");
                
            _libroDeletePage.ConfirmDelete();
        }

        [Then(@"todos los libros deberían estar seleccionados")]
        public void EntoncesTodosLosLibrosDeberianEstarSeleccionados()
        {
            Assert.That(_libroIndexPage.HasLibros(), Is.True,
                "La página debe mostrar libros (GET request exitoso)");
            Assert.That(_libroIndexPage.GetLibroCount(), Is.GreaterThan(0),
                "Debe haber al menos un libro en la lista");
        }

        [Then(@"el libro debería ser eliminado exitosamente")]
        public void EntoncesElLibroDeberiaSerEliminadoExitosamente()
        {
            Assert.That(_libroIndexPage.IsOnIndexPage(), Is.True,
                "Debería regresar a la página de índice de libros después de eliminar");
        }

        [Then(@"debería redirigir a la página de índice")]
        public void EntoncesDeberiaRedirigirALaPaginaDeIndice()
        {
            Assert.That(_libroIndexPage.IsOnIndexPage(), Is.True,
                "Debería estar en la página de índice de libros");
        }
    }
}
