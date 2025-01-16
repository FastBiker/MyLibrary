using MyLibrary.DataProviders.Extensions;
using MyLibrary.Entities;
using MyLibrary.Repositories;
using System.Text;

namespace MyLibrary.DataProviders;

public class BooksProvider : IBooksProvider
{
    private readonly IRepository<Book> _bookRepository;
    public BooksProvider(IRepository<Book> bookRepository)
    {
        _bookRepository = bookRepository;
    }

    //select
    public decimal GetMinimumPriceOffAllBooks()
    {
        var books = _bookRepository.GetAll();
        return (decimal)books.Select(x => x.Price).Min();
    }

    public List<Book> GetSpecificColumns()
    {
        var books = _bookRepository.GetAll();
        var list = books.Select(book => new Book
        {
            Id = book.Id,
            AuthorName = book.AuthorName,
            AuthorSurname = book.AuthorSurname,
            Title = book.Title,
            IsBorrowed = book.IsBorrowed,

        }).ToList();

        return list;
    }

    public List<string> GetUniqueBookOwners()
    {
        var books = _bookRepository.GetAll();
        var owners = books.Select(x => x.Owner).Distinct().ToList();
        return owners;
    }

    public string AnonimousClass()
    {
        var books = _bookRepository.GetAll();
        var list = books.Select(book => new
        {
            BookIdentifier = book.Id,
            Author1 = book.AuthorName,
            Author2 = book.AuthorSurname,
            BookTitle = book.Title,

        }).ToList();

        StringBuilder sb = new StringBuilder(2048);
        foreach (var book in list)
        {
            sb.AppendLine($"\nProduct ID: {book.BookIdentifier}");
            sb.AppendLine($"Book Author: {book.Author1} {book.Author2}");
            sb.AppendLine($"Book Title: {book.BookTitle}");
        }

        return sb.ToString();
    }

    //order by
    public List<Book> OrderByTitle()
    {
        var books = _bookRepository.GetAll();
        return books.OrderBy(x => x.Title).ToList();
    }

    public List<Book> OrderByTitleDescending()
    {
        var books = _bookRepository.GetAll();
        return books.OrderByDescending(x => x.Title).ToList();
    }

    public List<Book> OrderByAuthorSurnameAndTitle()
    {
        var books = _bookRepository.GetAll();
        return books
            .OrderBy(x => x.AuthorSurname)
            .ThenBy(x => x.Title)
            .ToList();
    }

    public List<Book> OrderByAuthorSurnameAndTitleDesc()
    {
        var books = _bookRepository.GetAll();
        return books
            .OrderByDescending(x => x.AuthorSurname)
            .ThenByDescending(x => x.Title)
            .ToList();
    }

    //where
    public List<Book> WhereStartsWith(string prefix)
    {
        var books = _bookRepository.GetAll();
        return books.Where(x => x.Title.StartsWith(prefix)).ToList();
    }

    public List<Book> WhereStartsWithAndCostIsGreaterThan(string prefix, decimal cost)
    {
        var books = _bookRepository.GetAll();
        return books.Where(x => x.Title.StartsWith(prefix) && x.Price > cost).ToList();
    }

    public List<Book> WhereOwnerIs(string owner)
    {
        var books = _bookRepository.GetAll();
        return books.ByOwner(owner).ToList();
    }

    public List<Book> WhereVolumeIsGreaterThan(int minPagesNumber)
    {
        var books = _bookRepository.GetAll();
        return books.Where(x => x.PageNumber > minPagesNumber).ToList();
    }

    public List<Book> WhereIsBorrowed()
    {
        var books = _bookRepository.GetAll();
        return books.Where(x => x.IsBorrowed).ToList();
    }

    public List<string> WhereTitleOfBooksWhoOwnerIs(string owner)
    {
        throw new NotImplementedException();
    }


    //first, last, single
    public Book FirstByOwner(string owner)
    {
        var books = _bookRepository.GetAll();
        return books.First(x => x.Owner == owner);
    }

    public Book? FirstOrDefaultByOwner(string owner)
    {
        var books = _bookRepository.GetAll();
        return books.FirstOrDefault(x => x.Owner == owner);
    }

    public Book FirstOrDefaultByOwnerWithDefault(string owner)
    {
        var books = _bookRepository.GetAll();
        return books.FirstOrDefault(x => x.Owner == owner, new Book { Id = -1, Title = "NOT FOUND"});
    }

    public Book? LastByOwner(string owner)
    {
        var books = _bookRepository.GetAll();
        return books.LastOrDefault(x => x.Owner == owner, new Book { Id = -1, Owner = "NOT FOUND"});
    }

    public Book SingleById(int id)
    {
        var books = _bookRepository.GetAll();
        return books.Single(x => x.Id == id);
    }

    public Book? SingleOrDefaultById(int id)
    {
        var books = _bookRepository.GetAll();
        return books.SingleOrDefault(x => x.Id == id, new Book { Id = -1, Title = "NOT FOUND"});
    }

    // Take, TakeWhile
    public List<Book> TakeBooks(int howMany)
    {
        var books = _bookRepository.GetAll();
        return books
            .OrderBy(x => x.Title)
            .Take(howMany)
            .ToList();
    }

    public List<Book> TakeBooks(Range range)
    {
        var books = _bookRepository.GetAll();
        return books
            .OrderBy(x => x.Title)
            .Take(range)
            .ToList();
    }

    public List<Book> TakeBooksWhileIdIs()
    {
        var books = _bookRepository.GetAll();
        return books
            .OrderBy(x => x.Id)
            .TakeWhile(x => x.Id < 30)
            .ToList();
    }

    // Skip, SkipWhile
    public List<Book> SkipBooks(int howMany)
    {
        var books = _bookRepository.GetAll();
        return books
            .OrderBy(x => x.Title)
            .Skip(howMany)
            .ToList();
    }

    public List<Book> SkipBooksWhileTitleStartsWith(int howMany, string prefix)
    {
        var books = _bookRepository.GetAll();
        return books
            .OrderBy(x => x.Title)
            .Skip(howMany)
            .SkipWhile(x => x.Title.StartsWith(prefix))
            .ToList();
    }

    // Distinct, DistinctBy
    public List<string> DistinctAllOwners()
    {
        var books = _bookRepository.GetAll();
        return books
            .Select(x => x.Owner)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    public List<Book> DistinctByOwners()
    {
        var books = _bookRepository.GetAll();
        return books
            .DistinctBy(x => x.Owner)
            .OrderBy(x => x.Owner)
            .ToList();
    }
}
