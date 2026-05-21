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
    public partial class CrewControl : UserControl
    {
        private int currentPage = 1;
        private int pageSize = 10;
        private int totalPages = 1;
        private string currentSearch = "";
        private string userRole;

        public CrewControl() : this(0, "User") { }

        public CrewControl(int userId, string role)
        {
            InitializeComponent();
            userRole = role;
            LoadCrew();
            bool canEdit = (role == "Admin" || role == "Manager");
            btnAdd.IsEnabled = canEdit;
            btnEdit.IsEnabled = canEdit;
            btnDelete.IsEnabled = (role == "Admin");
        }

        private void LoadCrew()
        {
            var query = AppConnect.context.CrewMembers.AsQueryable();
            if (!string.IsNullOrEmpty(currentSearch))
                query = query.Where(c => c.LastName.Contains(currentSearch) || c.FirstName.Contains(currentSearch));
            query = query.OrderBy(c => c.LastName);
            var allCrew = query.ToList();
            totalPages = (int)Math.Ceiling(allCrew.Count / (double)pageSize);
            if (totalPages == 0) totalPages = 1;
            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;
            var pagedCrew = allCrew.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            dgCrew.ItemsSource = pagedCrew;
            txtPage.Text = $"{currentPage} / {totalPages}";
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            currentSearch = txtSearch.Text;
            currentPage = 1;
            LoadCrew();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new CrewEditWindow(null);
            if (win.ShowDialog() == true) LoadCrew();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgCrew.SelectedItem as CrewMembers;
            if (selected == null) { MessageBox.Show("Выберите члена команды."); return; }
            var win = new CrewEditWindow(selected);
            if (win.ShowDialog() == true) LoadCrew();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var selected = dgCrew.SelectedItem as CrewMembers;
            if (selected == null) { MessageBox.Show("Выберите члена команды."); return; }
            if (MessageBox.Show($"Удалить {selected.LastName} {selected.FirstName}?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                AppConnect.context.CrewMembers.Remove(selected);
                AppConnect.context.SaveChanges();
                LoadCrew();
            }
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1) { currentPage--; LoadCrew(); }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < totalPages) { currentPage++; LoadCrew(); }
        }
    }
}
