namespace MyLibrary.Components.InputDataValidation;

public interface IInputDataValidation
{
    string InputIsNullOrEmpty(string? input, string inf);

    bool BoolValidation(string? input, string property);

    int IntInputValidation(string? input, string property);
}
