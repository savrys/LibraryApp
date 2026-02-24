using System.Linq;
using System.Windows;
using LibraryApp.Models;
using LibraryApp.Data;

namespace LibraryApp
{
    public partial class GenresWindow : Window
    {
        private LibraryContext _context;

        public GenresWindow(LibraryContext context)
        {
            InitializeComponent();
            _context = context;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadGenres();
        }

        private void LoadGenres()
        {
            GenresDataGrid.ItemsSource = _context.Genres.ToList();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new GenreEditWindow(_context);
            dialog.Owner = this;
            if (dialog.ShowDialog() == true)
                LoadGenres();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (GenresDataGrid.SelectedItem is Genre selected)
            {
                var dialog = new GenreEditWindow(_context, selected);
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                    LoadGenres();
            }
            else
            {
                MessageBox.Show("Выберите жанр для редактирования.");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (GenresDataGrid.SelectedItem is Genre selected)
            {
                // Проверим, есть ли книги этого жанра
                bool hasBooks = _context.Books.Any(b => b.GenreId == selected.Id);
                if (hasBooks)
                {
                    // По нашей настройке каскадного удаления книги удалятся вместе с жанром
                    // Спросим подтверждение
                    var result = MessageBox.Show($"Удаление жанра приведёт к удалению всех книг этого жанра. Продолжить?",
                                                  "Предупреждение", MessageBoxButton.YesNo);
                    if (result != MessageBoxResult.Yes)
                        return;
                }

                _context.Genres.Remove(selected);
                _context.SaveChanges();
                LoadGenres();
            }
            else
            {
                MessageBox.Show("Выберите жанр для удаления.");
            }
        }
    }
}