using MyLibrary.Entities;

namespace MyLibrary.DataProviders;

public interface IBooksProvider
{
    //inne
    List<Book> FilterBooks(int minPagesNumber);

    List<Book> GetBorrowedBooks();

    //select
    List<string> GetUniqueBookOwners();

    decimal GetMinimumPriceOffAllBooks();

    List<Book> GetSpecificColumns();

    string AnonimousClass();

    // order by
    List<Book> OrderByTitle();

    List<Book> OrderByTitleDescending();

    List<Book> OrderByAuthorSurnameAndTitle();

    List<Book> OrderByAuthorSurnameAndTitleDesc();

}
