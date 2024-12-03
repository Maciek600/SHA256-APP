using System;
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
        }
    }
}
