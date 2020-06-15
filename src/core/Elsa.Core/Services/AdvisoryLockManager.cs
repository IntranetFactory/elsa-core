using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using System.Data;
using NpgsqlTypes;

namespace Elsa.Services
{
    public class AdvisoryLockManager : IAdvisoryLockManager
    {
        private readonly ILogger<AdvisoryLockManager> logger;
        // connection string should probably be moved from here
        private readonly string connectionString = "Server=localhost;Database=Elsa;Port=5432;User Id=postgres;Password=adenin;";

        public AdvisoryLockManager(ILogger<AdvisoryLockManager> logger)
        {
            this.logger = logger;
        }

        public async Task<bool> Lock(string name, CancellationToken cancellationToken = default)
        {
            bool lockAcquired = false;

            try
            {
                var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                var hashedName = await HashName(name, connection, cancellationToken);

                NpgsqlCommand command = new NpgsqlCommand("pg_try_advisory_lock", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(hashedName);
                var advisoryLockReturnValue = command.Parameters.Add("ReturnValue", NpgsqlDbType.Boolean);
                advisoryLockReturnValue.Direction = ParameterDirection.Output;
                await command.ExecuteNonQueryAsync(cancellationToken);
                lockAcquired = Convert.ToBoolean(advisoryLockReturnValue.Value);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Exception occured while acquiring the advisory lock.");
            }

            return lockAcquired;
        }

        public async Task Unlock(string name, CancellationToken cancellationToken = default)
        {
            try
            {
                var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                var hashedName = await HashName(name, connection, cancellationToken);

                NpgsqlCommand command = new NpgsqlCommand("pg_advisory_unlock", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(hashedName);
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Exception occured while removing the advisory lock.");
            }
        }

        private async Task<object> HashName(string name, NpgsqlConnection connection, CancellationToken cancellationToken = default)
        {
            NpgsqlCommand command = new NpgsqlCommand("hashtext", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue(name);
            var hashedName = command.Parameters.Add("ReturnValue", NpgsqlDbType.Integer);
            hashedName.Direction = ParameterDirection.Output;
            await command.ExecuteNonQueryAsync(cancellationToken);

            return hashedName.Value;
        }
    }
}
