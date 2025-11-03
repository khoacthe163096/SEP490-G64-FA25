using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BE.models;

public partial class CarMaintenanceDbContext : DbContext
{
    public CarMaintenanceDbContext()
    {
    }

    public CarMaintenanceDbContext(DbContextOptions<CarMaintenanceDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Branch> Branches { get; set; }

    public virtual DbSet<Car> Cars { get; set; }

    public virtual DbSet<Component> Components { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<HistoryLog> HistoryLogs { get; set; }

    public virtual DbSet<MaintenanceRequest> MaintenanceRequests { get; set; }

    public virtual DbSet<MaintenanceTicket> MaintenanceTickets { get; set; }

    public virtual DbSet<MaintenanceTicketTechnician> MaintenanceTicketTechnicians { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<ScheduleService> ScheduleServices { get; set; }

    public virtual DbSet<ServicePackage> ServicePackages { get; set; }

    public virtual DbSet<ServiceTask> ServiceTasks { get; set; }

    public virtual DbSet<StatusLookup> StatusLookups { get; set; }

    public virtual DbSet<TicketComponent> TicketComponents { get; set; }

    public virtual DbSet<TotalReceipt> TotalReceipts { get; set; }

    public virtual DbSet<TypeComponent> TypeComponents { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VehicleCheckin> VehicleCheckins { get; set; }

    public virtual DbSet<VehicleCheckinImage> VehicleCheckinImages { get; set; }

    public virtual DbSet<VehicleType> VehicleTypes { get; set; }

    public virtual DbSet<Ward> Wards { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=HP\\SQLEXPRESS;Database=CarMaintenanceDB;User Id=sa;Password=123;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__address__3213E83F3C07714E");

            entity.ToTable("address");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .HasColumnName("postal_code");
            entity.Property(e => e.ProvinceId).HasColumnName("province_id");
            entity.Property(e => e.Street)
                .HasMaxLength(200)
                .HasColumnName("street");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.Property(e => e.WardId).HasColumnName("ward_id");

            entity.HasOne(d => d.Province).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.ProvinceId)
                .HasConstraintName("FK_address_province");

            entity.HasOne(d => d.Ward).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.WardId)
                .HasConstraintName("FK_address_ward");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__branch__3213E83FB74494EE");

            entity.ToTable("branch");

            entity.HasIndex(e => e.Name, "UQ__branch__72E12F1B13315AA1").IsUnique();

            entity.HasIndex(e => e.Name, "UQ__branch__72E12F1B843266F2").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<Car>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__car__3213E83F7DD65B68");

            entity.ToTable("car");

            entity.HasIndex(e => e.LicensePlate, "UQ__car__F72CD56ED5E8803D").IsUnique();

            entity.HasIndex(e => e.LicensePlate, "UQ__car__F72CD56EFF6A4EAC").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CarModel)
                .HasMaxLength(100)
                .HasColumnName("car_model");
            entity.Property(e => e.CarName)
                .HasMaxLength(100)
                .HasColumnName("car_name");
            entity.Property(e => e.Color)
                .HasMaxLength(50)
                .HasColumnName("color");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.LastModifiedBy).HasColumnName("last_modified_by");
            entity.Property(e => e.LastModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("last_modified_date");
            entity.Property(e => e.LicensePlate)
                .HasMaxLength(20)
                .HasColumnName("license_plate");
            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.VehicleEngineNumber)
                .HasMaxLength(100)
                .HasColumnName("vehicle_engine_number");
            entity.Property(e => e.VehicleTypeId).HasColumnName("vehicle_type_id");
            entity.Property(e => e.VinNumber)
                .HasMaxLength(100)
                .HasColumnName("vin_number");
            entity.Property(e => e.YearOfManufacture).HasColumnName("year_of_manufacture");

            entity.HasOne(d => d.Branch).WithMany(p => p.Cars)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__car__branch_id__0F624AF8");

            entity.HasOne(d => d.User).WithMany(p => p.Cars)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__car__user_id__10566F31");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Cars)
                .HasForeignKey(d => d.VehicleTypeId)
                .HasConstraintName("FK__car__vehicle_typ__114A936A");
        });

        modelBuilder.Entity<Component>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__componen__3213E83F2600DB5C");

            entity.ToTable("component");

            entity.HasIndex(e => e.Code, "UQ__componen__357D4CF9EB3F95FD").IsUnique();

            entity.HasIndex(e => e.Code, "UQ__componen__357D4CF9F4B55F2F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.PurchasePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("purchase_price");
            entity.Property(e => e.QuantityStock)
                .HasDefaultValue(0)
                .HasColumnName("quantity_stock");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");
            entity.Property(e => e.TypeComponentId).HasColumnName("type_component_id");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Branch).WithMany(p => p.Components)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__component__branc__123EB7A3");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Components)
                .HasForeignKey(d => d.StatusCode)
                .HasConstraintName("FK_component_status");

            entity.HasOne(d => d.TypeComponent).WithMany(p => p.Components)
                .HasForeignKey(d => d.TypeComponentId)
                .HasConstraintName("FK__component__type___1332DBDC");

            entity.HasMany(d => d.ServicePackages).WithMany(p => p.Components)
                .UsingEntity<Dictionary<string, object>>(
                    "ComponentPackage",
                    r => r.HasOne<ServicePackage>().WithMany()
                        .HasForeignKey("ServicePackageId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__component__servi__160F4887"),
                    l => l.HasOne<Component>().WithMany()
                        .HasForeignKey("ComponentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__component__compo__151B244E"),
                    j =>
                    {
                        j.HasKey("ComponentId", "ServicePackageId").HasName("PK__componen__C7D9836B110C47EE");
                        j.ToTable("component_package");
                        j.IndexerProperty<long>("ComponentId").HasColumnName("component_id");
                        j.IndexerProperty<long>("ServicePackageId").HasColumnName("service_package_id");
                    });
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__feedback__3213E83FFA70065C");

            entity.ToTable("feedback");

            entity.HasIndex(e => e.UserId, "IX_feedback_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Comment)
                .HasMaxLength(255)
                .HasColumnName("comment");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.MaintenanceTicketId).HasColumnName("maintenance_ticket_id");
            entity.Property(e => e.ParentId).HasColumnName("parent_id");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.MaintenanceTicket).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.MaintenanceTicketId)
                .HasConstraintName("FK__feedback__mainte__17036CC0");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__feedback__parent__17F790F9");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__feedback__user_i__18EBB532");
        });

        modelBuilder.Entity<HistoryLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__history___3213E83F0872CD73");

            entity.ToTable("history_log");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(100)
                .HasColumnName("action");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.NewData).HasColumnName("new_data");
            entity.Property(e => e.OldData).HasColumnName("old_data");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.HistoryLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__history_l__user___19DFD96B");
        });

        modelBuilder.Entity<MaintenanceRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__maintena__3213E83F9E85BD96");

            entity.ToTable("maintenance_request");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.RequestDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("request_date");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Branch).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__maintenan__branc__1AD3FDA4");

            entity.HasOne(d => d.Car).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__maintenan__car_i__1BC821DD");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.StatusCode)
                .HasConstraintName("FK_maintenance_request_status");

            entity.HasOne(d => d.User).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__maintenan__user___1CBC4616");
        });

        modelBuilder.Entity<MaintenanceTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__maintena__3213E83F42A812AD");

            entity.ToTable("maintenance_ticket");

            entity.HasIndex(e => e.CarId, "IX_maintenance_ticket_car_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.ConsulterId).HasColumnName("consulter_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.PriorityLevel)
                .HasMaxLength(20)
                .HasColumnName("priority_level");
            entity.Property(e => e.ScheduleServiceId).HasColumnName("schedule_service_id");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");
            entity.Property(e => e.TechnicianId).HasColumnName("technician_id");
            entity.Property(e => e.TotalEstimatedCost)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_estimated_cost");
            entity.Property(e => e.VehicleCheckinId).HasColumnName("vehicle_checkin_id");

            entity.HasOne(d => d.Branch).WithMany(p => p.MaintenanceTickets)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__maintenan__branc__1EA48E88");

            entity.HasOne(d => d.Car).WithMany(p => p.MaintenanceTickets)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__maintenan__car_i__1F98B2C1");

            entity.HasOne(d => d.Consulter).WithMany(p => p.MaintenanceTicketConsulters)
                .HasForeignKey(d => d.ConsulterId)
                .HasConstraintName("FK__maintenan__consu__208CD6FA");

            entity.HasOne(d => d.ScheduleService).WithMany(p => p.MaintenanceTickets)
                .HasForeignKey(d => d.ScheduleServiceId)
                .HasConstraintName("FK__maintenan__sched__2180FB33");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.MaintenanceTickets)
                .HasForeignKey(d => d.StatusCode)
                .HasConstraintName("FK_maintenance_ticket_status");

            entity.HasOne(d => d.Technician).WithMany(p => p.MaintenanceTicketTechnicians)
                .HasForeignKey(d => d.TechnicianId)
                .HasConstraintName("FK__maintenan__techn__22751F6C");

            entity.HasOne(d => d.VehicleCheckin).WithMany(p => p.MaintenanceTickets)
                .HasForeignKey(d => d.VehicleCheckinId)
                .HasConstraintName("FK_maintenance_ticket_vehicle_checkin");
        });

        modelBuilder.Entity<MaintenanceTicketTechnician>(entity =>
        {
            entity.HasKey(e => new { e.MaintenanceTicketId, e.TechnicianId }).HasName("PK__maintena__6AF1CD71D61B6839");

            entity.ToTable("maintenance_ticket_technician");

            entity.Property(e => e.MaintenanceTicketId).HasColumnName("maintenance_ticket_id");
            entity.Property(e => e.TechnicianId).HasColumnName("technician_id");
            entity.Property(e => e.AssignedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("assigned_date");
            entity.Property(e => e.RoleInTicket)
                .HasMaxLength(100)
                .HasColumnName("role_in_ticket");

            entity.HasOne(d => d.MaintenanceTicket).WithMany(p => p.MaintenanceTicketTechnicians)
                .HasForeignKey(d => d.MaintenanceTicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__maintenan__maint__25518C17");

            entity.HasOne(d => d.Technician).WithMany(p => p.MaintenanceTicketTechniciansNavigation)
                .HasForeignKey(d => d.TechnicianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__maintenan__techn__2645B050");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__permissi__3213E83FC26B3A0A");

            entity.ToTable("permission");

            entity.HasIndex(e => e.Code, "UQ__permissi__357D4CF9588F8F14").IsUnique();

            entity.HasIndex(e => e.Code, "UQ__permissi__357D4CF9EC8599B3").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__province__3213E83FFC2A0223");

            entity.ToTable("province");

            entity.HasIndex(e => e.Name, "UQ__province__72E12F1BBFD11105").IsUnique();

            entity.HasIndex(e => e.Name, "UQ__province__72E12F1BCBEDA648").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__role__3213E83FD11EE5EB");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__role_perm__permi__2739D489"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__role_perm__role___282DF8C2"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("PK__role_per__C85A54630E202CD8");
                        j.ToTable("role_permission");
                        j.IndexerProperty<long>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<long>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<ScheduleService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__schedule__3213E83F452E69DD");

            entity.ToTable("schedule_service");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.ScheduledDate)
                .HasColumnType("datetime")
                .HasColumnName("scheduled_date");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Branch).WithMany(p => p.ScheduleServices)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__schedule___branc__29221CFB");

            entity.HasOne(d => d.Car).WithMany(p => p.ScheduleServices)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__schedule___car_i__2A164134");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.ScheduleServices)
                .HasForeignKey(d => d.StatusCode)
                .HasConstraintName("FK_schedule_service_status");

            entity.HasOne(d => d.User).WithMany(p => p.ScheduleServices)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__schedule___user___2B0A656D");
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__service___3213E83FAEA6F10F");

            entity.ToTable("service_package");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");

            entity.HasOne(d => d.Branch).WithMany(p => p.ServicePackages)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_service_package_branch");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.ServicePackages)
                .HasForeignKey(d => d.StatusCode)
                .HasConstraintName("FK_service_package_status");
        });

        modelBuilder.Entity<ServiceTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__service___3213E83F224CD232");

            entity.ToTable("service_task");

            entity.HasIndex(e => e.MaintenanceTicketId, "IX_service_task_ticket_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.MaintenanceTicketId).HasColumnName("maintenance_ticket_id");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");
            entity.Property(e => e.TaskName)
                .HasMaxLength(100)
                .HasColumnName("task_name");

            entity.HasOne(d => d.MaintenanceTicket).WithMany(p => p.ServiceTasks)
                .HasForeignKey(d => d.MaintenanceTicketId)
                .HasConstraintName("FK__service_t__maint__2EDAF651");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.ServiceTasks)
                .HasForeignKey(d => d.StatusCode)
                .HasConstraintName("FK_service_task_status");
        });

        modelBuilder.Entity<StatusLookup>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PK__status_l__357D4CF84FCC0B8C");

            entity.ToTable("status_lookup");

            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TicketComponent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ticket_c__3213E83F03CACB0F");

            entity.ToTable("ticket_component");

            entity.HasIndex(e => e.MaintenanceTicketId, "IX_ticket_component_ticket_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ComponentId).HasColumnName("component_id");
            entity.Property(e => e.MaintenanceTicketId).HasColumnName("maintenance_ticket_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Component).WithMany(p => p.TicketComponents)
                .HasForeignKey(d => d.ComponentId)
                .HasConstraintName("FK__ticket_co__compo__30C33EC3");

            entity.HasOne(d => d.MaintenanceTicket).WithMany(p => p.TicketComponents)
                .HasForeignKey(d => d.MaintenanceTicketId)
                .HasConstraintName("FK__ticket_co__maint__31B762FC");
        });

        modelBuilder.Entity<TotalReceipt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__total_re__3213E83F06883DB5");

            entity.ToTable("total_receipt");

            entity.HasIndex(e => e.BranchId, "IX_total_receipt_branch_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountantId).HasColumnName("accountant_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrencyCode)
                .HasMaxLength(10)
                .HasDefaultValue("VND")
                .HasColumnName("currency_code");
            entity.Property(e => e.MaintenanceTicketId).HasColumnName("maintenance_ticket_id");
            entity.Property(e => e.Note)
                .HasMaxLength(255)
                .HasColumnName("note");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");

            entity.HasOne(d => d.Accountant).WithMany(p => p.TotalReceipts)
                .HasForeignKey(d => d.AccountantId)
                .HasConstraintName("FK__total_rec__accou__32AB8735");

            entity.HasOne(d => d.Branch).WithMany(p => p.TotalReceipts)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__total_rec__branc__339FAB6E");

            entity.HasOne(d => d.Car).WithMany(p => p.TotalReceipts)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__total_rec__car_i__3493CFA7");

            entity.HasOne(d => d.MaintenanceTicket).WithMany(p => p.TotalReceipts)
                .HasForeignKey(d => d.MaintenanceTicketId)
                .HasConstraintName("FK__total_rec__maint__3587F3E0");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.TotalReceipts)
                .HasForeignKey(d => d.StatusCode)
                .HasConstraintName("FK_total_receipt_status");
        });

        modelBuilder.Entity<TypeComponent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__type_com__3213E83FF0758C6D");

            entity.ToTable("type_component");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");

            entity.HasOne(d => d.Branch).WithMany(p => p.TypeComponents)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_type_component_branch");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.TypeComponents)
                .HasForeignKey(d => d.StatusCode)
                .HasConstraintName("FK_type_component_status");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user__3213E83FBF67F270");

            entity.ToTable("user");

            entity.HasIndex(e => e.BranchId, "IX_user_branch_id");

            entity.HasIndex(e => e.Username, "UQ__user__F3DBC57246709D74").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__user__F3DBC5729EFF6D1F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_date");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(20)
                .HasColumnName("gender");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.IsDelete)
                .HasDefaultValue(false)
                .HasColumnName("is_delete");
            entity.Property(e => e.LastModifiedBy).HasColumnName("last_modified_by");
            entity.Property(e => e.LastModifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("last_modified_date");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.ResetDate)
                .HasColumnType("datetime")
                .HasColumnName("reset_date");
            entity.Property(e => e.ResetKey)
                .HasMaxLength(100)
                .HasColumnName("reset_key");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");
            entity.Property(e => e.TaxCode)
                .HasMaxLength(50)
                .HasColumnName("tax_code");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Address).WithMany(p => p.Users)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__user__address_id__395884C4");

            entity.HasOne(d => d.Branch).WithMany(p => p.Users)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__user__branch_id__3A4CA8FD");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__user__role_id__3B40CD36");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusCode)
                .HasConstraintName("FK_user_status");
        });

        modelBuilder.Entity<VehicleCheckin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__vehicle___3213E83FD39961F5");

            entity.ToTable("vehicle_checkin");

            entity.HasIndex(e => e.CarId, "IX_vehicle_checkin_car_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.MaintenanceRequestId).HasColumnName("maintenance_request_id");
            entity.Property(e => e.Mileage).HasColumnName("mileage");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");

            entity.HasOne(d => d.Branch).WithMany(p => p.VehicleCheckins)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__vehicle_c__branc__3D2915A8");

            entity.HasOne(d => d.Car).WithMany(p => p.VehicleCheckins)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__vehicle_c__car_i__3E1D39E1");

            entity.HasOne(d => d.MaintenanceRequest).WithMany(p => p.VehicleCheckins)
                .HasForeignKey(d => d.MaintenanceRequestId)
                .HasConstraintName("FK__vehicle_c__maint__3F115E1A");

            entity.HasOne(d => d.StatusCodeNavigation).WithMany(p => p.VehicleCheckins)
                .HasForeignKey(d => d.StatusCode)
                .HasConstraintName("FK_vehicle_checkin_status");
        });

        modelBuilder.Entity<VehicleCheckinImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__vehicle___3213E83F72675E4A");

            entity.ToTable("vehicle_checkin_image");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl).HasColumnName("image_url");
            entity.Property(e => e.VehicleCheckinId).HasColumnName("vehicle_checkin_id");

            entity.HasOne(d => d.VehicleCheckin).WithMany(p => p.VehicleCheckinImages)
                .HasForeignKey(d => d.VehicleCheckinId)
                .HasConstraintName("FK__vehicle_c__vehic__41EDCAC5");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__vehicle___3213E83F1720B5B4");

            entity.ToTable("vehicle_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Ward>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ward__3213E83FF551C234");

            entity.ToTable("ward");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ProvinceId).HasColumnName("province_id");

            entity.HasOne(d => d.Province).WithMany(p => p.Wards)
                .HasForeignKey(d => d.ProvinceId)
                .HasConstraintName("FK__ward__province_i__42E1EEFE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
