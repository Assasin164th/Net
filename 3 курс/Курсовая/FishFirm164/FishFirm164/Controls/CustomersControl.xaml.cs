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
    public partial class CustomersControl : UserControl
    {
        private int currentPage = 1;
        private int pageSize = 15;
        private int totalPages = 1;
        private string userRole;

        public CustomersControl(int userId, string role)
        {
            InitializeComponent();
            userRole = role;
            bool canEdit = (role != "Guest"); // Все кроме гостя
            btnAdd.IsEnabled = canEdit;
            btnEdit.IsEnabled = canEdit;
            btnDelete.IsEnabled = canEdit;
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            var query = AppConnect.context.Customers.OrderBy(c => c.LastName);
            var all = query.ToList();
            totalPages = (int)Math.Ceiling(all.Count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;
            var paged = all.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            dgCustomers.ItemsSource = paged;
            txtPage.Text = $"{currentPage} / {totalPages}";
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new CustomerEditWindow(null);
            if (win.ShowDialog() == true) LoadCustomers();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgCustomers.SelectedItem as Customers;
            if (selected == null) { MessageBox.Show("Выберите клиента."); return; }
            var win = new CustomerEditWindow(selected);
            if (win.ShowDialog() == true) LoadCustomers();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgCustomers.SelectedItem as Customers;
            if (selected == null) { MessageBox.Show("Выберите клиента."); return; }
            if (MessageBox.Show($"Удалить клиента {selected.LastName}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AppConnect.context.Customers.Remove(selected);
                AppConnect.context.SaveChanges();
                LoadCustomers();
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e) { if (currentPage > 1) { currentPage--; LoadCustomers(); } }
        private void NextPage_Click(object sender, RoutedEventArgs e) { if (currentPage < totalPages) { currentPage++; LoadCustomers(); } }
    }
}
