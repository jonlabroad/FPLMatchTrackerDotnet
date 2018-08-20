using System.Collections.Generic;

public class NewEntries
{
    public bool has_next { get; set; }
    public int number { get; set; }
    public List<NewResult> results { get; set; } = new List<NewResult>();
}
