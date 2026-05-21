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
    public partial class CatchEditWindow : Window
    {
        private Catch currentCatch;
        private bool isNew;

        public CatchEditWindow(Catch catchRecord)
        {
            InitializeComponent();
            LoadTrips();
            LoadSpots();
            if (catchRecord == null)
            {
                currentCatch = new Catch();
                isNew = true;
            }
            else
            {
                currentCatch = catchRecord;
                isNew = false;
                LoadCatchData();
            }
            DataContext = currentCatch;
        }

        private void LoadTrips()
        {
            var trips = AppConnect.context.Trips.Include("Boats").ToList();
            foreach (var t in trips)
                t.TripDisplay = $"{t.Boats.BoatName} - {t.DepartureDate:dd.MM.yyyy}";
            cmbTrip.ItemsSource = trips;
        }

        private void LoadSpots()
        {
            cmbSpot.ItemsSource = AppConnect.context.FishingSpots.OrderBy(s => s.SpotName).ToList();
        }

        private void LoadCatchData()
        {
            cmbTrip.SelectedValue = currentCatch.TripID;
            cmbSpot.SelectedValue = currentCatch.SpotID;
            txtFishType.Text = currentCatch.FishType;
            txtWeight.Text = currentCatch.Weight.ToString();
            cmbQuality.Text = currentCatch.Quality;
            dpArrivalDate.SelectedDate = currentCatch.ArrivalDate;
            dpDepartureDate.SelectedDate = currentCatch.DepartureDate;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbTrip.SelectedValue == null) { MessageBox.Show("Выберите выход."); return; }
            if (cmbSpot.SelectedValue == null) { MessageBox.Show("Выберите банку."); return; }
            if (string.IsNullOrWhiteSpace(txtFishType.Text)) { MessageBox.Show("Введите сорт рыбы."); return; }
            if (!decimal.TryParse(txtWeight.Text, out decimal weight)) { MessageBox.Show("Введите корректный вес."); return; }
            if (!dpArrivalDate.SelectedDate.HasValue || !dpDepartureDate.SelectedDate.HasValue) { MessageBox.Show("Выберите даты прихода и отплытия."); return; }

            currentCatch.TripID = (int)cmbTrip.SelectedValue;
            currentCatch.SpotID = (int)cmbSpot.SelectedValue;
            currentCatch.FishType = txtFishType.Text;
            currentCatch.Weight = weight;
            currentCatch.Quality = (cmbQuality.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "хорошее";
            currentCatch.ArrivalDate = dpArrivalDate.SelectedDate.Value;
            currentCatch.DepartureDate = dpDepartureDate.SelectedDate.Value;

            if (isNew)
                AppConnect.context.Catch.Add(currentCatch);
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