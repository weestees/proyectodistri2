using Microsoft.Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.IdentityModel.Tokens;
using Owin;
using System.Text;
using System.Web.Http;
using Microsoft.Owin.Security.DataHandler.Encoder;

[assembly: OwinStartup(typeof(Proyecto2.Startup))]

namespace Proyecto2
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var issuer = "yourdomain.com";
            var audience = "yourdomain.com";
            var secret = Encoding.UTF8.GetBytes("ClaveSuperSecretadelPry2");

            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
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

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}