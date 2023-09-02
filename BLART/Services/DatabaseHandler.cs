namespace BLART.Services;

using System.Globalization;
using System.Net;
using System.Threading.Channels;
using BLART.Objects;
using Discord;
using Microsoft.Data.Sqlite;
using Modules;

public class DatabaseHandler
{
    private static string _connectionString = $"Data Source={Program.DatabaseFile}";

    public static async Task Init(bool updateTables = false)
    {
        Log.Info(nameof(Init), $"Initializing database at {_connectionString}");
        if (!File.Exists(Program.DatabaseFile) || updateTables)
        {
            Log.Info(nameof(Init), "Database not found, creating..");
            using SqliteConnection conn = new(_connectionString);
            conn.Open();
            
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating ping table..");
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Pings(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT, Message TEXT)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating warning table..");
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Warns(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT, Reason TEXT, StaffId TEXT, Issued TEXT)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating ban table..");
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Bans(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT, Reason TEXT, StaffId TEXT, Issued TEXT)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating red role table..");
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS RedRoles(Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId TEXT, Reason TEXT, StaffId TEXT, Issued TEXT)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating bug report table..");
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS BugReports(Id INTEGER PRIMARY KEY AUTOINCREMENT, messageId TEXT, threadId TEXT)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating self roles table..");
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS SelfRoles(Id INTEGER PRIMARY KEY AUTOINCREMENT, roleId)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating tags table..");
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS Tags(Id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, tag TEXT)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating sticked messages table..");
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS StickiedMessages(Id INTEGER PRIMARY KEY AUTOINCREMENT, channelId TEXT, message TEXT, staffId TEXT)";
                cmd.ExecuteNonQuery();
            }

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                Log.Info(nameof(Init), "Creating sticked messages ID table..");
                cmd.CommandText =
                    "CREATE TABLE IF NOT EXISTS StickiedMessagesIDs(Id INTEGER PRIMARY KEY AUTOINCREMENT, messageId TEXT, channelId TEXT)";
                cmd.ExecuteNonQuery();
            }
        }

        await BugReporting.LoadDatabaseEntries();
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
                DatabaseType.BugReport => "INSERT INTO BugReports(messageId, threadId) VALUES(@id, @string)",
                DatabaseType.SelfRole => "INSERT INTO SelfRoles(roleId) VALUES(@id)",
                DatabaseType.Tags => "INSERT INTO Tags(name, tag) VALUES(@name, @tag)",
                DatabaseType.StickiedMessage => "INSERT INTO StickiedMessages(channelId, message, staffId) VALUES(@id, @string, @staff)",
                DatabaseType.StickiedMessageIDs => "INSERT INTO StickiedMessagesIDs(messageId, channelId) VALUES(@id, @string)",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            cmd.Parameters.AddWithValue("@id", id.ToString());

            if (type is not DatabaseType.SelfRole)
            {
                if (type is DatabaseType.Tags)
                {
                    string[] tagArray = description.Split('|');
                    cmd.Parameters.AddWithValue("@name", tagArray[0]);
                    cmd.Parameters.AddWithValue("@tag", tagArray[1]);
                }
                else
                    cmd.Parameters.AddWithValue("@string", description);

                if (type is not DatabaseType.Ping && type is not DatabaseType.BugReport)
                {
                    cmd.Parameters.AddWithValue("@staff", staffId.ToString());
                    cmd.Parameters.AddWithValue("@issued", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
                }
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
                DatabaseType.BugReport => "DELETE FROM BugReports WHERE Id=@id",
                DatabaseType.SelfRole => "DELETE FROM SelfRoles WHERE Id=@id",
                DatabaseType.Tags => "DELETE FROM Tags WHERE Id=@id",
                DatabaseType.StickiedMessage => "DELETE FROM StickiedMessages WHERE Id=@id",
                DatabaseType.StickiedMessageIDs => "DELETE FROM StickiedMessagesIDs WHERE Id=@id",
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
                DatabaseType.BugReport => "DELETE FROM BugReports WHERE messageId=@id OR threadId=@id",
                DatabaseType.SelfRole => "DELETE FROM SelfRoles WHERE roleId=@id",
                DatabaseType.StickiedMessage => "DELETE FROM StickiedMessages WHERE channelId=@id",
                DatabaseType.StickiedMessageIDs => "DELETE FROM StickiedMessagesIDs WHERE messageId=@id",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
            
            cmd.Parameters.AddWithValue("@id", userId.ToString());
            cmd.ExecuteNonQuery();
        }
        
        conn.Close();
    }

    public static List<ulong> GetSelfRoles()
    {
        List<ulong> roleIds = new();
        List<ulong> toRemove = new();
        using (SqliteConnection conn = new(_connectionString))
        {
            conn.Open();

            using (SqliteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM SelfRoles";

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        try
                        {
                            ulong roleId = ulong.Parse(reader.GetString(1));
                            IRole role = Bot.Instance.Guild.GetRole(roleId);
                            if (role is null)
                            {
                                toRemove.Add(roleId);

                                continue;
                            }

                            roleIds.Add(roleId);
                        }
                        catch (Exception e)
                        {
                            Log.Error(nameof(GetSelfRoles), e);
                        }
                    }
                }
            }
            
            conn.Close();
        }

        foreach (ulong roleId in toRemove)
            RemoveEntry(roleId, DatabaseType.SelfRole);

        return roleIds;
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

    public static ulong GetMessageId(ulong threadId)
    {
        ulong messageId = 0;
        using (SqliteConnection conn = new(_connectionString))
        {
            conn.Open();
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM BugReports WHERE threadId=@id";
                cmd.Parameters.AddWithValue("@id", threadId);

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (ulong.TryParse(reader.GetString(1), out messageId))
                            break;
                    }
                }
            }

            conn.Close();
        }

        return messageId;
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

    public static Tag? GetTag(string name)
    {
        Tag? tag = null;
        using (SqliteConnection conn = new(_connectionString))
        {
            conn.Open();
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Tags WHERE name=@name";
                cmd.Parameters.AddWithValue("@name", name);

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tag = new Tag(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                        break;
                    }
                }
            }
            
            conn.Close();
        }

        if (tag is null)
            Log.Debug(nameof(GetTag), $"Returning null tag for {name}");
        return tag;
    }

    public static List<string> GetTagNames()
    {
        List<string> tags = new ();
        using (SqliteConnection conn = new(_connectionString))
        {
            conn.Open();
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Tags";

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tags.Add(reader.GetString(1));
                    }
                }
            }
            
            conn.Close();
        }

        return tags;
    }

    public static StickyMessage? GetStickyMessage(ulong channelId)
    {
        StickyMessage? stick = null;
        using (SqliteConnection conn = new(_connectionString))
        {
            conn.Open();
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM StickiedMessages WHERE channelId=@id";
                cmd.Parameters.AddWithValue("@id", channelId.ToString());

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string message = reader.GetString(2);
                        string staffId = reader.GetString(3);
                        stick = new StickyMessage(channelId, message, ulong.Parse(staffId));
                        break;
                    }
                }
            }

            conn.Close();
        }

        return stick;
    }

    public static string? GetStickyMessageID(ulong channelId)
    {
        string? result = null;

        ITextChannel channel = Bot.Instance.Guild.GetTextChannel(channelId);
        if (channel is null)
            return result;

        using (SqliteConnection conn = new(_connectionString))
        {
            conn.Open();
            using (SqliteCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM StickiedMessagesIDs WHERE channelId=@id";
                cmd.Parameters.AddWithValue("@id", channelId.ToString());

                using (SqliteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = reader.GetString(1);
                        break;
                    }
                }
            }
        }

        return result;
    }
}