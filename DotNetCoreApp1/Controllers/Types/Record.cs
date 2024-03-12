using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DotNetCoreApp1.Controllers.Types
{
    public class Record
    {
        public string[]? Tags { get; set; }
        public required Guid BookId { get; set; }
        public required DateTime LendedFrom { get; set; }
        public required DateTime LendedTo { get; set; }
        public bool SendNotification { get; set; }
    }
}
