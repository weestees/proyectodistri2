using System.Web.Mvc;
using Proyecto2.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Web;

namespace Proyecto2.Controllers
{
    [RoutePrefix("api/autenticacion")]
    public class AuthController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("login")]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = AuthenticateUser(model);

                if (user != null)
                {
                    var tokenString = GenerateJWT(user);
                    HttpContext.Response.Cookies.Add(new HttpCookie("AuthToken", tokenString));

                    if (user.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else if (user.Role == "Miembro")
                    {
                        return RedirectToAction("Index", "Miembro");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Nombre de usuario o contraseña incorrectos.");
                }
            }

            return View(model);
        }

        private User AuthenticateUser(LoginViewModel model)
        {
            if (model.Email == "admin@example.com" && model.Password == "password")
            {
                return new User { Email = model.Email, Role = "Admin" };
            }
            else if (model.Email == "miembro@example.com" && model.Password == "password")
            {
                return new User { Email = model.Email, Role = "Miembro" };
            }

            return null;
        }

        private string GenerateJWT(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourSecretKeyHere"));
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
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class User
    {
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
