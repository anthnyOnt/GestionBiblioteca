using Xunit;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GestionBibliotecaTestsIntegracion.Smoke
{
    public class ApplicationSmokeTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public ApplicationSmokeTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task SMK_001_Index_DeberiaResponder200()
        {
            using var client = _factory.CreateClient();
            var resp = await client.GetAsync("/");
            Xunit.Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }
    }
}
