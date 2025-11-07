using System;
using System.Collections.Generic;

namespace BE.vn.fpt.edu.DTOs.ServiceSchedule
{
    public class ResponseDto
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public long? CarId { get; set; }
        public string? CarName { get; set; }
        public string? LicensePlate { get; set; }
        public string? CarModel { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusName { get; set; }
        public long? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? BranchPhone { get; set; }

        public long? AcceptedById { get; set; }

        public string? AcceptedByName { get; set; }

        public DateTime? AcceptedAt { get; set; }

        public string? AcceptNote { get; set; }

        public List<NoteResponseDto> Notes { get; set; } = new List<NoteResponseDto>();
    }

    public class ListResponseDto
    {
        public long Id { get; set; }
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public long? CarId { get; set; }
        public string? CarName { get; set; }
        public string? LicensePlate { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string? StatusCode { get; set; }
        public string? StatusName { get; set; }
        public long? BranchId { get; set; }
        public string? BranchName { get; set; }

        public long? AcceptedById { get; set; }

        public string? AcceptedByName { get; set; }

        public DateTime? AcceptedAt { get; set; }

        public string? AcceptNote { get; set; }

        public List<NoteResponseDto> Notes { get; set; } = new List<NoteResponseDto>();
    }
}
