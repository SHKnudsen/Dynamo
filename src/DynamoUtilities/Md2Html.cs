using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Dynamo.Utilities
{
    public class Md2Html
    {
        private readonly Process process = new Process();
        private readonly bool started;
        /// <summary>
        /// Constructor
        /// Start the CLI tool and keep it around
        /// </summary>
        public Md2Html()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardInput = true,

                UseShellExecute = false,
                Arguments = @"",
                FileName = GetToolPath()
            };

            process.StartInfo = startInfo;
            try
            {
                process.Start();
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(ProcessExit);
                started = true;
            }
            catch(Win32Exception)
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Destructor
        /// Kill the CLI tool, if still running
        /// </summary>
        ~Md2Html()
        {
            KillProcess();
        }

        /// <summary>
        /// Converts a markdown string into Html.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="mdString"></param>
        /// <param name="mdPath"></param>
        public void ParseMd2Html(ref StringWriter writer, string mdString, string mdPath)
        {
            if (!started)
            {
                writer.WriteLine(GetErrorMessage());
                return;
            }

            process.StandardInput.WriteLine(@"<<<<<Convert>>>>>");
            process.StandardInput.WriteLine(mdPath);
            process.StandardInput.WriteLine(mdString);
            process.StandardInput.WriteLine(@"<<<<<Eod>>>>>");

            GetData(ref writer);
        }

        /// <summary>
        /// Sanitize Html
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Returns Sanitized Html</returns>
        public string SanitizeHtml(string content)
        {
            if (!started)
            {
                return GetErrorMessage();
            }

            process.StandardInput.WriteLine(@"<<<<<Sanitize>>>>>");
            process.StandardInput.WriteLine(content);
            process.StandardInput.WriteLine(@"<<<<<Eod>>>>>");

            var writer = new StringWriter();
            GetData(ref writer);

            return writer.ToString();
        }

        /// <summary>
        /// Read data from CLI tool
        /// <param name="writer"></param>
        /// </summary>
        private void GetData(ref StringWriter writer)
        {
            var done = false;

            while (!done)
            {
                var line = process.StandardOutput.ReadLine();
                if (line == null || line == @"<<<<<Eod>>>>>")
                {
                    done = true;
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        writer.WriteLine(line);
                    }
                }
            }
        }


        /// <summary>
        /// Compute the location of the CLI tool
        /// </summary>
        /// <returns>Returns full path to the CLI tool</returns>
        private static string GetToolPath ()
        {
            var rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new ArgumentNullException("Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)");
            var toolPath = Path.Combine(rootPath, @"Md2Html\Md2Html.exe");
            return toolPath;
        }

        /// <summary>
        /// Kill the CLI tool - if running
        /// </summary>
        private void KillProcess()
        {
            if (started && !process.HasExited)
            {
                process.Kill();
            }
        }
        /// <summary>
        /// Kill the CLI tool - if running - when the process exit
        /// </summary>
        /// <returns>Returns full path to the CLI tool</returns>
        private void ProcessExit(object sender, EventArgs e)
        {
            KillProcess();
        }

        /// <summary>
        /// Error message
        /// </summary>
        /// <returns>Returns error message</returns>
        private string GetErrorMessage()
        {
            return @"<p>Can't start '" + GetToolPath() + "'</p>";
        }
    }
}
