using MyLibrary.Data.Entities;
using System.Text.Json;

namespace MyLibrary.Data.Repositories;

public class FileRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    private const string fileName = "mylibrary.json";
    private readonly Action<T>? _itemAddedCallback;
    private readonly Action<T>? _itemRemovedCallback;
    private static int lastId = 0;
    protected List<T> _items = [];

    public FileRepository(Action<T>? itemAddedCallback = null, Action<T>? itemRemovedCallback = null)
    {
        _itemAddedCallback = itemAddedCallback;
        _itemRemovedCallback = itemRemovedCallback;
    }

    public event EventHandler<T> ItemAdded;
    public event EventHandler<T> ItemRemoved;

    public IEnumerable<T> GetAll()
    {
        if (File.Exists(fileName))
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var json = streamReader.ReadToEnd();
                if (string.IsNullOrWhiteSpace(json))
                {
                    throw new Exception("You haven't got any books in your library");
                }
                else
                {
                    return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                }
            }
        }
        else
        {
            throw new Exception("File doesn't exist");
        }
    }

    public T? GetById(int id)
    {
        throw new NotImplementedException();
    }

    public void Add(T item)
    {
        if (File.Exists(fileName))
        {
            using (var reader = new StreamReader(fileName))
            {
                var json = reader.ReadToEnd();
                _items = string.IsNullOrWhiteSpace(json)
                    ? new List<T>()
                    : JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
        }
        else
        {
            _items = new List<T>();
        }

        lastId = _items.Count > 0
            ? _items.Max(x => x.Id)
            : 0;

        item.Id = ++lastId;

        _items.Add(item);

        using (var writer = new StreamWriter(fileName))
        {
            var newJson = JsonSerializer.Serialize(_items);
            writer.Write(newJson);
        }
        _itemAddedCallback?.Invoke(item);
        ItemAdded?.Invoke(this, item);
    }

    public void Remove(T item)
    {
        if (File.Exists(fileName))
        {
            using (var streamReader = new StreamReader(fileName))
            {
                var json = streamReader.ReadToEnd();
                _items = string.IsNullOrWhiteSpace(json)
                    ? new List<T>()
                    : JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
        }
        else
        {
            throw new Exception("File doesn't exist");
        }

        var _itemToRemove = _items.FirstOrDefault(x => x.Id == item.Id);
        _items.Remove(_itemToRemove);
        _itemRemovedCallback?.Invoke(item);
        ItemRemoved?.Invoke(this, item);

        using (var streamWriter = new StreamWriter(fileName))
        {
            var newJson = JsonSerializer.Serialize(_items);
            streamWriter.Write(newJson);
        }
    }

    public void Save()
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine("The book was saved in the 'mylibrary.json' file" + Environment.NewLine);
        Console.ResetColor();
        // Save is not required with file
    }

    public void UpdateProperty(T item, Action<T> updateAction)
    {
        throw new NotImplementedException();
    }
}