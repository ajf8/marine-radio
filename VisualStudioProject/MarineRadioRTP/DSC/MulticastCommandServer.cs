using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using Jayrock.Json.Conversion;
using Jayrock.Json;
using System.Threading;

namespace MarineRadioRTP
{
    public class MulticastCommandServer
    {
        public const int
            REQUEST_MAXLEN = 4096,
            LOGICAL_CHANNEL = 81;

        public DscLog DscLog { get; set; }

        public event EventHandler<DistressEventArgs> OnDistress;
        public event EventHandler<ReceiveAISEventArgs> OnReceiveAIS;
        public event EventHandler<IndividualCallEventArgs> OnIndividualCall;
        public event EventHandler<IndividualCallAckEventArgs> OnIndividualCallAck;
        public event EventHandler<UnknownDSCMessageEventArgs> OnUnknownDSCMessage;
        public event EventHandler<JsonRpcEventArgs> OnPositionRequest;
        public event EventHandler<PositionReplyEventArgs> OnPositionReply;
        public event EventHandler<JsonRpcEventArgs> OnPollRequest;
        public event EventHandler<JsonRpcEventArgs> OnDistressAck;
        public event EventHandler<JsonRpcEventArgs> OnPollReply;
        public event EventHandler<JsonRpcEventArgs> OnTestCall;
        public event EventHandler<JsonRpcEventArgs> OnTestAck;
        public event EventHandler<AllShipsCallEventArgs> OnAllShipsCall;
        public event EventHandler<ScanReplyEventArgs> OnScanReply;
        public event EventHandler<ScanRequestEventArgs> OnScanRequest;
        public event EventHandler<VhfAnnounceEventArgs> OnVhfAnnounce;

        private Socket sock;
        private byte[] dataIn = new byte[REQUEST_MAXLEN];
        private string uid;

        public MulticastCommandServer(string uid)
        {
            this.uid = uid;
            this.DscLog = new DscLog();
            this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            EndPoint ep = (EndPoint)(new IPEndPoint(IPAddress.Any, MulticastEP.Port));
            this.sock.Bind(ep);
            this.sock.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(MulticastEP.Address));
            Trace.WriteLine(String.Format("Multicast JSON-RPC server: {0}:{1}", MulticastEP.Address, MulticastEP.Port));
            sock.BeginReceiveFrom(dataIn, 0, dataIn.Length, SocketFlags.None, ref ep, new AsyncCallback(ReceiveData), ep);
        }

        public static IPEndPoint MulticastEP
        {
            get
            {
                return ConfSingleton.Instance.Network.GetChannel(LOGICAL_CHANNEL);
            }
        }

        private void ReceiveData(IAsyncResult ar)
        {
            EndPoint newEp = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));
            sock.EndReceiveFrom(ar, ref newEp);
            /* Not completely sure a Clone() is necessary, but it's probably safer to avoid
             * the buffer being overwritten by a subsequent asynchronous receive. */
            ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessRequest), dataIn.Clone());
            sock.BeginReceiveFrom(dataIn, 0, dataIn.Length, SocketFlags.None, ref newEp, new AsyncCallback(ReceiveData), newEp);
        }

        public void Close()
        {
            this.sock.Close();
        }

        private void ProcessRequest(Object state)
        {
            try
            {
                byte[] data = (byte[])state;
                string str_request = UTF8Encoding.UTF8.GetString(data);
                JsonObject obj = (JsonObject)JsonConvert.Import(str_request);
                JsonObject args = (JsonObject)obj[ProtocolConstants.KEY_ARGUMENTS];
                // Bail out if the message originated locally.
                if (uid.Equals((string)args[ProtocolConstants.ARG_UID]))
                    return;
                // Message destined for another MMSI
                string method = (string)obj[ProtocolConstants.KEY_METHOD];
                if (!method.Equals(ProtocolConstants.METHOD_AIS) && !method.StartsWith("scan-"))
                {
                    Trace.WriteLine(String.Format("[DSC OUT@{0}] {1}", DateTime.Now, obj.ToString()));
                    DscLog.Add(obj);
                }
                if (args.Contains(ProtocolConstants.ARG_TARGET_MMSI))
                {
                    long targetMmsi = ((JsonNumber)args[ProtocolConstants.ARG_TARGET_MMSI]).ToInt64();
                    if (targetMmsi != ConfSingleton.Instance.MMSI)
                    {
                        Trace.WriteLine("Discarding. target-mmsi = " + targetMmsi);
                        return;
                    }
                }
                switch ((string)obj[ProtocolConstants.KEY_METHOD])
                {
                    case ProtocolConstants.METHOD_DISTRESS:
                        if (OnDistress != null)
                            OnDistress(this, new DistressEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_AIS:
                        if (OnReceiveAIS != null)
                            OnReceiveAIS(this, new ReceiveAISEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_INDVCALL:
                        if (OnIndividualCall != null)
                            OnIndividualCall(this, new IndividualCallEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_INDVACK:
                        if (OnIndividualCallAck != null)
                            OnIndividualCallAck(this, new IndividualCallAckEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_POSITION_REQUEST:
                        if (OnPositionRequest != null)
                            OnPositionRequest(this, new JsonRpcEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_POSITION_REPLY:
                        if (OnPositionReply != null)
                            OnPositionReply(this, new PositionReplyEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_POLL_REQUEST:
                        if (OnPollRequest != null)
                            OnPollRequest(this, new JsonRpcEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_POLL_REPLY:
                        if (OnPollReply != null)
                            OnPollReply(this, new JsonRpcEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_TEST_CALL:
                        if (OnTestCall != null)
                            OnTestCall(this, new JsonRpcEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_TEST_ACK:
                        if (OnTestAck != null)
                            OnTestAck(this, new JsonRpcEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_ALL_SHIPS_CALL:
                        if (OnAllShipsCall != null)
                            OnAllShipsCall(this, new AllShipsCallEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_DISTRESS_ACK:
                        if (OnDistressAck != null)
                            OnDistressAck(this, new JsonRpcEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_SCAN_REPLY:
                        if (OnScanReply != null)
                            OnScanReply(this, new ScanReplyEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_SCAN_REQUEST:
                        if (OnScanRequest != null)
                            OnScanRequest(this, new ScanRequestEventArgs(obj));
                        break;
                    case ProtocolConstants.METHOD_VHF_ANNOUNCE:
                        if (OnVhfAnnounce != null)
                            OnVhfAnnounce(this, new VhfAnnounceEventArgs(obj));
                        break;
                    default:
                        if (OnUnknownDSCMessage != null)
                            OnUnknownDSCMessage(this, new UnknownDSCMessageEventArgs() { Message = str_request });
                        break;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error processing request: " + ex.Message);
            }
        }
    }
}