using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CouplandUI
{
    /// <summary>
    /// Interaction logic for RoundedThumbnailCheckbox.xaml
    /// </summary>
    public partial class RoundedThumbnailCheckbox : UserControl
    {
        public RoundedThumbnailCheckbox()
        {
            InitializeComponent();
            Margin = new Thickness(5, 5, 5, 5);

            ImageBrush background_brush = new ImageBrush(
                Utilities.BitmapSourceFromString(
                    @"C:\Users\Michael\Documents\2015 Projects\JAMPLean\JAMPLean\Scene_Capture\SE_1040.jpg",
                    BitmapCreateOptions.DelayCreation
                )
            );
            background_brush.Stretch = Stretch.UniformToFill;
            Text.Content = "Emma";
            Text.Background = background_brush;
            Text.VerticalContentAlignment = VerticalAlignment.Bottom;
            Text.HorizontalContentAlignment = HorizontalAlignment.Center;

        }
    }
}
