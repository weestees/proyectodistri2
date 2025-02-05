using Proyecto2.Models;
using System.Linq;
using System.Web.Http;
using Proyecto2.DAL;
using System.Data.Entity;
using System;
using Newtonsoft.Json;

namespace Proyecto2.Controllers
{
    [RoutePrefix("api/tareas")]
    public class TareasApiController : ApiController
    {
        private GestorProyecto db = new GestorProyecto();

        public TareasApiController()
        {
            // Deshabilitar la carga diferida
            db.Configuration.LazyLoadingEnabled = false;
        }

        // Endpoint para obtener todas las tareas
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetTareas()
        {
            var tareas = db.Tareas.Include(t => t.UsuarioTareas).ToList();
            var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var json = JsonConvert.SerializeObject(tareas, settings);
            return Ok(json);
        }

        // Endpoint para obtener una tarea por id
        [HttpGet]
        [Route("{id:int?}")]
        public IHttpActionResult GetTarea(int? id)
        {
            if (id == null)
            {
                return BadRequest("El id no puede estar vacío.");
            }

            var tarea = db.Tareas.Include(t => t.UsuarioTareas).FirstOrDefault(t => t.Id == id);
            if (tarea == null)
            {
                return NotFound();
            }

            var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var json = JsonConvert.SerializeObject(tarea, settings);
            return Ok(json);
        }

        // Endpoint para filtrar tareas por estado
        [HttpGet]
        [Route("filter")]
        public IHttpActionResult FiltrarTareasPorEstado(string estado)
        {
            if (string.IsNullOrEmpty(estado))
            {
                return BadRequest("El estado no puede estar vacío.");
            }

            var tareas = db.Tareas.Include(t => t.UsuarioTareas)
                                  .Where(t => t.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase))
                                  .ToList();
            var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var json = JsonConvert.SerializeObject(tareas, settings);
            return Ok(json);
        }

        // Endpoint para cambiar el estado de una tarea
        [HttpPost]
        [Route("cambiarEstado")]
        public IHttpActionResult CambiarEstadoTarea(int id, string nuevoEstado)
        {
            if (string.IsNullOrEmpty(nuevoEstado))
            {
                return BadRequest("El nuevo estado no puede estar vacío.");
            }

            var tarea = db.Tareas.Include(t => t.UsuarioTareas).FirstOrDefault(t => t.Id == id);
            if (tarea == null)
            {
                return NotFound();
            }

            tarea.Estado = nuevoEstado;
            db.SaveChanges();

            return Ok(new { success = true, message = "Estado de la tarea actualizado correctamente." });
        }
    }
}
