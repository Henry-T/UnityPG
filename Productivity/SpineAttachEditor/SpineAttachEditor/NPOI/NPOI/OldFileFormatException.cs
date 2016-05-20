using System;
using System.Collections.Generic;
using System.Text;

namespace NPOI
{
    public class OldFileFormatException : ArgumentException
    {
        public OldFileFormatException(string s)
            : base(s)
        { 
        
        }
    }
}
