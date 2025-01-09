//using MyLibrary.Entities;
//using MyLibrary.Repositories;

//namespace MyLibrary.DataProviders;

//public class BooksProviderBasic : IBooksProvider
//{
//    private readonly IRepository<Book> _bookRepository;
//    public BooksProviderBasic(IRepository<Book> bookRepository)
//    {
//        _bookRepository = bookRepository;
//    }

//    public string AnonimousClass()
//    {
//        throw new NotImplementedException();
//    }

//    public List<Book> FilterBooks(int minPagesNumber)
//    {
//        var books = _bookRepository.GetAll();
//        var list = new List<Book>();

//        foreach (var book in books)
//        {
//            if (book.PageNumber > minPagesNumber)
//            {
//                list.Add(book);
//            }
//        }

//        return list;
//    }

//    public decimal GetMinimumPriceOffAllBooks()
//    {
//        var books = _bookRepository.GetAll();
//        decimal minPrice = decimal.MaxValue;
//        foreach (var book in books)
//        {
//            if (book.Price < minPrice)
//            {
//                minPrice = (decimal)book.Price;
//            }
//        }


//        return minPrice;
//    }

//    public List<Book> GetSpecificColumns()
//    {
//        throw new NotImplementedException();
//    }

//    public List<string> GetUniqueBookOwners()
//    {
//        var books = _bookRepository.GetAll();
//        var list = new List<string>();
//        foreach (var book in books)
//        {
//            if (!list.Contains(book.Owner))
//            {
//                list.Add(book.Owner);
//            }
//        }

//        return list;
//    }
//}
