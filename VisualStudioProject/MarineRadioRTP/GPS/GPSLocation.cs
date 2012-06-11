using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarineRadioRTP
{
    public class GpsLocation
    {
        private const int ROUNDING = 2;

        public double Longitude { get; set; }
        public double Latitude { get; set; }
        private Random rand;

        public GpsLocation(double longitude, double latitude)
        {
            this.rand = new Random();
            this.Longitude = longitude;
            this.Latitude = latitude;
        }

        public void Drift(double factor)
        {
            double driftX = this.rand.NextDouble();
            double driftY = this.rand.NextDouble();
            if (driftX > 0.5)
                driftX = (-driftX + 0.5);
            if (driftY > 0.5)
                driftY = (-driftY + 0.5);
            this.Longitude += driftX * factor;
            this.Latitude += driftY * factor;
        }

        public override string ToString()
        {
            return String.Format("{0}{1}`{2}{3}", Math.Round(this.Longitude, ROUNDING), this.Longitude > 0 ? "N" : "S", Math.Round(this.Latitude, ROUNDING), this.Latitude > 0 ? "E" : "W");
        }
    }
}
