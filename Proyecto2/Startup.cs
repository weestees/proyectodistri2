using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.IdentityModel.Tokens;
using Owin;
using System.Text;

[assembly: OwinStartup(typeof(Proyecto2.Startup))]

namespace Proyecto2
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var issuer = "tu_issuer";
            var audience = "tu_audience";

            // Clave secreta en texto plano (32 caracteres)
            var secret = Encoding.UTF8.GetBytes("ClaveSuperSecreta1234567890!@#$%^");

            app.Map("/api/autenticacion/login", loginApp =>
            {
                loginApp.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
                {
                    TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(secret)
                    }
                });
            });
        }
    }
}