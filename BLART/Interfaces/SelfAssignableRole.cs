using LinqToDB.Mapping;

namespace BLART.Interfaces;

[Table("self_assignable_roles")]
public class SelfAssignableRole : IDbTable
{
    [Column("role_id"), PrimaryKey]
    public ulong RoleId { get; set; }
}