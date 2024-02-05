using LinqToDB;
using LinqToDB.Mapping;

namespace BLART.Interfaces;

[Table("bans")]
public class Ban : IDbTable
{
    [Column("id"), PrimaryKey, Identity]
    public ulong Id { get; set; }
    
    [Column("user_id")]
    public ulong UserId { get; set; }
    
    [Column("staff_id")]
    public ulong StaffId { get; set; }

    [Column("reason", DataType = DataType.Text)]
    public string Reason { get; set; } = null!;
    
    [Column("issued_at")]
    public DateTimeOffset IssuedAt { get; set; }
}