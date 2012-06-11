using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Diagnostics;
using Jayrock.Json;
using System.Collections.Generic;

namespace MarineRadioRTP
{
    public partial class ICM411 : Form
    {
        private RtpAudioFacade rtp; // Responsible for managing RTP sessions.
        private MulticastCommandServer cmdSrv; // Responsible for receiving and publishing DSC events.
        private CommandDispatcher cmdDispatcher; // For sending JSON-RPC requests.
        private AISRegistry aisReg; // Used to populate the radar when it's opened.
        private OutgoingDistress outgoingDistress; // To encapsulate the retransmission timer.
        private AddressRecordCollection groupAddresses; // Address books.
        private AddressRecordCollection indvAddresses;
        private DscEnvelope dscEnvelope; // For temporarily storing data like target MMSI between steps.
        private CyclingMenuItemCollection mainMenu; // Keep track of this so new menu items can be added when messages are received.
        private int watchChannel = -1;

        private bool power = false;

        /* MMSI programming */
        private string lcdInput;
        private string lcdInputPreConfirm;
        private object tmp;
        private int cursor;
        private List<int> taggedChannels = new List<int>(new int[] { 16, 70 });

        public ICM411()
        {
            string uid = Guid.NewGuid().ToString();
            rtp = new RtpAudioFacade(ConfSingleton.Instance.Network);
            try
            {
                cmdSrv = new MulticastCommandServer(uid);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to start command server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
            cmdDispatcher = new CommandDispatcher(uid);
            aisReg = new AISRegistry(cmdSrv);
            outgoingDistress = new OutgoingDistress(rtp);
            groupAddresses = new AddressRecordCollection();
            indvAddresses = new AddressRecordCollection();
            deferredChanChangeTimer = new Timer();
            deferredChanChangeTimer.Interval = 1000;
            deferredChanChangeTimer.Enabled = false;
            deferredChanChangeTimer.Tick += new EventHandler(deferredChanChangeTimer_Tick);
            dscEnvelope = new DscEnvelope();
            outgoingDistress.DistressEnded += new EventHandler(outgoingDistress_DistressEnded);
            outgoingDistress.DistressStarted += new EventHandler(outgoingDistress_DistressStarted);
            outgoingDistress.TimeToSendDistress += new EventHandler<TimeToSendDistressEventArgs>(outgoingDistress_TimeToSendDistress);
            InitializeComponent();
            try
            {
                rtp.SetVoiceDevices(this);
            }
            catch (Exception)
            {
                //MessageBox.Show(String.Format("Unable to initialise sound hardware. This may be because there isn't a physical output device present.{0}{1}", Environment.NewLine + Environment.NewLine, e.InnerException != null ? e.InnerException.Message : e.Message), "DirectSound error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                notifyIcon.ShowBalloonTip(4000, "Unable to initialise sound hardware", "This may be because there is no physical output device connected.", ToolTipIcon.Error);
            }
        }

        private void ICM411_Load(object sender, EventArgs e)
        {
            cmdSrv.OnDistress += new EventHandler<DistressEventArgs>(cmd_OnDistress);
            //cmdSrv.OnReceiveAIS += new EventHandler<ReceiveAISEventArgs>(Cmd_OnReceiveAIS);
            cmdSrv.OnIndividualCall += new EventHandler<IndividualCallEventArgs>(CmdServer_OnIndividualCall);
            cmdSrv.OnIndividualCallAck += new EventHandler<IndividualCallAckEventArgs>(CmdServer_OnIndividualCallAck);
            cmdSrv.OnUnknownDSCMessage += new EventHandler<UnknownDSCMessageEventArgs>(CmdServer_OnUnknownDSCMessage);
            cmdSrv.OnPositionRequest += new EventHandler<JsonRpcEventArgs>(CmdServer_OnPositionRequest);
            cmdSrv.OnPositionReply += new EventHandler<PositionReplyEventArgs>(CmdServer_OnPositionReply);
            cmdSrv.OnPollRequest += new EventHandler<JsonRpcEventArgs>(CmdServer_OnPollRequest);
            cmdSrv.OnPollReply += new EventHandler<JsonRpcEventArgs>(CmdServer_OnPollReply);
            cmdSrv.OnTestCall += new EventHandler<JsonRpcEventArgs>(CmdServer_OnTestCall);
            cmdSrv.OnTestAck += new EventHandler<JsonRpcEventArgs>(CmdServer_OnTestAck);
            cmdSrv.OnAllShipsCall += new EventHandler<AllShipsCallEventArgs>(CmdServer_OnAllShipsCall);
            cmdSrv.OnDistressAck += new EventHandler<JsonRpcEventArgs>(cmdSrv_OnDistressAck);
            cmdSrv.OnScanRequest += new EventHandler<ScanRequestEventArgs>(cmdSrv_OnScanRequest);
            cmdSrv.OnScanReply += new EventHandler<ScanReplyEventArgs>(cmdSrv_OnScanReply);
            cmdSrv.OnVhfAnnounce += new EventHandler<VhfAnnounceEventArgs>(cmdSrv_OnVhfAnnounce);
            rtp.OnChannelChanged += new EventHandler<ChannelChangeEventArgs>(Rtp_OnChannelChanged);
            rtp.OnCaptureError += new EventHandler<AudioCaptureExceptionEventArgs>(Rtp_OnCaptureError);
            rtp.OnBroadcastBegin += new EventHandler(Rtp_OnBroadcastBegin);
            GPSUnit.Instance.OnUpdate += new EventHandler<GPSUnitUpdateEventArgs>(Gps_OnUpdate);
            cmdDispatcher.BeginTX += new EventHandler(CmdDispatcher_BeginRX);
            cmdDispatcher.EndTX += new EventHandler(CmdDispatcher_EndRX);
            notifyIcon.ShowBalloonTip(6, "ICM-411 Marine VHF/DSC Radio (Icom®)", "This model is powered on by turning the volume knob right. You can do this by clicking it.", ToolTipIcon.Info);
        }

        private void Rtp_OnBroadcastBegin(object sender, EventArgs e)
        {
            if (power)
                cmdDispatcher.Dispatch(DSCRequests.VhfAnnounce(rtp.Channel));
        }

        void cmdSrv_OnVhfAnnounce(object sender, VhfAnnounceEventArgs e)
        {
            if (power && e.ActiveChannel == watchChannel)
            {
                int watchChannelTmp = watchChannel;
                watchChannel = rtp.Channel;
                rtp.Channel = watchChannelTmp;
            }
        }

        private int chanScanResult = -1;

        private void cmdSrv_OnScanReply(object sender, ScanReplyEventArgs e)
        {
            if (chanScanResult < 0 || (e.ActiveChannel < chanScanResult && chanScanResult > rtp.Channel) || (chanScanResult > e.ActiveChannel && chanScanResult < rtp.Channel))
                chanScanResult = e.ActiveChannel;
        }

        private void cmdSrv_OnScanRequest(object sender, ScanRequestEventArgs e)
        {
            if (power && !rtp.Stopped && rtp.Channel > 0 && rtp.Channel != e.CurrentChannel)
                cmdDispatcher.Dispatch(DSCRequests.ScanReply(e.Mmsi, rtp.Channel));
        }

        private delegate void Rtp_OnCaptureErrorDelegate(object sender, AudioCaptureExceptionEventArgs e);
        private void Rtp_OnCaptureError(object sender, AudioCaptureExceptionEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Rtp_OnCaptureErrorDelegate(this.Rtp_OnCaptureError), sender, e);
            }
            else
            {
                MessageBox.Show(e.Exception.Message, "Audio capture error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                talkButton.IsHeld = false;
            }
        }
        private delegate void CmdServer_OnTestAckDelegate(object sender, JsonRpcEventArgs e);
        private void CmdServer_OnTestAck(object sender, JsonRpcEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CmdServer_OnTestAckDelegate(this.CmdServer_OnTestAck), sender, e);
            }
            else if (power)
            {
                NormalOperation();
                rightTopLabel.Text = "RCV TEST ACK";
                dscIndicatorLabel.Visible = true;
                entButton.AddMouseClickHandler(new MouseEventHandler(this.RecvTestAck_Accept));
                AddClearReturnNormal();
            }
        }

        private void RecvTestAck_Accept(object sender, MouseEventArgs e)
        {
            NormalOperation();
            ChangeChannel(68);
        }

        private delegate void CmdServer_OnAllShipsCallDelegate(object sender, AllShipsCallEventArgs e);
        void CmdServer_OnAllShipsCall(object sender, AllShipsCallEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CmdServer_OnAllShipsCallDelegate(this.CmdServer_OnAllShipsCall), sender, e);
            }
            else if (power)
            {
                NormalOperation(false);
                rightTopLabel.Text = "RCV ALL SHIPS";
                BeepNotification.Instance.Enabled = IsEmergencyCategory(e.Category);
                dscIndicatorLabel.Visible = true;
                entButton.AddMouseClickHandler(new MouseEventHandler(this.AllShipsCall_SelectChan16));
                AddClearReturnNormal();
            }
        }

        private bool IsEmergencyCategory(string category)
        {
            return category.Equals(ProtocolConstants.CATEGORY_DISTRESS) || category.Equals(ProtocolConstants.CATEGORY_URGENCY);
        }

        private void AllShipsCall_SelectChan16(object sender, MouseEventArgs e)
        {
            NormalOperation();
            SelectChannel16();
        }

        private delegate void CmdServer_OnTestCallDelegate(object sender, JsonRpcEventArgs e);
        void CmdServer_OnTestCall(object sender, JsonRpcEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CmdServer_OnTestCallDelegate(this.CmdServer_OnTestCall), sender, e);
            }
            else if (power)
            {
                NormalOperation(false);
                dscEnvelope.TargetMmsi = e.Mmsi;
                dscIndicatorLabel.Visible = true;
                mainMenu.AddCycleItem(new CyclingMenuItem("TEST ACK", new MouseEventHandler(this.MenuItem_TestAck)));
                rightTopLabel.Text = "RCV TEST CALL";
                BeepNotification.Instance.Start();
                entButton.AddMouseClickHandler(new MouseEventHandler(this.RecvTestCall_Accept));
                AddClearReturnNormal();
            }
        }

        private void RecvTestCall_Accept(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.TestAck(dscEnvelope.TargetMmsi));
            NormalOperation();
            ChangeChannel(68);
        }

        private void MenuItem_TestAck(object sender, EventArgs e)
        {
            ShowRecipientPrompt(new MouseEventHandler(this.TestAck_NamedAccept), new MouseEventHandler(this.TestAck_ManualInput));
        }

        private void TestAck_ManualInput(object sender, MouseEventArgs e)
        {
            BeginMMSI_Input(new MouseEventHandler(this.TestAck_ManualAccept));
        }

        private void TestAck_ManualAccept(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = MmsiResult();
            ShowReadyPrompt(new MouseEventHandler(this.TestAck_PostAccept));
        }

        private void TestAck_NamedAccept(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = IndvLookup();
            ShowReadyPrompt(new MouseEventHandler(this.TestAck_PostAccept));
        }

        private void TestAck_PostAccept(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.TestAck(dscEnvelope.TargetMmsi));
            NormalOperation();
        }

        private long IndvLookup()
        {
            return indvAddresses.LookupName(menuButton.CurrentItem.Name);
        }

        private long MmsiResult()
        {
            return long.Parse(lcdInput);
        }

        private void DelayedDispatch(JsonObject payload)
        {
            Timer t = new Timer();
            Random rand = new Random();
            t.Interval = rand.Next(1500, 4500);
            t.Tick += new EventHandler(t_Tick);
            t.Tag = payload;
            t.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            Timer t = (Timer)sender;
            t.Stop();
            cmdDispatcher.Dispatch((JsonObject)t.Tag);
            t.Dispose();
        }

        private delegate void CmdServer_OnPollRequestDelegate(object sedner, JsonRpcEventArgs e);
        private void CmdServer_OnPollRequest(object sender, JsonRpcEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CmdServer_OnPollRequestDelegate(this.CmdServer_OnPollRequest), sender, e);
            }
            else if (power)
            {
                NormalOperation(false);
                dscIndicatorLabel.Visible = true;
                BeepNotification.Instance.Start();
                rightTopLabel.Text = "RCV POLL REQUEST";
                if (ConfSingleton.Instance.AutoAck)
                {
                    DelayedDispatch(DSCRequests.PollReply(e.Mmsi));
                    AddEntReturnToNormal();
                }
                else
                {
                    dscEnvelope.TargetMmsi = e.Mmsi;
                    entButton.AddMouseClickHandler(new MouseEventHandler(this.PollReply_PostReady));
                }
                AddClearReturnNormal();
                mainMenu.AddCycleItem(new CyclingMenuItem("POLL REPLY", new MouseEventHandler(this.MenuItem_PollReply)));
            }
        }

        private void MenuItem_TestCall(object sender, MouseEventArgs e)
        {
            ShowRecipientPrompt(new MouseEventHandler(this.TestCall_NamedAccept), new MouseEventHandler(this.TestCall_ManualInput));
        }

        private void TestCall_NamedAccept(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = IndvLookup();
            ShowReadyPrompt(new MouseEventHandler(this.TestCall_PostReady));
        }

        private void ShowReadyPrompt(MouseEventHandler handler)
        {
            NormalOperation();
            rightTopLabel.Text = "READY";
            entButton.AddMouseClickHandler(handler);
        }

        private void TestCall_ManualInput(object sender, MouseEventArgs e)
        {
            BeginMMSI_Input(new MouseEventHandler(this.TestCall_AcceptManual));
        }

        private void TestCall_AcceptManual(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = MmsiResult();
            ShowReadyPrompt(new MouseEventHandler(this.TestCall_PostReady));
        }

        private void TestCall_PostReady(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.TestCall(dscEnvelope.TargetMmsi));
            NormalOperation();
        }

        private void MenuItem_PollReply(object sender, MouseEventArgs e)
        {
            ShowRecipientPrompt(new MouseEventHandler(this.PollReply_NamedInput), new MouseEventHandler(this.PollReply_ManualInput));
        }

        private void PollReply_NamedInput(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = IndvLookup();
            ShowReadyPrompt(new MouseEventHandler(this.PollReply_PostReady));
        }

        private void PollReply_ManualInput(object sender, MouseEventArgs e)
        {
            NormalOperation();
            BeginMMSI_Input(new MouseEventHandler(this.PollReply_ManualAccept));
        }

        private void PollReply_ManualAccept(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = MmsiResult();
            ShowReadyPrompt(new MouseEventHandler(this.PollReply_PostReady));
        }

        private void PollReply_PostReady(object sender, MouseEventArgs PollReply_PostReady)
        {
            cmdDispatcher.Dispatch(DSCRequests.PollReply(dscEnvelope.TargetMmsi));
            NormalOperation();
        }

        private delegate void CmdServer_OnPositionRequestDelegate(object sender, JsonRpcEventArgs e);
        void CmdServer_OnPositionRequest(object sender, JsonRpcEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CmdServer_OnPositionRequestDelegate(this.CmdServer_OnPositionRequest), sender, e);
            }
            else if (power)
            {
                NormalOperation(false);
                dscIndicatorLabel.Visible = true;
                BeepNotification.Instance.Start();
                rightTopLabel.Text = "RCV POS REQUEST";
                if (ConfSingleton.Instance.AutoAck)
                {
                    AddEntReturnToNormal();
                    DelayedDispatch(DSCRequests.PositionReply(e.Mmsi));
                }
                else
                {
                    dscEnvelope.TargetMmsi = e.Mmsi;
                    entButton.AddMouseClickHandler(new MouseEventHandler(this.PosReply_ManualRespond));
                }
                AddClearReturnNormal();
                mainMenu.AddCycleItem(new CyclingMenuItem("POS REPLY", new MouseEventHandler(this.MenuItem_PosReply)));
            }
        }

        private void PosReply_ManualRespond(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.PositionReply(dscEnvelope.TargetMmsi));
            NormalOperation();
        }

        private void AddEntReturnToNormal()
        {
            entButton.AddMouseClickHandler(new MouseEventHandler(this.ReturnToNormalOpMouseEvent));
        }

        private void ShowRecipientPrompt(MouseEventHandler namedHandler, MouseEventHandler manualHandler)
        {
            menuButton.NewCycleHandlerSet();
            foreach (AddressRecord record in this.indvAddresses)
            {
                menuButton.AddCycleItem(record.Name, namedHandler);
            }
            menuButton.AddCycleItem("MANUAL INPUT", manualHandler);
            menuButton.ForceCycleEvent();
        }

        private void MenuItem_PosReply(object sender, MouseEventArgs e)
        {
            ShowRecipientPrompt(new MouseEventHandler(this.PosReply_NamedAccept), new MouseEventHandler(this.PosReply_ManualInput));
        }

        private void PosReply_NamedAccept(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.PositionReply(IndvLookup()));
            NormalOperation();
        }

        private void PosReply_ManualInput(object sender, MouseEventArgs e)
        {
            BeginMMSI_Input(new MouseEventHandler(this.PosReply_ManualAccept));
        }

        private void PosReply_ManualAccept(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.PositionReply(MmsiResult()));
            NormalOperation();
        }
        
        #region UI Setup
        private void NormalOperation()
        {
            NormalOperation(true);
        }

        private void NormalOperation(bool stopBeep)
        {
            lock (this)
            {
                rightTopLabel.Text = "";
                posReplyIndicatorLabel.Visible = intIndicatorLabel.Visible
                    = rightTopLabel.Blinking = dscIndicatorLabel.Visible
                    = tagIndicatorLabel.Visible = false;
                if (stopBeep)
                    BeepNotification.Instance.Enabled = false;
                entButton.ClearMouseClickHandlers();
                dualChButton.ClearMouseClickHandlers();
                sixteenButton.RollbackHandlersTo(1);
                clearButton.ClearMouseClickHandlers();
                menuButton.ClearMouseClickHandlers();
                menuButton.RollbackCycleHandlersTo(1);
                chanUpButton.RollbackHandlersTo(1);
                chanDownButton.RollbackHandlersTo(1);
                handsetChanDown.RollbackHandlersTo(1);
                handsetChanUp.RollbackHandlersTo(1);
                scanButton.RollbackHandlersTo(1);
                scanButton.DisableMouseDownTimer();
                if (rtp.Channel < 0)
                {
                    SelectChannel16();
                }
                else
                {
                    //channelLabel.AddChannel(rtp.Channel);
                    //channelLabel.Text = rtp.Channel.ToString();
                    rtp.RaiseUpdateEvent();
                }
                outgoingDistress.Stop();
                GPSUnit.Instance.RaiseUpdateEvent();
            }
        }

        private bool IsRightLcdInNormalOperation()
        {
            return (IsFormattedGpsData(rightTopLabel.WholeText) && !posReplyIndicatorLabel.Visible) || rightTopLabel.WholeText.Equals("");
        }

        private bool IsFormattedGpsData(string s)
        {
            try
            {
                int i = s.IndexOf('`');
                if (s != null && i > 1)
                {
                    Double.Parse(s.Substring(0, i-1));
                    Double.Parse(s.Substring(i + 1, s.Length - i - 2));
                    return true;
                }
            }
            catch { }
            return false;
        }
        #endregion
        void menuButton_CyclingMenuClick(object sender, EventArgs e)
        {
            if (IsRightLcdInNormalOperation())
            {
                BindChanUpDownMenuCycle();
                menuButton.ForceCycleEvent();
            }
            else
            {
                NormalOperation();
            }
        }

        private void ChanUpMenuCycle(object sender, MouseEventArgs e)
        {
            menuButton.CycleUp();
        }

        private void ChanDownMenuCycle(object sender, MouseEventArgs e)
        {
            menuButton.CycleDown();
        }

        private void ReturnToNormalOpMouseEvent(object sender, MouseEventArgs e)
        {
            NormalOperation();
        }

        private delegate void CmdDispatcher_EndRXDelegate(object sender, EventArgs e);
        private void CmdDispatcher_EndRX(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(new CmdDispatcher_EndRXDelegate(this.CmdDispatcher_EndRX), sender, e);
                }
                catch { }
            }
            else if (power)
            {
                txIndicatorLabel.Visible = false;
            }
        }

        private delegate void CmdDispatcher_BeginRXDelegate(object sender, EventArgs e);
        private void CmdDispatcher_BeginRX(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CmdDispatcher_BeginRXDelegate(this.CmdDispatcher_BeginRX), sender, e);
            }
            else if (power)
            {
                txIndicatorLabel.Visible = true;
            }
        }

        private delegate void CmdServer_OnUnknownDSCMessageDelegate(object sender, UnknownDSCMessageEventArgs e);
        void CmdServer_OnUnknownDSCMessage(object sender, UnknownDSCMessageEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CmdServer_OnUnknownDSCMessageDelegate(this.CmdServer_OnUnknownDSCMessage), sender, e);
            }
            else if (power)
            {
                MessageBox.Show(e.Message, "Unknown DSC Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region DSC Menu -> Individual
        private delegate void CmdServer_OnIndividualCallAckDelegate(object sender, IndividualCallAckEventArgs e);
        private void CmdServer_OnIndividualCallAck(object sender, IndividualCallAckEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CmdServer_OnIndividualCallAckDelegate(this.CmdServer_OnIndividualCallAck), sender, e);
            }
            else if (power)
            {
                NormalOperation();
                if (e.Able)
                {
                    rightTopLabel.Text = "RCV ABLE ACK";
                    entButton.AddMouseClickHandler(new MouseEventHandler(this.Individual_AcceptAck));
                }
                else
                {
                    rightTopLabel.Text = "RCV UNABLE ACK: " + e.AckReason;
                    AddEntReturnToNormal();
                }
                AddClearReturnNormal();
                dscIndicatorLabel.Visible = true;
                dscEnvelope.TargetMmsi = e.TargetMmsi;
                dscEnvelope.TargetChan = e.TargetChan;
            }
        }

        private void Individual_AcceptAck(object sender, MouseEventArgs e)
        {
            NormalOperation();
            ChangeChannel(dscEnvelope.TargetChan);
        }

        private delegate void CmdServer_OnIndividualCallDelegate(object sender, IndividualCallEventArgs e);
        void CmdServer_OnIndividualCall(object sender, IndividualCallEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CmdServer_OnIndividualCallDelegate(this.CmdServer_OnIndividualCall), sender, e);
            }
            else if (power)
            {
                NormalOperation(false);
                rightTopLabel.Text = "RCV INDIVIDUAL";
                dscIndicatorLabel.Visible = true;
                dscEnvelope.TargetChan = e.TargetChan;
                dscEnvelope.TargetMmsi = e.Mmsi;
                entButton.AddMouseClickHandler(new MouseEventHandler(this.IndvAck_AbleUnablePrompt));
                AddClearReturnNormal();
                menuButton.AddCycleItem("INDV ACK", new MouseEventHandler(this.MenuItem_IndvAck));
                BeepNotification.Instance.Enabled = IsEmergencyCategory(e.Category);
            }
        }

        private void AddClearReturnNormal()
        {
            clearButton.AddMouseClickHandler(new MouseEventHandler(this.ReturnToNormalOpMouseEvent));
        }

        private void BindChanUpDownMenuCycle()
        {
            chanUpButton.AddMouseClickHandler(new MouseEventHandler(this.ChanUpMenuCycle));
            chanDownButton.AddMouseClickHandler(new MouseEventHandler(this.ChanDownMenuCycle));
        }

        void IndvAck_AbleUnablePrompt(object sender, MouseEventArgs e)
        {
            menuButton.NewCycleHandlerSet();
            BindChanUpDownMenuCycle();
            menuButton.AddCycleItem("ABLE", new MouseEventHandler(this.IndvAck_AbleUnableSelection));
            menuButton.AddCycleItem("UNABLE", new MouseEventHandler(this.IndvAck_AbleUnableSelection));
            menuButton.ForceCycleEvent();
        }

        private void IndvAck_AbleUnableSelection(object sender, MouseEventArgs e)
        {
            tmp = menuButton.CurrentItem.Name.Equals("ABLE");
            ShowReadyPrompt(new MouseEventHandler(this.IndvAck_PostReady));
        }

        private void IndvAck_PostReady(object sender, MouseEventArgs e)
        {
            bool able = (bool)tmp;
            if (able)
            {
                ChangeChannel(dscEnvelope.TargetChan);
            }
            cmdDispatcher.Dispatch(DSCRequests.IndvAck(dscEnvelope.TargetMmsi, dscEnvelope.TargetChan, able, able ? null : "No Reason Given"));
            NormalOperation();
        }
        #endregion

        private void MenuCycled(object sender, CyclingMenuItem e)
        {
            rightTopLabel.Text = e.Name;
            rightTopLabel.Blinking = false;
            entButton.AddMouseClickHandler(e.Handler);
        }

        private void Individual_DestinationPrompt_ManualInput(object sender, MouseEventArgs e)
        {
            BeginMMSI_Input(new MouseEventHandler(this.Individual_DestinationPrompt_ManualAccept));
        }

        private void Individual_DestinationPrompt_ManualAccept(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = MmsiResult();
            ShowIntershipChanPrompt(new MouseEventHandler(this.Individual_CallOnChannel));
        }

        private void ShowIntershipChanPrompt(MouseEventHandler handler)
        {
            menuButton.NewCycleHandlerSet();
            BindChanUpDownMenuCycle();
            for (int i = 1; i <= 8; i++)
            {
                menuButton.AddCycleItem("CH0" + i, handler, i);
            }
            menuButton.ForceCycleEvent();
        }

        private void Individual_CallOnChannel(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.IndividualCall(dscEnvelope.TargetMmsi, (int)menuButton.CurrentItem.Tag, ProtocolConstants.CATEGORY_ROUTINE));
            NormalOperation();
            rightTopLabel.Text = "WAITING FOR ACK";
        }

        private void Individual_DestinationPrompt_Accept(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = IndvLookup();
            ShowIntershipChanPrompt(new MouseEventHandler(this.Individual_CallOnChannel));
        }

        #region Distress

        private void distressButton_MouseDown(object sender, MouseEventArgs e)
        {
            if (power && distressButton.BackgroundImage == null && !rightTopLabel.WholeText.Equals("PUSH DISTRESS") && !rightTopLabel.WholeText.Equals("DSC REPEAT"))
            {
                rightTopLabel.Blinking = true;
                rightTopLabel.Text = ProtocolConstants.CATEGORY_DISTRESS;
            }
        }
        
        private void distressButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (power && distressButton.BackgroundImage != null && (rightTopLabel.WholeText.Equals(ProtocolConstants.CATEGORY_DISTRESS) || rightTopLabel.LastText.Equals(ProtocolConstants.CATEGORY_DISTRESS)))
                rightTopLabel.Revert();
        }

        private void DistressPressedThreeSeconds(object sender, EventArgs e)
        {
            if (power && distressButton.BackgroundImage == null)
                this.outgoingDistress.Enabled = true;
        }

        private void outgoingDistress_DistressEnded(object sender, EventArgs e)
        {
            GPSUnit.Instance.ResumeDrift();
            if (power && rightTopLabel.WholeText.Equals("DSC REPEAT"))
            {
                NormalOperation();
                if (rtp.Channel == 70)
                    ChangeChannel(16);
            }

        }

        private void outgoingDistress_TimeToSendDistress(object sender, TimeToSendDistressEventArgs e)
        {
            if (power)
                cmdDispatcher.Dispatch(DSCRequests.Distress(e.Nature));
        }

        private void outgoingDistress_DistressStarted(object sender, EventArgs e)
        {
            GPSUnit.Instance.SuspendDrift();
            NormalOperation();
            rightTopLabel.Text = "DSC REPEAT";
            rightTopLabel.Blinking = false;
            AddEntAndClearReturnNormal();
            ChangeChannel(70);
        }

        private void MenuItem_Distress(object sender, MouseEventArgs e)
        {
            menuButton.NewCycleHandlerSet();
            foreach (string nature in OutgoingDistress.DISTRESS_NATURES)
            {
                menuButton.AddCycleItem(nature, new MouseEventHandler(this.DistressNatureSelected));
            }
            menuButton.ForceCycleEvent();
        }

        private void DistressNatureSelected(object sender, MouseEventArgs e)
        {
            this.outgoingDistress.Nature = rightTopLabel.WholeText;
            NormalOperation();
            if (!outgoingDistress.Enabled)
            {
                rightTopLabel.Text = "PUSH DISTRESS";
                AddClearReturnNormal();
            }
            else
            {
                outgoingDistress.Enabled = true;
            }
        }

        private void distressPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if ((rightTopLabel.WholeText.Equals("PUSH DISTRESS") || rightTopLabel.LastText.Equals("PUSH DISTRESS")) && distressButton.BackgroundImage == null)
            {
                outgoingDistress.Enabled = true;
                distressButton.BackgroundImage = global::MarineRadioRTP.Properties.Resources.covered_distress;
            }
            else if (distressButton.BackgroundImage != null)
            {
                distressButton.BackgroundImage = null;
            }
            else
            {
                distressButton.BackgroundImage = global::MarineRadioRTP.Properties.Resources.covered_distress;
            }
        }
        #endregion

        #region GPS
        private delegate void Gps_OnUpdateDelegate(object sender, GPSUnitUpdateEventArgs e);
        private void Gps_OnUpdate(object sender, GPSUnitUpdateEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Gps_OnUpdateDelegate(this.Gps_OnUpdate), sender, e);
            }
            else if (power)
            {
                cmdDispatcher.Dispatch(DSCRequests.AIS(e.Location));
                lock (this)
                {
                    if (IsRightLcdInNormalOperation())
                    {
                        try
                        {
                            rightTopLabel.Text = e.Location.ToString();
                        }
                        catch { }
                        gpsIndicatorLabel.Visible = true;
                    }
                }
            }
        }
        #endregion

        #region Uninteresting UI events
        private void ICM411_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.O)
            {
                OpenConfDialog();
            }
            else if (e.Control && e.KeyCode == Keys.R)
            {
                OpenRadar();
            }
            else if (e.Shift && e.Control && e.KeyCode == Keys.D)
            {
                DscDebugger dscDebugger = DscDebugger.GetInstance(cmdSrv, cmdDispatcher);
                dscDebugger.Show();
                dscDebugger.BringToFront();
            }
        }
        #endregion

        #region Command server events
        private delegate void cmd_OnDistressDelegate(object sender, DistressEventArgs e);
        private void cmd_OnDistress(object sender, DistressEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new cmd_OnDistressDelegate(this.cmd_OnDistress), sender, e);
            }
            else if (power)
            {
                NormalOperation(false);
                dscIndicatorLabel.Visible = true;
                dscEnvelope.TargetMmsi = e.Mmsi;
                rightTopLabel.Text = "RCV DISTRESS: " + e.Nature;
                AddClearReturnNormal();
                entButton.AddMouseClickHandler(new MouseEventHandler(this.RcvDistress_SendAck));
                BeepNotification.Instance.Start();
                SelectChannel16();
            }
        }

        private void RcvDistress_SendAck(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.DistressAck(dscEnvelope.TargetMmsi));
            NormalOperation();
        }

        private delegate void Rtp_OnChannelChangedDelegate(object sender, ChannelChangeEventArgs e);
        private void Rtp_OnChannelChanged(object sender, ChannelChangeEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Rtp_OnChannelChangedDelegate(this.Rtp_OnChannelChanged), sender, e);
            }
            else if (power)
            {
                lock (this)
                {
                    SetChannelLabel(e.Channel);
                    intIndicatorLabel.Visible = true;
                    if (rtp.Stopped && talkButton.IsHeld)
                        rtp.RtpStartTalking();
                }
                Trace.WriteLine("Rtp_OnChannelChanged(): " + e.Channel);
            }
        }


        private void SetChannelLabel(int chan)
        {
            if (rtp.IsInRange(chan))
            {
                channelLabel.Text = chan.ToString();
                tagIndicatorLabel.Visible = taggedChannels.Contains(chan);
            }
        }

        #endregion

        #region Power On/Off
        private void pictureBox1_KnobChange(object sender, KnobChangeEventArgs e)
        {
            if (e.OldKnobIndex < 1 && e.NewKnobIndex > 0)
            {
                // power on
                this.Power = true;
            }
            else if (e.OldKnobIndex > 0 && e.NewKnobIndex < 1)
            {
                // power off
                this.Power = false;
            }
            //AudioMixerHelper.SetVolume(100 / 7 * e.NewKnobIndex);
        }

        private bool Power
        {
            get
            {
                return this.power;
            }
            set
            {
                this.power = value;
                if (value)
                {
                    PowerOn();
                }
                else
                {
                    PowerOff();
                }
            }
        }

        private void PowerOff()
        {
            lock (this)
            {
                menuButton.IsHoldable = sixteenButton.IsHoldable = true;
                entButton.ClearMouseClickHandlers();
                chanUpButton.ClearMouseClickHandlers();
                chanDownButton.ClearMouseClickHandlers();
                handsetChanDown.ClearMouseClickHandlers();
                handsetChanUp.ClearMouseClickHandlers();
                dualChButton.ClearMouseClickHandlers();
                sixteenButton.ClearMouseClickHandlers();
                menuButton.ClearMouseClickHandlers();
                clearButton.ClearMouseClickHandlers();
                scanButton.ClearMouseClickHandlers();
                scanButton.DisableMouseDownTimer();
                menuButton.ClearCycleItems();
                menuButton.DisableMouseDownTimer();
                distressButton.DisableMouseDownTimer();
                outgoingDistress.Enabled = gpsIndicatorLabel.Visible = rightTopLabel.Blinking
                    = dscIndicatorLabel.Visible = intIndicatorLabel.Visible = txIndicatorLabel.Visible
                    = posReplyIndicatorLabel.Visible = tagIndicatorLabel.Visible
                    = talkButton.IsHeld = false;
                lcdPanel.BackColor = Color.DimGray;
                rtp.DropRTPSession();
                cmdSrv.DscLog.Clear();
                aisReg.Clear();
                RadarWindow.CloseIfOpen();
                BeepNotification.Instance.Enabled = false;
                rightTopLabel.Text = channelLabel.Text = "";
                button2.Enabled = false;
            }
        }

        private void PowerOn()
        {
            menuButton.AddTimedMouseClickHandler(1000, new EventHandler(this.MMSICodeCheck));
            distressButton.AddTimedMouseClickHandler(3000, new EventHandler(this.DistressPressedThreeSeconds));
            lcdPanel.BackColor = Color.Transparent;
            if (menuButton.IsHeld)
            {
                menuButton.AddCycleItem("MMSI", new MouseEventHandler(this.MenuItem_InputMMSI));
            }
            menuButton.AddCycleItem(ProtocolConstants.CATEGORY_DISTRESS, new MouseEventHandler(this.MenuItem_Distress));
            menuButton.AddCycleItem("ADDRESS", new MouseEventHandler(this.MenuItem_Address));
            menuButton.AddCycleItem("INDIVIDUAL", new MouseEventHandler(this.MenuItem_Individual));
            menuButton.AddCycleItem("POS REQUEST", new MouseEventHandler(this.MenuItem_PosRequest));
            menuButton.AddCycleItem("POLL REQUEST", new MouseEventHandler(this.MenuItem_PollRequest));
            menuButton.AddCycleItem("TEST CALL", new MouseEventHandler(this.MenuItem_TestCall));
            menuButton.AddCycleItem("ALL SHIPS", new MouseEventHandler(this.MenuItem_AllShips));
            menuButton.AddCycleItem("AUTO ACK", new MouseEventHandler(this.MenuItem_AutoAck));
            menuButton.AddCycleItem("DSC LOG", new MouseEventHandler(this.MenuItem_DscLog));
            sixteenButton.AddMouseClickHandler(new MouseEventHandler(this.SixteenButtonHandler));
            chanUpButton.AddMouseClickHandler(new MouseEventHandler(this.ChanUpButton_NormalOp));
            handsetChanUp.AddMouseClickHandler(new MouseEventHandler(this.ChanUpButton_NormalOp));
            chanDownButton.AddMouseClickHandler(new MouseEventHandler(this.ChanDownButton_NormalOp));
            handsetChanDown.AddMouseClickHandler(new MouseEventHandler(this.ChanDownButton_NormalOp));
            scanButton.AddMouseClickHandler(new MouseEventHandler(this.ScanButton_Scan));
            this.mainMenu = menuButton.CurrentCycleList;
            this.menuButton.IsHeld = false;
            button2.Enabled = true;
            menuButton.IsHoldable = sixteenButton.IsHoldable = false;
            NormalOperation();
            if (sixteenButton.IsHeld)
            {
                sixteenButton.IsHeld = false;
            }
        }

        private void SixteenButtonHandler(object sender, MouseEventArgs e)
        {
            SelectChannel16();
        }

        private void ScanButton_Scan(object sender, MouseEventArgs e)
        {
            if (!rightTopLabel.WholeText.Equals("SCAN"))
            {
                NormalOperation();
                rightTopLabel.Text = "SCAN";
                rightTopLabel.Blinking = true;
                chanScanResult = -1;
                cmdDispatcher.Dispatch(DSCRequests.ScanRequest(rtp.Channel));
                Timer waitForReplyTimer = new Timer();
                waitForReplyTimer.Interval = 2000;
                waitForReplyTimer.Tick += new EventHandler(waitForReplyTimer_Tick);
                waitForReplyTimer.Start();
            }
        }

        void waitForReplyTimer_Tick(object sender, EventArgs e)
        {
            Timer waitForReplyTimer = (Timer)sender;
            waitForReplyTimer.Stop();
            if (rightTopLabel.WholeText == "SCAN")
            {
                Timer cycleTimer = new Timer();
                cycleTimer.Tag = rtp.Channel;
                cycleTimer.Interval = 200;
                cycleTimer.Tick += new EventHandler(cycleTimer_Tick);
                cycleTimer.Start();
            }
            waitForReplyTimer.Dispose();
        }

        void cycleTimer_Tick(object sender, EventArgs e)
        {
            Timer cycleTimer = (Timer)sender;
            int chan = (int)cycleTimer.Tag + 1;
            /* Scan was cancelled */
            if (!rightTopLabel.WholeText.Equals("SCAN"))
            {
                cycleTimer.Stop();
                return;
            }
            else if (chan > ConfSingleton.Instance.Network.LogicalChannels.Length - 1)
            {
                cycleTimer.Tag = 0;
            }
            else if (chan == rtp.Channel)
            {
                cycleTimer.Stop();
                NormalOperation();
                return;
            }
            else
            {
                cycleTimer.Tag = chan;
            }
            if (chan.Equals(chanScanResult))
            {
                cycleTimer.Stop();
                ChangeChannel(chan);
                cycleTimer.Dispose();
                NormalOperation();
            }
            SetChannelLabel(chan);
        }

        private Timer deferredChanChangeTimer;

        private void ChanUpButton_NormalOp(object sender, MouseEventArgs e)
        {
            if (chanDownButton.IsHeld || handsetChanDown.IsHeld)
            {
                chanDownButton.IsHeld = handsetChanDown.IsHeld = false;
                StartTagSet();
            }
            else
            {
                try
                {
                    DeferredChanChange(GetLcdChannel() +1);
                } catch {}
            }
        }

        private void DeferredChanChange(int chan)
        {
            if (power && IsRightLcdInNormalOperation() && rtp.IsInRange(chan))
            {
                deferredChanChangeTimer.Stop();
                SetChannelLabel(chan);
                deferredChanChangeTimer.Start();
            }
        }

        void deferredChanChangeTimer_Tick(object sender, EventArgs e)
        {
            deferredChanChangeTimer.Stop();
            if (power)
            {
                try
                {
                    ChangeChannel(GetLcdChannel());
                }
                catch { } 
            }
        }

        private void ChanUpLcdOnly(object sender, MouseEventArgs e)
        {
            SetChannelLabel(GetLcdChannel() + 1);
        }

        private void ChanDownLcdOnly(object sender, MouseEventArgs e)
        {
            SetChannelLabel(GetLcdChannel() - 1);
        }

        private void StartTagSet()
        {
            chanUpButton.AddMouseClickHandler(new MouseEventHandler(this.ChanUpLcdOnly));
            chanDownButton.AddMouseClickHandler(new MouseEventHandler(this.ChanDownLcdOnly));
            rightTopLabel.Text = "TAG SET";
            rightTopLabel.Blinking = true;
            scanButton.AddTimedMouseClickHandler(1000, new EventHandler(this.TagSet));
            scanButton.AddMouseClickHandler(new MouseEventHandler(this.NullMouseHandler));
            clearButton.AddMouseClickHandler(new MouseEventHandler(this.ReturnToNormalOpMouseEvent));
        }

        private int GetLcdChannel()
        {
            return int.Parse(channelLabel.Text);
        }

        private void TagSet(object sender, EventArgs e)
        {
            int chan = GetLcdChannel();
            if (taggedChannels.Contains(chan))
            {
                tagIndicatorLabel.Visible = false;
                taggedChannels.Remove(chan);
            }
            else
            {
                tagIndicatorLabel.Visible = true;
                taggedChannels.Add(chan);
            }
        }

        private void ChanDownButton_NormalOp(object sender, MouseEventArgs e)
        {
            if (chanUpButton.IsHeld || handsetChanUp.IsHeld)
            {
                chanUpButton.IsHeld = handsetChanUp.IsHeld = false;
                StartTagSet();
            }
            else
            {
                try
                {
                    DeferredChanChange(GetLcdChannel() - 1);
                }
                catch { }
            }
        }

        private void DscLog_AcceptCategory(object sender, MouseEventArgs e)
        {
            string[] results = cmdSrv.DscLog.GetByCategory(menuButton.CurrentItem.Name);
            menuButton.NewCycleHandlerSet();
            for (int i = 0; i < results.Length; i++)
            {
                menuButton.AddCycleItem(String.Format("#{0} {1}", i+1, results[i]), new MouseEventHandler(this.NullMouseHandler));
            }
            menuButton.ForceCycleEvent();
        }

        private void MenuItem_DscLog(object sender, MouseEventArgs e)
        {
            if (cmdSrv.DscLog.Count > 0)
            {
                menuButton.NewCycleHandlerSet();
                foreach (string s in cmdSrv.DscLog.Categories)
                {
                    menuButton.AddCycleItem(s, new MouseEventHandler(this.DscLog_AcceptCategory));
                }
                menuButton.ForceCycleEvent();
            }
            else
            {
                NormalOperation();
                TimedLcdText(2000, "LOG EMPTY");
            }
        }

        private void MenuItem_AutoAck(object sender, MouseEventArgs e)
        {
            menuButton.NewCycleHandlerSet();
            menuButton.AddCycleItem(ConfSingleton.Instance.AutoAck ? "ON" : "OFF", new MouseEventHandler(this.AutoAck_AcceptOnOff));
            menuButton.AddCycleItem(ConfSingleton.Instance.AutoAck ? "OFF" : "ON", new MouseEventHandler(this.AutoAck_AcceptOnOff));
            menuButton.ForceCycleEvent();
        }

        private void AutoAck_AcceptOnOff(object sender, MouseEventArgs e)
        {
            ConfSingleton.Instance.AutoAck = menuButton.CurrentItem.Name.Equals("ON");
            NormalOperation();
        }

        private void MenuItem_AllShips(object sender, MouseEventArgs e)
        {
            menuButton.NewCycleHandlerSet();
            menuButton.AddCycleItem(ProtocolConstants.CATEGORY_ROUTINE, new MouseEventHandler(this.AllShips_AcceptCategory));
            menuButton.AddCycleItem(ProtocolConstants.CATEGORY_SAFETY, new MouseEventHandler(this.AllShips_AcceptCategory));
            menuButton.AddCycleItem(ProtocolConstants.CATEGORY_DISTRESS, new MouseEventHandler(this.AllShips_AcceptCategory));
            menuButton.AddCycleItem(ProtocolConstants.CATEGORY_URGENCY, new MouseEventHandler(this.AllShips_AcceptCategory));
            menuButton.ForceCycleEvent();
        }

        private void AllShips_AcceptCategory(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.AllShipsCall(menuButton.CurrentItem.Name));
            NormalOperation();
            SelectChannel16();
        }

        private void SelectChannel16()
        {
            ChangeChannel(16);
        }

        private void MenuItem_PollRequest(object sender, MouseEventArgs e)
        {
            ShowRecipientPrompt(new MouseEventHandler(this.PollRequest_NamedAccept), new MouseEventHandler(this.PollRequest_BeginManualInput));
        }

        private void PollRequest_NamedAccept(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.PollRequest(IndvLookup()));
            NormalOperation();
        }

        private void PollRequest_BeginManualInput(object sender, MouseEventArgs e)
        {
            BeginMMSI_Input(new MouseEventHandler(this.PollRequest_ManualAccept));
        }

        private void PollRequest_ManualAccept(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.PollRequest(MmsiResult()));
            NormalOperation();
        }

        private void MenuItem_PosRequest(object sender, MouseEventArgs e)
        {
            ShowRecipientPrompt(new MouseEventHandler(this.PosRequest_AcceptNamed), new MouseEventHandler(this.PosRequest_ManualInput));
        }

        private void PosRequest_ManualInput(object sender, MouseEventArgs e)
        {
            BeginMMSI_Input(new MouseEventHandler(this.PosRequest_AcceptManual));
        }

        private void PosRequest_AcceptManual(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = MmsiResult();
            ShowReadyPrompt(new MouseEventHandler(this.PosRequest_PostReady));
        }

        private void PosRequest_AcceptNamed(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = IndvLookup();
            ShowReadyPrompt(new MouseEventHandler(this.PosRequest_PostReady));
        }

        private void PosRequest_PostReady(object sender, MouseEventArgs e)
        {
            cmdDispatcher.Dispatch(DSCRequests.PositionRequest(dscEnvelope.TargetMmsi));
            NormalOperation();
        }

        private void MenuItem_IndvAck(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetChan = -1;
            ShowRecipientPrompt(new MouseEventHandler(this.IndvAck_NameAccept), new MouseEventHandler(this.IndvAck_ManualInput));
        }

        private void IndvAck_ManualInput(object sender, MouseEventArgs e)
        {
            BeginMMSI_Input(new MouseEventHandler(this.IndvAck_ManualAccept));
        }

        private void IndvAck_ManualAccept(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = MmsiResult();
            IndvAck_AbleUnablePrompt(sender, e);
        }

        private void IndvAck_NameAccept(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = IndvLookup();
            IndvAck_AbleUnablePrompt(sender, e);
        }

        #endregion

        #region Menu Buttons
        private void MenuItem_Individual(object sender, MouseEventArgs e)
        {
            ShowRecipientPrompt(new MouseEventHandler(this.Individual_DestinationPrompt_Accept), new MouseEventHandler(this.Individual_DestinationPrompt_ManualInput));
        }

        private void BeginMMSI_Input(MouseEventHandler entHandler)
        {
            this.cursor = 0;
            sixteenButton.AddMouseClickHandler(new MouseEventHandler(this.FixedInputContext_16C));
            dualChButton.AddMouseClickHandler(new MouseEventHandler(this.FixedInputContext_DualCh));
            chanUpButton.AddMouseClickHandler(new MouseEventHandler(this.InputMMSI_ChanUp));
            chanDownButton.AddMouseClickHandler(new MouseEventHandler(this.InputMMSI_ChanDown));
            entButton.AddMouseClickHandler(entHandler);
            this.lcdInput = "123456789";
            ShowLcdInputText();
        }

        private void MenuItem_InputMMSI(object sender, MouseEventArgs e)
        {
            BeginMMSI_Input(new MouseEventHandler(this.InputMMSI_Ent_ConfirmMessage));
        }

        private void MenuItem_Address(object sender, MouseEventArgs e)
        {
            menuButton.NewCycleHandlerSet();
            menuButton.AddCycleItem("ADD INDV ID", new MouseEventHandler(this.Address_AddIndv));
            menuButton.AddCycleItem("DEL INDV ID", new MouseEventHandler(this.Address_DelIndv));
            menuButton.AddCycleItem("ADD GROUP ID", new MouseEventHandler(this.Address_AddGroup));
            menuButton.AddCycleItem("DEL GROUP ID", new MouseEventHandler(this.Address_DelGroup));
            menuButton.ForceCycleEvent();
        }

        private void NullMouseHandler(object sender, MouseEventArgs e)
        {
        }

        private void Address_AddIndv(object sender, MouseEventArgs e)
        {
            BeginMMSI_Input(new MouseEventHandler(this.Address_AddIndv_AcceptMMSI));
        }

        private const char
            NAME_ASCII_START = '0',
            NAME_ASCII_END = 'Z';

        private void InputName_ChanUp(object sender, MouseEventArgs e)
        {
            char[] c = lcdInput.ToCharArray();
            if (c[cursor] >= NAME_ASCII_END)
                c[cursor] = NAME_ASCII_START;
            else
                c[cursor]++;
            lcdInput = new string(c);
            ShowLcdInputText();
        }

        private void InputName_ChanDown(object sender, MouseEventArgs e)
        {
            char[] c = lcdInput.ToCharArray();
            if (c[cursor] <= NAME_ASCII_START)
                c[cursor] = NAME_ASCII_END;
            else
                c[cursor]--;
            lcdInput = new string(c);
            ShowLcdInputText();
        }

        private void VariableInputContext_Clear(object sender, MouseEventArgs e)
        {
            if (cursor - 1 >= 0)
            {
                lcdInput = lcdInput.Remove(cursor--);
            }
            ShowLcdInputText();
        }

        private void BeginNameInput()
        {
            sixteenButton.AddMouseClickHandler(new MouseEventHandler(this.VariableInputContext_16C));
            dualChButton.AddMouseClickHandler(new MouseEventHandler(this.VariableInputContext_DualCh));
            chanUpButton.AddMouseClickHandler(new MouseEventHandler(this.InputName_ChanUp));
            chanDownButton.AddMouseClickHandler(new MouseEventHandler(this.InputName_ChanDown));
            clearButton.AddMouseClickHandler(new MouseEventHandler(this.VariableInputContext_Clear));
            lcdInput = "A";
            ShowLcdInputText();
        }

        private void Address_AddIndv_AcceptMMSI(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = long.Parse(this.lcdInput);
            entButton.AddMouseClickHandler(new MouseEventHandler(this.Address_AddIndv_AcceptName));
            BeginNameInput();
        }

        private void Address_AddIndv_AcceptName(object sender, MouseEventArgs e)
        {
            NormalOperation();
            try
            {
                this.indvAddresses.AddEntry(new AddressRecord(this.lcdInput, dscEnvelope.TargetMmsi));
                TimedLcdText(2000, "SAVED");
            }
            catch (Exception ex)
            {
                TimedLcdText(2000, ex.Message);
            }
        }

        private void Address_DelIndv(object sender, MouseEventArgs e)
        {
            NormalOperation();
            if (indvAddresses.Count > 0)
            {
                menuButton.NewCycleHandlerSet();
                foreach (AddressRecord record in this.indvAddresses)
                {
                    menuButton.AddCycleItem(record.Name, new MouseEventHandler(this.Address_DelIndv_Confirm));
                }
                menuButton.ForceCycleEvent();
            }
            else
            {
                TimedLcdText(2000, "NO ADDRESSES");
            }
        }

        private void Address_DelIndv_Confirm(object sender, MouseEventArgs e)
        {
            this.lcdInputPreConfirm = rightTopLabel.WholeText;
            ShowReadyPrompt(new MouseEventHandler(this.Address_DelIndv_DoConfirm));
        }

        private void Address_DelIndv_DoConfirm(object sender, MouseEventArgs e)
        {
            indvAddresses.RemoveAddress(this.lcdInputPreConfirm);
            NormalOperation();
            TimedLcdText(2000, "REMOVED");
        }

        private void Address_AddGroup(object sender, MouseEventArgs e)
        {
            BeginMMSI_Input(new MouseEventHandler(this.Address_AddGroup_AcceptMMSI));
        }

        private void Address_AddGroup_AcceptMMSI(object sender, MouseEventArgs e)
        {
            dscEnvelope.TargetMmsi = long.Parse(this.lcdInput);
            entButton.AddMouseClickHandler(new MouseEventHandler(this.Address_AddGroup_AcceptName));
            BeginNameInput();
        }

        private void Address_AddGroup_AcceptName(object sender, MouseEventArgs e)
        {
            this.groupAddresses.Add(new AddressRecord(this.lcdInput, dscEnvelope.TargetMmsi));
            NormalOperation();
            TimedLcdText(2000, "SAVED");
        }

        private void Address_DelGroup(object sender, MouseEventArgs e)
        {
            if (indvAddresses.Count > 0)
            {
                menuButton.NewCycleHandlerSet();
                foreach (AddressRecord record in this.groupAddresses)
                {
                    menuButton.AddCycleItem(record.Name, new MouseEventHandler(this.Address_DelGroup_Confirm));
                }
                menuButton.ForceCycleEvent();
            }
            else
            {
                NormalOperation();
                TimedLcdText(2000, "NO GROUPS");
            }
        }

        private void Address_DelGroup_Confirm(object sender, MouseEventArgs e)
        {
            this.lcdInputPreConfirm = rightTopLabel.WholeText;
            ShowReadyPrompt(new MouseEventHandler(this.Address_DelGroup_DoConfirm));
        }

        private void Address_DelGroup_DoConfirm(object sender, MouseEventArgs e)
        {
            groupAddresses.RemoveAddress(this.lcdInputPreConfirm);
            NormalOperation();
            TimedLcdText(2000, "REMOVED");
        }
        #endregion

        #region MMSI Stuff

        private void MMSICodeCheck(object sender, EventArgs e)
        {
            if (power)
            {
                ConfSingleton settings = ConfSingleton.Instance;
                if (settings.MMSI > 0)
                {
                    rightTopLabel.Text = "MMSI " + settings.MMSI;
                }
                else
                {
                    rightTopLabel.Text = "NO MMSI";
                }
            }
        }

        private void DecrementLcdCursor()
        {
            if (this.cursor - 1 >= 0)
                this.cursor--;
        }

        private void IncrementLcdCursor(bool resize)
        {
            if (++cursor >= lcdInput.Length)
            {
                if (resize)
                    lcdInput = lcdInput + "A";
                else
                    cursor = 0;
            }
        }

        private void ShowLcdInputText()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lcdInput.Length; i++)
            {
                if (cursor.Equals(i))
                {
                    sb.Append('[');
                }
                sb.Append(lcdInput[i]);
                if (cursor.Equals(i))
                {
                    sb.Append(']');
                }
            }
            rightTopLabel.Text = sb.ToString();
        }

        private void FixedInputContext_16C(object sender, MouseEventArgs e)
        {
            IncrementLcdCursor(false);
            ShowLcdInputText();
        }

        private void FixedInputContext_DualCh(object sender, MouseEventArgs e)
        {
            DecrementLcdCursor();
            ShowLcdInputText();
        }

        private void VariableInputContext_16C(object sender, MouseEventArgs e)
        {
            IncrementLcdCursor(true);
            ShowLcdInputText();
        }

        private void VariableInputContext_DualCh(object sender, MouseEventArgs e)
        {
            DecrementLcdCursor();
            ShowLcdInputText();
        }

        private void InputMMSI_ChanUp(object sender, MouseEventArgs e)
        {
            char[] c = lcdInput.ToCharArray();
            int i = Int32.Parse(lcdInput[cursor].ToString()) + 1;
            if (i > 9)
                i = 0;
            c[cursor] = i.ToString()[0];
            lcdInput = new string(c);
            ShowLcdInputText();
        }

        private void InputMMSI_ChanDown(object sender, MouseEventArgs e)
        {
            char[] c = lcdInput.ToCharArray();
            int i = Int32.Parse(lcdInput[cursor].ToString()) - 1;
            if (i < 1)
                i = 9;
            c[cursor] = i.ToString()[0];
            lcdInput = new string(c);
            ShowLcdInputText();
        }

        private void InputMMSI_Ent_ConfirmMessage(object sender, MouseEventArgs e)
        {
            entButton.AddMouseClickHandler(new MouseEventHandler(this.InputMMSI_Ent_BeginConfirm));
            rightTopLabel.Text = "CONFIRMATION";
        }

        private void InputMMSI_Ent_BeginConfirm(object sender, MouseEventArgs e)
        {
            entButton.AddMouseClickHandler(new MouseEventHandler(this.InputMMSI_Ent_DoConfirm));
            this.lcdInputPreConfirm = this.lcdInput;
            this.lcdInput = "123456789";
            this.cursor = 0;
            ShowLcdInputText();
        }

        private void InputMMSI_Ent_DoConfirm(object sender, MouseEventArgs e)
        {
            NormalOperation();
            if (this.lcdInputPreConfirm.Equals(this.lcdInput))
            {
                ConfSingleton.Instance.MMSI = MmsiResult();
                TimedLcdText(2000, "SAVED");
            }
            else
            {
                TimedLcdText(2000, "INCORRECT");
            }
            this.lcdInput = this.lcdInputPreConfirm = null;
        }

        void clearRightLcdTimer_Tick(object sender, EventArgs e)
        {
            Timer t = sender as Timer;
            if (rightTopLabel.WholeText.Equals((string)t.Tag))
                NormalOperation();
            BeepNotification.Instance.Enabled = false;
            t.Dispose();
        }

        #endregion

        private void changeChannelBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int chan = (int)e.Argument;
            if (rtp.IsInRange(chan))
                rtp.Channel = (int)e.Argument;
        }

        private delegate void cmdSrv_OnDistressAckDelegate(object sender, JsonRpcEventArgs e);
        void cmdSrv_OnDistressAck(object sender, JsonRpcEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new cmdSrv_OnDistressAckDelegate(this.cmdSrv_OnDistressAck), sender, e);
            }
            else if (power)
            {
                NormalOperation();
                rightTopLabel.Text = "RCV DISTRESS ACK";
                AddEntAndClearReturnNormal();
                outgoingDistress.Stop();
                SelectChannel16();
            }
        }

        private void ChangeChannel(int channel)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(changeChannelBackgroundWorker_DoWork);
            bw.RunWorkerAsync(channel);
        }

        private delegate void CmdServer_OnPollReplyDelegate(object sender, JsonRpcEventArgs e);
        private void CmdServer_OnPollReply(object sender, JsonRpcEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CmdServer_OnPollReplyDelegate(this.CmdServer_OnPollReply), sender, e);
            }
            else if (power)
            {
                NormalOperation(false);
                dscIndicatorLabel.Visible = true;
                rightTopLabel.Text = "RCV POLL REPLY";
                BeepNotification.Instance.Start();
                AddEntAndClearReturnNormal();
            }
        }

        private void AddEntAndClearReturnNormal()
        {
            AddEntReturnToNormal();
            AddClearReturnNormal();
        }

        private void TimedLcdText(int interval, string text)
        {
            rightTopLabel.Text = text;
            AddEntReturnToNormal();
            Timer timer = new Timer();
            timer.Tag = text;
            timer.Interval = interval;
            timer.Tick += new EventHandler(clearRightLcdTimer_Tick);
            timer.Enabled = true;
        }

        private delegate void CmdServer_OnPositionReplyDelegate(object sender, PositionReplyEventArgs e);
        void CmdServer_OnPositionReply(object sender, PositionReplyEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CmdServer_OnPositionReplyDelegate(this.CmdServer_OnPositionReply), sender, e);
            }
            else if (power)
            {
                NormalOperation(false);
                BeepNotification.Instance.Start();
                dscIndicatorLabel.Visible = posReplyIndicatorLabel.Visible = true;
                rightTopLabel.Text = e.GpsLocation.ToString();
                AddEntAndClearReturnNormal();
            }
        }

        void clearPosReplyIndicatorTimer_Tick(object sender, EventArgs e)
        {
            Timer t = (Timer)sender;
            t.Stop();
            posReplyIndicatorLabel.Visible = false;
            GPSUnit.Instance.RaiseUpdateEvent();
            t.Dispose();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenConfDialog();
        }

        private void OpenConfDialog()
        {
            ConfigurationDialog.Instance.Show();
            ConfigurationDialog.Instance.BringToFront();
        }

        private void OpenRadar()
        {
            if (power)
            {
                RadarWindow radar = RadarWindow.GetInstance(aisReg);
                radar.Show();
                radar.BringToFront();
            }
        }

        private void showAISRadarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenRadar();
        }

        private void ICM411_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConfSingleton.Instance.Save();
        }

        private void marineRadioButton4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (talkButton.IsHeld && rtp.Stopped)
                {
                    rtp.RtpStartTalking();
                }
                else if (!talkButton.IsHeld && !rtp.Stopped)
                {
                    rtp.StopTalking();
                }
            }
        }
    }
}
