using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace CFWindow
{
    public partial class CFWindow : Window
    {
        public CFWindow()
        {
            // Set the style so we don't have to do that manually in XAML
            Style = (Style)Application.Current.Resources["CFWindow"];
        }

        public override void OnApplyTemplate()
        {
            ApplyWindowState();
            StateChanged += CFWindow_StateChanged;

            ((Button)GetTemplateChild("WindowMinimize")).Click += WindowMinimize_Click;
            ((Button)GetTemplateChild("WindowMaximize")).Click += WindowMaximize_Click;
            ((Button)GetTemplateChild("WindowClose")).Click += WindowClose_Click;
        }

        private void CFWindow_StateChanged(object sender, EventArgs e)
        {
            ApplyWindowState();
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
        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ApplyWindowState()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowChrome.GetWindowChrome(this).ResizeBorderThickness = new Thickness(0);

                WindowChrome.GetWindowChrome(this).CaptionHeight =
                    ((Thickness)Application.Current.Resources["CFWindow.Maximised.Frame.BorderThickness"]).Top +
                    ((Thickness)Application.Current.Resources["CFWindow.Maximised.Content.Margin"]).Top +
                    Helpers.MaximisedWindowOffset().Top;

                ((Border)GetTemplateChild("WindowFrame")).Margin = Helpers.MaximisedWindowOffset();
            }
            else if (WindowState == WindowState.Normal)
            {
                WindowChrome.GetWindowChrome(this).ResizeBorderThickness =
                    (Thickness)Application.Current.Resources["CFWindow.Chrome.ResizeBorderThickness"];

                WindowChrome.GetWindowChrome(this).CaptionHeight =
                    ((Thickness)Application.Current.Resources["CFWindow.Normal.Frame.BorderThickness"]).Top +
                    ((Thickness)Application.Current.Resources["CFWindow.Normal.Content.Margin"]).Top -
                    ((Thickness)Application.Current.Resources["CFWindow.Chrome.ResizeBorderThickness"]).Top;

                ((Border)GetTemplateChild("WindowFrame")).Margin = new Thickness(0);
            }
        }
    }
}
