using Elsa.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elsa.Persistence.EntityFrameworkCore.Extensions
{
    public static class ElsaOptionsExtensions
    {
        public static ElsaOptions AddPostgreSqlLockProvider(this ElsaOptions options, string connectionString)
        {
            return options.UseDistributedLockProvider(sp => new AdvisoryLockManager(connectionString, sp.GetRequiredService<ILogger<AdvisoryLockManager>>()));
        }
    }
}
