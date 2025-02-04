
using System.Web.Http;
using System.Web.Mvc;

namespace Proyecto2.Controllers
{
    [System.Web.Http.Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
