using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class DistressEventArgs : JsonRpcEventArgs
    {
        public DistressEventArgs(JsonObject obj)
            : base(obj)
        {
        }

        public string Nature
        {
            get
            {
                return (string)base.Args[ProtocolConstants.ARG_DISTRESS_NATURE];
            }
        }
    }
}
