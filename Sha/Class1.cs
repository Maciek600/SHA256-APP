// <author>Maciej Fajlhauer</author>
// <date>2024/2025</date>
// <version>1.0</version>
// <summary>
// Implements core operations required for SHA256 hash computation.
// Contains C# implementations of all basic mathematical functions
// used in the SHA256 algorithm.
// </summary>
namespace Sha
{
    /// <summary>
    /// Provides implementations of core SHA256 mathematical operations.
    /// These operations form the foundation of the SHA256 hashing algorithm.
    /// </summary>
    public class Class1
    {
        /// <summary>
        /// Performs a right rotation (circular right shift) on a 32-bit unsigned integer.
        /// </summary>
        /// <param name="x">The value to rotate</param>
        /// <param name="n">The number of positions to rotate right</param>
        /// <returns>The rotated value</returns>
        /// <remarks>
        /// ROTR-n(x) = (x >> n) OR (x << (32-n))
        /// This operation is different from a regular right shift as bits that
        /// are shifted off the right end appear on the left end.
        /// </remarks>
        public UInt32 ROTR(UInt32 x, byte n)
        {
            return (x >> n) | (x << (32 - n));
        }

        /// <summary>
        /// Implements the σ0 (sigma0) function used in message schedule array expansion.
        /// </summary>
        /// <param name="x">Input value</param>
        /// <returns>Result of σ0 operation</returns>
        /// <remarks>
        /// σ0(x) = ROTR⁷(x) ⊕ ROTR¹⁸(x) ⊕ SHR³(x)
        /// Where ⊕ represents XOR operation
        /// </remarks>
        public UInt32 Sigma0(UInt32 x)
        {
            return ROTR(x, 7) ^ ROTR(x, 18) ^ (x >> 3);
        }

        /// <summary>
        /// Implements the σ1 (sigma1) function used in message schedule array expansion.
        /// </summary>
        /// <param name="x">Input value</param>
        /// <returns>Result of σ1 operation</returns>
        /// <remarks>
        /// σ1(x) = ROTR¹⁷(x) ⊕ ROTR¹⁹(x) ⊕ SHR¹⁰(x)
        /// Where ⊕ represents XOR operation
        /// </remarks>
        public UInt32 Sigma1(UInt32 x)
        {
            return ROTR(x, 17) ^ ROTR(x, 19) ^ (x >> 10);
        }

        /// <summary>
        /// Implements the Maj (majority) function.
        /// Returns the majority value for each bit position.
        /// </summary>
        /// <param name="x">First input value</param>
        /// <param name="y">Second input value</param>
        /// <param name="z">Third input value</param>
        /// <returns>Result of Maj operation</returns>
        /// <remarks>
        /// Maj(x, y, z) = (x ∧ y) ⊕ (x ∧ z) ⊕ (y ∧ z)
        /// Where ∧ represents AND operation and ⊕ represents XOR operation
        /// </remarks>
        public UInt32 Maj(UInt32 x, UInt32 y, UInt32 z)
        {
            return (x & y) ^ (x & z) ^ (y & z);
        }

        /// <summary>
        /// Implements the Ch (choose) function.
        /// Selects bits from y or z based on the value of x.
        /// </summary>
        /// <param name="x">Control input value</param>
        /// <param name="y">First choice input</param>
        /// <param name="z">Second choice input</param>
        /// <returns>Result of Ch operation</returns>
        /// <remarks>
        /// Ch(x, y, z) = (x ∧ y) ⊕ (¬x ∧ z)
        /// Where ∧ represents AND operation, ⊕ represents XOR operation,
        /// and ¬ represents NOT operation
        /// </remarks>
        public UInt32 Ch(UInt32 x, UInt32 y, UInt32 z)
        {
            return (x & y) ^ (~x & z);
        }

        /// <summary>
        /// Implements the Σ0 (big sigma 0) function used in the compression function.
        /// </summary>
        /// <param name="x">Input value</param>
        /// <returns>Result of Σ0 operation</returns>
        /// <remarks>
        /// Σ0(x) = ROTR²(x) ⊕ ROTR¹³(x) ⊕ ROTR²²(x)
        /// Where ⊕ represents XOR operation
        /// </remarks>
        public UInt32 BigSigma0(UInt32 x)
        {
            return ROTR(x, 2) ^ ROTR(x, 13) ^ ROTR(x, 22);
        }

        /// <summary>
        /// Implements the Σ1 (big sigma 1) function used in the compression function.
        /// </summary>
        /// <param name="x">Input value</param>
        /// <returns>Result of Σ1 operation</returns>
        /// <remarks>
        /// Σ1(x) = ROTR⁶(x) ⊕ ROTR¹¹(x) ⊕ ROTR²⁵(x)
        /// Where ⊕ represents XOR operation
        /// </remarks>
        public UInt32 BigSigma1(UInt32 x)
        {
            return ROTR(x, 6) ^ ROTR(x, 11) ^ ROTR(x, 25);
        }
    }
}
