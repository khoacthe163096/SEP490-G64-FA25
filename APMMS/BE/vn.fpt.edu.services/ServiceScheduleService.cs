using System;
using System.Linq;
using AutoMapper;
using BE.vn.fpt.edu.DTOs.ServiceSchedule;
using BE.vn.fpt.edu.interfaces;
using BE.vn.fpt.edu.repository.IRepository;
using BE.vn.fpt.edu.models;
using Microsoft.EntityFrameworkCore;

namespace BE.vn.fpt.edu.services
{
    public class ServiceScheduleService : IServiceScheduleService
    {
        private const string AssignmentNotePrefix = "[ASSIGNMENT]";

        private readonly IServiceScheduleRepository _repository;
        private readonly ICarOfAutoOwnerRepository _carRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAutoOwnerRepository _autoOwnerRepository;
        private readonly CarMaintenanceDbContext _context;
        private readonly IMapper _mapper;

        public ServiceScheduleService(
            IServiceScheduleRepository repository,
            ICarOfAutoOwnerRepository carRepository,
            IUserRepository userRepository,
            IAutoOwnerRepository autoOwnerRepository,
            CarMaintenanceDbContext context,
            IMapper mapper)
        {
            _repository = repository;
            _carRepository = carRepository;
            _userRepository = userRepository;
            _autoOwnerRepository = autoOwnerRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<ResponseDto> CreateScheduleAsync(RequestDto request)
        {
            // Validate Car belongs to User
            var car = await _carRepository.GetByIdAsync(request.CarId);
            if (car == null)
                throw new ArgumentException("Car not found");

            if (car.UserId != request.UserId)
                throw new ArgumentException("Car does not belong to this user");

            // Validate scheduled date is in the future
            if (request.ScheduledDate <= DateTime.UtcNow)
                throw new ArgumentException("Scheduled date must be in the future");

            // Check if user already has a schedule at the same time
            var existingSchedules = await _repository.GetByUserIdAsync(request.UserId);
            var conflictingSchedule = existingSchedules.FirstOrDefault(s => 
                s.ScheduledDate.Date == request.ScheduledDate.Date &&
                s.StatusCode != "CANCELLED" &&
                s.StatusCode != "COMPLETED");

            if (conflictingSchedule != null)
                throw new ArgumentException("You already have a schedule on this date");

            var scheduleService = _mapper.Map<ScheduleService>(request);
            var createdSchedule = await _repository.CreateAsync(scheduleService);

            return MapToResponseDto(createdSchedule);
        }

        public async Task<ResponseDto> GetScheduleByIdAsync(long id)
        {
            var schedule = await _repository.GetByIdAsync(id);
            if (schedule == null)
                throw new ArgumentException("Schedule not found");

            return MapToResponseDto(schedule);
        }

        public async Task<List<ListResponseDto>> GetAllSchedulesAsync(int page = 1, int pageSize = 10)
        {
            var schedules = await _repository.GetAllAsync(page, pageSize);
            return schedules.Select(MapToListResponseDto).ToList();
        }

        public async Task<List<ListResponseDto>> GetSchedulesByUserIdAsync(long userId)
        {
            var schedules = await _repository.GetByUserIdAsync(userId);
            return schedules.Select(MapToListResponseDto).ToList();
        }

        public async Task<List<ListResponseDto>> GetSchedulesByBranchIdAsync(long branchId)
        {
            var schedules = await _repository.GetByBranchIdAsync(branchId);
            return schedules.Select(MapToListResponseDto).ToList();
        }

        public async Task<List<ListResponseDto>> GetSchedulesByStatusAsync(string statusCode)
        {
            var schedules = await _repository.GetByStatusAsync(statusCode);
            return schedules.Select(MapToListResponseDto).ToList();
        }

        public async Task<List<ListResponseDto>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var schedules = await _repository.GetByDateRangeAsync(startDate, endDate);
            return schedules.Select(MapToListResponseDto).ToList();
        }

        public async Task<ResponseDto> UpdateScheduleAsync(long id, UpdateScheduleDto request)
        {
            var schedule = await _repository.GetByIdAsync(id);
            if (schedule == null)
                throw new ArgumentException("Schedule not found");

            // Cannot update cancelled or completed schedules
            if (schedule.StatusCode == "CANCELLED" || schedule.StatusCode == "COMPLETED")
                throw new ArgumentException("Cannot update cancelled or completed schedule");

            // Update fields if provided
            if (request.ScheduledDate.HasValue)
            {
                if (request.ScheduledDate.Value <= DateTime.UtcNow)
                    throw new ArgumentException("Scheduled date must be in the future");
                schedule.ScheduledDate = request.ScheduledDate.Value;
            }

            if (request.BranchId.HasValue)
                schedule.BranchId = request.BranchId.Value;

            if (!string.IsNullOrWhiteSpace(request.StatusCode))
                schedule.StatusCode = request.StatusCode;

            var updatedSchedule = await _repository.UpdateAsync(schedule);
            return MapToResponseDto(updatedSchedule);
        }

