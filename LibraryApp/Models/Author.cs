using System;
using System.Collections.Generic;

namespace LibraryApp.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Country { get; set; } = string.Empty;

        // Связь многие-ко-многим с книгами
        public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

        public string FullName => $"{FirstName} {LastName}";
    }
}