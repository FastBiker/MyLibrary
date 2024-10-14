namespace MyLibrary.Entities;

public class Book : EntityBase
{
    public string AuthorName { get; set; }

    public string AuthorSurname { get; set; }

    public string Title { get; set; }

    public string? PublishingHouse { get; set; }

    public string? PlaceOfPublication { get; set; }

    public int? YearOfPublication { get; set; }

    public int? PageNumber { get; set; }

    public int? ISBN { get; set; }

    public string? PlaceInLibrary { get; set; }

    public string? Description { get; set; } 

    public string? OwnComments { get; set; } // np. (nie)przeczytana, (nie)polecana itp.

    public string? BookStatus { get; set; } // is lent; for sale; 

    public string? Owner {  get; set; }

    public decimal? Price { get; set; }

    public bool IsBorrowed { get; set; }

    public string? BorrowedBookOwner { get; set; }

    public DateTime? DateOfBorrowed { get; set; }

    public override string ToString()
        => $"Id: {Id}, Author: {AuthorName} {AuthorSurname}, Title: {Title}, Owner: {Owner}, IsBorrowed: {IsBorrowed}";
}
