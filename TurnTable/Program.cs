using System;
using System.Windows.Forms;

namespace CytoDx
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                MainWindow mainWindow = new MainWindow();
                mainWindow.config.ReadWriteConfig(RW.READ);

                Application.Run(mainWindow);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString(), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
