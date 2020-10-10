using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace CFWindow
{
    internal static class Helpers
    {
        // https://stackoverflow.com/a/61299269

        public static Thickness MaximisedWindowOffset()
        {
            int d = GetSystemMetrics(SystemMetric.SM_CXPADDEDBORDER);

            int x = GetSystemMetrics(SystemMetric.CXFRAME) + d;
            double dpiX = GetDpi(DeviceCap.LOGPIXELSX);
            double leftBorder = x / dpiX;

            int y = GetSystemMetrics(SystemMetric.CYFRAME) + d;
            double dpiY = GetDpi(DeviceCap.LOGPIXELSY);
            double topBorder = y / dpiY;

            return new Thickness(leftBorder, topBorder, leftBorder, topBorder);
        }

        private static double GetDpi(DeviceCap deviceCap)
        {
            IntPtr desktopWnd = IntPtr.Zero;
            IntPtr dc = GetDC(desktopWnd);
            double dpi;

            try
            {
                dpi = GetDeviceCaps(dc, deviceCap);
            }
            finally
            {
                ReleaseDC(desktopWnd, dc);
            }

            return dpi / 96f;
        }


        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, DeviceCap dcIndex);

        private enum DeviceCap
        {
            LOGPIXELSX = 88,
            LOGPIXELSY = 90
        }


        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SystemMetric smIndex);

        private enum SystemMetric
        {
            CXFRAME = 32,
            CYFRAME = 33,
            SM_CXPADDEDBORDER = 92
        }
    }
}
