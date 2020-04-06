using System.Diagnostics;
using System.IO;
using Kachuwa.Core.AntiVirus;

namespace Kachuwa.AntiVirus
{
    public class WindowsDefenderScanner : IVirusScanner
    {
        // private readonly string mpcmdrunLocation;

        /// <summary>
        /// Creates a new Windows defender scanner
        /// </summary>
        /// <param name="scannerPath">The location of the mpcmdrun.exe file e.g. C:\Program Files\Windows Defender\MpCmdRun.exe</param>
        public WindowsDefenderScanner(string scannerPath)
        {
            if (!File.Exists(scannerPath))
            {
                throw new FileNotFoundException();
            }

            this.ScannerPath = new FileInfo(scannerPath).FullName;
        }
        public WindowsDefenderScanner()
        {
            if (!File.Exists(@"C:\Program Files\Windows Defender\MpCmdRun.exe"))
            {
                throw new FileNotFoundException();
            }

            this.ScannerPath = new FileInfo(@"C:\Program Files\Windows Defender\MpCmdRun.exe").FullName;
        }

        public string Name { get; } = "Windows Defender";
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
                Arguments = $"-Scan -ScanType 3 -File \"{fileInfo.FullName}\" -DisableRemediation",
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
                case 2:
                    return ScanResult.ThreatFound;
                default:
                    return ScanResult.Error;
            }
        }
    }
}