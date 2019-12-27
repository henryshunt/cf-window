using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using static CFWindow.Helpers;
using Screen = System.Windows.Forms.Screen;

namespace CFWindow
{
    public partial class CFWindow : Window
    {
        private IntPtr WindowHandle;
        //private Helpers.MONITORINFO CurrentDisplay;
        private Screen CScreen;

        private Border WindowFrame;
        private Border WindowContent;
        private Image WindowIcon;
        private TextBlock WindowTitle;
        private StackPanel WindowButtons;
        private WindowButton WindowMinimise;
        private WindowButton WindowMaximise;
        private WindowButton WindowRestore;
        private WindowButton WindowClose;

        private bool DragFromMaximised = false;

        public CFWindow()
        {
            DataContext = this;

            // Set the style so we don't have to do that manually in XAML
            Style = (Style)Application.Current.Resources["CFWindow"];
        }

        public override void OnApplyTemplate()
        {
            WindowFrame = GetTemplateChild<Border>("WindowFrame");
            WindowContent = GetTemplateChild<Border>("WindowContent");
            WindowIcon = GetTemplateChild<Image>("WindowIcon");
            WindowTitle = GetTemplateChild<TextBlock>("WindowTitle");
            WindowButtons = GetTemplateChild<StackPanel>("WindowButtons");
            WindowMinimise = GetTemplateChild<WindowButton>("WindowMinimize");
            WindowMaximise = GetTemplateChild<WindowButton>("WindowMaximize");
            WindowRestore = GetTemplateChild<WindowButton>("WindowRestore");
            WindowClose = GetTemplateChild<WindowButton>("WindowClose");

            ApplyWindowState();

            // Hide elements based on properties
            if (ResizeMode == ResizeMode.NoResize)
            {
                WindowMinimise.Visibility = Visibility.Collapsed;
                WindowMaximise.Visibility = Visibility.Collapsed;
                WindowRestore.Visibility = Visibility.Collapsed;
                WindowClose.Width = (double)Application
                    .Current.Resources["Window.NoResize.CloseButton.Width"];
            }
            else if (ResizeMode == ResizeMode.CanMinimize)
            {
                WindowMaximise.IsEnabled = false;
                WindowRestore.IsEnabled = false;
            }

            // Add event handlers here as they need the visual tree
            SizeChanged += CFWindow_SizeChanged;
            StateChanged += CFWindow_StateChanged;
            Activated += CFWindow_ActiveChanged;
            Deactivated += CFWindow_ActiveChanged;
            LocationChanged += CFWindow_LocationChanged;

            WindowMinimise.Click += WindowMinimize_Click;
            WindowMaximise.Click += WindowMaximize_Click;
            WindowRestore.Click += WindowRestore_Click;
            WindowClose.Click += WindowClose_Click;

            SystemEvents.DisplaySettingsChanged += DisplaySettingsChanged;
        }

        private void CFWindow_LocationChanged(object sender, EventArgs e)
        {
            //CurrentDisplay = Helpers.GetWindowDisplay(WindowHandlePtr);

            //PresentationSource source = PresentationSource.FromVisual(this);
            //double dpiX = 0, dpiY = 0;
            //if (source != null)
            //{
            //    dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
            //    dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            //}

            //Console.WriteLine(dpiX + ", " + dpiY);

            CScreen = Screen.FromHandle(WindowHandle);
            //Console.WriteLine("{0} working: {1} left: {2} top: {3}", CScreen.DeviceName, CScreen.WorkingArea, Left, Top);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            WindowHandle = new WindowInteropHelper(this).Handle;

            HwndSource windowHandle = HwndSource.FromHwnd(WindowHandle);
            if (windowHandle != null)
                windowHandle.AddHook(new HwndSourceHook(WindowProcedure));

            // Get the OS-implemented window style information
            uint windowStyle = (uint)Helpers.GetWindowLongPtr(WindowHandle, Helpers.GWL_STYLE);

            // Remove the OS-implemented window border. If there's no border then Windows will not maximise the window
            // larger than the screen to hide it, and there is then no need for any hacks to re-align the window. This
            // is the crucial key to solving all of the sizing issues associated with custom window styling.
            // Based on https://stackoverflow.com/questions/2398746/removing-window-border
            windowStyle &= ~(Helpers.WS_THICKFRAME | Helpers.WS_CAPTION | Helpers.WS_SYSMENU);
            Helpers.SetWindowLongPtr(new HandleRef(this, WindowHandle), Helpers.GWL_STYLE, (IntPtr)windowStyle);
        }

