using System.ComponentModel.DataAnnotations;

namespace DotNetCoreApp1.Models.Types
{
    public class RecordDto
    {
        [Key]
        public required Guid RecordId { get; set; }
        public required Guid BookId { get; set; }
        public required DateTime LendedFrom { get; set; }
        public required DateTime LendedTo { get; set; }
        public string[]? Tags { get; set; }
        public bool SendNotification { get; set; }
    }
}
