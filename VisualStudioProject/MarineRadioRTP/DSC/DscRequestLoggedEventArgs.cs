using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jayrock.Json;

namespace MarineRadioRTP
{
    public class DscRequestLoggedEventArgs : EventArgs
    {
        public JsonObject RequestData { get; set; }
    }
}
