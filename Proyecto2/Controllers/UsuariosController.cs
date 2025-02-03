using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Proyecto2.DAL;
using Proyecto2.Models;
using System.Data.Entity.Infrastructure;

namespace Proyecto2.Controllers
{
    public class UsuariosController : Controller
    {
        private GestorProyecto db = new GestorProyecto();

        // Mostrar lista de usuarios
        public ActionResult Index()
        {
            var usuarios = db.Usuarios.ToList();
            return View(usuarios);
        }

        // Crear nuevo usuario
        [HttpGet]
        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Crear(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Usuarios.Add(usuario);
                    db.SaveChanges();
                    return Json(new { success = true });
                }
                catch (DbUpdateException ex)
                {
                    return Json(new { success = false, message = "Error al crear el usuario: " + ex.InnerException?.Message });
                }
            }
            return Json(new { success = false, message = "Error al crear el usuario. Verifique los datos ingresados." });
        }

        // Editar usuario
        [HttpGet]
        public ActionResult Editar(int id)
        {
            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Id,Nombre,Email,Rol,Password")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var usuarioExistente = db.Usuarios.AsNoTracking().FirstOrDefault(u => u.Id == usuario.Id);
                if (usuarioExistente != null)
                {
                    // Si la contraseña no ha sido cambiada, mantener la contraseña actual
                    if (string.IsNullOrEmpty(usuario.Password))
                    {
                        usuario.Password = usuarioExistente.Password;
                    }

                    // Asegurarse de que el campo Password no sea nulo
                    if (usuario.Password == null)
                    {
                        ModelState.AddModelError("", "El campo Password no puede ser nulo.");
                        return View(usuario);
                    }

                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View(usuario);
        }

        // Eliminar usuario
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var usuario = db.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var usuario = db.Usuarios.Find(id);
                db.Usuarios.Remove(usuario);
                db.SaveChanges();
                return RedirectToAction("Index", "Admin");
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error al eliminar el usuario: " + ex.InnerException?.Message);
                var usuario = db.Usuarios.Find(id);
                return View(usuario);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
