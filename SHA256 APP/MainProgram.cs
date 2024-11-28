using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHA256_APP
{
    internal class MainProgram
    {
        public static void ProcessFiles(string library)
        {
            string[] inputFilePaths = Directory.GetFiles("input"); // Katalog wejściowy

            Parallel.ForEach(inputFilePaths, filePath =>
            {
                byte[] fileData = File.ReadAllBytes(filePath); // Odczyt pliku wejściowego
                uint[] hashState = InitializeHashState(); // Inicjalizacja stanu haszowania

                uint[] block = PrepareBlock(fileData); // Przygotowanie bloku (padding)

                SHA256_Main.ProcessBlock(block, library, hashState);

                // Zapis wyniku
                string outputFilePath = $"{filePath}_X";
                File.WriteAllBytes(outputFilePath, ConvertHashToBytes(hashState));
            });

            Console.WriteLine("Przetwarzanie zakończone.");
        }

        private static uint[] InitializeHashState()
        {
            return new uint[]
            {
                0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a,
                0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19
            };
        }

        private static uint[] PrepareBlock(byte[] data)
        {
            // Logika przygotowania bloku (padding itp.)
            return new uint[16]; // Przykład: tylko 1 blok
        }

        private static byte[] ConvertHashToBytes(uint[] hashState)
        {
            byte[] result = new byte[hashState.Length * 4];
            for (int i = 0; i < hashState.Length; i++)
            {
                Array.Copy(BitConverter.GetBytes(hashState[i]), 0, result, i * 4, 4);
            }
            return result;
        }
    }

}
