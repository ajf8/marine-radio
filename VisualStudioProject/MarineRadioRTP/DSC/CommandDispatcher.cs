using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using Jayrock.Json;
using System.Timers;

namespace MarineRadioRTP
{
    public class CommandDispatcher
    {
        public event EventHandler<CommandDispatchedEventArgs> OnCmdDispatched;
        public event EventHandler BeginTX;
        public event EventHandler EndTX;

        private Socket sock;
        private Timer timer;
        private string uid;

        public CommandDispatcher(string uid)
        {
            this.uid = uid;
            this.sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.timer = new Timer();
            this.timer.Interval = 200;
            this.timer.Enabled = false;
            this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        public void Close()
        {
            this.sock.Close();
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            (sender as Timer).Stop();
            if (EndTX != null)
                EndTX(this, new EventArgs());
        }

        public void Dispatch(JsonObject data)
        {
            JsonObject args = (JsonObject)data[ProtocolConstants.KEY_ARGUMENTS];
            args.Put(ProtocolConstants.ARG_UID, uid);
            if ((string)data[ProtocolConstants.KEY_METHOD] != ProtocolConstants.METHOD_AIS)
            {
                Trace.WriteLine(String.Format("==> [{0}] {1}", DateTime.Now, data));
                if (OnCmdDispatched != null)
                    OnCmdDispatched(this, new CommandDispatchedEventArgs() { Request = data });
            }
            this.Dispatch(data.ToString());
        }

        public void Dispatch(string data)
        {
            this.Dispatch(UTF8Encoding.UTF8.GetBytes(data));
        }

        public void Dispatch(byte[] data)
        {
            if (BeginTX != null)
                BeginTX(this, new EventArgs());
            sock.BeginSendTo(data, 0, data.Length, SocketFlags.None, MulticastCommandServer.MulticastEP, new AsyncCallback(OnSend), null);
        }

        public void OnSend(IAsyncResult ar)
        {
            this.timer.Stop();
            this.timer.Start();
            try
            {
                sock.EndSend(ar);
            }
            catch { }
        }
    }
}
