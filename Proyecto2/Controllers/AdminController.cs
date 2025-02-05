using Proyecto2.Models;
using System.Web.Http;
using System.Web.Mvc;
using Proyecto2.DAL;
using System.Linq;

namespace Proyecto2.Controllers
{
    [System.Web.Http.Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private GestorProyecto db = new GestorProyecto();

        // GET: Admin/Index
        public ActionResult Index()
        {
            var adminEmail = User.Identity.Name;
            var admin = db.Usuarios.FirstOrDefault(u => u.Email == adminEmail);

            var model = new AdminViewModel
            {
                Nombre = admin != null ? admin.Nombre : "Administrador", // Obtener el nombre del administrador
                Usuarios = db.Usuarios.ToList() // Obtener todos los usuarios de la base de datos
            };

            return View(model);
        }
    }
}
