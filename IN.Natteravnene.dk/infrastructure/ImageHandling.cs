/**************************************************************************
Natteravnenes Intranet (c) by Dan Taxbøl

Natteravnenes Intranet is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License.

You should have received a copy of the license along with this
work.  If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.
***************************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;

namespace NR.Infrastructure
{
    public static class ImageHandling
    {

        /// <summary>
        /// Check if min. size and image ratio is ok, returns false otherwise
        /// </summary>
        /// <param name="image">image object</param>
        /// <param name="Width">Min Width</param>
        /// <param name="Height">Min Height</param>
        /// <param name="tolerance">Tolerence between Height anf Widt in %</param>
        /// <returns></returns>
        public static bool imageSizeRatioOk(Image image, int Width, int Height, int tolerance)
        {

            double ratio = (double)image.Width / image.Height;
            double ideeal = (double)Width/Height;

            return (image.Height >= Height & image.Width >= Width & Math.Abs(ratio - ideeal) <= (double)tolerance / 100);
        }
        
        /// <summary>
        /// Resize image to specified width and height
        /// </summary>
        /// <param name="image">Image Object</param>
        /// <param name="Width">Width of final image</param>
        /// <param name="Height">Height of final image</param>
        /// <returns></returns>
        public static void imageResize(ref Image image, int Width, int Height)
        {
            var newImage = new Bitmap(Width, Height);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, Width, Height);
            image = newImage;
        }

    }
}