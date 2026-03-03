using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using LibraryApp.Models;
using LibraryApp.Data;

namespace LibraryApp
{
    public partial class MainWindow : Window
    {
        private LibraryContext _context;
        private List<Book> _books = new List<Book>();

        public MainWindow()
        {
            InitializeComponent();
            _context = new LibraryContext();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadGenres();
                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}\n\nПодробности: {ex.StackTrace}");
            }
        }

        private void LoadGenres()
        {
            var genres = _context.Genres.ToList();
            genres.Insert(0, new Genre { Id = 0, Name = "Все жанры" });
            GenreFilterComboBox.ItemsSource = genres;
            GenreFilterComboBox.SelectedIndex = 0;
        }

        private void LoadBooks()
        {
            _books = _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .ToList();
            BooksDataGrid.ItemsSource = _books;
            UpdateTotalBooks();
        }

        private void UpdateTotalBooks()
        {
            int total = _books.Sum(b => b.QuantityInStock);
            TotalBooksTextBlock.Text = $"Всего книг: {total}";
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            var query = _context.Books
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .AsQueryable();

            string searchText = SearchTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(b => b.Title.Contains(searchText));
            }

            if (GenreFilterComboBox.SelectedValue is int genreId && genreId != 0)
            {
                query = query.Where(b => b.BookGenres.Any(bg => bg.GenreId == genreId));
            }

            _books = query.ToList();
            BooksDataGrid.ItemsSource = _books;
            UpdateTotalBooks();
        }

        private void AuthorsButton_Click(object sender, RoutedEventArgs e)
        {
            var authorsWindow = new AuthorsWindow(_context);
            authorsWindow.Owner = this;
            authorsWindow.ShowDialog();
            LoadBooks();
            LoadGenres();
        }

        private void GenresButton_Click(object sender, RoutedEventArgs e)
        {
            var genresWindow = new GenresWindow(_context);
            genresWindow.Owner = this;
            genresWindow.ShowDialog();
            LoadBooks();
            LoadGenres();
        }

        private void AddBookButton_Click(object sender, RoutedEventArgs e)
        {
            var bookWindow = new BookEditWindow(_context);
            bookWindow.Owner = this;
            bookWindow.ShowDialog();
            LoadBooks();
        }

        private void EditBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var bookWindow = new BookEditWindow(_context, selectedBook);
                bookWindow.Owner = this;
                bookWindow.ShowDialog();
                LoadBooks();
            }
            else
            {
                MessageBox.Show("Выберите книгу для редактирования.");
            }
        }

        private void DeleteBookButton_Click(object sender, RoutedEventArgs e)
        {
            if (BooksDataGrid.SelectedItem is Book selectedBook)
            {
                var result = MessageBox.Show($"Удалить книгу '{selectedBook.Title}'?", "Подтверждение", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _context.Books.Remove(selectedBook);
                    _context.SaveChanges();
                    LoadBooks();
                }
            }
            else
            {
                MessageBox.Show("Выберите книгу для удаления.");
            }
        }
    }
}