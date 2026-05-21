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
    public partial class CatchControl : UserControl
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;
        private string currentSearch = "";
        private string userRole;

        public CatchControl() : this(0, "User") { }

        public CatchControl(int userId, string role)
        {
            InitializeComponent();
            userRole = role;
            LoadCatch();
            bool canEdit = (role == "Admin" || role == "Manager");
            btnAdd.IsEnabled = canEdit;
            btnEdit.IsEnabled = canEdit;
            btnDelete.IsEnabled = (role == "Admin");
        }

        private void LoadCatch()
        {
            var query = AppConnect.context.Catch.Include("Trips.Boats").Include("FishingSpots").AsQueryable();
            if (!string.IsNullOrEmpty(currentSearch))
                query = query.Where(c => c.Trips.Boats.BoatName.Contains(currentSearch));
            query = query.OrderByDescending(c => c.ArrivalDate);
            var allCatch = query.ToList();
            totalPages = (int)Math.Ceiling(allCatch.Count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;
            var pagedCatch = allCatch.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            dgCatch.ItemsSource = pagedCatch;
            txtPage.Text = $"{currentPage} / {totalPages}";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearch = txtSearch.Text;
            currentPage = 1;
            LoadCatch();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new CatchEditWindow(null);
            if (win.ShowDialog() == true) LoadCatch();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgCatch.SelectedItem as Catch;
            if (selected == null) { MessageBox.Show("Выберите запись улова."); return; }
            var win = new CatchEditWindow(selected);
            if (win.ShowDialog() == true) LoadCatch();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgCatch.SelectedItem as Catch;
            if (selected == null) { MessageBox.Show("Выберите запись улова."); return; }
            if (MessageBox.Show($"Удалить запись улова (рыба: {selected.FishType}, вес: {selected.Weight} кг)?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AppConnect.context.Catch.Remove(selected);
                AppConnect.context.SaveChanges();
                LoadCatch();
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadCatch(); }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages) { currentPage++; LoadCatch(); }
        }
    }
}