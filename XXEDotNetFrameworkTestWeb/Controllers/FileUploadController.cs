using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using XXEDotNetFrameworkTestWeb.Models;

namespace XXEDotNetFrameworkTestWeb.Controllers
{
    public class FileUploadController : Controller
    {
        public ActionResult FileUpload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FileUpload(FileUploadModel model)
        {
            if (model.File != null && model.File.ContentLength > 0)
            {
                try
                {
                    XmlDocument xmlDoc = new XmlDocument();

                    //enable row below to make the app vulnerable to XXE even if all config for .NET Framework in web.config is correct
                    //xmlDoc.XmlResolver = new XmlUrlResolver();

                    xmlDoc.Load(model.File.InputStream);

                    XmlNodeList productNodes = xmlDoc.SelectNodes("//Product");

                    int maxId = 0;
                    if (ProductController.Products.Any()) 
                    {
                        maxId = ProductController.Products.Max(p => p.Id);
                    }

                    foreach (XmlNode node in productNodes)
                    {
                        var product = new ProductModel
                        {
                            Id = maxId++,
                            Name = node["Name"]?.InnerText,
                            Description = node["Description"]?.InnerText,
                            ProductId = int.Parse(node["ProductId"]?.InnerText)
                        };

                        ProductController.Products.Add(product);
                    }
                }
                catch 
                {
                    return View("FileUploadFailed");
                }

                // Never save in App_Data, but this is demo so...
                var fileName = Path.GetFileName(model.File.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data"), fileName);
                model.File.SaveAs(path);

                ViewBag.Message = "File uploaded successfully!";
            }
            else
            {
                ViewBag.Message = "Please select a file to upload.";
            }

            return View();
        }
    }
}