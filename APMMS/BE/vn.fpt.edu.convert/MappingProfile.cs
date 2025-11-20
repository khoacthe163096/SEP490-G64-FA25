using AutoMapper;
using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.DTOs.Auth;
using BE.vn.fpt.edu.DTOs.AutoOwner;
using BE.vn.fpt.edu.DTOs.Component;
using BE.vn.fpt.edu.DTOs.Employee;
using BE.vn.fpt.edu.DTOs.Feedback;
using BE.vn.fpt.edu.DTOs.HistoryLog;
using BE.vn.fpt.edu.DTOs.MaintenanceTicket;
using BE.vn.fpt.edu.DTOs.Profile;
using BE.vn.fpt.edu.DTOs.Report;
using BE.vn.fpt.edu.DTOs.Role;
using BE.vn.fpt.edu.DTOs.ServicePackage;
using BE.vn.fpt.edu.DTOs.ServiceSchedule;
using BE.vn.fpt.edu.DTOs.ServiceTask;
using BE.vn.fpt.edu.DTOs.TotalReceipt;
using BE.vn.fpt.edu.DTOs.TypeComponent;
using BE.vn.fpt.edu.DTOs.VehicleCheckin;
using BE.vn.fpt.edu.DTOs.Branch;
using System.Linq;

