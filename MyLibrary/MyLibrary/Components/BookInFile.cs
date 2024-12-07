using MyLibrary.Entities;
using MyLibrary.Repositories;
using System.Text.Json;

namespace MyLibrary.Components;

public class BookInFile<T> : IRepository<T> where T : class, IEntity, new()
{
    private const string fileName = "mylibrary.json";
    private readonly Action<T>? _itemAddedCallback;
    private static int lastId = 0;

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
            return JsonSerializer.Deserialize<List<T>>(jsonString) ?? new List<T>();
        }
        else
        {
            new Exception("File doesn't exist or is corrupt");
            return new List<T>();
        }
    }

    public T? GetById(int id)
    {
        throw new NotImplementedException();
    }

    public void Add(T item)
    {
        List<T> items;
        if (File.Exists(fileName))
        {
            var json = File.ReadAllText(fileName);
            items = string.IsNullOrWhiteSpace(json)
                ? new List<T>()
                : JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();           
        }
        else
        {
            items = new List<T>();
        }

        lastId = items.Count > 0
            ? items.Max(x => x.Id)
            : 0;

        item.Id = ++lastId;

        items.Add(item);
        var newJson = JsonSerializer.Serialize(items);
        File.WriteAllText(fileName, newJson);
        _itemAddedCallback?.Invoke(item);
        ItemAdded?.Invoke(this, item);
    }

    public void Remove(T item)
    {
        throw new NotImplementedException();
    }

    public void Save()
    {
        Console.WriteLine("Książka została zapisana w pliku 'mylirary.json'");
        // Save is not required with List
    }
}

//Oto zmodyfikowana wersja metody `Add` i przykładowa implementacja:

//```csharp
//private static int lastId = 0; // Do przechowywania ostatniego Id

//public void Add(Book item)
//{
//    List<Book> items;
//    if (File.Exists(fileName))
//    {
//        var json = File.ReadAllText(fileName);
//        items = string.IsNullOrWhiteSpace(json)
//            ? new List<Book>()
//            : JsonSerializer.Deserialize<List<Book>>(json) ?? new List<Book>();
//    }
//    else
//    {
//        items = new List<Book>();
//    }

//    // Ustaw nowe Id
//    lastId = items.Count > 0 ? items.Max(b => b.Id) : 0;
//    item.Id = ++lastId; // Zwiększ Id i przypisz do item

//    items.Add(item);
//    var newJson = JsonSerializer.Serialize(items);
//    File.WriteAllText(fileName, newJson);
//    _itemAddedCallback?.Invoke(item);
//    ItemAdded?.Invoke(this, item);
//}
