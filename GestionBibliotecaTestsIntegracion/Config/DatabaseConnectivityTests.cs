using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using GestionBiblioteca.Context;
using GestionBibliotecaTestsIntegracion.Config;

namespace GestionBibliotecaTestsIntegracion.Config
{
    public class DatabaseConnectivityTests
    {
        private MyDbContext CreateRealContext()
        {
            // Usa TestDb para resolver desde MYSQL_TEST_CONN o appsettings.json y autodetectar versión
            return TestDb.CreateMySqlContext();
        }

        [Fact]
        public async Task CFG_004_PuedeConectarABD()
        {
            using var ctx = CreateRealContext();
            Assert.True(await ctx.Database.CanConnectAsync());
        }

        [Theory]
        [InlineData("libro")]
        [InlineData("ejemplar")]
        [InlineData("usuario")]
        public async Task CFG_005_TablasExisten(string table)
        {
            using var ctx = CreateRealContext();
            var cn = ctx.Database.GetDbConnection();
            await cn.OpenAsync();

            using var cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @t";
            var p = cmd.CreateParameter();
            p.ParameterName = "@t";
            p.Value = table;
            cmd.Parameters.Add(p);

            var exists = Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
            Assert.True(exists, $"La tabla {table} no existe.");
        }
    }
}
