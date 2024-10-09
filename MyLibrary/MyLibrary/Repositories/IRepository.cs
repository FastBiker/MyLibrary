using MyLibrary.Entities;

namespace MyLibrary.Repositories;

public interface IRepository<T> : IReadRepository<T>, IWriteRepository<T>
    where T : class, IEntity
{
}
