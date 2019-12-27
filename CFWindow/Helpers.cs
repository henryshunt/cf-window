using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace CFWindow
{
    internal static class Helpers
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        internal static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        internal static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 8)
                return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
            else return new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

        internal const int GWL_STYLE = -16;
        internal const uint WS_BORDER = 0x00800000;
        internal const uint WS_DLGFRAME = 0x00400000;
        internal const uint WS_THICKFRAME = 0x00040000;
        internal const uint WS_CAPTION = WS_BORDER | WS_DLGFRAME;
        internal const uint WS_SYSMENU = 0x00080000;


        [DllImport("user32.dll")]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll")]
        internal static extern uint TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd,
            IntPtr lptpm);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        internal const uint WM_SYSCOMMAND = 0x0112;
        internal const uint TPM_LEFTBUTTON = 0x0000;
        internal const uint TPM_RETURNCMD = 0x0100;


        internal const uint WM_NCRBUTTONUP = 0x00A5;
        internal const uint WM_KEYUP = 0x0101;
        internal const uint VK_MENU = 0x12;
    }
}
