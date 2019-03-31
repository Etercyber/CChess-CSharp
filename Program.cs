using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Chess
{
    static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                Control.CheckForIllegalCrossThreadCalls = false;
                // Win32API.SetProcessDPIAware();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainFrm(new CChess()));
            }
            catch
            {
                MessageBox.Show("程序异常退出！", "警告");
                Application.ExitThread();
                Application.Exit();
            }
        }
    }
}