using MyLibrary.Entities;
using System.ComponentModel.DataAnnotations;

namespace MyLibrary.DataProviders;

public interface IBooksProvider
{
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

    //where

    List<Book> WhereStartsWith(string prefix);

    List<Book> WhereStartsWithAndCostIsGreaterThan(string prefix, decimal cost);

    List<Book> WhereOwnerIs(string color);

    List<Book> WhereVolumeIsGreaterThan(int minPagesNumber);

    List<Book> WhereIsBorrowed();

    List<string> WhereTitleOfBooksWhoOwnerIs(string owner);

    //first, last, single
    Book FirstByOwner(string owner);

    Book? FirstOrDefaultByOwner(string owner);

    Book FirstOrDefaultByOwnerWithDefault(string owner);

    Book? LastByOwner(string owner);

    Book SingleById(int id);

    Book? SingleOrDefaultById(int id);

    // take

    List<Book> TakeBooks(int howMany);

    List<Book> TakeBooks(Range range);

    List<Book> TakeBooksWhileIdIs();

    // Skip

    List<Book> SkipBooks(int howMany);

    List<Book> SkipBooksWhileTitleStartsWith(int howMany, string prefix);

    // Distinct, DistinctBy

    List<string> DistinctAllOwners();

    List<Book> DistinctByOwners();

    // Chunk
    List<Book[]> ChunkBooks(int size);
}
