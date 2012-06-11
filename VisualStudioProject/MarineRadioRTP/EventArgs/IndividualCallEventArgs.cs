using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class IndividualCallEventArgs : JsonRpcEventArgs
    {
        public IndividualCallEventArgs(JsonObject args)
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

        public long TargetMmsi
        {
            get
            {
                return ((JsonNumber)base.Args[ProtocolConstants.ARG_TARGET_MMSI]).ToInt64();
            }
        }

        public int TargetChan
        {
            get
            {
                return ((JsonNumber)base.Args[ProtocolConstants.ARG_TARGET_CHAN]).ToInt32();
            }
        }
    }
}
