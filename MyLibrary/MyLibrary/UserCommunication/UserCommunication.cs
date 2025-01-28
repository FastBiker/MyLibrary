using MyLibrary.DataProviders;
using MyLibrary.Entities;
using MyLibrary.Repositories;

namespace MyLibrary.UserCommunication;

public class UserCommunication : IUserCommunication
{
    public void Menu()
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("(1) wyświetl wszystkie książki; (2) dodaj nową książkę; (3) usuń książkę (q) opuść aplikację");
        Console.ResetColor();
    }

    public void Welcome()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Witamy w aplikacji 'MyLibrary', która pomoże Ci uporządkować Twój domowy zbiór książek");
        Console.WriteLine("======================================================================================");
        Console.ResetColor();
        Console.WriteLine();
    }
}
