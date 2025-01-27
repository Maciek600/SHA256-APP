namespace Sha
{
    public class Class1
    {
        // Funkcja pomocnicza dla operacji ROTR
        public UInt32 ROTR(UInt32 x, byte n) { return (x >> n) | (x << (32 - n)); }

        // Funkcja pomocnicza dla operacji Σ0
        public UInt32 Sigma0(UInt32 x) { return ROTR(x, 7) ^ ROTR(x, 18) ^ (x >> 3); }

        // Funkcja pomocnicza dla operacji Σ1
        public UInt32 Sigma1(UInt32 x) { return ROTR(x, 17) ^ ROTR(x, 19) ^ (x >> 10); }

        // Funkcja pomocnicza dla operacji Maj
        public UInt32 Maj(UInt32 x, UInt32 y, UInt32 z) { return (x & y) ^ (x & z) ^ (y & z); }

        // Funkcja pomocnicza dla operacji Ch
        public UInt32 Ch(UInt32 x, UInt32 y, UInt32 z) { return (x & y) ^ (~x & z); }

        // Funkcja pomocnicza dla operacji Σ
        public UInt32 BigSigma0(UInt32 x) { return ROTR(x, 2) ^ ROTR(x, 13) ^ ROTR(x, 22); }

        // Funkcja pomocnicza dla operacji Σ1
        public UInt32 BigSigma1(UInt32 x) { return ROTR(x, 6) ^ ROTR(x, 11) ^ ROTR(x, 25); }
    }
}
