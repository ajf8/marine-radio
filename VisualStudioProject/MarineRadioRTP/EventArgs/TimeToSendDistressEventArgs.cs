using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarineRadioRTP
{
    class TimeToSendDistressEventArgs : EventArgs
    {
        public string Nature { get; set; }
    }
}
