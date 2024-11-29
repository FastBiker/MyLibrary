using MyLibrary.Entities;
using MyLibrary.Repositories;
using System.Text.Json;

namespace MyLibrary.Components;

public class BookInFile<T> : IRepository<T> where T : class, IEntity, new()
{
    private const string fileName = "mylibrary.json";
    private readonly Action<T>? _itemAddedCallback;

    public BookInFile(Action<T>? itemAddedCallback = null)
    {
        _itemAddedCallback = itemAddedCallback;
    }

    public event EventHandler<T> ItemAdded;

    public IEnumerable<T> GetAll()
    {
        if (File.Exists(fileName))
        {
            var jsonString = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<List<T>>(jsonString)?? new List<T>();
        }
        else
        {
            Console.WriteLine("File doesn't exist");
            return new List<T>();
        }
    }

    public T? GetById(int id)
    {
        throw new NotImplementedException();
    }

    public void Add(T item)
    {
        var json = JsonSerializer.Serialize<T>(item);
        File.AppendAllText(fileName, json + Environment.NewLine);
        _itemAddedCallback?.Invoke(item);
        ItemAdded?.Invoke(this, item);
    }

    public void Remove(T item)
    {
        throw new NotImplementedException();
    }

    public void Save()
    {
        throw new NotImplementedException();
    }
}
