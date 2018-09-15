using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using N17Solutions.Semaphore.Data.Conventions;
using N17Solutions.Semaphore.Data.Extensions;
using N17Solutions.Semaphore.Domain.Model;

namespace N17Solutions.Semaphore.Data.Context
{
    public class SemaphoreContext : DbContext
    {
        #region Aggregate Roots
        public DbSet<Feature> Features { get; set; }
        public DbSet<Signal> Signals { get; set; }
        #endregion
        
        private readonly Assembly _efAssembly;
        
        public SemaphoreContext(DbContextOptions options) : base(options)
        {
            _efAssembly = typeof(SemaphoreContext).Assembly;
        }

        public SemaphoreContext(DbContextOptions options, Assembly efAssembly) : base(options)
        {
            _efAssembly = efAssembly;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnSavingChanges();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override int SaveChanges()
        {
            OnSavingChanges();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            OnSavingChanges();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            OnSavingChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            base.OnConfiguring(optionsBuilder);

            optionsBuilder.ReplaceService<IEntityMaterializerSource, DateTimeMaterializerSource>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder == null || _efAssembly == null)
                return;

            base.OnModelCreating(modelBuilder);
            modelBuilder.UseEntityTypeConfiguration(_efAssembly);
            modelBuilder.RemovePluralizingTableNameConvention();
        }

        private void OnSavingChanges()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                var entryClosure = entry;

                if (entryClosure.State != EntityState.Added && entryClosure.State != EntityState.Modified)
                    continue;

                if (!(entryClosure.Entity is ITimestampedEntity entity))
                    continue;

                entity.DateLastUpdated = now;

                if (entryClosure.State == EntityState.Added)
                    entity.DateCreated = now;
            }
        }
    }
}