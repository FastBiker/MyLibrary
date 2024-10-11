using Microsoft.EntityFrameworkCore;
using MyLibrary.Entities;

namespace MyLibrary.Data;

public class MyLibraryDbContext : DbContext
{
    public DbSet<Book> Books => Set<Book>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseInMemoryDatabase("StorageBookAppDb");
    }
}
