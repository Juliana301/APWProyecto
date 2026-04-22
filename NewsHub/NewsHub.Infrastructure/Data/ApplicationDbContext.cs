using Microsoft.EntityFrameworkCore;
using NewsHub.Domain.Entities;
using NewsHub.Domain.Entities.CatchError;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsHub.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region DbSets

        public DbSet<UserEnt> Users => Set<UserEnt>();
        public DbSet<RoleEnt> Roles => Set<RoleEnt>();
        public DbSet<UserRoleEnt> UserRoles => Set<UserRoleEnt>();

        public DbSet<SourceEnt> Sources => Set<SourceEnt>();
        public DbSet<SourceItemEnt> SourceItems => Set<SourceItemEnt>();
        public DbSet<SecretEnt> Secrets => Set<SecretEnt>();

        public DbSet<LogEnt> Logs => Set<LogEnt>();
        public DbSet<ImportHistoryEnt> ImportHistories => Set<ImportHistoryEnt>();
        
        public DbSet<LogErrorEnt> LogErrors => Set<LogErrorEnt>();

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
