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
    public partial class FishingSpotsControl : UserControl
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;
        private string currentSearch = "";
        private string userRole;

        public FishingSpotsControl() : this(0, "User") { }

        public FishingSpotsControl(int userId, string role)
        {
            InitializeComponent();
            userRole = role;
            LoadSpots();
            bool canEdit = (role == "Admin" || role == "Manager");
            btnAdd.IsEnabled = canEdit;
            btnEdit.IsEnabled = canEdit;
            btnDelete.IsEnabled = (role == "Admin");
        }

        private void LoadSpots()
        {
            var query = AppConnect.context.FishingSpots.AsQueryable();
            if (!string.IsNullOrEmpty(currentSearch))
                query = query.Where(s => s.SpotName.Contains(currentSearch));
            query = query.OrderBy(s => s.SpotName);
            var allSpots = query.ToList();
            totalPages = (int)Math.Ceiling(allSpots.Count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;
            var pagedSpots = allSpots.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            dgSpots.ItemsSource = pagedSpots;
            txtPage.Text = $"{currentPage} / {totalPages}";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearch = txtSearch.Text;
            currentPage = 1;
            LoadSpots();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new FishingSpotEditWindow(null);
            if (win.ShowDialog() == true) LoadSpots();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgSpots.SelectedItem as FishingSpots;
            if (selected == null) { MessageBox.Show("Выберите банку."); return; }
            var win = new FishingSpotEditWindow(selected);
            if (win.ShowDialog() == true) LoadSpots();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgSpots.SelectedItem as FishingSpots;
            if (selected == null) { MessageBox.Show("Выберите банку."); return; }
            if (MessageBox.Show($"Удалить банку \"{selected.SpotName}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AppConnect.context.FishingSpots.Remove(selected);
                AppConnect.context.SaveChanges();
                LoadSpots();
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadSpots(); }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages) { currentPage++; LoadSpots(); }
        }
    }
}