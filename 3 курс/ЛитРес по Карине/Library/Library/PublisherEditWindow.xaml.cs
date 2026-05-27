using Library.classes;
using System;
using System.Linq;
using System.Windows;

namespace Library
{
    public partial class PublisherEditWindow : Window
    {
        private Publisher currentPublisher;

        public PublisherEditWindow(Publisher publisher)
        {
            InitializeComponent();
            currentPublisher = publisher;
            LoadCities();
            if (publisher != null)
            {
                txtName.Text = publisher.Name;
                txtRating.Text = publisher.Rating.ToString();
                cmbCity.SelectedValue = publisher.CityId;
            }
        }

        private void LoadCities()
        {
            var cities = AppConnect.context.City.OrderBy(c => c.Name).ToList();
            cmbCity.ItemsSource = cities;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Введите название издательства.");
                return;
            }
            if (!int.TryParse(txtRating.Text, out int rating) || rating < 0 || rating > 5)
            {
                MessageBox.Show("Рейтинг должен быть целым числом от 0 до 5.");
                return;
            }
            if (cmbCity.SelectedItem == null)
            {
                MessageBox.Show("Выберите город.");
                return;
            }

            currentPublisher.Name = txtName.Text.Trim();
            currentPublisher.Rating = rating;
            currentPublisher.CityId = (int)cmbCity.SelectedValue;

            try
            {
                AppConnect.context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.Message}");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}