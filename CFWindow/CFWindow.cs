using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;

namespace CFWindow
{
    public partial class CFWindow : Window
    {
        private IntPtr WindowHandle;
        private Point MaximisePosition;

        private Border WindowFrame;
        private Border WindowContent;
        private Image WindowIcon;
        private TextBlock WindowTitle;
        private StackPanel WindowButtons;
        private WindowButton WindowMinimise;
        private WindowButton WindowMaximise;
        private WindowButton WindowRestore;
        private WindowButton WindowClose;

        public CFWindow()
        {
            DataContext = this;

            // Set the style so we don't have to do that manually in XAML
            Style = (Style)Application.Current.Resources["CFWindow"];
        }

        public override void OnApplyTemplate()
        {
            WindowFrame = (Border)GetTemplateChild("WindowFrame");
            WindowContent = (Border)GetTemplateChild("WindowContent");
            WindowIcon = (Image)GetTemplateChild("WindowIcon");
            WindowTitle = (TextBlock)GetTemplateChild("WindowTitle");
            WindowButtons = (StackPanel)GetTemplateChild("WindowButtons");
            WindowMinimise = (WindowButton)GetTemplateChild("WindowMinimize");
            WindowMaximise = (WindowButton)GetTemplateChild("WindowMaximize");
            WindowRestore = (WindowButton)GetTemplateChild("WindowRestore");
            WindowClose = (WindowButton)GetTemplateChild("WindowClose");

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
            StateChanged += CFWindow_StateChanged;
            Activated += CFWindow_ActiveChanged;
            Deactivated += CFWindow_ActiveChanged;

            WindowMinimise.Click += WindowMinimize_Click;
            WindowMaximise.Click += WindowMaximize_Click;
            WindowRestore.Click += WindowRestore_Click;
            WindowClose.Click += WindowClose_Click;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            WindowHandle = new WindowInteropHelper(this).Handle;

            HwndSource windowHandle = HwndSource.FromHwnd(WindowHandle);
            if (windowHandle != null)
                windowHandle.AddHook(new HwndSourceHook(WindowProcedure));
        }
        private IntPtr WindowProcedure(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled)
        {
            switch ((uint)msg)
            {
                // The OS maximises windows by making them slightly larger than the screen in order to hide the resize
                // borders. This message provides the default maximise position, which if negative tells us how big the
                // overhang is (allowing it to be negated later on)
                case Helpers.WM_GETMINMAXINFO:
                    {
                        Helpers.MINMAXINFO minMaxInfo = (Helpers.MINMAXINFO)Marshal.PtrToStructure(lparam,
                            typeof(Helpers.MINMAXINFO));

                        MaximisePosition = new Point(minMaxInfo.ptMaxPosition.X, minMaxInfo.ptMaxPosition.Y);
                        if (WindowState == WindowState.Maximized)
                            ApplyWindowState();

                        break;
                    }
            }

            return IntPtr.Zero;
        }

        private void CFWindow_StateChanged(object sender, EventArgs e)
        {
            ApplyWindowState();
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

                Point corrected = Helpers.TransformToPixels(-MaximisePosition.X, -MaximisePosition.Y);
                WindowFrame.BorderThickness = new Thickness(corrected.X, corrected.Y, corrected.X, corrected.Y);
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

                    GlassFrameThickness = new Thickness(0, 0, 0, 1),
                    CornerRadius = new CornerRadius(0)
                });

                WindowFrame.Margin = new Thickness(0);
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
    }
}
