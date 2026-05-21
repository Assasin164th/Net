using FishFirm164.classes;
using System;
using System.Collections.Generic;
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

namespace FishFirm164.Controls
{
    public partial class ProductsControl : UserControl
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;
        private string currentSearch = "";
        private int? currentCategoryId = null;
        private int? currentManufacturerId = null;
        private string userRole;

        public ProductsControl() : this(0, "User") { }

        public ProductsControl(int userId, string role)
        {
            InitializeComponent();
            userRole = role;
            LoadFilters();
            LoadProducts();

            bool canEdit = (role == "Admin" || role == "Manager" || role == "Storekeeper");
            btnAdd.IsEnabled = canEdit;
            btnEdit.IsEnabled = canEdit;
            btnDelete.IsEnabled = canEdit;
        }

        private void LoadFilters()
        {
            var categories = AppConnect.context.Categories.OrderBy(c => c.CategoryName).ToList();
            categories.Insert(0, new Categories { CategoryID = 0, CategoryName = "Все категории" });
            cmbCategory.ItemsSource = categories;
            cmbCategory.SelectedValue = 0;

            var manufacturers = AppConnect.context.Manufacturers.OrderBy(m => m.Name).ToList();
            manufacturers.Insert(0, new Manufacturers { ManufacturerID = 0, Name = "Все производители" });
            cmbManufacturer.ItemsSource = manufacturers;
            cmbManufacturer.SelectedValue = 0;
        }

        private void LoadProducts()
        {
            var query = AppConnect.context.Products
                .Include("Categories")
                .Include("Manufacturers")
                .AsQueryable();

            if (!string.IsNullOrEmpty(currentSearch))
                query = query.Where(p => p.ProductName.Contains(currentSearch));

            if (currentCategoryId.HasValue && currentCategoryId.Value > 0)
                query = query.Where(p => p.CategoryID == currentCategoryId.Value);

            if (currentManufacturerId.HasValue && currentManufacturerId.Value > 0)
                query = query.Where(p => p.ManufacturerID == currentManufacturerId.Value);

            query = query.OrderBy(p => p.ProductName);
            var allProducts = query.ToList();

            foreach (var p in allProducts)
            {
                var img = AppConnect.context.ProductImages
                    .FirstOrDefault(i => i.ProductID == p.ProductID && i.IsMain == true);
                p.MainImageUrl = img?.ImageURL;
            }

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
            var win = new ProductEditWindow(null);
            if (win.ShowDialog() == true)
                LoadProducts();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvProducts.SelectedItem as Products;
            if (selected == null)
            {
                MessageBox.Show("Выберите товар.");
                return;
            }
            var win = new ProductEditWindow(selected);
            if (win.ShowDialog() == true)
                LoadProducts();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = lvProducts.SelectedItem as Products;
            if (selected == null)
            {
                MessageBox.Show("Выберите товар.");
                return;
            }
            if (MessageBox.Show($"Удалить \"{selected.ProductName}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AppConnect.context.Products.Remove(selected);
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