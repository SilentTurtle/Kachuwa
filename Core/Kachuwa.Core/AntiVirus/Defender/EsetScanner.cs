﻿using System.Diagnostics;
using System.IO;
using Kachuwa.Core.AntiVirus;

namespace Kachuwa.AntiVirus
{
    public class EsetScanner : IVirusScanner
    {
        private readonly string esetClsLocation;

        /// <summary>
        /// Creates a new scanner
        /// </summary>
        /// <param name="esetClsLocation">The location of the ecls.exe file e.g. C:\Program Files\ESET\ESET Endpoint Antivirus\ecls.exe</param>
        public EsetScanner(string esetClsLocation)
        {
            if (!File.Exists(esetClsLocation))
            {
                throw new FileNotFoundException();
            }

            this.esetClsLocation = new FileInfo(esetClsLocation).FullName;
        }

        public EsetScanner()
        {
            var path = @"C:\Program Files\ESET\ESET Endpoint Antivirus\ecls.exe";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            this.esetClsLocation = new FileInfo(path).FullName;
        }

        public string Name { get; } = "ESET Antivirus";
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

            var startInfo = new ProcessStartInfo(this.esetClsLocation)
            {
                Arguments = $"\"{fileInfo.FullName}\" /no-log-console /preserve-time",
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
                case 50:
                    return ScanResult.ThreatFound;
                case 10:
                case 100:
                    return ScanResult.Error;
                default:
                    return ScanResult.Error;
            }
        }
    }
}