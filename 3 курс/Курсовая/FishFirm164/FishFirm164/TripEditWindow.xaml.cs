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
    public partial class TripEditWindow : Window
    {
        private Trips currentTrip;
        private bool isNew;

        public TripEditWindow(Trips trip)
        {
            InitializeComponent();
            LoadBoats();
            LoadCrew();
            if (trip == null)
            {
                currentTrip = new Trips();
                isNew = true;
            }
            else
            {
                currentTrip = trip;
                isNew = false;
                LoadTripData();
            }
            DataContext = currentTrip;
        }

        private void LoadBoats()
        {
            cmbBoat.ItemsSource = AppConnect.context.Boats.OrderBy(b => b.BoatName).ToList();
        }

        private void LoadCrew()
        {
            var crew = AppConnect.context.CrewMembers.OrderBy(c => c.LastName).ToList();
            lbCrew.ItemsSource = crew;
        }

        private void LoadTripData()
        {
            cmbBoat.SelectedValue = currentTrip.BoatID;
            dpDepartureDate.SelectedDate = currentTrip.DepartureDate;
            dpReturnDate.SelectedDate = currentTrip.ReturnDate;
            var selectedIds = AppConnect.context.TripCrew.Where(tc => tc.TripID == currentTrip.TripID).Select(tc => tc.CrewMemberID).ToList();
            foreach (var item in lbCrew.Items)
            {
                var crew = item as CrewMembers;
                if (crew != null && selectedIds.Contains(crew.CrewMemberID))
                    lbCrew.SelectedItems.Add(crew);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbBoat.SelectedValue == null) { MessageBox.Show("Выберите катер."); return; }
            if (!dpDepartureDate.SelectedDate.HasValue) { MessageBox.Show("Выберите дату выхода."); return; }

            currentTrip.BoatID = (int)cmbBoat.SelectedValue;
            currentTrip.DepartureDate = dpDepartureDate.SelectedDate.Value;
            currentTrip.ReturnDate = dpReturnDate.SelectedDate;

            if (isNew)
                AppConnect.context.Trips.Add(currentTrip);
            AppConnect.context.SaveChanges();

            var existingCrew = AppConnect.context.TripCrew.Where(tc => tc.TripID == currentTrip.TripID).ToList();
            AppConnect.context.TripCrew.RemoveRange(existingCrew);
            var newSelectedIds = lbCrew.SelectedItems.Cast<CrewMembers>().Select(c => c.CrewMemberID).ToList();
            foreach (var crewId in newSelectedIds)
            {
                AppConnect.context.TripCrew.Add(new TripCrew { TripID = currentTrip.TripID, CrewMemberID = crewId });
            }
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