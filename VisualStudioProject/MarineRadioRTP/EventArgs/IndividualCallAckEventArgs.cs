using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class IndividualCallAckEventArgs : IndividualCallEventArgs
    {
        public IndividualCallAckEventArgs(JsonObject args)
            : base(args)
        {
        }

        public string AckReason
        {
            get
            {
                return (string)base.Args[ProtocolConstants.ARG_INDVACK_REASON];
            }
        }

        public bool Able
        {
            get
            {
                return ((JsonNumber)base.Args[ProtocolConstants.ARG_INDVACK_ABLE]).ToInt32() > 0;
            }
        }
    }
}
