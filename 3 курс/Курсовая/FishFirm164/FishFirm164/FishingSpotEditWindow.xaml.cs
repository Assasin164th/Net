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
    public partial class Trips
    {
        public string TripDisplay { get; set; }
    }

    public partial class FishingSpotEditWindow : Window
    {
        private FishingSpots currentSpot;
        private bool isNew;

        public FishingSpotEditWindow(FishingSpots spot)
        {
            InitializeComponent();
            if (spot == null)
            {
                currentSpot = new FishingSpots();
                isNew = true;
            }
            else
            {
                currentSpot = spot;
                isNew = false;
            }
            DataContext = currentSpot;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentSpot.SpotName))
            {
                MessageBox.Show("Введите название банки.");
                return;
            }
            if (isNew)
                AppConnect.context.FishingSpots.Add(currentSpot);
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