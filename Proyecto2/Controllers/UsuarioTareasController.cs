using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Proyecto2.DAL;
using Proyecto2.Models;

namespace Proyecto2.Controllers
{
    public class UsuarioTareasController : ApiController
    {
        private GestorProyecto db = new GestorProyecto();

        // GET: api/UsuarioTareas
        public IQueryable<UsuarioTarea> GetUsuarioTareas()
        {
            return db.UsuarioTareas;
        }

        // GET: api/UsuarioTareas/5
        [ResponseType(typeof(UsuarioTarea))]
        public IHttpActionResult GetUsuarioTarea(int id)
        {
            UsuarioTarea usuarioTarea = db.UsuarioTareas.Find(id);
            if (usuarioTarea == null)
            {
                return NotFound();
            }

            return Ok(usuarioTarea);
        }

        // PUT: api/UsuarioTareas/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutUsuarioTarea(int id, UsuarioTarea usuarioTarea)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != usuarioTarea.UsuarioId)
            {
                return BadRequest();
            }

            db.Entry(usuarioTarea).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioTareaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/UsuarioTareas
        [ResponseType(typeof(UsuarioTarea))]
        public IHttpActionResult PostUsuarioTarea(UsuarioTarea usuarioTarea)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.UsuarioTareas.Add(usuarioTarea);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (UsuarioTareaExists(usuarioTarea.UsuarioId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = usuarioTarea.UsuarioId }, usuarioTarea);
        }

        // DELETE: api/UsuarioTareas/5
        [ResponseType(typeof(UsuarioTarea))]
        public IHttpActionResult DeleteUsuarioTarea(int id)
        {
            UsuarioTarea usuarioTarea = db.UsuarioTareas.Find(id);
            if (usuarioTarea == null)
            {
                return NotFound();
            }

            db.UsuarioTareas.Remove(usuarioTarea);
            db.SaveChanges();

            return Ok(usuarioTarea);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UsuarioTareaExists(int id)
        {
            return db.UsuarioTareas.Count(e => e.UsuarioId == id) > 0;
        }
    }
}