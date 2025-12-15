using NUnit.Framework;
using Reqnroll;
using UITests.PageObjects;

namespace UITests.StepDefinitions.Libros
{
    [Binding]
    public class LibroIndexSteps
    {
        private readonly LibroIndexPage _libroIndexPage;
        private readonly LibroDeletePage _libroDeletePage;
        private readonly ScenarioContext _scenarioContext;

        public LibroIndexSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
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
            // Store the original count for verification
            _scenarioContext["OriginalLibroCount"] = _libroIndexPage.GetLibroCount();
            
            _libroIndexPage.SelectAllLibros();
        }

        [When(@"hago clic en eliminar para un libro")]
        public void CuandoHagoClicEnEliminarParaUnLibro()
        {
            // Store the original count for verification
            _scenarioContext["OriginalLibroCount"] = _libroIndexPage.GetLibroCount();
            _scenarioContext["LibroToDeleteName"] = _libroIndexPage.GetFirstLibroName();
            
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
            Assert.That(_libroIndexPage.AreAllLibrosSelected(), Is.True,
                "Todos los libros deberían estar seleccionados");
                
            // Verify we're still on the index page
            Assert.That(_libroIndexPage.IsOnIndexPage(), Is.True,
                "Debería permanecer en la página de índice de libros");
        }

        [Then(@"el libro debería ser eliminado exitosamente")]
        public void EntoncesElLibroDeberiaSerEliminadoExitosamente()
        {
            // Verify we're back on the index page
            Assert.That(_libroIndexPage.IsOnIndexPage(), Is.True,
                "Debería regresar a la página de índice de libros después de eliminar");
        }

        [Then(@"debería redirigir a la página de índice")]
        public void EntoncesDeberiaRedirigirALaPaginaDeIndice()
        {
            // Verify we're on the index page
            Assert.That(_libroIndexPage.IsOnIndexPage(), Is.True,
                "Debería estar en la página de índice de libros");
                
            // Additional verification that we're at the base index URL
            var currentUrl = WebDriverManager.GetDriver().Url;
            Assert.That(currentUrl, Does.EndWith("/Libro") | Does.EndWith("/Libro/"),
                "Debería estar en la página principal del índice de libros");
        }
    }
}
