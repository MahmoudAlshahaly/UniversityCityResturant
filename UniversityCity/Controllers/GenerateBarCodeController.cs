using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UnivercityCityFood.Controllers
{
    [Authorize(Roles = "Admins")]
    public class GenerateBarCodeController : Controller
    {
        // GET: GenerateBarCode
        // GET: BarCode/GenerateBarCode
        public ActionResult BarcodeWriter()
        {
            return View();
        }
        [HttpPost]
        public ActionResult BarcodeWriter(string barcode)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Bitmap bitmap = new Bitmap(barcode.Length * 30, 80))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        Font ofont = new Font("IDAutomationHC39M", 15);
                        PointF point = new PointF(2f, 2f);
                        SolidBrush whiteBrush = new SolidBrush(Color.White);
                        graphics.FillRectangle(whiteBrush, 0, 0, bitmap.Width, bitmap.Height);
                        SolidBrush blackBrush = new SolidBrush(Color.DarkBlue);
                        graphics.DrawString("*" + barcode + "*", ofont, blackBrush, point);
                    }
                    bitmap.Save(memoryStream, ImageFormat.Jpeg);
                    ViewBag.BarcodeImage = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            return View();
        }
    }
}