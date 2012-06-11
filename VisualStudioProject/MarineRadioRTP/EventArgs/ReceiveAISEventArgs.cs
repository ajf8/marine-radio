using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class ReceiveAISEventArgs : JsonRpcEventArgs
    {
        private DateTime localTime;

        public ReceiveAISEventArgs(JsonObject obj)
            : base(obj)
        {
            this.localTime = DateTime.Now;
        }

        public DateTime LocalTime
        {
            get { return this.localTime; }
        }

        public bool IsRecent
        {
            get
            {
                return DateTime.Now.Subtract(localTime).TotalSeconds < 30;
            }
        }

        public GpsLocation GpsLocation
        {
            get
            {
                if (!base.Args.Contains(ProtocolConstants.ARG_GPS_LONGITUDE) || !base.Args.Contains(ProtocolConstants.ARG_GPS_LATITUDE))
                    throw new NoGpsDataException("GPS data was no included in the other party's request");
                return new GpsLocation(((JsonNumber)base.Args[ProtocolConstants.ARG_GPS_LONGITUDE]).ToDouble(), ((JsonNumber)base.Args[ProtocolConstants.ARG_GPS_LATITUDE]).ToDouble());
            }
        }
    }
}
