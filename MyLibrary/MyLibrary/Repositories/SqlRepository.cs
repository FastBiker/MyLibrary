using Microsoft.EntityFrameworkCore;
using MyLibrary.Entities;

namespace MyLibrary.Repositories;

public class SqlRepository
{
    private readonly DbSet<Book> _dbset;
    private readonly DbContext _dbContext;

    public SqlRepository(DbContext dbContext) 
    {
        _dbContext = dbContext;
        _dbset = _dbContext.Set<Book>();
    }

    public Book? GetById(int id) 
    {
        return _dbset.Find(id);
    }

    public void Add(Book item) 
    {
        _dbset.Add(item);
    }

    public void Remove(Book item) 
    {
        _dbset.Remove(item);
    }

    public void Save() 
    {
        _dbContext.SaveChanges();
    }
}
