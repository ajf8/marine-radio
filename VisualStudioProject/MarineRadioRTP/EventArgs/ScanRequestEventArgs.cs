using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class ScanRequestEventArgs : JsonRpcEventArgs
    {
        public ScanRequestEventArgs(JsonObject args)
            : base(args)
        {
        }

        public int CurrentChannel
        {
            get
            {
                return ((JsonNumber)base.Args[ProtocolConstants.ARG_CURRENT_CHAN]).ToInt32();
            }
        }
    }
}
