using MyLibrary.UserCommunication;

namespace MyLibrary.Components.InputDataValidation;

public class InputDataValidation : IInputDataValidation
{
    private readonly IUserCommunication _userCommunication;
    public InputDataValidation(IUserCommunication userCommunication)
    {
        _userCommunication = userCommunication;
    }
    public void BoolValidation(out string? input, out bool _isProperty, string property)
    {
        input = _userCommunication.WriteBoolPropertyValue(property);
        if (input == "+")
        {
            input = "true";
        }
        else if (input == "-" || string.IsNullOrEmpty(input))
        {
            input = "false";
        }
        else
        {
            throw new Exception($"Podane dane w '{property}' mają niewłaściwą wartość;" +
                "wpisz '+' jeśli jest wypożyczona, '-' jeśli nie jest, albo zostaw pole puste");
        }
        _isProperty = bool.Parse(input);
    }

    public string InputIsNullOrEmpty(string? input, string inf)
    {
        switch (inf)
        {
            case "Informacja obowiązkowa; dane muszą być wprowadzone":
                while (string.IsNullOrEmpty(input))
                {
                    input = _userCommunication.EnteringMandatoryData(inf);
                }
                break;
            case "Podana wartość jest null / informacja opcjonalana":
                if (string.IsNullOrEmpty(input))
                {
                    input = null;
                    _userCommunication.MessageOptionalData(inf);
                }
                break;
        }

        return input;
    }
}
