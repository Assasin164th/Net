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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FishFirm164
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Text.Trim();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Введите логин и пароль.";
                return;
            }

            try
            {
                var user = AppConnect.context.Users
                    .Where(u => u.Login == login && u.PasswordHash == password)
                    .Select(u => new { u.UserID, u.FullName, u.Login, u.RoleID, u.Roles.RoleName })
                    .FirstOrDefault();

                if (user != null)
                {
                    MainWindow main = new MainWindow(user.UserID, user.FullName, user.Login, user.RoleName);
                    main.Show();
                    this.Close();
                }
                else
                {
                    txtError.Text = "Неверный логин или пароль.";
                }
            }
            catch (Exception ex)
            {
                txtError.Text = "Ошибка БД: " + ex.Message;
            }
        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow(-1, "Гость", "guest", "Guest");
            main.Show();
            this.Close();
        }
    }
}