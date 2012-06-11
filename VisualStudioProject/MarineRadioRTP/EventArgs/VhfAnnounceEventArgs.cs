using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class VhfAnnounceEventArgs : JsonRpcEventArgs
    {
        public VhfAnnounceEventArgs(JsonObject obj)
            : base(obj)
        {
        }

        public int ActiveChannel
        {
            get
            {
                return ((JsonNumber)base.Args[ProtocolConstants.ARG_ACTIVE_CHAN]).ToInt32();
            }
        }
    }
}
