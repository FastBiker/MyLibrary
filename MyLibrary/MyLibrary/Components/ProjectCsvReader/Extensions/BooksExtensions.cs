using MyLibrary.Components.ProjectCsvReader.VariousBooksCollections;

namespace MyLibrary.Components.ProjectCsvReader.Extensions;

public static class BooksExtensions
{
    public static IEnumerable<RealBook> ToBook(this IEnumerable<string> source)
    {
        foreach (var line in source)
        {
            var columns = line.Split(';',StringSplitOptions.None);

            for (int i = 0; i < 9; i++)
            {
                if (string.IsNullOrEmpty(columns[i]))
                {
                    columns[i] = null;
                }
            }

            bool _isForSale;
            if (columns[6] == "tak")
            {
                _isForSale = true;
            }
            else
            {
                _isForSale = false;
            }

            decimal? _price;
            if (_isForSale == true)
            {
                if (decimal.TryParse(columns[7], out decimal result3) && result3 > 0)
                {
                    _price = result3;
                }
                else 
                {
                    _price = null;
                }
            }
            else
            {
                _price = null;
            }

            yield return new RealBook
            {
                Lp = columns[0],
                AuthorName = columns[1],
                AuthorSurname = columns[2],
                CollectiveAuthor = columns[3],
                Title = columns[4],
                PlaceInLibrary = columns[5],
                IsForSale = _isForSale,
                Price = _price, 
                Comments = columns[8]
            };
        }

        //Bez użycia "yield":
        //===================
        //var books = new List<RealBook>();

        //foreach (var line in source)
        //{
        //    var columns = line.Split(';');

        //    var book = new RealBook
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
        //    books.Add(new RealBook());
        //}

        //return books;
    }
}
