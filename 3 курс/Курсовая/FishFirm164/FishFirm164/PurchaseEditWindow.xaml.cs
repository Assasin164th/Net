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
    public partial class PurchaseEditWindow : Window
    {
        private int? purchaseId;
        private int currentUserId;
        private List<PurchaseItemTemp> items = new List<PurchaseItemTemp>();

        public PurchaseEditWindow(int? id, int userId)
        {
            InitializeComponent();
            purchaseId = id;
            currentUserId = userId;
            LoadSuppliers();
            if (purchaseId.HasValue)
                LoadPurchaseData();
        }

        private void LoadSuppliers()
        {
            cmbSupplier.ItemsSource = AppConnect.context.Suppliers.ToList();
        }

        private void LoadPurchaseData()
        {
            var purchase = AppConnect.context.Purchases.Find(purchaseId.Value);
            if (purchase != null)
            {
                cmbSupplier.SelectedValue = purchase.SupplierID;
                var purchaseItems = AppConnect.context.PurchaseItems
                    .Where(pi => pi.PurchaseID == purchaseId.Value)
                    .Select(pi => new PurchaseItemTemp
                    {
                        ProductID = pi.ProductID,
                        ProductName = pi.Products.ProductName,
                        Quantity = pi.Quantity,
                        Price = pi.PricePerUnit
                    }).ToList();
                items = purchaseItems;
                RefreshItemsGrid();
            }
        }

        private void BtnSelectProducts_Click(object sender, RoutedEventArgs e)
        {
            var win = new SelectProductsForPurchaseWindow(items.Select(i => i.ProductID).ToList());
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
            var item = btn?.Tag as PurchaseItemTemp;
            if (item != null)
            {
                items.Remove(item);
                RefreshItemsGrid();
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbSupplier.SelectedValue == null)
            {
                MessageBox.Show("Выберите поставщика.");
                return;
            }
            if (items.Count == 0)
            {
                MessageBox.Show("Добавьте товары.");
                return;
            }

            int supplierId = (int)cmbSupplier.SelectedValue;
            decimal totalAmount = items.Sum(i => i.Total);

            if (!purchaseId.HasValue)
            {
                var purchase = new Purchases
                {
                    SupplierID = supplierId,
                    PurchaseDate = DateTime.Now,
                    TotalAmount = totalAmount
                };
                AppConnect.context.Purchases.Add(purchase);
                AppConnect.context.SaveChanges();

                foreach (var item in items)
                {
                    var purchaseItem = new PurchaseItems
                    {
                        PurchaseID = purchase.PurchaseID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        PricePerUnit = item.Price
                    };
                    AppConnect.context.PurchaseItems.Add(purchaseItem);
                    var product = AppConnect.context.Products.Find(item.ProductID);
                    if (product != null)
                        product.QuantityInStock += item.Quantity;
                }
            }
            else
            {
                var purchase = AppConnect.context.Purchases.Find(purchaseId.Value);
                if (purchase != null)
                {
                    var oldItems = AppConnect.context.PurchaseItems.Where(pi => pi.PurchaseID == purchaseId.Value).ToList();
                    foreach (var old in oldItems)
                    {
                        var prod = AppConnect.context.Products.Find(old.ProductID);
                        if (prod != null) prod.QuantityInStock -= old.Quantity;
                    }
                    AppConnect.context.PurchaseItems.RemoveRange(oldItems);

                    purchase.SupplierID = supplierId;
                    purchase.TotalAmount = totalAmount;
                    AppConnect.context.SaveChanges();

                    foreach (var item in items)
                    {
                        var purchaseItem = new PurchaseItems
                        {
                            PurchaseID = purchase.PurchaseID,
                            ProductID = item.ProductID,
                            Quantity = item.Quantity,
                            PricePerUnit = item.Price
                        };
                        AppConnect.context.PurchaseItems.Add(purchaseItem);
                        var product = AppConnect.context.Products.Find(item.ProductID);
                        if (product != null) product.QuantityInStock += item.Quantity;
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

    public class PurchaseItemTemp
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Quantity * Price;
    }
}
