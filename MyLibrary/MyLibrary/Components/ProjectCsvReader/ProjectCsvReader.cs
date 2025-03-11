using MyLibrary.Components.ProjectCsvReader.Extensions;
using MyLibrary.Components.ProjectCsvReader.VariousBooksCollections;
using MyLibrary.Data.Entities;
using System.Globalization;
using CsvHelper;


namespace MyLibrary.Components.ProjectCsvReader;

public class ProjectCsvReader : ICsvReader
{
    public List<DataCleanBook> ProcessDataCleanBook(string filePath)
    {

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Nie znaleziono pliku 'Books_Data_Clean.csv'!");
            return new List<DataCleanBook>();
        }

        //odczyt pliku przy pomocy CsvHelper

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csv.GetRecords<DataCleanBook>().ToList();
        }
    }
    public List<Book> ProcessMyLibraryBook(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Nie znaleziono pliku MyHome!");
            return new List<Book>();
        }
        var books =
        File.ReadAllLines(filePath)
        .Where(x => x.Length > 1)
        .Select(x =>
        {
            var columns = x.Split(';', StringSplitOptions.None);
            for (int i = 0; i < 16; i++)
            {
                if (string.IsNullOrEmpty(columns[i]))
                {
                    columns[i] = null;
                }
            }
            int? _yearOfPublication;
            if (int.TryParse(columns[7], out int result1) && result1 > 999 && result1 < 10000)
            {
                _yearOfPublication = result1;
            }
            else
            {
                _yearOfPublication = null;
            }

            int? _pageNumber;
            if (int.TryParse(columns[8], out int result2) && result2 > 0)
            {
                _pageNumber = result2;
            }
            else
            {
                _pageNumber = null;
            }

            bool _isForSale;
            if (columns[12] == "True")
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
                if (decimal.TryParse(columns[13], out decimal result3) && result3 > 0)
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

            bool _isLent;
            if (columns[14] == "True")
            {
                _isLent = true;
            }
            else
            {
                _isLent = false;
            }

            bool _isBorrowed;
            if (columns[15] == "True")
            {
                _isBorrowed = true;
            }
            else
            {
                _isBorrowed = false;
            }

            DateTime? _dateOfBorrowedOrLent;
            if (DateTime.TryParse(columns[16], out DateTime result4))
            {
                _dateOfBorrowedOrLent = result4;
            }
            else
            {
                _dateOfBorrowedOrLent = null;
            }

            return new Book()
            {
                Id = int.Parse(columns[0], CultureInfo.InvariantCulture),
                AuthorName = columns[1],
                AuthorSurname = columns[2],
                CollectiveAuthor = columns[3],
                Title = columns[4],
                PublishingHouse = columns[5],
                PlaceOfPublication = columns[6],
                YearOfPublication = _yearOfPublication,
                PageNumber = _pageNumber,
                ISBN = columns[9],
                PlaceInLibrary = columns[10],
                Owner = columns[11],
                IsForSale = _isForSale,
                Price = _price,
                IsLent = _isLent,
                IsBorrowed = _isBorrowed,
                DateOfBorrowedOrLent = _dateOfBorrowedOrLent

            };
        });
        return books.ToList();
    }

    public List<RealBook> ProcessRealBooks(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Nie znaleziono pliku MyHome!");
            return new List<RealBook>();
        }

        var books =
            File.ReadAllLines(filePath)
            .Skip(1)
            .Where(x => x.Length > 1)
            .ToBook();

        return books.ToList();
    }

    public List<TopBook> ProcessTopBooks(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Nie znaleziono pliku książkaTop!");
            return new List<TopBook>();
        }

        var books =
            File.ReadAllLines(filePath)
            .Where(x => x.Length > 1)
            .Select(x =>
            {
                var columns = x.Split(';', StringSplitOptions.None);

                for (int i = 0; i < 4; i++)
                {
                    if (string.IsNullOrEmpty(columns[i]))
                    {
                        columns[i] = "0";
                    }
                }

                return new TopBook()
                {
                    Index = int.Parse(columns[0], CultureInfo.InvariantCulture),
                    Title = columns[1],
                    AuthorName = columns[2],
                    AuthorSurname = columns[3]
                };
            });
        return books.ToList();
    }
}