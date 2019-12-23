using System;
using System.Reflection;
using System.Windows;

namespace CFWindow
{
    internal static class Helpers
    {
        public static int DPI()
        {
            return (int)typeof(SystemParameters).GetProperty(
                "Dpi", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
        }

        public static double ScaleFactor()
        {
            return (double)DPI() / 96;
        }

        public static Thickness MarginForDPI()
        {
            Thickness thickness = new Thickness(8);
            switch (DPI())
            {
                case 120: thickness = new Thickness(7); break;
                case 144: thickness = new Thickness(7); break;
                case 168: thickness = new Thickness(7); break;
                case 192: thickness = new Thickness(7); break;
                case 240: thickness = new Thickness(7); break;
            }

            return thickness;
        }
    }
}
