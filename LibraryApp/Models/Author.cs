using System;
using System.Collections.Generic;

namespace LibraryApp.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;   // инициализация
        public string LastName { get; set; } = string.Empty;    // инициализация
        public DateTime BirthDate { get; set; }
        public string Country { get; set; } = string.Empty;     // инициализация

        // Навигационное свойство – инициализируем пустым списком, чтобы не было null
        public ICollection<Book> Books { get; set; } = new List<Book>();

        public string FullName => $"{FirstName} {LastName}";
    }
}