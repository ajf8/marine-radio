using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Jayrock.Json;
using Jayrock.Json.Conversion;
using System.Net;
using System.Net.Sockets;

namespace MarineRadioRTP
{
    public sealed class ConfSingleton
    {
        private const string
            CONFFILE = "settings.json",
            CONFKEY_MCAST_NETSTART = "mcast-net-start",
            CONFKEY_CAPTURE_DEVICE_INDEX = "capture-device-index",
            CONFKEY_MCAST_NETPREFIX = "mcast-net-prefix",
            CONFKEY_AUTO_ACK = "auto-ack",
            CONFKEY_SOUND_EFFECTS = "sound-effects",
            CONFKEY_COMPRESSION = "compression";

        private const int
            MMSI_LENGTH = 9;

        private JsonObject confMap;
        private MulticastNetwork network;

        private ConfSingleton()
        {
            ReloadConfMap();
        }

        private string GetConfFileLocation()
        {
            return Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), CONFFILE);
        }

        public void RefreshMulticastNet()
        {
            this.network = new MulticastNetwork(this.MulticastNetworkStart, this.MulticastPrefix);
        }

        public void ReloadConfMap()
        {
            StreamReader reader = null;
            FileStream inStream = null;
            try
            {
                inStream = new FileStream(GetConfFileLocation(), FileMode.Open, FileAccess.Read);
                Console.WriteLine("Reading from " + GetConfFileLocation());
                reader = new StreamReader(inStream);
                this.confMap = (JsonObject)JsonConvert.Import(reader.ReadToEnd());
            }
            catch (Exception ex)
            {
                this.confMap = new JsonObject();
                Trace.WriteLine("Unable to import configuration: " + ex.Message);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (inStream != null)
                    inStream.Close();
            }
        }

        /*public bool AutoAck
        {
            get
            {
                return confMap.Contains(CONFKEY_AUTO_ACK) ? GetBool(confMap[CONFKEY_AUTO_ACK]): false;
            }
            set
            {
                confMap[CONFKEY_AUTO_ACK] = value ? 1 : 0;
            }
        }*/

        public bool Compression
        {
            get
            {
                return confMap.Contains(CONFKEY_COMPRESSION) ? GetBool(confMap[CONFKEY_COMPRESSION]) : true;
            }
            set
            {
                confMap[CONFKEY_COMPRESSION] = BoolToInt(value);
            }
        }

        private int BoolToInt(bool value)
        {
            return value ? 1 : 0;
        }

        private bool autoAck = false;
        public bool AutoAck
        {
            get { return autoAck; }
            set { autoAck = value; }
        }

        public bool SoundEffects
        {
            get
            {
                return confMap.Contains(CONFKEY_SOUND_EFFECTS) ? GetBool(confMap[CONFKEY_SOUND_EFFECTS]) : true;
            }
            set
            {
                confMap[CONFKEY_SOUND_EFFECTS] = BoolToInt(value);
            }
        }

        public string MulticastNetworkStart
        {
            get
            {
                return confMap.Contains(CONFKEY_MCAST_NETSTART) ? (string)confMap[CONFKEY_MCAST_NETSTART] : "239.255.254.1";
            }
            set
            {
                if (IPAddress.Parse(value).AddressFamily != AddressFamily.InterNetwork)
                    throw new ArgumentException("Only IPv4 is supported.");
                this.confMap[CONFKEY_MCAST_NETSTART] = value;
            }
        }

        private bool GetBool(object o)
        {
            if (o.GetType().Equals(typeof(JsonNumber)))
            {
                return ((JsonNumber)o).ToInt32() > 0;
            }
            else
            {
                return (int)o > 0;
            }
        }

        private int GetInt(object o)
        {
            if (o.GetType().Equals(typeof(JsonNumber)))
            {
                return ((JsonNumber)o).ToInt32();
            }
            else
            {
                return (int)o;
            }
        }

        public MulticastNetwork Network
        {
            get
            {
                if (this.network == null)
                    RefreshMulticastNet();
                return this.network;
            }
            set
            {
                this.network = value;
            }
        }

        public int MulticastPrefix
        {
            get
            {
                return confMap.Contains(CONFKEY_MCAST_NETPREFIX) ? GetInt(confMap[CONFKEY_MCAST_NETPREFIX]) : 25;
            }
            set
            {
                this.confMap[CONFKEY_MCAST_NETPREFIX] = value;
            }
        }

        public int CaptureDeviceIndex
        {
            get
            {
                return confMap.Contains(CONFKEY_CAPTURE_DEVICE_INDEX) ? GetInt(confMap[CONFKEY_CAPTURE_DEVICE_INDEX]) : 0;
            }
            set
            {
                confMap[CONFKEY_CAPTURE_DEVICE_INDEX] = value;
            }
        }

        private long mmsi = -1;
        public long MMSI
        {
            set
            {
                if (this.mmsi.ToString().Length != 9)
                    throw new ArgumentException("An MMSI must be nine digits.");
                this.mmsi = value;
            }
            get { return this.mmsi > 0 ? this.mmsi : this.mmsi = GenerateMMSI(); }
        }

        private long GetLong(object o)
        {
            if (o.GetType().Equals(typeof(JsonNumber)))
            {
                return ((JsonNumber)o).ToInt64();
            }
            else
            {
                return (long)o;
            }
        }

        private long GenerateMMSI()
        {
            string machineName = Environment.MachineName.ToLower();
            if (machineName == "ajf-laptop" || machineName == "anf6-cs394-1")
            {
                return 123456789;
            }
            else if (machineName == "ashleigh-pc" || machineName == "anf6-cs394-2")
            {
                return 223456789;
            }
            StringBuilder sb = new StringBuilder(MMSI_LENGTH);
            Random rand = new Random();
            for (int i = 0; i < MMSI_LENGTH; i++)
            {
                sb.Append(rand.Next(1, 9));
            }
            return Int64.Parse(sb.ToString());
        }

        public void Save()
        {
            StreamWriter writer = null;
            FileStream outFile = null;
            Exception exception = null;
            try
            {
                RefreshMulticastNet();
                outFile = new FileStream(GetConfFileLocation(), FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(outFile);
                writer.WriteLine(this.confMap.ToString());
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            if (writer != null)
                writer.Close();
            if (outFile != null)
                outFile.Close();
            if (exception != null)
                throw exception;
        }

        private static ConfSingleton instance = null;
        private static readonly object padlock = new object();

        public static ConfSingleton Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ConfSingleton();
                    }
                    return instance;
                }
            }
        }
    }
}
