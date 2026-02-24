using System;
using System.Linq;
using System.Windows;
using LibraryApp.Models;
using LibraryApp.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore; // Добавлено для работы с Entity Framework

namespace LibraryApp
{
    public partial class BookEditWindow : Window
    {
        private LibraryContext _context;
        private Book _currentBook;
        private List<Author> _allAuthors; // Добавлено для хранения списка авторов

        // Конструктор для добавления
        public BookEditWindow(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
            LoadAuthorsAndGenres();
            _currentBook = new Book();
            DataContext = _currentBook;
        }

        // Конструктор для редактирования
        public BookEditWindow(LibraryContext context, Book book)
        {
            InitializeComponent();
            _context = context;
            LoadAuthorsAndGenres();
            _currentBook = book;

            // Заполняем поля
            TitleTextBox.Text = _currentBook.Title;
            GenreComboBox.SelectedValue = _currentBook.GenreId;
            PublishYearTextBox.Text = _currentBook.PublishYear.ToString();
            ISBNTextBox.Text = _currentBook.ISBN;
            QuantityTextBox.Text = _currentBook.QuantityInStock.ToString();

            // Выделяем авторов книги
            if (_currentBook.BookAuthors != null && AuthorsListBox.Items.Count > 0)
            {
                var authorIds = _currentBook.BookAuthors.Select(ba => ba.AuthorId).ToList();
                for (int i = 0; i < AuthorsListBox.Items.Count; i++)
                {
                    var author = AuthorsListBox.Items[i] as Author;
                    if (author != null && authorIds.Contains(author.Id))
                    {
                        AuthorsListBox.SelectedItems.Add(author);
                    }
                }
            }
        }

        private void LoadAuthorsAndGenres()
        {
            // Загружаем всех авторов
            _allAuthors = _context.Authors.ToList();
            AuthorsListBox.ItemsSource = _allAuthors;

            // Загружаем жанры
            GenreComboBox.ItemsSource = _context.Genres.ToList();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения полей
            if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
            {
                MessageBox.Show("Введите название книги.");
                return;
            }

            // Проверка выбора авторов
            if (AuthorsListBox.SelectedItems.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы одного автора.");
                return;
            }

            if (GenreComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите жанр.");
                return;
            }

            // Проверка года
            if (!int.TryParse(PublishYearTextBox.Text, out int year))
            {
                MessageBox.Show("Год издания должен быть числом.");
                return;
            }

            int currentYear = DateTime.Now.Year;
            if (year < 1000 || year > currentYear + 1)
            {
                MessageBox.Show($"Год издания должен быть от 1000 до {currentYear + 1}.");
                return;
            }

            // Проверка ISBN
            string isbn = ISBNTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(isbn))
            {
                MessageBox.Show("Введите ISBN.");
                return;
            }

            string validationMessage = ValidateISBN(isbn);
            if (validationMessage != null)
            {
                MessageBox.Show(validationMessage);
                return;
            }

            // Проверка количества
            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Количество должно быть неотрицательным числом.");
                return;
            }

            // Сохраняем основные данные книги
            _currentBook.Title = TitleTextBox.Text.Trim();
            _currentBook.GenreId = (int)GenreComboBox.SelectedValue;
            _currentBook.PublishYear = year;
            _currentBook.ISBN = isbn;
            _currentBook.QuantityInStock = quantity;

            // Сохраняем или обновляем книгу
            if (_currentBook.Id == 0)
                _context.Books.Add(_currentBook);
            else
                _context.Books.Update(_currentBook);

            _context.SaveChanges(); // Сохраняем, чтобы получить Id для новой книги

            // Обновляем связи с авторами
            // Удаляем старые связи
            var existingLinks = _context.BookAuthors.Where(ba => ba.BookId == _currentBook.Id);
            _context.BookAuthors.RemoveRange(existingLinks);

            // Добавляем новые связи
            foreach (Author selectedAuthor in AuthorsListBox.SelectedItems)
            {
                _context.BookAuthors.Add(new BookAuthor
                {
                    BookId = _currentBook.Id,
                    AuthorId = selectedAuthor.Id
                });
            }

            _context.SaveChanges(); // Сохраняем связи
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // Метод для проверки ISBN
        private string ValidateISBN(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                return "Введите ISBN.";
            }

            isbn = isbn.Trim();

            // Проверка формата ISBN-13
            if (Regex.IsMatch(isbn, @"^\d{3}-\d{1,5}-\d{1,7}-\d{1,6}-\d$"))
            {
                string[] parts = isbn.Split('-');

                if (parts[0] != "978" && parts[0] != "979")
                {
                    return "ISBN-13 должен начинаться с 978 или 979.";
                }

                for (int i = 0; i < parts.Length; i++)
                {
                    if (!parts[i].All(char.IsDigit))
                    {
                        return $"Часть {i + 1} ISBN должна содержать только цифры.";
                    }
                }

                return null; // ISBN-13 корректен
            }

            // Проверка формата ISBN-10
            else if (Regex.IsMatch(isbn, @"^\d{1,5}-\d{1,7}-\d{1,6}-[\dXx]$"))
            {
                string[] parts = isbn.Split('-');

                for (int i = 0; i < 3; i++)
                {
                    if (!parts[i].All(char.IsDigit))
                    {
                        return $"Часть {i + 1} ISBN должна содержать только цифры.";
                    }
                }

                string lastPart = parts[3].ToUpper();
                if (lastPart.Length != 1 || (!char.IsDigit(lastPart[0]) && lastPart != "X"))
                {
                    return "Последняя часть ISBN-10 должна быть одной цифрой или X.";
                }

                return null; // ISBN-10 корректен
            }

            return "Неверный формат ISBN. Используйте формат: 978-5-17-148855-4 (ISBN-13) или 5-17-148855-5 (ISBN-10)";
        }
    }
}