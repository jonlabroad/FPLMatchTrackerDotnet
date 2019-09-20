public class ExplainElement {
    public ExplainElement() {}

    public ExplainElement(ExplainElement other) {
        identifier = other.identifier;
        points = other.points;
        value = other.value;
    }

    public string identifier { get; set; }
    public int points { get; set; } = 0;
    public int value { get; set; } = 0;
}