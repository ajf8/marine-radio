using System;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using MSR.LST;
using MSR.LST.Net.Rtp;
using G711Audio;
using Microsoft.DirectX.DirectSound;
using System.Windows.Forms;
using System.Diagnostics;

namespace MarineRadioRTP
{
    public class RtpAudioFacade
    {
        private const short
            AUDIO_CHANNELS = 1,
            AUDIO_BITS = 16;

        private const int AUDIO_SAMPLES = 16000;

        /* Events */
        public EventHandler<ChannelChangeEventArgs> OnChannelChanged;
        public EventHandler<AudioCaptureExceptionEventArgs> OnCaptureError;
        public EventHandler OnBroadcastBegin;
        public EventHandler OnBroadcastEnd;

        private Control cooperativeForm;
        private int channel = -1;
        private bool bStop = true;
        public bool Stopped { get { return this.bStop; } }
        private int bufferSize;
        private Thread th_senderThread;
        private RtpSession rtpSession;
        private RtpSender rtpSender;
        private CaptureBufferDescription captureBufferDescription;
        private AutoResetEvent autoResetEvent;
        private Notify notify;
        private WaveFormat waveFormat;
        //private Capture capture;
        private CaptureBuffer captureBuffer;
        private Device device;
        private SecondaryBuffer playbackBuffer;
        private BufferDescription playbackBufferDescription;
        private MulticastNetwork network;

        public bool IsInRange(int chan)
        {
            return (chan > 0 && chan <= network.LogicalChannels.Length - 1);
        }

        public int Channel
        {
            get
            {
                return this.channel;
            }
            set
            {
                lock (this)
                {
                    if (IsInRange(value) && value != channel)
                    {
                        int oldChannel = this.channel;
                        if (rtpSession != null)
                            DropRTPSession();
                        this.channel = value;
                        JoinRTPSession(true);
                        if (OnChannelChanged != null)
                            OnChannelChanged(this, new ChannelChangeEventArgs() { OldChannel = oldChannel, Channel = value, Endpoint = network.GetChannel(value) });
                    }
                }
            }
        }

        public void RaiseUpdateEvent()
        {
            if (OnChannelChanged != null)
                OnChannelChanged(this, new ChannelChangeEventArgs() { OldChannel = channel, Channel = channel, Endpoint = network.GetChannel(channel) });
        }

        public RtpAudioFacade(MulticastNetwork network)
        {
            this.network = network;
        }

        private void RegisterRtpUserEvents()
        {
            RtpEvents.RtpParticipantAdded += new RtpEvents.RtpParticipantAddedEventHandler(RtpParticipantAdded);
            RtpEvents.RtpParticipantRemoved += new RtpEvents.RtpParticipantRemovedEventHandler(RtpParticipantRemoved);
            RtpEvents.PacketOutOfSequence += new RtpEvents.PacketOutOfSequenceEventHandler(RtpEvents_PacketOutOfSequence);
        }

        void RtpEvents_PacketOutOfSequence(object sender, RtpEvents.PacketOutOfSequenceEventArgs ea)
        {
            Trace.WriteLine(String.Format("{0}: out of sequence packet from {1}", DateTime.Now, ea.RtpStream.Properties.CName));
        }

        private void RegisterRtpStreamEvents()
        {
            RtpEvents.RtpStreamAdded += new RtpEvents.RtpStreamAddedEventHandler(RtpStreamAdded);
            RtpEvents.RtpStreamRemoved += new RtpEvents.RtpStreamRemovedEventHandler(RtpStreamRemoved);
        }

        private void RtpParticipantAdded(object sender, RtpEvents.RtpParticipantEventArgs ea)
        {
            Trace.WriteLine(ea.RtpParticipant.Name + " has joined the multicast RTP session.");
        }

        private void RtpParticipantRemoved(object sender, RtpEvents.RtpParticipantEventArgs ea)
        {
            Trace.WriteLine(ea.RtpParticipant.Name + " has left the multicast RTP session.");
        }

        private void RtpStreamAdded(object sender, RtpEvents.RtpStreamEventArgs ea)
        {
            ea.RtpStream.FrameReceived += new RtpStream.FrameReceivedEventHandler(FrameReceived);
        }

