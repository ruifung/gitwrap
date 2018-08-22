﻿using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GitWrap
{
    class Program
    {
        private static int outputLines = 0;

        static void Main(string[] args)
        {
            executeGitWithArgs(getBashPath(), args);   
        }

        static void executeGitWithArgs(String bashPath, string[] args)
        {
            if (!File.Exists(bashPath))
            {
                Console.Write("[-] Error: Bash.exe not found.");
                return;
            }

            ProcessStartInfo bashInfo = new ProcessStartInfo();
            bashInfo.FileName = bashPath;

            // Loop through args and pass them to git executable
            StringBuilder argsBld = new StringBuilder();
            argsBld.Append("-c \"git");

            for (int i = 0; i < args.Length; i++)
            {
                argsBld.Append(" " + PathConverter.convertPathFromWindowsToLinux(args[i]));
            }

            // Append quotation to close of the argument supplied to bash.exe
            argsBld.Append("\"");

            bashInfo.Arguments = argsBld.ToString();
            bashInfo.UseShellExecute = false;
            bashInfo.RedirectStandardOutput = true;
            bashInfo.RedirectStandardError = true;
            bashInfo.CreateNoWindow = true;

            var proc = new Process
            {
                StartInfo = bashInfo
            };

            proc.OutputDataReceived += CaptureOutput;
            proc.ErrorDataReceived += CaptureError;

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            proc.WaitForExit();
        }

        static void CaptureOutput(object sender, DataReceivedEventArgs e)
        {
            printOutputData(e.Data);
        }

        static void CaptureError(object sender, DataReceivedEventArgs e)
        {
            printOutputData(e.Data);
        }

        static String getBashPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
            @"System32\bash.exe");
        }

        static void printOutputData(String data)
        {
            if (data != null)
            {
                if (outputLines > 0)
                {
                    Console.Write(Environment.NewLine);
                }
                Console.Write(data);
                outputLines++;
            }
        }
    }
}
