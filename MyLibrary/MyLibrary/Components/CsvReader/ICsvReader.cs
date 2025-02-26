using MyLibrary.Components.CsvReader.VariousBooksCollections;
using MyLibrary.Data.Entities;

namespace MyLibrary.Components.CsvReader;

public interface ICsvReader
{
    List<RealBook> ProcessRealBooks(string filePath);

    List<TopBook> ProcessTopBooks(string filePath);

    List<Book> ProcessMyLibraryBook(string filePath);
}
