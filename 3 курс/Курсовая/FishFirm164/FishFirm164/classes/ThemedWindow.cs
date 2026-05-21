using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace FishFirm164
{
    public class ThemedWindow : Window
    {
        public ThemedWindow()
        {
            if (!Resources.MergedDictionaries.Any(d => d.Source?.OriginalString?.Contains("DarkTheme.xaml") == true))
            {
                var dict = new ResourceDictionary
                {
                    Source = new Uri("DarkTheme.xaml", UriKind.Relative)
                };
                Resources.MergedDictionaries.Add(dict);
            }
            Background = (Brush)FindResource("MainBackground");
            Foreground = (Brush)FindResource("TextForeground");
        }
    }
}