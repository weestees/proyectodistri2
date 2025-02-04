using Microsoft.AspNetCore.Mvc;
using System.Web.Mvc;

namespace Proyecto2.Controllers
{
    [Authorize(Roles = "Miembro")]
    public class MiembroController : Controller
    {
        public IActionResult Index()
        {
            return (IActionResult)View();
        }
    }
}
