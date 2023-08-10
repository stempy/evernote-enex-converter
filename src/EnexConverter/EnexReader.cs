using System.Xml.Linq;
using EnexConverter.Abstractions;

namespace EnexConverter;

public class EnexReader
{
    Noteattributes GetNoteAttribs(XElement note)
    {
        var na = note.Element("note-attributes");
        var author = na.Element("author")?.Value;
        var source = na.Element("source")?.Value;
        return new Noteattributes()
        {
            Author = author,
            Source = source
        };
    }


    public Enexport ParseEvernoteEnex(string file)
    {

        File.ReadAllText(file);
        var doc = XDocument.Load(file);
        var el = doc.Element("en-export");

        var exportDate = el.Attribute("export-date")?.Value;
        var appDate = el.Attribute("application")?.Value;
        var appVer = el.Attribute("version")?.Value;

        var export = new Enexport()
        {
            Application = appDate,
            ExportDate = exportDate,
            Version = appVer
        };

        var notes = new List<Note>();

        foreach (var noteEl in el.Descendants("note"))
        {
            var noteInfo = new Note()
            {
                Created = noteEl.Element("created")?.Value,
                Title = noteEl.Element("title")?.Value,
                Content = noteEl.Element("content")?.Value,
                Tag = noteEl.Element("tag")?.Value,
                Updated = noteEl.Element("updated")?.Value,
                Noteattributes = GetNoteAttribs(noteEl)
            };
            notes.Add(noteInfo);
        }

        export.Note = notes;
        return export;
    }

}