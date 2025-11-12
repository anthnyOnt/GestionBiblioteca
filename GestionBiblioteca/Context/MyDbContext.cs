using System;
using System.Collections.Generic;
using GestionBiblioteca.Entities;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace GestionBiblioteca.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Autor> Autors { get; set; }

    public virtual DbSet<Categoria> Categoria { get; set; }

    public virtual DbSet<Editorial> Editorials { get; set; }

    public virtual DbSet<Ejemplar> Ejemplars { get; set; }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Libro> Libros { get; set; }

    public virtual DbSet<Prestamo> Prestamos { get; set; }

    public virtual DbSet<PrestamoEjemplar> PrestamoEjemplars { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Evitar sobreescribir la configuración establecida en Program.cs o en pruebas
        // Solo configurar si no está configurado aún (fallback de desarrollo)
        if (!optionsBuilder.IsConfigured)
        {
            // Fallback opcional (se puede quitar si no se desea hardcodear credenciales)
            var conn = Environment.GetEnvironmentVariable("MYSQL_TEST_CONN");
            if (!string.IsNullOrWhiteSpace(conn))
            {
                optionsBuilder.UseMySql(conn, ServerVersion.AutoDetect(conn));
            }
            // Si no hay variable, se asume que AddDbContext ya configuró usando appsettings.json.
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Autor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("autor");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apellido)
                .HasMaxLength(25)
                .HasColumnName("apellido");
            entity.Property(e => e.Nombre)
                .HasMaxLength(25)
                .HasColumnName("nombre");

            entity.HasMany(d => d.IdLibros).WithMany(p => p.IdAutores)
                .UsingEntity<Dictionary<string, object>>(
                    "AutorLibro",
                    r => r.HasOne<Libro>().WithMany()
                        .HasForeignKey("IdLibro")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("autor_libro_ibfk_2"),
                    l => l.HasOne<Autor>().WithMany()
                        .HasForeignKey("IdAutor")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("autor_libro_ibfk_1"),
                    j =>
                    {
                        j.HasKey("IdAutor", "IdLibro")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("autor_libro");
                        j.HasIndex(new[] { "IdLibro" }, "id_libro");
                        j.IndexerProperty<int>("IdAutor").HasColumnName("id_autor");
                        j.IndexerProperty<int>("IdLibro").HasColumnName("id_libro");
                    });
        });

        modelBuilder.Entity<Categoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("categoria");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");
            entity.Property(e => e.UltimaActualizacion)
                .HasColumnType("timestamp")
                .HasColumnName("ultima_actualizacion");
        });

        modelBuilder.Entity<Editorial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("editorial");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Correo)
                .HasMaxLength(30)
                .HasColumnName("correo");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.SitioWeb)
                .HasMaxLength(50)
                .HasColumnName("sitio_web");
            entity.Property(e => e.Telefono)
                .HasMaxLength(12)
                .HasColumnName("telefono");
        });

        modelBuilder.Entity<Ejemplar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("ejemplar");

            entity.HasIndex(e => e.IdLibro, "id_libro");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Descripcion).HasColumnName("descripcion");
            entity.Property(e => e.Observaciones).HasColumnName("observaciones");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.Disponible).HasColumnName("disponible");
            entity.Property(e => e.FechaAdquisicion)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_adquisicion");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdLibro).HasColumnName("id_libro");
            entity.Property(e => e.UltimaActualizacion)
                .HasColumnType("timestamp")
                .HasColumnName("ultima_actualizacion");

            entity.HasOne(d => d.IdLibroNavigation).WithMany(p => p.Ejemplares)
                .HasForeignKey(d => d.IdLibro)
                .HasConstraintName("ejemplar_ibfk_1");
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("estado");

            entity.HasIndex(e => e.IdEjemplar, "id_ejemplar");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaDescripcion)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_descripcion");
            entity.Property(e => e.IdEjemplar).HasColumnName("id_ejemplar");
            entity.Property(e => e.Observacion)
                .HasMaxLength(200)
                .HasColumnName("observacion");
            entity.Property(e => e.UltimaActualizacion)
                .HasColumnType("timestamp")
                .HasColumnName("ultima_actualizacion");

            entity.HasOne(d => d.IdEjemplarNavigation).WithMany(p => p.Estados)
                .HasForeignKey(d => d.IdEjemplar)
                .HasConstraintName("estado_ibfk_1");
        });

        modelBuilder.Entity<Libro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("libro");

            entity.HasIndex(e => e.IdEditorial, "id_editorial");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Edicion)
                .HasMaxLength(20)
                .HasColumnName("edicion");
            entity.Property(e => e.FechaPublicacion)
                .HasColumnType("datetime")
                .HasColumnName("fecha_publicacion");
            entity.Property(e => e.IdEditorial).HasColumnName("id_editorial");
            entity.Property(e => e.Idioma)
                .HasMaxLength(20)
                .HasColumnName("idioma");
            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .HasColumnName("isbn");
            entity.Property(e => e.Sinopsis)
                .HasMaxLength(200)
                .HasColumnName("sinopsis");
            entity.Property(e => e.Titulo)
                .HasMaxLength(50)
                .HasColumnName("titulo");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.UltimaActualizacion)
                .HasColumnType("timestamp")
                .HasColumnName("ultima_actualizacion");

            entity.HasOne(d => d.IdEditorialNavigation).WithMany(p => p.Libros)
                .HasForeignKey(d => d.IdEditorial)
                .HasConstraintName("libro_ibfk_1");

            entity.HasMany(d => d.IdCategoria).WithMany(p => p.IdLibros)
                .UsingEntity<Dictionary<string, object>>(
                    "CategoriaLibro",
                    r => r.HasOne<Categoria>().WithMany()
                        .HasForeignKey("IdCategoria")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("categoria_libro_ibfk_2"),
                    l => l.HasOne<Libro>().WithMany()
                        .HasForeignKey("IdLibro")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("categoria_libro_ibfk_1"),
                    j =>
                    {
                        j.HasKey("IdLibro", "IdCategoria")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("categoria_libro");
                        j.HasIndex(new[] { "IdCategoria" }, "id_categoria");
                        j.IndexerProperty<int>("IdLibro").HasColumnName("id_libro");
                        j.IndexerProperty<sbyte>("IdCategoria").HasColumnName("id_categoria");
                    });
        });

        modelBuilder.Entity<Prestamo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("prestamo");

            entity.HasIndex(e => e.IdUsuario, "id_usuario");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Cancelado).HasColumnName("cancelado");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.FechaCancelacion)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_cancelacion");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.FechaPrestamo)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_prestamo");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.UltimaActualizacion)
                .HasColumnType("timestamp")
                .HasColumnName("ultima_actualizacion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Prestamos)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("prestamo_ibfk_1");
        });

        modelBuilder.Entity<PrestamoEjemplar>(entity =>
        {
            entity.HasKey(e => new { e.IdPrestamo, e.IdEjemplar })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("prestamo_ejemplar");

            entity.HasIndex(e => e.IdEjemplar, "id_ejemplar");

            entity.Property(e => e.IdPrestamo).HasColumnName("id_prestamo");
            entity.Property(e => e.IdEjemplar).HasColumnName("id_ejemplar");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Devuelto).HasColumnName("devuelto");
            entity.Property(e => e.FechaDevolucion)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_devolucion");
            entity.Property(e => e.FechaLimite)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_limite");
            entity.Property(e => e.UltimaActualizacion)
                .HasColumnType("timestamp")
                .HasColumnName("ultima_actualizacion");

            entity.HasOne(d => d.IdEjemplarNavigation).WithMany(p => p.PrestamoEjemplares)
                .HasForeignKey(d => d.IdEjemplar)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prestamo_ejemplar_ibfk_2");

            entity.HasOne(d => d.IdPrestamoNavigation).WithMany(p => p.PrestamoEjemplares)
                .HasForeignKey(d => d.IdPrestamo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("prestamo_ejemplar_ibfk_1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("usuario");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Activo).HasColumnName("activo");
            entity.Property(e => e.Ci)
                .HasMaxLength(10)
                .HasColumnName("ci");
            entity.Property(e => e.Contrasenia)
                .HasMaxLength(255)
                .HasColumnName("contrasenia");
            entity.Property(e => e.Correo)
                .HasMaxLength(45)
                .HasColumnName("correo");
            entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
            entity.Property(e => e.FechaCreacion)
                .HasColumnType("timestamp")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.PrimerApellido)
                .HasMaxLength(30)
                .HasColumnName("primer_apellido");
            entity.Property(e => e.PrimerNombre)
                .HasMaxLength(30)
                .HasColumnName("primer_nombre");
            entity.Property(e => e.Rol)
                .HasMaxLength(10)
                .HasColumnName("rol");
            entity.Property(e => e.SegundoApellido)
                .HasMaxLength(30)
                .HasColumnName("segundo_apellido");
            entity.Property(e => e.SegundoNombre)
                .HasMaxLength(30)
                .HasColumnName("segundo_nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(10)
                .HasColumnName("telefono");
            entity.Property(e => e.UltimaActualizacion)
                .HasColumnType("timestamp")
                .HasColumnName("ultima_actualizacion");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
