using DotNetCoreApp1.Models.Types;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DotNetCoreApp1.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BookDto> Books { get; set; }
        public DbSet<RecordDto> Records { get; set; }
        public DbSet<UserDto> Users { get; set; }
        public DbSet<PasswordDto> Passwords { get; set; }
    }
}
