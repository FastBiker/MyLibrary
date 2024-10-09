using MyLibrary.Data;
using MyLibrary.Entities;
using MyLibrary.Repositories;


var bookRepository = new SqlRepository<Book>(new MyLibraryDbContext());
bookRepository.Add(new Book { AuthorName = "John Ronald Reuel", AuthorSurname = "Tolkien", Title = "Władca pierścieni" });
bookRepository.Add(new Book { AuthorName = "Jerome David", AuthorSurname = "Salinger", Title = "Buszujący w zbożu" });
bookRepository.Add(new Book { AuthorName = "Jane", AuthorSurname = "Austen", Title = "Duma i uprzedzenie" });
bookRepository.Add(new Book { AuthorName = "Joseph", AuthorSurname = "Heller", Title = "Paragraf 22" });
bookRepository.Add(new Book { AuthorName = "Fitzgerald Francis", AuthorSurname = "Scott", Title = "Wielki Gatsby" });

GetBookById(bookRepository);
static void GetBookById(IReadRepository<IEntity> bookRepository)
{
    var book = bookRepository.GetById(2);
    Console.WriteLine(book.ToString());
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
