using Microsoft.EntityFrameworkCore;
using MyLibrary.Data.Entities;

namespace MyLibrary.Data;

public class MyLibraryDbContext : DbContext
{
    public MyLibraryDbContext(DbContextOptions<MyLibraryDbContext> options) 
        : base(options)
    {

    }

    public DbSet<Book> Books { get; set; }
}
