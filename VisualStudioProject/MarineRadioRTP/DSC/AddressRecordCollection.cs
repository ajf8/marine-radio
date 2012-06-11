using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class AddressRecordCollection : List<AddressRecord>
    {
        public AddressRecordCollection()
            : base()
        {
        }

        public AddressRecordCollection(JsonArray array)
            : base()
        {
            foreach (JsonObject obj in array)
            {
                this.Add(new AddressRecord(obj));
            }
        }

        public void AddEntry(AddressRecord record)
        {
            if (LookupName(record.Name) > 0)
                throw new ArgumentException("DUPLICATE NAME");
            if (ReverseLookup(record.Mmsi) != null)
                throw new ArgumentException("DUPLICATE MMSI");
            this.Add(record);
        }

        public long LookupName(string name)
        {
            foreach (AddressRecord record in this)
            {
                if (record.Name.Equals(name))
                {
                    return record.Mmsi;
                }
            }
            return -1;
        }

        public string ReverseLookup(long mmsi)
        {
            foreach (AddressRecord record in this)
            {
                if (record.Mmsi.Equals(mmsi))
                {
                    return record.Name;
                }
            }
            return null;
        }

        public void RemoveAddress(string name)
        {
            AddressRecord toRemove = null;
            // Removing from the collection while iterating over
            // it is not allowed, so save the record for removal
            // in this variable and break.
            foreach (AddressRecord record in this)
            {
                if (record.Name.Equals(name))
                {
                    toRemove = record;
                    break;
                }
            }
            if (toRemove != null)
                this.Remove(toRemove);
        }

        public override string ToString()
        {
            JsonArray array = new JsonArray();
            foreach (AddressRecord record in this)
            {
                array.Put(record.ToString());
            }
            return array.ToString();
        }
    }
}
