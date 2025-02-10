using MyLibrary.Components.CsvReader.VariousBooksCollections;
using System.Data.Common;

namespace MyLibrary.Components.CsvReader.Extensions;

public static class BooksExtensions
{
    public static IEnumerable<RealBook> ToBook(this IEnumerable<string> source)
    {
        var books = new List<RealBook>();

        foreach (var line in source)
        {
            var columns = line.Split(';');

            var book = new RealBook
            {
                Lp = int.Parse(columns[0]),
                AuthorName = columns[1],
                AuthorSurname = columns[2],
                CollectiveAuthor = columns[3],
                Title = columns[4],
                PlaceInLibrary = columns[5],
                IsForSale = bool.Parse(columns[6]),
                Price = decimal.Parse(columns[7]),
                Comments = columns[8]
            };
            books.Add(new RealBook());
        }

        return books;

        //foreach (var line in source)
        //{
        //    var columns = line.Split(';');

        //    yield return new RealBook
        //    {
        //        Lp = int.Parse(columns[0]),
        //        AuthorName = columns[1],
        //        AuthorSurname = columns[2],
        //        CollectiveAuthor = columns[3],
        //        Title = columns[4],
        //        PlaceInLibrary = columns[5],
        //        IsForSale = bool.Parse(columns[6]),
        //        Price = decimal.Parse(columns[7]),
        //        Comments = columns[8]
        //    };
        //}
    }
}
