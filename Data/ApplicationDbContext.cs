using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CBSESchool.Models;

namespace CBSESchool.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<SchoolClass> Classes { get; set; } = null!;
        public DbSet<Subject> Subjects { get; set; } = null!;
        public DbSet<Exam> Exams { get; set; } = null!;
        public DbSet<Mark> Marks { get; set; } = null!;
        public DbSet<Notice> Notices { get; set; } = null!;
        public DbSet<FeeStructure> FeeStructures { get; set; } = null!;
        public DbSet<StudentFee> StudentFees { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure decimal precisions
            builder.Entity<FeeStructure>()
                .Property(f => f.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Entity<StudentFee>()
                .Property(f => f.AmountPaid)
                .HasColumnType("decimal(18,2)");

            // Delete behaviors to prevent SQL Server multiple cascade path cycles
            builder.Entity<Subject>()
                .HasOne(s => s.SchoolClass)
                .WithMany(c => c.Subjects)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Student>()
                .HasOne(s => s.SchoolClass)
                .WithMany(c => c.Students)
                .HasForeignKey(s => s.ClassId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Mark>()
                .HasOne(m => m.Student)
                .WithMany(s => s.Marks)
                .HasForeignKey(m => m.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Mark>()
                .HasOne(m => m.Exam)
                .WithMany(e => e.Marks)
                .HasForeignKey(m => m.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Mark>()
                .HasOne(m => m.Subject)
                .WithMany()
                .HasForeignKey(m => m.SubjectId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<StudentFee>()
                .HasOne(f => f.Student)
                .WithMany(s => s.Fees)
                .HasForeignKey(f => f.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentFee>()
                .HasOne(f => f.FeeStructure)
                .WithMany()
                .HasForeignKey(f => f.FeeStructureId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
