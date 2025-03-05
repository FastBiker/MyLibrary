using System.Text;

namespace MyLibrary.Components.ProjectCsvReader.VariousBooksCollections;

public class RealBook
{
    public string? Lp { get; set; }

    public string? AuthorName { get; set; }

    public string? AuthorSurname { get; set; }

    public string? CollectiveAuthor { get; set; }

    public string Title { get; set; }

    public string? PlaceInLibrary { get; set; }

    public bool? IsForSale { get; set; }

    public decimal? Price { get; set; }

    public string? Comments { get; set; }

    #region ToString Override
    public override string ToString()
    {
        StringBuilder sb = new(1024);

        sb.AppendLine($"Lp. {Lp}");
        if (AuthorName != null && AuthorSurname != null)
        {
            sb.AppendLine($"Author: {AuthorName} {AuthorSurname}");
        }
        if (CollectiveAuthor != null)
        {
            sb.AppendLine($"CollectiveAuthor: {CollectiveAuthor}");
        }

        sb.AppendLine($"\tTitle: {Title}");

        
        if (PlaceInLibrary != null)
        {
            sb.AppendLine($"\tPlaceInLibrary: {PlaceInLibrary}");
        }
        if (IsForSale == true)
        {
            sb.AppendLine($"\tIsForSale: {IsForSale}");
            sb.AppendLine($"\tPrice: {Price:c}");
        }
        sb.AppendLine($"===============================================");

        return sb.ToString();
    }
    #endregion
}
