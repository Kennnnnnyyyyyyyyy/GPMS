using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;

namespace Gate_Pass_management.Infrastructure.Sqlite;

/// <summary>
/// Connection interceptor to set SQLite pragmas for optimal performance and reliability
/// </summary>
public class SqlitePragmaInterceptor : DbConnectionInterceptor
{
    public override void ConnectionOpened(DbConnection connection, ConnectionEndEventData eventData)
    {
        if (connection.GetType().Name == "SqliteConnection")
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                PRAGMA journal_mode=WAL;
                PRAGMA synchronous=NORMAL;
                PRAGMA foreign_keys=ON;
                PRAGMA busy_timeout=5000;
            ";
            command.ExecuteNonQuery();
        }

        base.ConnectionOpened(connection, eventData);
    }

    public override async Task ConnectionOpenedAsync(DbConnection connection, ConnectionEndEventData eventData, CancellationToken cancellationToken = default)
    {
        if (connection.GetType().Name == "SqliteConnection")
        {
            using var command = connection.CreateCommand();
            command.CommandText = @"
                PRAGMA journal_mode=WAL;
                PRAGMA synchronous=NORMAL;
                PRAGMA foreign_keys=ON;
                PRAGMA busy_timeout=5000;
            ";
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        await base.ConnectionOpenedAsync(connection, eventData, cancellationToken);
    }
}
