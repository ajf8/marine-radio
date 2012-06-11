using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Timers;

namespace MarineRadioRTP
{
    public class AISRegistry : Dictionary<string, ReceiveAISEventArgs>
    {
        public event EventHandler<AISNodeDiscoveryEventArgs> OnAISNodeDiscovery;
        public event EventHandler<AISNodeUpdateEventArgs> OnAISNodeUpdate;

        public AISRegistry(MulticastCommandServer cmdServer)
        {
            cmdServer.OnReceiveAIS += new EventHandler<ReceiveAISEventArgs>(cmdServer_OnReceiveAIS);
        }

        void cmdServer_OnReceiveAIS(object sender, ReceiveAISEventArgs e)
        {
            if (this.ContainsKey(e.MachineName) && OnAISNodeUpdate != null)
            {
                OnAISNodeUpdate(this, new AISNodeUpdateEventArgs() { ReceiveAISEventArgs = e });
            }
            else if (OnAISNodeDiscovery != null)
            {
                OnAISNodeDiscovery(this, new AISNodeDiscoveryEventArgs() { ReceiveAISEventArgs = e });
            }
            lock (this)
            {
                this[e.MachineName] = e;
            }
        }
    }
}
