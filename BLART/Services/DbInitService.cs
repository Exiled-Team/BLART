using BLART.Db;
using BLART.Interfaces;
using LinqToDB;

namespace BLART.Services;

public class DbInitService : BackgroundService
{
    private readonly ILogger<DbInitService> _logger;

    public DbInitService(ILogger<DbInitService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var assembly = typeof(Program).Assembly;
        var tableTypes = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IDbTable))).ToList();

        using (var db = new BlartDb())
        {
            var existingTables = db.DataProvider.GetSchemaProvider().GetSchema(db).Tables;
            foreach (var tableType in tableTypes)
            {
                if (existingTables.All(t => t.TableName != (string?)tableType.GetProperty("TableName")?.GetValue(tableType)))
                {
                    typeof(DataExtensions)
                        .GetMethod(nameof(DataExtensions.CreateTable), new[] { typeof(BlartDb) })
                        ?.MakeGenericMethod(tableType).Invoke(null, new object?[] { db });
                }
            }
        }
    }
}