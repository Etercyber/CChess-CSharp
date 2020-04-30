using System;
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
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainFrm(new CChess()));
            }
            catch
            {
                MessageBox.Show("程序异常退出");
                Application.ExitThread();
                Application.Exit();
            }
        }
    }
}