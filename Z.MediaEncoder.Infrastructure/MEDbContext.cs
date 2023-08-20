using MediatR;
using Microsoft.EntityFrameworkCore;
using Z.Infrastructure.EFCore;
using Z.MediaEncoder.Domain.Entities;

namespace Z.MediaEncoder.Infrastructure
{
    public class MEDbContext : BaseDbContext
    {
        public DbSet<EncodingItem> EncodingItems { get; set; }

        public MEDbContext(DbContextOptions options, IMediator? mediator) : base(options, mediator)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.EnableSoftDeletionGlobalFilter();
        }
    }
}
