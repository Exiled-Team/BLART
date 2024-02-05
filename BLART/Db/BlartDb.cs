using BLART.Interfaces;
using LinqToDB;
using LinqToDB.Data;

namespace BLART.Db;

public class BlartDb : DataConnection
{
    public BlartDb() : base(nameof(BlartDb)) { }

    public ITable<Ban> Bans => this.GetTable<Ban>();
    public ITable<PingTrigger> PingTriggers => this.GetTable<PingTrigger>();
    public ITable<RedRole> RedRoles => this.GetTable<RedRole>();
    public ITable<SelfAssignableRole> SelfAssignableRoles => this.GetTable<SelfAssignableRole>();
    public ITable<Tag> Tags => this.GetTable<Tag>();
    public ITable<Warn> Warns => this.GetTable<Warn>();
}