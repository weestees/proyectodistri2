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
    }

}