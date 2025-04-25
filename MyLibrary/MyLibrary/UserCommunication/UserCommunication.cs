using MyLibrary.Data.Entities;

namespace MyLibrary.UserCommunication;

public class UserCommunication : IUserCommunication
{
    public void Welcome()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Witamy w aplikacji 'MyLibrary', która pomoże Ci uporządkować Twój domowy zbiór książek");
        Console.WriteLine("======================================================================================");
        Console.ResetColor();
        Console.WriteLine();
    }

    public string? MainMenu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("(1) wyświetl wszystkie książki; (2) dodaj nową książkę; (3) usuń książkę; (4) filtry; (5) aktualizacja; " +
            "\n(q) opuść aplikację");
        Console.ResetColor();
        var input = Console.ReadLine();
        return input;
    }

    public void MainMethodsHeaders(string header)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{header}");
        Console.ResetColor();
    }

    public void ExceptionCatchedMainMethods(Exception e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Exception catched: {e.Message}");
        Console.ResetColor();
    }

    public void ExceptionWrongMenuInput()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Wrong input value");
        Console.ResetColor();
    }

    public void WriteItemToConsole(IEnumerable<IEntity> items)
    {
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }

    public void SelectAuthor(out string inf1, out string inf2, out string? input)
    {
        inf1 = "Informacja obowiązkowa; dane muszą być wprowadzone";
        inf2 = "Podana wartość jest null / informacja opcjonalana";
        Console.WriteLine("Wybierz: '1' - jeden autor albo '2' - autor zbiorowy");
        input = Console.ReadLine();
    }

    public string? WriteBookProperties(string property)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"\nWpisz {property}");
        Console.ResetColor();
        var input = Console.ReadLine();
        return input;
    }

    public string? SaveBook()
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"\nWpisz 'q', żeby zapisać książkę i powrócić do menu albo wciśnij Enter, aby zapisać książkę i dodać kolejną!");
        Console.ResetColor();
        var input = Console.ReadLine();
        return input;
    }

    public string? QueryIfSureRemoveBook(Book bookToRemove)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Czy na pewno chcesz usunąć książkę '{bookToRemove.Title}' (Id: {bookToRemove.Id}) ze swojej biblioteki?");
        Console.WriteLine("Wpisz '1', żeby usunąć książkę albo wciśnij Enter, aby powrócić do menu!");
        Console.ResetColor();
        var input = Console.ReadLine();
        return input;
    }

    public string? WriteBookPropertyValue(string property)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Podaj {property} książki:");
        Console.ResetColor();
        var input = Console.ReadLine();
        return input;
    }

    public string? WriteInput()
    {
        return Console.ReadLine();
    }

    public void WriteCopyBookToConsole(Book? copyBook)
    {
        Console.WriteLine(copyBook);
    }

    public string? EnteringMandatoryData(string inf)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine(inf);
        Console.ResetColor();
        var input = Console.ReadLine();
        return input;
    }

    public void MessageOptionalData(string inf)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(inf);
        Console.ResetColor();
    }

    public void WriteAuditInfoToConsoleUsingCallback(Book item, string action)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]-[{action}]-[{item.Title} (Id: {item.Id})]");
        Console.ResetColor();
    }

    public void WriteAuditInfoToConsoleUsingEventHandler(object? sender, Book e, string auditInfo)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}]-[{auditInfo}]-['{e.Title}' (Id: {e.Id}) from {sender?.GetType().Name}]");
        Console.ResetColor();
    }

    public string? WriteBoolPropertyValue(string property)
    {
        string? input;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"\nWpisz '+', jeśli {property}, '-' jeśli nie jest, albo zostaw pole puste");
        Console.ResetColor();
        input = Console.ReadLine();
        return input;
    }

    public string? BooksFilterMenu()
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
            "\n(s) ostatnia książka na liście, należąca do danego właściciela; \n(t) książka o danym Id; " +
            "\n(u) pierwsze 'x' książek z listy w kolejności alfabetycznej; " +
            "\n(v) książki w zakresie (x..y) z listy w kolejności alfabetycznej; \n(w) książki o Id mniejszym od 'x'; " +
            "\n(x) książki pomijając pierwszych 'x' w kolejności alfabetycznej; " +
            "\n(y) pomiń pierwszą ksiązkę w kolejności alfabetycznej i książki, któych tytuł zaczyna się na 'A';" +
            "\n(z) lista pierwszych książek wszystkich właścicieli, alfabetycznie wg właścicieli;" +
            "\n(ax) podział książek na paczki x-elementowe; \n(bx) wyszukaj ksiązkę po Id");
        Console.ResetColor();
        var input = Console.ReadLine();
        return input;
    }

    public void WriteMinimumPriceOffAllBooksToConsole(decimal cost)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"Najtańsza książka w twojej bibliotece kosztuje {cost:c}");
        Console.ResetColor();
    }

    public void FilterHeader(string filterHeader)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n{filterHeader}");
    }

    public void WriteFilterPropertyToConsole(string item)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(item);
        Console.ResetColor();
    }

    public void WriteFilterBooksToConsole(Book item)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(item);
        Console.ResetColor();
    }

    public string? InputFilterData(string inputHeader)
    {
        string? input;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"\n{inputHeader}");
        input = Console.ReadLine();
        return input;
    }

    public void InputRange(out string? input1, out string? input2)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\nPodaj zakres 'x..y, w któym chcesz wyświetlić książki (x, y - liczby całkowite dodatnie)");
        Console.WriteLine("x=");
        input1 = Console.ReadLine();
        Console.WriteLine("y=");
        input2 = Console.ReadLine();
    }

    public void WriteChunkToConsole(Book[] chunkBooks)
    {
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine($"\nCHUNK {chunkBooks}");
        foreach (var item in chunkBooks)
        {
            WriteFilterBooksToConsole(item);
        }
        Console.WriteLine("############################################");
    }

    public string? BookPropertiesUpdateMenu()
    {
        Console.WriteLine("Podaj, którą właściwość książki chcesz uaktualnić:" +
        "\na - AuthorName; \nb - AuthorSurname; \nc - CollectiveAuthor; \nd - Title; \ne - PublishingHouse; " +
        "\nf - PlaceOfPublication; \ng - YearOfPublication; \nh - PageNumber; \ni - ISBN; \nj - PlaceInLibrary; \nk - Owner; " +
        "\nl - IsForSale; \nm - Price; \nn - IsLent; \no - IsBorrowed; \np - DateOfBorrowedOrLent; \nq - wróć do menu głównego");
        var input3 = Console.ReadLine();
        return input3;
    }

}
