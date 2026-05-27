using Library.classes;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Library
{
    public partial class BookEditWindow : Window, INotifyPropertyChanged
    {
        public Book CurrentBook { get; set; }
        public string WindowName => CurrentBook.Id == 0 ? "Новая книга" : "Редактирование книги";

        private string _coverImagePath;
        public string CoverImagePath
        {
            get => _coverImagePath;
            set
            {
                _coverImagePath = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CoverImagePath)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CoverImageSource)));
            }
        }

        public BitmapImage CoverImageSource
        {
            get
            {
                if (string.IsNullOrEmpty(CoverImagePath)) return null;
                try
                {
                    return new BitmapImage(new Uri(CoverImagePath));
                }
                catch
                {
                    return null;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BookEditWindow(Book book)
        {
            InitializeComponent();
            DataContext = this;
            CurrentBook = book ?? new Book();

            LoadComboBoxes();
            SetSelectedValues();
            UpdateCoverPreview();
        }

        private void LoadComboBoxes()
        {
            cmbGenre.ItemsSource = AppConnect.context.Genre.OrderBy(g => g.Name).ToList();
            cmbPublisher.ItemsSource = AppConnect.context.Publisher.OrderBy(p => p.Name).ToList();
        }

        private void SetSelectedValues()
        {
            if (CurrentBook.PublisherId > 0)
                cmbPublisher.SelectedValue = CurrentBook.PublisherId;

            if (CurrentBook.Id != 0 && CurrentBook.Genre != null && CurrentBook.Genre.Any())
            {
                var firstGenre = CurrentBook.Genre.FirstOrDefault();
                if (firstGenre != null)
                    cmbGenre.SelectedValue = firstGenre.Id;
            }

            if (CurrentBook.Id == 0 && !CurrentBook.IsAvailable)
                CurrentBook.IsAvailable = true;
        }

        private void UpdateCoverPreview()
        {
            if (!string.IsNullOrEmpty(CurrentBook.CoverImage))
                CoverImagePath = CurrentBook.CoverImage;
            else
                CoverImagePath = null;
        }

        private void BtnSelectCover_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp",
                Title = "Выберите обложку книги"
            };
            if (dialog.ShowDialog() == true)
                CoverImagePath = dialog.FileName;
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(CurrentBook.Name))
            {
                MessageBox.Show("Введите название книги.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(CurrentBook.Author))
            {
                MessageBox.Show("Введите автора.");
                return false;
            }
            if (CurrentBook.Price < 0)
            {
                MessageBox.Show("Цена не может быть отрицательной.");
                return false;
            }
            if (CurrentBook.StockCount < 0)
            {
                MessageBox.Show("Количество на складе не может быть отрицательным.");
                return false;
            }
            if (cmbGenre.SelectedItem == null)
            {
                MessageBox.Show("Выберите жанр.");
                return false;
            }
            if (cmbPublisher.SelectedItem == null)
            {
                MessageBox.Show("Выберите издательство.");
                return false;
            }
            return true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            txtName.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtAuthor.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtPrice.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtStockCount.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            if (!ValidateFields())
                return;

            var selectedGenre = (Genre)cmbGenre.SelectedItem;
            var selectedPublisher = (Publisher)cmbPublisher.SelectedItem;
            CurrentBook.PublisherId = selectedPublisher.Id;

            if (!string.IsNullOrEmpty(CoverImagePath) && CoverImagePath != CurrentBook.CoverImage)
            {
                string fileName = System.IO.Path.GetFileName(CoverImagePath);
                string destPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Covers", fileName);
                try
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(destPath));
                    System.IO.File.Copy(CoverImagePath, destPath, true);
                    CurrentBook.CoverImage = "Covers/" + fileName;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка копирования: {ex.Message}");
                    return;
                }
            }
            else if (CurrentBook.CoverImage != null && string.IsNullOrEmpty(CoverImagePath))
                CurrentBook.CoverImage = null;

            try
            {
                if (CurrentBook.Id == 0)
                {
                    CurrentBook.Genre = new System.Collections.Generic.HashSet<Genre>();
                    CurrentBook.Genre.Add(selectedGenre);
                    AppConnect.context.Book.Add(CurrentBook);
                }
                else
                {
                    var existing = AppConnect.context.Book.Include("Genre").FirstOrDefault(b => b.Id == CurrentBook.Id);
                    if (existing != null)
                    {
                        existing.Name = CurrentBook.Name;
                        existing.Author = CurrentBook.Author;
                        existing.Price = CurrentBook.Price;
                        existing.StockCount = CurrentBook.StockCount;
                        existing.IsAvailable = CurrentBook.IsAvailable;
                        existing.PublisherId = CurrentBook.PublisherId;
                        existing.CoverImage = CurrentBook.CoverImage;

                        existing.Genre.Clear();
                        existing.Genre.Add(selectedGenre);
                    }
                }
                AppConnect.context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}