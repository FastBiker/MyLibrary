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

    public bool ValidateBoolInput(string? input, string property)
    {
        bool _isProperty = input == "+" ? true : input == "-" || string.IsNullOrEmpty(input) ? false : throw new 
            Exception($"Podane dane w '{property}' mają niewłaściwą wartość; " +
                "wpisz '+' jeśli jest wypożyczona, '-' jeśli nie jest, albo zostaw pole puste");

        return _isProperty;
    }

    public void ValidateFileName(string? inputFileName, string forbiddenCharacters)
    {
        foreach (char c in forbiddenCharacters)
        {
            if (IsValidFileName(inputFileName, c))
                throw new Exception($"Niewłaściwa nazwa pliku! \nNazwa pliku powinna mieć przynajmniej jeden znak, " +
                    $"wykluczając znaki: '{forbiddenCharacters}' oraz '.' na końcu nazwy!");
        }
    }

    private static bool IsValidFileName(string? inputFileName, char c)
    {
        return inputFileName.Contains(c) || inputFileName.EndsWith(".") || inputFileName.Length == 0;
    }

    public string HandleInputWhenEmptyOrNull(string? input, string inf)
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

    public int ValidateIntInput(string? input, string property)
    {
        int id = int.TryParse(input, out int result) && result > 0 
            ? result 
            : throw new Exception($"\nPodane dane w '{property}' mają niewłaściwą wartość; wpisz liczbę całkowitą większą od '0'!");
        
        return id;
    }

    public bool ValidateYearOfPublication(string input, string inf2)
    {
        if (int.TryParse(input, out int result) && result > 999 && result < 10000)
            return true;

        if (HandleInputWhenEmptyOrNull(input, inf2) == null)
            return false;

        _exceptionsHandler.InputInvalidValueException("rok wydania", "wpisz liczbę czterocyfrową dodatnią (rrrr)");
        return false;
    }

    public bool ValidatePagesNumber(string input, string inf2)
    {
        if (int.TryParse(input, out int result2) && result2 > 0)
            return true;

        if (HandleInputWhenEmptyOrNull(input, inf2) == null)
            return false;

        _exceptionsHandler.InputInvalidValueException("liczba stron", "wpisz liczbę całkowitą dodatnią");
        return false;
    }

    public bool ValidatePrice(string input, string inf2)
    {
        if (decimal.TryParse(input, out decimal result) && result > 0)
            return true;

        if (HandleInputWhenEmptyOrNull(input, inf2) == null)
            return false;

        _exceptionsHandler.InputInvalidValueException("cena książki", "wpisz dowolną liczbę większą od 0 (00,00)");
        return false;
    }

    public bool ValidateDateTime(string input, string inf2)
    {
        if (DateTime.TryParse(input, out DateTime result4))
            return true;

        if (HandleInputWhenEmptyOrNull(input, inf2) == null)
            return false;

        _exceptionsHandler.InputInvalidValueException("data (wy)pożyczenia", "podaj datę (wy)pożyczenia wg wzoru: dd.mm.rrrr");
        return false;
    }
}
