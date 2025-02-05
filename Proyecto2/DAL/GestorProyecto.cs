using Proyecto2.Models;
using System.Data.Entity;

namespace Proyecto2.DAL
{
    public class GestorProyecto : DbContext
    {
        public GestorProyecto() : base("name=GestorProyecto")
        {
        }

        public DbSet<Tarea> Tareas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<UsuarioTarea> Usuarios_Tareas { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapear la entidad UsuarioTarea a la tabla "Usuarios_Tareas"
            modelBuilder.Entity<UsuarioTarea>().ToTable("Usuarios_Tareas");
        }
    }
}
