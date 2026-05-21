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
    public partial class SuppliersControl : UserControl
    {
        private string userRole;

        public SuppliersControl(int userId, string role)
        {
            InitializeComponent();
            userRole = role;
            LoadSuppliers();

            bool canEdit = (role == "Admin" || role == "Manager" || role == "Storekeeper");
            btnAdd.IsEnabled = canEdit;
            btnEdit.IsEnabled = canEdit;
            btnDelete.IsEnabled = (role == "Admin");
        }

        private void LoadSuppliers()
        {
            dgSuppliers.ItemsSource = AppConnect.context.Suppliers.OrderBy(s => s.CompanyName).ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new SupplierEditWindow(null);
            if (win.ShowDialog() == true)
                LoadSuppliers();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgSuppliers.SelectedItem as Suppliers;
            if (selected == null)
            {
                MessageBox.Show("Выберите поставщика.");
                return;
            }
            var win = new SupplierEditWindow(selected);
            if (win.ShowDialog() == true)
                LoadSuppliers();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgSuppliers.SelectedItem as Suppliers;
            if (selected == null)
            {
                MessageBox.Show("Выберите поставщика.");
                return;
            }
            if (MessageBox.Show($"Удалить поставщика \"{selected.CompanyName}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AppConnect.context.Suppliers.Remove(selected);
                AppConnect.context.SaveChanges();
                LoadSuppliers();
            }
        }
    }
}