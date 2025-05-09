using CsvHelper.Configuration;
using MyLibrary.Data.Entities;

namespace MyLibrary.Components.CsvHandler;

public sealed class CsvHelperConfiguration : ClassMap<Book>
{
    public CsvHelperConfiguration() 
    {
        Map(x => x.DateOfBorrowedOrLent).TypeConverterOption.Format("dd.MM.yyyy");
        Map(x => x.Id).Index(1);
        Map(x => x.AuthorName).Index(2);
        Map(x => x.AuthorSurname).Index(3);
        Map(x => x.CollectiveAuthor).Index(4);
        Map(x => x.Title).Index(5);
        Map(x => x.PublishingHouse).Index(6);
        Map(x => x.PlaceOfPublication).Index(7);
        Map(x => x.YearOfPublication).Index(8);
        Map(x => x.PageNumber).Index(9);
        Map(x => x.ISBN).Index(10);
        Map(x => x.PlaceInLibrary).Index(11);
        Map(x => x.Owner).Index(12);
        Map(x => x.IsForSale).Index(13);
        Map(x => x.Price).Index(14);
        Map(x => x.IsLent).Index(15);
        Map(x => x.IsBorrowed).Index(16);
        Map(x => x.DateOfBorrowedOrLent).Index(17);
    }

}
