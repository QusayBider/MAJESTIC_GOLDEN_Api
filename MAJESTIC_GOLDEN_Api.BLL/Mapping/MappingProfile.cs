using AutoMapper;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Requests;
using MAJESTIC_GOLDEN_Api.DAL.DTO.Responses;
using MAJESTIC_GOLDEN_Api.DAL.Models;

namespace MAJESTIC_GOLDEN_Api.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Branch, BranchResponseDTO>();
            CreateMap<BranchRequestDTO, Branch>();
            CreateMap<UpdateBranchRequestDTO, Branch>();

            CreateMap<Patient, PatientResponseDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UserId)) // Map UserId to Id
                .ForMember(dest => dest.FullName_En, opt => opt.MapFrom(src => src.User != null ? src.User.FullName_En : ""))
                .ForMember(dest => dest.FullName_Ar, opt => opt.MapFrom(src => src.User != null ? src.User.FullName_Ar : ""))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User != null ? src.User.Gender.ToString() : ""))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.User != null && src.User.DateOfBirth.HasValue ? src.User.DateOfBirth.Value : DateTime.MinValue))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.User != null && src.User.Age.HasValue ? src.User.Age.Value : 0))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber ?? "" : ""))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.Address_En, opt => opt.MapFrom(src => src.User != null ? src.User.Address_En : null))
                .ForMember(dest => dest.Address_Ar, opt => opt.MapFrom(src => src.User != null ? src.User.Address_Ar : null))
                .ForMember(dest => dest.Occupation_En, opt => opt.MapFrom(src => src.User != null ? src.User.Occupation_En : null))
                .ForMember(dest => dest.Occupation_Ar, opt => opt.MapFrom(src => src.User != null ? src.User.Occupation_Ar : null))
                .ForMember(dest => dest.MaritalStatus_En, opt => opt.MapFrom(src => src.User != null && src.User.MaritalStatus.HasValue ? src.User.MaritalStatus.Value.ToString() : null))
                .ForMember(dest => dest.MaritalStatus_Ar, opt => opt.MapFrom(src => src.User != null && src.User.MaritalStatus.HasValue ? src.User.MaritalStatus.Value.ToString() : null))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.User != null && src.User.BranchId.HasValue ? src.User.BranchId.Value : 0))
                .ForMember(dest => dest.BranchName_En, opt => opt.MapFrom(src => src.User != null && src.User.Branch != null ? src.User.Branch.Name_En : ""))
                .ForMember(dest => dest.BranchName_Ar, opt => opt.MapFrom(src => src.User != null && src.User.Branch != null ? src.User.Branch.Name_Ar : ""))
                .ForMember(dest => dest.HasUserAccount, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.UserId)))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.User != null ? src.User.CreatedAt : DateTime.MinValue));
            
            CreateMap<Patient, PatientDetailedResponseDTO>()
                .IncludeBase<Patient, PatientResponseDTO>();
            
            CreateMap<PatientRequestDTO, Patient>();
            CreateMap<PatientPortalRegisterDTO, Patient>();

            CreateMap<Appointment, AppointmentResponseDTO>()
                .ForMember(dest => dest.FullName_En, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? src.Patient.User.FullName_En : ""))
                .ForMember(dest => dest.FullName_Ar, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? src.Patient.User.FullName_Ar : ""))
                .ForMember(dest => dest.PatientPhone, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? src.Patient.User.PhoneNumber : ""))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName_En : ""))
                .ForMember(dest => dest.BranchName_En, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name_En : ""))
                .ForMember(dest => dest.BranchName_Ar, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name_Ar : ""));
            
            CreateMap<AppointmentRequestDTO, Appointment>();

            CreateMap<Service, ServiceResponseDTO>();
            CreateMap<ServiceRequestDTO, Service>();

            CreateMap<Invoice, InvoiceResponseDTO>()
                .ForMember(dest => dest.PatientName_En, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? src.Patient.User.FullName_En : ""))
                .ForMember(dest => dest.PatientName_Ar, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? src.Patient.User.FullName_Ar : ""))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName_En : ""));
            
            CreateMap<InvoiceRequestDTO, Invoice>()
                .ForMember(dest => dest.PatientUserId, opt => opt.MapFrom(src => src.PatientId));
            
            CreateMap<InvoiceItem, InvoiceItemResponseDTO>()
                .ForMember(dest => dest.ServiceName_En, opt => opt.MapFrom(src => src.Service != null ? src.Service.Name_En : ""))
                .ForMember(dest => dest.ServiceName_Ar, opt => opt.MapFrom(src => src.Service != null ? src.Service.Name_Ar : ""));
            
            CreateMap<InvoiceItemRequestDTO, InvoiceItem>();

            CreateMap<Payment, PaymentResponseDTO>();
            CreateMap<PaymentRequestDTO, Payment>();

            CreateMap<LabRequest, LabRequestResponseDTO>()
                .ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientUserId))
                .ForMember(dest => dest.PatientName_En, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? src.Patient.User.FullName_En : ""))
                .ForMember(dest => dest.PatientName_Ar, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? src.Patient.User.FullName_Ar : ""))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName_En : ""))
                .ForMember(dest => dest.LabName, opt => opt.MapFrom(src => src.Laboratory != null && src.Laboratory.User != null ? src.Laboratory.User.FullName_En : ""));
            
            CreateMap<LabRequestCreateDTO, LabRequest>()
                .ForMember(dest => dest.PatientUserId, opt => opt.MapFrom(src => src.PatientId));
            CreateMap<LabRequestUpdateDTO, LabRequest>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Laboratory, LaboratoryResponseDTO>()
                .ForMember(dest => dest.UserFullNameEn, opt => opt.MapFrom(src => src.User != null ? src.User.FullName_En : null))
                .ForMember(dest => dest.UserFullNameAr, opt => opt.MapFrom(src => src.User != null ? src.User.FullName_Ar : null))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.User != null ? src.User.Email : null))
                .ForMember(dest => dest.UserPhoneNumber, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : null))
                .ForMember(dest => dest.UserAddressEn, opt => opt.MapFrom(src => src.User != null ? src.User.Address_En : null))
                .ForMember(dest => dest.UserAddressAr, opt => opt.MapFrom(src => src.User != null ? src.User.Address_Ar : null))
                .ForMember(dest => dest.TotalRequests, opt => opt.MapFrom(src => src.Requests != null ? src.Requests.Count : 0));

            CreateMap<LaboratoryCreateDTO, Laboratory>();

            CreateMap<LaboratoryUpdateDTO, Laboratory>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CaseTransfer, CaseTransferResponseDTO>()
                .ForMember(dest => dest.PatientName_En, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? src.Patient.User.FullName_En : ""))
                .ForMember(dest => dest.PatientName_Ar, opt => opt.MapFrom(src => src.Patient != null && src.Patient.User != null ? src.Patient.User.FullName_Ar : ""))
                .ForMember(dest => dest.FromDoctorName, opt => opt.MapFrom(src => src.FromDoctor != null ? src.FromDoctor.FullName_En : ""))
                .ForMember(dest => dest.ToDoctorName, opt => opt.MapFrom(src => src.ToDoctor != null ? src.ToDoctor.FullName_En : ""));
            
            CreateMap<CaseTransferRequestDTO, CaseTransfer>()
                .ForMember(dest => dest.PatientUserId, opt => opt.MapFrom(src => src.PatientId));

            CreateMap<PatientTooth, PatientToothResponseDTO>()
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor != null ? src.Doctor.FullName_En : ""));
            
            CreateMap<PatientToothRequestDTO, PatientTooth>()
                .ForMember(dest => dest.PatientUserId, opt => opt.MapFrom(src => src.PatientId));
        }
    }
}