        private IntPtr WindowProcedure(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            switch ((uint)msg)
            {
                // Override non-client area right click to show the system menu under the window icon
                case Helpers.WM_NCRBUTTONUP:
                    {
                        handled = true;
                        ShowSystemMenu();
                        break;
                    }

                // Show the system menu when the alt+space shortcut is triggered
                case Helpers.WM_KEYUP:
                    {
                        if ((int)wparam == Helpers.VK_MENU)
                        {
                            handled = true;
                            ShowSystemMenu();
                        }

                        break;
                    }
            }

            return IntPtr.Zero;
        }
        private void ShowSystemMenu()
        {
            string state = WindowState == WindowState.Normal ? "Normal" : "Maximised";

            double menuXPosition = 0;
            menuXPosition += ((Thickness)
                Application.Current.Resources["Window." + state + ".Frame.BorderThickness"]).Left;
            menuXPosition += ((Thickness)
                Application.Current.Resources["Window." + state + ".Icon.Margin"]).Left;

            double menuYPosition = 0;
            menuYPosition += ((Thickness)
                Application.Current.Resources["Window." + state + ".Frame.BorderThickness"]).Top;
            menuYPosition += ((Thickness)
                Application.Current.Resources["Window." + state + ".Icon.Margin"]).Top;
            menuYPosition += WindowIcon.Height;

            // Calculate position to show menu at (bottom-left corner of icon)
            Point point = new Point((Left + menuXPosition) * 2, (Top + menuYPosition) * 2);

            IntPtr menu = Helpers.GetSystemMenu(WindowHandle, false);

            // Show the menu and block until dismissed or a command is selected
            uint command = Helpers.TrackPopupMenuEx(menu, Helpers.TPM_LEFTBUTTON | Helpers.TPM_RETURNCMD,
                (int)point.X, (int)point.Y, WindowHandle, IntPtr.Zero);
            if (command == 0) return;

            // Trigger the command selected from the menu
            Helpers.PostMessage(
                new HandleRef(this, WindowHandle), Helpers.WM_SYSCOMMAND, new IntPtr(command), IntPtr.Zero);
        }

        private void CFWindow_StateChanged(object sender, EventArgs e)
        {
            ApplyWindowState();

            //Console.WriteLine("state");
            //SetWindowLongPtr(new HandleRef(this, WindowHandlePtr), GWL_STYLE, new IntPtr(WS_VISIBLE));

            //SetWindowPos(WindowHandlePtr, new IntPtr(0), 10, 10, 0, 0, (SetWindowPosFlags)
            //    (SWP_NOZORDER | SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_FRAMECHANGED));
        }
        private void CFWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //if (WindowState != WindowState.Maximized) return;

            //Screen screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);

            // Window has changed screen while maximised
            //if (PreviousScreen.Width != screen.WorkingArea.Width
            //    || PreviousScreen.Height != screen.WorkingArea.Height)
            //{
            //    PreviousScreen = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height);
            //    OnStateChanged(new EventArgs());
            //}
        }
        private void CFWindow_ActiveChanged(object sender, EventArgs e)
        {
            ApplyWindowActive();
        }

