using System.ComponentModel;

namespace MyLibrary.Entities;

public class Book : EntityBase
{
    public string? AuthorName { get; set; }

    public string? AuthorSurname { get; set; }

    public string? Title { get; set; }

    public string? PublishingHouse { get; set; }

    public string? PlaceOfPublication { get; set; }

    public int? YearOfPublication { get; set; }

    public int? PageNumber { get; set; }

    public int? ISBN { get; set; }

    public string? PlaceInLibrary { get; set; }

    public string? Description { get; set; } 

    public string? OwnComments { get; set; } // np. (nie)przeczytana, (nie)polecana itp.

    public string? BookStatus { get; set; } // w bibliotece; wypożyczona; do sprzedaży

    public decimal? Price { get; set; }

    public override string ToString() => $"Id: {Id}, Author: {AuthorName} {AuthorSurname}, Title: {Title}";
}
