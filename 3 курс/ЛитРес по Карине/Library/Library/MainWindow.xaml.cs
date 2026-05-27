using Library.classes;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Library
{
    public partial class MainWindow : Window
    {
        private string searchText = "";
        private int? selectedGenreId = null;
        private bool onlyInStock = false;
        private bool sortAscending = true;
        private Book selectedBook = null;

        public MainWindow(int userId, string userLogin, string userRole)
        {
            InitializeComponent();
            LoadGenres();
            LoadBooks();
        }

        private void LoadGenres()
        {
            var genres = AppConnect.context.Genre.OrderBy(g => g.Name).ToList();
            genres.Insert(0, new Genre { Id = 0, Name = "Все жанры" });
            cmbGenre.ItemsSource = genres;
            cmbGenre.SelectedValue = 0;
        }

        private void LoadBooks()
        {
            var query = AppConnect.context.Book.AsQueryable();

            if (!string.IsNullOrEmpty(searchText))
                query = query.Where(b => b.Name.Contains(searchText) || b.Author.Contains(searchText));

            if (selectedGenreId.HasValue && selectedGenreId.Value > 0)
                query = query.Where(b => b.Genre.Any(g => g.Id == selectedGenreId.Value));

            if (onlyInStock)
                query = query.Where(b => b.IsAvailable && b.StockCount > 0);

            if (sortAscending)
                query = query.OrderBy(b => b.Price);
            else
                query = query.OrderByDescending(b => b.Price);

            var books = query.ToList();
            icBooks.ItemsSource = books;

            decimal total = books.Sum(b => b.Price * b.StockCount);
            txtTotalCost.Text = $"{total:F2} руб.";

            selectedBook = null;
            Dispatcher.BeginInvoke(new Action(UpdateAllCoverImages), System.Windows.Threading.DispatcherPriority.Background);
        }

        private void UpdateAllCoverImages()
        {
            for (int i = 0; i < icBooks.Items.Count; i++)
            {
                var container = icBooks.ItemContainerGenerator.ContainerFromIndex(i) as ContentPresenter;
                if (container == null) continue;

                var img = FindVisualChild<System.Windows.Controls.Image>(container, "imgCover");
                var book = icBooks.Items[i] as Book;
                if (img != null && book != null)
                {
                    img.Source = LoadCoverImage(book.CoverImage);
                }
            }
        }

        private BitmapImage LoadCoverImage(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return GetPlaceholder();

            string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);
            if (!File.Exists(fullPath))
                return GetPlaceholder();

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(fullPath, UriKind.Absolute);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return GetPlaceholder();
            }
        }

        private BitmapImage GetPlaceholder()
        {
            try
            {
                string placeholderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "placeholder.png");
                if (File.Exists(placeholderPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(placeholderPath, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch { }
            return null;
        }

        private T FindVisualChild<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T t && t.Name == name) return t;
                var result = FindVisualChild<T>(child, name);
                if (result != null) return result;
            }
            return null;
        }

        private void BookBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            if (border != null)
            {
                selectedBook = border.Tag as Book;
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddBookWindow();
            win.Owner = this;
            if (win.ShowDialog() == true)
                LoadBooks();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBook == null)
            {
                MessageBox.Show("Выберите книгу (нажмите на плитку).", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var win = new BookEditWindow(selectedBook);
            if (win.ShowDialog() == true)
                LoadBooks();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBook == null)
            {
                MessageBox.Show("Выберите книгу (нажмите на плитку).", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool hasOrders = AppConnect.context.Order.Any(o => o.BookId == selectedBook.Id);
            if (hasOrders)
            {
                MessageBox.Show("Невозможно удалить книгу, так как на неё есть заказы.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Удалить \"{selectedBook.Name}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    AppConnect.context.Book.Remove(selectedBook);
                    AppConnect.context.SaveChanges();
                    LoadBooks(); // обновить список
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.InnerException?.Message ?? ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchText = txtSearch.Text.Trim();
            LoadBooks();
        }

        private void CmbGenre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = cmbGenre.SelectedItem as Genre;
            selectedGenreId = (selected?.Id == 0) ? null : selected?.Id;
            LoadBooks();
        }

        private void Filter_Changed(object sender, RoutedEventArgs e)
        {
            onlyInStock = chkOnlyInStock.IsChecked == true;
            LoadBooks();
        }

        private void Sort_Changed(object sender, RoutedEventArgs e)
        {
            sortAscending = rbPriceAsc.IsChecked == true;
            LoadBooks();
        }

        private void BtnPublishers_Click(object sender, RoutedEventArgs e)
        {
            var win = new PublishersWindow();
            win.Owner = this;
            win.ShowDialog();
        }
    }
}