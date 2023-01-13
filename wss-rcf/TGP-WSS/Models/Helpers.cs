using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TGP.WSS.Models
{
    public class Helpers
    {

        /// <summary>
        /// Metodo que convierte una imagen a Thumbnails
        /// </summary>
        /// <param name="myImage"></param>
        /// <returns></returns>
        public static byte[] GetThumbnails(byte[] myImage)
        {

            using (var ms = new System.IO.MemoryStream(myImage))
            {
                var image = System.Drawing.Image.FromStream(ms);

                var ratioX = (double)37 / image.Width;
                var ratioY = (double)37 / image.Height;
                var ratio = Math.Min(ratioX, ratioY);

                var width = (int)(image.Width * ratio);
                var height = (int)(image.Height * ratio);

                var newImage = new System.Drawing.Bitmap(width, height);
                System.Drawing.Graphics.FromImage(newImage).DrawImage(image, 0, 0, width, height);
                System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(newImage);

                System.Drawing.ImageConverter converter = new System.Drawing.ImageConverter();

                myImage = (byte[])converter.ConvertTo(bmp, typeof(byte[]));

                return myImage;
            }
        }

    }
}