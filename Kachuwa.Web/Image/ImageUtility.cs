using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kachuwa.Web
{
    public static class ImageUtility
    {
        public static async Task<Image> LoadImage(string url)
        {
            Image image = null;

            try
            {
                using (HttpClient httpClient = new HttpClient())
                using (HttpResponseMessage response = await httpClient.GetAsync(url))
                using (Stream inputStream = await response.Content.ReadAsStreamAsync())
                using (Bitmap temp = new Bitmap(inputStream))
                    image = new Bitmap(temp);
            }

            catch
            {
                // Add error logging here
            }

            return image;
        }
        public static Image CropImage(Image sourceImage, int sourceX, int sourceY, int sourceWidth, int sourceHeight, int destinationWidth, int destinationHeight)
        {
            Image destinationImage = new Bitmap(destinationWidth, destinationHeight);

            using (Graphics g = Graphics.FromImage(destinationImage))
                g.DrawImage(
                  sourceImage,
                  new Rectangle(0, 0, destinationWidth, destinationHeight),
                  new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                  GraphicsUnit.Pixel
                );

            return destinationImage;
        }
    }
}
