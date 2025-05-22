using MyLibrary.Components.CsvHandler;
using MyLibrary.Components.DataProviders;
using MyLibrary.Data;
using MyLibrary.Data.Entities;
using MyLibrary.Data.Entities.Extensions;
using MyLibrary.Data.Repositories;
using MyLibrary.Data.Repositories.Extensions;
using MyLibrary.UserCommunication;
using Microsoft.EntityFrameworkCore;
using MyLibrary.Components.InputDataValidation;
using MyLibrary.Components.ExceptionsHandler;

namespace MyLibrary;

public class App : IApp
{
    private readonly IBooksDataProvider _booksDataProvider;
    private readonly IUserCommunication _userCommunication;
    private readonly ICsvReader _csvReader;
    private readonly MyLibraryDbContext _myLibraryDbContext;
    private readonly IInputDataValidation _inputValidation;
    private readonly IExceptionsHandler _exceptionsHandler;
    private const string auditFileName = "audit_library.txt";
    private const string forbiddenCharacters = ":*?\"<>/|\\";

    public App(IBooksDataProvider booksDataProvider,
        IUserCommunication userCommunication, ICsvReader csvReader, MyLibraryDbContext myLibraryDbContext, 
        IInputDataValidation inputDataValidation, IExceptionsHandler exceptionsHandler)
    {
        _booksDataProvider = booksDataProvider;
        _userCommunication = userCommunication;
        _csvReader = csvReader;
        _myLibraryDbContext = myLibraryDbContext;
        _myLibraryDbContext.Database.EnsureCreated();
        _inputValidation = inputDataValidation;
        _exceptionsHandler = exceptionsHandler;
    }
    public void Run()
    {
        _userCommunication.Welcome();

        var optionsBuilder = new DbContextOptionsBuilder<MyLibraryDbContext>()
            .Options;

        using (var myLibraryDbContext = new MyLibraryDbContext(optionsBuilder))
        {
            var dbRepository = new DbRepository<Book>(myLibraryDbContext, BookAdded, BookRemoved, BookUpdated);
            dbRepository.ItemAdded += BookOnItemAdded;
            dbRepository.ItemRemoved += BookOnItemRemoved;
            dbRepository.ItemUpdated += BookOnItemUpdated;

            void BookAdded(Book item)
            {
                _userCommunication.WriteAuditInfoToConsoleUsingCallback(item, "BookAdded");
            }

            void BookRemoved(Book item)
            {
                _userCommunication.WriteAuditInfoToConsoleUsingCallback(item, "BookDeleted");
            }

            void BookUpdated(Book item)
            {
                _userCommunication.WriteAuditInfoToConsoleUsingCallback(item, "BookUpdated");
            }

            void BookOnItemAdded(object? sender, Book e)
            {
                WriteAuditInfoToFileAndConsole(sender, e, auditFileName, "BookAdded");
            }

            void BookOnItemRemoved(object? sender, Book e)
            {
                WriteAuditInfoToFileAndConsole(sender, e, auditFileName, "BookDeleted");
            }

            void BookOnItemUpdated(object? sender, Book e)
            {
                WriteAuditInfoToFileAndConsole(sender, e, auditFileName, "BookUpdated");
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
                        catch    (Exception e)
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
                    case "6":
                        _userCommunication.MainMethodsHeaders("Utwórz unikalną nazwę pliku CSV, do którego chcesz zapisać dane " +
                            "\n(powinna mieć przynajmniej jeden znak, " +
                            $"wykluczając znaki: '{forbiddenCharacters}' oraz '.' na końcu nazwy):");
                        try
                        {
                            InsertBooksFromDbToCsv(dbRepository);
                        }
                        catch (Exception e)
                        {
                            _userCommunication.ExceptionCatchedMainMethods(e);
                        }
                        break;
                    case "7":
                        _userCommunication.MainMethodsHeaders("Podaj ścieżkę dostępu do pliku CSV, z którego dane chcesz zapisać " +
                            "do biblioteki w bazie danych: \n/aby to się udało przygotuj najpierw plik CSV wg wzoru: " +
                            "\n------------------------------------------------------" +
                            "\n(1)nagłówek: \n[Id;AuthorName;AuthorSurname;CollectiveAuthor;Title;PublishingHouse;PlaceOfPublication;" +
                            "YearOfPublication;\nPageNumber;ISBN;PlaceInLibrary;Owner;IsForSale;Price;IsLent;IsBorrowed;DateOfBorrowedOrLent], " +
                            "\n(2)pola które muszą być wypełnione: \nId, Title, (AuthorName, AuthorSurname), albo CollectiveAuthor/");
                        try
                        {
                            InsertBooksFromCsvToDb(dbRepository);
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
                            _authorName = _inputValidation.HandleInputWhenEmptyOrNull(input, inf1);

                            input = _userCommunication.WriteBookProperties("nazwisko autora(*)");
                            _authorSurname = _inputValidation.HandleInputWhenEmptyOrNull(input, inf1);

                            _collectiveAuthor = null;
                            break;

                        case "2":
                            input = _userCommunication.WriteBookProperties("autora zbiorowego(*)");
                            _collectiveAuthor = _inputValidation.HandleInputWhenEmptyOrNull(input, inf1);

                            _authorName = null;
                            _authorSurname = null;
                            break;
                        default:
                            throw new Exception("Wrong input value");
                    }

                    input = _userCommunication.WriteBookProperties("tytuł książki(*)");
                    var _title = _inputValidation.HandleInputWhenEmptyOrNull(input, inf1);

                    input = _userCommunication.WriteBookProperties("nazwę wydawnictwa");
                    var _publishingHouse = _inputValidation.HandleInputWhenEmptyOrNull(input, inf2);

                    input = _userCommunication.WriteBookProperties("miejsce wydania");
                    var _placeOfPublication = _inputValidation.HandleInputWhenEmptyOrNull(input, inf2);

                    input = _userCommunication.WriteBookProperties("rok wydania (rrrr)");

                    int? _yearOfPublication = _inputValidation.ValidateYearOfPublication(input, inf2) ? int.Parse(input) : null;

                    input = _userCommunication.WriteBookProperties("liczbę stron");

                    int? _pagesNumber = _inputValidation.ValidatePagesNumber(input, inf2) ? int.Parse(input) : null;

                    input = _userCommunication.WriteBookProperties("ISBN");
                    var _iSBN = _inputValidation.HandleInputWhenEmptyOrNull(input, inf2);

                    input = _userCommunication.WriteBookProperties("lokalizację książki w twojwj bibliotece");
                    var _placeInLibrary = _inputValidation.HandleInputWhenEmptyOrNull(input, inf2);

                    input = _userCommunication.WriteBookProperties("właściciela książki");
                    var _owner = _inputValidation.HandleInputWhenEmptyOrNull(input, inf2);

                    bool _isForSale;
                    const string propertyForSale = "czy książka jest na sprzedaż";
                    input = _userCommunication.WriteBookProperties(propertyForSale);
                    _isForSale = _inputValidation.ValidateBoolInput(input, propertyForSale);

                    decimal? _price;
                    if (_isForSale)
                    {
                        input = _userCommunication.WriteBookProperties("cenę książki (jeśli jest na sprzedaż), " +
                            "wpisując dowolną liczbę większą od O wg wzoru: '00,00'");
                        _price = _inputValidation.ValidatePrice(input, inf2) ? decimal.Parse(input) : null;
                    }
                    else
                    {
                        _price = null;
                    }

                    bool _isLent;
                    const string propertyIsLent = "czy książka jest komuś pożyczona";
                    input = _userCommunication.WriteBookProperties(propertyIsLent);
                    _isLent = _inputValidation.ValidateBoolInput(input, propertyIsLent);

                    bool _isBorrowed;
                    const string propertyIsBorrowed = "czy książka jest wypożyczona";
                    input = _userCommunication.WriteBookProperties(propertyIsBorrowed);
                    _isBorrowed = _inputValidation.ValidateBoolInput(input, propertyIsBorrowed);

                    DateTime? _dateOfBorrowedOrLent;
                    if (_isBorrowed == true || _isLent == true)
                    {
                        input = _userCommunication.WriteBookProperties("datę (wy)pożyczenia wg wzoru: dd.mm.rrrr");

                        _dateOfBorrowedOrLent = _inputValidation.ValidateDateTime(input, inf2) ? DateTime.Parse(input) : null;
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

                    _userCommunication.MainMethodsHeaders("\nDodaj nową książkę:" + Environment.NewLine + "==================");
                }
            }

            void RemoveBook(IRepository<Book> dbRepository)
            {
                Book bookToRemove = FindBookByTitleOrIdMenu(dbRepository);

                string? input = _userCommunication.QueryIfSureRemoveBook(bookToRemove);

                if (input == "1")
                {
                    dbRepository.Remove(bookToRemove);

                    dbRepository.Save();
                }

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
                                _exceptionsHandler.InputInvalidValueException("minimalny koszt książki", "wpisz liczbę większą od '0'");
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

                            int minPagesNumber = _inputValidation.ValidateIntInput(input, "objętość książki");

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

                            int id = _inputValidation.ValidateIntInput(input, "Id");

                            _userCommunication.FilterHeader($"Książka o ID: {id}:" + Environment.NewLine + "=================");
                            var book3 = _booksDataProvider.SingleOrDefaultById(id);

                            _userCommunication.WriteFilterBooksToConsole(book3);
                            break;
                        case "u":
                            input = _userCommunication.InputFilterData("Ile pierwszych książek chcesz wyświetlić?");

                            int howMany = _inputValidation.ValidateIntInput(input, "ilość książek");

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

                            id = _inputValidation.ValidateIntInput(input, "Id");

                            _userCommunication.FilterHeader($"Książki o Id mniejszym od {id}:" + Environment.NewLine + "=============================");

                            foreach (var item in _booksDataProvider.TakeBooksWhileIdIs(id))
                            {
                                _userCommunication.WriteFilterBooksToConsole(item);
                            }
                            break;
                        case "x":
                            input = _userCommunication.InputFilterData("Ile pierwszych książek chcesz pominąć?");
                            howMany = int.TryParse(input, out int result7) && result7 > 0 ? result7 : throw new Exception("\nPodane dane " +
                                "w 'ilość książek' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");

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

                            howMany = _inputValidation.ValidateIntInput(input, "ilość książek w paczce/grupie");

                            _userCommunication.FilterHeader($"Podział książek na paczki {howMany}-elementowe:"
                                + Environment.NewLine + "=========================================");

                            foreach (var chunkBooks in _booksDataProvider.ChunkBooks(howMany))
                            {
                                _userCommunication.WriteChunkToConsole(chunkBooks);
                            }
                            break;
                        case "bx":
                            input = _userCommunication.InputFilterData("Podaj Id ksiązki");

                            id = _inputValidation.ValidateIntInput(input, "Id");

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
                            dbRepository.UpdateProperty(updateBook, x => x.AuthorName = input4);
                            break;
                        case "b":
                            input4 = _userCommunication.WriteBookProperties("poprawne nazwisko autora");
                            dbRepository.UpdateProperty(updateBook, x => x.AuthorSurname = input4);
                            break;
                        case "c":
                            input4 = _userCommunication.WriteBookProperties("poprawne dane autora zbiorowego");
                            dbRepository.UpdateProperty(updateBook, x => x.CollectiveAuthor = input4);
                            break;
                        case "d":
                            input4 = _userCommunication.WriteBookProperties("poprawną tytuł(*)");
                            dbRepository.UpdateProperty(updateBook, x => x.Title = _inputValidation.HandleInputWhenEmptyOrNull(input4, inf1));
                            break;
                        case "e":
                            input4 = _userCommunication.WriteBookProperties("poprawną nazwę wydawnictwa");
                            dbRepository.UpdateProperty(updateBook, x => x.PublishingHouse = input4);
                            break;
                        case "f":
                            input4 = _userCommunication.WriteBookProperties("poprawne miejsce wydania");
                            dbRepository.UpdateProperty(updateBook, x => x.PlaceOfPublication = input4);
                            break;
                        case "g":
                            input4 = _userCommunication.WriteBookProperties("poprawny rok wydania (rrrr)");
                            
                            int? _yearOfPublication = _inputValidation.ValidateYearOfPublication(input4, inf2) ? int.Parse(input4) : null;

                            dbRepository.UpdateProperty(updateBook, x => x.YearOfPublication = _yearOfPublication);
                            break;
                        case "h":
                            input4 = _userCommunication.WriteBookProperties("poprawną liczbę stron");

                            int? _pagesNumber = _inputValidation.ValidatePagesNumber(input4, inf2) ? int.Parse(input4) : null;

                            dbRepository.UpdateProperty(updateBook, x => x.PageNumber = _pagesNumber);
                            break;
                        case "i":
                            input4 = _userCommunication.WriteBookProperties("poprawny ISBN");
                            dbRepository.UpdateProperty(updateBook, x => x.ISBN = input4);
                            break;
                        case "j":
                            input4 = _userCommunication.WriteBookProperties("aktualne miejsce w bibliotece");
                            dbRepository.UpdateProperty(updateBook, x => x.PlaceInLibrary = input4);
                            break;
                        case "k":
                            input4 = _userCommunication.WriteBookProperties("poprawne dane właściciela");
                            dbRepository.UpdateProperty(updateBook, x => x.Owner = input4);
                            break;
                        case "l":
                            bool _isForSale;
                            const string propertyForSale = "czy książka jest na sprzedaż";
                            input4 = _userCommunication.WriteBookProperties(propertyForSale);
                            _isForSale = _inputValidation.ValidateBoolInput(input4, propertyForSale);
                            dbRepository.UpdateProperty(updateBook, x => x.IsForSale = _isForSale);
                            break;
                        case "m":
                            decimal? _price;
                            if (!updateBook.IsForSale)
                            {
                                throw new Exception("Jeśli chcesz wpisać cenę ksiązki, najpierw zaznacz, że jest na sprzedaż");
                            }
                            else
                            {
                                input4 = _userCommunication.WriteBookProperties("cenę książki (jeśli jest na sprzedaż), wpisując dowolną liczbę większą od O wg wzoru: '00,00'");

                                _price = _inputValidation.ValidatePrice(input4, inf2) ? decimal.Parse(input4) : (decimal?)null;

                            }
                            dbRepository.UpdateProperty(updateBook, x => x.Price = _price);
                            break;
                        case "n":
                            bool _isLent;
                            const string propertyIsLent = "czy książka jest komuś pożyczona";
                            input4 = _userCommunication.WriteBookProperties(propertyIsLent);
                            _isLent = _inputValidation.ValidateBoolInput(input4, propertyIsLent);
                            dbRepository.UpdateProperty(updateBook, x => x.IsLent = _isLent);
                            break;
                        case "o":
                            bool _isBorrowed;
                            const string propertyIsBorrowed = "czy książka jest wypożyczona";
                            input4 = _userCommunication.WriteBookProperties(propertyIsBorrowed);
                            _isBorrowed = _inputValidation.ValidateBoolInput(input4, propertyIsBorrowed);
                            dbRepository.UpdateProperty(updateBook, x => x.IsBorrowed = _isBorrowed);
                            break;
                        case "p":         
                            DateTime? _dateOfBorrowedOrLent;
                            if (!updateBook.IsBorrowed && !updateBook.IsLent)
                            {
                                throw new Exception("Aby wpisać datę (wy)pożyczenia, należy najpierw zaznaczyć, że książka jest " +
                                    "komuć pożyczona albo od kogoś wypożyczona");
                            }
                            else
                            {
                                input4 = _userCommunication.WriteBookProperties("poprawną datę (wy)pożyczenia wg wzoru: dd.mm.rrrr");
                                _dateOfBorrowedOrLent = _inputValidation.ValidateDateTime(input4, inf2) ? DateTime.Parse(input4) : null;
                            }
                            dbRepository.UpdateProperty(updateBook, x => x.DateOfBorrowedOrLent = _dateOfBorrowedOrLent);
                            break;
                        default:
                            _userCommunication.ExceptionWrongMenuInput();
                            break;
                    }

                    dbRepository.Save();
                }
            }

