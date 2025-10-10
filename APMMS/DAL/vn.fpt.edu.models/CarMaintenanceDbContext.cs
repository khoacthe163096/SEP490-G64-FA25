using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DAL.vn.fpt.edu.entities;
using DAL.vn.fpt.edu.models;

namespace DAL.vn.fpt.edu.models;

public partial class CarMaintenanceDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long, Microsoft.AspNetCore.Identity.IdentityUserClaim<long>, Microsoft.AspNetCore.Identity.IdentityUserRole<long>, Microsoft.AspNetCore.Identity.IdentityUserLogin<long>, Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>, Microsoft.AspNetCore.Identity.IdentityUserToken<long>>
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
        => optionsBuilder.UseSqlServer("server=DESKTOP-ITTJS91;database=CarMaintenanceDB;uid=sa;pwd=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Identity tables
        modelBuilder.Entity<ApplicationUser>(b =>
        {
            b.ToTable("user");
            b.HasKey(u => u.Id);
            b.Property(u => u.Id).HasColumnName("id");
            b.Property(u => u.UserName).HasColumnName("username");
            b.Property(u => u.Email).HasColumnName("email");
            b.Property(u => u.PasswordHash).HasColumnName("password");
            b.Property(u => u.PhoneNumber).HasColumnName("phone");
            b.Property(u => u.RoleId).HasColumnName("role_id");
            b.Ignore(u => u.NormalizedUserName);
            b.Ignore(u => u.NormalizedEmail);
            b.Ignore(u => u.EmailConfirmed);
            b.Ignore(u => u.PhoneNumberConfirmed);
            b.Ignore(u => u.SecurityStamp);
            b.Ignore(u => u.ConcurrencyStamp);
            b.Ignore(u => u.LockoutEnabled);
            b.Ignore(u => u.LockoutEnd);
            b.Ignore(u => u.AccessFailedCount);
            b.Ignore(u => u.TwoFactorEnabled);
        });

        modelBuilder.Entity<ApplicationRole>(b =>
        {
            b.ToTable("role");
            b.HasKey(r => r.Id);
            b.Property(r => r.Id).HasColumnName("id");
            b.Property(r => r.Name).HasColumnName("name");
            b.Ignore(r => r.NormalizedName);
            b.Ignore(r => r.ConcurrencyStamp);
        });

        // Ignore Identity tables that are not used
        modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityUserClaim<long>>();
        modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityRoleClaim<long>>();
        modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityUserLogin<long>>();
        modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityUserToken<long>>();
        modelBuilder.Ignore<Microsoft.AspNetCore.Identity.IdentityUserRole<long>>();
        
        // Ignore the original Role and User entities to avoid conflict with ApplicationRole and ApplicationUser
        modelBuilder.Ignore<Role>();
        modelBuilder.Ignore<User>();
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__address__3213E83F06B42C35");

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
            entity.HasKey(e => e.Id).HasName("PK__branch__3213E83FE3594657");

            entity.ToTable("branch");

            entity.HasIndex(e => e.Name, "UQ__branch__72E12F1BECC84B31").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__car__3213E83F018A3F2D");

            entity.ToTable("car");

            entity.HasIndex(e => e.LicensePlate, "UQ__car__F72CD56E69C59AB5").IsUnique();

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
                .HasConstraintName("FK__car__branch_id__403A8C7D");

            entity.HasOne(d => d.User).WithMany(p => p.Cars)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__car__user_id__3E52440B");

            entity.HasOne(d => d.VehicleType).WithMany(p => p.Cars)
                .HasForeignKey(d => d.VehicleTypeId)
                .HasConstraintName("FK__car__vehicle_typ__3F466844");
        });

        modelBuilder.Entity<Component>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__componen__3213E83FEF8FB5D3");

            entity.ToTable("component");

            entity.HasIndex(e => e.Code, "UQ__componen__357D4CF90AECE297").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.QuantityStock)
                .HasDefaultValue(0)
                .HasColumnName("quantity_stock");
            entity.Property(e => e.TypeComponentId).HasColumnName("type_component_id");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.TypeComponent).WithMany(p => p.Components)
                .HasForeignKey(d => d.TypeComponentId)
                .HasConstraintName("FK__component__type___47DBAE45");

            entity.HasMany(d => d.ServicePackages).WithMany(p => p.Components)
                .UsingEntity<Dictionary<string, object>>(
                    "ComponentPackage",
                    r => r.HasOne<ServicePackage>().WithMany()
                        .HasForeignKey("ServicePackageId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__component__servi__4D94879B"),
                    l => l.HasOne<Component>().WithMany()
                        .HasForeignKey("ComponentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__component__compo__4CA06362"),
                    j =>
                    {
                        j.HasKey("ComponentId", "ServicePackageId").HasName("PK__componen__C7D9836B03299182");
                        j.ToTable("component_package");
                        j.IndexerProperty<long>("ComponentId").HasColumnName("component_id");
                        j.IndexerProperty<long>("ServicePackageId").HasColumnName("service_package_id");
                    });
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__feedback__3213E83FA2E44621");

            entity.ToTable("feedback");

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
                .HasConstraintName("FK__feedback__mainte__797309D9");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK__feedback__parent__1CBC4616");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__feedback__user_i__787EE5A0");
        });

        modelBuilder.Entity<HistoryLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__history___3213E83F17401EB5");

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
                .HasConstraintName("FK__history_l__user___7E37BEF6");
        });

        modelBuilder.Entity<MaintenanceRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__maintena__3213E83F40C9A30A");

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
                .HasConstraintName("FK__maintenan__branc__534D60F1");

            entity.HasOne(d => d.Car).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__maintenan__car_i__5165187F");

            entity.HasOne(d => d.User).WithMany(p => p.MaintenanceRequests)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__maintenan__user___5070F446");
        });

        modelBuilder.Entity<MaintenanceTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__maintena__3213E83F50060CEC");

            entity.ToTable("maintenance_ticket");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.ConsulterId).HasColumnName("consulter_id");
            entity.Property(e => e.ScheduleServiceId).HasColumnName("schedule_service_id");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");
            entity.Property(e => e.TechnicianId).HasColumnName("technician_id");

            entity.HasOne(d => d.Branch).WithMany(p => p.MaintenanceTickets)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__maintenan__branc__6754599E");

            entity.HasOne(d => d.Car).WithMany(p => p.MaintenanceTickets)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__maintenan__car_i__6477ECF3");

            entity.HasOne(d => d.Consulter).WithMany(p => p.MaintenanceTicketConsulters)
                .HasForeignKey(d => d.ConsulterId)
                .HasConstraintName("FK__maintenan__consu__656C112C");

            entity.HasOne(d => d.ScheduleService).WithMany(p => p.MaintenanceTickets)
                .HasForeignKey(d => d.ScheduleServiceId)
                .HasConstraintName("FK__maintenan__sched__6383C8BA");

            entity.HasOne(d => d.Technician).WithMany(p => p.MaintenanceTicketTechnicians)
                .HasForeignKey(d => d.TechnicianId)
                .HasConstraintName("FK__maintenan__techn__66603565");
        });

        modelBuilder.Entity<MaintenanceTicketTechnician>(entity =>
        {
            entity.HasKey(e => new { e.MaintenanceTicketId, e.TechnicianId }).HasName("PK__maintena__6AF1CD71FDA5026D");

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
                .HasConstraintName("FK__maintenan__maint__0E6E26BF");

            entity.HasOne(d => d.Technician).WithMany(p => p.MaintenanceTicketTechniciansNavigation)
                .HasForeignKey(d => d.TechnicianId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__maintenan__techn__0F624AF8");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__permissi__3213E83F06480003");

            entity.ToTable("permission");

            entity.HasIndex(e => e.Code, "UQ__permissi__357D4CF914571226").IsUnique();

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
            entity.HasKey(e => e.Id).HasName("PK__province__3213E83FAF90B665");

            entity.ToTable("province");

            entity.HasIndex(e => e.Name, "UQ__province__72E12F1B02074DA7").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });


        modelBuilder.Entity<ScheduleService>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__schedule__3213E83FDF0C0532");

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
                .HasConstraintName("FK__schedule___branc__60A75C0F");

            entity.HasOne(d => d.Car).WithMany(p => p.ScheduleServices)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__schedule___car_i__5FB337D6");

            entity.HasOne(d => d.User).WithMany(p => p.ScheduleServices)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__schedule___user___5EBF139D");
        });

        modelBuilder.Entity<ServicePackage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__service___3213E83F898223E8");

            entity.ToTable("service_package");

            entity.Property(e => e.Id).HasColumnName("id");
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
        });

        modelBuilder.Entity<ServiceTask>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__service___3213E83F0D074796");

            entity.ToTable("service_task");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.MaintenanceTicketId).HasColumnName("maintenance_ticket_id");
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");
            entity.Property(e => e.TaskName)
                .HasMaxLength(100)
                .HasColumnName("task_name");

            entity.HasOne(d => d.MaintenanceTicket).WithMany(p => p.ServiceTasks)
                .HasForeignKey(d => d.MaintenanceTicketId)
                .HasConstraintName("FK__service_t__maint__6A30C649");
        });

        modelBuilder.Entity<StatusLookup>(entity =>
        {
            entity.HasKey(e => e.Code).HasName("PK__status_l__357D4CF81B29D667");

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
            entity.HasKey(e => e.Id).HasName("PK__ticket_c__3213E83FED95E67C");

            entity.ToTable("ticket_component");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ComponentId).HasColumnName("component_id");
            entity.Property(e => e.MaintenanceTicketId).HasColumnName("maintenance_ticket_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("unit_price");

            entity.HasOne(d => d.Component).WithMany(p => p.TicketComponents)
                .HasForeignKey(d => d.ComponentId)
                .HasConstraintName("FK__ticket_co__compo__6E01572D");

            entity.HasOne(d => d.MaintenanceTicket).WithMany(p => p.TicketComponents)
                .HasForeignKey(d => d.MaintenanceTicketId)
                .HasConstraintName("FK__ticket_co__maint__6D0D32F4");
        });

        modelBuilder.Entity<TotalReceipt>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__total_re__3213E83F94F6B881");

            entity.ToTable("total_receipt");

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
            entity.Property(e => e.StatusCode)
                .HasMaxLength(50)
                .HasColumnName("status_code");

            entity.HasOne(d => d.Accountant).WithMany(p => p.TotalReceipts)
                .HasForeignKey(d => d.AccountantId)
                .HasConstraintName("FK__total_rec__accou__74AE54BC");

            entity.HasOne(d => d.Branch).WithMany(p => p.TotalReceipts)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK__total_rec__branc__75A278F5");

            entity.HasOne(d => d.Car).WithMany(p => p.TotalReceipts)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__total_rec__car_i__70DDC3D8");

            entity.HasOne(d => d.MaintenanceTicket).WithMany(p => p.TotalReceipts)
                .HasForeignKey(d => d.MaintenanceTicketId)
                .HasConstraintName("FK__total_rec__maint__71D1E811");
        });

        modelBuilder.Entity<TypeComponent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__type_com__3213E83F1079E0A0");

            entity.ToTable("type_component");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });


        modelBuilder.Entity<VehicleCheckin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__vehicle___3213E83F0B264959");

            entity.ToTable("vehicle_checkin");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CarId).HasColumnName("car_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.MaintenanceRequestId).HasColumnName("maintenance_request_id");
            entity.Property(e => e.Mileage).HasColumnName("mileage");
            entity.Property(e => e.Notes)
                .HasMaxLength(255)
                .HasColumnName("notes");

            entity.HasOne(d => d.Car).WithMany(p => p.VehicleCheckins)
                .HasForeignKey(d => d.CarId)
                .HasConstraintName("FK__vehicle_c__car_i__5629CD9C");

            entity.HasOne(d => d.MaintenanceRequest).WithMany(p => p.VehicleCheckins)
                .HasForeignKey(d => d.MaintenanceRequestId)
                .HasConstraintName("FK__vehicle_c__maint__571DF1D5");
        });

        modelBuilder.Entity<VehicleCheckinImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__vehicle___3213E83FB2BBE90A");

            entity.ToTable("vehicle_checkin_image");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.VehicleCheckinId).HasColumnName("vehicle_checkin_id");

            entity.HasOne(d => d.VehicleCheckin).WithMany(p => p.VehicleCheckinImages)
                .HasForeignKey(d => d.VehicleCheckinId)
                .HasConstraintName("FK__vehicle_c__vehic__5AEE82B9");
        });

        modelBuilder.Entity<VehicleType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__vehicle___3213E83F0F86B948");

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
            entity.HasKey(e => e.Id).HasName("PK__ward__3213E83F8C7A9D69");

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
                .HasConstraintName("FK__ward__province_i__18EBB532");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
