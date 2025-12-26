using OpenTK.Mathematics;
using System;
using System.Globalization;

namespace LibGFX.Graphics
{
    /// <summary>
    /// A collection of convenient color presets and helpers.
    /// Colors are returned as Vector4 (r,g,b,a) with each component in range [0,1].
    /// </summary>
    public static class ColorPresets
    {
        // Common colors (alpha = 1.0f)
        public static Vector4 White => FromRgb(255, 255, 255);
        public static Vector4 AliceBlue => FromRgb(240, 248, 255);
        public static Vector4 AntiqueWhite => FromRgb(250, 235, 215);
        public static Vector4 Aqua => FromRgb(0, 255, 255);
        public static Vector4 Aquamarine => FromRgb(127, 255, 212);
        public static Vector4 Azure => FromRgb(240, 255, 255);
        public static Vector4 Beige => FromRgb(245, 245, 220);
        public static Vector4 Bisque => FromRgb(255, 228, 196);
        public static Vector4 Black => FromRgb(0, 0, 0);
        public static Vector4 BlanchedAlmond => FromRgb(255, 235, 205);
        public static Vector4 Blue => FromRgb(0, 0, 255);
        public static Vector4 BlueViolet => FromRgb(138, 43, 226);
        public static Vector4 Brown => FromRgb(165, 42, 42);
        public static Vector4 BurlyWood => FromRgb(222, 184, 135);
        public static Vector4 CadetBlue => FromRgb(95, 158, 160);
        public static Vector4 Chartreuse => FromRgb(127, 255, 0);
        public static Vector4 Chocolate => FromRgb(210, 105, 30);
        public static Vector4 Coral => FromRgb(255, 127, 80);
        public static Vector4 CornflowerBlue => FromRgb(100, 149, 237);
        public static Vector4 Crimson => FromRgb(220, 20, 60);
        public static Vector4 Cyan => FromRgb(0, 255, 255);
        public static Vector4 DarkBlue => FromRgb(0, 0, 139);
        public static Vector4 DarkCyan => FromRgb(0, 139, 139);
        public static Vector4 DarkGoldenrod => FromRgb(184, 134, 11);
        public static Vector4 DarkGray => FromRgb(169, 169, 169);
        public static Vector4 DarkGreen => FromRgb(0, 100, 0);
        public static Vector4 DarkKhaki => FromRgb(189, 183, 107);
        public static Vector4 DarkMagenta => FromRgb(139, 0, 139);
        public static Vector4 DarkOliveGreen => FromRgb(85, 107, 47);
        public static Vector4 DarkOrange => FromRgb(255, 140, 0);
        public static Vector4 DarkOrchid => FromRgb(153, 50, 204);
        public static Vector4 DarkRed => FromRgb(139, 0, 0);
        public static Vector4 DarkSalmon => FromRgb(233, 150, 122);
        public static Vector4 DarkSeaGreen => FromRgb(143, 188, 143);
        public static Vector4 DarkSlateBlue => FromRgb(72, 61, 139);
        public static Vector4 DarkSlateGray => FromRgb(47, 79, 79);
        public static Vector4 DarkTurquoise => FromRgb(0, 206, 209);
        public static Vector4 DarkViolet => FromRgb(148, 0, 211);
        public static Vector4 DeepPink => FromRgb(255, 20, 147);
        public static Vector4 DeepSkyBlue => FromRgb(0, 191, 255);
        public static Vector4 DimGray => FromRgb(105, 105, 105);
        public static Vector4 DodgerBlue => FromRgb(30, 144, 255);
        public static Vector4 Firebrick => FromRgb(178, 34, 34);
        public static Vector4 FloralWhite => FromRgb(255, 250, 240);
        public static Vector4 ForestGreen => FromRgb(34, 139, 34);
        public static Vector4 Fuchsia => FromRgb(255, 0, 255);
        public static Vector4 Gainsboro => FromRgb(220, 220, 220);
        public static Vector4 GhostWhite => FromRgb(248, 248, 255);
        public static Vector4 Gold => FromRgb(255, 215, 0);
        public static Vector4 Goldenrod => FromRgb(218, 165, 32);
        public static Vector4 Gray => FromRgb(128, 128, 128);
        public static Vector4 Green => FromRgb(0, 128, 0);
        public static Vector4 GreenYellow => FromRgb(173, 255, 47);
        public static Vector4 Honeydew => FromRgb(240, 255, 240);
        public static Vector4 HotPink => FromRgb(255, 105, 180);
        public static Vector4 IndianRed => FromRgb(205, 92, 92);
        public static Vector4 Indigo => FromRgb(75, 0, 130);
        public static Vector4 Ivory => FromRgb(255, 255, 240);
        public static Vector4 Khaki => FromRgb(240, 230, 140);
        public static Vector4 Lavender => FromRgb(230, 230, 250);
        public static Vector4 LavenderBlush => FromRgb(255, 240, 245);
        public static Vector4 LawnGreen => FromRgb(124, 252, 0);
        public static Vector4 LemonChiffon => FromRgb(255, 250, 205);
        public static Vector4 LightBlue => FromRgb(173, 216, 230);
        public static Vector4 LightCoral => FromRgb(240, 128, 128);
        public static Vector4 LightCyan => FromRgb(224, 255, 255);
        public static Vector4 LightGoldenrodYellow => FromRgb(250, 250, 210);
        public static Vector4 LightGray => FromRgb(211, 211, 211);
        public static Vector4 LightGreen => FromRgb(144, 238, 144);
        public static Vector4 LightPink => FromRgb(255, 182, 193);
        public static Vector4 LightSalmon => FromRgb(255, 160, 122);
        public static Vector4 LightSeaGreen => FromRgb(32, 178, 170);
        public static Vector4 LightSkyBlue => FromRgb(135, 206, 250);
        public static Vector4 LightSlateGray => FromRgb(119, 136, 153);
        public static Vector4 LightSteelBlue => FromRgb(176, 196, 222);
        public static Vector4 Lime => FromRgb(0, 255, 0);
        public static Vector4 LimeGreen => FromRgb(50, 205, 50);
        public static Vector4 Linen => FromRgb(250, 240, 230);
        public static Vector4 Magenta => FromRgb(255, 0, 255);
        public static Vector4 Maroon => FromRgb(128, 0, 0);
        public static Vector4 MediumAquamarine => FromRgb(102, 205, 170);
        public static Vector4 MediumBlue => FromRgb(0, 0, 205);
        public static Vector4 MediumOrchid => FromRgb(186, 85, 211);
        public static Vector4 MediumPurple => FromRgb(147, 112, 219);
        public static Vector4 MediumSeaGreen => FromRgb(60, 179, 113);
        public static Vector4 MediumSlateBlue => FromRgb(123, 104, 238);
        public static Vector4 MediumSpringGreen => FromRgb(0, 250, 154);
        public static Vector4 MediumTurquoise => FromRgb(72, 209, 204);
        public static Vector4 MediumVioletRed => FromRgb(199, 21, 133);
        public static Vector4 MidnightBlue => FromRgb(25, 25, 112);
        public static Vector4 MintCream => FromRgb(245, 255, 250);
        public static Vector4 MistyRose => FromRgb(255, 228, 225);
        public static Vector4 Moccasin => FromRgb(255, 228, 181);
        public static Vector4 NavajoWhite => FromRgb(255, 222, 173);
        public static Vector4 Navy => FromRgb(0, 0, 128);
        public static Vector4 OldLace => FromRgb(253, 245, 230);
        public static Vector4 Olive => FromRgb(128, 128, 0);
        public static Vector4 OliveDrab => FromRgb(107, 142, 35);
        public static Vector4 Orange => FromRgb(255, 165, 0);
        public static Vector4 OrangeRed => FromRgb(255, 69, 0);
        public static Vector4 Orchid => FromRgb(218, 112, 214);
        public static Vector4 PaleGoldenrod => FromRgb(238, 232, 170);
        public static Vector4 PaleGreen => FromRgb(152, 251, 152);
        public static Vector4 PaleTurquoise => FromRgb(175, 238, 238);
        public static Vector4 PaleVioletRed => FromRgb(219, 112, 147);
        public static Vector4 PapayaWhip => FromRgb(255, 239, 213);
        public static Vector4 PeachPuff => FromRgb(255, 218, 185);
        public static Vector4 Peru => FromRgb(205, 133, 63);
        public static Vector4 Pink => FromRgb(255, 192, 203);
        public static Vector4 Plum => FromRgb(221, 160, 221);
        public static Vector4 PowderBlue => FromRgb(176, 224, 230);
        public static Vector4 Purple => FromRgb(128, 0, 128);
        public static Vector4 Red => FromRgb(255, 0, 0);
        public static Vector4 RosyBrown => FromRgb(188, 143, 143);
        public static Vector4 RoyalBlue => FromRgb(65, 105, 225);
        public static Vector4 SaddleBrown => FromRgb(139, 69, 19);
        public static Vector4 Salmon => FromRgb(250, 128, 114);
        public static Vector4 SandyBrown => FromRgb(244, 164, 96);
        public static Vector4 SeaGreen => FromRgb(46, 139, 87);
        public static Vector4 Seashell => FromRgb(255, 245, 238);
        public static Vector4 Sienna => FromRgb(160, 82, 45);
        public static Vector4 Silver => FromRgb(192, 192, 192);
        public static Vector4 SkyBlue => FromRgb(135, 206, 235);
        public static Vector4 SlateBlue => FromRgb(106, 90, 205);
        public static Vector4 SlateGray => FromRgb(112, 128, 144);
        public static Vector4 Snow => FromRgb(255, 250, 250);
        public static Vector4 SpringGreen => FromRgb(0, 255, 127);
        public static Vector4 SteelBlue => FromRgb(70, 130, 180);
        public static Vector4 Tan => FromRgb(210, 180, 140);
        public static Vector4 Teal => FromRgb(0, 128, 128);
        public static Vector4 Thistle => FromRgb(216, 191, 216);
        public static Vector4 Tomato => FromRgb(255, 99, 71);
        public static Vector4 Turquoise => FromRgb(64, 224, 208);
        public static Vector4 Violet => FromRgb(238, 130, 238);
        public static Vector4 Wheat => FromRgb(245, 222, 179);
        public static Vector4 WhiteSmoke => FromRgb(245, 245, 245);
        public static Vector4 Yellow => FromRgb(255, 255, 0);
        public static Vector4 YellowGreen => FromRgb(154, 205, 50);

