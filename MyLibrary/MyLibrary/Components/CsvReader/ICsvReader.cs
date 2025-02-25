using MyLibrary.Components.CsvReader.VariousBooksCollections;
using System.Text;

namespace MyLibrary.Components.CsvReader;

public interface ICsvReader
{
    List<RealBook> ProcessRealBooks(string filePath);

    List<TopBook> ProcessTopBooks(string filePath);
}