        private void RtpStreamRemoved(object sender, RtpEvents.RtpStreamEventArgs ea)
        {
            ea.RtpStream.FrameReceived -= new RtpStream.FrameReceivedEventHandler(FrameReceived);
        }

        private void FrameReceived(object sender, RtpStream.FrameReceivedEventArgs ea)
        {
            PlayFrame(ea.Frame.Buffer);
            Trace.WriteLine(String.Format("{0}: frame {1} received. lost={2}, len={3}", DateTime.Now, ea.RtpStream.FramesReceived, ea.RtpStream.FramesLost, ea.Frame.Length));
        }

        private void UnregisterRtpEvents()
        {
            RtpEvents.RtpParticipantAdded -= new RtpEvents.RtpParticipantAddedEventHandler(RtpParticipantAdded);
            RtpEvents.RtpParticipantRemoved -= new RtpEvents.RtpParticipantRemovedEventHandler(RtpParticipantRemoved);
            RtpEvents.RtpStreamAdded -= new RtpEvents.RtpStreamAddedEventHandler(RtpStreamAdded);
            RtpEvents.RtpStreamRemoved -= new RtpEvents.RtpStreamRemovedEventHandler(RtpStreamRemoved);
            RtpEvents.PacketOutOfSequence -= new RtpEvents.PacketOutOfSequenceEventHandler(RtpEvents_PacketOutOfSequence);
        }

        private void DisposeRtpSession()
        {
            if (rtpSession != null)
            {
                rtpSession.Dispose();
                rtpSession = null;
                rtpSender = null;
            }
        }

        public void JoinRTPSession(bool receive)
        {
            if (receive)
            {
                RegisterRtpUserEvents();
                RegisterRtpStreamEvents();
            }
            rtpSession = new RtpSession(network.GetChannel(channel), new RtpParticipant(Environment.MachineName, Environment.MachineName), true, receive);
            rtpSender = rtpSession.CreateRtpSender("VOIP Listener", PayloadType.dynamicAudio, null);
        }

        public void DropRTPSession()
        {
            StopTalking();
            UnregisterRtpEvents();
            DisposeRtpSession();
            channel = -1;
        }

        private void StartRecordAndSend()
        {
            try
            {
                Capture capture = null;
                CaptureDevicesCollection captureDeviceCollection = new CaptureDevicesCollection();
                try
                {
                    capture = new Capture(captureDeviceCollection[ConfSingleton.Instance.CaptureDeviceIndex].DriverGuid);
                }
                catch
                {
                    capture = new Capture(captureDeviceCollection[0].DriverGuid);
                }
                captureBuffer = new CaptureBuffer(captureBufferDescription, capture);
                SetBufferEvents();
                int halfBuffer = bufferSize / 2;
                captureBuffer.Start(true);
                bool readFirstBufferPart = true;
                int offset = 0;
                MemoryStream memStream = new MemoryStream(halfBuffer);
                bStop = false;
                while (!bStop)
                {
                    autoResetEvent.WaitOne();
                    memStream.Seek(0, SeekOrigin.Begin);
                    captureBuffer.Read(offset, memStream, halfBuffer, LockFlag.None);
                    readFirstBufferPart = !readFirstBufferPart;
                    offset = readFirstBufferPart ? 0 : halfBuffer;
                    rtpSender.Send(ConfSingleton.Instance.Compression ? ALawEncoder.ALawEncode(memStream.GetBuffer()) : memStream.GetBuffer());
                }
            }
            catch (ThreadAbortException)
            {
                /* This is OK. It's raised when the record thread is stopped. */
            }
            /* Catch DirectSound's uninformative exceptions and attempt to expand on them... */
            catch (Exception ex)
            {
                if (OnCaptureError != null)
                {
                    AudioCaptureException captureException = new AudioCaptureException("There was a problem in the audio capture process. This is often due to no working capture device being available.", ex);
                    OnCaptureError(this, new AudioCaptureExceptionEventArgs() { Exception = captureException });
                }
            }
            finally
            {
                try
                {
                    if (captureBuffer != null)
                        captureBuffer.Stop();
                    bStop = true;
                }
                catch { }
            }
        }

