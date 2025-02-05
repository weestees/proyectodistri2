using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Proyecto2.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
}
