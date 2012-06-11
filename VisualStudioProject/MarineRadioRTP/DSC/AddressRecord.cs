using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class AddressRecord
    {
        private const string
            KEY_NAME = "name",
            KEY_MMSI = "mmsi";

        public long Mmsi;
        public string Name;

        public AddressRecord(string name, long mmsi)
        {
            this.Mmsi = mmsi;
            this.Name = name;
        }

        public AddressRecord(JsonObject obj)
        {
            this.Name = (string)obj[KEY_NAME];
            this.Mmsi = ((JsonNumber)obj[KEY_MMSI]).ToInt64();
        }

        public override string ToString()
        {
            JsonObject serialized = new JsonObject();
            serialized.Put(KEY_MMSI, this.Mmsi);
            serialized.Put(KEY_NAME, this.Name);
            return serialized.ToString();
        }
    }
}