        public async Task<ResponseDto> CancelScheduleAsync(long id, CancelScheduleDto? request = null)
        {
            var schedule = await _repository.GetByIdAsync(id);
            if (schedule == null)
                throw new ArgumentException("Schedule not found");

            // Cannot cancel already cancelled or completed schedules
            if (schedule.StatusCode == "CANCELLED")
                throw new ArgumentException("Schedule is already cancelled");

            if (schedule.StatusCode == "COMPLETED")
                throw new ArgumentException("Cannot cancel completed schedule");

            schedule.StatusCode = "CANCELLED";
            var updatedSchedule = await _repository.UpdateAsync(schedule);
            return MapToResponseDto(updatedSchedule);
        }

        public async Task<bool> DeleteScheduleAsync(long id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<ResponseDto> CreatePublicBookingAsync(PublicBookingDto request)
        {
            // Validate scheduled date is in the future
            if (request.ScheduledDate <= DateTime.UtcNow)
                throw new ArgumentException("Scheduled date must be in the future");

            // Find or create user
            User? user = null;
            
            // Try to find user by email first
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user = await _autoOwnerRepository.GetByEmailAsync(request.Email);
            }
            
            // If not found by email, try to find by phone
            if (user == null && !string.IsNullOrWhiteSpace(request.Phone))
            {
                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Phone == request.Phone && 
                                              (u.IsDelete == false || u.IsDelete == null) &&
                                              u.RoleId == 7); // Auto Owner role
            }

            // Create user if not exists
            if (user == null)
            {
                // Parse full name to first name and last name
                var nameParts = request.FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var firstName = nameParts.Length > 0 ? nameParts[0] : request.FullName;
                var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

                // Generate username from phone or email
                var username = !string.IsNullOrWhiteSpace(request.Phone) 
                    ? $"user_{request.Phone.Replace(" ", "").Replace("-", "")}"
                    : (!string.IsNullOrWhiteSpace(request.Email) 
                        ? request.Email.Split('@')[0] 
                        : $"user_{DateTime.UtcNow.Ticks}");

                // Ensure username is unique
                var originalUsername = username;
                var counter = 1;
                while (await _userRepository.UsernameExistsAsync(username))
                {
                    username = $"{originalUsername}{counter}";
                    counter++;
                }

                user = new User
                {
                    Username = username,
                    Password = "N/A", // No password for public booking users
                    Email = request.Email,
                    Phone = request.Phone,
                    FirstName = firstName,
                    LastName = lastName,
                    RoleId = 7, // Auto Owner role
                    StatusCode = "ACTIVE",
                    IsDelete = false,
                    CreatedDate = DateTime.UtcNow
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
            }

            // Find or create car
            Car? car = null;
            
            // Try to find car by license plate and user
            if (!string.IsNullOrWhiteSpace(request.LicensePlate))
            {
                car = await _context.Cars
                    .FirstOrDefaultAsync(c => c.LicensePlate == request.LicensePlate && 
                                              c.UserId == user.Id);
            }

            // Create car if not exists
            if (car == null)
            {
                car = new Car
                {
                    UserId = user.Id,
                    CarName = request.CarName,
                    LicensePlate = request.LicensePlate,
                    CarModel = request.CarModel,
                    CreatedDate = DateTime.UtcNow
                };

                await _carRepository.CreateAsync(car);
            }

            // Check if user already has a schedule at the same time
            var existingSchedules = await _repository.GetByUserIdAsync(user.Id);
            var conflictingSchedule = existingSchedules.FirstOrDefault(s => 
                s.ScheduledDate.Date == request.ScheduledDate.Date &&
                s.StatusCode != "CANCELLED" &&
                s.StatusCode != "COMPLETED");

            if (conflictingSchedule != null)
                throw new ArgumentException("Bạn đã có lịch hẹn vào ngày này rồi");

            // Create schedule
            var scheduleService = new ScheduleService
            {
                UserId = user.Id,
                CarId = car.Id,
                BranchId = request.BranchId,
                ScheduledDate = request.ScheduledDate,
                StatusCode = "PENDING"
            };

            var createdSchedule = await _repository.CreateAsync(scheduleService);
            return MapToResponseDto(createdSchedule);
        }

