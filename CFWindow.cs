using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace CFWindow
{
    /// <summary>
    /// Represents a WPF <see cref="Window"/> with a custom frame style.
    /// </summary>
    public class CFWindow : Window
    {
        static CFWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(CFWindow), new FrameworkPropertyMetadata(typeof(CFWindow)));
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CFWindow"/> class.
        /// </summary>
        public CFWindow() { }

        /// <summary>
        /// Attaches various event handlers to the window.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Loaded += CFWindow_Loaded;
            StateChanged += CFWindow_StateChanged;

            ((Button)GetTemplateChild("PART_Minimise")).Click += PART_Minimise_Click;
            ((Button)GetTemplateChild("PART_Maximise")).Click += PART_Maximise_Click;
            ((Button)GetTemplateChild("PART_Close")).Click += PART_Close_Click;

            SetStateDependentProps();
        }

        private void CFWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetStateDependentProps();
        }

        private void CFWindow_StateChanged(object sender, EventArgs e)
        {
            SetStateDependentProps();
        }

        private void PART_Minimise_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void PART_Maximise_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }

        private void PART_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Sets various properties of the window that depend on its current state.
        /// </summary>
        private void SetStateDependentProps()
        {
            Border frame = (Border)GetTemplateChild("PART_Frame");
            Border content = (Border)GetTemplateChild("PART_Content");
            WindowChrome chrome = WindowChrome.GetWindowChrome(this);

            if (WindowState == WindowState.Maximized)
            {
                // The OS maximises windows by making them slightly larger than the screen in order to
                // hide the border. This overhang needs to be negated
                frame.Margin = GetMaximiseCorrection();

                chrome.ResizeBorderThickness = new Thickness(0);
                chrome.CaptionHeight = frame.BorderThickness.Top + content.Margin.Top +
                    content.BorderThickness.Top + frame.Margin.Top;
            }
            else if (WindowState == WindowState.Normal)
            {
                frame.Margin = new Thickness(0);

                Thickness resizeBorder = new Thickness(
                    frame.BorderThickness.Left + content.Margin.Left + content.BorderThickness.Left,
                    frame.BorderThickness.Top + content.Margin.Top + content.BorderThickness.Top,
                    frame.BorderThickness.Right + content.Margin.Right + content.BorderThickness.Right,
                    frame.BorderThickness.Bottom + content.Margin.Bottom + content.BorderThickness.Bottom);

                // The top also contains the draggable area so the resize border must be limited in size
                if (resizeBorder.Top > 8)
                    resizeBorder.Top = 8;

                chrome.ResizeBorderThickness = resizeBorder;
                chrome.CaptionHeight = frame.BorderThickness.Top + content.Margin.Top +
                    content.BorderThickness.Top - resizeBorder.Top;
            }
        }

        /// <summary>
        /// Returns a value that corrects for the overhang, by the OS, of a window past the edges of the screen when
        /// it is maximised.
        /// </summary>
        private static Thickness GetMaximiseCorrection()
        {
            int padding = NativeMethods.GetSystemMetrics(
                NativeMethods.SystemMetric.SM_CXPADDEDBORDER);

            IntPtr context = NativeMethods.GetDeviceContext(IntPtr.Zero);

            try
            {
                double dpiX = NativeMethods.GetDeviceCaps(
                    context, NativeMethods.DeviceCap.LOGPIXELSX) / 96f;

                double vertical = (NativeMethods.GetSystemMetrics(
                    NativeMethods.SystemMetric.SM_CXFRAME) + padding) / dpiX;

                double dpiY = NativeMethods.GetDeviceCaps(
                    context, NativeMethods.DeviceCap.LOGPIXELSX) / 96f;

                double horizontal = (NativeMethods.GetSystemMetrics(
                    NativeMethods.SystemMetric.SM_CYFRAME) + padding) / dpiY;

                return new Thickness(vertical, horizontal, vertical, horizontal);
            }
            finally
            {
                NativeMethods.ReleaseDeviceContext(IntPtr.Zero, context);
            }
        }
    }
}
