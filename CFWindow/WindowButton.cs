using System;
using System.Windows;
using System.Windows.Controls;

namespace CFWindow
{
    public class WindowButton : Button
    {
        // Added to allow the disabled styling to be shown without the button being disabled
        public bool ShowDisabled
        {
            get { return (bool)GetValue(ShowDisabledProperty); }
            set { SetValue(ShowDisabledProperty, value); }
        }

        public static readonly DependencyProperty ShowDisabledProperty
            = DependencyProperty.Register("ShowDisabled", typeof(bool), typeof(WindowButton),
            new PropertyMetadata(false));
    }
}