        public async Task<ResponseDto> AcceptScheduleAsync(long id, AcceptScheduleDto request)
        {
            var schedule = await _repository.GetByIdAsync(id);
            if (schedule == null)
                throw new ArgumentException("Schedule not found");

            if (schedule.StatusCode == "CANCELLED")
                throw new ArgumentException("Cannot accept a cancelled schedule");

            if (schedule.StatusCode == "COMPLETED")
                throw new ArgumentException("Cannot accept a completed schedule");

            var consultant = await _userRepository.GetByIdAsync(request.ConsultantId);
            if (consultant == null)
                throw new ArgumentException("Consultant not found");

            if (consultant.RoleId != 6)
                throw new ArgumentException("User is not authorized to accept schedules");

            await _context.Entry(schedule)
                .Collection(s => s.ScheduleServiceNotes)
                .Query()
                .Include(note => note.Consultant)
                .LoadAsync();

            var existingAssignment = GetLatestAssignmentNote(schedule);
            if (existingAssignment != null)
            {
                if (existingAssignment.ConsultantId == request.ConsultantId)
                {
                    return MapToResponseDto(schedule);
                }

                var assignedName = BuildUserDisplayName(existingAssignment.Consultant);
                throw new InvalidOperationException($"Schedule already accepted by {assignedName ?? "another consultant"}.");
            }

            var noteMessage = string.IsNullOrWhiteSpace(request.Note)
                ? BuildUserDisplayName(consultant) ?? "Consulter accepted the schedule"
                : request.Note.Trim();

            var assignmentNote = new ScheduleServiceNote
            {
                ScheduleServiceId = schedule.Id,
                ConsultantId = consultant.Id,
                Note = $"{AssignmentNotePrefix}{noteMessage}",
                CreatedAt = DateTime.UtcNow,
                Consultant = consultant
            };

            _context.ScheduleServiceNotes.Add(assignmentNote);
            await _context.SaveChangesAsync();

            schedule.ScheduleServiceNotes.Add(assignmentNote);

            return MapToResponseDto(schedule);
        }

        public async Task<NoteResponseDto> AddNoteAsync(long scheduleId, AddNoteDto request)
        {
            var schedule = await _repository.GetByIdAsync(scheduleId);
            if (schedule == null)
                throw new ArgumentException("Schedule not found");

            var consultant = await _userRepository.GetByIdAsync(request.ConsultantId);
            if (consultant == null)
                throw new ArgumentException("Consultant not found");

            if (string.IsNullOrWhiteSpace(request.Note))
                throw new ArgumentException("Note cannot be empty");

            var note = new ScheduleServiceNote
            {
                ScheduleServiceId = scheduleId,
                ConsultantId = consultant.Id,
                Note = request.Note.Trim(),
                CreatedAt = DateTime.UtcNow,
                Consultant = consultant
            };

            _context.ScheduleServiceNotes.Add(note);
            await _context.SaveChangesAsync();

            return new NoteResponseDto
            {
                Id = note.Id,
                ScheduleServiceId = note.ScheduleServiceId,
                ConsultantId = note.ConsultantId,
                ConsultantName = BuildUserDisplayName(consultant) ?? consultant.Username ?? "Unknown",
                Note = note.Note,
                CreatedAt = note.CreatedAt,
                IsAssignmentNote = note.Note.StartsWith(AssignmentNotePrefix, StringComparison.OrdinalIgnoreCase)
            };
        }

        public async Task<List<NoteResponseDto>> GetNotesAsync(long scheduleId)
        {
            var schedule = await _repository.GetByIdAsync(scheduleId);
            if (schedule == null)
                throw new ArgumentException("Schedule not found");

            await _context.Entry(schedule)
                .Collection(s => s.ScheduleServiceNotes)
                .Query()
                .Include(note => note.Consultant)
                .OrderByDescending(note => note.CreatedAt)
                .LoadAsync();

            return schedule.ScheduleServiceNotes.Select(note => new NoteResponseDto
            {
                Id = note.Id,
                ScheduleServiceId = note.ScheduleServiceId,
                ConsultantId = note.ConsultantId,
                ConsultantName = BuildUserDisplayName(note.Consultant) ?? note.Consultant?.Username ?? "Unknown",
                Note = note.Note,
                CreatedAt = note.CreatedAt,
                IsAssignmentNote = note.Note.StartsWith(AssignmentNotePrefix, StringComparison.OrdinalIgnoreCase)
            }).ToList();
        }

