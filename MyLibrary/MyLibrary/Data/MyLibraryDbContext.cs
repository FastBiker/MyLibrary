using Microsoft.EntityFrameworkCore;
using MyLibrary.Data.Entities;

namespace MyLibrary.Data;

public class MyLibraryDbContext : DbContext
{
    public MyLibraryDbContext(DbContextOptions<MyLibraryDbContext> options) 
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlServer("Data Source=LAPTOP-R6OVM9N5\\SQLEXPRESS;Initial Catalog=MyLibraryStorage;Integrated Security=True;Trust Server Certificate=True")
            .EnableSensitiveDataLogging();
    }

    public DbSet<Book> Books {  get; set; }

    //public DbSet<BookToCsv> Books { get; set; }

}
