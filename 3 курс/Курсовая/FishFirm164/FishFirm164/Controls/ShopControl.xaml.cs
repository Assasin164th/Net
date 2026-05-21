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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FishFirm164.Controls
{
    public partial class ShopControl : UserControl
    {
        private int userId;
        private string userRole;

        public ShopControl(int userId, string role)
        {
            InitializeComponent();
            this.userId = userId;
            this.userRole = role;

            bool isAdmin = (role == "Admin");
            bool isManager = (role == "Manager");
            bool isDirector = (role == "Director");
            bool isStorekeeper = (role == "Storekeeper");
            bool isCashier = (role == "Cashier");
            bool isGuest = (role == "Guest");

            btnProducts.Visibility = Visibility.Visible;
            btnSales.Visibility = (isCashier || isManager || isAdmin || isDirector) ? Visibility.Visible : Visibility.Collapsed;
            btnPurchases.Visibility = (isStorekeeper || isManager || isAdmin || isDirector) ? Visibility.Visible : Visibility.Collapsed;
            btnCustomers.Visibility = (!isGuest) ? Visibility.Visible : Visibility.Collapsed;
            btnSuppliers.Visibility = (isManager || isAdmin || isDirector || isStorekeeper) ? Visibility.Visible : Visibility.Collapsed;
            btnUsers.Visibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            btnReports.Visibility = (isDirector || isAdmin || isManager) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OpenTab(string header, UserControl content)
        {
            foreach (TabItem item in shopTabControl.Items)
            {
                if (item.Header.ToString() == header)
                {
                    item.IsSelected = true;
                    return;
                }
            }
            TabItem newTab = new TabItem { Header = header, Content = content };
            shopTabControl.Items.Add(newTab);
            newTab.IsSelected = true;
        }

        private void BtnProducts_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Товары", new ProductsControl(userId, userRole));
        }
        private void BtnSales_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Продажи", new SalesControl(userId, userRole));
        }
        private void BtnPurchases_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Закупки", new PurchasesControl(userId, userRole));
        }
        private void BtnCustomers_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Клиенты", new CustomersControl(userId, userRole));
        }
        private void BtnSuppliers_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Поставщики", new SuppliersControl(userId, userRole));
        }
        private void BtnUsers_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Пользователи", new UsersControl(userId, userRole));
        }
        private void BtnReports_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Отчёты", new ReportsControl(userId, userRole));
        }
    }
}