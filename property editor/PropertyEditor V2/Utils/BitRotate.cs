using System.Collections.Generic;

namespace PropertyEditor
{
    public class BitRotate
    {
        public static void Shift(byte[] buffer, int start, int length, int bits)
        {
            int total = start + length;
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

        public static void Unshift(IList<byte> data, int start, int length, int bits)
        {
            var total = start + length;

            if (data.Count < total) total = data.Count;

            var last = data[total - 1];

            for (var i = total - 1; i >= start; i--)
            {
                var _current = i <= start ? last : data[i - 1];
                data[i] = (byte)((_current << (8 - bits)) | (data[i] >> bits));
            }
        }
    }
}
