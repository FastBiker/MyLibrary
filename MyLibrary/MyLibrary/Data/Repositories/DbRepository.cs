using Microsoft.EntityFrameworkCore;
using MyLibrary.Data.Entities;

namespace MyLibrary.Data.Repositories;

public class DbRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    private readonly MyLibraryDbContext _myLibraryDbContext;
    private readonly DbSet<T> _dbSet;
    private readonly Action<T>? _itemAddedCallback;
    private readonly Action<T>? _itemRemovedCallback;
    private readonly Action<T>? _itemUpdatedCallback;

    public DbRepository(MyLibraryDbContext myLibraryDbContext, Action<T>? itemAddedCallback = null, Action<T>? itemRemovedCallback = null, 
        Action<T>? itemUpdatedCallback = null)
    {
        _myLibraryDbContext = myLibraryDbContext;
        _dbSet = _myLibraryDbContext.Set<T>();
        _itemAddedCallback = itemAddedCallback;
        _itemRemovedCallback = itemRemovedCallback;
        _itemUpdatedCallback = itemUpdatedCallback;
    }

    public event EventHandler<T> ItemAdded;
    public event EventHandler<T> ItemRemoved;
    public event EventHandler<T> ItemUpdated;

    public void Add(T item) 
    {
        item.Id = 0;
        _dbSet.Add(item);
        _itemAddedCallback?.Invoke(item);
        ItemAdded?.Invoke(this, item);
    }

    public IEnumerable<T> GetAll()
    {
        var dataFromDb = (IEnumerable<T>)_dbSet.ToList();
        if (dataFromDb != null)
        {
            return dataFromDb;
        }
        else if (dataFromDb == null)
        {
            return new List<T>(); //Enumerable.Empty<T>();
        }
        else
        {
            throw new Exception("Wystąpił problem z pobraniem danych z bazy!");
        }
        
    }

    public T? GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public void Remove(T item)
    {
        _dbSet.Remove(item);
        _itemRemovedCallback?.Invoke(item);
        ItemRemoved?.Invoke(this, item);
    }

    public void UpdateProperty(T item, Action<T> updateAction)
    {
        updateAction(item);
        _itemUpdatedCallback?.Invoke(item);
        ItemUpdated?.Invoke(this, item);
    }

    public void Save()
    {
        _myLibraryDbContext.SaveChanges();
    }
}


//implementacja EventHandler dla UpdateProperty()
//``csharp
//public class YourClass
//{
//    public event EventHandler<ItemUpdatedEventArgs<T>> ItemUpdated;

//    public void UpdateProperty<T>(T item, Action<T> updateAction)
//    {
//        updateAction(item);

//        // Wywołanie zdarzenia
//        OnItemUpdated(new ItemUpdatedEventArgs<T>(item));
//    }

//    protected virtual void OnItemUpdated(ItemUpdatedEventArgs<T> e)
//    {
//        ItemUpdated?.Invoke(this, e);
//    }
//}

//public class ItemUpdatedEventArgs<T> : EventArgs
//{
//    public T Item { get; }

//    public ItemUpdatedEventArgs(T item)
//    {
//        Item = item;
//    }
//}
//```