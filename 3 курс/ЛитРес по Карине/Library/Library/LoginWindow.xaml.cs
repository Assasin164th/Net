using Library.classes;
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

namespace Library
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
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
            var user = AppConnect.context.Users
                .Where(u => u.login == login && u.pass == password)
                .Select(u => new { u.userID, u.login, u.roleID, u.Roles.name })
                .FirstOrDefault();

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                txtError.Text = "Введите логин и пароль.";
                return;
            }
            else if (user != null)
            {
                MainWindow main = new MainWindow(user.userID, user.login, user.name);
                main.Show();
                this.Close();
            }
            else
            {
                txtError.Text = "Ошибка БД";
            }
        }

        private void BtnGuest_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
