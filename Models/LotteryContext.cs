using Microsoft.EntityFrameworkCore;

namespace Lottery.Models;

public class LotteryContext : DbContext
{
    public LotteryContext(DbContextOptions options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
        this.ChangeTracker.LazyLoadingEnabled = false;
    }

    public virtual DbSet<Ticket> Tickets { get; set; }
    public virtual DbSet<Draw> Draws { get; set; }
    public virtual DbSet<Status> Status { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(x => x.TicketId);
            entity.Property(x=>x.TicketId)
                // .HasColumnName("identifier")
                // .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();
            entity
                .HasOne(x => x.Contestant)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.ContestantId);

            entity
                .HasOne(x => x.Draw)
                .WithMany(x => x.Tickets)
                .HasForeignKey(x => x.DrawId);
        });
        modelBuilder.Entity<Draw>(entity => { entity.HasKey(x => x.DrawId); });
        modelBuilder.Entity<Contestant>(entity =>
        {
            entity.HasKey(x => x.ContestantId);
            entity.Property(x=>x.ContestantId)
                // .HasColumnName("identifier")
                // .HasColumnType("uuid")
                .HasDefaultValueSql("uuid_generate_v4()")
                .IsRequired();
        });
        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(x => x.StatusId);
        });
    }
}