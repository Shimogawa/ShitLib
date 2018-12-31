using System;

namespace ShitLib.Net.Bilibili
{
    public class BException : Exception
    {
        public BException() : base()
        {
            
        }
        
        public BException(string msg) : base(msg)
        {
            
        }
    }
}