using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace CouplandUI
{
    /// <summary>
    /// Interaction logic for RoundedImage.xaml
    /// </summary>
    public partial class RoundedImage : UserControl
    {
        public RoundedImage()
        {
            InitializeComponent();
        }
        public RoundedImage(string imagesource)
        {
            InitializeComponent();
            Image.Source = Utilities.BitmapSourceFromString(imagesource);
        }
    }

    public static class Utilities
    {
        #region Overloaded Methods to Get BitmapSource for WPF Image Control
        public static System.Windows.Media.ImageSource BitmapSourceFromString(string source, BitmapCreateOptions createoptions = BitmapCreateOptions.IgnoreImageCache)
        {
            return BitmapSourceFromUri(new Uri(source), createoptions);
        }
        public static System.Windows.Media.ImageSource BitmapSourceFromUri(Uri source, BitmapCreateOptions createoptions = BitmapCreateOptions.IgnoreImageCache)
        {
            var bitmap_image = new System.Windows.Media.Imaging.BitmapImage();
            bitmap_image.BeginInit();
            bitmap_image.UriSource = source;
            bitmap_image.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
            bitmap_image.CreateOptions = createoptions;
            bitmap_image.EndInit();
            return bitmap_image;
            throw new FileNotFoundException(string.Format(@"Argument source {0} does not identify an existing file", source.OriginalString));
        }
        #endregion
    }
}
