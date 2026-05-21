using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace FishFirm164
{
    public partial class Products
    {
        public string MainImageUrl { get; set; }

        public BitmapImage ImgSource
        {
            get
            {
                if (string.IsNullOrEmpty(MainImageUrl))
                    return null;
                try
                {
                    string fullPath = Path.Combine(Environment.CurrentDirectory, MainImageUrl);
                    if (!File.Exists(fullPath))
                        return null;
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}