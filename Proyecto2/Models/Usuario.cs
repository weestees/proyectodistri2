﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proyecto2.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
    }
}
