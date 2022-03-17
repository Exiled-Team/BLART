namespace BLART.Services;

using System.Globalization;
using BLART.Objects;
using Microsoft.Data.Sqlite;

public class DatabaseHandler
{
    private static string _connectionString = $"Data Source={Program.DatabaseFile}";

    public static void Init()
    {
        Log.Info(nameof(Init), $"Initializing database at {_connectionString}");
        if (!File.Exists(Program.DatabaseFile))
        {
            Log.Info(nameof(Init), "Database not found, creating..");
            using SqliteConnection conn = new(_connectionString);
            conn.Open();
            
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating ping table..");
                cmd.CommandText =
                    "CREATE TABLE Pings(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT, Message TEXT)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating warning table..");
                cmd.CommandText =
                    "CREATE TABLE Warns(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT, Reason TEXT, StaffId TEXT, Issued TEXT)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating ban table..");
                cmd.CommandText =
                    "CREATE TABLE Bans(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT, Reason TEXT, StaffId TEXT, Issued TEXT)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating red role table..");
                cmd.CommandText =
                    "CREATE TABLE RedRoles(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT, Reason TEXT, StaffId TEXT, Issued TEXT)";
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void AddEntry(ulong id, string description, DatabaseType type, ulong staffId = 0)
    {
        using SqliteConnection conn = new(_connectionString);
        conn.Open();

        using (SqliteCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = type switch
            {
                DatabaseType.Ping => "INSERT INTO Pings(UserId, Message) VALUES(@id, @string)",
                DatabaseType.Ban => "INSERT INTO Bans(UserId, Reason, StaffId, Issued) VALUES(@id, @string, @staff, @issued)",
                DatabaseType.RedRole => "INSERT INTO RedRoles(UserId, Reason, StaffId, Issued) VALUES(@id, @string, @staff, @issued)",
                DatabaseType.Warn => "INSERT INTO Warns(UserId, Reason, StaffId, Issued) VALUES(@id, @string, @staff, @issued)",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            cmd.Parameters.AddWithValue("@id", id.ToString());
            cmd.Parameters.AddWithValue("@string", description);
            
            if (type is not DatabaseType.Ping)
            {
                cmd.Parameters.AddWithValue("@staff", staffId.ToString());
                cmd.Parameters.AddWithValue("@issued", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            }

            cmd.ExecuteNonQuery();
        }
        
        conn.Close();
    }

    public static void RemoveEntry(int id, DatabaseType type)
    {
        using SqliteConnection conn = new(_connectionString);
        conn.Open();

        using (SqliteCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = type switch
            {
                DatabaseType.Ping => "DELETE FROM Pings WHERE Id=@id",
                DatabaseType.Ban => "DELETE FROM Bans WHERE Id=@id",
                DatabaseType.Warn => "DELETE FROM Warns WHERE Id=@id",
                DatabaseType.RedRole => "DELETE FROM RedRoles WHERE Id=@id",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }
        
        conn.Close();
    }

    public static void RemoveEntry(ulong userId, DatabaseType type)
    {
        using SqliteConnection conn = new(_connectionString);
        conn.Open();

        using (SqliteCommand cmd = conn.CreateCommand())
        {
            cmd.CommandText = type switch
            {
                DatabaseType.Ping => "DELETE FROM Pings WHERE UserId=@id",
                DatabaseType.RedRole => "DELETE FROM RedRoles WHERE UserId=@id",
                DatabaseType.Warn => "DELETE FROM Warns WHERE UserId=@id",
                DatabaseType.Ban => "DELETE FROM Bans WHERE UserId=@id",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            cmd.Parameters.AddWithValue("@id", userId.ToString());
            cmd.ExecuteNonQuery();
        }
        
        conn.Close();
    }

    public static List<PunishmentInfo> GetPunishmentInfo(ulong userId, DatabaseType type)
    {
        List<PunishmentInfo> info = new();
        
        using (SqliteConnection conn = new(_connectionString))
        {
            conn.Open();
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = type switch
                {
                    DatabaseType.Ban => "SELECT * FROM Bans WHERE UserId=@id",
                    DatabaseType.Warn => "SELECT * FROM Warns WHERE UserId=@id",
                    DatabaseType.RedRole => "SELECT * FROM RedRoles WHERE UserId=@id",
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };
                cmd.Parameters.AddWithValue("@id", userId);

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string reason = reader.GetString(2);
                        string staffId = reader.GetString(3);
                        string issued = reader.GetString(4);
                        info.Add(new PunishmentInfo(userId, id, ulong.Parse(staffId), reason, DateTime.Parse(issued)));
                    }
                }
            }
            
            conn.Close();
        }

        return info;
    }

    public static PunishmentInfo? GetInfoById(int id, DatabaseType type)
    {
        using (SqliteConnection conn = new(_connectionString))
        {
            conn.Open();
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = type switch
                {
                    DatabaseType.Ban => "SELECT * FROM Bans WHERE Id=@id",
                    DatabaseType.Warn => "SELECT * FROM Warns WHERE Id=@id",
                    DatabaseType.RedRole => "SELECT * FROM RedRoles WHERE Id=@id",
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };
                cmd.Parameters.AddWithValue("@id", id);

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string userId = reader.GetString(1);
                        string reason = reader.GetString(2);
                        string staffId = reader.GetString(3);
                        string issued = reader.GetString(4);
                        return new PunishmentInfo(ulong.Parse(userId), id, ulong.Parse(staffId), reason, DateTime.Parse(issued));
                    }
                }
            }
            
            conn.Close();
        }

        return null;
    }
    
    public static PunishmentInfo? GetInfoById(ulong userId, DatabaseType type)
    {
        using (SqliteConnection conn = new(_connectionString))
        {
            conn.Open();
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = type switch
                {
                    DatabaseType.Ban => "SELECT * FROM Bans WHERE UserId=@id",
                    DatabaseType.Warn => "SELECT * FROM Warns WHERE UserId=@id",
                    DatabaseType.RedRole => "SELECT * FROM RedRoles WHERE UserId=@id",
                    _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
                };
                cmd.Parameters.AddWithValue("@id", userId.ToString());

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);
                        string reason = reader.GetString(2);
                        string staffId = reader.GetString(3);
                        string issued = reader.GetString(4);
                        return new PunishmentInfo(userId, id, ulong.Parse(staffId), reason, DateTime.Parse(issued));
                    }
                }
            }
            
            conn.Close();
        }

        return null;
    }

    public static string GetPingTrigger(ulong userId)
    {
        string message = string.Empty;
        using (SqliteConnection conn = new(_connectionString))
        {
            conn.Open();
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Pings WHERE UserId=@id";
                cmd.Parameters.AddWithValue("@id", userId.ToString());

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        message = reader.GetString(2);
                        break;
                    }
                }
            }
            
            conn.Close();
        }
        
        if (string.IsNullOrEmpty(message))
            Log.Debug(nameof(GetPingTrigger), $"Returning null ping trigger message for {userId}.");
        return message;
    }
}