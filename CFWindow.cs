using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace CFWindow
{
    public class CFWindow : Window
    {
        static CFWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CFWindow), new FrameworkPropertyMetadata(typeof(CFWindow)));
        }

        public override void OnApplyTemplate()
        {
            StateChanged += CFWindow_StateChanged;
            ((Button)GetTemplateChild("PART_Minimize")).Click += WindowMinimize_Click;
            ((Button)GetTemplateChild("PART_Maximize")).Click += WindowMaximize_Click;
            ((Button)GetTemplateChild("PART_Close")).Click += WindowClose_Click;

            RenderWindowState();
        }

        private void CFWindow_StateChanged(object sender, EventArgs e)
        {
            RenderWindowState();
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

        private void RenderWindowState()
        {
            Border frame = (Border)GetTemplateChild("PART_Frame");
            Border content = (Border)GetTemplateChild("PART_Content");
            WindowChrome chrome = WindowChrome.GetWindowChrome(this);

            if (WindowState == WindowState.Maximized)
            {
                // The OS maximises windows by making them slightly larger than the screen in order to
                // hide the border. This overenlargement is negated here in order to create a clean slate
                // where (0,0) in the outermost element of the window is always at (0,0) on the screen.
                frame.Margin = Utilities.MaximisedWindowOffset();

                chrome.ResizeBorderThickness = new Thickness(0);
                chrome.CaptionHeight = frame.BorderThickness.Top + content.Margin.Top +
                    content.BorderThickness.Top + 8;
            }
            else if (WindowState == WindowState.Normal)
            {
                frame.Margin = new Thickness(0);

                Thickness resizeBorder = new Thickness(
                    frame.BorderThickness.Left + content.Margin.Left + content.BorderThickness.Left,
                    frame.BorderThickness.Top + content.Margin.Top + content.BorderThickness.Top,
                    frame.BorderThickness.Right + content.Margin.Right + content.BorderThickness.Right,
                    frame.BorderThickness.Bottom + content.Margin.Bottom + content.BorderThickness.Bottom);

                if (resizeBorder.Top > 8)
                    resizeBorder.Top = 8;

                chrome.ResizeBorderThickness = resizeBorder;
                chrome.CaptionHeight = frame.BorderThickness.Top + content.Margin.Top +
                    content.BorderThickness.Top - 8;
            }
        }
    }
}
