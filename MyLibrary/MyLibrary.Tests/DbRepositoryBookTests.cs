using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Moq;
using MyLibrary.Data;
using MyLibrary.Data.Entities;
using MyLibrary.Data.Repositories;
using MyLibrary.Data.Repositories.Extensions;

namespace MyLibrary.Tests;

public class DbRepositoryBookTests
{
    [Test]
    public void AddBook_ShouldAddBookToDatabase()
    {
        //arrange
        var bookTest =
            new Book
            {
                AuthorName = "John Ronald Reuel",
                AuthorSurname = "Tolkien",
                Title = "W³adca pierœcieni",
                PublishingHouse = "Borussia",
                PlaceOfPublication = "Dortmund",
                YearOfPublication = 2021,
                PageNumber = 589,
                ISBN = "4567876543",
                PlaceInLibrary = "D7",
                Owner = "Piotr",
                IsForSale = true,
                Price = 43.20m
            };

        var optionsBuilder = new DbContextOptionsBuilder<MyLibraryDbContext>()
        .Options;
        var dbRepository = new DbRepository<Book>(new MyLibraryDbContext(optionsBuilder));

        //act
        dbRepository.Add(bookTest);
        var testTitle = bookTest.Title;
        var bookFromDb = dbRepository.GetAll().LastOrDefault(x => x.Title == "W³adca pierœcieni");
        var titleFromDb = bookFromDb.Title;

        //assert
        Assert.IsNotNull(titleFromDb);
        Assert.AreEqual(titleFromDb, bookTest.Title);
    }

    //[Test]
    //public void AddBatchBooks_ShouldAddBooksToDatabase()
    //{
    //    //arrange
    //    var books = new[]
    //    { 
    //        new Book { Id = 0, AuthorName = "John Ronald Reuel", AuthorSurname = "Tolkien", Title = "W³adca pierœcieni",
    //        PublishingHouse = "Borussia", PlaceOfPublication = "Dortmund", YearOfPublication = 2021, PageNumber = 589,
    //        ISBN = "4567876543", PlaceInLibrary = "D7", Owner = "Piotr", IsForSale = true, Price = 43.20m },

    //        new Book { Id = 0, AuthorName = "John Ronald Reuel", AuthorSurname = "Tolkien", Title = "W³adca pierœcieni",
    //        PublishingHouse = "Borussia", PlaceOfPublication = "Dortmund", YearOfPublication = 2021, PageNumber = 589,
    //        ISBN = "4567876543", PlaceInLibrary = "D7", Owner = "Piotr", IsForSale = true, Price = 43.20m },

    //        new Book { Id = 0, AuthorName = "John Ronald Reuel", AuthorSurname = "Tolkien", Title = "W³adca pierœcieni",
    //        PublishingHouse = "Borussia", PlaceOfPublication = "Dortmund", YearOfPublication = 2021, PageNumber = 589,
    //        ISBN = "4567876543", PlaceInLibrary = "D7", Owner = "Piotr", IsForSale = true, Price = 43.20m }
    //    };
    //    var optionsBuilder = new DbContextOptionsBuilder<MyLibraryDbContext>()
    //    .Options;
    //    var dbRepository = new DbRepository<Book>(new MyLibraryDbContext(optionsBuilder));
    //    //var db = new Database();
    //    //var dbRepository = new DbRepository<Book>(new Data.MyLibraryDbContext(
    //    //    new Microsoft.EntityFrameworkCore.DbContextOptions<Data.MyLibraryDbContext>()));

    //    //act
    //    dbRepository.AddBatch(books);
    //    dbRepository.Save();

    //    //assert
    //    Assert.AreEqual();
    //}

    [Test]
    public void AddBook_ShouldAddBookToDatabase_UsingMoq()
    {
        //arrange
        var mockSet = new Mock<DbSet<Book>>();
        var mockContext = new Mock<MyLibraryDbContext>();
        mockContext.Setup(m => m.Books).Returns(mockSet.Object);
        var testRepository = new DbRepository<Book>(mockContext.Object);
        var bookTest =
            new Book
            {
                AuthorName = "John Ronald Reuel",
                AuthorSurname = "Tolkien",
                Title = "W³adca pierœcieni",
                PublishingHouse = "Borussia",
                PlaceOfPublication = "Dortmund",
                YearOfPublication = 2021,
                PageNumber = 589,
                ISBN = "4567876543",
                PlaceInLibrary = "D7",
                Owner = "Piotr",
                IsForSale = true,
                Price = 43.20m
            };

        //Act
        
        testRepository.Add(bookTest);

        //Assert
        mockSet.Verify(m => m.Add(It.IsAny<Book>()), Times.Once);
    }

    [Test]
    public void Test4()
    {
        Assert.Pass();
    }
}

//jak zrobiæ test jednostkowy, który sprawdza czy aplikacja poprawnie dodaje ksi¹¿ki do bazy danych


//Zainstaluj NUnit**: W Visual Studio otwórz "NuGet Package Manager" i zainstaluj pakiet `NUnit` oraz `Moq` (do mockowania).
//[Test]
//public void AddBook_ShouldAddBookToDatabase()
//{
//    // Arrange
//    var book = new Book { Title = "Test Book", Author = "Author Name" };
//    var mockDbContext = new Mock<DatabaseContext>();
//    var repository = new BookRepository(mockDbContext.Object);

//    // Act
//    repository.AddBook(book);

//    // Assert
//    mockDbContext.Verify(db => db.SaveChanges(), Times.Once);
//}
//```

//Bez pakietu 'Moq'
//[Test]
//public void Test_AddBook_ShouldAddBookToRepository()
//{
//    // Arrange
//    var repository = new BookRepository();
//    var book = new Book { Title = "Test Book", Author = "Author" };

//    // Act
//    repository.AddBook(book);
//    var booksInRepo = repository.GetBooks();

//    // Assert
//    Assert.AreEqual(1, booksInRepo.Count);
//    Assert.AreEqual("Test Book", booksInRepo[0].Title);
//}
//   ```
