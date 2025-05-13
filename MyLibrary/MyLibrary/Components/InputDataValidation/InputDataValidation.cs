using MyLibrary.UserCommunication;
using MyLibrary.Components.ExceptionsHandler;

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
        bool _isProperty = input == "+" ? true : input == "-" || string.IsNullOrEmpty(input) ? false : throw new 
            Exception($"Podane dane w '{property}' mają niewłaściwą wartość; " +
                "wpisz '+' jeśli jest wypożyczona, '-' jeśli nie jest, albo zostaw pole puste");

        return _isProperty;
    }

    public void FileNameValidation(string? inputFileName, string forbiddenCharacters)
    {
        foreach (char c in forbiddenCharacters)
        {
            if (inputFileName.Contains(c) || inputFileName.EndsWith(".") || inputFileName.Length == 0)
                throw new Exception($"Niewłaściwa nazwa pliku! \nNazwa pliku powinna mieć przynajmniej jeden znak, " +
                    $"wykluczając znaki: '{forbiddenCharacters}' oraz '.' na końcu nazwy!");
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

                input = string.IsNullOrEmpty(input) ? null : input;
                if (input == null) _userCommunication.MessageOptionalData(inf);

                break;
        }

        return input;
    }

    public int IntInputValidation(string? input, string property)
    {
        int id = int.TryParse(input, out int result) && result > 0 
            ? result 
            : throw new Exception($"\nPodane dane w '{property}' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'!");
        
        return id;
    }

    public bool ValidatePrice(string input, string inf2)
    {
        if (decimal.TryParse(input, out decimal result) && result > 0)
        {
            return true;
        }
        if (InputIsNullOrEmpty(input, inf2) == null)
        {
            return false;
        }
        _exceptionsHandler.InputInvalidValueException("cena książki", "wpisz dowolną liczbę większą od 0 (00,00)");
        return false;
    }

    public bool ValidateDateTime(string input, string inf2)
    {
        if (DateTime.TryParse(input, out DateTime result4))
        {
            return true;
        }
        if (InputIsNullOrEmpty(input, inf2) == null)
        {
            return false;
        }
        _exceptionsHandler.InputInvalidValueException("data (wy)pożyczenia", "podaj datę (wy)pożyczenia wg wzoru: rrrr,mm,dd");
        return false;
    }
}
