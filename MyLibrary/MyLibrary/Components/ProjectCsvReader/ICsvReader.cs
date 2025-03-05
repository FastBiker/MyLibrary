using MyLibrary.Components.ProjectCsvReader.VariousBooksCollections;
using MyLibrary.Data.Entities;

namespace MyLibrary.Components.ProjectCsvReader;

public interface ICsvReader
{
    List<RealBook> ProcessRealBooks(string filePath);

    List<TopBook> ProcessTopBooks(string filePath);

    List<Book> ProcessMyLibraryBook(string filePath);

    List<DataCleanBook> ProcessDataCleanBook(string filePath);
}
