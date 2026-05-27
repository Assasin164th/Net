using Library.classes;
using System;
using System.Linq;
using System.Windows;

namespace Library
{
    public partial class AddPublisherWindow : Window
    {
        public AddPublisherWindow()
        {
            InitializeComponent();
            LoadCities();
        }

        private void LoadCities()
        {
            var cities = AppConnect.context.City.OrderBy(c => c.Name).ToList();
            cmbCity.ItemsSource = cities;
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

            var publisher = new Publisher
            {
                Name = txtName.Text.Trim(),
                Rating = int.Parse(txtRating.Text),
                CityId = (int)cmbCity.SelectedValue,
                Description = txtDescription.Text.Trim()
            };

            try
            {
                AppConnect.context.Publisher.Add(publisher);
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