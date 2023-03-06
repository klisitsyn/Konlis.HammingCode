using System.Diagnostics;
using System.Text;

public class HammingCodeCalculator
{
    enum VerificationResult { 
        OK, 
        Corrected, 
        Unrecoverable // not implemented
    }

    static Random random = new Random();

    public static void Main()
    {
        VerifyCorrectnessFlipZeroAndOneBits("10011010");
        VerifyCorrectnessFlipZeroAndOneBits("11");
        VerifyCorrectnessFlipZeroAndOneBits("1");
        VerifyCorrectnessFlipZeroAndOneBits("0");
        VerifyCorrectnessFlipZeroAndOneBits("00");
        VerifyCorrectnessFlipZeroAndOneBits("01");
        VerifyCorrectnessFlipZeroAndOneBits("10");
        VerifyCorrectnessFlipZeroAndOneBits("00000000000000000000000000");
        VerifyCorrectnessFlipZeroAndOneBits("11111111111111111111111111");
        VerifyCorrectnessFlipZeroAndOneBits("01001000101000100010011000");
    }

    static void VerifyCorrectnessFlipZeroAndOneBits(string val)
    {
        bool[] encoded = Encode(val);

        {   // don't alter the message
            Tuple<VerificationResult, string> resultUnspoiled = Decode(encoded);
            Debug.Assert(resultUnspoiled.Item1 == VerificationResult.OK && resultUnspoiled.Item2 == val);
        }

        // try flipping every bit of message one by one
        for (int i = 0; i < encoded.Length; i++)
        {
            encoded[i] = !encoded[i];
            {
                Tuple<VerificationResult, string> resultRecovered = Decode(encoded.ToArray());
                Debug.Assert(resultRecovered.Item1 == VerificationResult.Corrected && resultRecovered.Item2 == val);
            }

            /*{
                // homework - what happens if two or more bits are spoiled?
                int otherRandomBit = random.Next(encoded.Length - 1);
                otherRandomBit = otherRandomBit == i ? i + 1 : otherRandomBit;
                encoded[otherRandomBit] = !encoded[otherRandomBit];
                Tuple<VerificationResult, string> resultCorrupt = Decode(encoded.ToArray());
                Debug.Assert(resultCorrupt.Item1 == VerificationResult.Unrecoverable);
                encoded[otherRandomBit] = !encoded[otherRandomBit];
            }*/

            // restore encoded
            encoded[i] = !encoded[i];
        }
    }

    // Calculates number of parity bits necessary for given data length
    public static int SolveParityBitCount(int dataBitCount)
    {
        int parityBitCount = (int)Math.Ceiling(Math.Log2(dataBitCount));
        while ((int)Math.Pow(2, parityBitCount) - parityBitCount - 1 - dataBitCount < 0)
        {
            parityBitCount++;
        }

        return parityBitCount;
    }

    static bool[] Encode(string boolStr)
    {
        int parityBitCount = SolveParityBitCount(boolStr.Length);
        int totalLen = parityBitCount + boolStr.Length;
        bool[] encoded = new bool[totalLen];
        int parityBitSkipped = 0;
        int[] parityBitIndex = new int[parityBitCount];

        // emplace parity and data bits
        for (int i = 0; i < totalLen; i++)
        {
            bool iIsParityBit = Math.Pow(2, parityBitSkipped) - 1 == i;
            if (iIsParityBit)
            {
                parityBitIndex[parityBitSkipped] = i;
                parityBitSkipped++;
            }
            else
            {
                encoded[i] = boolStr[i - parityBitSkipped] == '1' ? true : false;
                if (encoded[i])
                {
                    foreach (int parityBitNo in GetParityBits(i + 1))
                    {
                        encoded[parityBitIndex[parityBitNo]] = !encoded[parityBitIndex[parityBitNo]];
                    }
                }
            }
        }

        return encoded;
    }

    static Tuple<VerificationResult, string> Decode(bool[] encoded)
    {
        int seenParityBitCount = 0;
        List<int> nonMatchingParityBitPositions = new List<int>(); 
        for (int i = 0; i < encoded.Length; i++) 
        {
            bool iIsParityBit = Math.Pow(2, seenParityBitCount) - 1 == i;
            if (iIsParityBit)
            {
                bool actualParity = GetActualParityForParityBitNo(encoded, seenParityBitCount, i);
                seenParityBitCount++;
                if (encoded[i] != actualParity)
                {
                    nonMatchingParityBitPositions.Add(i + 1);
                }
            }
        }
        if (nonMatchingParityBitPositions.Count > 0)
        {
            int indexToFlip = nonMatchingParityBitPositions.Sum() - 1;
            encoded[indexToFlip] = !encoded[indexToFlip];
            return Tuple.Create(VerificationResult.Corrected, GetPayloadRemoveParityBits(encoded));
        }
        else
        {
            return Tuple.Create(VerificationResult.OK, GetPayloadRemoveParityBits(encoded));
        }
    }

    // Returns payload from encoded data stripping out parity bits, doesn't do any checks
    static string GetPayloadRemoveParityBits(bool[] encoded)
    {
        StringBuilder sb = new StringBuilder();
        int parityBitSkipped = 0;
        for (int i = 0; i < encoded.Length; i++)
        {
            bool iIsParityBit = Math.Pow(2, parityBitSkipped) - 1 == i;
            if (iIsParityBit)
            {
                parityBitSkipped++;
            }
            else
            {
                sb.Append(encoded[i] ? "1" : "0");
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Calculates observed parity for given parity bit
    /// </summary>
    /// <param name="encoded">Encoded data</param>
    /// <param name="parityBitNo">Parity bit index, 1-based</param>
    /// <param name="parityBitIdx">Parity bit position in encoded data, 0-based</param>
    /// <returns>Parity for given bit calculated from given data</returns>
    static bool GetActualParityForParityBitNo(bool[] encoded, int parityBitNo, int parityBitIdx)
    {
        bool res = false;
        int parityBitMask = 1 << parityBitNo;
        for (int i = 0; i < encoded.Length; ++i)
        {
            if(((i + 1) & parityBitMask) != 0 && encoded[i] && parityBitIdx != i)
            {
                res = !res;
            }
        }
        return res;
    }

    /// <summary>
    /// Returns parity but indexes for given encoded data bit position
    /// </summary>
    /// <param name="pos">Encoded data bit index, 0-based</param>
    /// <returns>Parity bit index, 1 based. If there are total of 5 bits, return values are in [1..5] range</returns>
    static int[] GetParityBits(int pos)
    {
        int i = 0;
        List<int> parityBits = new List<int>();
        while(pos > 0)
        {
            if(pos % 2 > 0)
            {
                parityBits.Add(i);
            }
            i++;
            pos = pos >> 1;
        }
        return parityBits.ToArray();
    }
}