using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MAJESTIC_GOLDEN_Api.DAL.Data;
using MAJESTIC_GOLDEN_Api.DAL.Models;

namespace MAJESTIC_GOLDEN_Api.DAL.Utils
{
    public class SeedData : ISeedData
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public SeedData(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task DataSeedingAsync()
        {
            // Apply pending migrations
            if ((await _context.Database.GetPendingMigrationsAsync()).Any())
            {
               await _context.Database.MigrateAsync();
            }

            // Seed Branches
            if (!await _context.Branches.AnyAsync())
            {
                var branches = new List<Branch>
                {
                    new Branch
                    {
                        Name_En = "Main Branch - Downtown",
                        Name_Ar = "الفرع الرئيسي - وسط المدينة",
                        Address_En = "123 Main Street, Downtown",
                        Address_Ar = "١٢٣ شارع الرئيسي، وسط المدينة",
                        Phone = "+962-6-1234567",
                        Email = "main@majesticgolden.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Branch
                    {
                        Name_En = "West Branch - Mall Area",
                        Name_Ar = "الفرع الغربي - منطقة المول",
                        Address_En = "456 Mall Avenue, West District",
                        Address_Ar = "٤٥٦ شارع المول، المنطقة الغربية",
                        Phone = "+962-6-2345678",
                        Email = "west@majesticgolden.com",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                await _context.Branches.AddRangeAsync(branches);
            await _context.SaveChangesAsync();
            }

            // Seed Services
            if (!await _context.Services.AnyAsync())
            {
                var services = new List<Service>
                {
                    new Service
                    {
                        Name_En = "General Checkup",
                        Name_Ar = "فحص عام",
                        Description_En = "Complete dental examination and consultation",
                        Description_Ar = "فحص الأسنان الشامل والاستشارة",
                        BasePrice = 30.00m,
                        Category_En = "Consultation",
                        Category_Ar = "استشارة",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name_En = "Teeth Cleaning",
                        Name_Ar = "تنظيف الأسنان",
                        Description_En = "Professional teeth cleaning and polishing",
                        Description_Ar = "تنظيف وتلميع الأسنان المحترف",
                        BasePrice = 50.00m,
                        Category_En = "Cleaning",
                        Category_Ar = "تنظيف",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name_En = "Tooth Filling",
                        Name_Ar = "حشو الأسنان",
                        Description_En = "Composite or amalgam filling",
                        Description_Ar = "حشو كومبوزيت أو أمالغام",
                        BasePrice = 40.00m,
                        Category_En = "Restorative",
                        Category_Ar = "ترميمي",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name_En = "Root Canal Treatment",
                        Name_Ar = "علاج جذور الأسنان",
                        Description_En = "Complete root canal therapy",
                        Description_Ar = "علاج جذور كامل",
                        BasePrice = 150.00m,
                        Category_En = "Endodontics",
                        Category_Ar = "علاج الجذور",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name_En = "Tooth Extraction",
                        Name_Ar = "خلع الأسنان",
                        Description_En = "Simple or surgical tooth extraction",
                        Description_Ar = "خلع بسيط أو جراحي",
                        BasePrice = 60.00m,
                        Category_En = "Surgery",
                        Category_Ar = "جراحة",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name_En = "Teeth Whitening",
                        Name_Ar = "تبييض الأسنان",
                        Description_En = "Professional teeth whitening service",
                        Description_Ar = "خدمة تبييض الأسنان المحترفة",
                        BasePrice = 200.00m,
                        Category_En = "Cosmetic",
                        Category_Ar = "تجميلي",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name_En = "Dental Crown",
                        Name_Ar = "تاج الأسنان",
                        Description_En = "Ceramic or porcelain crown",
                        Description_Ar = "تاج سيراميك أو بورسلين",
                        BasePrice = 300.00m,
                        Category_En = "Prosthetic",
                        Category_Ar = "تعويضي",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name_En = "Dental Bridge",
                        Name_Ar = "جسر الأسنان",
                        Description_En = "Fixed dental bridge (3 units)",
                        Description_Ar = "جسر أسنان ثابت (٣ وحدات)",
                        BasePrice = 800.00m,
                        Category_En = "Prosthetic",
                        Category_Ar = "تعويضي",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name_En = "Dental Implant",
                        Name_Ar = "زراعة الأسنان",
                        Description_En = "Single tooth implant with crown",
                        Description_Ar = "زراعة سن واحد مع التاج",
                        BasePrice = 1200.00m,
                        Category_En = "Implantology",
                        Category_Ar = "زراعة",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Service
                    {
                        Name_En = "Orthodontic Consultation",
                        Name_Ar = "استشارة تقويم الأسنان",
                        Description_En = "Initial orthodontic assessment",
                        Description_Ar = "تقييم تقويم أولي",
                        BasePrice = 50.00m,
                        Category_En = "Orthodontics",
                        Category_Ar = "تقويم الأسنان",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }
                };
                await _context.Services.AddRangeAsync(services);
                await _context.SaveChangesAsync();
            }

            // Seed Demo Patients - removed for now
            // Patients now need to be created through ApplicationUser first with Patient role
            // Then Patient profile is created separately
            // This should be done through the API endpoints
        }

        public async Task IdentityRoleSeedingAsync()
        {
            // Seed Roles
            if (!await _roleManager.Roles.AnyAsync())
            {
                var roles = new List<string> { "HeadDoctor", "SubDoctor", "Receptionist", "Patient", "Appointments_Admin", "Patients_Admin" };
                foreach (var role in roles)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed Users
            if (!await _userManager.Users.AnyAsync())
            {
                var branch = await _context.Branches.FirstOrDefaultAsync();
                int? branchId = branch?.Id;

                var users = new List<(ApplicationUser User, string Password, string Role)>
                {
                    (new ApplicationUser
                    {
                        Email = "headdoctor@majesticgolden.com",
                        UserName = "headdoctor",
                        FullName_En = "Dr. Omar Al-Rashid",
                        FullName_Ar = "د. عمر الرشيد",
                        PhoneNumber = "+962-79-1111111",
                        EmailConfirmed = true,
                        BranchId = branchId,
                        Specialization = "General Dentistry & Management",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }, "HeadDoctor@123", "HeadDoctor"),

                    (new ApplicationUser
                    {
                        Email = "subdoctor@majesticgolden.com",
                        UserName = "subdoctor",
                        FullName_En = "Dr. Sarah Al-Mahmoud",
                        FullName_Ar = "د. سارة المحمود",
                        PhoneNumber = "+962-79-2222222",
                        EmailConfirmed = true,
                        BranchId = branchId,
                        Specialization = "Cosmetic Dentistry",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }, "SubDoctor@123", "SubDoctor"),

                    (new ApplicationUser
                    {
                        Email = "receptionist@majesticgolden.com",
                        UserName = "receptionist",
                        FullName_En = "Layla Ibrahim",
                        FullName_Ar = "ليلى إبراهيم",
                        PhoneNumber = "+962-79-3333333",
                        EmailConfirmed = true,
                        BranchId = branchId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }, "Receptionist@123", "Receptionist"),

                    (new ApplicationUser
                    {
                        Email = "patient@example.com",
                        UserName = "patient_demo",
                        FullName_En = "Ali Al-Zahrani",
                        FullName_Ar = "علي الزهراني",
                        PhoneNumber = "+962-79-4444444",
                        EmailConfirmed = true,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }, "Patient@123", "Patient")
                };

                foreach (var (user, password, role) in users)
                {
                    var result = await _userManager.CreateAsync(user, password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
            }

            await _context.SaveChangesAsync();
            }
        }
    }
}
