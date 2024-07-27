using Core.Entities.Common;
using Core.Entities.SpaceTraderModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class JsDbContext : DbContext
    {
        public DbSet<HighScore> HighScores { get; set; }

        public JsDbContext(DbContextOptions<JsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        /// <summary>
        /// Save all changes that are being  tracked
        /// </summary>
        /// <returns>Integer</returns>
        public override int SaveChanges()
        {
            // call BaseEntity.SetUpdated for every modified entity
            var entities = ChangeTracker.Entries();

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Modified)
                {
                    var currentEntity = (BaseEntity)entity.Entity;
                    currentEntity.SetUpdated();
                }
            }

            return base.SaveChanges();
        }

        /// <summary>
        /// Save changes asynchronously
        /// </summary>
        /// <param name="cancellationToken">Cancellation token of type CancellationToken</param>
        /// <returns>Task of type int</returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
