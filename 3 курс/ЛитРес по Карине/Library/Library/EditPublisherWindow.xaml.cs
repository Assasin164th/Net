using Library.classes;
using System;
using System.Linq;
using System.Windows;

namespace Library
{
    public partial class EditPublisherWindow : Window
    {
        private Publisher currentPublisher;

        public EditPublisherWindow(Publisher publisher)
        {
            InitializeComponent();
            currentPublisher = publisher;
            LoadCities();
            LoadData();
        }

        private void LoadCities()
        {
            var cities = AppConnect.context.City.OrderBy(c => c.Name).ToList();
            cmbCity.ItemsSource = cities;
        }

        private void LoadData()
        {
            if (currentPublisher != null)
            {
                txtName.Text = currentPublisher.Name;
                txtRating.Text = currentPublisher.Rating.ToString();
                cmbCity.SelectedValue = currentPublisher.CityId;
                txtDescription.Text = currentPublisher.Description;
            }
        }

        private bool Validate()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название издательства.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (!int.TryParse(txtRating.Text, out int rating) || rating < 0 || rating > 5)
            {
                MessageBox.Show("Рейтинг должен быть целым числом от 0 до 5.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (cmbCity.SelectedItem == null)
            {
                MessageBox.Show("Выберите город.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!Validate()) return;

            currentPublisher.Name = txtName.Text.Trim();
            currentPublisher.Rating = int.Parse(txtRating.Text);
            currentPublisher.CityId = (int)cmbCity.SelectedValue;
            currentPublisher.Description = txtDescription.Text.Trim();

            try
            {
                AppConnect.context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}