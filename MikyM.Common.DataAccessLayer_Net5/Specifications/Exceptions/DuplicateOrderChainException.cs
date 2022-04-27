using System;

namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Exceptions
{
    public class DuplicateOrderChainException : Exception
    {
        private new const string Message = "The specification contains more than one Order chain!";

        public DuplicateOrderChainException()
            : base(Message)
        {
        }

        public DuplicateOrderChainException(Exception innerException)
            : base(Message, innerException)
        {
        }
    }
}