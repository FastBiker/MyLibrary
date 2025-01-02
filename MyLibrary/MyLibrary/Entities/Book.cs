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

    public string? ISBN { get; set; }

    public string? PlaceInLibrary { get; set; }

    public string? Owner { get; set; }

    public bool? IsForSale { get; set; }

    public decimal? Price { get; set; }

    public bool IsLent { get; set; }

    public bool IsBorrowed { get; set; }

    public DateTime? DateOfBorrowed { get; set; }

    public override string ToString()
        => $"Id: {Id}," +
        $"\n Author: {AuthorName} {AuthorSurname}, " +
        $"\n Title: {Title}," +
        $"\n \tPublishingHouse: {PublishingHouse}," +
        $"\n \tPlaceOfPublication: {PlaceOfPublication}," +
        $"\n \tYearOfPublication: {YearOfPublication}, " +
        $"\n \tPageNumber: {PageNumber}," +
        $"\n \tISBN: {ISBN}," +
        $"\n \tPlaceInLibrary: {PlaceInLibrary}," +
        $"\n \tOwner: {Owner}, " +
        $"\n \tIsForSale: {IsForSale}" +
        $"\n \tPrice: {Price}," +
        $"\n \tIsLent: {IsLent}, " +
        $"\n \tIsBorrowed: {IsBorrowed}, " +
        $"\n \tDateOfBorrowed: {DateOfBorrowed} " +
        $"\n==============================================="
        + Environment.NewLine;
}
