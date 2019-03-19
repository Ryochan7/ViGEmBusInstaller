using System;
using System.Threading.Tasks;
using System.Windows;

namespace ViGEmBusInstaller
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool silentinstall = false;
            foreach (string arg in e.Args)
            {
                if (arg == "--silent")
                {
                    silentinstall = true;
                }
            }

            if (silentinstall)
            {
                DriverInstaller installer = new DriverInstaller();
                installer.CheckInstall();
                if (installer.IsInstalled())
                {
                    Shutdown();
                    return;
                }

                installer.RunFinished += (sender2, args) =>
                {
                    //Console.WriteLine("SHUTDOWN");

                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        Shutdown();
                    }));
                };

                Task.Run(() => installer.Run());
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
            }
        }
    }
}
