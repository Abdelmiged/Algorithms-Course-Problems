using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Problem
{
    // *****************************************
    // DON'T CHANGE CLASS OR FUNCTION NAME
    // YOU CAN ADD FUNCTIONS IF YOU NEED TO
    // *****************************************
    public static class IntegerMultiplication
    {
        #region YOUR CODE IS HERE

        //Your Code is Here:
        //==================
        /// <summary>
        /// Multiply 2 large integers of N digits in an efficient way [Karatsuba's Method]
        /// </summary>
        /// <param name="X">First large integer of N digits [0: least significant digit, N-1: most signif. dig.]</param>
        /// <param name="Y">Second large integer of N digits [0: least significant digit, N-1: most signif. dig.]</param>
        /// <param name="N">Number of digits (power of 2)</param>
        /// <returns>Resulting large integer of 2xN digits (left padded with 0's if necessarily) [0: least signif., 2xN-1: most signif.]</returns>
        static public byte[] IntegerMultiply(byte[] X, byte[] Y, int N)
        {
            if(X.Length <= 70 || Y.Length <= 70)
            {
                return multiplication(X, Y);
            }
            int half = N / 2;

            byte[] B = new byte[0], A = new byte[0], C = new byte[0], D = new byte[0];

            Parallel.Invoke(
                () =>
                {
                    B = byteCopyRightHalf(X);
                },
                () =>
                {
                    A = byteCopyLeftHalf(X);
                },
                () =>
                {
                    D = byteCopyRightHalf(Y);
                },
                () =>
                {
                    C = byteCopyLeftHalf(Y);
                }
            );

            byte[] M1 = new byte[0], M2 = new byte[0], BplusA = new byte[0], DplusC = new byte[0];

            Parallel.Invoke(
                () =>
                {
                    M1 = IntegerMultiply(A, C, half);
                },
                () =>
                {
                    M2 = IntegerMultiply(B, D, half);
                },
                () =>
                {
                    BplusA = addition(B, A);
                },
                () =>
                {
                    DplusC = addition(D, C);
                }
            );

            byte[] Z_beforesub = IntegerMultiply(BplusA, DplusC, half);
            byte[] ZminusM2 = subtraction(Z_beforesub, M2);
            byte[] Z = subtraction(ZminusM2, M1);

            Parallel.Invoke(
                () =>
                {
                    M2 = bytePadding(M2, 2 * half);
                },
                () =>
                {
                    Z = bytePadding(Z, half);
                }
            );
            return addition(addition(M2, Z), M1);
            
        }


        static public byte[] multiplication(byte[] firstnum, byte[] secondnum)
        {
            int size = (firstnum.Length >= secondnum.Length) ? firstnum.Length : secondnum.Length;
            byte[] output = new byte[2 * size];
            int startingpos = 0;
            int carry = 0, addeddigit = 0;
            foreach(var item2 in secondnum)
            {
                int pos = startingpos;
                foreach(var item1 in firstnum)
                {
                    int result = (item2 * item1) + carry;
                    addeddigit = result % 10;
                    carry = result / 10;
                    output[pos] += (byte)addeddigit;
                    if (output[pos] >= 10)
                    {
                        int right = output[pos] % 10;
                        int left = output[pos] / 10;
                        output[pos] = (byte)right;
                        output[pos + 1] += (byte)left;
                    }
                    pos++;
                }
                if(carry != 0)
                {
                    output[pos] += (byte)carry;
                }
                startingpos++;
                carry = 0;
                addeddigit = 0;
            }
            return output;
        }

        public static byte[] addition(byte[] firstnum, byte[] secondnum)
        {
            int size = (firstnum.Length >= secondnum.Length) ? firstnum.Length : secondnum.Length;
            byte[] output = new byte[size + 1];
            int carry = 0, addeddigit = 0;

            int i = 0;
            for (i = 0; i < size; i++)
            {
                int result = (firstnum.Length > i ? firstnum[i] : 0) + (secondnum.Length > i ? secondnum[i] : 0) + carry;
                addeddigit = result % 10;
                carry = result / 10;
                output[i] += (byte)addeddigit;
            }
            if (carry != 0)
            {
                output[i] += (byte)carry;
            }
            if(output[output.Length - 1] == 0 && output.Length != 1)
            {
                Array.Resize(ref output, output.Length - 1);
            }
            return output;
        }

        public static byte[] subtraction(byte[] firstnum, byte[] secondnum)
        {
            byte[] output = new byte[firstnum.Length];

            int borrow = 0;
            for (int i = 0; i < firstnum.Length; i++)
            {
                int result = firstnum[i] - (secondnum.Length > i ? secondnum[i] : 0) - borrow;
                borrow = (result < 0 ? borrow = 1 : borrow = 0);
                result = (result < 0 ? result += 10 : result);
                output[i] = (byte)result;
            }

            return output;
        }

        public static byte[] byteCopyRightHalf(byte[] filled)
        {
            bool odd = (filled.Length % 2 == 1) ? true : false;
            int startPos = filled.Length / 2;

            byte[] output = (odd) ? new byte[startPos + 1] : new byte[startPos];

            int indx = 0;
            for (int i = startPos; i < filled.Length; i++)
            {
                output[indx] = filled[i];
                indx++;
            }
            return output;
        }

        public static byte[] byteCopyLeftHalf(byte[] filled)
        {
            byte[] output = new byte[filled.Length / 2];

            for (int i = 0; i < filled.Length / 2; i++)
            {
                output[i] = filled[i];
            }
            return output;
        }

        public static byte[] bytePadding(byte[] filled, int zeroes)
        {
            byte[] output = new byte[filled.Length + zeroes];

            int indx = 0;
            for (int i = 0; i < output.Length; i++)
            {
                if (zeroes != 0)
                {
                    output[i] = 0;
                    zeroes--;
                    continue;
                }

                output[i] = filled[indx];
                indx++;
            }
            return output;
        }
        #endregion
    }
}