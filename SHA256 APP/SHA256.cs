using System;
using System.Runtime.InteropServices;
using System.Text;

namespace SHA256App
{
    internal class SHA256
    {
        // Tablica wartości początkowych H
        private static readonly uint[] H = new uint[] {
            0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a,
            0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19
        };

        // Stałe K
        private static readonly uint[] K = new uint[] {
            0x428a2f98, 0x71374491, 0xb5c0fbcf, 0xe9b5dba5,
            0x3956c25b, 0x59f111f1, 0x923f82a4, 0xab1c5ed5,
            0xd807aa98, 0x12835b01, 0x243185be, 0x550c7dc3,
            0x72be5d74, 0x80deb1fe, 0x9bdc06a7, 0xc19bf174,
            0xe49b69c1, 0xefbe4786, 0x0fc19dc6, 0x240ca1cc,
            0x2de92c6f, 0x4a7484aa, 0x5cb0a9dc, 0x76f988da,
            0x983e5152, 0xa831c66d, 0xb00327c8, 0xbf597fc7,
            0xc6e00bf3, 0xd5a79147, 0x06ca6351, 0x14292967,
            0x27b70a85, 0x2e1b2138, 0x4d2c6dfc, 0x53380d13,
            0x650a7354, 0x766a0abb, 0x81c2c92e, 0x92722c85,
            0xa2bfe8a1, 0xa81a664b, 0xc24b8b70, 0xc76c51a3,
            0xd192e819, 0xd6990624, 0xf40e3585, 0x106aa070,
            0x19a4c116, 0x1e376c08, 0x514edb6c, 0x1f7d8d20,
            0x28d12998, 0x32caab7b, 0x3c9ebe0a, 0x431d67c4,
            0x4cc5c3f8, 0x597f5b1b, 0x5fcb6b1f, 0x6c44198c,
            0x6f79df6e, 0x7a3e5f88, 0x7f847f7f, 0x839e2b59
        };

        // Deklaracje funkcji w asemblerze
        [DllImport("SHA256Asm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Sigma0Asm(uint x);

        [DllImport("SHA256Asm.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Sigma1Asm(uint x);

        public static string ComputeHash(string input, string libraryType = "cs")
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            byte[] paddedData = PadData(data);

            uint[] hashValues = (uint[])H.Clone(); // Kopiowanie początkowych wartości H

            for (int i = 0; i < paddedData.Length / 64; i++)
            {
                uint[] w = new uint[64];
                Buffer.BlockCopy(paddedData, i * 64, w, 0, 64);

                for (int j = 16; j < 64; j++)
                {
                    // Wybór implementacji Sigma0/Sigma1 na podstawie argumentu libraryType
                    w[j] = (libraryType == "asm")
                        ? Sigma1Asm(w[j - 2]) + w[j - 7] + Sigma0Asm(w[j - 15]) + w[j - 16]
                        : Sigma1(w[j - 2]) + w[j - 7] + Sigma0(w[j - 15]) + w[j - 16];
                }

                uint a = hashValues[0];
                uint b = hashValues[1];
                uint c = hashValues[2];
                uint d = hashValues[3];
                uint e = hashValues[4];
                uint f = hashValues[5];
                uint g = hashValues[6];
                uint h = hashValues[7];

                for (int j = 0; j < 64; j++)
                {
                    uint t1 = h + BigSigma1(e) + Ch(e, f, g) + K[j] + w[j];
                    uint t2 = BigSigma0(a) + Maj(a, b, c);
                    h = g;
                    g = f;
                    f = e;
                    e = d + t1;
                    d = c;
                    c = b;
                    b = a;
                    a = t1 + t2;
                }

                hashValues[0] += a;
                hashValues[1] += b;
                hashValues[2] += c;
                hashValues[3] += d;
                hashValues[4] += e;
                hashValues[5] += f;
                hashValues[6] += g;
                hashValues[7] += h;
            }

            StringBuilder sb = new StringBuilder();
            foreach (uint value in hashValues)
            {
                sb.Append(value.ToString("x8"));
            }

            return sb.ToString();
        }

        // Funkcja pomocnicza do paddingu danych
        private static byte[] PadData(byte[] data)
        {
            int paddingLength = 64 - ((data.Length + 9) % 64);
            byte[] paddedData = new byte[data.Length + paddingLength + 9];
            Array.Copy(data, paddedData, data.Length);
            paddedData[data.Length] = 0x80; // Dodanie 1 bitu

            ulong bitLength = (ulong)data.Length * 8;
            for (int i = 0; i < 8; i++)
            {
                paddedData[paddedData.Length - 1 - i] = (byte)(bitLength >> (i * 8));
            }

            return paddedData;
        }

        // Funkcja pomocnicza dla operacji ROTR
        private static uint ROTR(uint x, int n)
        {
            return (x >> n) | (x << (32 - n));
        }

        // Funkcja pomocnicza dla operacji Σ0
        private static uint Sigma0(uint x)
        {
            return ROTR(x, 7) ^ ROTR(x, 18) ^ (x >> 3);
        }

        // Funkcja pomocnicza dla operacji Σ1
        private static uint Sigma1(uint x)
        {
            return ROTR(x, 17) ^ ROTR(x, 19) ^ (x >> 10);
        }

        // Funkcja pomocnicza dla operacji Maj
        private static uint Maj(uint x, uint y, uint z)
        {
            return (x & y) ^ (x & z) ^ (y & z);
        }

        // Funkcja pomocnicza dla operacji Ch
        private static uint Ch(uint x, uint y, uint z)
        {
            return (x & y) ^ (~x & z);
        }

        // Funkcja pomocnicza dla operacji Σ
        private static uint BigSigma0(uint x)
        {
            return ROTR(x, 2) ^ ROTR(x, 13) ^ ROTR(x, 22);
        }

        // Funkcja pomocnicza dla operacji Σ1
        private static uint BigSigma1(uint x)
        {
            return ROTR(x, 6) ^ ROTR(x, 11) ^ ROTR(x, 25);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter text to hash: ");
            string input = Console.ReadLine();

            // Wybór biblioteki (C# vs ASM)
            Console.Write("Enter 'cs' for C# or 'asm' for ASM implementation: ");
            string libType = Console.ReadLine();

            string hash = SHA256.ComputeHash(input, libType);
            Console.WriteLine($"SHA-256 Hash: {hash}");
        }
    }
}
