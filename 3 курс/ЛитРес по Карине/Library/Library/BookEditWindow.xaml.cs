using Library.classes;
using Microsoft.Win32;
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
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.ComponentModel;

namespace Library
{
    public partial class BookEditWindow : Window
    {
        public Book CurrentBook { get; set; }

        public string WindowName
        {
            get
            {
                return CurrentBook.Id == 0 ? "Новая книга" : "Редактировагие книги";
            }
        }
        private bool isNew;

        public BookEditWindow(Book book)
        {
            InitializeComponent();
            DataContext = this;
            CurrentBook = book;

            using (var context = new BookStoreEntities())
            {
                cmbPublisher = context.Publisher.ToList();
            }
            LoadComboBoxes();
        }

        

        private void LoadComboBoxes()
        {
            cmbGenre.ItemsSource = AppConnect.context.Genre.OrderBy(c => c.Name).ToList();
            cmbPublisher.ItemsSource = AppConnect.context.Publisher.OrderBy(m => m.Name).ToList();

            if (CurrentBook.IsAvailable)
            {
                chkIsAvailable.IsChecked = true;
            }
        }

        private void Invalidate(string ComponentName = "ProductList")
        {
            if (PropertyChanged != null)
                PropertyChanged(
                    this,
                    new PropertyChangedEventArgs(ComponentName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            using (var context = new BookStoreEntities())
            {
                // Вся работа с БД должна быть завёрнута в исключения
                try
                {
                    Book book = null;
                    if (CurrentBook.Id != 0)
                        book = context.Book.Find(CurrentBook.Id);
                    else
                        book = new Book();

                    if (book != null)
                    {
                        // Добавляем проверки
                        book.Name = CurrentBook.Name;
                        book.Author = CurrentBook.Author;
                        book.Publisher = CurrentBook.Publisher;
                        book.Price = CurrentBook.Price;
                        book.IsAvailable = CurrentBook.IsAvailable;
                        if (book.Id == 0)
                            context.Book.Add(book);
                        else
                            context.Book.AddOrUpdate(book);

                        if (context.SaveChanges() > 0)
                        {
                            DialogResult = true;
                        }

                    }
                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null)
                        MessageBox.Show(ex.InnerException.Message);
                    else MessageBox.Show(ex.Message);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
