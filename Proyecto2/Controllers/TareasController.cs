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
        public ActionResult Create([Bind(Include = "Id,Titulo,Descripcion,UsuarioId")] Tarea tarea)
        {
            if (ModelState.IsValid)
            {
                tarea.Estado = "Pendiente"; // Estado siempre en "Pendiente"
                _context.Tareas.Add(tarea);
                _context.SaveChanges();
                return RedirectToAction("Index", "Admin"); // Redirigir al índice del administrador
            }

            var usuarios = _context.Usuarios
                     .Where(u => u.Rol == "Miembro") // Filtrar solo "Miembros"
                     .Select(u => new { u.Id, u.Nombre }) // Seleccionar solo Id y Nombre
                     .ToList();

            ViewBag.Usuarios = new SelectList(usuarios, "Id", "Nombre");
            return View(tarea);
        }

        // GET: Tareas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var tarea = _context.Tareas.Find(id);
            if (tarea == null)
            {
                return HttpNotFound();
            }

            ViewBag.Usuarios = new SelectList(_context.Usuarios, "Id", "Nombre", tarea.UsuarioId);
            return View(tarea);
        }

        // POST: Tareas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, [Bind(Include = "Id,Titulo,Descripcion,Estado,UsuarioId")] Tarea tarea)
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

            ViewBag.Usuarios = new SelectList(_context.Usuarios, "Id", "Nombre", tarea.UsuarioId);
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