        private ResponseDto MapToResponseDto(ScheduleService schedule)
        {
            var assignment = GetLatestAssignmentNote(schedule);

            // Map notes if already loaded
            var notes = new List<NoteResponseDto>();
            if (schedule.ScheduleServiceNotes != null && schedule.ScheduleServiceNotes.Any())
            {
                notes = schedule.ScheduleServiceNotes
                    .OrderByDescending(note => note.CreatedAt)
                    .Select(note => new NoteResponseDto
                    {
                        Id = note.Id,
                        ScheduleServiceId = note.ScheduleServiceId,
                        ConsultantId = note.ConsultantId,
                        ConsultantName = BuildUserDisplayName(note.Consultant) ?? note.Consultant?.Username ?? "Unknown",
                        Note = note.Note,
                        CreatedAt = note.CreatedAt,
                        IsAssignmentNote = note.Note.StartsWith(AssignmentNotePrefix, StringComparison.OrdinalIgnoreCase)
                    })
                    .ToList();
            }

            return new ResponseDto
            {
                Id = schedule.Id,
                UserId = schedule.UserId,
                UserName = schedule.User != null ? ($"{schedule.User.FirstName} {schedule.User.LastName}").Trim() : null,
                UserEmail = schedule.User?.Email,
                UserPhone = schedule.User?.Phone,
                CarId = schedule.CarId,
                CarName = schedule.Car?.CarName,
                LicensePlate = schedule.Car?.LicensePlate,
                CarModel = schedule.Car?.CarModel,
                ScheduledDate = schedule.ScheduledDate,
                StatusCode = schedule.StatusCode,
                StatusName = schedule.StatusCodeNavigation?.Name,
                BranchId = schedule.BranchId,
                BranchName = schedule.Branch?.Name,
                BranchPhone = schedule.Branch?.Phone,
                AcceptedById = assignment?.ConsultantId,
                AcceptedByName = BuildUserDisplayName(assignment?.Consultant),
                AcceptedAt = assignment?.CreatedAt,
                AcceptNote = ExtractAssignmentMessage(assignment),
                Notes = notes
            };
        }

        private ListResponseDto MapToListResponseDto(ScheduleService schedule)
        {
            var assignment = GetLatestAssignmentNote(schedule);

            return new ListResponseDto
            {
                Id = schedule.Id,
                UserId = schedule.UserId,
                UserName = schedule.User != null ? ($"{schedule.User.FirstName} {schedule.User.LastName}").Trim() : null,
                CarId = schedule.CarId,
                CarName = schedule.Car?.CarName,
                LicensePlate = schedule.Car?.LicensePlate,
                ScheduledDate = schedule.ScheduledDate,
                StatusCode = schedule.StatusCode,
                StatusName = schedule.StatusCodeNavigation?.Name,
                BranchId = schedule.BranchId,
                BranchName = schedule.Branch?.Name,
                AcceptedById = assignment?.ConsultantId,
                AcceptedByName = BuildUserDisplayName(assignment?.Consultant),
                AcceptedAt = assignment?.CreatedAt,
                AcceptNote = ExtractAssignmentMessage(assignment)
            };
        }

        private ScheduleServiceNote? GetLatestAssignmentNote(ScheduleService schedule)
        {
            if (schedule.ScheduleServiceNotes == null || schedule.ScheduleServiceNotes.Count == 0)
                return null;

            return schedule.ScheduleServiceNotes
                .Where(note => !string.IsNullOrWhiteSpace(note.Note) &&
                               note.Note.StartsWith(AssignmentNotePrefix, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(note => note.CreatedAt)
                .FirstOrDefault();
        }

        private static string? BuildUserDisplayName(User? user)
        {
            if (user == null)
                return null;

            var fullName = $"{user.FirstName} {user.LastName}".Trim();
            if (!string.IsNullOrWhiteSpace(fullName))
                return fullName;

            return !string.IsNullOrWhiteSpace(user.Username) ? user.Username : null;
        }

        private string? ExtractAssignmentMessage(ScheduleServiceNote? note)
        {
            if (note == null || string.IsNullOrWhiteSpace(note.Note))
                return null;

            if (!note.Note.StartsWith(AssignmentNotePrefix, StringComparison.OrdinalIgnoreCase))
                return note.Note;

            return note.Note.Substring(AssignmentNotePrefix.Length).Trim();
        }
    }
}
