using System;

namespace BitRotate
{
    public class BitRotate
    {
        public static void Shift(byte[] buffer, int start, int bits)
        {
            int total = start + 2048;
            if (buffer.Length < total)
            {
                total = buffer.Length;
            }
            byte first = buffer[start]; // buffer[0]
            byte current;
            for (int i = start; i < total; i++)
            {
                if (i >= (total - 1))
                {
                    current = first;
                }
                else
                {
                    current = buffer[i + 1];
                }
                buffer[i] = (byte)(current >> (8 - bits) | (buffer[i] << bits));
            }
        }

        public static void Unshift(byte[] data, int start, int bits)
        {
            var to = start + 2048;

            if (data.Length < to)
            {
                Console.WriteLine("To:" + to + " Data:" + data.Length);
                to = data.Length;
            }

            var last = data[to - 1];

            for (var i = to - 1; i >= start; i--)
            {
                var current = i <= start ? last : data[i - 1];

                data[i] = (byte)((current << (8 - bits)) | (data[i] >> bits));
            }
        }
    }
}
