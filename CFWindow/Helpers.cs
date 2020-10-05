using System;
using System.Runtime.InteropServices;
using System.Windows;
using Graphics = System.Drawing.Graphics;

namespace CFWindow
{
    internal static class Helpers
    {
        internal static Point TransformToPixels(double unitX, double unitY)
        {
            int pixelX, pixelY;
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                pixelX = (int)(unitX / (g.DpiX / 96));
                pixelY = (int)(unitY / (g.DpiY / 96));
            }

            // alternative:
            // using (Graphics g = Graphics.FromHdc(IntPtr.Zero)) { }

            return new Point(pixelX, pixelY);
        }


        internal const int WM_GETMINMAXINFO = 0x0024;

        [StructLayout(LayoutKind.Sequential)]
        public struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static implicit operator System.Drawing.Point(POINT p)
            {
                return new System.Drawing.Point(p.X, p.Y);
            }

            public static implicit operator POINT(System.Drawing.Point p)
            {
                return new POINT(p.X, p.Y);
            }
        }
    }
}
