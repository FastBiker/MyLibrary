namespace MyLibrary.Components.InputDataValidation;

public interface IInputDataValidation
{
    void FileNameValidation(string? inputFileName, string forbiddenCharacters);

    string InputIsNullOrEmpty(string? input, string inf);

    bool BoolValidation(string? input, string property);

    int IntInputValidation(string? input, string property);

    bool ValidatePrice(string input, string inf2);

    bool ValidateDateTime(string input, string inf2);
}
