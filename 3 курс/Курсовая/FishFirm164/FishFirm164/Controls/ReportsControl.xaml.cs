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
    public partial class ReportsControl : UserControl
    {
        public ReportsControl(int userId, string role)
        {
            InitializeComponent();
        }

        private void BtnStock_Click(object sender, RoutedEventArgs e)
        {
            periodPanel.Visibility = Visibility.Collapsed;
            var stock = AppConnect.context.Products
                .Select(p => new { p.ProductName, p.QuantityInStock, p.RetailPrice })
                .OrderBy(p => p.ProductName)
                .ToList();
            dgReport.ItemsSource = stock;
        }

        private void BtnProfit_Click(object sender, RoutedEventArgs e)
        {
            periodPanel.Visibility = Visibility.Visible;
            dgReport.ItemsSource = null;
        }

        private void BtnShowProfit_Click(object sender, RoutedEventArgs e)
        {
            if (!dpStart.SelectedDate.HasValue || !dpEnd.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите период.");
                return;
            }
            var start = dpStart.SelectedDate.Value;
            var end = dpEnd.SelectedDate.Value.AddDays(1);
            var sales = AppConnect.context.Sales
                .Where(s => s.SaleDate >= start && s.SaleDate < end)
                .SelectMany(s => s.SaleItems)
                .GroupBy(si => si.Products.ProductName)
                .Select(g => new
                {
                    Товар = g.Key,
                    Количество = g.Sum(si => si.Quantity),
                    Выручка = g.Sum(si => si.Quantity * si.PriceAtSale)
                })
                .OrderByDescending(x => x.Выручка)
                .ToList();
            dgReport.ItemsSource = sales;
        }

        private void BtnPopular_Click(object sender, RoutedEventArgs e)
        {
            periodPanel.Visibility = Visibility.Collapsed;
            var popular = AppConnect.context.SaleItems
                .GroupBy(si => si.Products.ProductName)
                .Select(g => new
                {
                    Товар = g.Key,
                    Продано = g.Sum(si => si.Quantity)
                })
                .OrderByDescending(x => x.Продано)
                .Take(10)
                .ToList();
            dgReport.ItemsSource = popular;
        }
    }
}