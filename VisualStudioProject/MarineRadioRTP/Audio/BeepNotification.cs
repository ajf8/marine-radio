using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;

namespace MarineRadioRTP
{
    public class BeepNotification
    {
        private const int DEFAULT_INTERVAL = 1000;
        private const int DEFAULT_DURATION = 1000 * 60 * 2; // 2 mins

        private System.Timers.Timer timer;
        public int Interval { get; set; }

        public double Duration
        {
            get { return timer.Interval; }
            set { timer.Interval = value; }
        }

        private BeepNotification()
        {
            this.Interval = DEFAULT_INTERVAL;
            this.timer = new System.Timers.Timer();
            this.timer.Enabled = false;
            this.timer.Interval = DEFAULT_DURATION;
            this.timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        public void Start(int interval)
        {
            this.Duration = interval;
            this.Enabled = true;
        }

        public void Start()
        {
            Start(DEFAULT_DURATION);
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Enabled = false;
        }

        private static BeepNotification instance = null;
        private static readonly object padlock = new object();

        public static BeepNotification Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new BeepNotification();
                    }
                    return instance;
                }
            }
        }

        public bool Enabled
        {
            get { return this.timer.Enabled; }
            set
            {
                if (!timer.Enabled && value && ConfSingleton.Instance.SoundEffects)
                {
                    timer.Enabled = true;
                    ThreadPool.QueueUserWorkItem(new WaitCallback(this.BeepThread));
                }
                else if (timer.Enabled && !value)
                {
                    timer.Enabled = false;
                }
            }
        }

        private void BeepThread(Object state)
        {
            while (timer.Enabled)
            {
                Win32.WinMM.PlayWavResource(global::MarineRadioRTP.Properties.Resources.notifyBeep);
                Thread.Sleep(this.Interval);
            }
        }
    }
}
