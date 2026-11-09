using Microsoft.EntityFrameworkCore;
using SmsService.Api.Models;

namespace SmsService.Api.Data;

public sealed class SmsDbContext(DbContextOptions<SmsDbContext> options) : DbContext(options)
{
    public DbSet<SmsMessage> Messages => Set<SmsMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var message = modelBuilder.Entity<SmsMessage>();
        message.HasKey(x => x.Id);
        message.Property(x => x.To).HasMaxLength(20).IsRequired();
        message.Property(x => x.Body).HasMaxLength(500).IsRequired();
        message.Property(x => x.Status).HasConversion<string>().HasMaxLength(20);
        message.Property(x => x.ProviderMessageId).HasMaxLength(100);
        message.Property(x => x.FailureReason).HasMaxLength(500);
        message.HasIndex(x => x.Status);
        message.HasIndex(x => x.CreatedAtUtc);
    }
}
