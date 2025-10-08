using AutoMapper;
using DAL.vn.fpt.edu.models;
using DAL.vn.fpt.edu.entities;
using BLL.vn.fpt.edu.DTOs.Auth;
using BLL.vn.fpt.edu.DTOs.AutoOwner;
using BLL.vn.fpt.edu.DTOs.Component;
using BLL.vn.fpt.edu.DTOs.Employee;
using BLL.vn.fpt.edu.DTOs.Feedback;
using BLL.vn.fpt.edu.DTOs.HistoryLog;
using BLL.vn.fpt.edu.DTOs.MaintenanceTicket;
using BLL.vn.fpt.edu.DTOs.Profile;
using BLL.vn.fpt.edu.DTOs.Report;
using BLL.vn.fpt.edu.DTOs.Role;
using BLL.vn.fpt.edu.DTOs.ServicePackage;
using BLL.vn.fpt.edu.DTOs.ServiceSchedule;
using BLL.vn.fpt.edu.DTOs.ServiceTask;
using BLL.vn.fpt.edu.DTOs.TotalReceipt;
using BLL.vn.fpt.edu.DTOs.TypeComponent;
using BLL.vn.fpt.edu.DTOs.VehicleCheckin;

namespace BLL.vn.fpt.edu.convert
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Auth mappings
            CreateMap<ApplicationUser, LoginResponseDto>();
            CreateMap<RegisterRequestDto, ApplicationUser>();
            CreateMap<ApplicationUser, RegisterResponseDto>();

            // AutoOwner mappings
            CreateMap<User, BLL.vn.fpt.edu.DTOs.AutoOwner.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.AutoOwner.RequestDto, User>();

            // Component mappings
            CreateMap<Component, BLL.vn.fpt.edu.DTOs.Component.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.Component.RequestDto, Component>();

            // Employee mappings
            CreateMap<User, BLL.vn.fpt.edu.DTOs.Employee.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.Employee.RequestDto, User>();

            // Feedback mappings
            CreateMap<Feedback, BLL.vn.fpt.edu.DTOs.Feedback.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.Feedback.CreateRequestDto, Feedback>();

            // HistoryLog mappings
            CreateMap<HistoryLog, BLL.vn.fpt.edu.DTOs.HistoryLog.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.HistoryLog.RequestDto, HistoryLog>();

            // MaintenanceTicket mappings
            CreateMap<MaintenanceTicket, BLL.vn.fpt.edu.DTOs.MaintenanceTicket.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.MaintenanceTicket.RequestDto, MaintenanceTicket>();

            // Profile mappings
            CreateMap<User, BLL.vn.fpt.edu.DTOs.Profile.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.Profile.RequestDto, User>();

            // Report mappings
            CreateMap<TotalReceipt, BLL.vn.fpt.edu.DTOs.Report.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.Report.RequestDto, TotalReceipt>();

            // Role mappings
            CreateMap<Role, BLL.vn.fpt.edu.DTOs.Role.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.Role.RequestDto, Role>();

            // ServicePackage mappings
            CreateMap<ServicePackage, BLL.vn.fpt.edu.DTOs.ServicePackage.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.ServicePackage.RequestDto, ServicePackage>();

            // ServiceSchedule mappings
            CreateMap<ScheduleService, BLL.vn.fpt.edu.DTOs.ServiceSchedule.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.ServiceSchedule.RequestDto, ScheduleService>();

            // ServiceTask mappings
            CreateMap<ServiceTask, BLL.vn.fpt.edu.DTOs.ServiceTask.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.ServiceTask.RequestDto, ServiceTask>();

            // TotalReceipt mappings
            CreateMap<TotalReceipt, BLL.vn.fpt.edu.DTOs.TotalReceipt.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.TotalReceipt.RequestDto, TotalReceipt>();

            // TypeComponent mappings
            CreateMap<TypeComponent, BLL.vn.fpt.edu.DTOs.TypeComponent.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.TypeComponent.RequestDto, TypeComponent>();

            // VehicleCheckin mappings
            CreateMap<VehicleCheckin, BLL.vn.fpt.edu.DTOs.VehicleCheckin.ResponseDto>();
            CreateMap<BLL.vn.fpt.edu.DTOs.VehicleCheckin.RequestDto, VehicleCheckin>();
        }
    }
}


