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
    public partial class BoatEditWindow : Window
    {
        private Boats currentBoat;
        private bool isNew;

        public BoatEditWindow(Boats boat)
        {
            InitializeComponent();
            if (boat == null)
            {
                currentBoat = new Boats();
                isNew = true;
            }
            else
            {
                currentBoat = boat;
                isNew = false;
            }
            DataContext = currentBoat;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentBoat.BoatName))
            {
                MessageBox.Show("Введите название катера.");
                return;
            }
            if (isNew)
                AppConnect.context.Boats.Add(currentBoat);
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