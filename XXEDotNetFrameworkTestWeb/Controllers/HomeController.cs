using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace XXEDotNetFrameworkTestWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // introduce referer leakage vulnerability
            Response.Headers["Referrer-Policy"] = "unsafe-url";

            // show referer
            ViewBag.Ref = Request.Headers["Referer"]; 

            return View();
        }

        public ActionResult About()
        {
            var clientPort = GetLastHeaderValue(Request, "X-Forwarded-Port");
            if (string.IsNullOrEmpty(clientPort))
            {
                Request.Headers["X-Forwarded-Port"] = "443";
            }

            var headers = new Dictionary<string, string>();
            foreach (string key in Request.Headers.AllKeys)
            {
                headers[key] = Request.Headers[key];
            }

            ViewBag.Message = "Your application description page.";

            return View(headers);
        }

        internal string GetLastHeaderValue(HttpRequestBase request, string headerName)
        {
            string headerValue = String.Empty;

            if (!string.IsNullOrWhiteSpace(headerName))
            {
                if (request.Headers[headerName] != null)  //No need to worry about casing since "Headers" is a NameValueCollection that by default uses case-insensitive hash code provider and the default case-insensitive comparer
                {
                    var ips = request.Headers[headerName].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (ips.Length > 0)
                    {
                        headerValue = ips.Last().Trim(); 
                    }
                }
            }

            return headerValue;

        }



        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult PasswordReset()
        {
            // introduce referer leakage vulnerability
            Response.Headers["Referrer-Policy"] = "unsafe-url";

            ViewBag.Message = "Password reset page.";

            return View();
        }

    }
}