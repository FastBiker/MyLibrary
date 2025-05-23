﻿using MyLibrary.Data.Entities;

namespace MyLibrary.Data.Repositories;

public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T>
    where T : class, IEntity
{
    void UpdateProperty(T item, Action<T> updateAction);
}
