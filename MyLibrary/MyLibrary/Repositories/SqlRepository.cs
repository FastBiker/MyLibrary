using Microsoft.EntityFrameworkCore;
using MyLibrary.Entities;

namespace MyLibrary.Repositories;

public delegate void ItemAdded<in T>(T item);

public class SqlRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    private readonly DbSet<T> _dbSet;
    private readonly DbContext _dbContext;
    private readonly ItemAdded<T>? _itemAddedCallback;

    public SqlRepository(DbContext dbContext, ItemAdded<T>? itemAddedCallback = null) 
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
        _itemAddedCallback = itemAddedCallback;
    }

    public IEnumerable<T> GetAll() 
    {
        return _dbSet.OrderBy(x => x.Id).ToList();
    }
    public T? GetById(int id) 
    {
        return _dbSet.Find(id);
    }

    public void Add(T item) 
    {
        _dbSet.Add(item);
        _itemAddedCallback?.Invoke(item);
    }

    public void Remove(T item) 
    {
        _dbSet.Remove(item);
    }

    public void Save() 
    {
        _dbContext.SaveChanges();
    }
}
