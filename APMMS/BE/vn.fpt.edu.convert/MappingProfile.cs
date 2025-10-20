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

namespace BE.vn.fpt.edu.convert
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth mappings removed - using database entities directly

            // AutoOwner mappings
            CreateMap<User, BE.vn.fpt.edu.DTOs.AutoOwner.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.AutoOwner.RequestDto, User>();

            // Component mappings
            CreateMap<Component, BE.vn.fpt.edu.DTOs.Component.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.Component.RequestDto, Component>();

            // Employee mappings
            CreateMap<User, BE.vn.fpt.edu.DTOs.Employee.EmployeeResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.Employee.EmployeeRequestDto, User>();

            // Feedback mappings
            CreateMap<Feedback, BE.vn.fpt.edu.DTOs.Feedback.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.Feedback.RequestDto, Feedback>();

            // HistoryLog mappings
            CreateMap<HistoryLog, BE.vn.fpt.edu.DTOs.HistoryLog.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.HistoryLog.RequestDto, HistoryLog>();

            // MaintenanceTicket mappings
            CreateMap<MaintenanceTicket, BE.vn.fpt.edu.DTOs.MaintenanceTicket.ResponseDto>();
            CreateMap<MaintenanceTicket, BE.vn.fpt.edu.DTOs.MaintenanceTicket.ListResponseDto>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) // Ignore CreatedDate since it doesn't exist in model
                .ForMember(dest => dest.CarName, opt => opt.MapFrom(src => src.Car != null ? src.Car.CarName : null))
                .ForMember(dest => dest.ConsulterName, opt => opt.MapFrom(src => src.Consulter != null ? $"{src.Consulter.FirstName} {src.Consulter.LastName}".Trim() : null))
                .ForMember(dest => dest.TechnicianName, opt => opt.MapFrom(src => src.Technician != null ? $"{src.Technician.FirstName} {src.Technician.LastName}".Trim() : null))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null));
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
            CreateMap<ServicePackage, BE.vn.fpt.edu.DTOs.ServicePackage.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.ServicePackage.RequestDto, ServicePackage>();

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
            CreateMap<TypeComponent, BE.vn.fpt.edu.DTOs.TypeComponent.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.TypeComponent.RequestDto, TypeComponent>();

            // VehicleCheckin mappings
            CreateMap<VehicleCheckin, BE.vn.fpt.edu.DTOs.VehicleCheckin.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.VehicleCheckin.VehicleCheckinRequestDto, VehicleCheckin>();

            // CarOfOutoOwner mappings
            CreateMap<Car, BE.vn.fpt.edu.DTOs.CarOfAutoOwner.ResponseDto>();
            CreateMap<BE.vn.fpt.edu.DTOs.CarOfAutoOwner.RequestDto, Car>();

        }
    }
}