using System.ComponentModel;

namespace MyLibrary.Entities;

public class BorrowedBook : EntityBase
{
    public string? AuthorName { get; set; }

    public string? AuthorSurname { get; set; }

    public string? Title { get; set; }

    public string? Owner { get; set; }

    public DateTime? DateOfBorrowed {  get; set; }

    public override string ToString() => $"Id: {Id}, Title: {Title} (Borrowed), Owner: {Owner}, Date of borrowed: {DateOfBorrowed}";
}
