using GestionBiblioteca.Context;
using GestionBiblioteca.Repository;
using GestionBiblioteca.Services.Autor;
using GestionBiblioteca.Services.Categoria;
using GestionBiblioteca.Services.Editorial;
using GestionBiblioteca.Services.Ejemplar;
using GestionBiblioteca.Services.Libro;
using GestionBiblioteca.Services.Prestamo;
using GestionBiblioteca.Services.Usuario;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register MyDbContext with DI
builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 43)) //  with your MySQL version
    )
);
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IEditorialService, EditorialService>();
builder.Services.AddScoped<IEjemplarService, EjemplarService>();
builder.Services.AddScoped<ILibroService, LibroService>();
builder.Services.AddScoped<IPrestamoService, PrestamoService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();


builder.Services.AddSession();

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

//Enable session
app.UseSession();


app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
    .WithStaticAssets();

app.Run();