namespace MyLibrary.Components.ProjectCsvReader.VariousBooksCollections;

public class TopBook
{
    public int? Index { get; set; }

    public string? Title { get; set; }

    public string? AuthorName { get; set; }

    public string? AuthorSurname { get; set; }

    public override string ToString() => $"Lp. {Index} \nAuthor: {AuthorName} {AuthorSurname} \nTitle: {Title}";
}
