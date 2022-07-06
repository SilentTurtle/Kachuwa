using System.Diagnostics;
using System.IO;
using Kachuwa.Core.AntiVirus;

namespace Kachuwa.AntiVirus
{
    public class AvastScanner : IVirusScanner
    {
       

        /// <summary>
        /// Creates a new scanner
        /// </summary>
        /// <param name="ashCmdLocation">The location of the ashCmd.exe file e.g. C:\Program Files\AVAST Software\avast</param>
        public AvastScanner(string ashCmdLocation)
        {
            if (!File.Exists(ashCmdLocation))
            {
                throw new FileNotFoundException();
            }

            this.ScannerPath = new FileInfo(ashCmdLocation).FullName;
        }

        public AvastScanner()
        {
            var scannerDefaultPath = @"C:\Program Files\AVAST Software\Avast\ashcmd.exe";
            if (!File.Exists(scannerDefaultPath))
            {
                throw new FileNotFoundException();
            }

            this.ScannerPath = new FileInfo(scannerDefaultPath).FullName;
        }

        public string Name { get; } = "Avast Antivirus";
        public string ScannerPath { get; set; }

        /// <summary>
        /// Scan a single file
        /// </summary>
        /// <param name="file">The file to scan</param>
        /// <param name="timeoutInMs">The maximum time in milliseconds to take for this scan</param>
        /// <returns>The scan result</returns>
        public ScanResult Scan(string file, int timeoutInMs = 30000)
        {
            if (!File.Exists(file))
            {
                return ScanResult.FileNotFound;
            }

            var fileInfo = new FileInfo(file);

            var process = new Process();

            var startInfo = new ProcessStartInfo(this.ScannerPath)
            {
                Arguments = $"\"{fileInfo.FullName}\" /p=4 /s",
                CreateNoWindow = true,
                ErrorDialog = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false
            };

            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit(timeoutInMs);

            if (!process.HasExited)
            {
                process.Kill();
                return ScanResult.Timeout;
            }

            switch (process.ExitCode)
            {
                case 0:
                    return ScanResult.NoThreatFound;
                case 1:
                    return ScanResult.ThreatFound;
                default:
                    return ScanResult.Error;
            }
        }
    }
}