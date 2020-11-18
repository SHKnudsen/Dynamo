using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Dynamo.Utilities
{
    internal class Md2Html : IDisposable
    {
        private readonly Process process = new Process();
        private bool started;
        /// <summary>
        /// Constructor
        /// Start the CLI tool and keep it around
        /// </summary>
        internal Md2Html()
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
                started = true;
            }
            catch(Win32Exception)
            {
                // Do nothing
            }
        }

        /// <summary>
        /// Kill the CLI tool, if still running
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            KillProcess();
        }

        /// <summary>
        /// Kill the CLI tool, if still running
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Converts a markdown string into Html.
        /// </summary>
        /// <param name="mdString"></param>
        /// <param name="mdPath"></param>
        /// <returns>Returns converted markdown as html</returns>
        internal string ParseMd2Html(string mdString, string mdPath)
        {
            if (!started)
            {
                return GetErrorMessage();
            }

            process.StandardInput.WriteLine(@"<<<<<Convert>>>>>");
            process.StandardInput.WriteLine(mdPath);
            process.StandardInput.WriteLine(mdString);
            process.StandardInput.WriteLine(@"<<<<<Eod>>>>>");

            var writer = new StringWriter();
            GetData(ref writer);

            return writer.ToString();
        }

        /// <summary>
        /// Sanitize Html
        /// </summary>
        /// <param name="content"></param>
        /// <returns>Returns Sanitized Html</returns>
        internal string SanitizeHtml(string content)
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
                started = false;
            }
        }

        /// <summary>
        /// Error message
        /// </summary>
        /// <returns>Returns error message</returns>
        private string GetErrorMessage()
        {
            return @"<p>Can't start '" + GetToolPath() + @"'</p>";
        }
    }
}
