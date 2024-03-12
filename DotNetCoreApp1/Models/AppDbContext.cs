using DotNetCoreApp1.Models.Types;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DotNetCoreApp1.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<BookDto> Books { get; set; }
        public virtual DbSet<RecordDto> Records { get; set; }
        public virtual DbSet<UserDto> Users { get; set; }
        public virtual DbSet<PasswordDto> Passwords { get; set; }
    }
}
