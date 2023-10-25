using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_task_manager
{
    public class ServerSide
    {
        public static void Start()
        {
            string applicationPath = @"C:\Illia\step_exercise\c#\.NET_Framework\Network_programming\ConsoleApp_lab_3_Tcp\ConsoleApp_lab_3_Tcp\bin\Debug\ConsoleApp_lab_3_Tcp.exe";

            // Create a new process start info
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = applicationPath,
                RedirectStandardOutput = true, // Redirect standard output for reading
                UseShellExecute = false,      // Don't use the system shell
                CreateNoWindow = false         // Don't create a window for the new process
            };

            // Create and start the process
            Process process = new Process
            {
                StartInfo = psi
            };

            process.Start();
        }
    }
}
