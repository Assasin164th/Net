using Library.classes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Library
{
    public partial class LoginWindow : Window
    {
        private int failedAttempts = 0;
        private string currentCaptchaText = "";

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Введите логин и пароль.";
                return;
            }

            // Если капча активна - проверяем её
            if (captchaPanel.Visibility == Visibility.Visible)
            {
                if (string.IsNullOrEmpty(txtCaptcha.Text.Trim()))
                {
                    txtError.Text = "Введите код с картинки.";
                    return;
                }
                if (txtCaptcha.Text.Trim() != currentCaptchaText)
                {
                    txtError.Text = "Неверный код. Попробуйте снова.";
                    GenerateCaptcha(); // Обновляем капчу при ошибке
                    txtCaptcha.Clear();
                    return;
                }
            }

            var user = AppConnect.context.Users
                .Where(u => u.Login == login && u.Password == password)
                .Select(u => new { u.Id, u.Login, RoleName = u.Roles.Name })
                .FirstOrDefault();

            if (user != null)
            {
                MainWindow main = new MainWindow(user.Id, user.Login, user.RoleName);
                main.Show();
                this.Close();
            }
            else
            {
                failedAttempts++;
                if (failedAttempts >= 1 && captchaPanel.Visibility != Visibility.Visible)
                {
                    ShowCaptcha();
                }
                else if (captchaPanel.Visibility == Visibility.Visible)
                {
                    GenerateCaptcha(); // Обновить капчу при повторной ошибке
                    txtCaptcha.Clear();
                }
                txtError.Text = "Неверный логин или пароль.";
            }
        }

        private void ShowCaptcha()
        {
            captchaPanel.Visibility = Visibility.Visible;
            GenerateCaptcha();
            // Изменяем высоту окна, чтобы вместить капчу
            this.Height = 500;
        }

        private void GenerateCaptcha()
        {
            // Генерация случайного текста (4-6 символов)
            Random rand = new Random();
            int length = rand.Next(4, 7);
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789";
            char[] stringChars = new char[length];
            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[rand.Next(chars.Length)];
            }
            currentCaptchaText = new string(stringChars);

            // Создаём изображение капчи
            captchaImage.Source = GenerateCaptchaImage(currentCaptchaText);
        }

        private BitmapImage GenerateCaptchaImage(string text)
        {
            int width = 120;
            int height = 50;
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                // Фон с шумом
                Random rand = new Random();
                drawingContext.DrawRectangle(Brushes.WhiteSmoke, null, new System.Windows.Rect(0, 0, width, height));
                for (int i = 0; i < 100; i++)
                {
                    drawingContext.DrawRectangle(
                        new SolidColorBrush(Color.FromArgb(50, (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))),
                        null,
                        new System.Windows.Rect(rand.Next(width), rand.Next(height), 1, 1));
                }
                // Линии
                for (int i = 0; i < 5; i++)
                {
                    drawingContext.DrawLine(
                        new Pen(new SolidColorBrush(Color.FromArgb(80, (byte)rand.Next(256), (byte)rand.Next(256), (byte)rand.Next(256))), 1),
                        new System.Windows.Point(rand.Next(width), rand.Next(height)),
                        new System.Windows.Point(rand.Next(width), rand.Next(height)));
                }
                // Текст
                var formattedText = new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Arial"),
                    20,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(drawingVisual).PixelsPerDip);
                // Смещение для искажения
                double x = (width - formattedText.Width) / 2 + rand.Next(-5, 5);
                double y = (height - formattedText.Height) / 2 + rand.Next(-3, 3);
                drawingContext.DrawText(formattedText, new System.Windows.Point(x, y));
            }

            // Рендерим в BitmapImage
            var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(drawingVisual);
            var bitmapImage = new BitmapImage();
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(renderTarget));
            using (var stream = new System.IO.MemoryStream())
            {
                encoder.Save(stream);
                stream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private void BtnRefreshCaptcha_Click(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
            txtCaptcha.Clear();
        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(0, "Гость", "Guest");
            main.Show();
            this.Close();
        }
    }
}