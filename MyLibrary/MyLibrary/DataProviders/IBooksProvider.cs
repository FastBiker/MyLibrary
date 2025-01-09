using MyLibrary.Entities;

namespace MyLibrary.DataProviders;

public interface IBooksProvider
{
    List<Book> FilterBooks(int minPagesNumber);

    List<string> GetUniqueBookOwners();

    decimal GetMinimumPriceOffAllBooks();

    List<Book> GetSpecificColumns();

    string AnonimousClass();
}
