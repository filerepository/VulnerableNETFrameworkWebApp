using System.Web.Mvc;

namespace XXEDotNetFrameworkTestWeb.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            // Bad way to read headers, missing multiple values if present

            var clientIp = Request.Headers["X-Forwarded-For"];

            if (clientIp == "10.10.10.10")
            {
                return View();
            }
            else
            {
                return View("Forbidden");
            }

        }
    }

}