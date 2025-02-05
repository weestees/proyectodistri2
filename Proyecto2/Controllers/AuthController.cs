using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
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
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
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
                    // Se deserializa la respuesta en un objeto LoginResponse
                    var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseData);
                    if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                    {
                        // Decodificar el token JWT para obtener el rol
                        var handler = new JwtSecurityTokenHandler();
                        var jwtToken = handler.ReadToken(loginResponse.Token) as JwtSecurityToken;
                        string role = jwtToken?.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

                        // Se almacena el token y el rol en la sesión (opcional)
                        Session["Token"] = loginResponse.Token;
                        Session["Role"] = role;

                        // Redirigir según el rol obtenido
                        if (role == "Administrador")
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                        else if (role == "Miembro")
                        {
                            return RedirectToAction("Index", "Miembro");
                        }
                        else
                        {
                            // Si el rol no coincide con ninguno, se regresa al login
                            ModelState.AddModelError("", "Rol no reconocido.");
                            return View(model);
                        }
                    }
                }

                ModelState.AddModelError("", "Credenciales incorrectas.");
                return View(model);
            }
        }

        // Acción para cerrar sesión
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
