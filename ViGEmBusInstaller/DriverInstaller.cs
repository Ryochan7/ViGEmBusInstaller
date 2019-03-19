using Nefarius.Devcon;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace ViGEmBusInstaller
{
    class DriverInstaller
    {
        private double progress;
        public event EventHandler ProgressChanged;
        public double Progress { get => progress;
            set
            {
                if (progress == value) return;
                progress = value;
                ProgressChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private bool vigemInstalled;
        public event EventHandler RunFinished;

        public void CheckInstall()
        {
            vigemInstalled = Util.IsViGEmInstalled();
        }

        public bool IsInstalled()
        {
            return vigemInstalled;
        }

        public async void Run()
        {
            bool deviceCreated = false;

            vigemInstalled = false;
            AppLogger.Log("Start ViGEmBus install");
            Progress = 0.0;

            string archiveName = "ViGEmBus_signed_Win7-10_x86_x64_latest.zip";
            using (WebClient wb = new WebClient())
            {
                AppLogger.Log("Start downloading ViGEmBus archive");
                wb.DownloadProgressChanged += UpdateViGEmDlProgress;
                await wb.DownloadFileTaskAsync(new Uri("https://downloads.vigem.org/stable/latest/windows/x86_64/ViGEmBus_signed_Win7-10_x86_x64_latest.zip"),
                    Path.Combine(Util.exepath, archiveName));
            }
            
            string archivePath = Path.Combine(Util.exepath, archiveName);
            string vigemPath = Path.Combine(Util.exepath, "ViGEmBus");

            if (!File.Exists(archivePath))
            {
                AppLogger.Log("Failed to find ViGEmBus archive file");
                Progress = 100.0;
                vigemInstalled = false;
                return;
            }

            if (Directory.Exists(vigemPath))
            {
                Directory.Delete(vigemPath, true);
            }

            Directory.CreateDirectory(vigemPath);
            try
            {
                ZipFile.ExtractToDirectory(archivePath, vigemPath);
                //ZipFile.ExtractToDirectory(Path.Combine(Util.exepath, "ViGEmBus_signed_Win7-10_x86_x64_v1.14.3.0.zip"),
                //    vigemPath);
            } // Saved so the user can uninstall later
            catch
            {
                AppLogger.Log($"Failed to extract {archivePath}");
                Progress = 100.0;
                vigemInstalled = false;
                return;
            }

            AppLogger.Log("Creating Virtual Gamepad Emulation Bus Device");
            Progress = 33.3;

            deviceCreated = Devcon.Create("System", Util.sysGuid, @"Root\ViGEmBus");
            //Console.WriteLine("SUBMIT!!!: " + result.ToString());

            AppLogger.Log("Installing ViGEmBus Driver");
            Progress = 66.6;

            string infPath = Path.Combine(vigemPath, Util.arch, "ViGEmBus.inf");
            AppLogger.Log($"Installing driver from {infPath}");
            //result = Devcon.Install(@"C:\Users\ryoch\Downloads\Sources\ViGEm\x64\Release\ViGEmBus\ViGEmBus.inf", out temp);
            vigemInstalled = Devcon.Install(infPath, out bool temp);
            //Console.WriteLine("1SUBMIT!!!: " + result.ToString());

            if (vigemInstalled)
            {
                AppLogger.Log("ViGEmBus is now installed\n");
                Progress = 100.0;
            }
            else
            {
                RemoveDevice();
                AppLogger.Log("ViGEmBus install failed\n");
                Progress = 100.0;
            }

            RunFinished?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateViGEmDlProgress(object sender, DownloadProgressChangedEventArgs args)
        {
            AppLogger.Log($"Downloading ViGEmBus: {args.ProgressPercentage.ToString()}%",
                false);
        }

        private void RemoveDevice()
        {
            string instanceId = Util.ViGEmBusInstanceId();
            if (!string.IsNullOrEmpty(instanceId))
            {
                Devcon.Remove(Util.sysGuid, instanceId);
            }
        }

        public void Uninstall()
        {
            bool result = false;

            string infFile = Util.ViGEmBusDevProp(NativeMethods.DEVPKEY_Device_DriverInfPath);

            AppLogger.Log("Start Uninstalling ViGEmBus");
            Progress = 0.0;

            AppLogger.Log("Removing Virtual Gamepad Emulation Bus Device");
            Progress = 33.3;

            string instanceId = Util.ViGEmBusInstanceId();
            if (!string.IsNullOrEmpty(instanceId))
            {
                result = Devcon.Remove(Util.sysGuid, instanceId);
            }

            AppLogger.Log("Removing ViGEmBus driver from driver store");
            Progress = 66.6;

            result = NativeMethods.SetupUninstallOEMInfW(infFile, 0x0001, IntPtr.Zero);

            AppLogger.Log("Finished\n");
            Progress = 100.0;

            vigemInstalled = false;
        }
    }
}
