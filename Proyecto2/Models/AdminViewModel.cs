using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto2.Models
{
    public class AdminViewModel
    {
        public string Nombre { get; set; }
        public List<Usuario> Usuarios { get; set; }
    }


}