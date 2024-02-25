using ClosedXML.Excel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using XXEDotNetFrameworkTestWeb.Models;

namespace XXEDotNetFrameworkTestWeb.Controllers
{
    public class ProductController : Controller
    {
        //Should be stored in database but using this for demo
        public static List<ProductModel> Products = new List<ProductModel>();

        public ProductController()
        {
        }

        public ActionResult Index()
        {
            var products = Products.ToList();
            return View(products);
        }

        public ActionResult Product()
        {
            var products = Products.ToList();
            return View(products);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                Products.Add(product);

                return RedirectToAction("Index");
            }

            return View(product);
        }

        public ActionResult Edit(int id)
        {
            var product = Products.Find(p => p.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                var productToUpdate = Products.Find(p => p.Id == product.Id);
                productToUpdate.Name = product.Name;
                productToUpdate.Description = product.Description;
                productToUpdate.ProductComment = SanitizeInputInABadWay(product.ProductComment);

                return RedirectToAction("Index");
            }

            return View(product);
        }


        public static string SanitizeInputInABadWay(string input)
        {
            // First, encode the entire string to HTML entities to neutralize most XSS vectors
            string encodedInput = HttpUtility.HtmlEncode(input);

            // Use a regular expression to find all encoded instances of <a href=""> tags
            // This pattern is optimistic and definitely misses edge cases
            var regex = new Regex(@"&lt;a href=&quot;(.*?)&quot;&gt;(.*?)&lt;/a&gt;", RegexOptions.IgnoreCase);

            // Replace all found encoded <a> tags with "properly" sanitized <a> tags.
            string sanitizedInput = regex.Replace(encodedInput, match =>
            {
                bool isDangersousLink = match.Groups[1].Value.ToLower().Contains("javascript");

                string href = isDangersousLink ? "#" : HttpUtility.HtmlDecode(match.Groups[1].Value);
                
                string text = HttpUtility.HtmlDecode(match.Groups[2].Value);

                // Re-encode the text to ensure it's "safely" encoded.
                text = HttpUtility.HtmlEncode(text);

                return $"<a href=\"{href}\">{text}</a>";
            });

            return sanitizedInput;
        }

        public ActionResult ExportProductsToExcel()
        {
            List<ProductModel> products = Products; 

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Products");
                worksheet.Cell(1, 1).Value = "Id";
                worksheet.Cell(1, 2).Value = "Name";
                worksheet.Cell(1, 3).Value = "Description";
                worksheet.Cell(1, 4).Value = "ProductId";

                int currentRow = 2;
                foreach (var product in products)
                {
                    worksheet.Cell(currentRow, 1).Value = product.Id;
                    worksheet.Cell(currentRow, 2).Value = product.Name;
                    worksheet.Cell(currentRow, 3).Value = product.Description;
                    worksheet.Cell(currentRow, 4).Value = product.ProductId;
                    currentRow++;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush(); 
                    stream.Position = 0;

                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
                }
            }
        }
    }
}