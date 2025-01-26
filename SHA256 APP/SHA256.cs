using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SHA256App
{
    internal class SHA256
    {
        // Tablica wartości początkowych H
        private static readonly UInt32[] H = new UInt32[8] {
            0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a,
            0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19
        };

        // Stałe K
        private static readonly UInt32[] K = new UInt32[64] {
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
        [DllImport(@"C:\Users\macie\source\repos\SHA256 APP\x64\Debug\JAAsm.dll")]
        public static extern void Sigma0Asm(ref UInt32 x);

        [DllImport(@"C:\Users\macie\source\repos\SHA256 APP\x64\Debug\JAAsm.dll")]
        public static extern void Sigma1Asm(ref UInt32 x);

        [DllImport(@"C:\Users\macie\source\repos\SHA256 APP\x64\Debug\JAAsm.dll")]
        public static extern UInt32 BigSigma0Asm(UInt32 x);

        [DllImport(@"C:\Users\macie\source\repos\SHA256 APP\x64\Debug\JAAsm.dll")]
        public static extern UInt32 BigSigma1Asm(UInt32 x);

        [DllImport(@"C:\Users\macie\source\repos\SHA256 APP\x64\Debug\JAAsm.dll")]
        public static extern UInt32 ChAsm(UInt32 x, UInt32 y, UInt32 z);

        [DllImport(@"C:\Users\macie\source\repos\SHA256 APP\x64\Debug\JAAsm.dll")]
        public static extern UInt32 MajAsm(UInt32 x, UInt32 y, UInt32 z);

        [DllImport(@"C:\Users\macie\source\repos\SHA256 APP\x64\Debug\JAAsm.dll")]
        public static extern UInt32 ROTRAsm(UInt32 x, byte n);

        public static string ComputeHash(string input, string libraryType = "cs")
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            byte[] paddedData = PadData(data);

            UInt32[] hashValues = (UInt32[])H.Clone(); // Kopiowanie początkowych wartości H

            for (int i = 0; i < paddedData.Length / 64; i++)
            {
                UInt32[] w = new UInt32[64];
                Buffer.BlockCopy(paddedData, i * 64, w, 0, 64);

                for (int j = 16; j < 64; j++)
                {
                    if (libraryType == "asm")
                    {
                        //w[j] = Sigma1Asm(w[j - 2]) + w[j - 7] + Sigma0Asm(w[j - 15]) + w[j - 16];
                        Sigma1Asm(ref w[j - 2]);
                        Sigma0Asm(ref w[j - 15]);
                        
                        try
                        {
                            w[j] = w[j - 2] + w[j - 7] + w[j - 15] + w[j - 16];
                        }
                        catch (SEHException ex)
                        {
                            Debug.WriteLine($"SEHException: {ex.Message}");
                        }

                        Console.WriteLine($"ASM: w[{j}] = {w[j]}");
                        Debug.WriteLine($"ASM: w[{j}] = {w[j]}");
                    }
                    else
                    {
                        w[j] = Sigma1(w[j - 2]) + w[j - 7] + Sigma0(w[j - 15]) + w[j - 16];
                        Console.WriteLine($"CS: w[{j}] = {w[j]}");
                        Debug.WriteLine($"CS: w[{j}] = {w[j]}");

                    }

                }

                UInt32 a = hashValues[0];
                UInt32 b = hashValues[1];
                UInt32 c = hashValues[2];
                UInt32 d = hashValues[3];
                UInt32 e = hashValues[4];
                UInt32 f = hashValues[5];
                UInt32 g = hashValues[6];
                UInt32 h = hashValues[7];

                for (int j = 0; j < 64; j++)
                {
                    UInt32 t1, t2;

                    if (libraryType == "asm")
                    {
                        t1 = h + BigSigma1(e) + Ch(e, f, g) + K[j] + w[j];
                        t2 = BigSigma0(a) + Maj(a, b, c);
                        Console.WriteLine($"ASM: t1 = {t1}, t2 = {t2}");
                        Debug.WriteLine($"ASM: t1 = {t1}, t2 = {t2}");

                    }
                    else
                    {
                        t1 = h + BigSigma1(e) + Ch(e, f, g) + K[j] + w[j];
                        t2 = BigSigma0(a) + Maj(a, b, c);
                        Console.WriteLine($"CS: t1 = {t1}, t2 = {t2}");
                        Debug.WriteLine($"CS: t1 = {t1}, t2 = {t2}");
                    }
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

            /*StringBuilder sb = new StringBuilder();
            foreach (UInt32 value in hashValues)
            {
                Debug.WriteLine(value);
               
                sb.Append(value.ToString("x8"));
            }
            Debug.WriteLine ("String builder:"+sb.ToString());
            return sb.ToString();*/
            string hash = string.Concat(hashValues.Select(value => value.ToString("x8")));
            Debug.WriteLine("Hash: " + hash);
            return hash;
        }

        private static byte[] PadData(byte[] data)
        {
            int paddingLength = 64 - ((data.Length + 9) % 64);
            byte[] paddedData = new byte[data.Length + paddingLength + 9];
            Array.Copy(data, paddedData, data.Length);
            paddedData[data.Length] = 0x80; // Dodanie 1 bitu

            ulong bitLength = (ulong)data.Length * 8;
            BitConverter.GetBytes(bitLength)
                .Reverse() // Odwracamy bajty dla big-endian
                .ToArray()
                .CopyTo(paddedData, paddedData.Length - 8);

            return paddedData;
        }


        // Funkcja pomocnicza dla operacji ROTR
        private static UInt32 ROTR(UInt32 x, byte n){return (x >> n) | (x << (32 - n));}

        // Funkcja pomocnicza dla operacji Σ0
        private static UInt32 Sigma0(UInt32 x){return ROTR(x, 7) ^ ROTR(x, 18) ^ (x >> 3);}

        // Funkcja pomocnicza dla operacji Σ1
        private static UInt32 Sigma1(UInt32 x){return ROTR(x, 17) ^ ROTR(x, 19) ^ (x >> 10);}

        // Funkcja pomocnicza dla operacji Maj
        private static UInt32 Maj(UInt32 x, UInt32 y, UInt32 z){return (x & y) ^ (x & z) ^ (y & z);}

        // Funkcja pomocnicza dla operacji Ch
        private static UInt32 Ch(UInt32 x, UInt32 y, UInt32 z){return (x & y) ^ (~x & z);}

        // Funkcja pomocnicza dla operacji Σ
        private static UInt32 BigSigma0(UInt32 x){return ROTR(x, 2) ^ ROTR(x, 13) ^ ROTR(x, 22);}

        // Funkcja pomocnicza dla operacji Σ1
        private static UInt32 BigSigma1(UInt32 x){return ROTR(x, 6) ^ ROTR(x, 11) ^ ROTR(x, 25);}
    }


}
