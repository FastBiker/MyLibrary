using MyLibrary.Components.DataProviders.Extensions;
using MyLibrary.Data.Entities;
using MyLibrary.Data.Repositories;
using MyLibrary.UserCommunication;
using System.Text;

namespace MyLibrary.Components.DataProviders;

public class BooksDataProvider : IBooksDataProvider
{
    private readonly IRepository<Book> _bookRepository;
    private readonly IUserCommunication _userCommunication;
    public BooksDataProvider(IRepository<Book> bookRepository, IUserCommunication userCommunication)
    {
        _bookRepository = bookRepository;
        _userCommunication = userCommunication;
    }

    //select
    public decimal GetMinimumPriceOffAllBooks()
    {
        var books = _bookRepository.GetAll();
        return (decimal)books.Select(x => x.Price).Min();
    }

    public List<Book> GetOnlyAuthorAndTitle()
    {
        var books = _bookRepository.GetAll();
        var list = books.Select(book => new Book
        {
            Id = book.Id,
            AuthorName = book.AuthorName,
            AuthorSurname = book.AuthorSurname,
            CollectiveAuthor = book.CollectiveAuthor,
            Title = book.Title,

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
        var list = books.Where(x => x.Title.StartsWith(prefix)).ToList();
        if (list.Count == 0)
        {
            throw new Exception($"Brak książek, które zaczynają się od '{prefix}'");
        }
        return list;
    }

    public List<Book> WhereStartsWithAndCostIsGreaterThan(string prefix, decimal cost)
    {
        var books = _bookRepository.GetAll();
        var list = books.Where(x => x.Title.StartsWith(prefix) && x.Price > cost).ToList();
        if (list.Count == 0)
        {
            throw new Exception($"Brak książek, które zaczynają się od '{prefix}' i kosztują więcej niż {cost:c}");
        }
        return list;
    }

    public List<Book> WhereOwnerIs(string owner)
    {
        var books = _bookRepository.GetAll();
        var list = books.ByOwner(owner).ToList();
        if (list.Count == 0)
        {
            throw new Exception($"NOT FOUND");
        }
        return list;
    }

    public List<Book> WhereVolumeIsGreaterThan(int minPagesNumber)
    {
        var books = _bookRepository.GetAll();
        var list = books.Where(x => x.PageNumber > minPagesNumber).ToList();
        if (list.Count == 0)
        {
            throw new Exception($"NOT FOUND");
        }
        return list;
    }

    public List<Book> WhereIsBorrowed()
    {
        var books = _bookRepository.GetAll();
        var list = books.Where(x => x.IsBorrowed).ToList();
        if (list.Count == 0)
        {
            throw new Exception($"NOT FOUND");
        }
        return list;
    }

    public List<Book> WhereIsLent()
    {
        var books = _bookRepository.GetAll();
        var list = books.Where(x => x.IsLent).ToList();
        if (list.Count == 0)
        {
            throw new Exception($"NOT FOUND");
        }
        return list;
    }

    public List<Book> WhereIsForSale()
    {
        var books = _bookRepository.GetAll();
        var list = books.Where(x => x.IsForSale.HasValue && x.IsForSale.Value).ToList();
        if (list.Count == 0)
        {
            throw new Exception($"NOT FOUND");
        }
        return list;
    }

    public List<Book> GetOnlyTitleAndPlaceInLibrary()
    {
        var books = _bookRepository.GetAll();
        var list = books.Select(book => new Book
        {
            Id = book.Id,
            Title = book.Title,
            PlaceInLibrary = book.PlaceInLibrary,
        })
            .OrderBy(x => x.Title)
            .ToList();
        return list;
    }

    public List<string> WhereTitlesOfBooksWhoOwnerIs(string owner)
    {
        var books = _bookRepository.GetAll();
        var list = books.ByOwner(owner).ToList();
        if (list.Count == 0)
        {
            throw new Exception($"NOT FOUND");
        }
        var titlesOfOwner = list.Select(x => x.Title).ToList();
        return titlesOfOwner;
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

    public Book? FirstOrDefaultByOwnerWithDefault(string owner)
    {
        var books = _bookRepository.GetAll();
        return books.FirstOrDefault(x => x.Owner == owner, new Book { Id = -1, Title = "NOT FOUND" });
    }

    public Book? LastOrDefaultByOwnerWithDefault(string owner)
    {
        var books = _bookRepository.GetAll();
        return books.LastOrDefault(x => x.Owner == owner, new Book { Id = -1, Owner = "NOT FOUND" });
    }

    public Book SingleById(int id)
    {
        var books = _bookRepository.GetAll();
        return books.Single(x => x.Id == id);
    }

    public Book? SingleOrDefaultById(int id)
    {
        var books = _bookRepository.GetAll();
        return books.SingleOrDefault(x => x.Id == id, new Book { Id = -1, Title = "NOT FOUND" });
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

    public List<Book> TakeBooksWhileIdIs(int id)
    {
        var books = _bookRepository.GetAll();
        return books
            .OrderBy(x => x.Id)
            .TakeWhile(x => x.Id < id)
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

    public List<Book[]> ChunkBooks(int size)
    {
        ;
        var books = _bookRepository.GetAll();
        var chunkBooks = books.Chunk(size).ToList();

        return chunkBooks;


    }

    public Book FindBookByTitle(IRepository<Book> dbRepository)
    {
        string? input1 = _userCommunication.WriteBookPropertyValue("tytuł");
        var foundBooks = dbRepository.GetAll().Where(x => x.Title == input1).ToList();
        Book foundBook;
        if (foundBooks.Count > 1)
        {
            List<int> Ids = foundBooks.Select(x => x.Id).ToList();

            throw new Exception($"There are {foundBooks.Count} books (Id: {string.Join(", ", Ids)}) with the title '{input1}';" +
                $"\nFind a book that intrests you by its Id!");
        }
        else if (foundBooks.Count == 1)
        {
            foundBook = foundBooks.Single();
        }
        else
        {
            throw new Exception($"Book '{input1}' hasn't found in your library");
        }

        return foundBook;
    }

    public Book FindBookById(IRepository<Book> dbRepository)
    {
        var input2 = _userCommunication.WriteBookPropertyValue("Id");
        int id;
        Book foundBook;
        if (int.TryParse(input2, out int result6) && result6 > 0)
        {
            id = result6;
        }
        else
        {
            throw new Exception("\nPodane dane w 'Id' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od 0");
        }

        var book = dbRepository.GetById(id);

        if (book != null)
        {
            foundBook = book;
        }
        else
        {
            throw new Exception($"Book '{input2}' hasn't found in your library");
        }
        return foundBook;
    }
}
