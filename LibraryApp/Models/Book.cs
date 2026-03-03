using System.Collections.Generic;
using System.Linq;

namespace LibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int PublishYear { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }

        // Навигационные свойства
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
        public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();

        // Вычисляемые поля для отображения
        public string AuthorsDisplay => BookAuthors != null && BookAuthors.Any()
            ? string.Join(", ", BookAuthors.Select(ba => ba.Author?.FullName ?? "Неизвестный автор"))
            : "";

        public string GenresDisplay => BookGenres != null && BookGenres.Any()
            ? string.Join(", ", BookGenres.Select(bg => bg.Genre?.Name ?? "Неизвестный жанр"))
            : "";
    }
}