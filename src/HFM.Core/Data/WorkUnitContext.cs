using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HFM.Core.Data
{
    public partial class WorkUnitContext : DbContext
    {
        public DbSet<WorkUnitEntity> WorkUnits { get; set; }
        public DbSet<ProteinEntity> Proteins { get; set; }
        public DbSet<ClientEntity> Clients { get; set; }
        public DbSet<WorkUnitFrameEntity> WorkUnitFrames { get; set; }
        public DbSet<VersionEntity> Versions { get; set; }

        private readonly string _connectionString;
        private readonly Action<string> _logTo;

        public WorkUnitContext(string connectionString) : this(connectionString, null)
        {

        }

        public WorkUnitContext(string connectionString, Action<string> logTo)
        {
            _connectionString = connectionString;
            _logTo = logTo;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Data Source=WorkUnits.db");
            optionsBuilder.UseSqlite(_connectionString);
            if (_logTo is not null)
            {
                optionsBuilder.LogTo(_logTo, LogLevel.Information);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WorkUnitEntity>()
                .HasOne(x => x.Protein)
                .WithMany()
                .HasForeignKey(x => x.ProteinID);

            modelBuilder.Entity<WorkUnitEntity>()
                .HasOne(x => x.Client)
                .WithMany()
                .HasForeignKey(x => x.ClientID);

            modelBuilder.Entity<WorkUnitEntity>()
                .HasMany(x => x.Frames)
                .WithOne()
                .HasForeignKey(x => x.WorkUnitID);

            modelBuilder.Entity<WorkUnitFrameEntity>()
                .HasKey(x => new { x.WorkUnitID, x.FrameID });
        }
    }
}
