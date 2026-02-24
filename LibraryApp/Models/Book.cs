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

        // Внешний ключ для жанра
        public int GenreId { get; set; }

        // Навигационные свойства
        public Genre Genre { get; set; } = null!;

        // Связь многие-ко-многим с авторами
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        // ДОБАВЬТЕ ЭТО СВОЙСТВО ЗДЕСЬ:
        public string AuthorsDisplay => BookAuthors != null
            ? string.Join(", ", BookAuthors.Select(ba => ba.Author?.FullName ?? "Неизвестный автор"))
            : "";
    }
}