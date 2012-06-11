using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using MarineRadioRTP;

namespace Blaney
{
    public class RadarItem
    {
        public ReceiveAISEventArgs AisData { get; set; }
        private double _azimuth;
        private double _elevation;
        private int _width;
        private int _height;
        private DateTime _created = DateTime.Now;

        public long Mmsi
        {
            get { return AisData.Mmsi; }
        }

        public string ID
        {
            get { return AisData.MachineName; }
        }

        public bool IsRecent
        {
            get { return AisData.IsRecent; }
        }

        public double Azimuth
        {
            get
            {
                return _azimuth;
            }
            set
            {
                _azimuth = value;
                while (_azimuth < 0)
                {
                    _azimuth += 360;
                }
                if (_azimuth >= 360)
                    _azimuth = _azimuth % 360;
            }
        }

        public double Elevation
        {
            get
            {
                return _elevation;
            }
            set
            {
                _elevation = value;
                if (_elevation > 90)
                    _elevation = 90;
                else if (_elevation < 0)
                    _elevation = 0;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
            set
            {
            }
        }
        public int Width
        {
            get
            {
                return _width;
            }
            set
            {
            }
        }
        public DateTime Created
        {
            get
            {
                return _created;
            }
        }

        public RadarItem(ReceiveAISEventArgs aisData, int size)
        {
            AisData = aisData;
            _width = size;
            _height = size;
        }

        public void DrawItem(Radar radar, Graphics g)
        {
            PointF cp = radar.AzEl2XY(_azimuth, _elevation);
            PointF topLeft = new PointF(cp.X - ((float)_width / 2), cp.Y - ((float)_height / 2));
            string itemTag = IsRecent ? this.Mmsi.ToString() : String.Format("{0} ({1})", this.Mmsi, AisData.LocalTime.ToString("HH:mm"));
            g.FillEllipse(new SolidBrush(IsRecent ? radar.CustomLineColor : Color.Gray), new RectangleF(topLeft, new SizeF((float)_width, (float)_height)));
            g.DrawString(itemTag, new Font("Digital-7", 13F), new SolidBrush(IsRecent ? Color.Yellow : Color.Gray), new PointF(topLeft.X + _width + 4, topLeft.Y));
        }

        public int CompareTo(RadarItem item)
        {
            return this.ID.CompareTo(item.ID);
        }
    }
}
