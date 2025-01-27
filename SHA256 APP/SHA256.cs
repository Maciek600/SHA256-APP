using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;
using Sha;

namespace SHA256App
{
    public class SHA256
    {
        Class1 sha = new Class1();
        // Initial hash values H and constants K remain the same
        private static readonly UInt32[] H = new UInt32[8] {
            0x6a09e667, 0xbb67ae85, 0x3c6ef372, 0xa54ff53a,
            0x510e527f, 0x9b05688c, 0x1f83d9ab, 0x5be0cd19
        };

        // K constants remain the same
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
            0x19a4c116, 0x1e376c08, 0x2748774c, 0x34b0bcb5,
            0x391c0cb3, 0x4ed8aa4a, 0x5b9cca4f, 0x682e6ff3,
            0x748f82ee, 0x78a5636f, 0x84c87814, 0x8cc70208,
            0x90befffa, 0xa4506ceb, 0xbef9a3f7, 0xc67178f2
        };

        // Assembly function imports remain the same...
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
        public static extern void ChAsm(ref UInt32 x, ref UInt32 y, ref UInt32 z);

        [DllImport(@"C:\Users\macie\source\repos\SHA256 APP\x64\Debug\JAAsm.dll")]
        public static extern UInt32 MajAsm( UInt32 x, UInt32 y,  UInt32 z);

        [DllImport(@"C:\Users\macie\source\repos\SHA256 APP\x64\Debug\JAAsm.dll")]
        public static extern UInt32 ROTRAsm(UInt32 x, byte n);


        public string ComputeHash(string input, string libraryType = "cs")
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            byte[] paddedData = PadData(data);

            UInt32[] hashValues = (UInt32[])H.Clone();

            for (int chunk = 0; chunk < paddedData.Length / 64; chunk++)
            {
                UInt32[] w = new UInt32[64];

                // Convert chunk to message schedule array
                for (int i = 0; i < 16; i++)
                {
                    w[i] = (UInt32)((paddedData[chunk * 64 + i * 4] << 24) |
                                   (paddedData[chunk * 64 + i * 4 + 1] << 16) |
                                   (paddedData[chunk * 64 + i * 4 + 2] << 8) |
                                   (paddedData[chunk * 64 + i * 4 + 3]));
                }

                // Extend the first 16 words into the remaining 48 words
                for (int i = 16; i < 64; i++)
                {
                    if (libraryType == "asm")
                    {
                        UInt32 w15 = w[i - 15];
                        UInt32 w2 = w[i - 2];
                        Sigma0Asm(ref w15);
                        Sigma1Asm(ref w2);
                        w[i] = w[i - 16] + w15 + w[i - 7] + w2;
                    }
                    else
                    {
                        UInt32 s0 = sha.Sigma0(w[i - 15]);
                        UInt32 s1 = sha.Sigma1(w[i - 2]);
                        w[i] = w[i - 16] + s0 + w[i - 7] + s1;
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

                // Compression loop
                for (int i = 0; i < 64; i++)
                {
                    UInt32 S1 = (libraryType == "asm") ? BigSigma1Asm(e) : sha.BigSigma1(e);
                    UInt32 ch;
                    if(libraryType == "asm")
                    {
                        UInt32 temp = e;
                        ch = temp;
                        ChAsm(ref ch, ref f, ref g);
                     
                    }
                    else
                    {
                        ch = sha.Ch(e, f, g);
                    }
                   // UInt32 ch = (libraryType == "asm") ? ChAsm(ref e, ref f, ref g) : sha.Ch(e, f, g);
                    UInt32 temp1 = h + S1 + ch + K[i] + w[i];
                    /*UInt32 S0;
                    if(libraryType == "asm") {
                        UInt32 aa = a;
                        BigSigma0Asm(ref aa);
                        S0 = aa;
                    } else
                    {
                        S0 = sha.BigSigma0(a);
                    }*/
                    UInt32 S0 = (libraryType == "asm") ? BigSigma0Asm(a) : sha.BigSigma0(a);
                    //uint maj = (libraryType == "asm") ? MajAsm(a, b, c) : sha.Maj(a, b, c);
                    UInt32 maj;
                    if(libraryType == "asm")
                    {
                        /*UInt32 temp = a;
                        maj = temp;
                        MajAsm(ref maj, ref b, ref c);*/
                        maj = MajAsm(a, b, c);
                    }
                    else
                    {

                        maj = sha.Maj(a, b,c);
                    }
                    UInt32 temp2 = S0 + maj;

                    h = g;
                    g = f;
                    f = e;
                    e = d + temp1;
                    d = c;
                    c = b;
                    b = a;
                    a = temp1 + temp2;
                }

                // Update hash values
                hashValues[0] += a;
                hashValues[1] += b;
                hashValues[2] += c;
                hashValues[3] += d;
                hashValues[4] += e;
                hashValues[5] += f;
                hashValues[6] += g;
                hashValues[7] += h;
            }

            return string.Concat(hashValues.Select(value => value.ToString("x8")));
        }

        private static byte[] PadData(byte[] data)
        {
            int originalLength = data.Length;
            int paddingLength = 64 - ((originalLength + 8) % 64);
            if (paddingLength < 1) paddingLength += 64;

            byte[] paddedData = new byte[originalLength + paddingLength + 8];
            Array.Copy(data, paddedData, originalLength);

            // Add the '1' bit
            paddedData[originalLength] = 0x80;

            // Add the length in bits as a 64-bit big-endian number
            ulong bitLength = (ulong)originalLength * 8;
            for (int i = 0; i < 8; i++)
            {
                paddedData[paddedData.Length - 8 + i] = (byte)(bitLength >> (56 - (i * 8)));
            }

            return paddedData;
        }


        
    }


}
