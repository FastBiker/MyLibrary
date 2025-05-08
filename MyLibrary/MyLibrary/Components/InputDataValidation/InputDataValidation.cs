using MyLibrary.Components.ExceptionsHandler;
using MyLibrary.UserCommunication;

namespace MyLibrary.Components.InputDataValidation;

public class InputDataValidation : IInputDataValidation
{
    private readonly IUserCommunication _userCommunication;
    private readonly IExceptionsHandler _exceptionsHandler;
    public InputDataValidation(IUserCommunication userCommunication, IExceptionsHandler exceptionsHandler)
    {
        _userCommunication = userCommunication;
        _exceptionsHandler = exceptionsHandler;
    }

    public bool BoolValidation(string? input, string property)
    {
        bool _isProperty;
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
            _exceptionsHandler.InputInvalidValueException(property, "wpisz '+' jeśli jest (wy)pożyczona/na sprzedaż, '-' jeśli nie jest, albo zostaw pole puste");
            //throw new Exception($"Podane dane w '{property}' mają niewłaściwą wartość; " +
            //    "wpisz '+' jeśli jest wypożyczona, '-' jeśli nie jest, albo zostaw pole puste");
        }
        
        return _isProperty = bool.Parse(input);
    }

    public void FileNameValidation(string? inputFileName, string forbiddenCharacters)
    {
        //string forbiddenCharacters = ":*?\"<>/|\\";
        foreach (char c in forbiddenCharacters)
        {
            if (inputFileName.Contains(c) || inputFileName.EndsWith(".") || inputFileName.Length == 0)
            {
                throw new Exception($"Niewłaściwa nazwa pliku! \nNazwa pliku powinna mieć przynajmniej jeden znak, " +
                    $"wykluczając znaki: '{forbiddenCharacters}' oraz '.' na końcu nazwy!");
            }
        }
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

    public int IntInputValidation(string? input, string property)
    {
        int id;
        if (int.TryParse(input, out int result) && result > 0)
        {
            id = result;
        }
        else
        {
            _exceptionsHandler.InputInvalidValueException(property, "wpisz liczbę całkowitą większą od '0'!");
            id = -1;
            //throw new Exception($"\nPodane dane w '{property}' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'!");
        }
        
        return id;
    }
}
