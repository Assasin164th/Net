using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.IO;

namespace Library.classes
{
    internal class Image
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
