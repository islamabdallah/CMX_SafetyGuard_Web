using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using WebDriverViolation.Models.Models;
using WebDriverViolation.Models.Models.MasterModels;

namespace Repository.EntityFramework
{
    public class APPDBContext : DbContext
    {
        public APPDBContext(DbContextOptions<APPDBContext> options) : base(options)
        {

        }

        public DbSet<Violation> Violations { get; set; }

        public DbSet<ViolationType> ViolationTypes { get; set; }

        public DbSet<TruckRunningTracking> TruckRunningTrackings { get; set; }

        public DbSet<ViolationNotification> ViolationNotification { get; set; }

        public DbSet<UserViolationNotification> UserViolationNotification { get; set; }

        public DbSet<ConfirmationStatus> ConfirmationStatuses { get; set; }

        public DbSet<Employee> Employees { get; set; }

         public DbSet<Truck> Trucks { get; set; }
        public DbSet<ViolationTypeAccuracyLavel> ViolationTypeAccuracyLavels { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserViolationNotification>().HasKey(UN => new { UN.ViolationNotificationId, UN.userId });
            //modelBuilder.Entity<ViolationTypeAccuracyLavel>().HasKey(UN => new { UN.LevelId, UN.ViolationTypeId });
        }

    }


    public class APPDbContextFactory : IDesignTimeDbContextFactory<APPDBContext>
    {
        //private readonly IConfiguration _configuration;

        //public APPDbContextFactory(IConfiguration configuration)
        //{
        //        _configuration = configuration;
        //}
        public APPDBContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<APPDBContext>();
            optionsBuilder.UseSqlServer("Data source=USCLDEGYVMP01\\CEMEXSQLSERVER;Database=TakeFive;integrated security=True;TrustServerCertificate=True");

            return new APPDBContext(optionsBuilder.Options);
        }
    }
}
