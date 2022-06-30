using System;
using System.Text;

namespace PropertyEditor
{
    public class BReader
    {
        public byte[] _buffer;
        public int _offset = 0;
        public BReader(byte[] buffer)
        {
            this._buffer = buffer;
        }

        public long GetCurrentPostion()
        {
            return _offset;
        }

        public int ReadInt()
        {
            int num = BitConverter.ToInt32(_buffer, _offset);
            _offset += 4;
            return num;
        }
        public uint ReadUInt()
        {
            uint num = BitConverter.ToUInt32(_buffer, _offset);
            _offset += 4;
            return num;
        }
        public byte ReadByte()
        {
            try
            {
                byte num = _buffer[_offset++];
                return num;
            }
            catch { return 0; }
        }

        public byte[] ReadBytes(int Length)
        {
            byte[] result = new byte[Length];
            Array.Copy(_buffer, _offset, result, 0, Length);
            _offset += Length;
            return result;
        }

        public short ReadShort()
        {
            short num = BitConverter.ToInt16(_buffer, _offset);
            _offset += 2;
            return num;
        }

        public ushort ReadUShort()
        {
            ushort num = BitConverter.ToUInt16(_buffer, _offset);
            _offset += 2;
            return num;
        }

        public double ReadDouble()
        {
            double num = BitConverter.ToDouble(_buffer, _offset);
            _offset += 8;
            return num;
        }
        public float ReadFloat()
        {
            float num = BitConverter.ToSingle(_buffer, _offset);
            _offset += 4;
            return num;
        }
        public long ReadLong()
        {
            long num = BitConverter.ToInt64(_buffer, _offset);
            _offset += 8;
            return num;
        }

        public ulong ReadULong()
        {
            ulong num = BitConverter.ToUInt64(_buffer, _offset);
            _offset += 8;
            return num;
        }

        public string ReadString(int Length)
        {
            string str = "";
            try
            {
                str = Encoding.GetEncoding(1252).GetString(_buffer, _offset, Length);
                int length = str.IndexOf((char)0);
                if (length != -1)
                    str = str.Substring(0, length);
                this._offset += Length;
            }
            catch
            {
            }
            return str;
        }
        public string ReadString(int Length, int CodePage)
        {
            string str = "";
            try
            {
                str = Encoding.GetEncoding(CodePage).GetString(_buffer, _offset, Length);
                int length = str.IndexOf((char)0);
                if (length != -1)
                    str = str.Substring(0, length);
                this._offset += Length;
            }
            catch
            {
            }
            return str;
        }
        public string ReadString()
        {
            string result = "";
            try
            {
                int count = (_buffer.Length - _offset);
                result = Encoding.Unicode.GetString(_buffer, _offset, count);
                int idx = result.IndexOf(char.MinValue);
                if (idx != -1)
                    result = result.Substring(0, idx);
                _offset += result.Length + 1;
            }
            catch
            {
            }
            return result;
        }
    }
}
