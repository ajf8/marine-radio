using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MarineRadioRTP
{
    public class CyclingMenuItem : EventArgs
    {
        public string Name { get; set; }
        public MouseEventHandler Handler { get; set; }
        public object Tag { get; set; }

        public CyclingMenuItem(string name, MouseEventHandler handler, object tag)
        {
            this.Tag = tag;
            this.Name = name;
            this.Handler = handler;
        }

        public CyclingMenuItem(string name, MouseEventHandler handler)
        {
            this.Name = name;
            this.Handler = handler;
        }
    }
}
