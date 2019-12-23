using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shell;
using Screen = System.Windows.Forms.Screen;

namespace CFWindow
{
    public class CFWindow : Window
    {
        private Size PreviousScreen;

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
            // Set the style so we don't have to do that in XAML
            Style = (Style)Application.Current.Resources["CFWindow"];
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
            WindowMinimise = GetTemplateChild<WindowButton>("WindowMinimize");
            WindowMaximise = GetTemplateChild<WindowButton>("WindowMaximize");
            WindowRestore = GetTemplateChild<WindowButton>("WindowRestore");
            WindowClose = GetTemplateChild<WindowButton>("WindowClose");
            
            ApplyWindowStateStyle();
            ApplyWindowActiveStyle();

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
            Loaded += new RoutedEventHandler(Window_Loaded);
            SizeChanged += new SizeChangedEventHandler(Window_SizeChanged);
            StateChanged += new EventHandler(Window_StateChanged);
            Activated += Window_ActiveChanged;
            Deactivated += Window_ActiveChanged;

            SystemEvents.DisplaySettingsChanged
                += new EventHandler(SystemEvents_DisplaySettingsChanged);

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

            ApplyWindowStateStyle();
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState != WindowState.Maximized) return;

            Screen screen = Screen.FromHandle(new WindowInteropHelper(this).Handle);

            // Window has changed screen while maximised
            if (PreviousScreen.Width != screen.WorkingArea.Width
                || PreviousScreen.Height != screen.WorkingArea.Height)
            {
                PreviousScreen = new Size(screen.WorkingArea.Width, screen.WorkingArea.Height);
                OnStateChanged(new EventArgs());
            }
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            ApplyWindowStateStyle();
        }
        private void Window_ActiveChanged(object sender, EventArgs e)
        {
            ApplyWindowActiveStyle();
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
            WindowState = WindowState.Minimized;
        }
        private void WindowMaximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleWindowState();
        }
        private void WindowRestore_Click(object sender, RoutedEventArgs e)
        {
            ToggleWindowState();
        }
        private void WindowClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ToggleWindowState()
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }

        private void ApplyWindowStateStyle()
        {
            if (WindowState == WindowState.Minimized) return;
            else if (WindowState == WindowState.Maximized)
            {
                // Switch to the style resources for the maximised window state
                WindowFrame.BorderThickness = (Thickness)Application
                    .Current.Resources["Window.Maximised.Frame.BorderThickness"];
                WindowIcon.Margin = (Thickness)Application
                    .Current.Resources["Window.Maximised.Icon.Margin"];
                WindowTitle.Margin = (Thickness)Application
                    .Current.Resources["Window.Maximised.Title.Margin"];
                WindowTitle.FontSize = (double)Application
                    .Current.Resources["Window.Maximised.Title.FontSize"];
                WindowButtons.Margin = (Thickness)Application
                    .Current.Resources["Window.Maximised.Buttons.Margin"];
                WindowButtons.Height = (double)Application
                    .Current.Resources["Window.Maximised.Buttons.Height"];
                WindowContent.Margin = (Thickness)Application
                    .Current.Resources["Window.Maximised.Content.Margin"];
                WindowContent.BorderThickness = (Thickness)Application
                    .Current.Resources["Window.Maximised.Content.BorderThickness"];

                if (ResizeMode != ResizeMode.NoResize)
                {
                    WindowMaximise.Visibility = Visibility.Collapsed;
                    WindowRestore.Visibility = Visibility.Visible;
                }

                // Set the WindowChrome (for system-provided resize, drag, etc.)
                WindowChrome.SetWindowChrome(this, new WindowChrome
                {
                    ResizeBorderThickness = (Thickness)Application
                        .Current.Resources["Window.Chrome.ResizeBorderThickness"],

                    CaptionHeight = ((Thickness)Application.Current.Resources[
                            "Window.Maximised.Frame.BorderThickness"]).Top +
                        ((Thickness)Application.Current.Resources[
                            "Window.Maximised.Content.Margin"]).Top +
                        ((Thickness)Application.Current.Resources[
                            "Window.Maximised.Content.BorderThickness"]).Top - 1,
                    GlassFrameThickness = new Thickness(1)
                });

                WindowFrame.Margin = Helpers.MarginForDPI();
            }
            else if (WindowState == WindowState.Normal)
            {
                // Switch to the style resources for the maximised window state
                WindowFrame.BorderThickness = (Thickness)Application
                    .Current.Resources["Window.Normal.Frame.BorderThickness"];
                WindowIcon.Margin = (Thickness)Application
                    .Current.Resources["Window.Normal.Icon.Margin"];
                WindowTitle.Margin = (Thickness)Application
                    .Current.Resources["Window.Normal.Title.Margin"];
                WindowTitle.FontSize = (double)Application
                    .Current.Resources["Window.Normal.Title.FontSize"];
                WindowButtons.Margin = (Thickness)Application
                    .Current.Resources["Window.Normal.Buttons.Margin"];
                WindowButtons.Height = (double)Application
                    .Current.Resources["Window.Normal.Buttons.Height"];
                WindowContent.Margin = (Thickness)Application
                    .Current.Resources["Window.Normal.Content.Margin"];
                WindowContent.BorderThickness = (Thickness)Application
                    .Current.Resources["Window.Normal.Content.BorderThickness"];

                if (ResizeMode != ResizeMode.NoResize)
                {
                    WindowMaximise.Visibility = Visibility.Visible;
                    WindowRestore.Visibility = Visibility.Collapsed;
                }

                // Set the WindowChrome (for system-provided resize, drag, etc.)
                WindowChrome.SetWindowChrome(this, new WindowChrome
                {
                    ResizeBorderThickness = (Thickness)Application
                        .Current.Resources["Window.Chrome.ResizeBorderThickness"],

                    CaptionHeight = ((Thickness)Application.Current.Resources[
                            "Window.Normal.Frame.BorderThickness"]).Top +
                        ((Thickness)Application.Current.Resources[
                            "Window.Normal.Content.Margin"]).Top +
                        ((Thickness)Application.Current.Resources[
                            "Window.Normal.Content.BorderThickness"]).Top - 8,
                    GlassFrameThickness = new Thickness(1)
                });

                WindowFrame.Margin = new Thickness(0);
            }
        }
        private void ApplyWindowActiveStyle()
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
                    .Current.Resources["Window.Inctive.Frame.BorderBrush"];

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
