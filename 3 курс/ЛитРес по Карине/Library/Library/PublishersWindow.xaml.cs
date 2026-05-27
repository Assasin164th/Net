using Library.classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Library
{
    public partial class PublishersWindow : Window
    {
        private int pageSize = 7;
        private int currentPage = 1;
        private int totalPages = 1;
        private List<PublisherView> allItems;

        public PublishersWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            var query = AppConnect.context.Publisher
                .OrderBy(p => p.Name)
                .Select(p => new PublisherView
                {
                    Id = p.Id,
                    Name = p.Name,
                    Rating = p.Rating,
                    CityName = p.City.Name,
                    BookCount = p.Book.Count
                })
                .ToList();

            allItems = query;
            totalPages = (int)Math.Ceiling(allItems.Count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;

            var paged = allItems.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            dgPublishers.ItemsSource = paged;

            txtTotalPages.Text = $"из {totalPages}";
            txtPageNumber.Text = currentPage.ToString();
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            btnFirst.IsEnabled = currentPage > 1;
            btnPrev.IsEnabled = currentPage > 1;
            btnNext.IsEnabled = currentPage < totalPages;
            btnLast.IsEnabled = currentPage < totalPages;
        }

        private void Refresh()
        {
            LoadData();
        }

        private void GoToPage(int page)
        {
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;
            if (currentPage != page)
            {
                currentPage = page;
                LoadData();
            }
        }

        private void BtnFirst_Click(object sender, RoutedEventArgs e) => GoToPage(1);
        private void BtnPrev_Click(object sender, RoutedEventArgs e) => GoToPage(currentPage - 1);
        private void BtnNext_Click(object sender, RoutedEventArgs e) => GoToPage(currentPage + 1);
        private void BtnLast_Click(object sender, RoutedEventArgs e) => GoToPage(totalPages);

        private void TxtPageNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(txtPageNumber.Text, out int page))
                GoToPage(page);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new AddPublisherWindow();
            win.Owner = this;
            if (win.ShowDialog() == true)
                Refresh();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgPublishers.SelectedItem as PublisherView;
            if (selected == null)
            {
                MessageBox.Show("Выберите издательство для редактирования.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var publisher = AppConnect.context.Publisher.Find(selected.Id);
            if (publisher != null)
            {
                var win = new EditPublisherWindow(publisher);
                win.Owner = this;
                if (win.ShowDialog() == true)
                    Refresh();
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgPublishers.SelectedItem as PublisherView;
            if (selected == null)
            {
                MessageBox.Show("Выберите издательство для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            if (MessageBox.Show($"Удалить издательство \"{selected.Name}\"?\nВсе связанные книги останутся без издателя (возможно, потребуется установить NULL).",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    var publisher = AppConnect.context.Publisher.Find(selected.Id);
                    if (publisher != null)
                    {
                        AppConnect.context.Publisher.Remove(publisher);
                        AppConnect.context.SaveChanges();
                        Refresh();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class PublisherView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Rating { get; set; }
        public string CityName { get; set; }
        public int BookCount { get; set; }
    }
}