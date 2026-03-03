using System.Windows;
using LibraryApp.Models;
using LibraryApp.Data;

namespace LibraryApp
{
    public partial class GenreEditWindow : Window
    {
        private LibraryContext _context;
        private Genre _currentGenre;

        public GenreEditWindow(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
            _currentGenre = new Genre();
        }

        public GenreEditWindow(LibraryContext context, Genre genre) : this(context)
        {
            _currentGenre = genre;
            NameTextBox.Text = genre.Name;
            DescriptionTextBox.Text = genre.Description;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Название жанра обязательно.");
                return;
            }

            string newName = NameTextBox.Text.Trim();

            // Проверка на существование жанра с таким же именем
            bool exists = _context.Genres.Any(g => g.Name == newName && g.Id != _currentGenre.Id);
            if (exists)
            {
                MessageBox.Show("Жанр с таким названием уже существует.");
                return;
            }

            _currentGenre.Name = newName;
            _currentGenre.Description = DescriptionTextBox.Text.Trim();

            if (_currentGenre.Id == 0)
                _context.Genres.Add(_currentGenre);
            else
                _context.Genres.Update(_currentGenre);

            _context.SaveChanges();
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}