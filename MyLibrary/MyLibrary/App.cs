using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using MyLibrary.Components.ProjectCsvReader;
using MyLibrary.Components.ProjectCsvReader.VariousBooksCollections;
using MyLibrary.Components.DataProviders;
using MyLibrary.Data;
using MyLibrary.Data.Entities;
using MyLibrary.Data.Entities.Extensions;
using MyLibrary.Data.Repositories;
using MyLibrary.Data.Repositories.Extensions;
using MyLibrary.UserCommunication;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Xml.Linq;

namespace MyLibrary;

public class App : IApp
{
    private readonly IRepository<Book> _fileRepository;
    private readonly IBooksDataProvider _booksDataProvider;
    private readonly IUserCommunication _userCommunication;
    private readonly ICsvReader _csvReader;

    public App(IRepository<Book> fileRepository, IBooksDataProvider booksDataProvider, IUserCommunication userCommunication, ICsvReader csvReader)
    {
        _fileRepository = fileRepository;
        _booksDataProvider = booksDataProvider;
        _userCommunication = userCommunication;
        _csvReader = csvReader;
    }
    public void Run()
    {
        //pliki zewnętrzne CSV z książkami i operacje na nich
        var realBooks = _csvReader.ProcessRealBooks("Resources\\Files\\My_Home_Library.csv");
        var top259Books = _csvReader.ProcessTopBooks("Resources\\Files\\BooksTop259.csv");
        var top100Books = _csvReader.ProcessTopBooks("Resources\\Files\\BooksTop100.csv");
        var dataCleanBook = _csvReader.ProcessDataCleanBook("Resources\\Files\\Books_Data_Clean.csv");

        foreach (var book in dataCleanBook) 
        {
            Console.WriteLine(book);
        }

        //CreateXml();
        //QueryXml();




        //zapisywanie nowych książek do pliku JSON, usuwanie, odczytywanie, filtrowanie
        _userCommunication.Welcome();

        string auditFileName = "audit_library.txt";

        var bookRepository = new SqlRepository<Book>(new MyLibraryDbContext(), BookAdded);
        bookRepository.ItemAdded += BookOnItemAdded;

        var fileRepository = new FileRepository<Book>(BookAdded, BookRemoved);
        fileRepository.ItemAdded += BookOnItemAdded;
        fileRepository.ItemRemoved += BookOnItemRemoved;

        void BookRemoved(Book item)
        {
            _userCommunication.WriteAuditInfoToConsoleUsingCallback(item, "BookDeleted");
        }

        void BookAdded(Book item)
        {
            _userCommunication.WriteAuditInfoToConsoleUsingCallback(item, "BookAdded");
        }

        void BookOnItemAdded(object? sender, Book e)
        {
            WriteAuditInfoToFileAndConsole(sender, e, auditFileName, "BookAdded");
        }

        void BookOnItemRemoved(object? sender, Book e)
        {
            WriteAuditInfoToFileAndConsole(sender, e, auditFileName, "BookDeleted");
        }


        while (true)
        {
            string? input = _userCommunication.MainMenu();
            if (input == "q")
            {
                break;
            }
            switch (input)
            {
                case "1":
                    _userCommunication.MainMethodsHeaders("Lista książek z Twojej biblioteki domowej:"
                        + Environment.NewLine + "==========================================");
                    try
                    {
                        WriteAllToConsole(bookRepository);
                        WriteAllToConsole(fileRepository);
                    }
                    catch (Exception e)
                    {
                        _userCommunication.ExceptionCatchedMainMethods(e);
                    }
                    break;

                case "2":
                    _userCommunication.MainMethodsHeaders("Dodaj nową książkę, podając kolejno informacje o niej; " +
                        "'*' oznacza konieczność wpisania danych; " +
                        "w przypadku pozostałych danych, jeśli nie chcesz ich wprowadzać, przejdź dalej, wciskając 'Enter'");
                    try
                    {
                        AddBooks(bookRepository, fileRepository);
                    }
                    catch (Exception e)
                    {
                        _userCommunication.ExceptionCatchedMainMethods(e);
                    }
                    break;

                case "3":
                    _userCommunication.MainMethodsHeaders("Usuń książkę, wpisując jej tytuł:");
                    try
                    {
                        RemoveBook(fileRepository);
                    }
                    catch (Exception e)
                    {
                        _userCommunication.ExceptionCatchedMainMethods(e);
                    }

                    break;

                case "4":
                    _userCommunication.MainMethodsHeaders("Filtry:" + Environment.NewLine + "-------");
                    try
                    {
                        FilterBooks(_booksDataProvider);
                    }
                    catch (Exception e)
                    {
                        _userCommunication.ExceptionCatchedMainMethods(e);
                    }
                    break;
                default:
                    _userCommunication.ExceptionWrongMenuInput();
                    break;
            }

        }
        void WriteAllToConsole(IReadRepository<IEntity> repository)
        {
            var items = repository.GetAll();
            _userCommunication.WriteItemToConsole(items);
        }

        void AddBooks(IRepository<Book> bookRepository, IRepository<Book> fileRepository)
        {
            while (true)
            {
                _userCommunication.SelectAuthor(out string inf1, out string inf2, out string? input);
                string _authorSurname;
                string _collectiveAuthor;
                string _authorName;
                switch (input)
                {
                    case "1":
                        input = _userCommunication.WriteBookProperties("imię autora(*)");
                        _authorName = InputIsNullOrEmpty(input, inf1);

                        input = _userCommunication.WriteBookProperties("nazwisko autora(*)");
                        _authorSurname = InputIsNullOrEmpty(input, inf1);

                        _collectiveAuthor = null;
                        break;

                    case "2":
                        input = _userCommunication.WriteBookProperties("autora zbiorowego(*)");
                        _collectiveAuthor = InputIsNullOrEmpty(input, inf1);

                        _authorName = null;
                        _authorSurname = null;
                        break;
                    default:
                        throw new Exception("Wrong input value");
                }

                input = _userCommunication.WriteBookProperties("tytuł książki(*)");
                var _title = InputIsNullOrEmpty(input, inf1);

                input = _userCommunication.WriteBookProperties("nazwę wydawnictwa");
                var _publishingHouse = InputIsNullOrEmpty(input, inf2);

                input = _userCommunication.WriteBookProperties("miejsce wydania");
                var _placeOfPublication = InputIsNullOrEmpty(input, inf2);

                input = _userCommunication.WriteBookProperties("rok wydania (rrrr)");
                int? _yearOfPublication;
                if (int.TryParse(input, out int result) && result > 999 && result < 10000)
                {
                    _yearOfPublication = result;
                }
                else if (InputIsNullOrEmpty(input, inf2) == null)
                {
                    _yearOfPublication = null;
                }
                else
                {
                    throw new Exception("\nPodane dane w 'rok wydania' mają niewłaściwą wartość; wpisz liczbę czterocyfrową dodatnią (rrrr)");
                }

                input = _userCommunication.WriteBookProperties("liczbę stron");
                int? _pagesNumber;
                if (int.TryParse(input, out int result2) && result2 > 0)
                {
                    _pagesNumber = result2;
                }
                else if (InputIsNullOrEmpty(input, inf2) == null)
                {
                    _pagesNumber = null;
                }
                else
                {
                    throw new Exception("\nPodane dane w 'liczba stron' mają niewłaściwą wartość; wpisz liczbę całkowitą dodatnią");
                }

                input = _userCommunication.WriteBookProperties("ISBN");
                var _iSBN = InputIsNullOrEmpty(input, inf2);

                input = _userCommunication.WriteBookProperties("lokalizację książki w twojwj bibliotece");
                var _placeInLibrary = InputIsNullOrEmpty(input, inf2);

                input = _userCommunication.WriteBookProperties("właściciela książki");
                var _owner = InputIsNullOrEmpty(input, inf2);

                bool _isForSale;
                const string propertyForSale = "książka jest na sprzedaż";
                BoolValidation(out input, out _isForSale, propertyForSale);

                decimal? _price;
                if (_isForSale == true)
                {
                    input = _userCommunication.WriteBookProperties("cenę książki (jeśli jest na sprzedaż), wpisując dowolną liczbę większą od O wg wzoru: '00,00'");
                    if (decimal.TryParse(input, out decimal result3) && result3 > 0)
                    {
                        _price = result3;
                    }
                    else if (InputIsNullOrEmpty(input, inf2) == null)
                    {
                        _price = null;
                    }
                    else
                    {
                        throw new Exception("Podana liczba w 'cena książki' ma niewłaściwą wartość; wpisz dowolną liczbę większą od 0 (00,00)");
                    }
                }
                else
                {
                    _price = null;
                }


                bool _isLent;
                const string propertyIsLent = "książka jest komuś pożyczona";
                BoolValidation(out input, out _isLent, propertyIsLent);

                bool _isBorrowed;
                const string propertyIsBorrowed = "książka jest wypożyczona";
                BoolValidation(out input, out _isBorrowed, propertyIsBorrowed);

                DateTime? _dateOfBorrowedOrLent;
                if (_isBorrowed == true || _isLent == true)
                {
                    input = _userCommunication.WriteBookProperties("datę (wy)pożyczenia wg wzoru: dd.mm.rrrr");
                    if (DateTime.TryParse(input, out DateTime result4))
                    {
                        _dateOfBorrowedOrLent = result4;
                    }
                    else if (InputIsNullOrEmpty(input, inf2) == null)
                    {
                        _dateOfBorrowedOrLent = null;
                    }
                    else
                    {
                        throw new Exception("Podane dane w 'data wypożyczenia' mają niewłaściwą wartość; " +
                            "podaj datę wypożyczenia wg wzoru: rrrr,mm,dd");
                    }
                }
                else
                {
                    _dateOfBorrowedOrLent = null;
                }

                var inputBreak = _userCommunication.SaveBook();

                var books = new[]
                {
                    new Book
                    {
                        AuthorName = _authorName,
                        AuthorSurname = _authorSurname,
                        CollectiveAuthor = _collectiveAuthor,
                        Title = _title,
                        PublishingHouse = _publishingHouse,
                        PlaceOfPublication = _placeOfPublication,
                        YearOfPublication = _yearOfPublication,
                        PageNumber = _pagesNumber,
                        ISBN = _iSBN,
                        PlaceInLibrary = _placeInLibrary,
                        Owner = _owner,
                        IsForSale = _isForSale,
                        Price = _price,
                        IsLent = _isLent,
                        IsBorrowed = _isBorrowed,
                        DateOfBorrowedOrLent = _dateOfBorrowedOrLent,
                    }
                };

                //bookRepository.AddBatch(books);
                fileRepository.AddBatch(books);

                if (inputBreak == "q")
                {
                    break;
                }
                else
                {
                    _userCommunication.MainMethodsHeaders("\nDodaj nową książkę:" + Environment.NewLine + "==================");
                }
            }
        }

        void RemoveBook(IRepository<Book> fileRepository)
        {
            string? input = _userCommunication.WriteRemovedBookTitle();
            var books = fileRepository.GetAll();
            var bookToRemove = books.FirstOrDefault(x => x.Title == input);

            if (bookToRemove != null)
            {
                fileRepository.Remove(bookToRemove);
            }
            else
            {
                throw new Exception($"Book '{input}' hasn't found in your library");
            }
        }

        var originalBook = new Book { Id = 101, AuthorName = "John Ronald Reuel", AuthorSurname = "Tolkien", Title = "Władca pierścieni" };
        var copyBook = originalBook.Copy();
        _userCommunication.WriteCopyBookToConsole(copyBook);

        string InputIsNullOrEmpty(string? input, string inf)
        {
            switch (inf)
            {
                case "Informacja obowiązkowa; dane muszą być wprowadzone":
                    while (string.IsNullOrEmpty(input))
                    {
                        input = _userCommunication.EnteringMandatoryData(inf);
                    }
                    break;
                case "Podana wartość jest null / informacja opcjonalana":
                    if (string.IsNullOrEmpty(input))
                    {
                        input = null;
                        _userCommunication.MessageOptionalData(inf);
                    }
                    break;
            }

            return input;
        }

        void WriteAuditInfoToFileAndConsole(object? sender, Book e, string auditFileName, string auditInfo)
        {
            using (var streamWriter = new StreamWriter(auditFileName, true))
            {
                streamWriter.Write($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]-[{auditInfo}]-['{e.Title}' (Id: {e.Id}) from {sender?.GetType().Name}]" + Environment.NewLine);
            }
            _userCommunication.WriteAuditInfoToConsoleUsingEventHandler(sender, e, auditInfo);
        }

        void BoolValidation(out string? input, out bool _isProperty, string property)
        {
            input = _userCommunication.WriteBoolPropertyValue(property);
            if (input == "+")
            {
                input = "true";
            }
            else if (input == "-" || string.IsNullOrEmpty(input))
            {
                input = "false";
            }
            else
            {
                throw new Exception($"Podane dane w '{property}' mają niewłaściwą wartość;" +
                    "wpisz '+' jeśli jest wypożyczona, '-' jeśli nie jest, albo zostaw pole puste");
            }
            _isProperty = bool.Parse(input);
        }

        void FilterBooks(IBooksDataProvider _booksDataProvider)
        {
            while (true)
            {
                string? input = _userCommunication.BooksFilterMenu();
                if (input == "q")
                {
                    break;
                }
                switch (input)
                {
                    case "a":
                        var cost = _booksDataProvider.GetMinimumPriceOffAllBooks();
                        _userCommunication.WriteMinimumPriceOffAllBooksToConsole(cost);
                        break;
                    case "b":
                        _userCommunication.FilterHeader("Alfabetyczna lista wszystkich włścicieli książek z mojej biblioteki:" + Environment.NewLine
                            + "=====================================================================");
                        foreach (var item in _booksDataProvider.DistinctAllOwners())
                        {
                            _userCommunication.WriteFilterPropertyToConsole(item);
                        }
                        break;
                    case "c":
                        _userCommunication.FilterHeader("Tylko autorzy i tytuły:" + Environment.NewLine + "=========================");
                        foreach (var item in _booksDataProvider.GetOnlyAuthorAndTitle())
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "d":
                        _userCommunication.FilterHeader("Książki wg tytułów:" + Environment.NewLine + "=====================");
                        foreach (var item in _booksDataProvider.OrderByTitle())
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "e":
                        _userCommunication.FilterHeader("Książki wg tytułów od 'z':" + Environment.NewLine + "============================");
                        foreach (var item in _booksDataProvider.OrderByTitleDescending())
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "f":
                        _userCommunication.FilterHeader("Książki wg nazwisk autorów i wg tytułów:" + Environment.NewLine
                            + "==========================================");
                        foreach (var item in _booksDataProvider.OrderByAuthorSurnameAndTitle())
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "g":
                        _userCommunication.FilterHeader("Książki wg nazwisk autorów i wg tytułów od'z':" + Environment.NewLine
                            + "================================================");
                        foreach (var item in _booksDataProvider.OrderByAuthorSurnameAndTitleDesc())
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "h":
                        input = _userCommunication.InputFilterData("Podaj znak / znaki rozpoczynające tytuł:");
                        _userCommunication.FilterHeader("Książki, których tytuły zaczynają się na '{input}':" + Environment.NewLine
                            + "===============================================");
                        foreach (var item in _booksDataProvider.WhereStartsWith(input))
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "i":
                        var input1 = _userCommunication.InputFilterData("\nPodaj znak/znaki rozpoczynające tytuł:");
                        var input2 = _userCommunication.InputFilterData("\nPodaj liczbę większą od '0'");
                        if (decimal.TryParse(input2, out decimal result) && result > 0)
                        {
                            cost = result;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'minimalny koszt książki' mają niewłaściwą wartość; wpisz liczbę większą od '0'");
                        }
                        _userCommunication.FilterHeader("Książki, które zaczynają się na literę '{input1}' i kosztują więcej niż {cost:c}:"
                            + Environment.NewLine + "===================================================================================");
                        foreach (var item in _booksDataProvider.WhereStartsWithAndCostIsGreaterThan(input1, cost))
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "j":
                        input = _userCommunication.InputFilterData("\nPodaj nazwę włąściciela:");
                        _userCommunication.FilterHeader("Książki, których właścicielem jest {input}:" + Environment.NewLine
                            + "================================================");
                        foreach (var item in _booksDataProvider.WhereOwnerIs(input))
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "k":
                        input = _userCommunication.InputFilterData("\nPodaj nazwę włąściciela:");
                        _userCommunication.FilterHeader("Tytuły Książek, których właścicielem jest {input}:" + Environment.NewLine
                            + "=====================================================");
                        foreach (var item in _booksDataProvider.WhereTitlesOfBooksWhoOwnerIs(input))
                        {
                            _userCommunication.WriteFilterPropertyToConsole(item);
                        }
                        break;
                    case "l":
                        input = _userCommunication.InputFilterData("\nPodaj liczbę większą od '0'");
                        int minPagesNumber;
                        if (int.TryParse(input, out int result1) && result1 > 0)
                        {
                            minPagesNumber = result1;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'objętość książki' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        _userCommunication.FilterHeader("Książki, których objętość jest większa niż {minPagesNumber} stron(y):" + Environment.NewLine
                            + "=======================================================");
                        foreach (var item in _booksDataProvider.WhereVolumeIsGreaterThan(minPagesNumber))
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "m":
                        _userCommunication.FilterHeader("Książki wypożyczone:" + Environment.NewLine + "====================");
                        foreach (var item in _booksDataProvider.WhereIsBorrowed())
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "n":
                        _userCommunication.FilterHeader("Książki pożyczone:" + Environment.NewLine + "==================");
                        foreach (var item in _booksDataProvider.WhereIsLent())
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "o":
                        _userCommunication.FilterHeader("Książki na sprzedaż:" + Environment.NewLine + "====================");
                        foreach (var item in _booksDataProvider.WhereIsForSale())
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "p":
                        _userCommunication.FilterHeader("Tylko tytuł i miejsce w bibliotece:" + Environment.NewLine + "===================================");
                        foreach (var item in _booksDataProvider.GetOnlyTitleAndPlaceInLibrary())
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "r":
                        input = _userCommunication.InputFilterData("Podaj nazwę włąściciela");
                        _userCommunication.FilterHeader($"Pierwsza książka należąca do {input}:" + Environment.NewLine
                            + "======================================");
                        var book1 = _booksDataProvider.FirstOrDefaultByOwnerWithDefault(input);
                        _userCommunication.WriteFilterBooksToConsole(book1);
                        break;
                    case "s":
                        input = _userCommunication.InputFilterData("Podaj nazwę włąściciela");
                        Console.ForegroundColor = ConsoleColor.Green;
                        _userCommunication.FilterHeader($"Ostatnia książka należąca do {input}:" + Environment.NewLine
                            + "======================================");
                        var book2 = _booksDataProvider.LastOrDefaultByOwnerWithDefault(input);
                        _userCommunication.WriteFilterBooksToConsole(book2);
                        break;
                    case "t":
                        input = _userCommunication.InputFilterData("Podaj ID:");
                        int id;
                        if (int.TryParse(input, out int result2) && result2 > 0)
                        {
                            id = result2;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'ID' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        _userCommunication.FilterHeader($"Książka o ID: {id}:" + Environment.NewLine + "=================");
                        var book3 = _booksDataProvider.SingleOrDefaultById(id);
                        _userCommunication.WriteFilterBooksToConsole(book3);
                        break;
                    case "u":
                        input = _userCommunication.InputFilterData("Ile pierwszych książek chcesz wyświetlić?");
                        int howMany;
                        if (int.TryParse(input, out int result3) && result3 > 0)
                        {
                            howMany = result3;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'ilość książek' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        _userCommunication.FilterHeader($"Pierwsze {howMany} książki(książek) z listy w kolejności alfabetycznej:"
                            + Environment.NewLine + "==============================================================");
                        foreach (var item in _booksDataProvider.TakeBooks(howMany))
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "v":
                        _userCommunication.InputRange(out input1, out input2);
                        int x;
                        int y;
                        if (int.TryParse(input1, out int result4) && int.TryParse(input2, out int result5) && result4 > 0 && result5 > 0)
                        {
                            x = result4;
                            y = result5;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'podaj zakres' mają niewłaściwą wartość; " +
                                "wpisz liczby całkowite większą od '0' wg wzoru (x..y)");
                        }
                        _userCommunication.FilterHeader($"Książki od {x} do {y} z listy w kolejności alfabetycznej:"
                            + Environment.NewLine + "=====================================================");
                        foreach (var item in _booksDataProvider.TakeBooks(x..y))
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "w":
                        input = _userCommunication.InputFilterData("Podaj Id:");
                        if (int.TryParse(input, out int result6) && result6 > 0)
                        {
                            id = result6;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'Id' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        _userCommunication.FilterHeader($"Książki o Id mniejszym od {id}:" + Environment.NewLine + "=============================");
                        foreach (var item in _booksDataProvider.TakeBooksWhileIdIs(id))
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "x":
                        input = _userCommunication.InputFilterData("Ile pierwszych książek chcesz pominąć?");
                        if (int.TryParse(input, out int result7) && result7 > 0)
                        {
                            howMany = result7;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'ilość książek' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        _userCommunication.FilterHeader($"Pokaż książki pomijając pierwszych {howMany} w kolejności alfabetycznej:"
                            + Environment.NewLine + "=================================================================");
                        foreach (var item in _booksDataProvider.SkipBooks(howMany))
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "y":
                        _userCommunication.FilterHeader("Pomiń pierwszą książkę w kolejności alfabetycznej i książki, któych tytuł zaczyna się na A:"
                            + Environment.NewLine + "==========================================================================================");
                        foreach (var item in _booksDataProvider.SkipBooksWhileTitleStartsWith(1, "A"))
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "z":
                        _userCommunication.FilterHeader("Lista pierwszych książek wszystkich właścicieli z mojej biblioteki, alfabetycznie wg właścicieli:"
                            + Environment.NewLine + "==========================================================================================");
                        foreach (var item in _booksDataProvider.DistinctByOwners())
                        {
                            _userCommunication.WriteFilterBooksToConsole(item);
                        }
                        break;
                    case "ax":
                        input = _userCommunication.InputFilterData("Na ilu elementowe grupy / paczki chcesz podzielić wszystkie książki?");
                        if (int.TryParse(input, out int result8) && result8 > 0)
                        {
                            howMany = result8;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'ilość książek w paczce/grupie' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        _userCommunication.FilterHeader("Podział książek na paczki {howMany}-elementowe:"
                            + Environment.NewLine + "=========================================");
                        foreach (var chunkBooks in _booksDataProvider.ChunkBooks(howMany))
                        {
                            _userCommunication.WriteChunkToConsole(chunkBooks);
                        }
                        break;
                    default:
                        _userCommunication.ExceptionWrongMenuInput();
                        break;
                }
            }
        }
    }

    private void CreateXml()
    {
        var myLibraryBooks = _csvReader.ProcessMyLibraryBook("C:Resources\\Files\\mylibrary.csv");

        var document = new XDocument();

        var books = new XElement("Books", myLibraryBooks
            .Select(x =>
            new XElement("Book",
                new XAttribute("Id", x.Id),
                new XAttribute("AuthorName", x.AuthorName ?? ""),
                new XAttribute("AuthorSurname", x.AuthorSurname ?? ""),
                new XAttribute("CollectiveAuthor", x.CollectiveAuthor ?? ""),
                new XAttribute("Title", x.Title),
                new XAttribute("PlaceInLibrary", x.PlaceInLibrary ?? ""),
                new XAttribute("PagesNumber", x.PageNumber ?? 0),
                new XAttribute("Owner", x.Owner ?? ""))));

        document.Add(books);
        document.Save("mylibrary.xml");
        Console.WriteLine("Plik XML zapisany!");
    }

    private static void QueryXml()
    {
        var document = XDocument.Load("mylibrary.xml");

        var titles = document
            .Element("Books")?
            .Elements("Book")
            .Where(x => x.Attribute("Owner")?.Value == "Donald Trump")
            .Select(x => x.Attribute("Title")?.Value);

        foreach (var title in titles)
        {
            Console.WriteLine(title);
        }
    }
}
