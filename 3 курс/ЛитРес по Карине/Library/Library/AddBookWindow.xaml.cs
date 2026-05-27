using Library.classes;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Library
{
    public partial class AddBookWindow : Window
    {
        private string selectedCoverPath = null;

        public AddBookWindow()
        {
            InitializeComponent();
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            cmbPublisher.ItemsSource = AppConnect.context.Publisher.OrderBy(p => p.Name).ToList();
            cmbGenre.ItemsSource = AppConnect.context.Genre.OrderBy(g => g.Name).ToList();
        }

        private void BtnSelectCover_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите обложку книги"
            };
            if (dialog.ShowDialog() == true)
            {
                selectedCoverPath = dialog.FileName;
                ShowCoverImage(selectedCoverPath);
            }
        }

        private void ShowCoverImage(string path)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(path, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                imgCover.Source = bitmap;
            }
            catch
            {
                imgCover.Source = GetPlaceholder();
            }
        }

        private BitmapImage GetPlaceholder()
        {
            string placeholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "placeholder.png");
            if (File.Exists(placeholderPath))
            {
                try
                {
                    var img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(placeholderPath, UriKind.Absolute);
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                    return img;
                }
                catch { }
            }
            return null;
        }

        private void Txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null)
                tb.Background = Brushes.White;
        }

        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            if (cmb != null)
                cmb.Background = Brushes.White;
        }

        private bool ValidateFields()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                txtName.Background = Brushes.LightCoral;
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                txtAuthor.Background = Brushes.LightCoral;
                isValid = false;
            }
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                txtPrice.Background = Brushes.LightCoral;
                isValid = false;
            }
            if (!int.TryParse(txtStockCount.Text, out int stock) || stock < 0)
            {
                txtStockCount.Background = Brushes.LightCoral;
                isValid = false;
            }
            if (cmbPublisher.SelectedItem == null)
            {
                cmbPublisher.Background = Brushes.LightCoral;
                isValid = false;
            }
            if (cmbGenre.SelectedItem == null)
            {
                cmbGenre.Background = Brushes.LightCoral;
                isValid = false;
            }

            if (!isValid)
                MessageBox.Show("Заполните все обязательные поля.\nЦена и остаток не могут быть отрицательными.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);

            return isValid;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateFields())
                return;

            string coverRelativePath = null;
            if (!string.IsNullOrEmpty(selectedCoverPath))
            {
                string fileName = Path.GetFileName(selectedCoverPath);
                string destDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Covers");
                Directory.CreateDirectory(destDir);
                string destPath = Path.Combine(destDir, fileName);
                try
                {
                    File.Copy(selectedCoverPath, destPath, true);
                    coverRelativePath = "Covers/" + fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка копирования обложки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var newBook = new Book
            {
                Name = txtName.Text.Trim(),
                Author = txtAuthor.Text.Trim(),
                Price = decimal.Parse(txtPrice.Text),
                StockCount = int.Parse(txtStockCount.Text),
                IsAvailable = chkIsAvailable.IsChecked == true,
                PublisherId = (int)cmbPublisher.SelectedValue,
                CoverImage = coverRelativePath
            };

            var selectedGenre = cmbGenre.SelectedItem as Genre;
            newBook.Genre = new System.Collections.Generic.HashSet<Genre> { selectedGenre };

            try
            {
                AppConnect.context.Book.Add(newBook);
                AppConnect.context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}