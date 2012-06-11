using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;


namespace MarineRadioRTP
{
    public class DSCRequests
    {
        public static JsonObject Distress(string nature)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_DISTRESS);
            RetrieveArgObject(request).Put(ProtocolConstants.ARG_DISTRESS_NATURE, nature);
            return request;
        }

        public static JsonObject DistressAck(long targetMmsi)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_DISTRESS_ACK);
            RetrieveArgObject(request).Put(ProtocolConstants.ARG_TARGET_MMSI, targetMmsi);
            return request;
        }

        public static JsonObject IndividualCall(long targetMmsi, int chan, string category)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_INDVCALL);
            JsonObject arguments = RetrieveArgObject(request);
            arguments.Put(ProtocolConstants.ARG_TARGET_CHAN, chan);
            arguments.Put(ProtocolConstants.ARG_TARGET_MMSI, targetMmsi);
            arguments.Put(ProtocolConstants.ARG_CATEGORY, category);
            return request;
        }

        public static JsonObject IndvAck(long mmsi, int chan, bool able, string reason)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_INDVACK);
            JsonObject arguments = RetrieveArgObject(request);
            arguments.Put(ProtocolConstants.ARG_TARGET_CHAN, chan);
            arguments.Put(ProtocolConstants.ARG_TARGET_MMSI, mmsi);
            arguments.Put(ProtocolConstants.ARG_INDVACK_ABLE, able ? 1 : 0);
            arguments.Put(ProtocolConstants.ARG_INDVACK_REASON, reason);
            return request;
        }

        public static JsonObject TestCall(long targetMmsi)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_TEST_CALL);
            RetrieveArgObject(request).Put(ProtocolConstants.ARG_TARGET_MMSI, targetMmsi);
            return request;
        }

        public static JsonObject AllShipsCall(string category)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_ALL_SHIPS_CALL);
            RetrieveArgObject(request).Put(ProtocolConstants.ARG_CATEGORY, category);
            return request;
        }

        public static JsonObject TestAck(long targetMmsi)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_TEST_ACK);
            RetrieveArgObject(request).Put(ProtocolConstants.ARG_TARGET_MMSI, targetMmsi);
            return request;
        }

        public static JsonObject AIS(GpsLocation location)
        {
            JsonObject obj = CreateBasicObject(ProtocolConstants.METHOD_AIS);
            JsonObject args = RetrieveArgObject(obj);
            args.Put(ProtocolConstants.ARG_GPS_LONGITUDE, location.Longitude);
            args.Put(ProtocolConstants.ARG_GPS_LATITUDE, location.Latitude);
            return obj;
        }

        public static JsonObject VhfAnnounce(int channel)
        {
            JsonObject obj = CreateBasicObject(ProtocolConstants.METHOD_VHF_ANNOUNCE);
            RetrieveArgObject(obj).Put(ProtocolConstants.ARG_ACTIVE_CHAN, channel);
            return obj;
        }

        public static JsonObject PositionReply(long requestorMmsi)
        {
            JsonObject obj = CreateBasicObject(ProtocolConstants.METHOD_POSITION_REPLY);
            JsonObject args = RetrieveArgObject(obj);
            args.Put(ProtocolConstants.ARG_GPS_LONGITUDE, GPSUnit.Instance.Location.Longitude);
            args.Put(ProtocolConstants.ARG_GPS_LATITUDE, GPSUnit.Instance.Location.Latitude);
            args.Put(ProtocolConstants.ARG_TARGET_MMSI, requestorMmsi);
            return obj;
        }

        public static JsonObject PollRequest(long targetMmsi)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_POLL_REQUEST);
            RetrieveArgObject(request).Put(ProtocolConstants.ARG_TARGET_MMSI, targetMmsi);
            return request;
        }

        public static JsonObject PollReply(long targetMmsi)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_POLL_REPLY);
            RetrieveArgObject(request).Put(ProtocolConstants.ARG_TARGET_MMSI, targetMmsi);
            return request;
        }

        /* Support methods. */

        private static JsonObject RetrieveArgObject(JsonObject obj)
        {
            return (JsonObject)obj[ProtocolConstants.KEY_ARGUMENTS];
        }

        public static JsonObject ScanRequest(int chan)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_SCAN_REQUEST);
            RetrieveArgObject(request).Put(ProtocolConstants.ARG_CURRENT_CHAN, chan);
            return request;
        }

        public static JsonObject ScanReply(long targetMmsi, int activeChannel)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_SCAN_REPLY);
            JsonObject args = RetrieveArgObject(request);
            args.Put(ProtocolConstants.ARG_ACTIVE_CHAN, activeChannel);
            args.Put(ProtocolConstants.ARG_TARGET_MMSI, targetMmsi);
            return request;
        }

        public static JsonObject PositionRequest(long targetMmsi)
        {
            JsonObject request = CreateBasicObject(ProtocolConstants.METHOD_POSITION_REQUEST);
            RetrieveArgObject(request).Put(ProtocolConstants.ARG_TARGET_MMSI, targetMmsi);
            return request;
        }

        private static JsonObject CreateBasicObject(string method)
        {
            JsonObject obj = new JsonObject();
            obj.Put(ProtocolConstants.KEY_METHOD, method);
            obj.Put(ProtocolConstants.KEY_ARGUMENTS, CreateBasicArgumentObject());
            return obj;
        }

        private static JsonObject CreateBasicArgumentObject()
        {
            JsonObject obj = new JsonObject();
            obj.Put(ProtocolConstants.ARG_MACHINENAME, Environment.MachineName);
            obj.Put(ProtocolConstants.ARG_MMSI, ConfSingleton.Instance.MMSI);
            obj.Put(ProtocolConstants.ARG_TIMESTAMP, DateTime.Now.ToFileTimeUtc());
            return obj;
        }
    }
}
