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
    public partial class SaleEditWindow : Window
    {
        private int? saleId;
        private int currentUserId;
        private List<SaleItemTemp> items = new List<SaleItemTemp>();

        public SaleEditWindow(int? id, int userId)
        {
            InitializeComponent();
            saleId = id;
            currentUserId = userId;
            LoadCustomers();
            if (saleId.HasValue)
                LoadSaleData();
        }

        private void LoadCustomers()
        {
            var customers = AppConnect.context.Customers.OrderBy(c => c.LastName).ToList();
            cmbCustomer.ItemsSource = customers;
            cmbCustomer.SelectedValuePath = "CustomerID";
        }

        private void LoadSaleData()
        {
            var sale = AppConnect.context.Sales.Find(saleId.Value);
            if (sale != null)
            {
                cmbCustomer.SelectedValue = sale.CustomerID;
                var saleItems = AppConnect.context.SaleItems.Where(si => si.SaleID == saleId.Value)
                    .Select(si => new SaleItemTemp
                    {
                        ProductID = si.ProductID,
                        ProductName = si.Products.ProductName,
                        Quantity = si.Quantity,
                        Price = si.PriceAtSale
                    }).ToList();
                items = saleItems;
                RefreshItemsGrid();
            }
        }

        private void BtnSelectProducts_Click(object sender, RoutedEventArgs e)
        {
            var win = new SelectProductsWindow(items.Select(i => i.ProductID).ToList());
            if (win.ShowDialog() == true)
            {
                items = win.SelectedItems;
                RefreshItemsGrid();
            }
        }

        private void RefreshItemsGrid()
        {
            dgItems.ItemsSource = null;
            dgItems.ItemsSource = items;
            txtTotal.Text = items.Sum(i => i.Total).ToString("C2");
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn?.Tag as SaleItemTemp;
            if (item != null)
            {
                items.Remove(item);
                RefreshItemsGrid();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbCustomer.SelectedValue == null)
            {
                MessageBox.Show("Выберите клиента.");
                return;
            }
            if (items.Count == 0)
            {
                MessageBox.Show("Добавьте товары.");
                return;
            }

            int customerId = (int)cmbCustomer.SelectedValue;
            decimal totalAmount = items.Sum(i => i.Total);

            if (!saleId.HasValue)
            {
                var sale = new Sales
                {
                    CustomerID = customerId,
                    SaleDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    PaymentMethod = "Наличные",
                    UserID = currentUserId
                };
                AppConnect.context.Sales.Add(sale);
                AppConnect.context.SaveChanges();

                foreach (var item in items)
                {
                    var saleItem = new SaleItems
                    {
                        SaleID = sale.SaleID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        PriceAtSale = item.Price
                    };
                    AppConnect.context.SaleItems.Add(saleItem);
                    var product = AppConnect.context.Products.Find(item.ProductID);
                    if (product != null)
                        product.QuantityInStock -= item.Quantity;
                }
            }
            else
            {
                var sale = AppConnect.context.Sales.Find(saleId.Value);
                if (sale != null)
                {
                    var oldItems = AppConnect.context.SaleItems.Where(si => si.SaleID == saleId.Value).ToList();
                    foreach (var old in oldItems)
                    {
                        var prod = AppConnect.context.Products.Find(old.ProductID);
                        if (prod != null) prod.QuantityInStock += old.Quantity;
                    }
                    AppConnect.context.SaleItems.RemoveRange(oldItems);

                    sale.CustomerID = customerId;
                    sale.TotalAmount = totalAmount;
                    sale.PaymentMethod = "Наличные";
                    AppConnect.context.SaveChanges();

                    foreach (var item in items)
                    {
                        var saleItem = new SaleItems
                        {
                            SaleID = sale.SaleID,
                            ProductID = item.ProductID,
                            Quantity = item.Quantity,
                            PriceAtSale = item.Price
                        };
                        AppConnect.context.SaleItems.Add(saleItem);
                        var product = AppConnect.context.Products.Find(item.ProductID);
                        if (product != null) product.QuantityInStock -= item.Quantity;
                    }
                }
            }
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

    public class SaleItemTemp
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }
}
