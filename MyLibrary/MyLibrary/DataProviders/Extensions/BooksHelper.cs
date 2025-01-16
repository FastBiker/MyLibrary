using MyLibrary.Entities;

namespace MyLibrary.DataProviders.Extensions;

public static class BooksHelper
{
    public static IEnumerable<Book> ByOwner(this IEnumerable<Book> query, string owner)
    {
       return query.Where(x => x.Owner == owner);
    }
}
