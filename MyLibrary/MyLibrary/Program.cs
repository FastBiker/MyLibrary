using MyLibrary.Data;
using MyLibrary.Entities;
using MyLibrary.Repositories;
using MyLibrary.Repositories.Extensions;
using MyLibrary.Entities.Extensions;
using MyLibrary.Components;


Console.WriteLine("Witamy w aplikacji 'MyLibrary', która pomoże Ci uporządkować Twój domowy zbiór książek");
Console.WriteLine("======================================================================================");
Console.WriteLine();

var bookRepository = new SqlRepository<Book>(new MyLibraryDbContext(), BookAdded);
bookRepository.ItemAdded += BookOnItemAdded;

var bookInFile = new BookInFile<Book>(BookAdded);
bookInFile.ItemAdded += BookOnItemAdded;

static void BookAdded(Book item)
{
    Console.WriteLine($"The new book '{item.Title}' has been added to your library");
}

void BookOnItemAdded(object? sender, Book e)
{
    Console.WriteLine($"Book added => {e.Title} from {sender?.GetType().Name}");
}


while (true)
{
    Console.WriteLine("(1) wyświetl wszystkie ksiązki; (2) dodaj nową książkę; (3) usuń książkę (q) opuść aplikację");
    var input = Console.ReadLine();
    if (input == "q")
    {
        break;
    }
    switch (input)
    {
        case "1":
            Console.WriteLine("Lista książek z Twojej biblioteki domowej:");
            Console.WriteLine("==========================================");
            Console.WriteLine();
            WriteAllToConsole(bookRepository);
            break;
        case "2":
            Console.WriteLine("Dodaj nową książkę, podając kolejno informacje o niej; '*' oznacza konieczność wpisania danych; " +
                "w przypadku pozostałych danych, jeśli nie chcesz ich wprowadzać, przejdź dalej, wciskając 'Enter'");
            try
            {
                AddBooks(bookRepository);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception catched: {e.Message}");
            }
            break;
        case "3":
            Console.WriteLine("Usuń książkę");
            //RemoveBook(bookRepository);
            break;
        default: 
            Console.WriteLine("Wrong input value");
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

void AddBooks(IRepository<Book> bookRepository)
{
    while (true)
    {
        var inf1 = "Informacja obowiązkowa; wypełnij pole";
        var inf2 = "jeśli nie chcesz podać danych nie wpisuj nic";

        Console.WriteLine("Wpisz imię/imiona autora(*)");
        var input = Console.ReadLine();
        var _authorName = InputIsNullOrEmpty(input, inf1);

        Console.WriteLine("Wpisz nazwisko autora(*)");
        input = Console.ReadLine();
        var _authorSurname = InputIsNullOrEmpty(input, inf1);

        Console.WriteLine("Wpisz tytuł książki(*)");
        input = Console.ReadLine();
        var _title = InputIsNullOrEmpty(input, inf1);

        Console.WriteLine("Wpisz nazwę wydawnictwa");
        input = Console.ReadLine();
        var _publishingHouse = InputIsNullOrEmpty(input, inf2);

        Console.WriteLine("Wpisz miejsce wydania");
        input = Console.ReadLine();
        var _placeOfPublication = InputIsNullOrEmpty(input, inf2);

        Console.WriteLine("Wpisz rok wydania (rrrr)");
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
            throw new Exception("Podane dane w 'rok wydania' mają niewłaściwą wartość; wpisz liczbę czterocyfrową dodatnią (rrrr)");
        }

        Console.WriteLine("Wpisz liczbę stron");
        input = Console.ReadLine();
        int? _pageNumber;
        if (int.TryParse(input, out int result2) && result2 > 0)
        {
            _pageNumber = result2;
        }
        else if (InputIsNullOrEmpty(input, inf2) == null)
        {
            _pageNumber = null;
        }
        else
        {
            throw new Exception("Podane dane w 'liczba stron' mają niewłaściwą wartość; wpisz liczbę całkowitą dodatnią");
        }

        Console.WriteLine("Wpisz ISBN");
        input = Console.ReadLine();
        var _iSBN = InputIsNullOrEmpty(input, inf2);

        Console.WriteLine("Podaj lokalizację książki w twojwj bibliotece");
        input = Console.ReadLine();
        var _placeInLibrary = InputIsNullOrEmpty(input, inf2);

        Console.WriteLine("Opis książki");
        input = Console.ReadLine();
        var _description = InputIsNullOrEmpty(input, inf2);

        Console.WriteLine("Dodaj własny komentarz odnośnie książki (np. nie/przeczyna, wypożyczona, pożyczona, na sprzedaż)");
        input = Console.ReadLine();
        var _bookstatus = InputIsNullOrEmpty(input, inf2);

        Console.WriteLine("Podaj właściciela książki");
        input = Console.ReadLine();
        var _owner = InputIsNullOrEmpty(input, inf2);

        Console.WriteLine("Wpisz cenę książki (jeśli jest na sprzedaż), wpisując dowolną liczbę większą od O wg wzoru: '00,00'");
        input = Console.ReadLine();
        decimal? _price;
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


        Console.WriteLine("Wpisz '+', jeśli książka jest wyżyczona, '-' jeśli nie jest, albo zostaw pole puste");
        input = Console.ReadLine();
        if (input == "+")
        {
            input = "true";
        }
        else if (input == "-" || InputIsNullOrEmpty(input, inf2) == null)
        {
            input = "false";
        }
        else
        {
            throw new Exception("Podane dane w 'książka jest wypożyczona' mają niewłaściwą wartość;" +
                "wpisz wpisz '+' jeśli jest wypożyczona, '-' jeśli nie jest, albo zostaw pole puste");
        }

        var _isBorrowed = bool.Parse(input);

        Console.WriteLine("Podaj datę wypożyczenia wg wzoru: dd.mm.rrrr");
        input = Console.ReadLine();
        DateTime? _dateOfBorrowed;
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

        Console.WriteLine("Wpisz 'q', żeby zapisać książkę i powrócić do menu albo wciśnij Enter, aby zapisać książkę i dodać kolejną");
        var inputBreak = Console.ReadLine();

        var books = new[]
        {
            new Book
            {
                AuthorName = _authorName,
                AuthorSurname = _authorSurname,
                Title = _title,
                PublishingHouse = _publishingHouse,
                PlaceOfPublication = _placeOfPublication,
                YearOfPublication = _yearOfPublication,
                PageNumber = _pageNumber,
                ISBN = _iSBN,
                PlaceInLibrary = _placeInLibrary,
                Description = _description,
                Bookstatus = _bookstatus,
                Owner = _owner,
                Price = _price,
                IsBorrowed = _isBorrowed,
                DateOfBorrowed = _dateOfBorrowed,
            }
        };

        bookRepository.AddBatch(books);
        //bookInFile.AddBatch(books);

        if (inputBreak == "q")
        {
            break;
        }
        else
        {
            Console.WriteLine("Dodaj nową książkę:" + Environment.NewLine + "==================");
        }
    }
}

static void RemoveBook(IRepository<Book> bookRepository)
{
    var books = bookRepository.GetAll();
    foreach (var book in books)
    {
        books.Select(x => x.Title);
        bookRepository.Remove(book);
    }
}

var originalBook = new Book { Id = 101, AuthorName = "John Ronald Reuel", AuthorSurname = "Tolkien", Title = "Władca pierścieni" };
var copyBook = originalBook.Copy();
Console.WriteLine(copyBook);

static string InputIsNullOrEmpty(string? input, string inf)
{
    switch (inf)
    {
        case "Informacja obowiązkowa; wypełnij pole":
            while (string.IsNullOrEmpty(input))
            {
                Console.WriteLine(inf);
                input = Console.ReadLine();
            }
            break;
        case "jeśli nie chcesz podać danych nie wpisuj nic":
            if (string.IsNullOrEmpty(input))
            {
                input = null;
                Console.WriteLine("Podana wartość jest null");
            }
            break;
    }

    return input;
}


//1.   „Władca pierścieni” , John Ronald Reuel Tolkien  
//2.   „Buszujący w zbożu” , Jerome David Salinger
//3.   "Harry Potter - seria" , J.K.Rowling
//4.   "Duma i uprzedzenie", Jane Austen
//5.   „Paragraf 22”, Joseph Heller
//6.   „Wielki Gatsby”, Fitzgerald Francis Scott  (o tej książce pisałam tu)
//7.   "Alicja w Krainie Czarów", Caroll Lewis
//8.   „Kubuś Puchatek” i „Chatka Puchatka” , A.A. Milne
//9.   „Anna Karenina”, Tołstoj Lew
//10.  „Sto lat samotności”, Gabriel García Márquez
//11.  "Zabić drozda", Lee Harper
//12.  "Rok 1984", Orwell George
//13.  "Jane Eyre", Charlotte Bronte
//14.  „Grona gniewu”, John Steinbeck
//15.  „Folwark zwierzęcy”, George Orwell
