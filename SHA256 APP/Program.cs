using System;
using System.Reflection.Metadata;
using System.Windows.Forms;

namespace SHA256_APP
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Inicjalizacja aplikacji okienkowej
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
            /*string content = "Hello World!";
            string selectedLibrary = "asm";
            string hash = SHA256App.SHA256.ComputeHash(content, selectedLibrary);

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = "test";
            string extension = ".txt";
            string outputFilePath = Path.Combine(desktopPath, $"{fileName}X{extension}");
            File.WriteAllText(outputFilePath, hash);*/
        }
    }
}
