using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using EnexConverter.Abstractions;
using Microsoft.Extensions.Logging;
using ReverseMarkdown;

namespace EnexConverter
{
    public class EnexToMarkdown
    {
        private class Sanitized
        {
            // https://msdn.microsoft.com/en-us/library/aa365247.aspx#naming_conventions
            // http://stackoverflow.com/questions/146134/how-to-remove-illegal-characters-from-path-and-filenames
            private static readonly Regex removeInvalidChars = new Regex($"[{Regex.Escape(new string(Path.GetInvalidFileNameChars()))}]",
                RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

            public static string SanitizedFileName(string fileName, string replacement = "-")
            {
                ArgumentException.ThrowIfNullOrEmpty(fileName);
                return removeInvalidChars.Replace(fileName, replacement)
                    .Replace(" ", "-").Replace(',','-');
            }
        }

        private readonly ILogger _logger;

        public EnexToMarkdown(ILogger logger)
        {
            _logger = logger;
        }

        public void GenerateMarkdownFiles(string enexFile, string basePath, string destBasePath)
        {
            var reader = new EnexReader();
            var enex = reader.ParseEvernoteEnex(enexFile);
            var enexPath = Path.GetDirectoryName(enexFile);
            var relativePath = Path.GetRelativePath(basePath, enexPath);

            var fullDestPath = Path.Combine(destBasePath, relativePath);
            var filename = Path.GetFileNameWithoutExtension(enexFile);

            fullDestPath = Path.Combine(fullDestPath, filename);
            if (!Directory.Exists(fullDestPath))
            {
                Directory.CreateDirectory(fullDestPath);
            }

            _logger.LogInformation("{EnexFile} =>", enexFile.Replace(basePath,""));
            foreach (var note in enex.Note)
            {
                try
                {
                    var markdown = GenerateMarkdown(note);

                    var created = ParseDateTime(note.Created);
                    var updated = ParseDateTime(note.Updated);

                    var cleanName = Sanitized.SanitizedFileName(note.Title);

                    var noteFilename = $"{FormatDate(created)}_{cleanName}";

                    // Truncate to the maximum length
                    if (noteFilename.Length > 80)
                    {
                        noteFilename = noteFilename.Substring(0, 80);
                    }

                    noteFilename += ".md";
                    var noteFile = Path.Combine(fullDestPath, noteFilename);

                    File.WriteAllText(noteFile, markdown);
                    File.SetCreationTime(noteFile, created);
                    _logger.LogInformation("\tNote: {NoteFile}", Path.GetFileName(noteFile));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "\tNote-Failed: {Filename} / Note={Title} - ERROR: {Message}", filename, note.Title, ex.Message);
                }
            }
        }

        public string GetContent(string content)
        {
            var config = new ReverseMarkdown.Config
            {
                // Include the unknown tag completely in the result (default as well)
                UnknownTags = Config.UnknownTagsOption.PassThrough,
                // generate GitHub flavoured markdown, supported for BR, PRE and table tags
                GithubFlavored = true,
                // will ignore all comments
                RemoveComments = true,
                // remove markdown output for links where appropriate
                SmartHrefHandling = true
            };

            var converter = new ReverseMarkdown.Converter(config);

            var parser = new HtmlParser();
            var doc = parser.ParseDocument(content);
            var contentHtml = doc.QuerySelector("en-note").InnerHtml;

            return converter.Convert(contentHtml);
        }

        public string GenerateMarkdown(Note note)
        {
            var sb = new StringBuilder();

            var created = ParseDateTime(note.Created);
            var updated = ParseDateTime(note.Updated);

            var frontMatter = new Dictionary<string, string>
            {
                ["title"] = note.Title,
                ["created"] = FormatDate(created),
                ["updated"] = FormatDate(updated),
                ["author"] = note.Noteattributes.Author,
                ["source"] = note.Noteattributes.Source
            };

            sb.AppendLine("---");
            sb.Append(string.Join(Environment.NewLine, frontMatter.Select(y => $"{y.Key}: {y.Value}")));
            sb.AppendLine(Environment.NewLine + "---");
            sb.AppendLine();

            sb.Append(GetContent(note.Content));
            return sb.ToString();
        }

        DateTime ParseDateTime(string dtStr)
        {
            return DateTime.ParseExact(dtStr, "yyyyMMddTHHmmssZ",
            System.Globalization.CultureInfo.InvariantCulture);
        }

        string FormatDate(DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd-HHmmss");
        }

    }
}
