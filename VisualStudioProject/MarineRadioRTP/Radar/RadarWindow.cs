using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Blaney;
using System.Diagnostics;

namespace MarineRadioRTP
{
    public partial class RadarWindow : Form
    {
        private const int ITEM_SIZE = 8;

        private Radar _radar;
        private Dictionary<string, RadarItem> itemRegister;
        private AISRegistry aisReg;

        private RadarWindow(AISRegistry aisReg)
        {
            this.aisReg = aisReg;
            this.itemRegister = new Dictionary<string, RadarItem>();
            InitializeComponent();
        }

        void Registry_OnAISNodeUpdate(object sender, AISNodeUpdateEventArgs e)
        {
            GpsLocation loc = e.ReceiveAISEventArgs.GpsLocation;
            UpdateRadians(itemRegister[e.ReceiveAISEventArgs.MachineName], loc);
            itemRegister[e.ReceiveAISEventArgs.MachineName].AisData = e.ReceiveAISEventArgs;
        }

        private const double RADIANS_MAGIC_NUMBER = 57.29578;
        private void UpdateRadians(RadarItem item, GpsLocation remoteLoc)
        {
            GpsLocation localLoc = GPSUnit.Instance.Location;
            /*double g = remoteLoc.Longitude - localLoc.Longitude;
            double grad = g / RADIANS_MAGIC_NUMBER;
            double nrad = localLoc.Longitude / RADIANS_MAGIC_NUMBER;
            double lrad = localLoc.Latitude / RADIANS_MAGIC_NUMBER;
            double srad = remoteLoc.Longitude / RADIANS_MAGIC_NUMBER;
#if TRACE
            Trace.WriteLine(String.Format("localLoc: {0}", localLoc));
            Trace.WriteLine(String.Format("remoteLoc: {0}", remoteLoc));
            Trace.WriteLine(String.Format("grad={0}, nrad={1}, lrad={2}, srad={3}", grad, nrad, lrad, srad));
#endif
            double azi = (Math.PI - (-Math.Atan((Math.Tan(grad) / Math.Sin(lrad)))));
            item.Azimuth = azi * RADIANS_MAGIC_NUMBER;
            double a = Math.Cos(grad);
            double b = Math.Cos(lrad);
            double ele = Math.Atan((a * b) / (Math.Sqrt(1 - (a * a) * (b * b))));
            //item.Elevation = ele > 0 ? ele : -ele * RADIANS_DIVISOR;
            item.Elevation = ele * RADIANS_MAGIC_NUMBER;
#if TRACE
            Trace.WriteLine(String.Format("azi={0},ele={1}", azi, ele));
            Trace.WriteLine(String.Format("item.Azimuth={0}, item.Elevation={1}", item.Azimuth, item.Elevation));
#endif
             */
            item.Azimuth = remoteLoc.Latitude - localLoc.Latitude * 50;
            double y = remoteLoc.Longitude - localLoc.Longitude * 1.5;
            item.Elevation = y < 0 ? -y : y;
        }

        void Registry_OnAISNodeDiscovery(object sender, AISNodeDiscoveryEventArgs e)
        {
            AddNode(e.ReceiveAISEventArgs);
        }

        private void RadarWindow_Load(object sender, EventArgs e)
        {
            aisReg.OnAISNodeDiscovery += new EventHandler<AISNodeDiscoveryEventArgs>(Registry_OnAISNodeDiscovery);
            aisReg.OnAISNodeUpdate += new EventHandler<AISNodeUpdateEventArgs>(Registry_OnAISNodeUpdate);
            _radar = new Radar(pictureBox1.Width);
            pictureBox1.Image = _radar.Image;
            _radar.ImageUpdate += new ImageUpdateHandler(_radar_ImageUpdate);
            _radar.DrawScanInterval = 60;
            _radar.DrawScanLine = true;
            lock (aisReg)
            {
                foreach (KeyValuePair<string, ReceiveAISEventArgs> pair in aisReg)
                {
                    AddNode(pair.Value);
                }
            }
        }

        private void AddNode(ReceiveAISEventArgs e)
        {
            RadarItem item = new RadarItem(e, ITEM_SIZE);
            UpdateRadians(item, e.GpsLocation);
            _radar.AddItem(item);
            lock (itemRegister)
            {
                itemRegister.Add(item.ID, item);
            }
        }

        void _radar_ImageUpdate(object sender, ImageUpdateEventArgs e)
        {
            pictureBox1.Image = e.Image;
        }

        private static RadarWindow instance = null;
        private static readonly object padlock = new object();

        public static RadarWindow GetInstance(AISRegistry aisReg)
        {
            lock (padlock)
            {
                if (instance == null || instance.IsDisposed)
                {
                    instance = new RadarWindow(aisReg);
                }
                return instance;
            }
        }

        public static void CloseIfOpen()
        {
            if (instance != null && !instance.IsDisposed)
                instance.Close();
        }

        private void RadarWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            aisReg.OnAISNodeUpdate -= new EventHandler<AISNodeUpdateEventArgs>(this.Registry_OnAISNodeUpdate);
            aisReg.OnAISNodeDiscovery -= new EventHandler<AISNodeDiscoveryEventArgs>(this.Registry_OnAISNodeDiscovery);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}