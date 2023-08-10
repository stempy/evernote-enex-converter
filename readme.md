# Evernote Enex To Markdown converter

This converts all Evernote `.enex` export files inside a folder to markdown files, while preserving folder structure of enex files.

It requires [.NET SDK](https://dotnet.microsoft.com/en-us/download) to run.

Clone this repo and and in the repo directory, run like so:

Usage:

Get help

```sh
dotnet run --project ./src/EnexConverter -- --help

Description:
  Enex to Markdown Converter

Usage:
  EnexConverter [options]

Options:
  --source <source>  Enex Base Folder. This is where all the enex files live. [default: http://localhost:7000]
  --dest <dest>      Dest Folder to create markdown files in preserving enex directory structure. Will create dir if not exist.
  --force            Force overwrite if directory exists and is not empty
  --version          Show version information
  -?, -h, --help     Show help and usage information
```

Example:

> NOTE: if running from the repository root with `dotnet`, the arguments after the `--project` folder have an additional `--`. This isnt needed for a published CLI tool, only while running with `dotnet`

```sh
dotnet run --project ./src/EnexConverter -- --source "[your-enex-source-path]" --dest "[dest-for-md-files]" --force
```


