using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto2.Models
{
    public class UsuarioTarea
    {
        [Key, Column(Order = 0)]
        public int UsuarioId { get; set; }

        [Key, Column(Order = 1)]
        public int TareaId { get; set; }

        public virtual Usuario Usuario { get; set; }
        public virtual Tarea Tarea { get; set; }
    }
}
