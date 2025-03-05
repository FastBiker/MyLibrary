namespace MyLibrary.Components.ProjectCsvReader.VariousBooksCollections;

public class DataCleanBook
{
    [CsvHelper.Configuration.Attributes.Name("index")]
    public uint Index { get; set; }

    [CsvHelper.Configuration.Attributes.Name("Publishing Year")]
    public float? PublishingYear { get; set; } // rok wydania

    [CsvHelper.Configuration.Attributes.Name("Book Name")]
    public string? BookName { get; set; } //tytuł

    [CsvHelper.Configuration.Attributes.Name("Author")]
    public string? Author { get; set; } 

    [CsvHelper.Configuration.Attributes.Name("language_code")]
    public string? LanguageCode { get; set; } //język, w którym jest napisana wydana książka

    [CsvHelper.Configuration.Attributes.Name("Author_Rating")]
    public string? AuthorRating { get; set; } //autor oceny

    [CsvHelper.Configuration.Attributes.Name("Book_average_rating")]
    public float? BookAverageRating { get; set; } //przeciętna ocena książki

    [CsvHelper.Configuration.Attributes.Name("Book_ratings_count")]
    public float? BookRatingsCount { get; set; } //liczba ocen książki

    [CsvHelper.Configuration.Attributes.Name("genre")]
    public string? Genre { get; set; } //gatunek 

    [CsvHelper.Configuration.Attributes.Name("gross sales")]
    public decimal? GrossSales { get; set; } //sprzedaż brutto

    [CsvHelper.Configuration.Attributes.Name("publisher revenue")]
    public decimal? PublisherRevenue { get; set; } //przychód wydawcy

    [CsvHelper.Configuration.Attributes.Name("sale price")]
    public decimal? SalePrice { get; set; } //cena sprzedaży

    [CsvHelper.Configuration.Attributes.Name("sales rank")]
    public uint? SalesRank { get; set; } //ranking sprzedaży

    [CsvHelper.Configuration.Attributes.Name("Publisher ")]
    public string? Publisher { get; set; } // wydawca

    [CsvHelper.Configuration.Attributes.Name("units sold")]
    public uint? UnitsSold { get; set; } // sprzedane jednostki/ilość sprzedanych książek

    public override string ToString() =>
        $"Index: {Index}" +
        $"\nTitle: {BookName}, Author: {Author} ({LanguageCode}) - Genre: {Genre}" +
        $"\n\tPublisher: {Publisher}, PublishingYear: {PublishingYear}" +
        $"\n\tAuthorRating: {AuthorRating}, BookAverageRating: {BookAverageRating}, BookRatingsCount: {BookRatingsCount}" +
        $"\n\tGrossSales: {GrossSales}, PublisherRevenue: {PublisherRevenue}, SalePrice: {SalePrice}" +
        $"\n\tSalesRank: {SalesRank}, UnitsSold: {UnitsSold}";








}

//Publishing Year,Book Name,Author,language_code,Author_Rating,Book_average_rating,Book_ratings_count,genre,gross sales,publisher revenue,
//sale price,sales rank,Publisher ,units sold