        private void PlayFrame(byte[] data)
        {
            try
            {
                playbackBuffer = new SecondaryBuffer(playbackBufferDescription, device);
                if (ConfSingleton.Instance.Compression)
                {
                    byte[] byteDecodedData = new byte[data.Length * 2];
                    ALawDecoder.ALawDecode(data, out byteDecodedData);
                    playbackBuffer.Write(0, byteDecodedData, LockFlag.None);
                }
                else
                {
                    playbackBuffer.Write(0, data, LockFlag.None);
                }
                playbackBuffer.Play(0, BufferPlayFlags.Default);
            }
            catch (Exception)
            {
                RefreshSoundDevice();
            }
        }

        protected void SetBufferEvents()
        {
            try
            {
                autoResetEvent = new AutoResetEvent(false);
                notify = new Notify(captureBuffer);
                BufferPositionNotify bufferPositionNotify1 = new BufferPositionNotify();
                bufferPositionNotify1.Offset = bufferSize / 2 - 1;
                bufferPositionNotify1.EventNotifyHandle = autoResetEvent.SafeWaitHandle.DangerousGetHandle();
                BufferPositionNotify bufferPositionNotify2 = new BufferPositionNotify();
                bufferPositionNotify2.Offset = bufferSize - 1; //
                bufferPositionNotify2.EventNotifyHandle = autoResetEvent.SafeWaitHandle.DangerousGetHandle();
                notify.SetNotificationPositions(new BufferPositionNotify[] { bufferPositionNotify1, bufferPositionNotify2 }); // The Tow Positions (First & Last) 
            }
            catch (Exception)
            {
            }
        }

        private void RefreshSoundDevice()
        {
            device = new Device();
            device.SetCooperativeLevel(cooperativeForm, CooperativeLevel.Normal);
        }

        public void ConfigureSoundDevice(short channels, Control appForm, short bitsPerSample, int samplesPerSecond)
        {
            this.cooperativeForm = appForm;
            RefreshSoundDevice();
            waveFormat = new WaveFormat();
            waveFormat.Channels = channels;
            waveFormat.FormatTag = WaveFormatTag.Pcm;
            waveFormat.SamplesPerSecond = samplesPerSecond;
            waveFormat.BitsPerSample = bitsPerSample;
            waveFormat.BlockAlign = (short)(channels * (bitsPerSample / (short)8));
            waveFormat.AverageBytesPerSecond = waveFormat.BlockAlign * samplesPerSecond;
            captureBufferDescription = new CaptureBufferDescription();
            captureBufferDescription.BufferBytes = waveFormat.AverageBytesPerSecond / 5;
            captureBufferDescription.Format = waveFormat;
            playbackBufferDescription = new BufferDescription();
            playbackBufferDescription.BufferBytes = waveFormat.AverageBytesPerSecond / 5;
            playbackBufferDescription.Format = waveFormat;
            playbackBuffer = new SecondaryBuffer(playbackBufferDescription, new Device());
            bufferSize = captureBufferDescription.BufferBytes;
        }

        public void SetVoiceDevices(Control appForm)
        {
            ConfigureSoundDevice(AUDIO_CHANNELS, // channels
                appForm, // application form for real time cooperation
                AUDIO_BITS, // bits
                AUDIO_SAMPLES); // samples
        }

        public void RtpStartTalking()
        {
            try
            {
                th_senderThread = new Thread(new ThreadStart(StartRecordAndSend));
                th_senderThread.IsBackground = true;
                th_senderThread.Start();
                if (OnBroadcastBegin != null)
                    OnBroadcastBegin(this, new EventArgs());
            }
            catch { }
        }

        public void StopTalking()
        {
            bStop = true;
            if (th_senderThread != null)
            {
                try
                {
                    th_senderThread.Abort();
                    if (OnBroadcastEnd != null)
                        OnBroadcastEnd(this, new EventArgs());
                }
                catch { }
            }
        }
    }
}