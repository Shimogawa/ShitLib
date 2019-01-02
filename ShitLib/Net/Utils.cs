using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

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

	    public static long GetTimeStampSeconds()
	    {
		    return (DateTime.UtcNow.ToUniversalTime().Ticks - 621355968000000000L) / 10000000;
	    }

	    public static string ToStringForm<T>(this IEnumerable<T> list)
	    {
		    var sb = new StringBuilder();
		    sb.Append(typeof(T));
		    sb.Append('[');
		    foreach (var item in list)
		    {
			    sb.Append(item);
			    sb.Append(", ");
		    }

		    sb.Remove(sb.Length - 2, 2);
		    sb.Append(']');
		    return sb.ToString();
	    }

	    public static string ToStringForm<K, V>(this IDictionary<K, V> dict)
	    {
		    var sb = new StringBuilder();
		    sb.Append($"({typeof(K)}, {typeof(V)})[\n");
		    foreach (var key in dict.Keys)
		    {
			    sb.Append("{");
			    sb.Append(key);
			    sb.Append(", ");
			    sb.Append(dict[key]);
			    sb.Append("},\n");
		    }

		    sb.Remove(sb.Length - 2, 2);
		    sb.Append("\n]");
		    return sb.ToString();
	    }
	}
}