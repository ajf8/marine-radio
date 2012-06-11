using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;

namespace MarineRadioRTP
{
    public class GPSUnit
    {
        public event EventHandler<GPSUnitUpdateEventArgs> OnUpdate;

        public double DriftFactor { get; set; }
        public GpsLocation Location { get; set; }
        private Timer timer;

        private static GPSUnit instance = null;
        private static readonly object padlock = new object();

        public static GPSUnit Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GPSUnit();
                    }
                    return instance;
                }
            }
        }

        private GPSUnit()
        {
            this.DriftFactor = 1;
            this.Location = new GpsLocation(59.98, 60.11);
            this.timer = new Timer();
            this.DriftInterval = 8 * 1000;
            this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            this.timer.Start();
        }

        public double DriftInterval
        {
            get
            {
                return this.timer.Interval;
            }
            set
            {
                this.timer.Interval = value;
            }
        }

        public void Drift()
        {
            Location.Drift(this.DriftFactor);
            RaiseUpdateEvent();
        }

        public void RaiseUpdateEvent()
        {
            if (OnUpdate != null)
                OnUpdate(this, new GPSUnitUpdateEventArgs() { Location = this.Location });
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Drift();
        }

        public void SuspendDrift()
        {
            this.timer.Enabled = false;
        }

        public void ResumeDrift()
        {
            this.timer.Enabled = true;
        }

        public bool IsBroadcasting
        {
            get
            {
                return this.timer.Enabled;
            }
        }
    }
}
