using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Controls;
using System.Reflection;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CouplandUI
{
    public class Utilities
    {
        #region Scale Image Methods & Overloads
        public enum PictureOrientation { None, Landscape, Portrait };
        public static Bitmap ScalePicture(FileInfo picturesource, ushort maxdimension, PictureOrientation force_orientation = PictureOrientation.None)
        {
            return ScalePicture(picturesource.FullName, maxdimension, force_orientation);
        }
        public static Bitmap ScalePicture(string picturesource, ushort maxdimension, PictureOrientation force_orientation = PictureOrientation.None)
        {
            using (Bitmap picture_from_file = Bitmap.FromFile(picturesource) as Bitmap)
            {
                if (force_orientation == PictureOrientation.Portrait || picture_from_file.Height > picture_from_file.Width) { return ScalePortraitPicture(picturesource, maxdimension); }
                else if (force_orientation == PictureOrientation.Landscape || picture_from_file.Height < picture_from_file.Width) { return ScaleLandscapePicture(picturesource, maxdimension); }
            }
            throw new PictureManipulationException(
                string.Format(
                    @"Unknown error occured in {0}, picturesource:{1}, maxdimension:{2}",
                    "ScalePicture",
                    picturesource,
                    maxdimension
                )
            );
        }

        private static Bitmap ScalePortraitPicture(FileInfo picturesource, ushort maxdimension)
        {
            return ScalePortraitPicture(picturesource.FullName, maxdimension);
        }
        private static Bitmap ScalePortraitPicture(string picturesource, ushort maxdimension)
        {
            using (Bitmap picture_from_file = Bitmap.FromFile(picturesource) as Bitmap)
            {
                float width = maxdimension;
                float owidth = (float)picture_from_file.Width;
                float multiplier = width / owidth;
                float height = (float)picture_from_file.Height * multiplier;
                ushort scale_width = GetIntFloor(width);
                ushort scale_height = GetIntFloor(height);
                return new Bitmap(picture_from_file, new Size(scale_width, scale_height));
            }
        }
        private static Bitmap ScaleLandscapePicture(FileInfo picturesource, ushort maxdimension)
        {
            return ScaleLandscapePicture(picturesource.FullName, maxdimension);
        }
        private static Bitmap ScaleLandscapePicture(string picturesource, ushort maxdimension)
        {
            using (Bitmap picture_from_file = Bitmap.FromFile(picturesource) as Bitmap)
            {
                float height = maxdimension;
                float oheight = (float)picture_from_file.Height;
                float multiplier = height / oheight;
                float width = (float)picture_from_file.Width * multiplier;
                ushort scale_width = GetIntFloor(width);
                ushort scale_height = GetIntFloor(height);
                return new Bitmap(picture_from_file, new Size(scale_width, scale_height));
            }
        }
        #endregion

        #region Crop Image Methods & Overloads
        public static Bitmap CropCapture(FileInfo picturefile, int maxdimension)
        {
            return CropCapture(Bitmap.FromFile(picturefile.FullName) as Bitmap, maxdimension);
        }
        public static Bitmap CropCapture(string picturefile, int maxdimension)
        {
            return CropCapture(Bitmap.FromFile(picturefile) as Bitmap, maxdimension);
        }
        public static Bitmap CropCapture(Bitmap picture_bitmap, int maxdimension)
        {
            using (picture_bitmap)
            {
                if (picture_bitmap.Width > picture_bitmap.Height) { return CropLandscapePicture(picture_bitmap, maxdimension); }
                else { return CropPortraitPicture(picture_bitmap, maxdimension); }
            }
        }
        private static Bitmap CropLandscapePicture(Bitmap picture, int maxdimension)
        {
            using (picture)
            {
                System.Windows.Int32Rect converted_crop = new System.Windows.Int32Rect();
                converted_crop.X = (picture.Width - maxdimension) / 2;
                converted_crop.Y = 0;
                Rectangle crop_box = new Rectangle(converted_crop.X, converted_crop.Y, maxdimension, maxdimension);
                return picture.Clone(crop_box, picture.PixelFormat);
            }
        }
        private static Bitmap CropPortraitPicture(Bitmap picture, int maxdimension)
        {
            using (picture)
            {
                System.Windows.Int32Rect converted_crop = new System.Windows.Int32Rect();
                converted_crop.X = 0;
                converted_crop.Y = (picture.Height - maxdimension) / 2; ;
                Rectangle crop_box = new Rectangle(converted_crop.X, converted_crop.Y, maxdimension, maxdimension);
                return picture.Clone(crop_box, picture.PixelFormat);
            }
        }
        #endregion

        #region Sort Images
        /*
         * Contrived properties to show/implement sorting
         */
        public ushort ID;
        public string Name;
        public class ComparePictures : IComparer<Utilities>
        {
            public int Compare(Utilities s1, Utilities s2)
            {
                return (string.Compare(s1.Name, s2.Name));
            }
        }
        public void ContrivedSort()
        {
            List<Utilities> pictures = new List<Utilities>();
            for (ushort i = 0; i < 20; i++)
            {
                Utilities picture_instance = new Utilities();
                picture_instance.ID = i;
                picture_instance.Name = Guid.NewGuid().ToString();
                pictures.Add(picture_instance);
            }
            pictures.Sort(new ComparePictures());
        }
        #endregion

        #region Objects to substitute to prevent name ambiguity
        #region Drawing Versions        
        public static System.Drawing.Brush DBrush;
        public static System.Drawing.Rectangle DRectangle;
        public static System.Drawing.Image DImage;
        public static System.Drawing.Size DSize;
        #endregion

        #region WPF Versions
        public static System.Windows.Media.Brush WBrush;
        public static System.Windows.Shapes.Rectangle WRectangle;
        public static System.Windows.Controls.Image WImage;
        public static System.Windows.Size WSize;
        #endregion
        #endregion

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

        #region Method to Get size of UIElement May or May Not work depending on when it is called (during window layout)

        public static double GetElementWidth(System.Windows.FrameworkElement element)
        {
            System.Drawing.Size size = GetElementSize(element);
            return GetIntCeiling(element.DesiredSize.Width + element.Margin.Left + element.Margin.Right);
        }
        public static double GetElementHeight(System.Windows.FrameworkElement element)
        {
            System.Drawing.Size size = GetElementSize(element);
            return GetIntCeiling(element.DesiredSize.Height + element.Margin.Top + element.Margin.Bottom);
        }

        public static System.Drawing.Size GetElementSize(System.Windows.FrameworkElement element)
        {
            element.Measure(new System.Windows.Size(double.PositiveInfinity, double.PositiveInfinity));
            ushort width = GetIntCeiling(element.DesiredSize.Width);
            ushort height = GetIntCeiling(element.DesiredSize.Height);
            return new System.Drawing.Size(width, height);
        }
        #endregion

        #region Numeric Methods
        public static ushort GetIntFloor(object decimal_value)
        {
            string object_type = decimal_value.GetType().Name;
            if (object_type == "float" || object_type == "double" || object_type == "decimal")
            {
                return Convert.ToUInt16(Math.Floor((decimal)decimal_value));
            }
            return Convert.ToUInt16(decimal_value);
        }
        public static ushort GetIntCeiling(object decimal_value)
        {            
            string object_type = decimal_value.GetType().Name;
            if (object_type == "float" || object_type == "double" || object_type == "decimal")
            {
                return Convert.ToUInt16(Math.Ceiling((decimal)decimal_value));
            }
            return Convert.ToUInt16(decimal_value);
        }
        #endregion

        #region String & File Methods
        public static FileInfo MoveFile(string sourcepath, string targetdirectory, bool overwrite = false)
        {
            return MoveFile(new FileInfo(sourcepath), new DirectoryInfo(targetdirectory), overwrite);
        }
        public static FileInfo MoveFile(FileInfo sourcefileinfo, DirectoryInfo targetdirectoryinfo, bool overwrite = false)
        {
            string separator = targetdirectoryinfo.FullName.EndsWith("\\") ? "" : "\\";
            string target_file = string.Format(@"{0}{1}{2}", targetdirectoryinfo.Parent.FullName, separator, sourcefileinfo.Name);
            if (File.Exists(target_file) && !overwrite)
            {
                throw new FileExistsException(string.Format(@"Target file {0} already exists and overwrite flag is set to false", target_file));
            }
            File.Move(sourcefileinfo.FullName, target_file);
            return new FileInfo(target_file);
        }
        public static string GetFileNameWithoutExtension(string filename)
        {
            return GetFileNameWithoutExtension(new FileInfo(filename));
        }
        public static string GetFileNameWithoutExtension(FileInfo filename)
        {
            return filename.Name.Replace(filename.Extension, "");
        }
        public static List<FileInfo> GetFileInfos(string directory, string pattern, SearchOption searchoption, ushort limit = ushort.MinValue)
        {
            return GetFileInfos(new DirectoryInfo(directory), pattern, searchoption, limit);
        }
        public static List<FileInfo> GetFileInfos(DirectoryInfo directory, string pattern, SearchOption searchoption, ushort limit = ushort.MinValue)
        {
            List<FileInfo> files = directory.GetFiles(pattern, searchoption).ToList<System.IO.FileInfo>();
            if (files.Any() && limit > 0)
            {
                return files.TakeWhile(x => x.Exists).ToList<FileInfo>();
            }
            return files;
        }
        public static List<string> ReadAllFileLines(string filename)
        {
            List<string> lines = new List<string>();
            lines.AddRange(File.ReadAllLines(filename));
            return lines;
        }
        #endregion

        #region JSON Serialization
        /*
            * Requires 
            * 
            * Reference to System.Runtime.Serialization
            * using System.Runtime.Serialization;
            * using System.Runtime.Serialization.Json;
        */
        public bool Serialize(object objecttoserialize, string targetfile)
        {
            return Serialize(objecttoserialize, new FileInfo(targetfile));
        }
        public bool Serialize(object objecttoserialize, FileInfo targetfile)
        {
            bool result = false;
            DataContractJsonSerializer json_serializer = new DataContractJsonSerializer(objecttoserialize.GetType());
            using (FileStream data_stream = new FileStream(targetfile.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                json_serializer.WriteObject(data_stream, objecttoserialize);
                result = true;
            }
            return result;
        }

        public object DeSerialize(object objecttodeserialize, string targetfile)
        {
            return DeSerialize(objecttodeserialize, new FileInfo(targetfile));
        }
        public object DeSerialize(object objecttodeserialize, FileInfo targetfile)
        {
            object result = null;
            if (!File.Exists(targetfile.FullName))
            {
                throw new SerializationException(string.Format(@"Cannot deserialize, {0} file does not exist.", targetfile.FullName));
            }
            DataContractJsonSerializer json_serializer = new DataContractJsonSerializer(objecttodeserialize.GetType());
            using (FileStream data_stream = new FileStream(targetfile.FullName, FileMode.Open, FileAccess.Read))
            {
                result = json_serializer.ReadObject(data_stream);
            }
            return result;
        }
        #endregion

        #region Holy Mother of God, can get isplaying....
        public static MediaState GetMediaState(MediaElement myMedia)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = hlp.GetValue(myMedia);
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)stateField.GetValue(helperObject);
            return state;
        }
        #endregion  

        #region Animations
        public static void SwapBorders(Border hiding, Border showing, string[] files, double duration = 2)
        {
            showing.Opacity = 0;
            DoubleAnimation fade_out = new DoubleAnimation(
                hiding.Opacity, 0, TimeSpan.FromSeconds(duration)
            );
            fade_out.Completed += delegate
            {
                showing.Visibility = System.Windows.Visibility.Visible;
                hiding.Visibility = System.Windows.Visibility.Hidden;
                DoubleAnimation fade_in = new DoubleAnimation(
                    showing.Opacity, 1, TimeSpan.FromSeconds(duration)
                );
                showing.BeginAnimation(Canvas.OpacityProperty, fade_in);
            };
            hiding.BeginAnimation(Canvas.OpacityProperty, fade_out);
        }        
        #endregion

        #region get random image
        private static Random rnd = new Random();
        public static BitmapImage GetRandomBackground(string[] bgs)
        {
            string random_bg = bgs[rnd.Next(0, bgs.Length)];
            BitmapImage bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.UriSource = new Uri(random_bg);
            bmp.EndInit();      
            return bmp;            
        }
        #endregion
    }
}