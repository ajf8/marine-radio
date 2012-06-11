using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class AllShipsCallEventArgs : JsonRpcEventArgs
    {
        public AllShipsCallEventArgs(JsonObject args)
            : base(args)
        {
        }

        public string Category
        {
            get
            {
                return (string)base.Args[ProtocolConstants.ARG_CATEGORY];
            }
        }
    }
}
