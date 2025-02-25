namespace MyLibrary.Components.CsvReader.VariousBooksCollections;

public class TopBook
{
    public int? Lp { get; set; }

    public string? Title { get; set; }

    public string? AuthorName { get; set; }

    public string? AuthorSurname { get; set; }

    public override string ToString() => $"Lp. {Lp} \nAuthor: {AuthorName} {AuthorSurname} \nTitle: {Title}";
}
