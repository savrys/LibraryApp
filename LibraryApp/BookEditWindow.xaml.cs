using System.Linq;
using System.Windows;
using LibraryApp.Models;
using LibraryApp.Data;
using System.Text.RegularExpressions; // Добавлено для регулярных выражений

namespace LibraryApp
{
    public partial class BookEditWindow : Window
    {
        private LibraryContext _context;
        private Book _currentBook;

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
            AuthorComboBox.SelectedValue = _currentBook.AuthorId;
            GenreComboBox.SelectedValue = _currentBook.GenreId;
            PublishYearTextBox.Text = _currentBook.PublishYear.ToString();
            ISBNTextBox.Text = _currentBook.ISBN;
            QuantityTextBox.Text = _currentBook.QuantityInStock.ToString();
        }

        private void LoadAuthorsAndGenres()
        {
            AuthorComboBox.ItemsSource = _context.Authors.ToList();
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

            if (AuthorComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите автора.");
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

            // ПРОВЕРКА ISBN
            string isbn = ISBNTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(isbn))
            {
                MessageBox.Show("Введите ISBN.");
                return;
            }

            // Проверка формата ISBN (передаём исходную строку с дефисами)
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

            // Если всё хорошо, сохраняем
            _currentBook.Title = TitleTextBox.Text.Trim();
            _currentBook.AuthorId = (int)AuthorComboBox.SelectedValue;
            _currentBook.GenreId = (int)GenreComboBox.SelectedValue;
            _currentBook.PublishYear = year;
            _currentBook.ISBN = isbn; // сохраняем исходный ISBN с дефисами
            _currentBook.QuantityInStock = quantity;

            if (_currentBook.Id == 0)
                _context.Books.Add(_currentBook);
            else
                _context.Books.Update(_currentBook);

            _context.SaveChanges();
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

            // Убираем лишние пробелы в начале и конце
            isbn = isbn.Trim();

            // Проверка формата ISBN-13 (пример: 978-5-17-148855-4)
            // Шаблон: 3 цифры - 1-5 цифр - 1-7 цифр - 1-6 цифр - 1 цифра
            if (Regex.IsMatch(isbn, @"^\d{3}-\d{1,5}-\d{1,7}-\d{1,6}-\d$"))
            {
                string[] parts = isbn.Split('-');

                // Проверка префикса (должен быть 978 или 979)
                if (parts[0] != "978" && parts[0] != "979")
                {
                    return "ISBN-13 должен начинаться с 978 или 979.";
                }

                // Проверка, что все части содержат только цифры
                for (int i = 0; i < parts.Length; i++)
                {
                    if (!parts[i].All(char.IsDigit))
                    {
                        return $"Часть {i + 1} ISBN должна содержать только цифры.";
                    }
                }

                return null; // ISBN-13 корректен по формату
            }

            // Проверка формата ISBN-10 (пример: 5-17-148855-5)
            // Шаблон: 1-5 цифр - 1-7 цифр - 1-6 цифр - 1 цифра или X
            else if (Regex.IsMatch(isbn, @"^\d{1,5}-\d{1,7}-\d{1,6}-[\dXx]$"))
            {
                string[] parts = isbn.Split('-');

                // Проверка, что первые три части содержат только цифры
                for (int i = 0; i < 3; i++)
                {
                    if (!parts[i].All(char.IsDigit))
                    {
                        return $"Часть {i + 1} ISBN должна содержать только цифры.";
                    }
                }

                // Проверка последней части (цифра или X)
                string lastPart = parts[3].ToUpper();
                if (lastPart.Length != 1 || (!char.IsDigit(lastPart[0]) && lastPart != "X"))
                {
                    return "Последняя часть ISBN-10 должна быть одной цифрой или X.";
                }

                return null; // ISBN-10 корректен по формату
            }

            return "Неверный формат ISBN. Используйте формат: 978-5-17-148855-4 (ISBN-13) или 5-17-148855-5 (ISBN-10)";
        }
    }
}