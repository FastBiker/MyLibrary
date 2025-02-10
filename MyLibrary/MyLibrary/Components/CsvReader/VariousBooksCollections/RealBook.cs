namespace MyLibrary.Components.CsvReader.VariousBooksCollections;

public class RealBook
{
    public int Lp { get; set; }

    public string? AuthorName { get; set; }

    public string? AuthorSurname { get; set; }

    public string? CollectiveAuthor { get; set; }

    public string Title { get; set; }

    public string? PlaceInLibrary { get; set; }

    public bool? IsForSale { get; set; }

    public decimal? Price { get; set; }

    public string? Comments { get; set; }
}
