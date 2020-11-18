using System;
using System.IO;
using System.Linq;
using Ganss.XSS;
using Markdig;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Md2Html
{
    /// <summary>
    /// Handles markdown files by converting them to Html, so they can display in a browser
    /// </summary>
    internal class MarkdownHandler
    {
        private readonly MarkdownPipeline pipeline;


        private static MarkdownHandler instance;
        internal static MarkdownHandler Instance
        {
            get
            {
                if (instance is null) { instance = new MarkdownHandler(); }
                return instance;
            }
        }

        private MarkdownHandler()
        {
            var pipelineBuilder = new MarkdownPipelineBuilder();
            pipeline = pipelineBuilder
                .UseAdvancedExtensions()
                .Build();
        }

        /// <summary>
        /// Converts a markdown string into Html.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="mdString"></param>
        /// <param name="mdPath"></param>
        /// <returns>Returns true if any script tags was removed from the string</returns>
        internal void ParseToHtml(ref StringWriter writer, string mdString, string mdPath)
        {
            if (writer is null)
                throw new ArgumentNullException(nameof(writer));

            if (string.IsNullOrWhiteSpace(mdString))
                return;

            // Remove scripts from user content for security reasons.
            var renderer = new HtmlRenderer(writer);
            pipeline.Setup(renderer);

            var document = MarkdownParser.Parse(mdString, pipeline);
            ConvertRelativeLocalImagePathsToAbsolute(mdPath, document);

            renderer.Render(document);
        }

        /// <summary>
        /// For markdown local images needs to be in the same folder as the md file
        /// referencing it with a relative path "./image.png", when we convert to html
        /// we need the full path. This method finds relative image paths and converts them to absolute paths.
        /// </summary>
        private static void ConvertRelativeLocalImagePathsToAbsolute(string mdFilePath, MarkdownDocument document)
        {
            var imageLinks = document.Descendants<ParagraphBlock>()
                .SelectMany(x => x.Inline.Descendants<LinkInline>())
                .Where(x => x.IsImage)
                .Select(x => x).ToList();

            foreach (var image in imageLinks)
            {
                if (!image.Url.StartsWith("./"))
                    continue;

                var imageName = image.Url.Split(new string[] { "./" }, StringSplitOptions.None);
                var dir = Path.GetDirectoryName(mdFilePath);

                var htmlImagePathPrefix = @"file:///";
                var absoluteImagePath = Path.Combine(dir, imageName.Last());

                image.Url = $"{htmlImagePathPrefix}{absoluteImagePath}";
            }
        }

        private static readonly HtmlSanitizer HtmlSanitizer = new HtmlSanitizer();

        /// <summary>
        /// Clean up possible dangerous HTML content from the content string.
        /// </summary>
        /// <param name="content"></param>
        /// <returns>return sanitized content string</returns>
        internal static string Sanitize(string content)
        {
           return HtmlSanitizer.Sanitize(content);
        }
    }
}
