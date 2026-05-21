using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using FishFirm164.classes;

namespace FishFirm164
{
    public partial class ProductEditWindow : Window
    {
        private Products currentProduct;
        private bool isNew;

        public ProductEditWindow(Products product)
        {
            InitializeComponent();
            if (product == null)
            {
                currentProduct = new Products();
                isNew = true;
            }
            else
            {
                currentProduct = product;
                isNew = false;
            }
            DataContext = currentProduct;
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            cmbCategory.ItemsSource = AppConnect.context.Categories.OrderBy(c => c.CategoryName).ToList();
            cmbManufacturer.ItemsSource = AppConnect.context.Manufacturers.OrderBy(m => m.Name).ToList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(currentProduct.ProductName))
            {
                MessageBox.Show("Введите название товара.");
                return;
            }

            if (isNew)
                AppConnect.context.Products.Add(currentProduct);
            else
            {
                var existing = AppConnect.context.Products.Find(currentProduct.ProductID);
                if (existing != null)
                {
                    existing.ProductName = currentProduct.ProductName;
                    existing.CategoryID = currentProduct.CategoryID;
                    existing.ManufacturerID = currentProduct.ManufacturerID;
                    existing.Description = currentProduct.Description;
                    existing.QuantityInStock = currentProduct.QuantityInStock;
                    existing.Unit = currentProduct.Unit;
                    existing.CostPrice = currentProduct.CostPrice;
                    existing.RetailPrice = currentProduct.RetailPrice;
                    existing.MinStock = currentProduct.MinStock;
                }
                else
                    AppConnect.context.Products.Add(currentProduct);
            }

            try
            {
                AppConnect.context.SaveChanges();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения: " + ex.Message);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}