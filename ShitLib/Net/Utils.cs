using System;
using System.Net.Sockets;

namespace ShitLib.Net
{
    public static class Utils
    {
        public static byte[] ToBigEndian(this byte[] arr)
        {
            return ToBigEndian(arr, 0, arr.Length);
        }
        
        public static byte[] ToBigEndian(this byte[] arr, int offset, int len)
        {
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(arr, offset, len);
            }

            return arr;
        }
        
        public static void ReadB(this NetworkStream stream, 
            byte[] buffer, int offset, int count)
        {
            if (offset + count > buffer.Length)
                throw new ArgumentException();
            var read = 0;
            while (read < count)
            {
                var available = stream.Read(buffer, offset, count - read);
                if (available == 0)
                {
                    throw new ObjectDisposedException(null);
                }

                read += available;
                offset += available;
            }
        }
    }
}