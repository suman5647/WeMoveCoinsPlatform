using QRCoder;
using System;
using System.IO;

namespace WMC.Logic
{
    public static class QRCodeHelper
    {
       public static string GenerateBase64QRCode(string value)
        {
            using (var ms = new MemoryStream(GenerateQRCode(value)))
            {
                string SigBase64 = Convert.ToBase64String(ms.ToArray());
                return SigBase64;
            }

        }
        public static byte[] GenerateQRCode(string value)
        {
            string qrCstring = value;
            var qrCodeData = new QRCodeGenerator().CreateQrCode(qrCstring, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);
            return qrCodeAsBitmapByteArr;
            
        }
    }
}
