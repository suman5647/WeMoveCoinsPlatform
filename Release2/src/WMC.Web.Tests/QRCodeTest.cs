using Microsoft.VisualStudio.TestTools.UnitTesting;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMC.Web.Tests
{
    [TestClass]
    public class QRCodeTest
    {
        [TestMethod]
        public void GenerateQRCodeTest()
        {
            var ciQR = new CultureInfo("en-US"); //get culture info

            string qrCodestr = string.Format("bitcoin:{0}?amount={1}", "2Mxj4kYmshJ8JpaMnajvZafTRLVt2dMJGLn", (0.00480000M).ToString("N8", ciQR));
            var qrCodeData = new QRCoder.QRCodeGenerator().CreateQrCode(qrCodestr, QRCoder.QRCodeGenerator.ECCLevel.Q);
            //QRCoder.BitmapByteQRCodeHelper.GetQRCode("bitcoin:2Mxj4kYmshJ8JpaMnajvZafTRLVt2dMJGLn?amount=0.00480000", QRCoder.QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);

            //From here on, you can implement your platform-dependent byte[]-to-image code 

            //e.g. Windows 10 - Full .NET Framework
           // Bitmap bmp;
            using (var ms = new MemoryStream(qrCodeAsBitmapByteArr))
            {
            
             //   bmp = new Bitmap(ms);
               // bmp.Save("C:/TFS/test.bmp");
                string SigBase64 = Convert.ToBase64String(ms.ToArray());
                string html = "<img src='data:image/png;base64," + SigBase64 + "'>";
              
            }

        }
    }
}
