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
    public partial class CustomerEditWindow : Window
    {
        private Customers current;
        private bool isNew;
        public CustomerEditWindow(Customers customer)
        {
            InitializeComponent();
            if (customer == null)
            {
                current = new Customers();
                isNew = true;
            }
            else
            {
                current = customer;
                isNew = false;
            }
            DataContext = current;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                MessageBox.Show("Введите фамилию и имя.");
                return;
            }
            if (isNew)
                AppConnect.context.Customers.Add(current);
            AppConnect.context.SaveChanges();
            DialogResult = true;
            Close();
        }
        private void BtnCancel_Click(object sender, RoutedEventArgs e) { DialogResult = false; Close(); }
    }
}