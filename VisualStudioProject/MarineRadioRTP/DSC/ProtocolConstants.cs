using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarineRadioRTP
{
    class ProtocolConstants
    {
        public const string
            KEY_METHOD = "method",
            KEY_ARGUMENTS = "arguments",
            ARG_UID = "uid",
            ARG_MACHINENAME = "machine-name",
            ARG_MMSI = "mmsi",
            ARG_TIMESTAMP = "timestamp",
            ARG_GPS_LONGITUDE = "gps-longitude",
            ARG_GPS_LATITUDE = "gps-latitude",
            ARG_DISTRESS_NATURE = "distress-nature",
            ARG_TARGET_CHAN = "target-chan",
            ARG_TARGET_MMSI = "target-mmsi",
            ARG_CATEGORY = "category",
            ARG_INDVACK_ABLE = "indvack-able",
            ARG_INDVACK_REASON = "indvack-reason",
            ARG_ALLSHIPS_CATEGORY = "all-ships-category",
            ARG_ACTIVE_CHAN = "active-chan",
            ARG_CURRENT_CHAN = "current-chan",
            METHOD_DISTRESS = "distress",
            METHOD_AIS = "ais",
            METHOD_INDVCALL = "indvcall",
            METHOD_INDVACK = "indvack",
            METHOD_POSITION_REQUEST = "position-request",
            METHOD_POSITION_REPLY = "position-reply",
            METHOD_POLL_REQUEST = "poll-request",
            METHOD_POLL_REPLY = "poll-reply",
            METHOD_TEST_CALL = "test-call",
            METHOD_TEST_ACK = "test-ack",
            METHOD_ALL_SHIPS_CALL = "all-ships-call",
            METHOD_DISTRESS_ACK = "distress-ack",
            METHOD_SCAN_REQUEST = "scan-request",
            METHOD_SCAN_REPLY = "scan-reply",
            METHOD_VHF_ANNOUNCE = "vhf-announce",
            CATEGORY_ROUTINE = "ROUTINE",
            CATEGORY_URGENCY = "URGENCY",
            CATEGORY_DISTRESS = "DISTRESS",
            CATEGORY_SAFETY = "SAFETY";
    }
}
