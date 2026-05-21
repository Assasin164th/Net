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

    public partial class CrewMembers
    {
        public string FullName => $"{LastName} {FirstName} ({Position})";
    }

    public partial class CrewEditWindow : Window
    {
        private CrewMembers currentCrew;
        private bool isNew;

        public CrewEditWindow(CrewMembers crew)
        {
            InitializeComponent();
            if (crew == null)
            {
                currentCrew = new CrewMembers();
                isNew = true;
            }
            else
            {
                currentCrew = crew;
                isNew = false;
            }
            DataContext = currentCrew;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentCrew.LastName) || string.IsNullOrWhiteSpace(currentCrew.FirstName))
            {
                MessageBox.Show("Введите фамилию и имя.");
                return;
            }
            if (isNew)
                AppConnect.context.CrewMembers.Add(currentCrew);
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