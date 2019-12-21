using System;
using System.Reflection;
using System.Windows;

namespace CFWindow
{
    internal static class Helpers
    {
        public static int DPI()
        {
            Console.WriteLine("DPI: " + (int)typeof(SystemParameters).GetProperty(
                "Dpi", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null));
            return (int)typeof(SystemParameters).GetProperty(
                "Dpi", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null);
        }

        public static double ScaleFactor()
        {
            Console.WriteLine("SCALE: " + (double)DPI() / 96);
            return (double)DPI() / 96;
        }

        public static Thickness MarginForDPI()
        {
            Thickness thickness = new Thickness(8);
            switch (DPI())
            {
                case 120: thickness = new Thickness(7); break;
                case 144: thickness = new Thickness(7, 7, 3, 1); break;
                case 168: thickness = new Thickness(6, 6, 2, 0); break;
                case 192: thickness = new Thickness(7); break;
                case 240: thickness = new Thickness(6, 6, 0, 0); break;
            }

            return thickness;
        }
    }
}
