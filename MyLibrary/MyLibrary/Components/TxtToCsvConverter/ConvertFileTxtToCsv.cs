namespace MyLibrary.Components.TxtToCsvConverter;

public class ConvertFileTxtToCsv : IConvertFileTxtToCsv
{
    public ConvertFileTxtToCsv()
    {
    }

    public void ConvertWithReadAllLines()
    {
        string inputFileTxtPath = "C:\\Projekty\\MyLibrary\\MyLibrary\\MyLibrary\\Resources\\Files\\książkiTop100.txt";
        string outputFileCsv = "C:\\Projekty\\MyLibrary\\MyLibrary\\MyLibrary\\Resources\\Files\\książkiTop100.csv";

        var lines = File.ReadAllLines(inputFileTxtPath);

        using (StreamWriter writer = new StreamWriter(outputFileCsv))
        {
            foreach (var line in lines)
            {
                var csvLine = line.Trim().Replace(" – ", ";").Replace("- ", ";");
                writer.WriteLine(csvLine);
            }
        }
        Console.WriteLine("Konwersja zakończona!");
    }

    public void ReplaceString()
    {
        string inputFileCsvPath = "C:\\Projekty\\MyLibrary\\MyLibrary\\MyLibrary\\Resources\\Files\\My_Home_Library.csv";
        string outputFileCsv = "C:\\Projekty\\MyLibrary\\MyLibrary\\MyLibrary\\Resources\\Files\\My_Home_Library.csv" +
            "";

        var lines = File.ReadAllLines(inputFileCsvPath);

        using (StreamWriter writer = new StreamWriter(outputFileCsv))
        {
            foreach (var line in lines)
            {
                var csvLine = line.Replace("Tak", "true");
                writer.WriteLine(csvLine);
            }
        }
        Console.WriteLine("Zmiana 'stringa' zakończona!");
    }
}