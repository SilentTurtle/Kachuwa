﻿using System.Diagnostics;
using System.IO;
using Kachuwa.Core.AntiVirus;

namespace Kachuwa.AntiVirus
{
    public class AVGScanner : IVirusScanner
    {
        private const int RETURNCODE_OK = 0;
        private const int RETURNCODE_USERSTOP = 1;
        private const int RETURNCODE_ERROR = 2;
        private const int RETURNCODE_WARNING = 3;
        private const int RETURNCODE_PUPDETECTED = 4;
        private const int RETURNCODE_VIRUSDETECTED = 5;
        private const int RETURNCODE_PWDARCHIVE = 6;



        /// <summary>
        /// Creates a new scanner
        /// </summary>
        /// <param name="scannerPath">The location of the avgscanx.exe (x86) or avgscana.exe (x64) file e.g. C:\Program Files\AVAST Software\avast</param>
        public AVGScanner(string scannerPath)
        {
            if (!File.Exists(scannerPath))
            {
                throw new FileNotFoundException();
            }

            this.ScannerPath = new FileInfo(scannerPath).FullName;
        }

        public AVGScanner()
        {
            var scannerDefaultPath = @"C:\Program Files (x86)\AVG\Av\avgscanx.exe";
            if (!File.Exists(scannerDefaultPath))
            {
                throw new FileNotFoundException();
            }

            this.ScannerPath = new FileInfo(scannerDefaultPath).FullName;
        }

        public string Name { get; } = "AVG";
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
                Arguments = $"/SCAN=\"{fileInfo.FullName}\"",
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
                case RETURNCODE_OK:
                    return ScanResult.NoThreatFound;
                case RETURNCODE_VIRUSDETECTED:
                case RETURNCODE_PUPDETECTED:
                    return ScanResult.ThreatFound;
                case RETURNCODE_USERSTOP:
                case RETURNCODE_ERROR:
                case RETURNCODE_WARNING:
                case RETURNCODE_PWDARCHIVE:
                default:
                    return ScanResult.Error;
            }
        }
    }
}