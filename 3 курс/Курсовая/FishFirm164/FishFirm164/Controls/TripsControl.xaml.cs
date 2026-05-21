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

namespace FishFirm164.Controls
{
    public partial class TripsControl : UserControl
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;
        private string currentSearch = "";
        private string userRole;

        public TripsControl() : this(0, "User") { }

        public TripsControl(int userId, string role)
        {
            InitializeComponent();
            userRole = role;
            LoadTrips();
            bool canEdit = (role == "Admin" || role == "Manager");
            btnAdd.IsEnabled = canEdit;
            btnEdit.IsEnabled = canEdit;
            btnDelete.IsEnabled = (role == "Admin");
        }

        private void LoadTrips()
        {
            var query = AppConnect.context.Trips.Include("Boats").AsQueryable();
            if (!string.IsNullOrEmpty(currentSearch))
                query = query.Where(t => t.Boats.BoatName.Contains(currentSearch));
            query = query.OrderByDescending(t => t.DepartureDate);
            var allTrips = query.ToList();
            totalPages = (int)Math.Ceiling(allTrips.Count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;
            var pagedTrips = allTrips.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            dgTrips.ItemsSource = pagedTrips;
            txtPage.Text = $"{currentPage} / {totalPages}";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearch = txtSearch.Text;
            currentPage = 1;
            LoadTrips();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new TripEditWindow(null);
            if (win.ShowDialog() == true) LoadTrips();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgTrips.SelectedItem as Trips;
            if (selected == null) { MessageBox.Show("Выберите выход."); return; }
            var win = new TripEditWindow(selected);
            if (win.ShowDialog() == true) LoadTrips();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgTrips.SelectedItem as Trips;
            if (selected == null) { MessageBox.Show("Выберите выход."); return; }
            if (MessageBox.Show($"Удалить выход от {selected.DepartureDate:d}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var crew = AppConnect.context.TripCrew.Where(tc => tc.TripID == selected.TripID);
                AppConnect.context.TripCrew.RemoveRange(crew);
                var catches = AppConnect.context.Catch.Where(c => c.TripID == selected.TripID);
                AppConnect.context.Catch.RemoveRange(catches);
                AppConnect.context.Trips.Remove(selected);
                AppConnect.context.SaveChanges();
                LoadTrips();
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadTrips(); }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages) { currentPage++; LoadTrips(); }
        }
    }
}