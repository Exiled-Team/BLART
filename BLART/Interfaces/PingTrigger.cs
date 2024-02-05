using LinqToDB;
using LinqToDB.Mapping;

namespace BLART.Interfaces;

[Table("ping_triggers")]
public class PingTrigger : IDbTable
{
    [Column("user_id"), PrimaryKey]
    public ulong UserId { get; set; }

    [Column("message", DataType = DataType.Text)]
    public string Message { get; set; } = null!;
}