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
    public partial class SelectProductsForPurchaseWindow : Window
    {
        private List<int> excludedIds;
        private List<ProductSelectTempForPurchase> allProducts;
        public List<PurchaseItemTemp> SelectedItems { get; private set; }

        public SelectProductsForPurchaseWindow(List<int> excludedProductIds)
        {
            InitializeComponent();
            excludedIds = excludedProductIds ?? new List<int>();
            SelectedItems = new List<PurchaseItemTemp>();
            LoadProducts();
        }

        private void LoadProducts()
        {
            var query = AppConnect.context.Products.AsQueryable();
            if (excludedIds.Any())
                query = query.Where(p => !excludedIds.Contains(p.ProductID));

            allProducts = query.Select(p => new ProductSelectTempForPurchase
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                RetailPrice = p.RetailPrice,
                Quantity = 1
            }).ToList();

            dgProducts.ItemsSource = allProducts;
        }

        private void TxtSearch_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var search = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(search))
                dgProducts.ItemsSource = allProducts;
            else
                dgProducts.ItemsSource = allProducts.Where(p => p.ProductName.Contains(search)).ToList();
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            var selected = allProducts.Where(p => p.Quantity > 0).ToList();
            if (!selected.Any())
            {
                MessageBox.Show("Выберите хотя бы один товар и укажите количество.");
                return;
            }

            SelectedItems = selected.Select(p => new PurchaseItemTemp
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Quantity = p.Quantity,
                Price = p.RetailPrice
            }).ToList();

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }

    public class ProductSelectTempForPurchase
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal RetailPrice { get; set; }
        public int Quantity { get; set; }
    }
}