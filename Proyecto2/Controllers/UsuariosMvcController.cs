using Microsoft.AspNetCore.Mvc;
using Proyecto2.DAL;
using Proyecto2.Models;
using System.Linq;
using System.Web.Mvc;
using System;

namespace Proyecto2.Controllers
{
    [Route("UsuariosMvc/[action]")]
    public class UsuariosMvcController : Controller
    {
        private readonly GestorProyecto _context;

        public UsuariosMvcController()
        {
            _context = new GestorProyecto();
        }

        [HttpGet]
        public ActionResult Index()
        {
            var usuarios = _context.Usuarios.ToList();
            return View(new AdminViewModel { Usuarios = usuarios });
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View("Editar", usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var usuarioExistente = _context.Usuarios.Find(usuario.Id);
                    if (usuarioExistente != null)
                    {
                        usuarioExistente.Nombre = usuario.Nombre;
                        usuarioExistente.Email = usuario.Email;
                        usuarioExistente.Rol = usuario.Rol;

                        if (!string.IsNullOrEmpty(usuario.Password))
                        {
                            usuarioExistente.Password = usuario.Password;
                        }

                        _context.Entry(usuarioExistente).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                        ViewBag.Message = "Usuario actualizado exitosamente.";
                        return RedirectToAction("Index", "Admin");
                    }
                    ViewBag.Error = "Usuario no encontrado.";
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al actualizar el usuario: " + ex.Message;
                }
            }
            return View("Editar", usuario);
        }

        [HttpGet]
        public ActionResult Crear()
        {
            return View("Crear");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Usuarios.Add(usuario);
                    _context.SaveChanges();
                    ViewBag.Message = "Usuario creado exitosamente.";
                    return RedirectToAction("Index", "Admin");
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Error al crear el usuario: " + ex.Message;
                }
            }
            return View("Crear", usuario);
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View("Eliminar", usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var usuario = _context.Usuarios.Find(id);
                if (usuario != null)
                {
                    _context.Usuarios.Remove(usuario);
                    _context.SaveChanges();
                    ViewBag.Message = "Usuario eliminado exitosamente.";
                }
                else
                {
                    ViewBag.Error = "Usuario no encontrado.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error al eliminar el usuario: " + ex.Message;
            }
            return RedirectToAction("Index", "Admin");
        }
    }
}
