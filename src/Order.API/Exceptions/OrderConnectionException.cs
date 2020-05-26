using System;
using System.Runtime.Serialization;

namespace Order.API.Exceptions
{
    [Serializable]
    public class OrderConnectionException : Exception
    {
        protected OrderConnectionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        public OrderConnectionException(string message, Exception inner) : base(message, inner)
        {
        }
        public OrderConnectionException(string message) : base(message)
        {

        }
        public OrderConnectionException() : base()
        {


        }
    }
}
