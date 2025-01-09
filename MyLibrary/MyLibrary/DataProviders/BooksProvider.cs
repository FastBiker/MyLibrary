using MyLibrary.Entities;
using MyLibrary.Repositories;

namespace MyLibrary.DataProviders;

public class BooksProvider : IBooksProvider
{
    private readonly IRepository<Book> _bookRepository;
    public BooksProvider(IRepository<Book> bookRepository)
    {
        _bookRepository = bookRepository;
    }
    public string AnonimousClass()
    {
        throw new NotImplementedException();
    }

    public List<Book> FilterBooks(int minPage)
    {
        throw new NotImplementedException();
    }

    public decimal GetMinimumPriceOffAllBooks()
    {
        var books = _bookRepository.GetAll();
        return (decimal)books.Select(x => x.Price).Min();
    }

    public List<Book> GetSpecificColumns()
    {
        throw new NotImplementedException();
    }

    public List<string> GetUniqueBookOwners()
    {
        var books = _bookRepository.GetAll();
        var owners = books.Select(x => x.Owner).Distinct().ToList();
        return owners;
    }
}
