using System;
using System.Reflection.Metadata;
using System.Windows.Forms;
// <author>Maciej Fajlhauer</author>
// <date>2024/2025</date>
// <version>1.0</version>
// <summary>
// Entry point for the SHA256 Calculator application.
// This application provides SHA256 hash calculation functionality
// with support for multi-threading and different implementations (ASM/C#).
// </summary>
namespace SHA256_APP
{
    /// <summary>
    /// Main program class containing the application entry point.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Inicjalizacja aplikacji okienkowej
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
           
        }
    }
}
