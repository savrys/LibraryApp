namespace LibraryApp.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;       // инициализация
        public int PublishYear { get; set; }
        public string ISBN { get; set; } = string.Empty;        // инициализация
        public int QuantityInStock { get; set; }

        // Внешние ключи
        public int AuthorId { get; set; }
        public int GenreId { get; set; }

        // Навигационные свойства – используем null! (подавляем предупреждение,
        // так как EF Core заполнит их при загрузке)
        public Author Author { get; set; } = null!;
        public Genre Genre { get; set; } = null!;
    }
}