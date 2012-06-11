using System;
using System.Runtime.InteropServices;
using System.Resources;
using System.IO;

namespace Win32
{
    public class WinMM
    {
        public const UInt32 SND_ASYNC = 1;
        public const UInt32 SND_MEMORY = 4;

        [DllImport("Winmm.dll")]
        public static extern bool PlaySound(byte[] data, IntPtr hMod, UInt32 dwFlags);
        
        public WinMM()
        {
        }

        public static void PlayWavResource(Stream wav)
        {
            if (wav == null)
                return;
            byte[] bStr = new Byte[wav.Length];
            wav.Read(bStr, 0, (int)wav.Length);
            PlaySound(bStr, IntPtr.Zero, SND_ASYNC | SND_MEMORY);
        }
    }
}