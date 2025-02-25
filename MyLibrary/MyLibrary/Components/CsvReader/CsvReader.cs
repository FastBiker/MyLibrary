using MyLibrary.Components.CsvReader.Extensions;
using MyLibrary.Components.CsvReader.VariousBooksCollections;
using System.Globalization;

namespace MyLibrary.Components.CsvReader;

public class CsvReader : ICsvReader
{
    public List<RealBook> ProcessRealBooks(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Nie znaleziono pliku MyHome!");
            return new List<RealBook>();
        }

        var books =
            File.ReadAllLines(filePath)
            .Skip(1)
            .Where(x => x.Length > 1)
            .ToBook();

        return books.ToList();
    }

    public List<TopBook> ProcessTopBooks(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Nie znaleziono pliku książkaTop!");
            return new List<TopBook>();
        }

        var books =
            File.ReadAllLines(filePath)
            .Where(x => x.Length > 1)
            .Select(x =>
            {
                var columns = x.Split(';', StringSplitOptions.None);

                for (int i = 0; i < 4; i++)
                {
                    if (string.IsNullOrEmpty(columns[i]))
                    {
                        columns[i] = "0";
                    }
                }

                return new TopBook()
                {
                    Lp = int.Parse(columns[0], CultureInfo.InvariantCulture),
                    Title = columns[1],
                    AuthorName = columns[2],
                    AuthorSurname = columns[3]
                };
            });
        return books.ToList();
    }
}

//using System;
//using System.IO;

//class Program
//{
//    static void Main()
//    {
//        string path = "sciezka/do/pliku.csv";

//        using (var reader = new StreamReader(path))
//        {
//            while (!reader.EndOfStream)
//            {
//                var line = reader.ReadLine();                  // Odczytaj linię z pliku
//                var values = line.Split(',', StringSplitOptions.None); // Rozdziel linię na kolumny

//                foreach (var value in values)
//                {
//                    if (string.IsNullOrEmpty(value))           // Sprawdź, czy pole jest puste
//                    {
//                        Console.Write("Puste; ");              // Obsłuż pustą komórkę
//                    }
//                    else
//                    {
//                        Console.Write(value + "; ");           // Wyświetl zawartość
//                    }
//                }
//                Console.WriteLine();
//            }
//        }
//    }
//}

//po użyciu metody Encoding.UTF8 w programie zaczęły się podkreślenia wężykiem na niebiesko - jak wycofać tę metodę, bo po skasowaniu Encoding podkreślenią są nadal

//public List<RealBook> ProcessRealBooks(string filePath)
//{
//    if (!File.Exists(filePath))
//    {
//        Console.WriteLine("Nie znaleziono pliku MyHome!");
//        return new List<RealBook>();
//    }

//    var books =
//        File.ReadAllLines(filePath)
//        .Skip(1)
//        .Where(x => x.Length > 1)
//        .ToBook();

//    return books.ToList();
//}