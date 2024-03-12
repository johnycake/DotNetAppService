using System.ComponentModel.DataAnnotations;

namespace DotNetCoreApp1.Models.Types
{
    public class BookDto
    {
        [Key]
        public Guid BookId { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Genre { get; set; }
        public string? Content { get; set; }
    }
}
