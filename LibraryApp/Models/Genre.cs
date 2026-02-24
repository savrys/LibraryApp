using System.Collections.Generic;

namespace LibraryApp.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;        // инициализация
        public string Description { get; set; } = string.Empty; // инициализация

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}