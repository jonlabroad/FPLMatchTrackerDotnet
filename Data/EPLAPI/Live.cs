using System.Collections.Generic;

public class Live
{
    public List<Fixture> fixtures { get; set; } = new List<Fixture>();
    public Dictionary<int, LiveElement> elements {get; set;} = new Dictionary<int, LiveElement>();

}
