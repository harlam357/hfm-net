﻿using System.Data;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HFM.Core.Data;

public class WorkUnitContextOptions
{
    public SqliteConnection Connection { get; set; }

    public string ConnectionString { get; set; }

    public Action<string> LogTo { get; set; }

    public LogLevel LogLevel { get; set; } = LogLevel.Debug;
}

public class WorkUnitContextOptionsBuilder
{
    public WorkUnitContextOptions Options { get; } = new();

    public WorkUnitContextOptionsBuilder UseConnection(SqliteConnection connection)
    {
        Options.Connection = connection;
        return this;
    }

    public WorkUnitContextOptionsBuilder UseConnectionString(string connectionString)
    {
        Options.ConnectionString = connectionString;
        return this;
    }

    public WorkUnitContextOptionsBuilder LogTo(Action<string> action, LogLevel minimumLevel = LogLevel.Debug)
    {
        Options.LogTo = action;
        Options.LogLevel = minimumLevel;
        return this;
    }
}

public partial class WorkUnitContext : DbContext
{
    public DbSet<WorkUnitEntity> WorkUnits { get; set; }
    public DbSet<ProteinEntity> Proteins { get; set; }
    public DbSet<ClientEntity> Clients { get; set; }
    public DbSet<PlatformEntity> Platforms { get; set; }
    public DbSet<WorkUnitFrameEntity> WorkUnitFrames { get; set; }
    public DbSet<VersionEntity> Versions { get; set; }

    private readonly WorkUnitContextOptions _options;
    private SqliteConnection _connection;
    private bool _contextOwnsConnection;

    public WorkUnitContext()
    {
        _options = new WorkUnitContextOptions();
    }

    public WorkUnitContext(SqliteConnection connection)
    {
        _options = new WorkUnitContextOptions { Connection = connection };
    }

    public WorkUnitContext(string connectionString)
    {
        _options = new WorkUnitContextOptions { ConnectionString = connectionString };
    }

    public WorkUnitContext(Action<WorkUnitContextOptionsBuilder> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(configureOptions);

        var builder = new WorkUnitContextOptionsBuilder();
        configureOptions(builder);
        _options = builder.Options;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _connection = _options.Connection;
        if (_connection is null)
        {
            _connection = new SqliteConnection(_options.ConnectionString);
            _connection.Open();
            _contextOwnsConnection = true;
        }
        else if (_connection.State == ConnectionState.Closed)
        {
            _connection.Open();
            _contextOwnsConnection = true;
        }

        _connection.CreateFunction(nameof(ToSlotName), (Func<string, int?, string>)ToSlotName);
        _connection.CreateFunction(nameof(ToSlotType), (Func<string, string>)ToSlotType);
        _connection.CreateFunction(nameof(CalculatePPD),
            (Func<int, int, double, double, double, double, string, string, int, double>)CalculatePPD);
        _connection.CreateFunction(nameof(CalculateCredit),
            (Func<int, int, double, double, double, double, string, string, int, double>)CalculateCredit);

        optionsBuilder.UseSqlite(_connection);
        if (_options.LogTo is not null)
        {
            optionsBuilder.LogTo(_options.LogTo, _options.LogLevel);
        }

        base.OnConfiguring(optionsBuilder);
    }

    public override void Dispose()
    {
        if (_contextOwnsConnection)
        {
            _connection.Dispose();
        }
        base.Dispose();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkUnitEntity>(entity =>
        {
            // Ignore properties populated by UDF
            entity
                .Ignore(x => x.SlotName)
                .Ignore(x => x.SlotType)
                .Ignore(x => x.PPD)
                .Ignore(x => x.Credit);

            // Setup relationships
            entity
                .HasOne(x => x.Protein)
                .WithMany()
                .HasForeignKey(x => x.ProteinID);

            entity
                .HasOne(x => x.Client)
                .WithMany()
                .HasForeignKey(x => x.ClientID);

            entity
                .HasOne(x => x.Platform)
                .WithMany()
                .HasForeignKey(x => x.PlatformID);

            entity
                .HasMany(x => x.Frames)
                .WithOne()
                .HasForeignKey(x => x.WorkUnitID);
        });

        modelBuilder.Entity<ClientEntity>(entity =>
        {
            // Client NOT NULL properties
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.ConnectionString).IsRequired();
        });

        modelBuilder.Entity<PlatformEntity>(entity =>
        {
            // Platform NOT NULL properties
            entity.Property(x => x.ClientVersion).IsRequired();
            entity.Property(x => x.OperatingSystem).IsRequired();
            entity.Property(x => x.Implementation).IsRequired();
            entity.Property(x => x.Processor).IsRequired();
        });

        modelBuilder.Entity<VersionEntity>(entity =>
        {
            // Versions NOT NULL properties
            entity.Property(x => x.Version).IsRequired();
        });

        modelBuilder.Entity<WorkUnitFrameEntity>(entity =>
        {
            // WorkUnitFrame PK
            entity.HasKey(x => new { x.WorkUnitID, x.FrameID });
        });
    }
}
