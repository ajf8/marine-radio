using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class DscLogItem
    {
        private bool isRead = false;
        public bool IsRead
        {
            get { return this.isRead; }
            set { this.isRead = value; }
        }

        public string Category
        {
            get { return ((string)Data[ProtocolConstants.KEY_METHOD]).ToUpper(); }
        }

        public JsonObject Data { get; set; }

        public DscLogItem(JsonObject data)
        {
            this.Data = data;
        }

        public string LogEntry
        {
            get
            {
                JsonObject args = (JsonObject)this.Data[ProtocolConstants.KEY_ARGUMENTS];
                StringBuilder sb = new StringBuilder();
                sb.Append(String.Format("{0} frm {1} (", Category, ((JsonNumber)args[ProtocolConstants.ARG_MMSI]).ToString()));
                foreach (string key in args.Names)
                {
                    switch (key)
                    {
                        case ProtocolConstants.ARG_MACHINENAME:
                            break;
                        case ProtocolConstants.ARG_UID:
                            break;
                        case ProtocolConstants.ARG_TIMESTAMP:
                            AppendAttribute(sb, "TIME", DateTime.FromFileTimeUtc(((JsonNumber)args[key]).ToInt64()).ToString("HH:mm"));
                            break;
                        case ProtocolConstants.ARG_TARGET_MMSI:
                            break;
                        case ProtocolConstants.ARG_GPS_LONGITUDE:
                            break;
                        case ProtocolConstants.ARG_GPS_LATITUDE:
                            AppendAttribute(sb, "POSITION", new GpsLocation(((JsonNumber)args[ProtocolConstants.ARG_GPS_LONGITUDE]).ToDouble(), ((JsonNumber)args[ProtocolConstants.ARG_GPS_LATITUDE]).ToDouble()).ToString());
                            break;
                        case ProtocolConstants.ARG_MMSI:
                            break;
                        default:
                            AppendAttribute(sb, key, args[key]);
                            break;
                    }
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(')');
                return sb.ToString();
            }
        }

        private void AppendAttribute(StringBuilder sb, string key, object value)
        {
            sb.Append(String.Format("{0}={1} ", key, value));
        }
    }

    public class DscLog
    {
        private List<DscLogItem> log = new List<DscLogItem>();
        private const int DISPLAY_NUMBER_ITEMS = 20;
        public event EventHandler<DscRequestLoggedEventArgs> OnLogged;

        public IEnumerator<DscLogItem> GetEnumerator()
        {
            return log.GetEnumerator();
        }

        public void Add(JsonObject obj)
        {
            log.Add(new DscLogItem(obj));
            if (OnLogged != null)
                OnLogged(this, new DscRequestLoggedEventArgs() { RequestData = obj });
        }

        public void Clear()
        {
            log.Clear();
        }

        public int Count
        {
            get { return log.Count; }
        }

        public string[] GetByCategory(string category)
        {
                List<string> results = new List<string>();
            foreach(DscLogItem item in log)
            {
                if (item.Category.Equals(category))
                    results.Add(item.LogEntry);
            }
                return results.ToArray();
        }

        public string[] Categories
        {
            get
            {
                List<string> categories = new List<string>();
                foreach (DscLogItem item in log)
                {
                    if (!categories.Contains(item.Category))
                        categories.Add(item.Category);
                }
                return categories.ToArray();
            }
        }
    }
}
