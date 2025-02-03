using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web.Mvc;
using Microsoft.IdentityModel.Tokens;
using Proyecto2.DAL;
using Proyecto2.Models;

namespace Proyecto2.Controllers
{
    public class AuthController : Controller
    {
        private GestorProyecto db = new GestorProyecto();
        private readonly string _jwtKey = "ClaveSuperSecreta1234567890!@#$%^"; // 32 caracteres

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = AuthenticateUser(model.Email, model.Password);
                if (user != null)
                {
                    var token = GenerateToken(user);
                    // Autenticación exitosa, redirigir según el rol del usuario
                    if (user.Rol == "Administrador")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (user.Rol == "Miembro")
                    {
                        return RedirectToAction("Index", "User");
                    }
                }
                ModelState.AddModelError("", "Email o contraseña incorrectos.");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        private string GenerateToken(Usuario user)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                issuer: "tu_issuer",
                audience: "tu_audience",
                claims: new List<Claim>
                {
                        new Claim(ClaimTypes.Name, user.Nombre),
                        new Claim(ClaimTypes.Role, user.Rol)
                },
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signinCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return tokenString;
        }

        private Usuario AuthenticateUser(string email, string password)
        {
            var user = db.Usuarios.FirstOrDefault(u => u.Email == email);
            if (user != null && VerifyPassword(password, user.Password))
            {
                return user;
            }
            return null;
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // Implementa la lógica de verificación de contraseña aquí
            // Por ejemplo, si las contraseñas están encriptadas, desencripta y compara
            return enteredPassword == storedPassword; // Cambia esto según sea necesario
        }
    }
}