            void InsertBooksFromDbToCsv(IRepository<Book> repository)
            {
                var inputFileName = _userCommunication.WriteInput();

                _inputValidation.ValidateFileName(inputFileName, forbiddenCharacters);

                if (File.Exists($"{inputFileName}.csv"))
                    throw new Exception($"Plik '{inputFileName}.csv' już istnieje! Podaj inną nazwę!");

                var fileRepository = new FileRepository<Book>(inputFileName, BookAdded);
                fileRepository.ItemAdded += BookOnItemAdded;

                var items = repository.GetAll().ToArray();
                fileRepository.AddBatch(items);
            }

            void InsertBooksFromCsvToDb(IRepository<Book> dbRepository)
            {
                var inputFilePath = _userCommunication.WriteInput();
                var items = _csvReader.ProcessMyLibraryBook(inputFilePath).ToArray();

                if (!File.Exists(inputFilePath))
                    throw new Exception($"Podany plik nie została znaleziony!");

                dbRepository.AddBatch(items);
            }
        }
    }

    private void WriteAuditInfoToFileAndConsole(object? sender, Book e, string auditFileName, string auditInfo)
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
                foundBook = _booksDataProvider.FindBookByTitle(dbRepository);
                break;
            case "2":
                foundBook = _booksDataProvider.FindBookById(dbRepository);
                break;
            default:
                throw new Exception("Wrong input value");
        }

        return foundBook;
    }
}
