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

namespace FishFirm164
{
    public partial class SalesControl : UserControl
    {
        private int currentPage = 1;
        private int pageSize = 15;
        private int totalPages = 1;
        private string userRole;
        private int currentUserId;

        public SalesControl(int userId, string role)
        {
            InitializeComponent();
            currentUserId = userId;
            userRole = role;
            LoadSales();

            // Права: только администратор, директор, менеджер могут удалять/изменять (кассир только добавление)
            bool canEdit = (role == "Admin" || role == "Director" || role == "Manager");
            bool canDelete = (role == "Admin" || role == "Director" || role == "Manager");
            btnEdit.IsEnabled = canEdit;
            btnDelete.IsEnabled = canDelete;
            // Добавление доступно кассиру, менеджеру, админу, директору
            btnAdd.IsEnabled = (role == "Cashier" || role == "Manager" || role == "Admin" || role == "Director");
        }

        private void LoadSales()
        {
            var query = AppConnect.context.Sales
                .Include("Customers")
                .Include("Users")
                .OrderByDescending(s => s.SaleDate)
                .AsQueryable();

            var allSales = query.ToList();
            totalPages = (int)Math.Ceiling(allSales.Count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;

            var paged = allSales.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            dgSales.ItemsSource = paged;
            txtPage.Text = $"{currentPage} / {totalPages}";
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new SaleEditWindow(null, currentUserId);
            if (win.ShowDialog() == true)
                LoadSales();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgSales.SelectedItem as Sales;
            if (selected == null)
            {
                MessageBox.Show("Выберите продажу.");
                return;
            }
            var win = new SaleEditWindow(selected.SaleID, currentUserId);
            if (win.ShowDialog() == true)
                LoadSales();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgSales.SelectedItem as Sales;
            if (selected == null)
            {
                MessageBox.Show("Выберите продажу.");
                return;
            }
            if (MessageBox.Show($"Удалить продажу №{selected.SaleID}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var items = AppConnect.context.SaleItems.Where(si => si.SaleID == selected.SaleID).ToList();
                AppConnect.context.SaleItems.RemoveRange(items);
                AppConnect.context.Sales.Remove(selected);
                AppConnect.context.SaveChanges();
                LoadSales();
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadSales(); }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages) { currentPage++; LoadSales(); }
        }

        private void DgSales_SelectionChanged(object sender, SelectionChangedEventArgs e) { }
    }
}