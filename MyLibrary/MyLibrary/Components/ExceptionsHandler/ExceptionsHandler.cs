namespace MyLibrary.Components.ExceptionsHandler;

public class ExceptionsHandler : IExceptionsHandler
{
    public void InputInvalidValueException(string property, string action)
    {
        throw new Exception(message: $"\nPodane dane w '{property}' mają niewłaściwą wartość; {action}!");
    }
}
