using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FishFirm164.classes;

namespace FishFirm164
{
    public partial class SelectProductsWindow : Window
    {
        private List<int> excludedProductIds;
        private List<ProductSelectTemp> allProducts;

        public List<SaleItemTemp> SelectedItems { get; private set; }

        public SelectProductsWindow(List<int> existingProductIds)
        {
            InitializeComponent();
            excludedProductIds = existingProductIds ?? new List<int>();
            SelectedItems = new List<SaleItemTemp>();
            LoadProducts();
        }

        private void LoadProducts()
        {
            var query = AppConnect.context.Products.AsQueryable();
            if (excludedProductIds.Any())
                query = query.Where(p => !excludedProductIds.Contains(p.ProductID));

            allProducts = query.Select(p => new ProductSelectTemp
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                RetailPrice = p.RetailPrice,
                SelectedQuantity = 1
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
            var selected = allProducts.Where(p => p.SelectedQuantity > 0).ToList();
            if (!selected.Any())
            {
                MessageBox.Show("Выберите хотя бы один товар и укажите количество.");
                return;
            }

            SelectedItems = selected.Select(p => new SaleItemTemp
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Quantity = p.SelectedQuantity,
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

    public class ProductSelectTemp
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal RetailPrice { get; set; }
        public int SelectedQuantity { get; set; }
    }
}