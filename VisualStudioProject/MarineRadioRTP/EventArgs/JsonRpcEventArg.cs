using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class JsonRpcEventArgs : EventArgs
    {
        private JsonObject request;

        public string Method
        {
            get
            {
                return (string)request[ProtocolConstants.KEY_METHOD];
            }
        }

        public JsonObject Args
        {
            get
            {
                return (JsonObject)this.request[ProtocolConstants.KEY_ARGUMENTS];
            }
        }

        public JsonRpcEventArgs(JsonObject obj)
            : base()
        {
            this.request = obj;
        }

        public long Mmsi
        {
            get
            {
                return ToLong(Args[ProtocolConstants.ARG_MMSI]);
            }
        }

        public string MachineName
        {
            get
            {
                return (string)Args[ProtocolConstants.ARG_MACHINENAME];
            }
        }

        public DateTime Timestamp
        {
            get
            {
                return DateTime.FromFileTimeUtc(ToLong(Args[ProtocolConstants.ARG_TIMESTAMP]));
            }
        }

        public string UID
        {
            get
            {
                return (string)Args[ProtocolConstants.ARG_UID];
            }
        }

        public static long ToLong(object obj)
        {
            return ((JsonNumber)obj).ToInt64();
        }
    }
}
