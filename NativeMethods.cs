using System;
using System.Runtime.InteropServices;

namespace CFWindow
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDeviceContext(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern int ReleaseDeviceContext(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, DeviceCap index);

        public enum DeviceCap
        {
            LOGPIXELSX = 88,
            LOGPIXELSY = 90
        }

        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric nIndex);

        public enum SystemMetric
        {
            SM_CXFRAME = 32,
            SM_CYFRAME = 33,
            SM_CXPADDEDBORDER = 92
        }
    }
}
