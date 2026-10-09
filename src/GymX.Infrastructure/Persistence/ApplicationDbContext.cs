using GymX.Application.Common.Interfaces;
using GymX.Domain.Entities.Identity;
using GymX.Domain.Entities.Finance;
using GymX.Domain.Entities.Members;
using GymX.Domain.Entities.Packages;
using GymX.Domain.Entities.Employees;
using GymX.Domain.Entities.Attendance;
using GymX.Domain.Entities.Training;
using GymX.Domain.Entities.Payroll;
using GymX.Domain.Entities.Notifications;
using GymX.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymX.Infrastructure.Persistence
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        AuditableEntityInterceptor auditableEntityInterceptor,
        SoftDeleteInterceptor softDeleteInterceptor) : DbContext(options), IApplicationDbContext, IUnitOfWork
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(auditableEntityInterceptor, softDeleteInterceptor);
            base.OnConfiguring(optionsBuilder);
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
        public DbSet<FaceData> FaceData => Set<FaceData>();

        public DbSet<Member> Members => Set<Member>();
        public DbSet<MemberPackage> MemberPackages => Set<MemberPackage>();
        public DbSet<MemberCheckin> MemberCheckins => Set<MemberCheckin>();
        public DbSet<DayTicket> DayTickets => Set<DayTicket>();

        public DbSet<MembershipPackage> MembershipPackages => Set<MembershipPackage>();

        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Specialization> Specializations => Set<Specialization>();
        public DbSet<Trainer> Trainers => Set<Trainer>();
        public DbSet<TrainerDocument> TrainerDocuments => Set<TrainerDocument>();
        public DbSet<TrainingService> TrainingServices => Set<TrainingService>();

        public DbSet<ShiftDefinition> ShiftDefinitions => Set<ShiftDefinition>();
        public DbSet<EmployeeWorkSchedule> EmployeeWorkSchedules => Set<EmployeeWorkSchedule>();
        public DbSet<EmployeeAttendanceRecord> EmployeeAttendanceRecords => Set<EmployeeAttendanceRecord>();

        public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
        public DbSet<TrainingProgress> TrainingProgresses => Set<TrainingProgress>();
        public DbSet<TrainerReview> TrainerReviews => Set<TrainerReview>();

        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Payment> Payments => Set<Payment>();

        public DbSet<PenaltyRule> PenaltyRules => Set<PenaltyRule>();
        public DbSet<Violation> Violations => Set<Violation>();
        public DbSet<SalaryFormula> SalaryFormulas => Set<SalaryFormula>();
        public DbSet<PayrollPeriod> PayrollPeriods => Set<PayrollPeriod>();
        public DbSet<Payslip> Payslips => Set<Payslip>();

        public DbSet<Notification> Notifications => Set<Notification>();
    }
 }
