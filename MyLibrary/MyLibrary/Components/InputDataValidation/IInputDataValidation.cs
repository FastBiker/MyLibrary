namespace MyLibrary.Components.InputDataValidation;

public interface IInputDataValidation
{
    void ValidateFileName(string? inputFileName, string forbiddenCharacters);

    string HandleInputWhenEmptyOrNull(string? input, string inf);

    bool ValidateBoolInput(string? input, string property);

    int ValidateIntInput(string? input, string property);

    bool ValidateYearOfPublication(string input, string inf2);

    bool ValidatePagesNumber(string input, string inf2);

    bool ValidatePrice(string input, string inf2);

    bool ValidateDateTime(string input, string inf2);
}
