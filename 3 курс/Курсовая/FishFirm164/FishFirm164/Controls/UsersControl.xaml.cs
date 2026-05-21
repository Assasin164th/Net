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
    public partial class UsersControl : UserControl
    {
        public UsersControl(int userId, string role)
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            dgUsers.ItemsSource = AppConnect.context.Users.Include("Roles").OrderBy(u => u.FullName).ToList();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new UserEditWindow(null);
            if (win.ShowDialog() == true)
                LoadUsers();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgUsers.SelectedItem as Users;
            if (selected == null)
            {
                MessageBox.Show("Выберите пользователя.");
                return;
            }
            var win = new UserEditWindow(selected);
            if (win.ShowDialog() == true)
                LoadUsers();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgUsers.SelectedItem as Users;
            if (selected == null)
            {
                MessageBox.Show("Выберите пользователя.");
                return;
            }
            if (MessageBox.Show($"Удалить пользователя {selected.FullName}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AppConnect.context.Users.Remove(selected);
                AppConnect.context.SaveChanges();
                LoadUsers();
            }
        }
    }
}