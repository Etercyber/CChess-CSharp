using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Chess
{
    class Win32API
    {
        private Win32API()
        {

        }

        [DllImport(@"user32.dll")]
        public static extern int SetProcessDPIAware();

        [DllImport(@"kernel32.dll")]
        public static extern void RtlMoveMemory(IntPtr lpDst, IntPtr szSrc, uint dwCount);

        [DllImport(@"oleaut32.dll")]
        public static extern IntPtr SysAllocStringByteLen(IntPtr lpsz, uint dwLen);

        protected const int SND_SYNC = 0x0;
        protected const int SND_ASYNC = 0x1;
        protected const int SND_NODEFAULT = 0x2;
        protected const int SND_MEMORY = 0x4;
        protected const int SND_LOOP = 0x8;
        protected const int SND_NOSTOP = 0x10;
        protected const int SND_NOWAIT = 0x2000;
        protected const int SND_ALIAS = 0x10000;
        protected const int SND_ALIAS_ID = 0x110000;
        protected const int SND_FILENAME = 0x20000;
        protected const int SND_RESOURCE = 0x40004;
        protected const int SND_PURGE = 0x40;
        protected const int SND_APPLICATION = 0x80;

        [DllImport("Winmm.dll", CharSet = CharSet.Auto)]
        protected extern static bool PlaySound(string strFile, IntPtr hMod, int flag);

        public static bool PlaySoundFile(string strSoundFile, bool bSynch)
        {
            int Flags;

            if (!System.IO.File.Exists(strSoundFile))
                return false;
            if (bSynch)
                Flags = SND_FILENAME | SND_SYNC;
            else
                Flags = SND_FILENAME | SND_ASYNC;

            return PlaySound(strSoundFile, IntPtr.Zero, Flags);
        }
    }
}
