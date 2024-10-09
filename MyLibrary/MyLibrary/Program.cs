using MyLibrary.Data;
using MyLibrary.Entities;
using MyLibrary.Repositories;


var bookRepository = new SqlRepository<Book>(new MyLibraryDbContext());
AddBooks(bookRepository);
AddPeterBooks(bookRepository);
AddJowiBooks(bookRepository);
AddChildrenBooks(bookRepository);
WriteAllToConsole(bookRepository);

static void AddBooks(IRepository<Book> bookRepository)
{
    bookRepository.Add(new Book { AuthorName = "John Ronald Reuel", AuthorSurname = "Tolkien", Title = "Władca pierścieni" });
    bookRepository.Add(new Book { AuthorName = "Jerome David", AuthorSurname = "Salinger", Title = "Buszujący w zbożu" });
    bookRepository.Save();
}

static void AddPeterBooks(IWriteRepository<PeterBook> peterBookRepository)
{
    peterBookRepository.Add(new PeterBook { AuthorName = "Jane", AuthorSurname = "Austen", Title = "Duma i uprzedzenie" });
    peterBookRepository.Add(new PeterBook { AuthorName = "Joseph", AuthorSurname = "Heller", Title = "Paragraf 22" });
    peterBookRepository.Add(new PeterBook { AuthorName = "Lee", AuthorSurname = "Harper", Title = "Zabić drozda" });
    peterBookRepository.Save();
}

static void AddJowiBooks(IWriteRepository<JowiBook> jowiBookRepository)
{
    jowiBookRepository.Add(new JowiBook { AuthorName = "Lew", AuthorSurname = "Tołstoj", Title = "Anna Karenina" });
    jowiBookRepository.Add(new JowiBook { AuthorName = "Gabriel García", AuthorSurname = "Márquez", Title = "Sto lat samotności" });
    jowiBookRepository.Add(new JowiBook { AuthorName = "Fitzgerald Francis", AuthorSurname = "Scott", Title = "Wielki Gatsby" });
    jowiBookRepository.Save();
}

static void AddChildrenBooks(IWriteRepository<ChildrenBook> chlidrenRepository)
{
    chlidrenRepository.Add(new ChildrenBook { AuthorName = "Caroll", AuthorSurname = "Lewis", Title = "Alicja w Krainie Czarów" });
    chlidrenRepository.Add(new ChildrenBook { AuthorName = "Alan Alexander", AuthorSurname = "Milne", Title = "Kubuś Puchatek" });
    chlidrenRepository.Add(new ChildrenBook { AuthorName = "Alan Alexander", AuthorSurname = "Milne", Title = "Chatka Puchatka" });
    chlidrenRepository.Add(new ChildrenBook { AuthorName = "Hans Christian", AuthorSurname = "Andersen", Title = "Baśnie" });
    chlidrenRepository.Add(new ChildrenBook { AuthorName = "Henryk", AuthorSurname = "Sienkiewicz", Title = "W pustyni i w puszczy" });
    chlidrenRepository.Save();

}

static void WriteAllToConsole(IReadRepository<IEntity> repository)
{
    var items = repository.GetAll();
    foreach (var item in items)
    {
        Console.WriteLine(item);
    }
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
