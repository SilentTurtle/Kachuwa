using System;
using System.Collections.Generic;
using System.Text;

namespace Kachuwa.Core.AntiVirus
{
   
    /// <summary>
    /// Generic interface for virus scanner
    /// </summary>
    public interface IVirusScanner
    {

        string Name { get;  }
        string ScannerPath { get; set; }
        /// <summary>
        /// Scan a single file
        /// </summary>
        /// <param name="file">The file to scan</param>
        /// <param name="timeoutInMs">The maximum time in milliseconds to take for this scan</param>
        /// <returns>The scan result</returns>
        ScanResult Scan(string file, int timeoutInMs = 30000);
    }
}
