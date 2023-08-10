using Microsoft.Extensions.Logging;
using System.CommandLine;
using EnexConverter;
using Microsoft.Extensions.Logging.Console;

var srcPath = new Option<string>(
    name: "--source",
    description: "Enex Base Folder. This is where all the enex files live.");
var destPathOption = new Option<string>(
    name: "--dest",
    description: "Dest Folder to create markdown files in. Preserving enex directory structure. Will create dir if not exist.");
var forceWriteOption = new Option<bool>(
    name: "--force",
    description: "Force overwrite if directory exists and is not empty");
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder
        .AddSimpleConsole(options =>
        {
            options.ColorBehavior = LoggerColorBehavior.Default;
            options.IncludeScopes = false;
        }) 
        .SetMinimumLevel(LogLevel.Debug);
});

var logger = loggerFactory.CreateLogger("logger");
RootCommand rootCommand = new(description: "Enex to Markdown Converter")
{
    srcPath,
    destPathOption,
    forceWriteOption
};
rootCommand.SetHandler(
    async (string srcDir, string destDir, bool forceWrite) =>
    {
        if (string.IsNullOrEmpty(srcDir))
        {
            Console.WriteLine("no enex base path specified. Use --help for help");
            Environment.ExitCode = 1;
            return;
        }

        if (string.IsNullOrEmpty(destDir))
        {
            Console.WriteLine("no dest path specified. use --help for help");
            Environment.ExitCode = 1;
            return;
        }

        ConvertEnexFilesToMarkdown(srcDir, destDir, forceWrite);
    },
    srcPath,
    destPathOption, 
    forceWriteOption);
await rootCommand.InvokeAsync(args);

void ConvertEnexFilesToMarkdown(string srcPath, string destPath, bool forceWrite)
{
    logger.LogInformation("Converting Enex files from {NewLine}{SrcPath} => {DestPath}",
        Environment.NewLine, srcPath, destPath);

    if (!forceWrite 
        && Directory.Exists(destPath) 
        && Directory.GetFiles(destPath,"*.*", SearchOption.AllDirectories).Length>0)
    {
        Console.WriteLine($"{destPath} contains existing files.  Use --force flag to overwrite");
        Environment.ExitCode = 1;
        return;
    }

    if (!Directory.Exists(destPath))
    {
        Directory.CreateDirectory(destPath);
        logger.LogInformation("Created {DestPath}", destPath);
    }

    var enexFiles = Directory.GetFiles(srcPath, "*.enex", SearchOption.AllDirectories);
    var enexToMarkdown = new EnexToMarkdown(logger);

    foreach (var item in enexFiles)
    {
        enexToMarkdown.GenerateMarkdownFiles(item, srcPath, destPath);
    }

    logger.LogInformation("Markdown files written to \"{DestPath}\"",destPath);
}