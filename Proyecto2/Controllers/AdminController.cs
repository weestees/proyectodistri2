using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Proyecto2.DAL;
using Proyecto2.Models;

namespace Proyecto2.Controllers
{
    public class AdminController : Controller
    {
        private GestorProyecto db = new GestorProyecto();

        public ActionResult Index()
        {
            var admin = db.Usuarios.FirstOrDefault(u => u.Rol == "Administrador");
            ViewBag.Usuarios = db.Usuarios.ToList();
            return View(admin);
        }

        // Redirigir a los métodos del UsuariosController
        public ActionResult Usuarios()
        {
            return RedirectToAction("Index", "Usuarios");
        }

        public ActionResult CrearUsuario()
        {
            return RedirectToAction("Crear", "Usuarios");
        }

        public ActionResult EditarUsuario(int id)
        {
            return RedirectToAction("Editar", "Usuarios", new { id });
        }

        [HttpPost]
        public ActionResult EliminarUsuario(int id)
        {
            var usuario = db.Usuarios.Find(id);
            if (usuario != null)
            {
                db.Usuarios.Remove(usuario);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Usuario no encontrado." });
        }

        // Crear tarea
        [HttpGet]
        public ActionResult CrearTarea()
        {
            ViewBag.Miembros = new SelectList(db.Usuarios.Where(u => u.Rol == "Miembro"), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public ActionResult CrearTarea(Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Tareas.Add(tarea);
                    db.SaveChanges();
                    return RedirectToAction("Usuarios");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Error al crear la tarea: " + ex.InnerException?.Message);
                }
            }
            ViewBag.Miembros = new SelectList(db.Usuarios.Where(u => u.Rol == "Miembro"), "Id", "Nombre");
            return View(tarea);
        }
    }
}
