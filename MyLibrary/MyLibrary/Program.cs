using MyLibrary.Data;
using MyLibrary.Entities;
using MyLibrary.Repositories;
using MyLibrary.Repositories.Extensions;
using MyLibrary.Entities.Extensions;


var bookRepository = new SqlRepository<Book>(new MyLibraryDbContext(), BookAdded);
bookRepository.ItemAdded += BookRepositoryOnItemAdded;

void BookRepositoryOnItemAdded(object? sender, Book e)
{
    Console.WriteLine($"Book added => {e.Title} from {sender?.GetType().Name}");
}

AddBooks(bookRepository);
WriteAllToConsole(bookRepository);

static void BookAdded(Book item)
{
    Console.WriteLine($"{item.Title} (ADDED)");
}

static void AddBooks(IRepository<Book> bookRepository)
{
    var books = new[]
    {
        new Book { AuthorName = "John Ronald Reuel", AuthorSurname = "Tolkien", Title = "Władca pierścieni" },
        new Book { AuthorName = "Jerome David", AuthorSurname = "Salinger", Title = "Buszujący w zbożu" },
        new Book { AuthorName = "Joseph", AuthorSurname = "Heller", Title = "Paragraf 22", Owner = "Peter" },
        new Book { AuthorName = "Jane", AuthorSurname = "Austen", Title = "Duma i uprzedzenie" },
        new Book { AuthorName = "Lee", AuthorSurname = "Harper", Title = "Zabić drozda" },
        new Book { AuthorName = "Lew", AuthorSurname = "Tołstoj", Title = "Anna Karenina" },
        new Book { AuthorName = "Gabriel García", AuthorSurname = "Márquez", Title = "Sto lat samotności" },
        new Book { AuthorName = "Fitzgerald Francis", AuthorSurname = "Scott", Title = "Wielki Gatsby" },
        new Book { AuthorName = "Caroll", AuthorSurname = "Lewis", Title = "Alicja w Krainie Czarów" },
        new Book { AuthorName = "Alan Alexander", AuthorSurname = "Milne", Title = "Kubuś Puchatek" },
        new Book { AuthorName = "Alan Alexander", AuthorSurname = "Milne", Title = "Chatka Puchatka" },
        new Book { AuthorName = "Hans Christian", AuthorSurname = "Andersen", Title = "Baśnie" },
        new Book { AuthorName = "Henryk", AuthorSurname = "Sienkiewicz", Title = "W pustyni i w puszczy", IsBorrowed = true }
    };

    bookRepository.AddBatch(books);
}

static void WriteAllToConsole(IReadRepository<IEntity> repository)
{
    var items = repository.GetAll();
    foreach (var item in items)
    {
        Console.WriteLine(item);
    }
}

var originalBook = new Book {Id = 101, AuthorName = "John Ronald Reuel", AuthorSurname = "Tolkien", Title = "Władca pierścieni" };
var copyBook = originalBook.Copy();
Console.WriteLine(copyBook);

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
