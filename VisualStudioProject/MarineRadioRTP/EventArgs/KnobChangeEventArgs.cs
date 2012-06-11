using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarineRadioRTP
{
    class KnobChangeEventArgs : EventArgs
    {
        public int NewKnobIndex;
        public int OldKnobIndex;
    }
}
