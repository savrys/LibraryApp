using System;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;
using LibraryApp.Data;

namespace LibraryApp
{
    public partial class AuthorsWindow : Window
    {
        private LibraryContext _context;

        public AuthorsWindow(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAuthors();
        }

        private void LoadAuthors()
        {
            AuthorsDataGrid.ItemsSource = _context.Authors.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AuthorEditWindow(_context);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
                LoadAuthors();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuthorsDataGrid.SelectedItem is Author selected)
            {
                var dialog = new AuthorEditWindow(_context, selected);
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                    LoadAuthors();
            }
            else
            {
                MessageBox.Show("Выберите автора для редактирования.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (AuthorsDataGrid.SelectedItem is Author selected)
            {
                // Проверим, есть ли у автора книги
                bool hasBooks = _context.Books.Any(b => b.AuthorId == selected.Id);
                if (hasBooks)
                {
                    MessageBox.Show("Нельзя удалить автора, у которого есть книги.");
                    return;
                }

                var result = MessageBox.Show($"Удалить автора {selected.FirstName} {selected.LastName}?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Authors.Remove(selected);
                    _context.SaveChanges();
                    LoadAuthors();
                }
            }
            else
            {
                MessageBox.Show("Выберите автора для удаления.");
            }
        }
    }
}