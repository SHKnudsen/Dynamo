using System;
using System.IO;
using NDesk.Options;

namespace Md2Html
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (CheckForHelp(args))
            {
                // Display help and exit
                return;
            }

            Console.WriteLine("");

            while (true)
            {
                var line = Console.ReadLine();
                if (line == @"<<<<<Sanitize>>>>>")
                {
                    Sanitize();
                }
                else if (line == @"<<<<<Convert>>>>>")
                {
                    Convert();
                }
                Console.WriteLine(@"<<<<<Eod>>>>>");
            }
        }

        static void Sanitize()
        {
            StringWriter data = new StringWriter();
            GetData(ref data);

            var output = MarkdownHandler.Sanitize(data.ToString());

            Console.WriteLine(output);
        }

        static void Convert()
        {
            var mdPath = Console.ReadLine();

            StringWriter data = new StringWriter();
            GetData(ref data);

            var instance = MarkdownHandler.Instance;
            StringWriter output = new StringWriter();
            instance.ParseToHtml(ref output, data.ToString(), mdPath);

            Console.WriteLine(output.ToString());
        }

        static void GetData(ref StringWriter data)
        {
            while (true)
            {
                var line = Console.ReadLine();
                if (line == @"<<<<<Eod>>>>>")
                {
                    break;
                }
                data.WriteLine(line);
            }
        }

        static bool CheckForHelp(string[] args)
        {
            bool help = false;
            var options = new OptionSet()
            {
                {@"h|?|help", @"Show help and exit", v => help = v != null },
            };

            options.Parse(args);
            if (help)
            {
                options.WriteOptionDescriptions(Console.Out);
                DisplayHelp();
            }

            return help;
        }
        static void DisplayHelp()
        {
            Console.WriteLine(@"");
            Console.WriteLine(@"This tool converts Markdown to Html or Sanitize html");
            Console.WriteLine(@"and reads from stdin and outputs to stdout");
            Console.WriteLine(@"");
            Console.WriteLine(@"Converting Markdown to Html");
            Console.WriteLine(@"---------------------------");
            Console.WriteLine(@"Format markdown as follows:");
            Console.WriteLine(@"<<<<<Convert>>>>>");
            Console.WriteLine(@"Markdown formatted data here");
            Console.WriteLine(@"<<<<<Eod>>>>>");
            Console.WriteLine(@"");
            Console.WriteLine(@"Output will be:");
            Console.WriteLine(@"Html formatted data");
            Console.WriteLine(@"<<<<<Eod>>>>>");
            Console.WriteLine(@"");
            Console.WriteLine(@"Sanitize Html");
            Console.WriteLine(@"-------------");
            Console.WriteLine(@"Format html data as follows:");
            Console.WriteLine(@"<<<<<Sanitize>>>>>");
            Console.WriteLine(@"Html data to sanitize here");
            Console.WriteLine(@"<<<<<Eod>>>>>");
            Console.WriteLine(@"");
            Console.WriteLine(@"Output will be:");
            Console.WriteLine(@"Sanitized Html data");
            Console.WriteLine(@"<<<<<Eod>>>>>");
            Console.WriteLine(@"");
            Console.WriteLine(@"");
        }
    }
}
