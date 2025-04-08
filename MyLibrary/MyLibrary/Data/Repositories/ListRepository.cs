﻿using MyLibrary.Data.Entities;

namespace MyLibrary.Data.Repositories;

public class ListRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    protected readonly List<T> _items = new();

    public IEnumerable<T> GetAll()
    {
        return _items.ToList();
    }

    public T GetById(int id)
    {
        return _items.Single(item => item.Id == id);
    }
    public void Add(T item)
    {
        item.Id = _items.Count + 1;
        _items.Add(item);
    }
    public void Remove(T item)
    {
        _items.Remove(item);
    }
    public void Save()
    {
        // Save is not required with List
    }

    public void UpdateProperty<T1>(T1 item, Action<T1> updateAction)
    {
        throw new NotImplementedException();
    }
}
