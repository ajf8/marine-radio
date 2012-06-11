using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarineRadioRTP
{
    public class AISNodeUpdateEventArgs : EventArgs
    {
        public ReceiveAISEventArgs ReceiveAISEventArgs { get; set; }
    }
}
