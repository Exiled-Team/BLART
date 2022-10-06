namespace BLART.Objects;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }

    public Tag(int id, string name, string text)
    {
        Id = id;
        Name = name;
        Text = text;
    }
}