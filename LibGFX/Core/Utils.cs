using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibGFX.Core
{
    public class Utils
    {
        public static Bitmap CreateEmptyNormalMap(int width, int height)
        {
            Bitmap normalMap = new Bitmap(width, height);

            Color normalColor = Color.FromArgb(128, 128, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    normalMap.SetPixel(x, y, normalColor);
                }
            }

            return normalMap;
        }

        public static Bitmap CreateEmptyTexture(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            Color color = Color.FromArgb(255, 255, 255);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bitmap.SetPixel(x, y, color);
                }
            }

            return bitmap;
        }
    }
}
