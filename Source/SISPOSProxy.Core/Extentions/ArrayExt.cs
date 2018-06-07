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

        public static void Populate<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }
    }
}
