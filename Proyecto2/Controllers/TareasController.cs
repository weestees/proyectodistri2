using Proyecto2.DAL;
using Proyecto2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Proyecto2.Controllers
{
    public class TareasController : Controller
    {
        private readonly GestorProyecto _context;

        public TareasController()
        {
            _context = new GestorProyecto();
        }

        // GET: Tareas
        public ActionResult Index()
        {
            var tareas = _context.Tareas.ToList();
            return View(tareas);
        }

        // GET: Tareas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var tarea = _context.Tareas
                .FirstOrDefault(m => m.Id == id);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            return View(tarea);
        }

        // GET: Tareas/Create
        public ActionResult Create()
        {
            var usuarios = _context.Usuarios
                     .Where(u => u.Rol == "Miembro") // Filtrar solo "Miembros"
                     .Select(u => new { u.Id, u.Nombre }) // Seleccionar solo Id y Nombre
                     .ToList();

            ViewBag.Usuarios = new SelectList(usuarios, "Id", "Nombre");
            return View();
        }

        // POST: Tareas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Titulo,Descripcion")] Tarea tarea, int[] usuarioIds)
        {
            if (ModelState.IsValid)
            {
                tarea.Estado = "Pendiente"; // Estado siempre en "Pendiente"
                _context.Tareas.Add(tarea);
                _context.SaveChanges();

                if (usuarioIds != null)
                {
                    foreach (var usuarioId in usuarioIds)
                    {
                        var usuarioTarea = new UsuarioTarea
                        {
                            UsuarioId = usuarioId,
                            TareaId = tarea.Id
                        };
                        _context.UsuarioTareas.Add(usuarioTarea);
                    }
                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(Index)); // Redirigir al índice de tareas
            }

            var usuarios = _context.Usuarios
                     .Where(u => u.Rol == "Miembro") // Filtrar solo "Miembros"
                     .Select(u => new { u.Id, u.Nombre }) // Seleccionar solo Id y Nombre
                     .ToList();

            ViewBag.Usuarios = new SelectList(usuarios, "Id", "Nombre");
            return View(tarea);
        }

        // POST: Tareas/CreateForAllMembers
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateForAllMembers()
        {
            var usuarios = _context.Usuarios
                         .Where(u => u.Rol == "Miembro")
                         .ToList();

            foreach (var usuario in usuarios)
            {
                var tarea = new Tarea
                {
                    Titulo = "Nueva Tarea",
                    Descripcion = "Descripción de la tarea",
                    Estado = "Pendiente"
                };
                _context.Tareas.Add(tarea);
                _context.SaveChanges();

                var usuarioTarea = new UsuarioTarea
                {
                    UsuarioId = usuario.Id,
                    TareaId = tarea.Id
                };
                _context.UsuarioTareas.Add(usuarioTarea);
            }

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // ...

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var tarea = _context.Tareas
                .Include("UsuarioTareas.Usuario") // Se usa un string con el nombre de la propiedad de navegación
                .FirstOrDefault(t => t.Id == id);


            if (tarea == null)
            {
                return HttpNotFound();
            }

            var usuarios = _context.Usuarios
                     .Where(u => u.Rol == "Miembro") // Filtrar solo "Miembros"
                     .Select(u => new { u.Id, u.Nombre }) // Seleccionar solo Id y Nombre
                     .ToList();

            ViewBag.Usuarios = new MultiSelectList(usuarios, "Id", "Nombre", tarea.UsuarioTareas.Select(ut => ut.UsuarioId).ToArray());
            return View(tarea);
        }

        // POST: Tareas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "Id,Titulo,Descripcion,Estado")] Tarea tarea, int[] usuarioIds)
        {
            if (id != tarea.Id)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(tarea).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();

                    var existingUsuarioTareas = _context.UsuarioTareas.Where(ut => ut.TareaId == tarea.Id).ToList();
                    foreach (var usuarioTarea in existingUsuarioTareas)
                    {
                        _context.UsuarioTareas.Remove(usuarioTarea);
                    }
                    _context.SaveChanges();

                    if (usuarioIds != null)
                    {
                        foreach (var usuarioId in usuarioIds)
                        {
                            var usuarioTarea = new UsuarioTarea
                            {
                                UsuarioId = usuarioId,
                                TareaId = tarea.Id
                            };
                            _context.UsuarioTareas.Add(usuarioTarea);
                        }
                        _context.SaveChanges();
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    if (!TareaExists(tarea.Id))
                    {
                        return HttpNotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            var usuarios = _context.Usuarios
                     .Where(u => u.Rol == "Miembro") // Filtrar solo "Miembros"
                     .Select(u => new { u.Id, u.Nombre }) // Seleccionar solo Id y Nombre
                     .ToList();

            ViewBag.Usuarios = new MultiSelectList(usuarios, "Id", "Nombre", usuarioIds);
            return View(tarea);
        }

        // GET: Tareas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var tarea = _context.Tareas
                .FirstOrDefault(m => m.Id == id);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            return View(tarea);
        }

        // POST: Tareas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var tarea = _context.Tareas.Find(id);
            _context.Tareas.Remove(tarea);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool TareaExists(int id)
        {
            return _context.Tareas.Any(e => e.Id == id);
        }
    }
}
