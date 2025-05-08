using CsvHelper;
using CsvHelper.Configuration;
using MyLibrary.Components.CsvHandler;
using MyLibrary.Data.Entities;
using System.Globalization;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyLibrary.Data.Repositories;

public class FileRepository<T> : IRepository<T> where T : class, IEntity, new()
{
    private readonly ICsvReader _csvReader;
    private const string jsonFileName = "mylibrary.json";
    private string fullCsvFileName;
    private const string csvFileExtension = ".csv";
    private readonly Action<T>? _itemAddedCallback;
    private readonly Action<T>? _itemRemovedCallback;
    private static int lastId = 0;
    protected List<T> _items = [];

    public FileRepository(ICsvReader csvReader, string fileName,  Action<T>? itemAddedCallback = null, 
        Action<T>? itemRemovedCallback = null)
    {
        _csvReader = csvReader;
        _itemAddedCallback = itemAddedCallback;
        _itemRemovedCallback = itemRemovedCallback;
        fullCsvFileName = $"{fileName}{csvFileExtension}";
    }

    public event EventHandler<T> ItemAdded;
    public event EventHandler<T> ItemRemoved;

    public IEnumerable<T> GetAll()
    {
        if (File.Exists(jsonFileName))
        {
            using (var streamReader = new StreamReader(jsonFileName))
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

    //public void Add(T item) // to json
    //{
    //    if (File.Exists(jsonFileName))
    //    {
    //        using (var reader = new StreamReader(jsonFileName))
    //        {
    //            var json = reader.ReadToEnd();
    //            _items = string.IsNullOrWhiteSpace(json)
    //                ? new List<T>()
    //                : JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
    //        }
    //    }
    //    else
    //    {
    //        _items = new List<T>();
    //    }

    //    lastId = _items.Count > 0
    //        ? _items.Max(x => x.Id)
    //        : 0;

    //    item.Id = ++lastId;

    //    _items.Add(item);

    //    using (var writer = new StreamWriter(jsonFileName))
    //    {
    //        var newJson = JsonSerializer.Serialize(_items);
    //        writer.Write(newJson);
    //    }
    //    _itemAddedCallback?.Invoke(item);
    //    ItemAdded?.Invoke(this, item);
    //}

    public void Add(T item) // to CSV
    {
        using (var writer = new StreamWriter(fullCsvFileName, true, System.Text.Encoding.UTF8))
        using (var csv = new CsvWriter(writer, new CultureInfo("pl-PL")))
        {
            csv.Context.RegisterClassMap<CsvHelperConfiguration>();
            FileInfo file = new FileInfo(fullCsvFileName);
            if (file.Length == 0)
            {
                csv.WriteHeader<T>();
                csv.NextRecord();
            }

            csv.WriteRecord(item);
            csv.NextRecord();
        }
        _itemAddedCallback?.Invoke(item);
        ItemAdded?.Invoke(this, item);
    }

    public void Remove(T item)
    {
        if (File.Exists(jsonFileName))
        {
            using (var streamReader = new StreamReader(jsonFileName))
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

        using (var streamWriter = new StreamWriter(jsonFileName))
        {
            var newJson = JsonSerializer.Serialize(_items);
            streamWriter.Write(newJson);
        }
    }

    public void Save()
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"Książki zostały zapisane w pliku {fullCsvFileName}" + Environment.NewLine);
        Console.ResetColor();
        // Save is not required with file
    }

    public void UpdateProperty(T item, Action<T> updateAction)
    {
        throw new NotImplementedException();
    }
}