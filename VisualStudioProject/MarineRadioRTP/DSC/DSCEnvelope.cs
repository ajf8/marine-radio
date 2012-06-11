using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarineRadioRTP
{
    class DscEnvelope
    {
        public int TargetChan { get; set; }
        public long TargetMmsi { get; set; }
        public bool Able { get; set; }
    }
}
