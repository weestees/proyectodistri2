using System.Web.Mvc;
using Proyecto2.DAL;
using Proyecto2.Models;
using System.Linq;

namespace Proyecto2.Controllers
{
    [Authorize(Roles = "Miembro")]
    public class MiembroController : Controller
    {
        private GestorProyecto db = new GestorProyecto();

        public ActionResult Index()
        {
            var usuarioEmail = User.Identity.Name; // Se espera que contenga "maria.lopez@gmail.com"
            var usuario = db.Usuarios.FirstOrDefault(u => u.Email == usuarioEmail);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            var tareasAsignadas = db.Usuarios_Tareas
                .Where(ut => ut.UsuarioId == usuario.Id)
                .Select(ut => ut.Tarea)
                .ToList();

            var model = new MiembroViewModel
            {
                Tareas = tareasAsignadas
            };

            return View(model);
        }
    }
}
