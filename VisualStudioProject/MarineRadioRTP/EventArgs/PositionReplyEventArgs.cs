using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class PositionReplyEventArgs : JsonRpcEventArgs
    {
        public PositionReplyEventArgs(JsonObject obj)
            : base(obj)
        {
        }

        public GpsLocation GpsLocation
        {
            get
            {
                if (!base.Args.Contains(ProtocolConstants.ARG_GPS_LONGITUDE) || !base.Args.Contains(ProtocolConstants.ARG_GPS_LATITUDE))
                    throw new NoGpsDataException("GPS data was not included in the other party's request");
                return new GpsLocation(((JsonNumber)base.Args[ProtocolConstants.ARG_GPS_LONGITUDE]).ToDouble(), ((JsonNumber)base.Args[ProtocolConstants.ARG_GPS_LATITUDE]).ToDouble());
            }
        }
    }
}
