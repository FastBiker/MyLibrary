using MyLibrary.Data.Entities;

namespace MyLibrary.UserCommunication;

public interface IUserCommunication
{
    void Welcome();

    string? MainMenu();

    void MainMethodsHeaders(string header);

    void ExceptionCatchedMainMethods(Exception e);

    void ExceptionWrongMenuInput();

    void WriteItemToConsole(IEnumerable<IEntity> items);

    void SelectAuthor(out string inf1, out string inf2, out string? input);

    string? WriteBookProperties(string property);

    string? SaveBook();

    string? WriteRemovedBookTitle();

    void WriteCopyBookToConsole(Book? copyBook);

    string? EnteringMandatoryData(string inf);

    void MessageOptionalData(string inf);

    void WriteAuditInfoToConsoleUsingCallback(Book item, string action);

    void WriteAuditInfoToConsoleUsingEventHandler(object? sender, Book e, string auditInfo);

    string? WriteBoolPropertyValue(string property);

    string? BooksFilterMenu();

    void WriteMinimumPriceOffAllBooksToConsole(decimal cost);

    void FilterHeader(string filterHeader);

    void WriteFilterPropertyToConsole(string item);

    void WriteFilterBooksToConsole(Book item);

    string? InputFilterData(string inputHeader);

    void InputRange(out string? input1, out string? input2);

    void WriteChunkToConsole(Book[] chunkBooks);
}
