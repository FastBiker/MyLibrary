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

    //inne
    public List<Book> FilterBooks(int minPage)
    {
        throw new NotImplementedException();
    }
    
    public List<Book> GetBorrowedBooks()
    {
        throw new NotImplementedException();
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
}