        /// <summary>
        /// Create a Vector4 color from 0-255 RGB values and optional alpha (0..255).
        /// </summary>
        public static Vector4 FromRgb(byte r, byte g, byte b, byte a = 255)
            => new Vector4(r / 255f, g / 255f, b / 255f, a / 255f);

        /// <summary>
        /// Parse a hex string like "#RRGGBB" or "#RRGGBBAA" into a Vector4.
        /// Accepts "RRGGBB" or "RRGGBBAA" (with or without leading '#').
        /// </summary>
        public static Vector4 FromHex(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("hex must not be null or empty", nameof(hex));

            var h = hex.Trim().TrimStart('#');
            if (h.Length != 6 && h.Length != 8)
                throw new FormatException("Hex color must be 6 or 8 hex digits (RRGGBB or RRGGBBAA).");

            uint val = uint.Parse(h, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            if (h.Length == 6)
            {
                byte r = (byte)((val & 0xFF0000) >> 16);
                byte g = (byte)((val & 0x00FF00) >> 8);
                byte b = (byte)(val & 0x0000FF);
                return FromRgb(r, g, b);
            }
            else // 8 digits: RRGGBBAA
            {
                byte r = (byte)((val & 0xFF000000) >> 24);
                byte g = (byte)((val & 0x00FF0000) >> 16);
                byte b = (byte)((val & 0x0000FF00) >> 8);
                byte a = (byte)(val & 0x000000FF);
                return FromRgb(r, g, b, a);
            }
        }

        /// <summary>
        /// Return a copy of color with modified alpha (0..1).
        /// </summary>
        public static Vector4 WithAlpha(Vector4 color, float alpha)
        {
            return new Vector4(color.X, color.Y, color.Z, MathHelper.Clamp(alpha, 0f, 1f));
        }

        /// <summary>
        /// Linearly interpolate between two colors (t in [0,1]).
        /// </summary>
        public static Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            t = MathHelper.Clamp(t, 0f, 1f);
            return new Vector4(
                MathHelper.Lerp(a.X, b.X, t),
                MathHelper.Lerp(a.Y, b.Y, t),
                MathHelper.Lerp(a.Z, b.Z, t),
                MathHelper.Lerp(a.W, b.W, t)
            );
        }
    }
}