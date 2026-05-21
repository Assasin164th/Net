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
    public partial class SupplierEditWindow : Window
    {
        private Suppliers currentSupplier;
        private bool isNew;

        public SupplierEditWindow(Suppliers supplier)
        {
            InitializeComponent();
            if (supplier == null)
            {
                currentSupplier = new Suppliers();
                isNew = true;
            }
            else
            {
                currentSupplier = supplier;
                isNew = false;
            }
            DataContext = currentSupplier;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentSupplier.CompanyName))
            {
                MessageBox.Show("Введите название компании.");
                return;
            }

            if (isNew)
                AppConnect.context.Suppliers.Add(currentSupplier);
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