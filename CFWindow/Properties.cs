using System.Windows;
using System.Windows.Media;

namespace CFWindow
{
    public partial class CFWindow
    {
        #region Static Properties        
        public TextAlignment CFTitleTextAlignment { get; set; } = TextAlignment.Center;
        public SolidColorBrush CFTitleForeground { get; set; } = new SolidColorBrush(Colors.Black);
        public Thickness CFMinimiseButtonMargin { get; set; } = new Thickness(0, 0, 1, 0);
        public double CFMinimiseButtonWidth { get; set; } = 26;
        public Thickness CFMaximiseButtonMargin { get; set; } = new Thickness(0, 0, 1, 0);
        public double CFMaximiseButtonWidth { get; set; } = 26;
        public Thickness CFCloseButtonMargin { get; set; } = new Thickness(0, 0, 0, 0);

        private double _CFCloseButtonWidth = 50;
        public double CFCloseButtonWidth
        {
            get
            {
                if (this.ResizeMode == ResizeMode.NoResize)
                    return CFNoResizeCloseButtonWidth;
                else return _CFCloseButtonWidth;
            }
            set { _CFCloseButtonWidth = value; }
        }

        public double CFNoResizeCloseButtonWidth { get; set; } = 31;

        public SolidColorBrush CFFrameBackground { get; set; }
            = new SolidColorBrush(Color.FromRgb(90, 140, 187));
        public SolidColorBrush CFFrameBorderColour { get; set; }
            = new SolidColorBrush(Color.FromRgb(69, 107, 143));
        public SolidColorBrush CFContentBorderColor { get; set; }
            = new SolidColorBrush(Color.FromRgb(77, 119, 159));

        public SolidColorBrush CFInactiveFrameBackground { get; set; }
            = new SolidColorBrush(Color.FromRgb(235, 235, 235));
        public SolidColorBrush CFInactiveFrameBorderColour { get; set; }
            = new SolidColorBrush(Color.FromRgb(211, 211, 211));
        public SolidColorBrush CFInactiveContentBorderColor { get; set; }
            = new SolidColorBrush(Color.FromRgb(218, 218, 218));
        #endregion

        #region Restored State Properties
        public Thickness CFRestoredFrameBorderThickness { get; set; } = new Thickness(1);
        public Thickness CFRestoredIconMargin { get; set; } = new Thickness(6, 6, 0, 0);
        public Thickness CFRestoredTitleMargin { get; set; } = new Thickness(5, 3, 5, 0);
        public double CFRestoredTitleFontSize { get; set; } = 15;
        public Thickness CFRestoredButtonsMargin { get; set; } = new Thickness(0, 0, 6, 0);
        public double CFRestoredButtonsHeight { get; set; } = 22;
        public Thickness CFRestoredContentMargin { get; set; } = new Thickness(6, 28, 6, 6);
        public Thickness CFRestoredContentBorderThickess { get; set; } = new Thickness(1);
        #endregion

        #region Maximised State Properties
        public Thickness CFMaximisedFrameBorderThickness { get; set; } = new Thickness(0);
        public Thickness CFMaximisedIconMargin { get; set; } = new Thickness(4, 4, 0, 0);
        public Thickness CFMaximisedTitleMargin { get; set; } = new Thickness(5, 2, 5, 0);
        public double CFMaximisedTitleFontSize { get; set; } = 14;
        public Thickness CFMaximisedButtonsMargin { get; set; } = new Thickness(0, 0, 4, 0);
        public double CFMaximisedButtonsHeight { get; set; } = 20;
        public Thickness CFMaximisedContentMargin { get; set; } = new Thickness(0, 24, 0, 0);
        public Thickness CFMaximisedContentBorderThickess { get; set; } = new Thickness(0, 1, 0, 0);
        #endregion
    }
}
