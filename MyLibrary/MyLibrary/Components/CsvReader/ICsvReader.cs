using MyLibrary.Components.CsvReader.VariousBooksCollections;

namespace MyLibrary.Components.CsvReader;

public interface ICsvReader
{
    List<RealBook> ProcessBooks(string filePath);
}
