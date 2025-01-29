using MyLibrary.Data;
using MyLibrary.DataProviders;
using MyLibrary.Entities;
using MyLibrary.Entities.Extensions;
using MyLibrary.Repositories;
using MyLibrary.Repositories.Extensions;
using MyLibrary.UserCommunication;

namespace MyLibrary;

public class App : IApp
{
    private readonly IRepository<Book> _fileRepository;
    private readonly IBooksDataProvider _booksDataProvider;
    private readonly IUserCommunication _userCommunication;

    public App(IRepository<Book> fileRepository, IBooksDataProvider booksDataProvider, IUserCommunication userCommunication)
    {
        _fileRepository = fileRepository;
        _booksDataProvider = booksDataProvider;
        _userCommunication = userCommunication;
    }
    public void Run()
    {
        _userCommunication.Welcome();
        //Console.ForegroundColor = ConsoleColor.Red;
        //Console.WriteLine("Witamy w aplikacji 'MyLibrary', która pomoże Ci uporządkować Twój domowy zbiór książek");
        //Console.WriteLine("======================================================================================");
        //Console.ResetColor();
        //Console.WriteLine();

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
            //_userCommunication.Menu();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("(1) wyświetl wszystkie książki; (2) dodaj nową książkę; (3) usuń książkę; (4) filtry; (q) opuść aplikację");
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

                case "4":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Filtry:");
                    Console.WriteLine("-------");
                    Console.ResetColor();
                    try
                    {
                        FilterBooks(_booksDataProvider);
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

                DateTime? _dateOfBorrowedOrLent;
                if (_isBorrowed == true || _isLent == true)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("\nPodaj datę (wy)pożyczenia wg wzoru: dd.mm.rrrr");
                    Console.ResetColor();
                    input = Console.ReadLine();
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
                        DateOfBorrowedOrLent = _dateOfBorrowedOrLent,
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

        //###########################################################################################################
        static void FilterBooks(IBooksDataProvider _booksDataProvider)
        {
            while(true)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("(q) wróć do menu; \n(a) najtańsza książka; \n(b) włąściciele książek alfabetycznie; \n(c) tylko autorzy i tytuły; " +
                    "\n(d) książki wg tytułów alfabetycznie; \n(e) książki wg tytułów od 'z'; \n(f) książki wg nazwisk autorów i wg tytułów; " +
                    "\n(g) książki wg nazwisk autorów i wg tytułów od'z'; \n(h) książki, które zaczynają się na wybraną literę; " +
                    "\n(i) książki, które zaczynają się na wybraną literę i kosztują więcej niż wybrana kwota; " +
                    "\n(j) książki, których właścicielem jest wybrana osoba; \n(k) tytuły książek danego właściciela " +
                    "\n(l) książki, których objętość jest większa niż wybrana liczba stron; " +
                    "\n(m) książki wypożyczone od kogoś; \n(n) książki pożyczone komuś; \n(o) książki na sprzedaż; " +
                    "\n(p) tylko tytuł i lokalizacja w bibliotece??; \n(r) pierwsza książka w bibliotece należąca do danego włąśiciela; " +
                    "\n(s) ostatnia książka na liście, należąca do danego właściciela; \n(t) książka o daym Id; " +
                    "\n(u) pierwsze 'x' książek z listy w kolejności alfabetycznej; " +
                    "\n(v) książki w zakresie (x..y) z listy w kolejności alfabetycznej; \n(w) książki o Id mniejszym od 'x'; " +
                    "\n(x) książki pomijając pierwszych 'x' w kolejności alfabetycznej; " +
                    "\n(y) pomiń pierwszą ksiązkę w kolejności alfabetycznej i książki, któych tytuł zaczyna się na 'A';" +
                    "\n(z) lista pierwszych książek wszystkich właścicieli, alfabetycznie wg właścicieli;" +
                    "\n(ax) podział książek na paczki x-elementowe");
                Console.ResetColor();
                var input = Console.ReadLine();
                if (input == "q")
                {
                    break;
                }
                switch (input)
                {
                    case "a":
                        var cost = _booksDataProvider.GetMinimumPriceOffAllBooks();
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"Najtańsza książka w twojej bibliotece kosztuje {cost:c}");
                        Console.ResetColor();
                        break;
                    case "b":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nAlfabetyczna lista wszystkich włścicieli książek z mojej biblioteki:");
                        Console.WriteLine("=====================================================================");
                        foreach (var item in _booksDataProvider.DistinctAllOwners())
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "c":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nTylko autorzy i tytuły:");
                        Console.WriteLine("=========================");
                        foreach (var item in _booksDataProvider.GetOnlyAuthorAndTitle())
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "d":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nKsiążki wg tytułów:");
                        Console.WriteLine("=====================");
                        foreach (var item in _booksDataProvider.OrderByTitle())
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "e":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nKsiążki wg tytułów od 'z':");
                        Console.WriteLine("============================");
                        foreach (var item in _booksDataProvider.OrderByTitleDescending())
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "f":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nKsiążki wg nazwisk autorów i wg tytułów:");
                        Console.WriteLine("==========================================");
                        foreach (var item in _booksDataProvider.OrderByAuthorSurnameAndTitle())
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "g":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nKsiążki wg nazwisk autorów i wg tytułów od'z':");
                        Console.WriteLine("================================================");
                        foreach (var item in _booksDataProvider.OrderByAuthorSurnameAndTitleDesc())
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "h":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nPodaj znak/znaki rozpoczynające tytuł:");
                        input = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nKsiążki, których tytuły zaczynają się na '{input}':");
                        Console.WriteLine("===============================================");
                        foreach (var item in _booksDataProvider.WhereStartsWith(input))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "i":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nPodaj znak/znaki rozpoczynające tytuł:");
                        var input1 = Console.ReadLine();
                        Console.WriteLine("\nPodaj liczbę większą od '0'");
                        var input2 = Console.ReadLine();
                        if (decimal.TryParse(input2, out decimal result) && result > 0)
                        {
                            cost = result;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'minimalny koszt książki' mają niewłaściwą wartość; wpisz liczbę większą od '0'");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nKsiążki, które zaczynają się na literę '{input1}' i kosztują więcej niż {cost:c}:");
                        Console.WriteLine("===================================================================================");
                        foreach (var item in _booksDataProvider.WhereStartsWithAndCostIsGreaterThan(input1, cost))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "j":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nPodaj nazwę włąściciela:");
                        input = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nKsiążki, których właścicielem jest {input}:");
                        Console.WriteLine("================================================");
                        foreach (var item in _booksDataProvider.WhereOwnerIs(input))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "k":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nPodaj nazwę włąściciela:");
                        input = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nTytuły Książek, których właścicielem jest {input}:");
                        Console.WriteLine("=====================================================");
                        foreach (var item in _booksDataProvider.WhereTitlesOfBooksWhoOwnerIs(input))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "l":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nPodaj liczbę większą od '0'");
                        input = Console.ReadLine();
                        int minPagesNumber;
                        if (int.TryParse(input, out int result1) && result1 > 0)
                        {
                            minPagesNumber = result1;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'objętość książki' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nKsiążki, których objętość jest większa niż {minPagesNumber} stron(y):");
                        Console.WriteLine("=======================================================");
                        foreach (var item in _booksDataProvider.WhereVolumeIsGreaterThan(minPagesNumber))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "m":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nKsiążki wypożyczone:");
                        Console.WriteLine("====================");
                        foreach (var item in _booksDataProvider.WhereIsBorrowed())
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "n":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nKsiążki pożyczone:");
                        Console.WriteLine("==================");
                        foreach (var item in _booksDataProvider.WhereIsLent())
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "o":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nKsiążki na sprzedaż:");
                        Console.WriteLine("====================");
                        foreach (var item in _booksDataProvider.WhereIsForSale())
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "p":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nTylko tytuł i miejsce w bibliotece:");
                        Console.WriteLine("===================================");
                        foreach (var item in _booksDataProvider.GetOnlyTitleAndPlaceInLibrary())
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "r":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nPodaj nazwę włąściciela");
                        input = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nPierwsza książka należąca do {input}:");
                        Console.WriteLine("======================================");
                        var book1 = _booksDataProvider.FirstOrDefaultByOwnerWithDefault(input);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(book1);
                        Console.ResetColor();
                        break;
                    case "s":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nPodaj nazwę włąściciela");
                        input = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nOstatnia książka na liście książka należąca do {input}:");
                        Console.WriteLine("==============================================================");
                        var book2 = _booksDataProvider.LastOrDefaultByOwnerWithDefault(input);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(book2);
                        Console.ResetColor();
                        break;
                    case "t":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nPodaj ID:");
                        input = Console.ReadLine();
                        int id;
                        if (int.TryParse(input, out int result2) && result2 > 0)
                        {
                            id = result2;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'ID' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nKsiążka o ID: {id}:");
                        Console.WriteLine("=====================");
                        var book3 = _booksDataProvider.SingleOrDefaultById(id);
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine(book3);
                        Console.ResetColor();
                        break;
                    case "u":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nIle pierwszych książek chcesz wyświetlić?");
                        input = Console.ReadLine();
                        int howMany;
                        if (int.TryParse(input, out int result3) && result3 > 0)
                        {
                            howMany = result3;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'ilość książek' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nPierwsze {howMany} książki(książek) z listy w kolejności alfabetycznej:");
                        Console.WriteLine("==============================================================");
                        foreach (var item in _booksDataProvider.TakeBooks(howMany))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "v":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nPodaj zakres, w któym chcesz wyświetlić książki wg wzoru 'x..y':");
                        input1 = Console.ReadLine();Console.WriteLine("..");input2 = Console.ReadLine();
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
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nPokaż książki od {x} do {y} z listy w kolejności alfabetycznej:");
                        Console.WriteLine("================================================================");
                        foreach (var item in _booksDataProvider.TakeBooks(x..y))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "w":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nPodaj Id:");
                        input = Console.ReadLine();
                        if (int.TryParse(input, out int result6) && result6 > 0)
                        {
                            id = result6;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'Id' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nPokaż książki o Id mniejszym od {id}:");
                        Console.WriteLine("=====================================");
                        foreach (var item in _booksDataProvider.TakeBooksWhileIdIs(id))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "x":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nIle pierwszych książek chcesz pominąć?");
                        input = Console.ReadLine();
                        if (int.TryParse(input, out int result7) && result7 > 0)
                        {
                            howMany = result7;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'ilość książek' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nPokaż książki pomijając pierwszych {howMany} w kolejności alfabetycznej:");
                        Console.WriteLine("=======================================================================");
                        foreach (var item in _booksDataProvider.SkipBooks(howMany))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "y":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nPomiń pierwszą książkę w kolejności alfabetycznej i książki, któych tytuł zaczyna się na A:");
                        Console.WriteLine("=============================================================================================");
                        foreach (var item in _booksDataProvider.SkipBooksWhileTitleStartsWith(1, "A"))
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "z":
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("\nLista pierwszych książek wszystkich właścicieli książek z mojej biblioteki, alfabetycznie wg właścicieli:");
                        Console.WriteLine("=========================================================================================================");
                        foreach (var item in _booksDataProvider.DistinctByOwners())
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine($"\n{item}");
                            Console.ResetColor();
                        }
                        break;
                    case "ax":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\nNa ilu elementowe grupy/paczki chcesz podzielić wszystkie książki?");
                        input = Console.ReadLine();
                        if (int.TryParse(input, out int result8) && result8 > 0)
                        {
                            howMany = result8;
                        }
                        else
                        {
                            throw new Exception("\nPodane dane w 'ilość książek w paczce/grupie' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'");
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nPodział książek na paczki {howMany}-elementowe:");
                        Console.WriteLine("=========================================");
                        foreach (var chunkBooks in _booksDataProvider.ChunkBooks(howMany))
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine($"\nCHUNK {chunkBooks}");
                            foreach (var item in chunkBooks)
                            {
                                Console.ForegroundColor = ConsoleColor.Blue;
                                Console.WriteLine(item);
                                Console.ResetColor();
                            }
                            Console.WriteLine("############################################");
                        }
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Wrong input value");
                        Console.ResetColor();
                        break;
                }
            }
            


        }
    }
}
