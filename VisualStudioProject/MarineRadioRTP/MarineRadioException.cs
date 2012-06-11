using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarineRadioRTP
{
    public class MarineRadioException : ApplicationException
    {
        public MarineRadioException()
            : base()
        {
        }

        public MarineRadioException(string message)
            : base(message)
        {
        }

        public MarineRadioException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
