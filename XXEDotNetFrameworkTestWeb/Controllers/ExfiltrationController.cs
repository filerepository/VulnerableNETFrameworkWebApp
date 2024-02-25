using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace XXEDotNetFrameworkTestWeb.Controllers
{
    public class ExfiltrationController : Controller
    {
        //Should be stored in database but using this for demo
        public static List<string> ExfiltratedFiles = new List<string>();

        public ActionResult Index()
        {
            string x = HttpUtility.UrlDecode(Request.QueryString["x"]);
            ExfiltratedFiles.Add (x);   

            return View(ExfiltratedFiles);
        }
    }
}