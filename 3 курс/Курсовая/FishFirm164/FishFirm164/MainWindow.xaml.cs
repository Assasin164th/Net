using FishFirm164;
using FishFirm164.classes;
using FishFirm164.Controls;
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
    public partial class MainWindow : Window
    {
        private int userId;
        private string userRole;
        private ShopControl shopControl;
        private FleetControl fleetControl;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(int id, string name, string log, string role)
        {
            InitializeComponent();
            userId = id;
            userRole = role;
            txtUserInfo.Text = $"{name} ({role})";

            shopControl = new ShopControl(userId, userRole);
            fleetControl = new FleetControl(userId, userRole);

            // Принудительно находим mainContent
            var contentControl = this.FindName("mainContent") as ContentControl;
            if (contentControl != null)
                contentControl.Content = shopControl;
            else
                MessageBox.Show("Ошибка: mainContent не найден. Проверьте XAML.", "Критическая ошибка");
        }

        private void RbShop_Checked(object sender, RoutedEventArgs e)
        {
            var contentControl = this.FindName("mainContent") as ContentControl;
            if (contentControl != null) contentControl.Content = shopControl;
        }

        private void RbFleet_Checked(object sender, RoutedEventArgs e)
        {
            var contentControl = this.FindName("mainContent") as ContentControl;
            if (contentControl != null) contentControl.Content = fleetControl;
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }
    }
}