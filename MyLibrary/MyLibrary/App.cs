using MyLibrary.Data;
using MyLibrary.DataProviders;
using MyLibrary.Entities;
using MyLibrary.Entities.Extensions;
using MyLibrary.Repositories;
using MyLibrary.Repositories.Extensions;

namespace MyLibrary;

public class App : IApp
{
    private readonly IRepository<Book> _fileRepository;
    private readonly IBooksProvider _booksProvider;

    public App(IRepository<Book> fileRepository, IBooksProvider booksProvider)
    {
        _fileRepository = fileRepository;
        _booksProvider = booksProvider;
    }
    public void Run()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Witamy w aplikacji 'MyLibrary', która pomoże Ci uporządkować Twój domowy zbiór książek");
        Console.WriteLine("======================================================================================");
        Console.ResetColor();
        Console.WriteLine();

        string auditFileName = "audit_library.txt";

        var bookRepository = new SqlRepository<Book>(new MyLibraryDbContext(), BookAdded);
        bookRepository.ItemAdded += BookOnItemAdded;

        var bookInFile = new FileRepository<Book>(BookAdded, BookRemoved);
        bookInFile.ItemAdded += BookOnItemAdded;
        bookInFile.ItemRemoved += BookOnItemRemoved;

        static void BookRemoved(Book item)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]-[BookDeleted]-[{item.Title} (Id: {item.Id})]");
            Console.ResetColor();
        }

        static void BookAdded(Book item)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]-[BookAdded]-[{item.Title} (Id: {item.Id})]");
            Console.ResetColor();
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
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("(1) wyświetl wszystkie książki; (2) dodaj nową książkę; (3) usuń książkę (q) opuść aplikację");
            Console.ResetColor();
            var input = Console.ReadLine();
            if (input == "q")
            {
                break;
            }
            switch (input)
            {
                case "1":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Lista książek z Twojej biblioteki domowej:");
                    Console.WriteLine("==========================================");
                    Console.ResetColor();
                    Console.WriteLine();
                    try
                    {
                        WriteAllToConsole(bookRepository);
                        WriteAllToConsole(bookInFile);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Exception catched: {e.Message}");
                        Console.ResetColor();
                    }
                    break;

                case "2":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Dodaj nową książkę, podając kolejno informacje o niej; '*' oznacza konieczność wpisania danych; " +
                        "w przypadku pozostałych danych, jeśli nie chcesz ich wprowadzać, przejdź dalej, wciskając 'Enter'");
                    Console.ResetColor();
                    try
                    {
                        AddBooks(bookRepository, bookInFile);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Exception catched: {e.Message}");
                        Console.ResetColor();
                    }
                    break;

                case "3":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Usuń książkę, wpisując jej tytuł:");
                    Console.ResetColor();
                    try
                    {
                        RemoveBook(bookInFile);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Exception catched: {e.Message}");
                        Console.ResetColor();
                    }

                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Wrong input value");
                    Console.ResetColor();
                    break;
            }

        }

        static void WriteAllToConsole(IReadRepository<IEntity> repository)
        {
            var items = repository.GetAll();
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
        }

        void AddBooks(IRepository<Book> bookRepository, IRepository<Book> bookInFile)
        {
            while (true)
            {
                var inf1 = "Informacja obowiązkowa; dane muszą być wprowadzone";
                var inf2 = "Podana wartość jest null / informacja opcjonalana";

                Console.WriteLine("\nWybierz: '1' - jeden autor albo '2' - autor zbiorowy");
                var input = Console.ReadLine();
                string _authorSurname;
                string _collectiveAuthor;
                string _authorName;
                switch (input)
                {
                    case "1":
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("\nWpisz imię/imiona autora(*)");
                        Console.ResetColor();
                        input = Console.ReadLine();
                        _authorName = InputIsNullOrEmpty(input, inf1);

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("\nWpisz nazwisko autora(*)");
                        Console.ResetColor();
                        input = Console.ReadLine();
                        _authorSurname = InputIsNullOrEmpty(input, inf1);

                        _collectiveAuthor = null;
                        break;

                    case "2":
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("\nWpisz autora zbiorowego(*)");
                        Console.ResetColor();
                        input = Console.ReadLine();
                        _collectiveAuthor = InputIsNullOrEmpty(input, inf1);

                        _authorName = null; 
                        _authorSurname = null;
                        break ;
                    default:
                        throw new Exception("Wrong input value");
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nWpisz tytuł książki(*)");
                Console.ResetColor();
                input = Console.ReadLine();
                var _title = InputIsNullOrEmpty(input, inf1);

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nWpisz nazwę wydawnictwa");
                Console.ResetColor();
                input = Console.ReadLine();
                var _publishingHouse = InputIsNullOrEmpty(input, inf2);

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nWpisz miejsce wydania");
                Console.ResetColor();
                input = Console.ReadLine();
                var _placeOfPublication = InputIsNullOrEmpty(input, inf2);

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nWpisz rok wydania (rrrr)");
                Console.ResetColor();
                input = Console.ReadLine();
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

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nWpisz liczbę stron");
                Console.ResetColor();
                input = Console.ReadLine();
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

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nWpisz ISBN");
                Console.ResetColor();
                input = Console.ReadLine();
                var _iSBN = InputIsNullOrEmpty(input, inf2);

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nPodaj lokalizację książki w twojwj bibliotece");
                Console.ResetColor();
                input = Console.ReadLine();
                var _placeInLibrary = InputIsNullOrEmpty(input, inf2);

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\nPodaj właściciela książki");
                Console.ResetColor();
                input = Console.ReadLine();
                var _owner = InputIsNullOrEmpty(input, inf2);

                bool _isForSale;
                const string propertyForSale = "książka jest na sprzedaż";
                BoolValidation(out input, out _isForSale, propertyForSale);

                decimal? _price;
                if (_isForSale == true)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("\nWpisz cenę książki (jeśli jest na sprzedaż), wpisując dowolną liczbę większą od O wg wzoru: '00,00'");
                    Console.ResetColor();
                    input = Console.ReadLine();
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

                DateTime? _dateOfBorrowed;
                if (_isBorrowed == true || _isLent == true)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("\nPodaj datę (wy)pożyczenia wg wzoru: dd.mm.rrrr");
                    Console.ResetColor();
                    input = Console.ReadLine();
                    if (DateTime.TryParse(input, out DateTime result4))
                    {
                        _dateOfBorrowed = result4;
                    }
                    else if (InputIsNullOrEmpty(input, inf2) == null)
                    {
                        _dateOfBorrowed = null;
                    }
                    else
                    {
                        throw new Exception("Podane dane w 'data wypożyczenia' mają niewłaściwą wartość; " +
                            "podaj datę wypożyczenia wg wzoru: rrrr,mm,dd");
                    }
                }
                else
                {
                    _dateOfBorrowed = null;
                }

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("\nWpisz 'q', żeby zapisać książkę i powrócić do menu albo wciśnij Enter, aby zapisać książkę i dodać kolejną");
                Console.ResetColor();
                var inputBreak = Console.ReadLine();

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
                        DateOfBorrowed = _dateOfBorrowed,
                    }
                };

                //bookRepository.AddBatch(books);
                bookInFile.AddBatch(books);

                if (inputBreak == "q")
                {
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\nDodaj nową książkę:" + Environment.NewLine + "==================");
                    Console.ResetColor();
                }
            }
        }

        static void RemoveBook(IRepository<Book> bookInFile)
        {
            var input = Console.ReadLine();
            var books = bookInFile.GetAll();
            var bookToRemove = books.FirstOrDefault(x => x.Title == input);

            if (bookToRemove != null)
            {
                bookInFile.Remove(bookToRemove);
            }
            else
            {
                throw new Exception($"Book '{input}' hasn't found in your library");
            }
        }

        var originalBook = new Book { Id = 101, AuthorName = "John Ronald Reuel", AuthorSurname = "Tolkien", Title = "Władca pierścieni" };
        var copyBook = originalBook.Copy();
        Console.WriteLine(copyBook);

        static string InputIsNullOrEmpty(string? input, string inf)
        {
            switch (inf)
            {
                case "Informacja obowiązkowa; dane muszą być wprowadzone":
                    while (string.IsNullOrEmpty(input))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine(inf);
                        Console.ResetColor();
                        input = Console.ReadLine();
                    }
                    break;
                case "Podana wartość jest null / informacja opcjonalana":
                    if (string.IsNullOrEmpty(input))
                    {
                        input = null;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine(inf);
                        Console.ResetColor();
                    }
                    break;
            }

            return input;
        }

        static void WriteAuditInfoToFileAndConsole(object? sender, Book e, string auditFileName, string auditInfo)
        {
            using (var streamWriter = new StreamWriter(auditFileName, true))
            {
                streamWriter.Write($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]-[{auditInfo}]-['{e.Title}' (Id: {e.Id}) from {sender?.GetType().Name}]" + Environment.NewLine);
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]-[{auditInfo}]-['{e.Title}' (Id: {e.Id}) from {sender?.GetType().Name}]");
            Console.ResetColor();
        }

        static void BoolValidation(out string? input, out bool _isProperty, string property)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\nWpisz '+', jeśli {property}, '-' jeśli nie jest, albo zostaw pole puste");
            Console.ResetColor();
            input = Console.ReadLine();
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

        //select
        var minPrice = _booksProvider.GetMinimumPriceOffAllBooks();
        Console.WriteLine($"Najtańsza książka w twojej bibliotece kosztuje {minPrice:c}");

        Console.WriteLine($"\nWłaściciele książek z twojej biblioteki:");
        Console.WriteLine("===========================================");
        foreach (var book in _booksProvider.GetUniqueBookOwners())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nNiektóre dane z książek:");
        Console.WriteLine("========================");
        foreach (var book in _booksProvider.GetSpecificColumns())
        {
            Console.WriteLine(book);
        }

        var newBook = _booksProvider.AnonimousClass();
        Console.WriteLine($"\nWybrane dane książek:" + Environment.NewLine + $"--------------------- {newBook}");

        //order by
        Console.WriteLine("\nKsiążki wg tytułów:");
        Console.WriteLine("=====================");
        foreach (var book in _booksProvider.OrderByTitle())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nKsiążki wg tytułów od 'z':");
        Console.WriteLine("=====================");
        foreach (var book in _booksProvider.OrderByTitleDescending())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nKsiążki wg nazwisk autorów i wg tytułów:");
        Console.WriteLine("==========================================");
        foreach (var book in _booksProvider.OrderByAuthorSurnameAndTitle())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nKsiążki wg nazwisk autorów i wg tytułów od'z':");
        Console.WriteLine("================================================");
        foreach (var book in _booksProvider.OrderByAuthorSurnameAndTitleDesc())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nKsiążki, które zaczynają się na konkretną literę 'G':");
        Console.WriteLine("=======================================================");
        foreach (var book in _booksProvider.WhereStartsWith("G"))
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nKsiążki, które zaczynają się na konkretną literę 'P' i kosztują więcej niż 20 zł:");
        Console.WriteLine("===================================================================================");
        foreach (var book in _booksProvider.WhereStartsWithAndCostIsGreaterThan("P", 20))
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nKsiążki, których właścicielem jest Joe Biden:");
        Console.WriteLine("================================================");
        foreach (var book in _booksProvider.WhereOwnerIs("Joe Biden"))
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nKsiążki, których objętość jest większa niż 200 stron:");
        Console.WriteLine("=======================================================");
        foreach (var book in _booksProvider.WhereVolumeIsGreaterThan(200))
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nKsiążki wypożyczone:");
        Console.WriteLine("======================");
        foreach (var book in _booksProvider.WhereIsBorrowed())
        {
            Console.WriteLine(book);
        }

        Console.WriteLine("\nPietrwsza książka należąca do Piotra:");
        Console.WriteLine("=======================================");
        var book1 = _booksProvider.FirstByOwner("Piotr");
        Console.WriteLine(book1);

        Console.WriteLine("\nKsiążka o Id = 61:");
        Console.WriteLine("===================");
        var book2 = _booksProvider.SingleOrDefaultById(61);
        Console.WriteLine(book2);
    }
}
