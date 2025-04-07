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
//using AutoMapper;
using System.Threading.Tasks;
using System;
using static System.Reflection.Metadata.BlobBuilder;
//using MyLibrary.Components.MappingProfile;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using static System.Runtime.InteropServices.JavaScript.JSType;
using MyLibrary.Components.InputDataValidation;

namespace MyLibrary;

public class App : IApp
{
    private readonly IRepository<Book> _dbRepository;
    private readonly IBooksDataProvider _booksDataProvider;
    private readonly IUserCommunication _userCommunication;
    private readonly ICsvReader _csvReader;
    private readonly MyLibraryDbContext _myLibraryDbContext;
    private readonly IInputDataValidation _inputValidation;
    //private readonly IMapper _mapper;

    public App(IRepository<Book> dbRepository, IBooksDataProvider booksDataProvider,
        IUserCommunication userCommunication, ICsvReader csvReader, MyLibraryDbContext myLibraryDbContext, 
        IInputDataValidation inputDataValidation)//, IMapper mapper)
    {
        _dbRepository = dbRepository;
        _booksDataProvider = booksDataProvider;
        _userCommunication = userCommunication;
        _csvReader = csvReader;
        _myLibraryDbContext = myLibraryDbContext;
        _myLibraryDbContext.Database.EnsureCreated();
        _inputValidation = inputDataValidation;
        //_mapper = mapper;
    }
    public void Run()
    {
        //InsertData();
        //InsertDataWithAutoMapper();
        //ReadAllBookFromDb();
        //ReadGrupedBooksFromDb();


        //var monika = this.ReadFirst("Nela na wyspie kangura");
        //_myLibraryDbContext.Books.Remove(monika);
        //_myLibraryDbContext.SaveChanges();

        //zapisywanie nowych książek do pliku JSON, usuwanie, odczytywanie, filtrowanie
        _userCommunication.Welcome();

        string auditFileName = "audit_library.txt";

        var optionsBuilder = new DbContextOptionsBuilder<MyLibraryDbContext>()
            .Options;

        using (var myLibraryDbContext = new MyLibraryDbContext(optionsBuilder))
        {
            var dbRepository = new DbRepository<Book>(myLibraryDbContext, BookAdded, BookRemoved);
            dbRepository.ItemAdded += BookOnItemAdded;
            dbRepository.ItemRemoved += BookOnItemRemoved;

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
                            WriteAllToConsole(dbRepository);
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
                            AddBooks(dbRepository);
                        }
                        catch (Exception e)
                        {
                            _userCommunication.ExceptionCatchedMainMethods(e);
                        }
                        break;

                    case "3":
                        _userCommunication.MainMethodsHeaders("Wskaż i usuń książkę;" 
                            +"\n(1) podając tytuł; (2) podając ID; wciśnij Enter, aby wróć do menu głównego");
                        try
                        {
                            RemoveBook(dbRepository);
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
                    case "5":
                        _userCommunication.MainMethodsHeaders("Wskaż książkę, której dane chcesz uaktualnić/poprawić; " +
                            "\n(1) podając tytuł; (2) podając ID; wciśnij Enter, aby wróć do menu głównego ");
                        try
                        {
                            UpdateBooks(dbRepository);
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

            void AddBooks(IRepository<Book> dbRepository)
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
                            _authorName = _inputValidation.InputIsNullOrEmpty(input, inf1);

                            input = _userCommunication.WriteBookProperties("nazwisko autora(*)");
                            _authorSurname = _inputValidation.InputIsNullOrEmpty(input, inf1);

                            _collectiveAuthor = null;
                            break;

                        case "2":
                            input = _userCommunication.WriteBookProperties("autora zbiorowego(*)");
                            _collectiveAuthor = _inputValidation.InputIsNullOrEmpty(input, inf1);

                            _authorName = null;
                            _authorSurname = null;
                            break;
                        default:
                            throw new Exception("Wrong input value");
                    }

                    input = _userCommunication.WriteBookProperties("tytuł książki(*)");
                    var _title = _inputValidation.InputIsNullOrEmpty(input, inf1);

                    input = _userCommunication.WriteBookProperties("nazwę wydawnictwa");
                    var _publishingHouse = _inputValidation.InputIsNullOrEmpty(input, inf2);

                    input = _userCommunication.WriteBookProperties("miejsce wydania");
                    var _placeOfPublication = _inputValidation.InputIsNullOrEmpty(input, inf2);

                    input = _userCommunication.WriteBookProperties("rok wydania (rrrr)");
                    int? _yearOfPublication;
                    if (int.TryParse(input, out int result) && result > 999 && result < 10000)
                    {
                        _yearOfPublication = result;
                    }
                    else if (_inputValidation.InputIsNullOrEmpty(input, inf2) == null)
                    {
                        _yearOfPublication = null;
                    }
                    else
                    {
                        InputInvalidValueException("rok wydania", "wpisz liczbę czterocyfrową dodatnią (rrrr)");
                        return;
                    }

                    input = _userCommunication.WriteBookProperties("liczbę stron");
                    int? _pagesNumber;
                    if (int.TryParse(input, out int result2) && result2 > 0)
                    {
                        _pagesNumber = result2;
                    }
                    else if (_inputValidation.InputIsNullOrEmpty(input, inf2) == null)
                    {
                        _pagesNumber = null;
                    }
                    else
                    {
                        InputInvalidValueException("liczba stron", "wpisz liczbę całkowitą dodatnią");
                        return;
                    }

                    input = _userCommunication.WriteBookProperties("ISBN");
                    var _iSBN = _inputValidation.InputIsNullOrEmpty(input, inf2);

                    input = _userCommunication.WriteBookProperties("lokalizację książki w twojwj bibliotece");
                    var _placeInLibrary = _inputValidation.InputIsNullOrEmpty(input, inf2);

                    input = _userCommunication.WriteBookProperties("właściciela książki");
                    var _owner = _inputValidation.InputIsNullOrEmpty(input, inf2);

                    bool _isForSale;
                    const string propertyForSale = "książka jest na sprzedaż";
                    _inputValidation.BoolValidation(out input, out _isForSale, propertyForSale);

                    decimal? _price;
                    if (_isForSale == true)
                    {
                        input = _userCommunication.WriteBookProperties("cenę książki (jeśli jest na sprzedaż), wpisując dowolną liczbę większą od O wg wzoru: '00,00'");
                        if (decimal.TryParse(input, out decimal result3) && result3 > 0)
                        {
                            _price = result3;
                        }
                        else if (_inputValidation.InputIsNullOrEmpty(input, inf2) == null)
                        {
                            _price = null;
                        }
                        else
                        {
                            InputInvalidValueException("cena książki", "wpisz dowolną liczbę większą od 0 (00,00)");
                            return;
                        }
                    }
                    else
                    {
                        _price = null;
                    }


                    bool _isLent;
                    const string propertyIsLent = "książka jest komuś pożyczona";
                    _inputValidation.BoolValidation(out input, out _isLent, propertyIsLent);

                    bool _isBorrowed;
                    const string propertyIsBorrowed = "książka jest wypożyczona";
                    _inputValidation.BoolValidation(out input, out _isBorrowed, propertyIsBorrowed);

                    DateTime? _dateOfBorrowedOrLent;
                    if (_isBorrowed == true || _isLent == true)
                    {
                        input = _userCommunication.WriteBookProperties("datę (wy)pożyczenia wg wzoru: dd.mm.rrrr");
                        if (DateTime.TryParse(input, out DateTime result4))
                        {
                            _dateOfBorrowedOrLent = result4;
                        }
                        else if (_inputValidation.InputIsNullOrEmpty(input, inf2) == null)
                        {
                            _dateOfBorrowedOrLent = null;
                        }
                        else
                        {
                            InputInvalidValueException("data (wy)pożyczenia", "podaj datę wypożyczenia wg wzoru: rrrr,mm,dd");
                            return;
                        }
                    }
                    else
                    {
                        _dateOfBorrowedOrLent = null;
                    }

                    var inputBreak = _userCommunication.SaveBook();

                    var books = new[]
                    {
                        new Book {
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
                    }};

                    dbRepository.AddBatch(books);

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

            void RemoveBook(IRepository<Book> dbRepository)
            {
                Book bookToRemove = FindBookByTitleOrIdMenu(dbRepository);

                dbRepository.Remove(bookToRemove);

                dbRepository.Save();
            }

            var originalBook = new Book { Id = 101, AuthorName = "John Ronald Reuel", AuthorSurname = "Tolkien", Title = "Władca pierścieni" };
            var copyBook = originalBook.Copy();
            _userCommunication.WriteCopyBookToConsole(copyBook);

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
                            _userCommunication.FilterHeader($"Książki, których tytuły zaczynają się na '{input}':" + Environment.NewLine
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
                                InputInvalidValueException("minimalny koszt książki", "wpisz liczbę większą od '0'");
                                return;
                            }
                            _userCommunication.FilterHeader($"Książki, które zaczynają się na literę '{input1}' i kosztują więcej niż {cost:c}:"
                                + Environment.NewLine + "===================================================================================");
                            foreach (var item in _booksDataProvider.WhereStartsWithAndCostIsGreaterThan(input1, cost))
                            {
                                _userCommunication.WriteFilterBooksToConsole(item);
                            }
                            break;
                        case "j":
                            input = _userCommunication.InputFilterData("\nPodaj nazwę włąściciela:");
                            _userCommunication.FilterHeader($"Książki, których właścicielem jest {input}:" + Environment.NewLine
                                + "================================================");
                            foreach (var item in _booksDataProvider.WhereOwnerIs(input))
                            {
                                _userCommunication.WriteFilterBooksToConsole(item);
                            }
                            break;
                        case "k":
                            input = _userCommunication.InputFilterData("\nPodaj nazwę włąściciela:");
                            _userCommunication.FilterHeader($"Tytuły Książek, których właścicielem jest {input}:" + Environment.NewLine
                                + "=====================================================");
                            foreach (var item in _booksDataProvider.WhereTitlesOfBooksWhoOwnerIs(input))
                            {
                                _userCommunication.WriteFilterPropertyToConsole(item);
                            }
                            break;
                        case "l":
                            input = _userCommunication.InputFilterData("\nPodaj liczbę większą od '0'");

                            int minPagesNumber = IntInputValidation(input, "objętość książki");

                            _userCommunication.FilterHeader($"Książki, których objętość jest większa niż {minPagesNumber} stron(y):" + Environment.NewLine
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
                            input = _userCommunication.InputFilterData("Podaj Id:");

                            int id = IntInputValidation(input, "Id");

                            _userCommunication.FilterHeader($"Książka o ID: {id}:" + Environment.NewLine + "=================");
                            var book3 = _booksDataProvider.SingleOrDefaultById(id);

                            _userCommunication.WriteFilterBooksToConsole(book3);
                            break;
                        case "u":
                            input = _userCommunication.InputFilterData("Ile pierwszych książek chcesz wyświetlić?");

                            int howMany = IntInputValidation(input, "ilość książek");

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

                            id = IntInputValidation(input, "Id");

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

                            howMany = IntInputValidation(input, "ilość książek w paczce/grupie");

                            _userCommunication.FilterHeader($"Podział książek na paczki {howMany}-elementowe:"
                                + Environment.NewLine + "=========================================");

                            foreach (var chunkBooks in _booksDataProvider.ChunkBooks(howMany))
                            {
                                _userCommunication.WriteChunkToConsole(chunkBooks);
                            }
                            break;
                        case "bx":
                            input = _userCommunication.InputFilterData("Podaj Id ksiązki");

                            id = IntInputValidation(input, "Id");

                            _userCommunication.FilterHeader($"Książka o Id = {id}:"
                                + Environment.NewLine + "===================");

                            var book4 = dbRepository.GetById(id);
                            if (book4 == null)
                            {
                                Console.WriteLine("Nie znaleziono książki o tym Id!");
                            }
                            else
                            {
                                _userCommunication.WriteFilterBooksToConsole(book4);
                            }
                            break;
                        default:
                            _userCommunication.ExceptionWrongMenuInput();
                            break;
                    }
                }
            }

