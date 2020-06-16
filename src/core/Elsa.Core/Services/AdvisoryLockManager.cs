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
    public class AdvisoryLockManager : IDistributedLockProvider
    {
        private readonly string connectionString;
        private readonly ILogger<AdvisoryLockManager> logger;
        private readonly IDictionary<string, NpgsqlConnection> locks = new Dictionary<string, NpgsqlConnection>();

        public AdvisoryLockManager(string connectionString, ILogger<AdvisoryLockManager> logger)
        {
            this.logger = logger;
            this.connectionString = connectionString;
        }

        // AcquireLock is used to create/acquire an advisory lock for the specified workflow_instance_xxx in the database by calling pg_try_advisory_lock command.
        // If lock is succesfully acquired - it returns true, otherwise - it returns false.
        public async Task<bool> AcquireLockAsync(string name, CancellationToken cancellationToken = default)
        {
            bool lockAcquired = false;
            // the string that we want to use as an advisory lock
            string advisoryLockString = "workflow_instance_" + name;
            // open database connection
            var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            try
            {
                // get integer value by hashing advisoryLockString
                var hashedName = await HashName(advisoryLockString, connection, cancellationToken);
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
                await connection.CloseAsync();
                logger.LogError(ex, $"Exception occured while acquiring the advisory lock.");
            }

            // save the connection in the dictionary if the lock is successfully acquired so that it can be used when releasing the lock
            if (lockAcquired)
            {
                locks[advisoryLockString] = connection;
                return true;
            }
            else
            {
                connection.Close();
                return false;
            }
        }

        public async Task ReleaseLockAsync(string name, CancellationToken cancellationToken = default)
        {
            // the string that we want to use as an advisory lock
            string advisoryLockString = "workflow_instance_" + name;
            // try to get the connection if it exists in the locks dictionary
            if (!locks.ContainsKey(advisoryLockString)) return;
            var connection = locks[advisoryLockString];
            if (connection == null) return;

            try
            {
                // get integer value by hashing advisoryLockString
                var hashedName = await HashName(advisoryLockString, connection, cancellationToken);
                // creates a command to be executed
                NpgsqlCommand command = new NpgsqlCommand("pg_advisory_unlock", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue(hashedName);
                // execute command
                await command.ExecuteNonQueryAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                await connection.CloseAsync();
                logger.LogError(ex, $"Exception occured while removing the advisory lock.");
            }
            finally
            {
                connection.Close();
                locks.Remove(advisoryLockString);
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
