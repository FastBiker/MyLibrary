using System.Text;

namespace MyLibrary.Entities;

public class Book : EntityBase
{
    public string? AuthorName { get; set; }

    public string? AuthorSurname { get; set; }

    public string? CollectiveAuthor { get; set; }

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

    #region ToString Override
    public override string ToString()
    {
        StringBuilder sb = new(1024);

        sb.AppendLine($"Id: {Id}");
        if ( AuthorName != null && AuthorSurname != null)
        {
            sb.AppendLine($"Author: {AuthorName} {AuthorSurname}");
        }
        if (CollectiveAuthor != null)
        {
            sb.AppendLine($"CollectiveAuthor: {CollectiveAuthor}");
        }

        sb.AppendLine($"\tTitle: {Title}");

        if (PublishingHouse != null)
        {
            sb.AppendLine($"\tPublishingHouse: {PublishingHouse}");
        }
        if (PlaceOfPublication != null)
        {
            sb.AppendLine($"\tPlaceOfPublication: {PlaceOfPublication}");
        }
        if (YearOfPublication.HasValue)
        {
            sb.AppendLine($"\tYearOfPublication: {YearOfPublication}");
        }
        if (PageNumber.HasValue)
        {
            sb.AppendLine($"\tPageNumber: {PageNumber}");
        }
        if (ISBN != null)
        {
            sb.AppendLine($"\tISBN: {ISBN}");
        }
        if (PlaceInLibrary != null)
        {
            sb.AppendLine($"\tPlaceInLibrary: {PlaceInLibrary}");
        }
        if (Owner != null)
        {
            sb.AppendLine($"\tOwner: {Owner}");
        }
        if (IsForSale == true)
        {
            sb.AppendLine($"\tIsForSale: {IsForSale}");
            sb.AppendLine($"\tPrice: {Price:c}");
        }
        if (IsLent == true)
        {
            sb.AppendLine($"\tIsLent: {IsLent}");
        }
        if (IsBorrowed == true)
        {
            sb.AppendLine($"\tIsBorrowed: {IsBorrowed}");
        }
        if (DateOfBorrowed.HasValue)
        {
            sb.AppendLine($"\tDateOfBorrowed: {DateOfBorrowed}");
        }
        sb.AppendLine($"===============================================");

        return sb.ToString();
    }

        
        
        //$"Id: {Id}," +
        //$"\n Author: {AuthorName} {AuthorSurname}, " +
        //$"\n Title: {Title}," +
        //$"\n \tPublishingHouse: {PublishingHouse}," +
        //$"\n \tPlaceOfPublication: {PlaceOfPublication}," +
        //$"\n \tYearOfPublication: {YearOfPublication}, " +
        //$"\n \tPageNumber: {PageNumber}," +
        //$"\n \tISBN: {ISBN}," +
        //$"\n \tPlaceInLibrary: {PlaceInLibrary}," +
        //$"\n \tOwner: {Owner}, " +
        //$"\n \tIsForSale: {IsForSale}" +
        //$"\n \tPrice: {Price}," +
        //$"\n \tIsLent: {IsLent}, " +
        //$"\n \tIsBorrowed: {IsBorrowed}, " +
        //$"\n \tDateOfBorrowed: {DateOfBorrowed} " +
        //$"\n==============================================="
        //+ Environment.NewLine;
    #endregion
}
