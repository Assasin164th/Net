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
    public partial class PurchasesControl : UserControl
    {
        private int userId;
        private string userRole;

        public PurchasesControl(int id, string role)
        {
            InitializeComponent();
            userId = id;
            userRole = role;
            LoadPurchases();

            bool canEdit = (role == "Admin" || role == "Manager" || role == "Storekeeper");
            btnAdd.IsEnabled = canEdit;
            btnEdit.IsEnabled = canEdit;
            btnDelete.IsEnabled = (role == "Admin");
        }

        private void LoadPurchases()
        {
            var purchases = AppConnect.context.Purchases
                .Include("Suppliers")
                .OrderByDescending(p => p.PurchaseDate)
                .ToList();
            dgPurchases.ItemsSource = purchases;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new PurchaseEditWindow(null, userId);
            if (win.ShowDialog() == true)
                LoadPurchases();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgPurchases.SelectedItem as Purchases;
            if (selected == null)
            {
                MessageBox.Show("Выберите закупку.");
                return;
            }
            var win = new PurchaseEditWindow(selected.PurchaseID, userId);
            if (win.ShowDialog() == true)
                LoadPurchases();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgPurchases.SelectedItem as Purchases;
            if (selected == null)
            {
                MessageBox.Show("Выберите закупку.");
                return;
            }
            if (MessageBox.Show($"Удалить закупку №{selected.PurchaseID}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var items = AppConnect.context.PurchaseItems.Where(pi => pi.PurchaseID == selected.PurchaseID);
                AppConnect.context.PurchaseItems.RemoveRange(items);
                AppConnect.context.Purchases.Remove(selected);
                AppConnect.context.SaveChanges();
                LoadPurchases();
            }
        }
    }
}