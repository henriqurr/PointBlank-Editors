/*
 * Arquivo: ReceiveGPacket.cs
 * Código criado pela MoMz Games
 * Última data de modificação: 25/11/2016
 * Sintam inveja, não nos atinge
 */

using System;
using System.Text;

namespace BitRotate
{
    public class ReceiveGPacket
    {
        private byte[] _buffer;
        private int _offset;
        public ReceiveGPacket(byte[] buff)
        {
            _buffer = buff;
        }
        public byte[] getBuffer()
        {
            return _buffer;
        }
        public int readD()
        {
            int num = BitConverter.ToInt32(_buffer, _offset);
            _offset += 4;
            return num;
        }
        public uint readUD()
        {
            uint num = BitConverter.ToUInt32(_buffer, _offset);
            _offset += 4;
            return num;
        }
        public byte readC()
        {
            try
            {
                byte num = _buffer[_offset++];
                return num;
            }
            catch { return 0; }
        }

        public byte[] readB(int Length)
        {
            byte[] result = new byte[Length];
            Array.Copy(_buffer, _offset, result, 0, Length);
            _offset += Length;
            return result;
        }

        public short readH()
        {
            short num = BitConverter.ToInt16(_buffer, _offset);
            _offset += 2;
            return num;
        }

        public ushort readUH()
        {
            ushort num = BitConverter.ToUInt16(_buffer, _offset);
            _offset += 2;
            return num;
        }

        public double readF()
        {
            double num = BitConverter.ToDouble(_buffer, _offset);
            _offset += 8;
            return num;
        }
        public float readT()
        {
            float num = BitConverter.ToSingle(_buffer, _offset);
            _offset += 4;
            return num;
        }
        public long readQ()
        {
            long num = BitConverter.ToInt64(_buffer, _offset);
            _offset += 8;
            return num;
        }

        public string readS(int Length)
        {
            string str = "";
            try
            {
                str = ConfigGB.EncodeText.GetString(_buffer, _offset, Length);
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
        public string readS(int Length, int CodePage)
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
        public string readS()
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
    public static class ConfigGB
    {
        public static string dbName, dbHost, dbUser, dbPass;
        public static int dbPort;
        public static Encoding EncodeText = Encoding.GetEncoding(1252);
    }
}