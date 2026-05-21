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
    public partial class UserEditWindow : Window
    {
        private Users currentUser;
        private bool isNew;

        public UserEditWindow(Users user)
        {
            InitializeComponent();
            if (user == null)
            {
                currentUser = new Users();
                isNew = true;
            }
            else
            {
                currentUser = user;
                isNew = false;
            }
            DataContext = currentUser;
            LoadRoles();
        }

        private void LoadRoles()
        {
            cmbRole.ItemsSource = AppConnect.context.Roles.ToList();
            cmbRole.SelectedValue = currentUser.RoleID;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentUser.FullName) || string.IsNullOrWhiteSpace(currentUser.Login))
            {
                MessageBox.Show("Введите ФИО и логин.");
                return;
            }

            if (isNew)
            {
                currentUser.PasswordHash = txtPassword.Password; 
                AppConnect.context.Users.Add(currentUser);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(txtPassword.Password))
                    currentUser.PasswordHash = txtPassword.Password;
            }
            currentUser.RoleID = (int)cmbRole.SelectedValue;
            AppConnect.context.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
