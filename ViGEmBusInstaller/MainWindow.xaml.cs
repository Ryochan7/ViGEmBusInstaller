using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ViGEmBusInstaller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DriverInstaller installer = new DriverInstaller();

        public MainWindow()
        {
            InitializeComponent();
            AppLogger.LogEvent += (message) =>
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    LogMsgBox.AppendText($"{DateTime.Now.ToString()}: {message}\n");
                    progressLb.Content = message.TrimEnd();
                }));
            };

            driverProgressBar.DataContext = installer;
            installBtn.IsEnabled = false;
            uninstallBtn.IsEnabled = false;
            InitialDriverCheck();
            LogMsgBox.ScrollToEnd();
            //installer.Run();
        }

        private async void InitialDriverCheck()
        {
            LogMsgBox.AppendText($"{DateTime.Now.ToString()}: Checking if ViGEmBus is already installed\n");
            await Task.Run(() => installer.CheckInstall());
            bool installed = installer.IsInstalled();
            installBtn.IsEnabled = !installed;
            uninstallBtn.IsEnabled = installed;
            if (installed)
            {
                LogMsgBox.AppendText($"{DateTime.Now.ToString()}: ViGEmBus is already installed\n");
            }
            else
            {
                LogMsgBox.AppendText($"{DateTime.Now.ToString()}: ViGEmBus is not installed\n");
            }
        }

        private void InstallButtonClick(object sender, RoutedEventArgs e)
        {
            installBtn.IsEnabled = false;

            installer.RunFinished += PostInstallCheck;
            Task.Run(() => installer.Run());
        }

        private void PostInstallCheck(object sender, EventArgs args)
        {
            installer.RunFinished -= PostInstallCheck;

            Dispatcher.BeginInvoke((Action)(() =>
            {
                bool installed = installer.IsInstalled();
                installBtn.IsEnabled = !installed;
                InstallCheckRefresh();
            }));
        }

        private void LogMsgBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            LogMsgBox.ScrollToEnd();
        }

        private void InstallCheckRefresh()
        {
            installer.CheckInstall();
            bool installed = installer.IsInstalled();
            installBtn.IsEnabled = !installed;
            uninstallBtn.IsEnabled = installed;
        }

        private void UninstallBtn_Click(object sender, RoutedEventArgs e)
        {
            uninstallBtn.IsEnabled = false;
            UninstallDriver();
        }

        private async void UninstallDriver()
        {
            await Task.Run(() =>
            {
                installer.Uninstall();
            });

            InstallCheckRefresh();
        }
    }
}
