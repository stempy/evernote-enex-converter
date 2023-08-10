namespace EnexConverter.Abstractions;

public class Note
{
    public string Title { get; set; }
    public string Created { get; set; }
    public string Updated { get; set; }
    public string Tag { get; set; }
    public Noteattributes Noteattributes { get; set; }
    public string Content { get; set; }
}