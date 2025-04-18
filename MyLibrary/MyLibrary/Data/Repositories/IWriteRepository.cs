﻿using MyLibrary.Data.Entities;

namespace MyLibrary.Data.Repositories;

public interface IWriteRepository<in T> where T : class, IEntity
{
    void Add(T item);
    void Remove(T item);
    void UpdateProperty<T>(T item, Action<T> updateAction);
    void Save();
}
