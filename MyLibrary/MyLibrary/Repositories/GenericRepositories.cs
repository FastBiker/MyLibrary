using MyLibrary.Entities;

namespace MyLibrary.Repositories;

public class GenericRepositories<T> where T : IEntity
{
    private readonly List<T> _items = new ();

    public void Add(T item) 
    {
        item.Id = _items.Count +1;
        _items.Add(item);
    }

    public void Save() 
    {
        foreach(var peterBook in _items) 
        {
            Console.WriteLine(peterBook);
        }
    }

    public T GetById(int id) 
    {
        return _items.Single(item => item.Id == id);
    }
}
