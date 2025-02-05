using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;
using Proyecto2.Models;

namespace Proyecto2.Controllers
{
    public class AuthController : Controller
    {
        // Ajusta la URL de la API (el puerto es 44367)
        private static readonly string ApiUrl = "https://localhost:44367/api/autenticacion/login";

        // GET: Auth/Login
        public ActionResult Login()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            using (var client = new HttpClient())
            {
                var jsonContent = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Llamada a la API para validar las credenciales y obtener el token
                HttpResponseMessage response = await client.PostAsync(ApiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    // Verificar si la respuesta es JSON
                    if (response.Content.Headers.ContentType.MediaType == "application/json")
                    {
                        // Se deserializa la respuesta en un objeto LoginResponse
                        var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseData);
                        if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                        {
                            // Decodificar el token JWT para obtener el rol
                            var handler = new JwtSecurityTokenHandler();
                            var jwtToken = handler.ReadToken(loginResponse.Token) as JwtSecurityToken;
                            string role = jwtToken?.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                            // Crear un ticket de autenticación de formularios que incluya el rol en los UserData
                            var authTicket = new FormsAuthenticationTicket(
                                1,                                // Versión
                                model.Email,                      // Nombre de usuario
                                DateTime.Now,                     // Fecha de emisión
                                DateTime.Now.AddMinutes(30),      // Fecha de expiración
                                false,                            // Persistencia (recordar o no)
                                role                              // UserData: en este caso, el rol
                            );

                            // Encriptar el ticket y crear la cookie
                            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
                            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
                            {
                                HttpOnly = true,
                                Expires = authTicket.Expiration
                            };
                            Response.Cookies.Add(authCookie);

                            // También puedes almacenar el token en sesión si lo necesitas
                            Session["Token"] = loginResponse.Token;
                            Session["Role"] = role;

                            // Redirigir según el rol obtenido
                            if (role.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
                            {
                                return RedirectToAction("Index", "Admin");
                            }
                            else if (role.Equals("Miembro", StringComparison.OrdinalIgnoreCase))
                            {
                                return RedirectToAction("Index", "Miembro");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Rol no reconocido.");
                                return View(model);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Respuesta inesperada del servidor.");
                        return View(model);
                    }
                }

                ModelState.AddModelError("", "Credenciales incorrectas.");
                return View(model);
            }
        }

        // Acción para cerrar sesión
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
