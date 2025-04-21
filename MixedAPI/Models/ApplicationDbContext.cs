using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MixedAPI.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ContactForm> ContactForms { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseDetail> CourseDetails { get; set; }
        public DbSet<DocumentAttachment> DocumentAttachments { get; set; }
        public DbSet<ContactFormForPanel> ContactFormForPanels { get; set; }
        public DbSet<ContactFormForCompany> ContactFormForCompanies { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContactForm>()
                .HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<ContactFormForPanel>()
           .Property(f => f.Status)
           .HasConversion<int>();

            modelBuilder.Entity<ContactFormForPanel>()
                .Property(f => f.Priority)
                .HasConversion<int>();
        }
    }
}
