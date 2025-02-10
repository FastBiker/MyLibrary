using MyLibrary.Components.CsvReader.Extensions;
using MyLibrary.Components.CsvReader.VariousBooksCollections;

namespace MyLibrary.Components.CsvReader;

public class CsvReader : ICsvReader
{
    public List<RealBook> ProcessBooks(string filePath)
    {
        if (!File.Exists(filePath))
        {
            return new List<RealBook>();
        }

        var books =
            File.ReadAllLines(filePath)
            .Skip(1)
            .Where(x => x.Length > 1)
            .ToBook();

        return books.ToList();
    }
}
