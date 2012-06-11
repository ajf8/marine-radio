using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Threading;
using Jayrock.Json;

// events to test
/*        public event EventHandler<DistressEventArgs> OnDistress;
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
*/
namespace MarineRadioRTP.NUnitFixtures
{
    [TestFixture]
    public class DscTests
    {
        private const int RESPONSE_WAIT_MILISECONDS = 500;

        private MulticastCommandServer cmdSrv;
        private CommandDispatcher cmdDispatcher;
        private JsonRpcEventArgs received;

        private long TargetMmsi
        {
            get { return ConfSingleton.Instance.MMSI; }
        }

        public DscTests()
        {
            this.cmdSrv = new MulticastCommandServer(Guid.NewGuid().ToString());
            this.cmdDispatcher = new CommandDispatcher(Guid.NewGuid().ToString());
        }

        [Test]
        public void VhfAnnounce()
        {
            cmdSrv.OnVhfAnnounce += new EventHandler<VhfAnnounceEventArgs>(cmdSrv_OnVhfAnnounce);
            DispatchAndWait(DSCRequests.VhfAnnounce(20));
            AssertMethod(ProtocolConstants.METHOD_VHF_ANNOUNCE);
            Assert.AreEqual(((VhfAnnounceEventArgs)received).ActiveChannel, 20);
        }

        void cmdSrv_OnVhfAnnounce(object sender, VhfAnnounceEventArgs e)
        {
            this.received = e;
        }

        [Test]
        public void ScanRequest()
        {
            cmdSrv.OnScanRequest += new EventHandler<ScanRequestEventArgs>(cmdSrv_OnScanRequest);
            DispatchAndWait(DSCRequests.ScanRequest(1));
            AssertMethod(ProtocolConstants.METHOD_SCAN_REQUEST);
            Assert.AreEqual(((ScanRequestEventArgs)received).CurrentChannel, 1);
        }

        void cmdSrv_OnScanRequest(object sender, ScanRequestEventArgs e)
        {
            this.received = e;
        }

        [Test]
        public void TestCall()
        {
            cmdSrv.OnTestCall += new EventHandler<JsonRpcEventArgs>(cmdSrv_OnGenericEvent);
            DispatchAndWait(DSCRequests.TestCall(TargetMmsi));
            AssertMethod(ProtocolConstants.METHOD_TEST_CALL);
        }

        [Test]
        public void Distress()
        {
            cmdSrv.OnDistress += new EventHandler<DistressEventArgs>(cmdSrv_OnGenericEvent);
            DispatchAndWait(DSCRequests.Distress("PIRACY"));
            AssertMethod(ProtocolConstants.METHOD_DISTRESS);
            Assert.AreEqual(((DistressEventArgs)received).Nature, "PIRACY");
        }

        [Test]
        public void PosReply()
        {
            cmdSrv.OnPositionReply += new EventHandler<PositionReplyEventArgs>(cmdSrv_OnGenericEvent);
            DispatchAndWait(DSCRequests.PositionReply(TargetMmsi));
            AssertMethod(ProtocolConstants.METHOD_POSITION_REPLY);
        }

        [Test]
        public void AllShips()
        {
            cmdSrv.OnAllShipsCall += new EventHandler<AllShipsCallEventArgs>(cmdSrv_OnGenericEvent);
            DispatchAndWait(DSCRequests.AllShipsCall(ProtocolConstants.CATEGORY_URGENCY));
            AssertMethod(ProtocolConstants.METHOD_ALL_SHIPS_CALL);
            Assert.AreEqual(((AllShipsCallEventArgs)received).Category, ProtocolConstants.CATEGORY_URGENCY);
        }

        [Test]
        public void PollReply()
        {
            cmdSrv.OnPollReply += new EventHandler<JsonRpcEventArgs>(cmdSrv_OnPollReply);
            DispatchAndWait(DSCRequests.PollReply(TargetMmsi));
            AssertMethod(ProtocolConstants.METHOD_POLL_REPLY);
        }

        void cmdSrv_OnPollReply(object sender, JsonRpcEventArgs e)
        {
            this.received = e;
        }

        [Test]
        public void PosRequest()
        {
            cmdSrv.OnPositionRequest += new EventHandler<JsonRpcEventArgs>(cmdSrv_OnGenericEvent);
            DispatchAndWait(DSCRequests.PositionRequest(TargetMmsi));
            AssertMethod(ProtocolConstants.METHOD_POSITION_REQUEST);
        }

        [Test]
        public void PollRequest()
        {
            cmdSrv.OnPollRequest += new EventHandler<JsonRpcEventArgs>(cmdSrv_OnGenericEvent);
            DispatchAndWait(DSCRequests.PollRequest(TargetMmsi));
            AssertMethod(ProtocolConstants.METHOD_POLL_REQUEST);
        }

        [Test]
        public void IndvCall()
        {
            cmdSrv.OnIndividualCall += new EventHandler<IndividualCallEventArgs>(cmdSrv_OnGenericEvent);
            DispatchAndWait(DSCRequests.IndividualCall(TargetMmsi, 10, ProtocolConstants.CATEGORY_SAFETY));
            AssertMethod(ProtocolConstants.METHOD_INDVCALL);
        }

        private void AssertMethod(string method)
        {
            Assert.AreEqual(method, received.Method);
        }

        private void DispatchAndWait(JsonObject request)
        {
            cmdDispatcher.Dispatch(request);
            Thread.Sleep(RESPONSE_WAIT_MILISECONDS);
        }

        void cmdSrv_OnGenericEvent(object sender, JsonRpcEventArgs e)
        {
            this.received = e;
        }

        [TearDown]
        public void TearDown()
        {
            received = null;
        }
    }
}
