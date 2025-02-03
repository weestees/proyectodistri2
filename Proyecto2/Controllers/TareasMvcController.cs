using System.Linq;
using System.Web.Mvc;
using Proyecto2.DAL;
using Proyecto2.Models;

namespace Proyecto2.Controllers
{
    public class TareasMvcController : Controller
    {
        private GestorProyecto db = new GestorProyecto();

        // Mostrar lista de tareas
        [HttpGet]
        public ActionResult Index()
        {
            var tareas = db.Tareas.ToList();
            return View(tareas);
        }

        // Editar estado de una tarea
        [HttpGet]
        public ActionResult EditarEstado(int id)
        {
            var tarea = db.Tareas.Find(id);
            if (tarea == null)
            {
                return HttpNotFound();
            }
            return View(tarea);
        }

        [HttpPost]
        public ActionResult EditarEstado(Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tarea).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tarea);
        }
    }
}
