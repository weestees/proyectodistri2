using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using Proyecto2.DAL;
using Proyecto2.Models;

namespace Proyecto2.Controllers
{
    [RoutePrefix("api/autenticacion")]
    public class AuthApiController : ApiController
    {
        private GestorProyecto db = new GestorProyecto();
        private readonly string _jwtKey = "ClaveSuperSecreta1234567890!@#$%^"; // 32 caracteres

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login([FromBody] LoginViewModel model)
        {
            if (model == null)
                return BadRequest("Invalid client request: model is null");

            var user = db.Usuarios.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || !VerifyPassword(model.Password, user.Password)) // Verifica la contraseña correctamente
                return Unauthorized();

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
            return Ok(new { Token = tokenString });
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // Implementa la lógica de verificación de contraseña aquí
            using (var sha256 = SHA256.Create())
            {
                var hashedPassword = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword)));
                return hashedPassword == storedPassword;
            }
        }
    }
}
