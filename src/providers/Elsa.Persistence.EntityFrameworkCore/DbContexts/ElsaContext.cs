using System.Collections.Generic;
using Elsa.Models;
using Elsa.Persistence.EntityFrameworkCore.CustomSchema;
using Elsa.Persistence.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NodaTime;
using NodaTime.Serialization.JsonNet;

namespace Elsa.Persistence.EntityFrameworkCore.DbContexts
{
    public class ElsaContext : DbContext
    {
        private readonly JsonSerializerSettings serializerSettings;

        public ElsaContext(DbContextOptions<ElsaContext> options) : base(options)
        {
            serializerSettings = new JsonSerializerSettings().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            DbContextCustomSchema = options.GetDbContextCustomSchema();
        }

        /// <summary>
        /// Constructor to initialise a new <see cref="ElsaContext"/> that will be ignored by Dependency Injection.
        /// Use this constructor when creating derived classes, i.e. for each database provider implementation.
        /// </summary>
        /// <param name="options"></param>
        /// <remarks>Protected for following reason: https://github.com/aspnet/EntityFramework.Docs/issues/594</remarks>
        protected ElsaContext(DbContextOptions options) : base(options)
        {
            serializerSettings = new JsonSerializerSettings().ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            DbContextCustomSchema = options.GetDbContextCustomSchema();
        }

        /// <summary>
        /// The CustomSchemaModelCacheKeyFactory will not resolve services from the DI container for constructor injection
        /// so this is necessary in order to set the custom schema for the Model Cache.
        /// </summary>
        internal IDbContextCustomSchema DbContextCustomSchema { get; }
        public DbSet<WorkflowDefinitionEntity> WorkflowDefinitions { get; set; }
        public DbSet<WorkflowDefinitionVersionEntity> WorkflowDefinitionVersions { get; set; }
        public DbSet<WorkflowInstanceEntity> WorkflowInstances { get; set; }
        public DbSet<WorkflowInstanceTaskEntity> WorkflowInstanceTasks { get; set; }
        public DbSet<WorkflowDefinitionActivityEntity> WorkflowDefinitionActivities { get; set; }
        public DbSet<WorkflowDefinitionConnectionEntity> WorkflowDefinitionConnections { get; set; }
        public DbSet<WorkflowInstanceLogEntity> WorkflowInstanceLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureCustomSchema(modelBuilder);
            base.OnModelCreating(modelBuilder);
            ConfigureWorkflowDefinition(modelBuilder);
            ConfigureWorkflowDefinitionVersion(modelBuilder);
            ConfigureWorkflowInstance(modelBuilder);
            ConfigureWorkflowDefinitionActivity(modelBuilder);
            ConfigureWorkflowInstanceTask(modelBuilder);
            ConfigureWorkflowDefinitionConnection(modelBuilder);
            ConfigureExecutionLogEntryEntity(modelBuilder);
        }

        private void ConfigureWorkflowDefinition(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<WorkflowDefinitionEntity>();
            entity.HasMany(x => x.WorkflowDefinitionVersions).WithOne(x => x.WorkflowDefinition);
        }

        private void ConfigureWorkflowDefinitionVersion(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<WorkflowDefinitionVersionEntity>();

            entity.Property(x => x.Id).UseIdentityColumn();
            entity.Property(x => x.DefinitionId);
            entity.Property(x => x.Variables).HasConversion(x => Serialize(x), x => Deserialize<Variables>(x));
            entity.HasMany(x => x.Activities).WithOne(x => x.WorkflowDefinitionVersion);
            entity.HasMany(x => x.Connections).WithOne(x => x.WorkflowDefinitionVersion);
            entity.HasOne(x => x.WorkflowDefinition).WithMany(x => x.WorkflowDefinitionVersions).HasForeignKey(x => x.DefinitionId);
        }

        private void ConfigureCustomSchema(ModelBuilder modelBuilder)
        {            
            if (DbContextCustomSchema != null && DbContextCustomSchema.UseCustomSchema)
            {
                modelBuilder.HasDefaultSchema(DbContextCustomSchema.Schema);

                // Apply the custom mapping to support the non-default schema to the types in used in this context.
                modelBuilder.ApplyConfiguration(new SchemaEntityTypeConfiguration<WorkflowDefinitionActivityEntity>(DbContextCustomSchema));
                modelBuilder.ApplyConfiguration(new SchemaEntityTypeConfiguration<WorkflowInstanceTaskEntity>(DbContextCustomSchema));
                modelBuilder.ApplyConfiguration(new SchemaEntityTypeConfiguration<WorkflowDefinitionConnectionEntity>(DbContextCustomSchema));
                modelBuilder.ApplyConfiguration(new SchemaEntityTypeConfiguration<WorkflowDefinitionEntity>(DbContextCustomSchema));
                modelBuilder.ApplyConfiguration(new SchemaEntityTypeConfiguration<WorkflowDefinitionVersionEntity>(DbContextCustomSchema));
                modelBuilder.ApplyConfiguration(new SchemaEntityTypeConfiguration<WorkflowInstanceEntity>(DbContextCustomSchema));
            }
        }

        private void ConfigureWorkflowInstance(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<WorkflowInstanceEntity>();

            entity.Property(x => x.Id).UseIdentityColumn();
            entity.Property(x => x.Status).HasConversion<string>();

            entity
                .HasMany(x => x.ExecutionLog)
                .WithOne(x => x.WorkflowInstance);

            entity
                .Property(x => x.Fault)
                .HasConversion(
                    x => Serialize(x),
                    x => Deserialize<WorkflowFault>(x)
                );
            
            entity
                .HasMany(x => x.WorkflowInstanceTasks)
                .WithOne(x => x.WorkflowInstance);
        }

        private void ConfigureWorkflowDefinitionActivity(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<WorkflowDefinitionActivityEntity>();

            entity.Property(x => x.Id).UseIdentityColumn();

            entity
                .Property(x => x.State)
                .HasConversion(x => Serialize(x), x => Deserialize<Variables>(x));

            // from activity instance table
            entity
                .Property(x => x.Output)
                .HasConversion(x => Serialize(x), x => Deserialize<Variable>(x));
        }
        
        private void ConfigureWorkflowDefinitionConnection(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<WorkflowDefinitionConnectionEntity>();

            entity.Property(x => x.Id).UseIdentityColumn();
        }
        
        private void ConfigureWorkflowInstanceTask(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<WorkflowInstanceTaskEntity>();

            entity.HasKey(x => x.Id);

            entity
                .Property(x => x.Input)
                .HasConversion(x => Serialize(x), x => Deserialize<Variable>(x));
        }

        private void ConfigureExecutionLogEntryEntity(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<WorkflowInstanceLogEntity>();

            entity.HasKey(x => x.Id);

            entity
                .HasOne(x => x.WorkflowInstance)
                .WithMany(x => x.ExecutionLog);
        }

        private string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, serializerSettings);
        }

        private T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, serializerSettings);
        }
    }
}