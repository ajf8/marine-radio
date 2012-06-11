using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Diagnostics;

namespace MarineRadioRTP
{
    class OutgoingDistress
    {
        public event EventHandler<TimeToSendDistressEventArgs> TimeToSendDistress;
        public event EventHandler DistressStarted;
        public event EventHandler DistressEnded;

        private RtpAudioFacade rtp;

        public static string[] DISTRESS_NATURES
        {
            get
            {
                return new string[] {
                    "UNDESIGNATED",
                    "EXPLOSION",
                    "FLOODING",
                    "COLLISION",
                    "GROUNDING",
                    "CAPSIZING",
                    "SINKING",
                    "ADRIFT",
                    "ABANDONING",
                    "PIRACY",
                    "MOB" };
            }
        }

        private const int DEFAULT_INTERVAL = 210000; // miliseconds

        public OutgoingDistress(RtpAudioFacade rtp)
            : this(DEFAULT_INTERVAL, rtp)
        {
        }

        public void Start()
        {
            this.Enabled = true;
        }

        public void Stop()
        {
            this.Enabled = false;
        }

        public OutgoingDistress(int interval, RtpAudioFacade rtp)
        {
            this.rtp = rtp;
            this.timer = new Timer();
            timer.Interval = interval;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = false;
            this.Nature = "UNDESIGNATED";
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (TimeToSendDistress != null)
                TimeToSendDistress(this, new TimeToSendDistressEventArgs() { Nature = this.Nature });
        }

        public string Nature { get; set; }

        public double Interval
        {
            get
            {
                return timer.Interval;
            }
            set
            {
                timer.Interval = value;
            }
        }

        private Timer timer;

        public bool Enabled
        {
            get
            {
                return timer.Enabled;
            }
            set
            {   
                if (!timer.Enabled && value)
                {
                    if (DistressStarted != null)
                        DistressStarted(this, new EventArgs());
                    if (TimeToSendDistress != null)
                        TimeToSendDistress(this, new TimeToSendDistressEventArgs() { Nature = this.Nature });
                }
                else if (timer.Enabled && !value)
                {
                    if (DistressEnded != null)
                        DistressEnded(this, new EventArgs());
                }
                timer.Enabled = value;
            }
        }
    }
}
