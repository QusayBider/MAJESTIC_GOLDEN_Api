using MAJESTIC_GOLDEN_Api.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MAJESTIC_GOLDEN_Api.DAL.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<PatientTooth> PatientTeeth { get; set; }
        public DbSet<PatientAttachment> PatientAttachments { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PatientDebt> PatientDebts { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<CaseTransfer> CaseTransfers { get; set; }
        public DbSet<LabRequest> LabRequests { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Laboratory> Laboratories { get; set; }


        public DbSet<TreatmentCase> TreatmentCases { get; set; }
        public DbSet<CaseTreatment> CaseTreatments { get; set; }
        public DbSet<CaseDoctor> CaseDoctors { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            // Identity tables
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UsersRoles");

            builder.Ignore<IdentityUserClaim<string>>();
            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityUserToken<string>>();
            builder.Ignore<IdentityRoleClaim<string>>();

            // Configure relationships
            
            // ApplicationUser - Branch
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            // Patient - ApplicationUser (One-to-Zero-or-One)
            // Every Patient is a User, UserId is both PK and FK
            builder.Entity<Patient>()
                .HasKey(p => p.UserId);
            
            builder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.PatientProfile)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Laboratory>(entity =>
            {
                entity.ToTable("Laboratory");
                entity.HasKey(l => l.Id);

                entity.HasOne(l => l.User)
                    .WithOne(u => u.LaboratoryProfile)
                    .HasForeignKey<Laboratory>(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(l => l.Requests)
                    .WithOne(r => r.Laboratory)
                    .HasForeignKey(r => r.LaboratoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PatientTooth - Composite Key Configuration
            builder.Entity<PatientTooth>()
                .HasKey(pt => new { pt.ToothId, pt.PatientUserId });

            builder.Entity<PatientTooth>()
                .HasOne(pt => pt.Patient)
                .WithMany(p => p.PatientTeeth)
                .HasForeignKey(pt => pt.PatientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PatientTooth>()
                .HasOne(pt => pt.Doctor)
                .WithMany(d => d.PatientTeeth)
                .HasForeignKey(pt => pt.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            // PatientAttachment
            builder.Entity<PatientAttachment>()
                .HasOne(pa => pa.Patient)
                .WithMany(p => p.Attachments)
                .HasForeignKey(pa => pa.PatientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Invoice
            builder.Entity<Invoice>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PatientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Invoice>()
                .HasOne(i => i.Doctor)
                .WithMany(d => d.InvoicesAsDoctor)
                .HasForeignKey(i => i.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);

            // InvoiceItem
            builder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Invoice)
                .WithMany(i => i.Items)
                .HasForeignKey(ii => ii.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<InvoiceItem>()
                .HasOne(ii => ii.Service)
                .WithMany(s => s.InvoiceItems)
                .HasForeignKey(ii => ii.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Payment
            builder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(i => i.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // PatientDebt
            builder.Entity<PatientDebt>()
                .HasOne(pd => pd.Patient)
                .WithMany(p => p.Debts)
                .HasForeignKey(pd => pd.PatientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PatientDebt>()
                .HasOne(pd => pd.Invoice)
                .WithOne(i => i.Debt)
                .HasForeignKey<PatientDebt>(pd => pd.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment
            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.AppointmentsAsDoctor)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Branch)
                .WithMany(b => b.Appointments)
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            // CaseTransfer
            builder.Entity<CaseTransfer>()
                .HasOne(ct => ct.Patient)
                .WithMany(p => p.CaseTransfers)
                .HasForeignKey(ct => ct.PatientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CaseTransfer>()
                .HasOne(ct => ct.FromDoctor)
                .WithMany(d => d.CaseTransfersFrom)
                .HasForeignKey(ct => ct.FromDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CaseTransfer>()
                .HasOne(ct => ct.ToDoctor)
                .WithMany(d => d.CaseTransfersTo)
                .HasForeignKey(ct => ct.ToDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // LabRequest
            builder.Entity<LabRequest>()
                .HasOne(lr => lr.Patient)
                .WithMany(p => p.LabRequests)
                .HasForeignKey(lr => lr.PatientUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LabRequest>()
                .HasOne(lr => lr.Laboratory)
                .WithMany(l => l.Requests)
                .HasForeignKey(lr => lr.LaboratoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<LabRequest>()
                .HasOne(lr => lr.Doctor)
                .WithMany(d => d.LabRequests)
                .HasForeignKey(lr => lr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Decimal precision
            builder.Entity<Service>()
                .Property(s => s.BasePrice)
                .HasPrecision(18, 2);

            builder.Entity<Invoice>()
                .Property(i => i.SubTotal)
                .HasPrecision(18, 2);

            builder.Entity<Invoice>()
                .Property(i => i.Discount)
                .HasPrecision(18, 2);

            builder.Entity<Invoice>()
                .Property(i => i.Tax)
                .HasPrecision(18, 2);

            builder.Entity<Invoice>()
                .Property(i => i.Total)
                .HasPrecision(18, 2);

            builder.Entity<Invoice>()
                .Property(i => i.PaidAmount)
                .HasPrecision(18, 2);

            builder.Entity<Invoice>()
                .Property(i => i.RemainingAmount)
                .HasPrecision(18, 2);

            builder.Entity<InvoiceItem>()
                .Property(ii => ii.UnitPrice)
                .HasPrecision(18, 2);

            builder.Entity<InvoiceItem>()
                .Property(ii => ii.Discount)
                .HasPrecision(18, 2);

            builder.Entity<InvoiceItem>()
                .Property(ii => ii.Total)
                .HasPrecision(18, 2);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            builder.Entity<PatientDebt>()
                .Property(pd => pd.AmountDue)
                .HasPrecision(18, 2);

            builder.Entity<LabRequest>()
                .Property(lr => lr.Cost)
                .HasPrecision(18, 2);

            // Indexes for better performance
            // PhoneNumber index removed to allow duplicate phone numbers across users
            // builder.Entity<ApplicationUser>()
            //     .HasIndex(u => u.PhoneNumber);

            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email);

            builder.Entity<Appointment>()
                .HasIndex(a => a.AppointmentDateTime);

            builder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();

            builder.Entity<LabRequest>()
                .HasIndex(lr => lr.RequestNumber)
                .IsUnique();

            // ========== Treatment Cases Configuration ==========
            
            // TreatmentCase - Patient
            builder.Entity<TreatmentCase>()
                .HasOne(tc => tc.Patient)
                .WithMany(p => p.TreatmentCases)
                .HasForeignKey(tc => tc.PatientUserId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // TreatmentCase - Branch
            builder.Entity<TreatmentCase>()
                .HasOne(tc => tc.Branch)
                .WithMany()
                .HasForeignKey(tc => tc.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // TreatmentCase - Invoice (One-to-One optional)
            builder.Entity<TreatmentCase>()
                .HasOne(tc => tc.Invoice)
                .WithOne()
                .HasForeignKey<TreatmentCase>(tc => tc.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // CaseTreatment - TreatmentCase
            builder.Entity<CaseTreatment>()
                .HasOne(ct => ct.Case)
                .WithMany(tc => tc.Treatments)
                .HasForeignKey(ct => ct.CaseId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // CaseTreatment - Service
            builder.Entity<CaseTreatment>()
                .HasOne(ct => ct.Service)
                .WithMany()
                .HasForeignKey(ct => ct.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);
            
            // CaseTreatment - Doctor
            builder.Entity<CaseTreatment>()
                .HasOne(ct => ct.Doctor)
                .WithMany()
                .HasForeignKey(ct => ct.DoctorId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // CaseDoctor - Composite Key
            builder.Entity<CaseDoctor>()
                .HasKey(cd => new { cd.CaseId, cd.DoctorId });
            
            // CaseDoctor - TreatmentCase
            builder.Entity<CaseDoctor>()
                .HasOne(cd => cd.Case)
                .WithMany(tc => tc.CaseDoctors)
                .HasForeignKey(cd => cd.CaseId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // CaseDoctor - ApplicationUser
            builder.Entity<CaseDoctor>()
                .HasOne(cd => cd.Doctor)
                .WithMany()
                .HasForeignKey(cd => cd.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Decimal Precision for CaseTreatment
            builder.Entity<CaseTreatment>()
                .Property(ct => ct.UnitPrice)
                .HasPrecision(18, 2);
            
            builder.Entity<CaseTreatment>()
                .Property(ct => ct.TotalPrice)
                .HasPrecision(18, 2);
            
            // Indexes
            builder.Entity<TreatmentCase>()
                .HasIndex(tc => tc.CaseNumber)
                .IsUnique();
            
            builder.Entity<TreatmentCase>()
                .HasIndex(tc => tc.CaseDate);
            
            builder.Entity<TreatmentCase>()
                .HasIndex(tc => tc.Status);
        }
    }
}
