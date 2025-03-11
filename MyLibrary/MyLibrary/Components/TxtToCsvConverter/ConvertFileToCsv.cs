using MyLibrary.Data.Entities;
using MyLibrary.Data.Repositories;

namespace MyLibrary.Components.TxtToCsvConverter;

public class ConvertFileToCsv : IConvertFileToCsv
{
    private IRepository<Book> _fileRepository;

    public ConvertFileToCsv(IRepository<Book> fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public void Convert()
    {
        ConvertJsonToCsv(_fileRepository);
        

        //void ConvertTxtWithReadAllLines()
        //{
        //    string inputFileTxtPath = "C:\\Projekty\\MyLibrary\\MyLibrary\\MyLibrary\\Resources\\Files\\książkiTop100.txt";
        //    string outputFileCsv = "C:\\Projekty\\MyLibrary\\MyLibrary\\MyLibrary\\Resources\\Files\\książkiTop100.csv";

        //    var lines = File.ReadAllLines(inputFileTxtPath);

        //    using (StreamWriter writer = new StreamWriter(outputFileCsv))
        //    {
        //        foreach (var line in lines)
        //        {
        //            var csvLine = line.Trim().Replace(" – ", ";").Replace("- ", ";");
        //            writer.WriteLine(csvLine);
        //        }
        //    }
        //    Console.WriteLine("Konwersja zakończona!");
        //}

        //void ReplaceString()
        //{
        //    string inputFileCsvPath = "C:\\Projekty\\MyLibrary\\MyLibrary\\MyLibrary\\Resources\\Files\\My_Home_Library.csv";
        //    string outputFileCsv = "C:\\Projekty\\MyLibrary\\MyLibrary\\MyLibrary\\Resources\\Files\\My_Home_Library.csv";

        //    var lines = File.ReadAllLines(inputFileCsvPath);

        //    using (StreamWriter writer = new StreamWriter(outputFileCsv))
        //    {
        //        foreach (var line in lines)
        //        {
        //            var csvLine = line.Replace("Tak", "true");
        //            writer.WriteLine(csvLine);
        //        }
        //    }
        //    Console.WriteLine("Zmiana 'stringa' zakończona!");
        //}
    }
    void ConvertJsonToCsv(IRepository<Book> _fileRepository)
    {
        //string jsonFilePath = "mylibrary.json";
        string csvFilePath = "C:\\Projekty\\MyLibrary\\MyLibrary\\MyLibrary\\bin\\Debug\\net8.0\\Resources\\Files\\mylibrary.csv";

        var books = _fileRepository.GetAll();
        var list = books.Select(book => new Book
        {
            Id = book.Id,
            AuthorName = book.AuthorName,
            AuthorSurname = book.AuthorSurname,
            CollectiveAuthor = book.CollectiveAuthor,
            Title = book.Title,
            PublishingHouse = book.PublishingHouse,
            PlaceOfPublication = book.PlaceOfPublication,
            YearOfPublication = book.YearOfPublication,
            PageNumber = book.PageNumber,
            ISBN = book.ISBN,
            PlaceInLibrary = book.PlaceInLibrary,
            Owner = book.Owner,
            IsForSale = book.IsForSale,
            Price = book.Price,
            IsLent = book.IsLent,
            IsBorrowed = book.IsBorrowed,
            DateOfBorrowedOrLent = book.DateOfBorrowedOrLent
        }).ToList();

        using (StreamWriter writer = new StreamWriter(csvFilePath))
        {
            foreach (var book in list)
            {
                writer.WriteLine(book);
            }
        }

        Console.WriteLine("Plik JSON został przekonwertowany na CSV!");
    }


}