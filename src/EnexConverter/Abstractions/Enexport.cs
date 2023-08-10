namespace EnexConverter.Abstractions;

public class Enexport
{
    public IEnumerable<Note> Note { get; set; }
    public string ExportDate { get; set; }
    public string Application { get; set; }
    public string Version { get; set; }
    public string Text { get; set; }
}