using System;
using System.Collections.Generic;
using System.Text;

namespace WM.GUID.Application.Exceptions
{

    public class ExpiredException : Exception
    {
        public ExpiredException()
        {
        }

        public ExpiredException(string message)
            : base(message)
        {
        }

        public ExpiredException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
