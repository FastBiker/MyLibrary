namespace MyLibrary.Components.InputDataValidation;

public interface IInputDataValidation
{
    string InputIsNullOrEmpty(string? input, string inf);

    void BoolValidation(out string? input, out bool _isProperty, string property);
}
