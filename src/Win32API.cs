using System;
using System.Runtime.InteropServices;

namespace Chess
{
    class Win32API
    {
        [DllImport("kernel32.dll")]
        public static extern void RtlMoveMemory(IntPtr lpDst, IntPtr szSrc, uint dwCount);

        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        public extern static bool PlaySound(string strFile, IntPtr hMod, int flag);
    }
}
