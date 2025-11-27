using MAJESTIC_GOLDEN_Api.BLL.Services.Classes;
using MAJESTIC_GOLDEN_Api.BLL.Services.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Classes;
using MAJESTIC_GOLDEN_Api.DAL.Repositories.Interfaces;
using MAJESTIC_GOLDEN_Api.DAL.Utils;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace MAJESTIC_GOLDEN_Api.PLL
{
    internal static class AppConfiguration
    {
        internal static void Config(this IServiceCollection services) {


            // Repositories
           services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
           services.AddScoped<IUserRepository, UserRepository>();
           services.AddScoped<IBranchRepository, BranchRepository>();
           services.AddScoped<IPatientRepository, PatientRepository>();
           services.AddScoped<IAppointmentRepository, AppointmentRepository>();
           services.AddScoped<IInvoiceRepository, InvoiceRepository>();
           services.AddScoped<ILabRequestRepository, LabRequestRepository>();
           services.AddScoped<ITreatmentCaseRepository, TreatmentCaseRepository>();
           services.AddScoped<ILaboratoryRepository, LaboratoryRepository>();
           services.AddScoped<IAuditLogRepository, AuditLogRepository>();
           services.AddScoped<IFileRepository, FileRepository>();

            // Services
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IBranchService, BranchService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IServiceManagementService, ServiceManagementService>();
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IInvoiceDocumentService, InvoiceDocumentService>();
            services.AddScoped<ILabRequestService, LabRequestService>();
            services.AddScoped<ICaseTransferService, CaseTransferService>();
            services.AddScoped<IDentalChartService, DentalChartService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<ITreatmentCaseService, TreatmentCaseService>();
            services.AddScoped<ILaboratoryService, LaboratoryService>();
            services.AddScoped<IAuditLogger, AuditLogger>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<ICheckOutService, CheckOutService>();

            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<ISeedData, SeedData>();
            services.AddScoped<IEmailSender, EmailSender>();
        }
    }
}
