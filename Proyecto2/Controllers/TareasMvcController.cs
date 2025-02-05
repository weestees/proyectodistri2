using System;
using System.Linq;
using System.Web.Mvc;
using Proyecto2.DAL;
using Proyecto2.Models;
using System.Data.Entity;

namespace Proyecto2.Controllers
{
    public class TareasMvcController : Controller
    {
        private readonly GestorProyecto _context;

        public TareasMvcController()
        {
            _context = new GestorProyecto();
        }

        [HttpGet]
        public ActionResult Index()
        {
            var tareas = _context.Tareas.Include(t => t.UsuarioTareas).ToList();
            return View(tareas);
        }

        [HttpGet]
        public ActionResult Crear()
        {
            var tarea = new Tarea
            {
                FechaCreacion = DateTime.Now,
                Estado = "Pendiente"
            };
            return View(tarea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Tareas.Add(tarea);
                    _context.SaveChanges();

                    TempData["Message"] = "Tarea creada exitosamente.";
                    return RedirectToAction("Asignar", new { id = tarea.Id });
                }
                catch (Exception ex)
                {
                    ViewBag.Error = $"Error al crear la tarea: {ex.Message} {(ex.InnerException?.Message ?? "")}";
                    TempData["Error"] = ViewBag.Error;
                }
            }

            return View(tarea);
        }

        [HttpGet]
        public ActionResult Asignar(int id)
        {
            var tarea = _context.Tareas.Find(id);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            ViewBag.Usuarios = new MultiSelectList(_context.Usuarios.Where(u => u.Rol == "Miembro").ToList(), "Id", "Nombre");
            return View(tarea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Asignar(int id, bool asignarATodos, int[] usuarioIds)
        {
            var tarea = _context.Tareas.Find(id);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            try
            {
                if (asignarATodos)
                {
                    var usuarios = _context.Usuarios.Where(u => u.Rol == "Miembro").ToList();
                    foreach (var usuario in usuarios)
                    {
                        _context.Usuarios_Tareas.Add(new UsuarioTarea { TareaId = tarea.Id, UsuarioId = usuario.Id });
                    }
                }
                else if (usuarioIds != null)
                {
                    foreach (var usuarioId in usuarioIds)
                    {
                        _context.Usuarios_Tareas.Add(new UsuarioTarea { TareaId = tarea.Id, UsuarioId = usuarioId });
                    }
                }

                _context.SaveChanges();
                TempData["Message"] = "Tarea asignada exitosamente.";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error al asignar la tarea: {ex.Message} {(ex.InnerException?.Message ?? "")}";
                TempData["Error"] = ViewBag.Error;
            }

            ViewBag.Usuarios = new MultiSelectList(_context.Usuarios.Where(u => u.Rol == "Miembro").ToList(), "Id", "Nombre");
            return View(tarea);
        }

        [HttpGet]
        public ActionResult EditarEstado(int id)
        {
            var tarea = _context.Tareas.Find(id);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            return View(tarea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEstado(int id, string estado)
        {
            var tarea = _context.Tareas.Find(id);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            tarea.Estado = estado == "on" ? "Completado" : "Pendiente";
            _context.SaveChanges();

            return RedirectToAction("Index", "Miembro");
        }
    }
}
