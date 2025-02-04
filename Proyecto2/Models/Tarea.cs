using Proyecto2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto2.Models
{
    public class Tarea
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; }

        public string Descripcion { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } // "Pendiente", "En Progreso", "Completada"

        [Required]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relación muchos a muchos con Usuario a través de UsuarioTarea
        public virtual ICollection<UsuarioTarea> UsuarioTareas { get; set; }
    }
}
