using System;
using System.Windows;
using LibraryApp.Models;
using LibraryApp.Data;

namespace LibraryApp
{
    public partial class AuthorEditWindow : Window
    {
        private LibraryContext _context;
        private Author _currentAuthor;

        public AuthorEditWindow(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
            _currentAuthor = new Author();
            BirthDatePicker.SelectedDate = DateTime.Today;
        }

        public AuthorEditWindow(LibraryContext context, Author author) : this(context)
        {
            _currentAuthor = author;
            FirstNameTextBox.Text = author.FirstName;
            LastNameTextBox.Text = author.LastName;
            BirthDatePicker.SelectedDate = author.BirthDate;
            CountryTextBox.Text = author.Country;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Введите имя автора.");
                return;
            }

            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
            {
                MessageBox.Show("Введите фамилию автора.");
                return;
            }

            if (BirthDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату рождения.");
                return;
            }

            // Проверка имени и фамилии (только буквы и тире)
            string firstName = FirstNameTextBox.Text.Trim();
            string lastName = LastNameTextBox.Text.Trim();
            string country = CountryTextBox.Text.Trim();

            if (!IsValidName(firstName))
            {
                MessageBox.Show("Имя может содержать только буквы и тире.");
                return;
            }

            if (!IsValidName(lastName))
            {
                MessageBox.Show("Фамилия может содержать только буквы и тире.");
                return;
            }

            // Проверка страны
            if (string.IsNullOrWhiteSpace(country))
            {
                MessageBox.Show("Введите страну.");
                return;
            }

            // Список реальных стран (можно расширить)
            string[] validCountries = new string[]
            {
        "Россия", "Украина", "Беларусь", "Казахстан", "США", "Канада", "Мексика",
        "Бразилия", "Аргентина", "Великобритания", "Франция", "Германия", "Италия",
        "Испания", "Португалия", "Нидерланды", "Бельгия", "Швейцария", "Австрия",
        "Польша", "Чехия", "Словакия", "Венгрия", "Болгария", "Румыния", "Греция",
        "Швеция", "Норвегия", "Финляндия", "Дания", "Исландия", "Япония", "Китай",
        "Индия", "Корея", "Вьетнам", "Таиланд", "Турция", "Израиль", "Египет",
        "ЮАР", "Австралия", "Новая Зеландия"
            };

            // Проверяем, есть ли страна в списке (регистронезависимо)
            bool isValidCountry = false;
            foreach (string validCountry in validCountries)
            {
                if (string.Equals(country, validCountry, StringComparison.OrdinalIgnoreCase))
                {
                    isValidCountry = true;
                    country = validCountry; // Используем правильное написание из списка
                    break;
                }
            }

            if (!isValidCountry)
            {
                string countryList = string.Join(", ", validCountries);
                MessageBox.Show($"Укажите реальную страну из списка:\n{countryList}");
                return;
            }

            // Если всё хорошо, сохраняем
            _currentAuthor.FirstName = firstName;
            _currentAuthor.LastName = lastName;
            _currentAuthor.BirthDate = BirthDatePicker.SelectedDate.Value;
            _currentAuthor.Country = country;

            if (_currentAuthor.Id == 0)
                _context.Authors.Add(_currentAuthor);
            else
                _context.Authors.Update(_currentAuthor);

            _context.SaveChanges();
            DialogResult = true;
            Close();
        }

        // Вспомогательный метод для проверки имени (только буквы и тире)
        private bool IsValidName(string name)
        {
            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != '-')
                {
                    return false;
                }
            }
            return !string.IsNullOrWhiteSpace(name);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}