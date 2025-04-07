using MyLibrary.Data.Entities;
using MyLibrary.Data.Repositories;

namespace MyLibrary.Components.DataProviders;

public interface IBooksDataProvider
{
    //select
    List<string> GetUniqueBookOwners();

    decimal GetMinimumPriceOffAllBooks();

    List<Book> GetOnlyAuthorAndTitle();

    // order by
    List<Book> OrderByTitle();

    List<Book> OrderByTitleDescending();

    List<Book> OrderByAuthorSurnameAndTitle();

    List<Book> OrderByAuthorSurnameAndTitleDesc();

    //where

    List<Book> WhereStartsWith(string prefix);

    List<Book> WhereStartsWithAndCostIsGreaterThan(string prefix, decimal cost);

    List<Book> WhereOwnerIs(string color);

    List<Book> WhereVolumeIsGreaterThan(int minPagesNumber);

    List<Book> WhereIsBorrowed();

    List<Book> WhereIsLent();

    List<Book> WhereIsForSale();

    List<Book> GetOnlyTitleAndPlaceInLibrary();

    List<string> WhereTitlesOfBooksWhoOwnerIs(string owner);

    //first, last, single
    Book FirstByOwner(string owner);

    Book? FirstOrDefaultByOwner(string owner);

    Book? FirstOrDefaultByOwnerWithDefault(string owner);

    Book? LastOrDefaultByOwnerWithDefault(string owner);

    Book SingleById(int id);

    Book? SingleOrDefaultById(int id);

    // take

    List<Book> TakeBooks(int howMany);

    List<Book> TakeBooks(Range range);

    List<Book> TakeBooksWhileIdIs(int id);

    // Skip

    List<Book> SkipBooks(int howMany);

    List<Book> SkipBooksWhileTitleStartsWith(int howMany, string prefix);

    // Distinct, DistinctBy

    List<string> DistinctAllOwners();

    List<Book> DistinctByOwners();

    // Chunk
    List<Book[]> ChunkBooks(int size);

    Book FindBookByTitle(IRepository<Book> dbRepository);

    Book FindBookById(IRepository<Book> dbRepository);
}
