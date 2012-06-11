using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarineRadioRTP
{
    public class AudioCaptureException : MarineRadioException
    {
        public AudioCaptureException(string message)
            : base(message)
        {
        }

        public AudioCaptureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class AudioCaptureExceptionEventArgs : EventArgs
    {
        public AudioCaptureException Exception { get; set; }
    }
}
