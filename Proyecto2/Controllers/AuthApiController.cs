using Proyecto2.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Web.Http;
using System.Linq;
using Proyecto2.DAL;
using System.Web.UI.WebControls;
using System.Web.Helpers;
using System.Net.Http;

namespace Proyecto2.Controllers
{
    [RoutePrefix("api/autenticacion")]
    public class AuthApiController : ApiController
    {
        private GestorProyecto db = new GestorProyecto();

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(LoginViewModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Faltan datos.");
            }

            try
            {
                var user = AuthenticateUser(model);

                if (user != null)
                {
                    var tokenString = GenerateJWT(user);
                    return Ok(new { success = true, role = user.Role, token = tokenString });
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error al intentar iniciar sesión.", ex));
            }
        }

        private User AuthenticateUser(LoginViewModel model)
        {
            try
            {
                var user = db.Usuarios.FirstOrDefault(u => u.Email.Equals(model.Email, StringComparison.OrdinalIgnoreCase));
                if (user != null && user.Password == model.Password)
                {
                    return new User { Email = user.Email, Password = user.Password, Role = user.Rol };
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar las credenciales.", ex);
            }
        }

        private string GenerateJWT(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ABCDEFGHIJ1234567890abcdefghijKLMNOP"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("role", user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "yourdomain.com",
                audience: "yourdomain.com",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet]
        [Route("redirect")]
        public IHttpActionResult RedirectUser(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl))
            {
                return BadRequest("ReturnUrl no puede estar vacío.");
            }

            return Redirect(returnUrl);
        }

        [HttpPost]
        [Route("logout")]
        public IHttpActionResult Logout()
        {
            // Aquí puedes implementar la lógica para invalidar el token si es necesario
            // Por ejemplo, puedes mantener una lista de tokens inválidos en la base de datos

            // Redirigir al usuario a la página de inicio de sesión
            var response = Request.CreateResponse(System.Net.HttpStatusCode.Redirect);
            response.Headers.Location = new Uri("/Auth/Login", UriKind.Relative);
            return ResponseMessage(response);
        }
    }

    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}