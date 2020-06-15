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

        // Lock is used to create/acquire an advisory lock for the specified workflow_instance_xxx in the database by calling pg_try_advisory_lock command.
        // If lock is succesfully acquired - it returns true, otherwise - it returns false.
        public async Task<bool> Lock(string name, CancellationToken cancellationToken = default)
        {
            bool lockAcquired = false;

            try
            {
                // open database connection
                var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                // get integer value from hashed string "name"
                var hashedName = await HashName(name, connection, cancellationToken);
                // creates a command to be executed
                NpgsqlCommand command = new NpgsqlCommand("pg_try_advisory_lock", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(hashedName);
                // specifies the return value of pg_try_advisory_lock command and it's type
                var advisoryLockReturnValue = command.Parameters.Add("ReturnValue", NpgsqlDbType.Boolean);
                advisoryLockReturnValue.Direction = ParameterDirection.Output;
                // execute command
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
                // open database connection
                var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();
                // get integer value from hashed string "name"
                var hashedName = await HashName(name, connection, cancellationToken);
                // creates a command to be executed
                NpgsqlCommand command = new NpgsqlCommand("pg_advisory_unlock", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(hashedName);
                // execute command
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Exception occured while removing the advisory lock.");
            }
        }


        // HashName is used to return an integer value by hashing a string "name". 
        // This is used because advisory locking functions require integers instead of strings.
        private async Task<object> HashName(string name, NpgsqlConnection connection, CancellationToken cancellationToken = default)
        {
            // creates a command to be executed
            NpgsqlCommand command = new NpgsqlCommand("hashtext", connection);
            command.CommandType = CommandType.StoredProcedure;
            // provides parameter value to the command
            command.Parameters.AddWithValue(name);
            // specifies the return value of hashtext command and it's type
            var hashedName = command.Parameters.Add("ReturnValue", NpgsqlDbType.Integer);
            hashedName.Direction = ParameterDirection.Output;
            await command.ExecuteNonQueryAsync(cancellationToken);

            return hashedName.Value;
        }
    }
}
