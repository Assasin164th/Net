using Library.classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Library
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int userID;
        private string userRole;
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;
        private string currentSearch = "";
        private int? currentCategoryId = null;
        private int? currentManufacturerId = null;

        public MainWindow(int userID, string userRole, string RoleName)
        {
            InitializeComponent();
            this.userID = userID;
            this.userRole = userRole;
            userRole = RoleName;
            LoadFilters();
            LoadProducts();

            bool canEdit = (RoleName == "Admin" || RoleName == "Manager" || RoleName == "Storekeeper");
            btnAdd.IsEnabled = canEdit;
            btnEdit.IsEnabled = canEdit;
            btnDelete.IsEnabled = canEdit;
        }

        private void LoadFilters()
        {
            var categories = AppConnect.context.Genre.OrderBy(c => c.Name).ToList();
            categories.Insert(0, new Genre { Id = 0, Name = "Все категории" });
            cmbCategory.ItemsSource = categories;
            cmbCategory.SelectedValue = 0;

            var manufacturers = AppConnect.context.Publisher.OrderBy(m => m.Name).ToList();
            manufacturers.Insert(0, new Publisher { Id = 0, Name = "Все производители" });
            cmbManufacturer.ItemsSource = manufacturers;
            cmbManufacturer.SelectedValue = 0;
        }

        private void LoadProducts()
        {
            userName.Text = userRole;
            var query = AppConnect.context.Book
                .Include("Genre")
                .Include("Publisher")
                .AsQueryable();

            if (!string.IsNullOrEmpty(currentSearch))
                query = query.Where(p => p.Name.Contains(currentSearch));

            if (currentCategoryId.HasValue && currentCategoryId.Value > 0)
                query = query.Where(p => p.Id == currentCategoryId.Value);

            if (currentManufacturerId.HasValue && currentManufacturerId.Value > 0)
                query = query.Where(p => p.Id == currentManufacturerId.Value);

            query = query.OrderBy(p => p.Name);
            var allProducts = query.ToList();

            //foreach (var p in allProducts)
            //{
            //    var img = AppConnect.context.ProductImages
            //        .FirstOrDefault(i => i.ProductID == p.ProductID && i.IsMain == true);
            //    p.MainImageUrl = img?.ImageURL;
            //}

            totalPages = (int)Math.Ceiling(allProducts.Count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;

            var pagedProducts = allProducts.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            lvProducts.ItemsSource = pagedProducts;
            txtPage.Text = $"{currentPage} / {totalPages}";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearch = txtSearch.Text;
            currentPage = 1;
            LoadProducts();
        }

        private void CmbFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentCategoryId = (cmbCategory.SelectedValue as int?) == 0 ? null : cmbCategory.SelectedValue as int?;
            currentManufacturerId = (cmbManufacturer.SelectedValue as int?) == 0 ? null : cmbManufacturer.SelectedValue as int?;
            currentPage = 1;
            LoadProducts();
        }

        private void BtnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Text = "";
            cmbCategory.SelectedValue = 0;
            cmbManufacturer.SelectedValue = 0;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new BookEditWindow(null);
            if (win.ShowDialog() == true)
                LoadProducts();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvProducts.SelectedItem as Book;
            if (selected == null)
            {
                MessageBox.Show("Выберите товар.");
                return;
            }
            var win = new BookEditWindow(selected);
            if (win.ShowDialog() == true)
                LoadProducts();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvProducts.SelectedItem as Book;
            if (selected == null)
            {
                MessageBox.Show("Выберите товар.");
                return;
            }
            if (MessageBox.Show($"Удалить \"{selected.Name}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AppConnect.context.Book.Remove(selected);
                AppConnect.context.SaveChanges();
                LoadProducts();
            }
        }

        private void LvProducts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BtnEdit_Click(sender, null);
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadProducts(); }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages) { currentPage++; LoadProducts(); }
        }
    }
}
