using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StaffWork.Core.Models;
using System.Reflection.Emit;

namespace StaffWork.Core.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Department> Departments { get; set; }
        public DbSet<WorkType> WorkTypes { get; set; }
        public DbSet<WorkDaily> WorkDailies { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TaskModel> TaskModels { get; set; }
        public DbSet<TaskUser> TaskUsers { get; set; }
        public DbSet<TaskReminder> TaskReminders { get; set; }
        public DbSet<TaskFile> TaskFiles { get; set; }
        public DbSet<PersonalReminder> PersonalReminders { get; set; }
        public DbSet<PersonalReminderFile> PersonalReminderFiles { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            var cascadeFKs = builder.Model.GetEntityTypes()
              .SelectMany(t => t.GetForeignKeys())
              .Where(fk => fk.DeleteBehavior == DeleteBehavior.Cascade && !fk.IsOwnership);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;


            builder.Entity<TaskModel>()
                .HasMany(d => d.AssignedUsers)
                .WithOne(u => u.TaskModel)
                .HasForeignKey(u => u.TaskModelId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(builder);
        }
    }
}
