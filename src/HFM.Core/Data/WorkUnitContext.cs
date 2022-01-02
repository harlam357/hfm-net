using Microsoft.EntityFrameworkCore;

namespace HFM.Core.Data
{
    public class WorkUnitContext : DbContext
    {
        public DbSet<WorkUnitEntity> WorkUnits { get; set; }
        public DbSet<ProteinEntity> Proteins { get; set; }
        public DbSet<ClientEntity> Clients { get; set; }
        public DbSet<WorkUnitFrameEntity> WorkUnitFrames { get; set; }

        private readonly string _connectionString;

        public WorkUnitContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlite("Data Source=WorkUnits.db");
            optionsBuilder.UseSqlite(_connectionString);

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