namespace BE.vn.fpt.edu.convert
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth mappings removed - using database entities directly

            // AutoOwner mappings
            CreateMap<User, BE.vn.fpt.edu.DTOs.AutoOwner.ResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null))
                
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone));

            CreateMap<BE.vn.fpt.edu.DTOs.AutoOwner.RequestDto, User>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Password, opt => opt.Ignore()); // Password được xử lý riêng trong service để tránh bị mất

            // Component mappings
            CreateMap<BE.vn.fpt.edu.DTOs.Component.RequestDto, Component>()
                .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id.HasValue))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 0L));

            CreateMap<Component, BE.vn.fpt.edu.DTOs.Component.ResponseDto>()
                .ForMember(dest => dest.TypeComponentName, opt => opt.Ignore())
                .ForMember(dest => dest.BranchName, opt => opt.Ignore());


            // Employee mappings
            CreateMap<User, BE.vn.fpt.edu.DTOs.Employee.EmployeeResponseDto>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.Name : null))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.BranchId))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null))
                .ForMember(dest => dest.FullAddress, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => src.Dob.HasValue ? new DateTime(src.Dob.Value.Year, src.Dob.Value.Month, src.Dob.Value.Day) : (DateTime?)null));
            CreateMap<BE.vn.fpt.edu.DTOs.Employee.EmployeeRequestDto, User>()
                .ForMember(dest => dest.Dob, opt => opt.MapFrom(src => ParseDobString(src.Dob)));

            // Feedback mappings

            CreateMap<Feedback, BE.vn.fpt.edu.DTOs.Feedback.ResponseDto>()
     .ForMember(dest => dest.UserName,
        opt => opt.MapFrom(src =>
            src.User != null
                ? $"{src.User.FirstName} {src.User.LastName}".Trim()
                : null))
    .ForMember(dest => dest.Image,
        opt => opt.MapFrom(src => src.User != null ? src.User.Image : null));

            CreateMap<BE.vn.fpt.edu.DTOs.Feedback.RequestDto, Feedback>();

            // HistoryLog mappings
            CreateMap<HistoryLog, BE.vn.fpt.edu.DTOs.HistoryLog.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.HistoryLog.RequestDto, HistoryLog>();

            // MaintenanceTicket mappings
            CreateMap<MaintenanceTicket, BE.vn.fpt.edu.DTOs.MaintenanceTicket.ResponseDto>()
                // Basic fields
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ServiceCategoryId, opt => opt.MapFrom(src => src.ServiceCategoryId))
                .ForMember(dest => dest.ServiceCategoryName, opt => opt.MapFrom(src => src.ServiceCategory != null ? src.ServiceCategory.Name : null))
                .ForMember(dest => dest.CarName, opt => opt.MapFrom(src => src.SnapshotCarName ?? (src.Car != null ? src.Car.CarName : null)))
                .ForMember(dest => dest.ConsulterName, opt => opt.MapFrom(src => src.SnapshotConsulterName ?? (src.Consulter != null ? ($"{src.Consulter.FirstName} {src.Consulter.LastName}").Trim() : null)))
                .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.Technician != null ? ($"{src.Technician.FirstName} {src.Technician.LastName}").Trim() : null))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.SnapshotBranchName ?? (src.Branch != null ? src.Branch.Name : null)))
                .ForMember(dest => dest.ScheduleServiceName, opt => opt.MapFrom(src => src.ScheduleService != null ? src.ScheduleService.ScheduledDate.ToString("dd/MM/yyyy") : null))
                // Customer info from car owner
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.SnapshotCustomerName ?? (src.Car != null && src.Car.User != null ? ($"{src.Car.User.FirstName} {src.Car.User.LastName}").Trim() : null)))
                .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.SnapshotCustomerPhone ?? (src.Car != null && src.Car.User != null ? src.Car.User.Phone : null)))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.SnapshotCustomerEmail ?? (src.Car != null && src.Car.User != null ? src.Car.User.Email : null)))
                .ForMember(dest => dest.CustomerAddress, opt => opt.MapFrom(src => src.SnapshotCustomerAddress ?? (src.Car != null && src.Car.User != null && !string.IsNullOrWhiteSpace(src.Car.User.Address) ? src.Car.User.Address : null)))
                // Vehicle info
                .ForMember(dest => dest.LicensePlate, opt => opt.MapFrom(src => src.SnapshotLicensePlate ?? (src.Car != null ? src.Car.LicensePlate : null)))
                .ForMember(dest => dest.CarModel, opt => opt.MapFrom(src => src.SnapshotCarModel ?? (src.Car != null ? src.Car.CarModel : null)))
                .ForMember(dest => dest.VinNumber, opt => opt.MapFrom(src => src.SnapshotVinNumber ?? (src.Car != null ? src.Car.VinNumber : null)))
                .ForMember(dest => dest.VehicleEngineNumber, opt => opt.MapFrom(src => src.SnapshotEngineNumber ?? (src.Car != null ? src.Car.VehicleEngineNumber : null)))
                .ForMember(dest => dest.YearOfManufacture, opt => opt.MapFrom(src => src.SnapshotYearOfManufacture ?? (src.Car != null ? src.Car.YearOfManufacture : null)))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.SnapshotVehicleType ?? (src.Car != null && src.Car.VehicleType != null ? src.Car.VehicleType.Name : null)))
                .ForMember(dest => dest.VehicleTypeId, opt => opt.MapFrom(src => src.SnapshotVehicleTypeId ?? (src.Car != null ? src.Car.VehicleTypeId : null)))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.SnapshotColor ?? (src.Car != null ? src.Car.Color : null)))
                // Vehicle checkin info
                .ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.SnapshotMileage ?? (src.VehicleCheckin != null ? src.VehicleCheckin.Mileage : null)))
                .ForMember(dest => dest.CheckinNotes, opt => opt.MapFrom(src => src.VehicleCheckin != null ? src.VehicleCheckin.Notes : null))
                .ForMember(dest => dest.CheckinImages, opt => opt.MapFrom(src => src.VehicleCheckin != null && src.VehicleCheckin.VehicleCheckinImages != null ? src.VehicleCheckin.VehicleCheckinImages.Select(i => i.ImageUrl).ToList() : new List<string>()))
                // Service Package info
                .ForMember(dest => dest.ServicePackageId, opt => opt.MapFrom(src => src.ServicePackageId))
                .ForMember(dest => dest.ServicePackageName, opt => opt.MapFrom(src => src.ServicePackage != null ? src.ServicePackage.Name : null))
                .ForMember(dest => dest.ServicePackagePrice, opt => opt.MapFrom(src => src.ServicePackagePrice));
            CreateMap<MaintenanceTicket, BE.vn.fpt.edu.DTOs.MaintenanceTicket.ListResponseDto>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.CarName, opt => opt.MapFrom(src => src.SnapshotCarName ?? (src.Car != null ? src.Car.CarName : null)))
                .ForMember(dest => dest.ConsulterName, opt => opt.MapFrom(src => src.SnapshotConsulterName ?? (src.Consulter != null ? $"{src.Consulter.FirstName} {src.Consulter.LastName}".Trim() : null)))
                .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.Technician != null ? $"{src.Technician.FirstName} {src.Technician.LastName}".Trim() : null))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.SnapshotBranchName ?? (src.Branch != null ? src.Branch.Name : null)))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.SnapshotCustomerName ?? (src.Car != null && src.Car.User != null ? $"{src.Car.User.FirstName} {src.Car.User.LastName}".Trim() : null)))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code));
            CreateMap<BE.vn.fpt.edu.DTOs.MaintenanceTicket.RequestDto, MaintenanceTicket>();

            // Profile mappings
            CreateMap<User, BE.vn.fpt.edu.DTOs.Profile.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.Profile.RequestDto, User>();

            // Report mappings
            CreateMap<TotalReceipt, BE.vn.fpt.edu.DTOs.Report.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.Report.RequestDto, TotalReceipt>();

            // Role mappings
            CreateMap<Role, BE.vn.fpt.edu.DTOs.Role.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.Role.RequestDto, Role>();

            // ServicePackage mappings
            CreateMap<BE.vn.fpt.edu.DTOs.ServicePackage.RequestDto, ServicePackage>()
             .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id.HasValue))
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 0L))
             .ForMember(dest => dest.Components, opt => opt.Ignore()); // handle manually in service

            CreateMap<ServicePackage, BE.vn.fpt.edu.DTOs.ServicePackage.ResponseDto>()
                .ForMember(dest => dest.Components, opt => opt.Ignore()); // fill manually

            // ServiceSchedule mappings
            CreateMap<ScheduleService, BE.vn.fpt.edu.DTOs.ServiceSchedule.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.ServiceSchedule.RequestDto, ScheduleService>();

            // ServiceTask mappings
            CreateMap<ServiceTask, BE.vn.fpt.edu.DTOs.ServiceTask.ServiceTaskResponseDto>();
            CreateMap<ServiceTask, BE.vn.fpt.edu.DTOs.ServiceTask.ServiceTaskListResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.ServiceTask.ServiceTaskRequestDto, ServiceTask>();
            CreateMap<BE.vn.fpt.edu.DTOs.ServiceTask.ServiceTaskUpdateDto, ServiceTask>();

            // TotalReceipt mappings
            CreateMap<TotalReceipt, BE.vn.fpt.edu.DTOs.TotalReceipt.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.TotalReceipt.RequestDto, TotalReceipt>();
            
            // TypeComponent mappings
            CreateMap<BE.vn.fpt.edu.DTOs.TypeComponent.RequestDto, TypeComponent>()
            .ForMember(dest => dest.Id, opt => opt.Condition(src => src.Id.HasValue))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id ?? 0L));

            CreateMap<TypeComponent, BE.vn.fpt.edu.DTOs.TypeComponent.ResponseDto>();

            // VehicleCheckin mappings
            CreateMap<VehicleCheckin, BE.vn.fpt.edu.DTOs.VehicleCheckin.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.VehicleCheckin.VehicleCheckinRequestDto, VehicleCheckin>();

            // CarOfOutoOwner mappings
            CreateMap<Car, BE.vn.fpt.edu.DTOs.CarOfAutoOwner.ResponseDto>()
                .ForMember(dest => dest.VehicleTypeName, opt => opt.MapFrom(src => src.VehicleType != null ? src.VehicleType.Name : null));
            CreateMap<BE.vn.fpt.edu.DTOs.CarOfAutoOwner.RequestDto, Car>();

            // Branch mappings
            CreateMap<Branch, BranchResponseDto>();
            CreateMap<BranchRequestDto, Branch>();

        }

        private static DateOnly? ParseDobString(string? dobString)
        {
            if (string.IsNullOrWhiteSpace(dobString))
                return null;
            
            // Parse dd-MM-yyyy format
            if (DateOnly.TryParseExact(dobString, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out DateOnly date))
                return date;
            
            // Fallback: try standard formats
            if (DateTime.TryParse(dobString, out DateTime dt))
                return DateOnly.FromDateTime(dt);
            
            return null;
        }
    }
}