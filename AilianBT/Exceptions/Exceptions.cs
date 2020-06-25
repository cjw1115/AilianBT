using System;

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

    public class OpenStreamFailedException : Exception
    {
    }
}
