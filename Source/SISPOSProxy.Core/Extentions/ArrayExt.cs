using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SISPOSProxy.Core.Extentions
{
    static class ArrayExt
    {
        public static T[] Slice<T>(this T[] source, int from, int count)
        {
            T[] result = new T[count];
            Buffer.BlockCopy(source, from, result, 0, count);

            return result;
        }

        public static void Populate<T>(this T[] source, T value)
        {
            for (int i = 0; i < source.Length; i++)
            {
                source[i] = value;
            }
        }

        public static bool ContainsSubArray<T>(this T[] source, T[] subarray)
        {
            if (source.Length == 0 || subarray.Length == 0) return false;

            var result = true;

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i].Equals(subarray[0]))
                {
                    result = true;

                    for (int j = 1; j < subarray.Length; j++)
                    {
                        if (!source[i + j].Equals(subarray[j]))
                        {
                            result = false;
                            break;
                        }
                    }
                }
            }

            return result;
        }
    }
}
