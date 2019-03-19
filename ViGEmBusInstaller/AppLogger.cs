using System;
using System.IO;

namespace ViGEmBusInstaller
{
    class AppLogger
    {
        public delegate void LogEventHandler(string args);
        public static event LogEventHandler LogEvent;
        public static StreamWriter logfile = new StreamWriter(Util.exepath + "\\log.txt", false);

        public static void Log(string message, bool toFile = true)
        {
            LogEvent?.Invoke(message);
            if (toFile)
            {
                logfile.WriteLine($"{DateTime.Now.ToString()}: {message.Replace("\n", Environment.NewLine)}");
                logfile.Flush();
            }
        }

        public static void CloseLogFile()
        {
            logfile.Close();
        }
    }
}
