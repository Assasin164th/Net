using System.Windows;
using System.Windows.Controls;

namespace FishFirm164.Controls
{
    public partial class FleetControl : UserControl
    {
        private int userId;
        private string userRole;

        public FleetControl(int userId, string role)
        {
            InitializeComponent();
            this.userId = userId;
            this.userRole = role;
        }

        private void OpenTab(string header, UserControl content)
        {
            foreach (TabItem item in fleetTabControl.Items)
            {
                if (item.Header.ToString() == header)
                {
                    item.IsSelected = true;
                    return;
                }
            }
            TabItem newTab = new TabItem { Header = header, Content = content };
            fleetTabControl.Items.Add(newTab);
            newTab.IsSelected = true;
        }

        private void BtnBoats_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Катера", new BoatsControl(userId, userRole));
        }
        private void BtnCrew_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Команда", new CrewControl(userId, userRole));
        }
        private void BtnSpots_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Банки", new FishingSpotsControl(userId, userRole));
        }
        private void BtnTrips_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Выходы", new TripsControl(userId, userRole));
        }
        private void BtnCatch_Click(object sender, RoutedEventArgs e)
        {
            OpenTab("Улов", new CatchControl(userId, userRole));
        }
    }
}