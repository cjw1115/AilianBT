using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AilianBT.Exceptions
{
    public class NetworkException:Exception
    {
        public NetworkException(string message):base(message)
        {
        }
    }
    public class ResolveException : Exception
    {
        public ResolveException(string message) : base(message)
        {
        }
    }
    public class UriIllegalException : Exception
    {
        public UriIllegalException(string message) : base(message)
        {
        }
    }
    

}
