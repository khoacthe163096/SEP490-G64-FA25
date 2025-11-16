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



        public async Task<ResponseDto> CreateScheduleAsync(RequestDto request, long? currentUserId = null)

        {

            // Validate Car belongs to User

            var car = await _carRepository.GetByIdAsync(request.CarId);

            if (car == null)

                throw new ArgumentException("Car not found");



            if (car.UserId != request.UserId)

                throw new ArgumentException("Car does not belong to this user");



            // Validate ServiceCategory if provided

            if (request.ServiceCategoryId.HasValue)

            {

                var serviceCategory = await _context.ServiceCategories

                    .FirstOrDefaultAsync(sc => sc.Id == request.ServiceCategoryId.Value);

                if (serviceCategory == null)

                    throw new ArgumentException("Service category not found");

            }



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

            // Set audit fields - khi tạo mới, chỉ set CreatedAt/CreatedBy
            var now = DateTime.UtcNow;
            scheduleService.CreatedAt = now;
            scheduleService.UpdatedAt = null; // Chưa có cập nhật nào
            scheduleService.CreatedBy = currentUserId;
            scheduleService.UpdatedBy = null; // Chưa có cập nhật nào

            var createdSchedule = await _repository.CreateAsync(scheduleService);

            return await MapToResponseDtoAsync(createdSchedule);

        }



        public async Task<ResponseDto> GetScheduleByIdAsync(long id)

        {

            var schedule = await _repository.GetByIdAsync(id);

            if (schedule == null)

                throw new ArgumentException("Schedule not found");



            // Load ServiceCategory if exists
            if (schedule.ServiceCategoryId.HasValue)
            {
                await _context.Entry(schedule)
                    .Reference(s => s.ServiceCategory)
                    .LoadAsync();
            }

            return await MapToResponseDtoAsync(schedule);

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



        public async Task<List<ListResponseDto>> GetSchedulesByStatusAsync(string statusCode, long? branchId = null)

        {

            var schedules = await _repository.GetByStatusAsync(statusCode, branchId);

            return schedules.Select(MapToListResponseDto).ToList();

        }



        public async Task<List<ListResponseDto>> GetSchedulesByDateRangeAsync(DateTime startDate, DateTime endDate, long? branchId = null)

        {

            var schedules = await _repository.GetByDateRangeAsync(startDate, endDate, branchId);

            return schedules.Select(MapToListResponseDto).ToList();

        }



        public async Task<ResponseDto> UpdateScheduleAsync(long id, UpdateScheduleDto request, long? currentUserId = null)

        {

            var schedule = await _repository.GetByIdAsync(id);

            if (schedule == null)

                throw new ArgumentException("Schedule not found");



            // Load notes to check if schedule has been accepted
            await _context.Entry(schedule)
                .Collection(s => s.ScheduleServiceNotes)
                .Query()
                .Include(note => note.Consultant)
                .LoadAsync();

            // Check if schedule has been accepted (has assignment note)
            var assignment = GetLatestAssignmentNote(schedule);
            if (assignment == null && schedule.StatusCode == "PENDING")
                throw new ArgumentException("Cannot update schedule that has not been accepted. Please accept the schedule first.");

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



            if (request.ServiceCategoryId.HasValue)
            {
                // Validate ServiceCategory if provided
                var serviceCategory = await _context.ServiceCategories
                    .FirstOrDefaultAsync(sc => sc.Id == request.ServiceCategoryId.Value);
                if (serviceCategory == null)
                    throw new ArgumentException("Service category not found");
                schedule.ServiceCategoryId = request.ServiceCategoryId.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.StatusCode))

                schedule.StatusCode = request.StatusCode;

            // Update audit fields
            schedule.UpdatedAt = DateTime.UtcNow;
            schedule.UpdatedBy = currentUserId;

            var updatedSchedule = await _repository.UpdateAsync(schedule);

            return await MapToResponseDtoAsync(updatedSchedule);

        }



        public async Task<ResponseDto> CancelScheduleAsync(long id, CancelScheduleDto? request = null, long? currentUserId = null)

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

            // Update audit fields
            schedule.UpdatedAt = DateTime.UtcNow;
            schedule.UpdatedBy = currentUserId;

            // Tạo ghi chú ghi lại việc hủy lịch
            if (currentUserId.HasValue)
            {
                var user = await _userRepository.GetByIdAsync(currentUserId.Value);
                if (user != null)
                {
                    var cancelReason = !string.IsNullOrWhiteSpace(request?.Reason)
                        ? request.Reason.Trim()
                        : null;

                    var cancelNote = new ScheduleServiceNote
                    {
                        ScheduleServiceId = schedule.Id,
                        ConsultantId = user.Id,
                        Note = $"[CANCELLED]{BuildUserDisplayName(user) ?? user.Username ?? "Người dùng"} đã hủy lịch hẹn{(cancelReason != null ? $": {cancelReason}" : "")}",
                        CreatedAt = DateTime.UtcNow,
                        Consultant = user
                    };

                    _context.ScheduleServiceNotes.Add(cancelNote);
                }
            }

            // Update schedule và save tất cả changes (bao gồm cả note) cùng lúc
            var updatedSchedule = await _repository.UpdateAsync(schedule);
            // Note đã được thêm vào context trước khi UpdateAsync, nên cần save lại để đảm bảo note được lưu
            // (UpdateAsync đã save schedule rồi, nhưng note mới thêm vào sau nên cần save lại)
            await _context.SaveChangesAsync();

            return await MapToResponseDtoAsync(updatedSchedule);

        }



        public async Task<ResponseDto> CompleteScheduleAsync(long id, long? currentUserId = null)

        {

            // Dùng cách GIỐNG HỆT CancelScheduleAsync vì nó đã hoạt động

            var schedule = await _repository.GetByIdAsync(id);

            if (schedule == null)

                throw new ArgumentException("Schedule not found");



            // Cannot complete already cancelled or completed schedules

            if (schedule.StatusCode == "CANCELLED")

                throw new ArgumentException("Cannot complete cancelled schedule");



            if (schedule.StatusCode == "COMPLETED")

                throw new ArgumentException("Schedule is already completed");



            schedule.StatusCode = "COMPLETED";

            // Update audit fields
            schedule.UpdatedAt = DateTime.UtcNow;
            schedule.UpdatedBy = currentUserId;

            var updatedSchedule = await _repository.UpdateAsync(schedule);

            return await MapToResponseDtoAsync(updatedSchedule);

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



            // Validate ServiceCategory if provided

            if (request.ServiceCategoryId.HasValue)

            {

                var serviceCategory = await _context.ServiceCategories

                    .FirstOrDefaultAsync(sc => sc.Id == request.ServiceCategoryId.Value);

                if (serviceCategory == null)

                    throw new ArgumentException("Service category not found");

            }



            // Tạo CustomerGuest để lưu thông tin khách hàng không có tài khoản

            var customerGuest = new CustomerGuest

            {

                Name = request.FullName.Trim(),

                Phone = request.Phone.Trim(),

                Email = request.Email?.Trim(),

                CarName = request.CarName?.Trim(),

                CarModel = request.CarModel?.Trim(),

                LicensePlate = request.LicensePlate?.Trim(),

                BranchId = request.BranchId,

                CreatedAt = DateTime.UtcNow

            };



            _context.CustomerGuests.Add(customerGuest);

            await _context.SaveChangesAsync();



            // Kiểm tra xem guest đã có lịch hẹn vào ngày này chưa

            var existingSchedules = await _context.ScheduleServices

                .Where(s => s.GuestId == customerGuest.Id &&

                           s.ScheduledDate.Date == request.ScheduledDate.Date &&

                           s.StatusCode != "CANCELLED" &&

                           s.StatusCode != "COMPLETED")

                .ToListAsync();



            if (existingSchedules.Any())

                throw new ArgumentException("Bạn đã có lịch hẹn vào ngày này rồi");



            // Create schedule với guest_id (không có user_id và car_id)

            var scheduleService = new ScheduleService

            {

                GuestId = customerGuest.Id,

                UserId = null, // Public booking không có user_id

                CarId = null,  // Public booking không có car_id

                BranchId = request.BranchId,

                ScheduledDate = request.ScheduledDate,

                StatusCode = "PENDING",

                ServiceCategoryId = request.ServiceCategoryId

            };

            // Set audit fields for public booking (no user, so CreatedBy/UpdatedBy are null)
            var now = DateTime.UtcNow;
            scheduleService.CreatedAt = now;
            scheduleService.UpdatedAt = null; // Chưa có cập nhật nào
            scheduleService.CreatedBy = null; // Public booking không có user
            scheduleService.UpdatedBy = null; // Chưa có cập nhật nào

            var createdSchedule = await _repository.CreateAsync(scheduleService);

            return await MapToResponseDtoAsync(createdSchedule);

        }



        public async Task<ResponseDto> AcceptScheduleAsync(long id, AcceptScheduleDto request, long? currentUserId = null)

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



            // Load User và Car để lấy thông tin nếu cần

            await _context.Entry(schedule)

                .Reference(s => s.User)

                .LoadAsync();



            await _context.Entry(schedule)

                .Reference(s => s.Car)

                .LoadAsync();



            // Nếu schedule có user_id nhưng chưa có guest_id, và user là public booking (Password = "N/A")

            // thì tạo CustomerGuest và chuyển sang guest_id

            if (schedule.UserId.HasValue && !schedule.GuestId.HasValue && schedule.User != null && schedule.User.Password == "N/A")

            {

                // Tạo CustomerGuest từ thông tin user và car

                var customerGuest = new CustomerGuest

                {

                    Name = $"{schedule.User.FirstName} {schedule.User.LastName}".Trim(),

                    Phone = schedule.User.Phone ?? string.Empty,

                    Email = schedule.User.Email,

                    CarName = schedule.Car?.CarName,

                    CarModel = schedule.Car?.CarModel,

                    LicensePlate = schedule.Car?.LicensePlate,

                    BranchId = schedule.BranchId ?? consultant.BranchId,

                    LinkedUserId = schedule.UserId, // Liên kết với user để sau này có thể chuyển đổi

                    CreatedAt = DateTime.UtcNow

                };



                _context.CustomerGuests.Add(customerGuest);

                await _context.SaveChangesAsync(); // Save để lấy ID



                // Chuyển schedule từ user_id sang guest_id

                schedule.GuestId = customerGuest.Id;

                schedule.UserId = null; // Set null vì CHECK constraint chỉ cho phép một trong hai

                schedule.CarId = null;  // Set null vì car thuộc về user, không thuộc về guest

            }



            var existingAssignment = GetLatestAssignmentNote(schedule);

            if (existingAssignment != null)

            {

                if (existingAssignment.ConsultantId == request.ConsultantId)

                {

                    return await MapToResponseDtoAsync(schedule);

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



            // Cập nhật status thành IN_PROGRESS khi nhận công việc (chỉ khi đang là PENDING)

            if (schedule.StatusCode == "PENDING")

            {

                schedule.StatusCode = "IN_PROGRESS";

            }

            // Update audit fields
            schedule.UpdatedAt = DateTime.UtcNow;
            schedule.UpdatedBy = currentUserId;

            await _context.SaveChangesAsync();



            schedule.ScheduleServiceNotes.Add(assignmentNote);

            return await MapToResponseDtoAsync(schedule);

        }



        public async Task<NoteResponseDto> AddNoteAsync(long scheduleId, AddNoteDto request, long? currentUserId = null)

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

            // Update schedule audit fields when note is added
            schedule.UpdatedAt = DateTime.UtcNow;
            schedule.UpdatedBy = currentUserId;



            _context.ScheduleServiceNotes.Add(note);

            // Update schedule in context
            _context.Entry(schedule).Property(s => s.UpdatedAt).IsModified = true;
            _context.Entry(schedule).Property(s => s.UpdatedBy).IsModified = true;

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



        private ResponseDto MapToResponseDto(ScheduleService schedule, User? createdByUser = null, User? updatedByUser = null)

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



            // Lấy branchId từ consultant nếu đã nhận đơn, nếu không thì lấy từ schedule

            var finalBranchId = assignment?.Consultant?.BranchId ?? schedule.BranchId;

            var finalBranchName = assignment?.Consultant?.Branch?.Name ?? schedule.Branch?.Name;



            // Xác định thông tin khách hàng: ưu tiên Guest, sau đó mới đến User

            string? userName = null;

            string? userEmail = null;

            string? userPhone = null;

            string? carName = null;

            string? licensePlate = null;

            string? carModel = null;

            bool isPublicBooking = false;



            if (schedule.GuestId.HasValue && schedule.Guest != null)

            {

                // Lấy thông tin từ Guest

                userName = schedule.Guest.Name;

                userEmail = schedule.Guest.Email;

                userPhone = schedule.Guest.Phone;

                carName = schedule.Guest.CarName;

                licensePlate = schedule.Guest.LicensePlate;

                carModel = schedule.Guest.CarModel;

                isPublicBooking = true;

            }

            else if (schedule.UserId.HasValue && schedule.User != null)

            {

                // Lấy thông tin từ User

                userName = ($"{schedule.User.FirstName} {schedule.User.LastName}").Trim();

                userEmail = schedule.User.Email;

                userPhone = schedule.User.Phone;

                carName = schedule.Car?.CarName;

                licensePlate = schedule.Car?.LicensePlate;

                carModel = schedule.Car?.CarModel;

                isPublicBooking = schedule.User.Password == "N/A";

            }



            return new ResponseDto

            {

                Id = schedule.Id,

                UserId = schedule.UserId,

                UserName = userName,

                UserEmail = userEmail,

                UserPhone = userPhone,

                CarId = schedule.CarId,

                CarName = carName,

                LicensePlate = licensePlate,

                CarModel = carModel,

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

                ConsultantBranchId = finalBranchId,

                ConsultantBranchName = finalBranchName,

                ServiceCategoryId = schedule.ServiceCategoryId,
                ServiceCategoryName = schedule.ServiceCategory?.Name,
                IsPublicBooking = isPublicBooking,

                Notes = notes,

                // Audit trail fields
                CreatedAt = schedule.CreatedAt,
                UpdatedAt = schedule.UpdatedAt,
                CreatedBy = schedule.CreatedBy,
                UpdatedBy = schedule.UpdatedBy,
                CreatedByName = createdByUser != null ? BuildUserDisplayName(createdByUser) : null,
                UpdatedByName = updatedByUser != null ? BuildUserDisplayName(updatedByUser) : null

            };

        }



        private ListResponseDto MapToListResponseDto(ScheduleService schedule)

        {

            var assignment = GetLatestAssignmentNote(schedule);



            // Xác định thông tin khách hàng: ưu tiên Guest, sau đó mới đến User

            string? userName = null;

            string? carName = null;

            string? licensePlate = null;

            bool isPublicBooking = false;



            if (schedule.GuestId.HasValue && schedule.Guest != null)

            {

                // Lấy thông tin từ Guest

                userName = schedule.Guest.Name;

                carName = schedule.Guest.CarName;

                licensePlate = schedule.Guest.LicensePlate;

                isPublicBooking = true;

            }

            else if (schedule.UserId.HasValue && schedule.User != null)

            {

                // Lấy thông tin từ User

                userName = ($"{schedule.User.FirstName} {schedule.User.LastName}").Trim();

                carName = schedule.Car?.CarName;

                licensePlate = schedule.Car?.LicensePlate;

                isPublicBooking = schedule.User.Password == "N/A";

            }



            return new ListResponseDto

            {

                Id = schedule.Id,

                UserId = schedule.UserId,

                UserName = userName,

                CarId = schedule.CarId,

                CarName = carName,

                LicensePlate = licensePlate,

                ScheduledDate = schedule.ScheduledDate,

                StatusCode = schedule.StatusCode,

                StatusName = schedule.StatusCodeNavigation?.Name,

                BranchId = schedule.BranchId,

                BranchName = schedule.Branch?.Name,

                AcceptedById = assignment?.ConsultantId,

                AcceptedByName = BuildUserDisplayName(assignment?.Consultant),

                AcceptedAt = assignment?.CreatedAt,

                AcceptNote = ExtractAssignmentMessage(assignment),

                IsPublicBooking = isPublicBooking

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




        private async Task<ResponseDto> MapToResponseDtoAsync(ScheduleService schedule)
        {
            // Load user info for audit fields
            User? createdByUser = null;
            User? updatedByUser = null;

            if (schedule.CreatedBy.HasValue)
            {
                createdByUser = await _userRepository.GetByIdAsync(schedule.CreatedBy.Value);
            }

            if (schedule.UpdatedBy.HasValue)
            {
                updatedByUser = await _userRepository.GetByIdAsync(schedule.UpdatedBy.Value);
            }

            return MapToResponseDto(schedule, createdByUser, updatedByUser);
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

