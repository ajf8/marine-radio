using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarineRadioRTP
{
    public class NoGpsDataException : MarineRadioException
    {
        public NoGpsDataException(string message)
            : base(message)
        {
        }
    }
}