        private void WindowMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void WindowMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }
        private void WindowRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }
        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DisplaySettingsChanged(object sender, EventArgs e)
        {
            //CurrentDisplay = Helpers.GetWindowDisplay(WindowHandlePtr);
            OnStateChanged(new EventArgs());
        }

        private void ApplyWindowState()
        {
            if (WindowState == WindowState.Minimized) return;
            else if (WindowState == WindowState.Maximized)
            {
                WindowFrame.BorderThickness = (Thickness)Application
                    .Current.Resources["Window.Maximised.Frame.BorderThickness"];
                WindowIcon.Margin = (Thickness)Application.Current.Resources["Window.Maximised.Icon.Margin"];
                WindowTitle.Margin = (Thickness)Application.Current.Resources["Window.Maximised.Title.Margin"];
                WindowTitle.FontSize = (double)Application.Current.Resources["Window.Maximised.Title.FontSize"];
                WindowButtons.Margin = (Thickness)Application.Current.Resources["Window.Maximised.Buttons.Margin"];
                WindowButtons.Height = (double)Application.Current.Resources["Window.Maximised.Buttons.Height"];
                WindowContent.Margin = (Thickness)Application.Current.Resources["Window.Maximised.Content.Margin"];
                WindowContent.BorderThickness = (Thickness)Application
                    .Current.Resources["Window.Maximised.Content.BorderThickness"];

                if (ResizeMode != ResizeMode.NoResize)
                {
                    WindowMaximise.Visibility = Visibility.Collapsed;
                    WindowRestore.Visibility = Visibility.Visible;
                }

                WindowChrome.SetWindowChrome(this, new WindowChrome
                {
                    ResizeBorderThickness = new Thickness(0),
                    CaptionHeight =
                        ((Thickness)Application.Current.Resources["Window.Maximised.Frame.BorderThickness"]).Top +
                        ((Thickness)Application.Current.Resources["Window.Maximised.Content.Margin"]).Top,

                    GlassFrameThickness = new Thickness(0),
                    CornerRadius = new CornerRadius(0)
                });
            }
            else if (WindowState == WindowState.Normal)
            {
                WindowFrame.BorderThickness = (Thickness)Application
                    .Current.Resources["Window.Normal.Frame.BorderThickness"];
                WindowIcon.Margin = (Thickness)Application.Current.Resources["Window.Normal.Icon.Margin"];
                WindowTitle.Margin = (Thickness)Application.Current.Resources["Window.Normal.Title.Margin"];
                WindowTitle.FontSize = (double)Application.Current.Resources["Window.Normal.Title.FontSize"];
                WindowButtons.Margin = (Thickness)Application.Current.Resources["Window.Normal.Buttons.Margin"];
                WindowButtons.Height = (double)Application.Current.Resources["Window.Normal.Buttons.Height"];
                WindowContent.Margin = (Thickness)Application.Current.Resources["Window.Normal.Content.Margin"];
                WindowContent.BorderThickness = (Thickness)Application
                    .Current.Resources["Window.Normal.Content.BorderThickness"];

                if (ResizeMode != ResizeMode.NoResize)
                {
                    WindowMaximise.Visibility = Visibility.Visible;
                    WindowRestore.Visibility = Visibility.Collapsed;
                }

                // Set the WindowChrome to utilise system-provided resize functionality
                WindowChrome.SetWindowChrome(this, new WindowChrome
                {
                    ResizeBorderThickness =
                        (Thickness)Application.Current.Resources["Window.Chrome.ResizeBorderThickness"],

                    CaptionHeight =
                        ((Thickness)Application.Current.Resources["Window.Normal.Frame.BorderThickness"]).Top +
                        ((Thickness)Application.Current.Resources["Window.Normal.Content.Margin"]).Top,

                    GlassFrameThickness = new Thickness(0),
                    CornerRadius = new CornerRadius(0)
                });
            }
        }
        private void ApplyWindowActive()
        {
            if (IsActive)
            {
                WindowFrame.Background = (SolidColorBrush)Application
                    .Current.Resources["Window.Active.Frame.Background"];
                WindowFrame.BorderBrush = (SolidColorBrush)Application
                    .Current.Resources["Window.Active.Frame.BorderBrush"];

                WindowMinimise.ShowDisabled = false;
                WindowMaximise.ShowDisabled = false;
                WindowRestore.ShowDisabled = false;
                WindowClose.ShowDisabled = false;

                WindowContent.BorderBrush = (SolidColorBrush)Application
                    .Current.Resources["Window.Active.Content.BorderBrush"];
            }
            else
            {
                WindowFrame.Background = (SolidColorBrush)Application
                    .Current.Resources["Window.Inctive.Frame.Background"];
                WindowFrame.BorderBrush = (SolidColorBrush)Application
                    .Current.Resources["Window.Inactive.Frame.BorderBrush"];

                WindowMinimise.ShowDisabled = true;
                WindowMaximise.ShowDisabled = true;
                WindowRestore.ShowDisabled = true;
                WindowClose.ShowDisabled = true;

                WindowContent.BorderBrush = (SolidColorBrush)Application
                    .Current.Resources["Window.Inactive.Content.BorderBrush"];
            }
        }

        private T GetTemplateChild<T>(string childName) where T : DependencyObject
        {
            return (T)GetTemplateChild(childName);
        }
    }
}
