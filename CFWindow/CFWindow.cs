using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Screen = System.Windows.Forms.Screen;

namespace CFWindow
{
    public partial class CFWindow : Window
    {
        private WindowState PreviousState;
        private Size PreviousScreen;

        private Border WindowFrame;
        private Border WindowContent;
        private Image WindowIcon;
        private TextBlock WindowTitle;
        private StackPanel WindowButtons;
        private Button WindowMinimise;
        private Button WindowMaximise;
        private Button WindowRestore;
        private Button WindowClose;

        static CFWindow()
        {
            // Needed as part of automatically using Themes/Generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CFWindow),
                new FrameworkPropertyMetadata(typeof(CFWindow)));
        }

        public CFWindow()
        {
            DataContext = this;
        }

        public override void OnApplyTemplate()
        {
            // Get visual tree element references
            WindowFrame = GetTemplateChild<Border>("WindowFrame");
            WindowContent = GetTemplateChild<Border>("WindowContent");
            WindowIcon = GetTemplateChild<Image>("WindowIcon");
            WindowTitle = GetTemplateChild<TextBlock>("WindowTitle");
            WindowButtons = GetTemplateChild<StackPanel>("WindowButtons");
            WindowMinimise = GetTemplateChild<Button>("WindowMinimize");
            WindowMaximise = GetTemplateChild<Button>("WindowMaximize");
            WindowRestore = GetTemplateChild<Button>("WindowRestore");
            WindowClose = GetTemplateChild<Button>("WindowClose");

            // Apply property values
            WindowFrame.Background = CFFrameBackground;
            WindowFrame.BorderBrush = CFFrameBorderColour;
            WindowContent.BorderBrush = CFContentBorderColor;

            // Hide elements based on properties
            if (this.ResizeMode == ResizeMode.NoResize)
            {
                WindowMinimise.Visibility = Visibility.Collapsed;
                WindowMaximise.Visibility = Visibility.Collapsed;
                WindowRestore.Visibility = Visibility.Collapsed;
            }
            else if (this.ResizeMode == ResizeMode.CanMinimize)
            {
                WindowMaximise.IsEnabled = false;
                WindowRestore.IsEnabled = false;
            }

            // Add event handlers here as they need the visual tree
            base.Loaded += new RoutedEventHandler(Window_Loaded);
            base.SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);
            base.StateChanged += new EventHandler(Window_StateChanged);
            SystemEvents.DisplaySettingsChanged
                += new EventHandler(SystemEvents_DisplaySettingsChanged);
            this.Activated += Window_Activated;
            this.Deactivated += Window_Deactivated;

            WindowMinimise.Click += WindowMinimize_Click;
            WindowMaximise.Click += WindowMaximize_Click;
            WindowRestore.Click += WindowRestore_Click;
            WindowClose.Click += WindowClose_Click;
            base.OnApplyTemplate();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Store size of screen that window is on
            Screen screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
            PreviousScreen = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height);

            // Configure the window for its initial state
            ApplyWindowState(this.WindowState);
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (base.WindowState == WindowState.Maximized)
            {
                Screen screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);

                if (PreviousScreen.Width != screen.WorkingArea.Width
                    || PreviousScreen.Height != screen.WorkingArea.Height)
                {
                    PreviousScreen = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height);
                    OnStateChanged(new EventArgs());
                }
            }
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            ApplyWindowState(this.WindowState);
            PreviousState = this.WindowState;
        }
        private void Window_Activated(object sender, EventArgs e)
        {
            WindowFrame.Background = CFFrameBackground;
            WindowFrame.BorderBrush = CFFrameBorderColour;
            WindowContent.BorderBrush = CFContentBorderColor;
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            WindowFrame.Background = CFInactiveFrameBackground;
            WindowFrame.BorderBrush = CFInactiveFrameBorderColour;
            WindowContent.BorderBrush = CFInactiveContentBorderColor;
        }
        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            // Update size of screen that window is on and update window
            Screen screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);
            PreviousScreen = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height);
            OnStateChanged(new EventArgs());
        }

        private void WindowMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
        private void WindowMaximize_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleWindowState();
        }
        private void WindowRestore_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleWindowState();
        }
        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private T GetTemplateChild<T>(string childName) where T : DependencyObject
        {
            return (T)base.GetTemplateChild(childName);
        }
        private void ToggleWindowState()
        {
            if (base.WindowState != WindowState.Maximized)
                base.WindowState = WindowState.Maximized;
            else base.WindowState = WindowState.Normal;
        }
        private void ApplyWindowState(WindowState state)
        {
            if (state == WindowState.Maximized)
            {
                WindowFrame.BorderThickness = CFMaximisedFrameBorderThickness;
                WindowIcon.Margin = CFMaximisedIconMargin;
                WindowTitle.Margin = CFMaximisedTitleMargin;
                WindowTitle.FontSize = CFMaximisedTitleFontSize;
                WindowButtons.Margin = CFMaximisedButtonsMargin;
                WindowButtons.Height = CFMaximisedButtonsHeight;
                WindowContent.Margin = CFMaximisedContentMargin;
                WindowContent.BorderThickness = CFMaximisedContentBorderThickess;

                if (this.ResizeMode != ResizeMode.NoResize)
                {
                    WindowMaximise.Visibility = Visibility.Collapsed;
                    WindowRestore.Visibility = Visibility.Visible;
                }

                WindowFrame.Margin = Helpers.MarginForDPI();
            }
            else if (state == WindowState.Normal)
            {
                WindowFrame.BorderThickness = CFRestoredFrameBorderThickness;
                WindowIcon.Margin = CFRestoredIconMargin;
                WindowTitle.Margin = CFRestoredTitleMargin;
                WindowTitle.FontSize = CFRestoredTitleFontSize;
                WindowButtons.Margin = CFRestoredButtonsMargin;
                WindowButtons.Height = CFRestoredButtonsHeight;
                WindowContent.Margin = CFRestoredContentMargin;
                WindowContent.BorderThickness = CFRestoredContentBorderThickess;

                if (this.ResizeMode != ResizeMode.NoResize)
                {
                    WindowMaximise.Visibility = Visibility.Visible;
                    WindowRestore.Visibility = Visibility.Collapsed;
                }

                WindowFrame.Margin = new Thickness(0);
            }
        }
    }
}
