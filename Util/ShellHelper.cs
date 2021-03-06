using System;
using System.Diagnostics;

public static class ShellHelper
    {
        public static string PythonScript(string script)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python3",
                    Arguments = $"{script}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }