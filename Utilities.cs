using System;
using System.Runtime.InteropServices;

namespace CFWindow
{
    public static class Utilities
    {
        [DllImport("user32.dll", EntryPoint = "GetDC")]
        public static extern IntPtr GetDeviceContext(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern int ReleaseDeviceContext(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", EntryPoint = "GetDeviceCaps")]
        public static extern int GetDeviceCapability(IntPtr hdc, DeviceCapability capability);

        public enum DeviceCapability
        {
            LogicalPixelsX = 88,
            LogicalPixelsY = 90
        }

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        public static extern int GetSystemMetrics(SystemMetric metric);

        public enum SystemMetric
        {
            FrameX = 32,
            FrameY = 33,
            BorderPadding = 92
        }
    }
}
