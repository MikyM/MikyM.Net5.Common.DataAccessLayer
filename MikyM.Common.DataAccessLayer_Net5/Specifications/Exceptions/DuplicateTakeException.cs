﻿using System;

namespace MikyM.Common.DataAccessLayer_Net5.Specifications.Exceptions
{
    public class DuplicateTakeException : Exception
    {
        private new const string Message =
            "Duplicate use of Take(). Ensure you don't use both WithPaginationFilter() and Take() in the same specification!";

        public DuplicateTakeException()
            : base(Message)
        {
        }

        public DuplicateTakeException(Exception innerException)
            : base(Message, innerException)
        {
        }
    }
}