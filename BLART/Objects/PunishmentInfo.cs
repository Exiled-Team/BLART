namespace BLART.Objects;

public class PunishmentInfo
{
    public ulong UserId { get; set; }
    public int Id { get; set; }
    public ulong StaffId { get; set; }
    public string Reason { get; set; }
    public DateTime Issued { get; set; }

    public PunishmentInfo(ulong userId, int id, ulong staffId, string reason, DateTime issued)
    {
        UserId = userId;
        Id = id;
        StaffId = staffId;
        Reason = reason;
        Issued = issued;
    }

    public void Deconstruct(out ulong userId, out int id, out ulong staffId, out string reason, out DateTime issued)
    {
        userId = UserId;
        id = Id;
        staffId = StaffId;
        reason = Reason;
        issued = Issued;
    }
}