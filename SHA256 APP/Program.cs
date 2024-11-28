using System;
using System.Windows.Forms;

namespace SHA256_APP
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Inicjalizacja aplikacji (formy)
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());

            // Po zamkni�ciu formy, przetwarzanie plik�w
            Console.WriteLine("Wybierz implementacj�: ASM lub C#");
            string library = Console.ReadLine()?.ToUpper() == "ASM" ? "ASM" : "C#";

            // Uruchomienie logiki przetwarzania plik�w
            MainProgram.ProcessFiles(library);
        }
    }
}
