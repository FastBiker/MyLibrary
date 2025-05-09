using MyLibrary.Components.CsvHandler.VariousBooksCollections;
using MyLibrary.Data.Entities;

namespace MyLibrary.Components.CsvHandler;

public interface ICsvReader
{
    List<Book> ProcessMyLibraryBookWithCsvHelper(string filePath);

    List<Book> ProcessRealBooks(string filePath);

    List<Book> ProcessTopBooks(string filePath);

    List<Book> ProcessMyLibraryBook(string filePath);

    List<DataCleanBook> ProcessDataCleanBook(string filePath);
}
