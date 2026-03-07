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

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();

        public DbSet<Source> Sources => Set<Source>();
        public DbSet<SourceItem> SourceItems => Set<SourceItem>();
        public DbSet<Secret> Secrets => Set<Secret>();

        public DbSet<Log> Logs => Set<Log>();
        public DbSet<ImportHistory> ImportHistories => Set<ImportHistory>();
        
        public DbSet<LogError> LogErrors => Set<LogError>();

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
