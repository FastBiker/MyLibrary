namespace MyLibrary.Components.ExceptionsHandler;

public interface IExceptionsHandler
{
    void InputInvalidValueException(string property, string action);
}
