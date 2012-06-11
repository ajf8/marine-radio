using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace MarineRadioRTP
{
    public class ChannelChangeEventArgs : EventArgs
    {
        public int Channel { get; set; }
        public int OldChannel { get; set; }
        public IPEndPoint Endpoint { get; set; }
    }
}
