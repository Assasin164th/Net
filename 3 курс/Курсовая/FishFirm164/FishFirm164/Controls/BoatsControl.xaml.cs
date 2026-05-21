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
    public partial class BoatsControl : UserControl
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;
        private string currentSearch = "";
        private string userRole;

        public BoatsControl() : this(0, "User") { }

        public BoatsControl(int userId, string role)
        {
            InitializeComponent();
            userRole = role;
            LoadBoats();
            bool canEdit = (role == "Admin" || role == "Manager");
            btnAdd.IsEnabled = canEdit;
            btnEdit.IsEnabled = canEdit;
            btnDelete.IsEnabled = (role == "Admin");
        }

        private void LoadBoats()
        {
            var query = AppConnect.context.Boats.AsQueryable();
            if (!string.IsNullOrEmpty(currentSearch))
                query = query.Where(b => b.BoatName.Contains(currentSearch));
            query = query.OrderBy(b => b.BoatName);
            var allBoats = query.ToList();
            totalPages = (int)Math.Ceiling(allBoats.Count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;
            var pagedBoats = allBoats.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            dgBoats.ItemsSource = pagedBoats;
            txtPage.Text = $"{currentPage} / {totalPages}";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearch = txtSearch.Text;
            currentPage = 1;
            LoadBoats();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new BoatEditWindow(null);
            if (win.ShowDialog() == true) LoadBoats();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgBoats.SelectedItem as Boats;
            if (selected == null) { MessageBox.Show("Выберите катер."); return; }
            var win = new BoatEditWindow(selected);
            if (win.ShowDialog() == true) LoadBoats();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgBoats.SelectedItem as Boats;
            if (selected == null) { MessageBox.Show("Выберите катер."); return; }
            if (MessageBox.Show($"Удалить катер \"{selected.BoatName}\"?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AppConnect.context.Boats.Remove(selected);
                AppConnect.context.SaveChanges();
                LoadBoats();
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadBoats(); }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages) { currentPage++; LoadBoats(); }
        }
    }
}