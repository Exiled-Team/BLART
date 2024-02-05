using LinqToDB;
using LinqToDB.Mapping;

namespace BLART.Interfaces;

[Table("tags")]
public class Tag : IDbTable
{
    [Column("name", DataType = DataType.Text), PrimaryKey]
    public string Name { get; set; } = null!;

    [Column("text", DataType = DataType.Text)]
    public string Text { get; set; } = null!;
    
    [Column("user_id")]
    public ulong UserId { get; set; }
}