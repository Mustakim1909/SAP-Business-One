using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;

namespace EInvoice
{
    public class QrGenerator
    {
        public void GenerateQRCodeAsBytes(string url,string folderpath,string invno)
        {
            string filename = $"{invno}.png";
            string outputpath = Path.Combine(folderpath, filename);
            if (File.Exists(outputpath))
            {
                File.Delete(outputpath);
            }
            if (!Directory.Exists(folderpath))
            {
                Directory.CreateDirectory(folderpath);
            }
            var qrWriter = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 200,
                    Width = 200,
                    Margin = 1
                }
            };

            var qrimage = qrWriter.Write(url);
            qrimage.Save(outputpath, ImageFormat.Png);
        }
    }
}