            void UpdateBooks(IRepository<Book> dbRepository)
            {
                Book updateBook = FindBookByTitleOrIdMenu(dbRepository);

                _userCommunication.WriteFilterBooksToConsole(updateBook);

                while (true)
                {
                    string? input3 = _userCommunication.BookPropertiesUpdateMenu();
                    if (input3 == "q")
                    {
                        break;
                    }
                    string? input4;
                    var inf1 = "Informacja obowiązkowa; dane muszą być wprowadzone";
                    var inf2 = "Podana wartość jest null / informacja opcjonalana";
                    switch (input3)
                    {
                        case "a":
                            input4 = _userCommunication.WriteBookProperties("poprawne imię autora");
                            updateBook.AuthorName = input4;
                            break;
                        case "b":
                            input4 = _userCommunication.WriteBookProperties("poprawne nazwisko autora");
                            updateBook.AuthorSurname = input4;
                            break;
                        case "c":
                            input4 = _userCommunication.WriteBookProperties("poprawne dane autora zbiorowego");
                            updateBook.CollectiveAuthor = input4;
                            break;
                        case "d":
                            input4 = _userCommunication.WriteBookProperties("poprawną tytuł(*)");
                            updateBook.Title = _inputValidation.InputIsNullOrEmpty(input4, inf1); ;
                            break;
                        case "e":
                            input4 = _userCommunication.WriteBookProperties("poprawną nazwę wydawnictwa");
                            updateBook.PublishingHouse = input4;
                            break;
                        case "f":
                            input4 = _userCommunication.WriteBookProperties("poprawne miejsce wydania");
                            updateBook.PlaceOfPublication = input4;
                            break;
                        case "g":
                            input4 = _userCommunication.WriteBookProperties("poprawny rok wydania (rrrr)");
                            int? _yearOfPublication;
                            if (int.TryParse(input4, out int result) && result > 999 && result < 10000)
                            {
                                _yearOfPublication = result;
                            }
                            else if (_inputValidation.InputIsNullOrEmpty(input4, inf2) == null)
                            {
                                _yearOfPublication = null;
                            }
                            else
                            {
                                InputInvalidValueException("rok wydania", "wpisz liczbę czterocyfrową dodatnią (rrrr)");
                                return;
                            }
                            updateBook.YearOfPublication = _yearOfPublication;
                            break;
                        case "h":
                            input4 = _userCommunication.WriteBookProperties("poprawną liczbę stron");
                            int? _pagesNumber;
                            if (int.TryParse(input4, out int result2) && result2 > 0)
                            {
                                _pagesNumber = result2;
                            }
                            else if (_inputValidation.InputIsNullOrEmpty(input4, inf2) == null)
                            {
                                _pagesNumber = null;
                            }
                            else
                            {
                                InputInvalidValueException("liczba stron", "wpisz liczbę całkowitą dodatnią");
                                return;
                            }
                            updateBook.PageNumber = _pagesNumber;
                            break;
                        case "i":
                            input4 = _userCommunication.WriteBookProperties("poprawny ISBN");
                            updateBook.ISBN = input4;
                            break;
                        case "j":
                            input4 = _userCommunication.WriteBookProperties("aktualne miejsce w bibliotece");
                            updateBook.PlaceInLibrary = input4;
                            break;
                        case "k":
                            input4 = _userCommunication.WriteBookProperties("poprawne dane właściciela");
                            updateBook.Owner = input4;
                            break;
                        case "l":
                            bool _isForSale;
                            const string propertyForSale = "książka jest na sprzedaż";
                            _inputValidation.BoolValidation(out input4, out _isForSale, propertyForSale);
                            updateBook.IsForSale = _isForSale;
                            break;
                        case "m":
                            decimal? _price;
                            if (updateBook.IsForSale == true)
                            {
                                input4 = _userCommunication.WriteBookProperties("cenę książki (jeśli jest na sprzedaż), wpisując dowolną liczbę większą od O wg wzoru: '00,00'");
                                if (decimal.TryParse(input4, out decimal result3) && result3 > 0)
                                {
                                    _price = result3;
                                }
                                else if (_inputValidation.InputIsNullOrEmpty(input4, inf2) == null)
                                {
                                    _price = null;
                                }
                                else
                                {
                                    InputInvalidValueException("cena książki", "wpisz dowolną liczbę większą od 0 (00,00)");
                                    return;
                                }
                            }
                            else
                            {
                                throw new Exception("Jeśli chcesz wpisać cenę ksiązki, najpierw zaznacz, że jest na sprzedaż");
                            }
                            updateBook.Price = _price;
                            break;
                        case "n":
                            bool _isLent;
                            const string propertyIsLent = "książka jest komuś pożyczona";
                            _inputValidation.BoolValidation(out input4, out _isLent, propertyIsLent);
                            updateBook.IsLent = _isLent;
                            break;
                        case "o":
                            bool _isBorrowed;
                            const string propertyIsBorrowed = "książka jest wypożyczona";
                            _inputValidation.BoolValidation(out input4, out _isBorrowed, propertyIsBorrowed);
                            updateBook.IsBorrowed = _isBorrowed;
                            break;
                        case "p":
                            input4 = _userCommunication.WriteBookProperties("poprawne miejsce wydania");
                            DateTime? _dateOfBorrowedOrLent;
                            if (updateBook.IsBorrowed == true || updateBook.IsLent == true)
                            {
                                input4 = _userCommunication.WriteBookProperties("poprawną datę (wy)pożyczenia wg wzoru: dd.mm.rrrr");
                                if (DateTime.TryParse(input4, out DateTime result4))
                                {
                                    _dateOfBorrowedOrLent = result4;
                                }
                                else if (_inputValidation.InputIsNullOrEmpty(input4, inf2) == null)
                                {
                                    _dateOfBorrowedOrLent = null;
                                }
                                else
                                {
                                    InputInvalidValueException("data (wy)pożyczenia", "podaj datę wypożyczenia wg wzoru: rrrr,mm,dd");
                                    return;
                                }
                            }
                            else
                            {
                                throw new Exception("Aby wpisać datę (wy)pożyczenia, należy najpierw zaznaczyć, że książka jest " +
                                    "komuć pożyczona albo od kogoś wypożyczona");
                            }
                            updateBook.DateOfBorrowedOrLent = _dateOfBorrowedOrLent;
                            break;
                        default:
                            _userCommunication.ExceptionWrongMenuInput();
                            break;
                    }

                    dbRepository.Save();
                }
            }
        }
    }

    private static int IntInputValidation(string? input, string property)
    {
        int id;
        if (int.TryParse(input, out int result) && result > 0)
        {
            id = result;
        }
        else
        {
            throw new Exception($"\nPodane dane w '{property}' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'!");
        }

        return id;
    }

    private static void InputInvalidValueException(string property, string action)
    {
        throw new Exception(message: $"\nPodane dane w '{property}' mają niewłaściwą wartość; {action}!");
    }

    void WriteAuditInfoToFileAndConsole(object? sender, Book e, string auditFileName, string auditInfo)
    {
        using (var streamWriter = new StreamWriter(auditFileName, true))
        {
            streamWriter.Write($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]-[{auditInfo}]-['{e.Title}' (Id: {e.Id}) from {sender?.GetType().Name}]" + Environment.NewLine);
        }
        _userCommunication.WriteAuditInfoToConsoleUsingEventHandler(sender, e, auditInfo);
    }

    private Book FindBookByTitleOrIdMenu(IRepository<Book> dbRepository)
    {
        Book foundBook;
        var input = _userCommunication.WriteInput();
        switch (input)
        {
            case "1":
                foundBook = FindBookByTitle(dbRepository);
                break;
            case "2":
                foundBook = FindBookById(dbRepository);
                break;
            default:
                throw new Exception("Wrong input value");
        }

        return foundBook;
    }

    private Book FindBookByTitle(IRepository<Book> dbRepository)
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

    private Book FindBookById(IRepository<Book> dbRepository)
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

    //private Book? ReadFirst(string name)
    //{
    //    return _myLibraryDbContext.Books.FirstOrDefault(x => x.Title == name);
    //}

    //private void ReadGrupedBooksFromDb()
    //{
    //    var grups = _myLibraryDbContext.Books
    //        .GroupBy(x => x.Owner)
    //        .Select(x => new
    //        {
    //            Owner = x.Key,
    //            Books = x.OrderBy(book => book.Title).ToList()
    //        })
    //        .ToList();

    //    foreach (var grup in grups)
    //    {
    //        Console.WriteLine($"Owner: {grup.Owner}");
    //        Console.WriteLine("===================");

    //        foreach(var book in grup.Books)
    //        {
    //            Console.WriteLine($"\t{book.AuthorName} {book.AuthorSurname}{book.CollectiveAuthor}, \"{book.Title}\"");
    //        }
    //        Console.WriteLine();
    //    }
    //}

    //private void ReadAllBookFromDb()
    //{
    //    var booksFromDb = _myLibraryDbContext.Books.ToList();

    //    foreach (var bookFromDb in booksFromDb)
    //    {
    //        Console.WriteLine($"{bookFromDb.AuthorName} {bookFromDb.AuthorSurname}{bookFromDb.CollectiveAuthor}," +
    //            $"\n\t{bookFromDb.Title} \n\t{bookFromDb.PageNumber}");
    //    }
    //}

    //private void InsertDataWithAutoMapper()
    //{

    //    var optionsBuilder = new DbContextOptionsBuilder<MyLibraryDbContext>();

    //    using (var context = new MyLibraryDbContext(optionsBuilder.Options))
    //    {
    //        var books = _csvReader.ProcessMyLibraryBook("Resources\\Files\\mylibrary.csv");
    //        var bookEntities = _mapper.Map<IEnumerable<BookEntity>>(books);
    //        context.BookEntities.AddRange(bookEntities);
    //        context.SaveChanges();
    //    }
    //}

    //==================================================================================
    //using (var scope = app.Services.CreateScope())
    //{
    //    var context = scope.ServiceProvider.GetRequiredService<MyLibraryDbContext>();
    //    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

    //    var books = _csvReader.ProcessMyLibraryBook("Resources\\Files\\mylibrary.csv");
    //    var bookEntities = mapper.Map<IEnumerable<BookEntity>>(books);

    //    context.Books.AddRange(bookEntities);
    //    context.SaveChanges();
    //}
    //=================================================================================
    //var books = _csvReader.ProcessMyLibraryBook("Resources\\Files\\mylibrary.csv");

    //var source = new Book { Name = "Test" };
    //var bookEntities = _mapper.Map<BookEntity>(books);

    //_myLibraryDbContext.AddRange(bookEntities);

    //_myLibraryDbContext.SaveChanges();
    //}

    //    private void InsertData()
    //    {
    //        var books = _csvReader.ProcessMyLibraryBook("Resources\\Files\\mylibrary.csv");

    //        //var config = new MapperConfiguration(cfg => cfg.CreateMap<Book, BookEntity>());

    //        //var mapper = config.CreateMapper();

    //        //var bookEntities = mapper.Map<List<BookEntity>>(books);

    //        //_myLibraryDbContext.AddRange(bookEntities);

    //        foreach (var book in books)
    //        {
    //            _myLibraryDbContext.Books.Add(new Book
    //            {
    //                AuthorName = book.AuthorName,

    //                AuthorSurname = book.AuthorSurname,

    //                CollectiveAuthor = book.CollectiveAuthor,

    //                Title = book.Title,

    //                PublishingHouse = book.PublishingHouse,

    //                PlaceOfPublication = book.PlaceOfPublication,

    //                YearOfPublication = book.YearOfPublication,

    //                PageNumber = book.PageNumber,

    //                ISBN = book.ISBN,

    //                PlaceInLibrary = book.PlaceInLibrary,

    //                Owner = book.Owner,

    //                IsForSale = book.IsForSale,

    //                Price = book.Price,

    //                IsLent = book.IsLent,

    //                IsBorrowed = book.IsBorrowed,

    //                DateOfBorrowedOrLent = book.DateOfBorrowedOrLent,
    //            });
    //        }

    //        _myLibraryDbContext.SaveChanges();
    //    }
}
