using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BE.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToTypeComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "branch",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    address_id = table.Column<long>(type: "bigint", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__branch__3213E83FE3594657", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permission",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__permissi__3213E83F06480003", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "province",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__province__3213E83FAF90B665", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__role__3213E83F5568318F", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "status_lookup",
                columns: table => new
                {
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__status_l__357D4CF81B29D667", x => x.code);
                });

            migrationBuilder.CreateTable(
                name: "type_component",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__type_com__3213E83F1079E0A0", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vehicle___3213E83F0F86B948", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ward",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    province_id = table.Column<long>(type: "bigint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ward__3213E83F8C7A9D69", x => x.id);
                    table.ForeignKey(
                        name: "FK__ward__province_i__18EBB532",
                        column: x => x.province_id,
                        principalTable: "province",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "role_permission",
                columns: table => new
                {
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    permission_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__role_per__C85A54635B06099E", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "FK__role_perm__permi__2D27B809",
                        column: x => x.permission_id,
                        principalTable: "permission",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__role_perm__role___2C3393D0",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "service_package",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    status_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__service___3213E83F898223E8", x => x.id);
                    table.ForeignKey(
                        name: "FK_service_package_status",
                        column: x => x.status_code,
                        principalTable: "status_lookup",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "component",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    quantity_stock = table.Column<int>(type: "int", nullable: true, defaultValue: 0),
                    type_component_id = table.Column<long>(type: "bigint", nullable: true),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__componen__3213E83FEF8FB5D3", x => x.id);
                    table.ForeignKey(
                        name: "FK__component__branc__2739D489",
                        column: x => x.branch_id,
                        principalTable: "branch",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__component__type___47DBAE45",
                        column: x => x.type_component_id,
                        principalTable: "type_component",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "address",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    postal_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    province_id = table.Column<long>(type: "bigint", nullable: true),
                    ward_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__address__3213E83F06B42C35", x => x.id);
                    table.ForeignKey(
                        name: "FK_address_province",
                        column: x => x.province_id,
                        principalTable: "province",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_address_ward",
                        column: x => x.ward_id,
                        principalTable: "ward",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "component_package",
                columns: table => new
                {
                    component_id = table.Column<long>(type: "bigint", nullable: false),
                    service_package_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__componen__C7D9836B03299182", x => new { x.component_id, x.service_package_id });
                    table.ForeignKey(
                        name: "FK__component__compo__4CA06362",
                        column: x => x.component_id,
                        principalTable: "component",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__component__servi__4D94879B",
                        column: x => x.service_package_id,
                        principalTable: "service_package",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    last_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    image = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    tax_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    status_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    is_delete = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    reset_key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    reset_date = table.Column<DateTime>(type: "datetime", nullable: true),
                    role_id = table.Column<long>(type: "bigint", nullable: true),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    address_id = table.Column<long>(type: "bigint", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified_date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__user__3213E83FC10C173A", x => x.id);
                    table.ForeignKey(
                        name: "FK__user__address_id__37A5467C",
                        column: x => x.address_id,
                        principalTable: "address",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__user__branch_id__36B12243",
                        column: x => x.branch_id,
                        principalTable: "branch",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__user__role_id__35BCFE0A",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_user_status",
                        column: x => x.status_code,
                        principalTable: "status_lookup",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "car",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    car_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    car_model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vehicle_type_id = table.Column<long>(type: "bigint", nullable: true),
                    color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    license_plate = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    vehicle_engine_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vin_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    year_of_manufacture = table.Column<int>(type: "int", nullable: true),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    last_modified_by = table.Column<long>(type: "bigint", nullable: true),
                    last_modified_date = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__car__3213E83F018A3F2D", x => x.id);
                    table.ForeignKey(
                        name: "FK__car__branch_id__403A8C7D",
                        column: x => x.branch_id,
                        principalTable: "branch",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__car__user_id__3E52440B",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__car__vehicle_typ__3F466844",
                        column: x => x.vehicle_type_id,
                        principalTable: "vehicle_type",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "history_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    old_data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    new_data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__history___3213E83F17401EB5", x => x.id);
                    table.ForeignKey(
                        name: "FK__history_l__user___7E37BEF6",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "maintenance_request",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    car_id = table.Column<long>(type: "bigint", nullable: true),
                    request_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    status_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    branch_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__maintena__3213E83F40C9A30A", x => x.id);
                    table.ForeignKey(
                        name: "FK__maintenan__branc__534D60F1",
                        column: x => x.branch_id,
                        principalTable: "branch",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__maintenan__car_i__5165187F",
                        column: x => x.car_id,
                        principalTable: "car",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__maintenan__user___5070F446",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_maintenance_request_status",
                        column: x => x.status_code,
                        principalTable: "status_lookup",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "schedule_service",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    car_id = table.Column<long>(type: "bigint", nullable: true),
                    scheduled_date = table.Column<DateTime>(type: "datetime", nullable: false),
                    status_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    branch_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__schedule__3213E83FDF0C0532", x => x.id);
                    table.ForeignKey(
                        name: "FK__schedule___branc__60A75C0F",
                        column: x => x.branch_id,
                        principalTable: "branch",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__schedule___car_i__5FB337D6",
                        column: x => x.car_id,
                        principalTable: "car",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__schedule___user___5EBF139D",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_schedule_service_status",
                        column: x => x.status_code,
                        principalTable: "status_lookup",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "vehicle_checkin",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    car_id = table.Column<long>(type: "bigint", nullable: true),
                    maintenance_request_id = table.Column<long>(type: "bigint", nullable: true),
                    mileage = table.Column<int>(type: "int", nullable: true),
                    notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    status_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vehicle___3213E83F0B264959", x => x.id);
                    table.ForeignKey(
                        name: "FK__vehicle_c__branc__3A4CA8FD",
                        column: x => x.branch_id,
                        principalTable: "branch",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__vehicle_c__car_i__5629CD9C",
                        column: x => x.car_id,
                        principalTable: "car",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__vehicle_c__maint__571DF1D5",
                        column: x => x.maintenance_request_id,
                        principalTable: "maintenance_request",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_vehicle_checkin_status",
                        column: x => x.status_code,
                        principalTable: "status_lookup",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "maintenance_ticket",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    schedule_service_id = table.Column<long>(type: "bigint", nullable: true),
                    car_id = table.Column<long>(type: "bigint", nullable: true),
                    consulter_id = table.Column<long>(type: "bigint", nullable: true),
                    technician_id = table.Column<long>(type: "bigint", nullable: true),
                    status_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vehicle_checkin_id = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    total_estimated_cost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    start_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    end_time = table.Column<DateTime>(type: "datetime", nullable: true),
                    priority_level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__maintena__3213E83F50060CEC", x => x.id);
                    table.ForeignKey(
                        name: "FK__maintenan__branc__6754599E",
                        column: x => x.branch_id,
                        principalTable: "branch",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__maintenan__car_i__6477ECF3",
                        column: x => x.car_id,
                        principalTable: "car",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__maintenan__consu__656C112C",
                        column: x => x.consulter_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__maintenan__sched__6383C8BA",
                        column: x => x.schedule_service_id,
                        principalTable: "schedule_service",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__maintenan__techn__66603565",
                        column: x => x.technician_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_maintenance_ticket_status",
                        column: x => x.status_code,
                        principalTable: "status_lookup",
                        principalColumn: "code");
                    table.ForeignKey(
                        name: "FK_maintenance_ticket_vehicle_checkin",
                        column: x => x.vehicle_checkin_id,
                        principalTable: "vehicle_checkin",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "vehicle_checkin_image",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vehicle_checkin_id = table.Column<long>(type: "bigint", nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__vehicle___3213E83FB2BBE90A", x => x.id);
                    table.ForeignKey(
                        name: "FK__vehicle_c__vehic__5AEE82B9",
                        column: x => x.vehicle_checkin_id,
                        principalTable: "vehicle_checkin",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "feedback",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    maintenance_ticket_id = table.Column<long>(type: "bigint", nullable: true),
                    rating = table.Column<int>(type: "int", nullable: true),
                    comment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    parent_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__feedback__3213E83FA2E44621", x => x.id);
                    table.ForeignKey(
                        name: "FK__feedback__mainte__797309D9",
                        column: x => x.maintenance_ticket_id,
                        principalTable: "maintenance_ticket",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__feedback__parent__1CBC4616",
                        column: x => x.parent_id,
                        principalTable: "feedback",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__feedback__user_i__787EE5A0",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "maintenance_ticket_technician",
                columns: table => new
                {
                    maintenance_ticket_id = table.Column<long>(type: "bigint", nullable: false),
                    technician_id = table.Column<long>(type: "bigint", nullable: false),
                    assigned_date = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    role_in_ticket = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__maintena__6AF1CD71FDA5026D", x => new { x.maintenance_ticket_id, x.technician_id });
                    table.ForeignKey(
                        name: "FK__maintenan__maint__0E6E26BF",
                        column: x => x.maintenance_ticket_id,
                        principalTable: "maintenance_ticket",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__maintenan__techn__0F624AF8",
                        column: x => x.technician_id,
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "service_task",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    maintenance_ticket_id = table.Column<long>(type: "bigint", nullable: true),
                    task_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__service___3213E83F0D074796", x => x.id);
                    table.ForeignKey(
                        name: "FK__service_t__maint__6A30C649",
                        column: x => x.maintenance_ticket_id,
                        principalTable: "maintenance_ticket",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_service_task_status",
                        column: x => x.status_code,
                        principalTable: "status_lookup",
                        principalColumn: "code");
                });

            migrationBuilder.CreateTable(
                name: "ticket_component",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    maintenance_ticket_id = table.Column<long>(type: "bigint", nullable: true),
                    component_id = table.Column<long>(type: "bigint", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ticket_c__3213E83FED95E67C", x => x.id);
                    table.ForeignKey(
                        name: "FK__ticket_co__compo__6E01572D",
                        column: x => x.component_id,
                        principalTable: "component",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__ticket_co__maint__6D0D32F4",
                        column: x => x.maintenance_ticket_id,
                        principalTable: "maintenance_ticket",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "total_receipt",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    car_id = table.Column<long>(type: "bigint", nullable: true),
                    maintenance_ticket_id = table.Column<long>(type: "bigint", nullable: true),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    currency_code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, defaultValue: "VND"),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    accountant_id = table.Column<long>(type: "bigint", nullable: true),
                    branch_id = table.Column<long>(type: "bigint", nullable: true),
                    status_code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    note = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__total_re__3213E83F94F6B881", x => x.id);
                    table.ForeignKey(
                        name: "FK__total_rec__accou__74AE54BC",
                        column: x => x.accountant_id,
                        principalTable: "user",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__total_rec__branc__75A278F5",
                        column: x => x.branch_id,
                        principalTable: "branch",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__total_rec__car_i__70DDC3D8",
                        column: x => x.car_id,
                        principalTable: "car",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK__total_rec__maint__71D1E811",
                        column: x => x.maintenance_ticket_id,
                        principalTable: "maintenance_ticket",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_total_receipt_status",
                        column: x => x.status_code,
                        principalTable: "status_lookup",
                        principalColumn: "code");
                });

            migrationBuilder.CreateIndex(
                name: "IX_address_province_id",
                table: "address",
                column: "province_id");

            migrationBuilder.CreateIndex(
                name: "IX_address_ward_id",
                table: "address",
                column: "ward_id");

            migrationBuilder.CreateIndex(
                name: "UQ__branch__72E12F1BECC84B31",
                table: "branch",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_car_branch_id",
                table: "car",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_car_user_id",
                table: "car",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_car_vehicle_type_id",
                table: "car",
                column: "vehicle_type_id");

            migrationBuilder.CreateIndex(
                name: "UQ__car__F72CD56E69C59AB5",
                table: "car",
                column: "license_plate",
                unique: true,
                filter: "[license_plate] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_component_branch_id",
                table: "component",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_component_type_component_id",
                table: "component",
                column: "type_component_id");

            migrationBuilder.CreateIndex(
                name: "UQ__componen__357D4CF90AECE297",
                table: "component",
                column: "code",
                unique: true,
                filter: "[code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_component_package_service_package_id",
                table: "component_package",
                column: "service_package_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_maintenance_ticket_id",
                table: "feedback",
                column: "maintenance_ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_parent_id",
                table: "feedback",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_feedback_user_id",
                table: "feedback",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_history_log_user_id",
                table: "history_log",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_request_branch_id",
                table: "maintenance_request",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_request_car_id",
                table: "maintenance_request",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_request_status_code",
                table: "maintenance_request",
                column: "status_code");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_request_user_id",
                table: "maintenance_request",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_ticket_branch_id",
                table: "maintenance_ticket",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_ticket_car_id",
                table: "maintenance_ticket",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_ticket_consulter_id",
                table: "maintenance_ticket",
                column: "consulter_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_ticket_schedule_service_id",
                table: "maintenance_ticket",
                column: "schedule_service_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_ticket_status_code",
                table: "maintenance_ticket",
                column: "status_code");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_ticket_technician_id",
                table: "maintenance_ticket",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_ticket_vehicle_checkin_id",
                table: "maintenance_ticket",
                column: "vehicle_checkin_id");

            migrationBuilder.CreateIndex(
                name: "IX_maintenance_ticket_technician_technician_id",
                table: "maintenance_ticket_technician",
                column: "technician_id");

            migrationBuilder.CreateIndex(
                name: "UQ__permissi__357D4CF914571226",
                table: "permission",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__province__72E12F1B02074DA7",
                table: "province",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_role_permission_permission_id",
                table: "role_permission",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_service_branch_id",
                table: "schedule_service",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_service_car_id",
                table: "schedule_service",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_service_status_code",
                table: "schedule_service",
                column: "status_code");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_service_user_id",
                table: "schedule_service",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_service_package_status_code",
                table: "service_package",
                column: "status_code");

            migrationBuilder.CreateIndex(
                name: "IX_service_task_status_code",
                table: "service_task",
                column: "status_code");

            migrationBuilder.CreateIndex(
                name: "IX_service_task_ticket_id",
                table: "service_task",
                column: "maintenance_ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_component_component_id",
                table: "ticket_component",
                column: "component_id");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_component_ticket_id",
                table: "ticket_component",
                column: "maintenance_ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_total_receipt_accountant_id",
                table: "total_receipt",
                column: "accountant_id");

            migrationBuilder.CreateIndex(
                name: "IX_total_receipt_branch_id",
                table: "total_receipt",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_total_receipt_car_id",
                table: "total_receipt",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_total_receipt_maintenance_ticket_id",
                table: "total_receipt",
                column: "maintenance_ticket_id");

            migrationBuilder.CreateIndex(
                name: "IX_total_receipt_status_code",
                table: "total_receipt",
                column: "status_code");

            migrationBuilder.CreateIndex(
                name: "IX_user_address_id",
                table: "user",
                column: "address_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_branch_id",
                table: "user",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_role_id",
                table: "user",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_status_code",
                table: "user",
                column: "status_code");

            migrationBuilder.CreateIndex(
                name: "UQ__user__F3DBC5725BB1EDAA",
                table: "user",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_checkin_branch_id",
                table: "vehicle_checkin",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_checkin_car_id",
                table: "vehicle_checkin",
                column: "car_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_checkin_maintenance_request_id",
                table: "vehicle_checkin",
                column: "maintenance_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_checkin_status_code",
                table: "vehicle_checkin",
                column: "status_code");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_checkin_image_vehicle_checkin_id",
                table: "vehicle_checkin_image",
                column: "vehicle_checkin_id");

            migrationBuilder.CreateIndex(
                name: "IX_ward_province_id",
                table: "ward",
                column: "province_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "component_package");

            migrationBuilder.DropTable(
                name: "feedback");

            migrationBuilder.DropTable(
                name: "history_log");

            migrationBuilder.DropTable(
                name: "maintenance_ticket_technician");

            migrationBuilder.DropTable(
                name: "role_permission");

            migrationBuilder.DropTable(
                name: "service_task");

            migrationBuilder.DropTable(
                name: "ticket_component");

            migrationBuilder.DropTable(
                name: "total_receipt");

            migrationBuilder.DropTable(
                name: "vehicle_checkin_image");

            migrationBuilder.DropTable(
                name: "service_package");

            migrationBuilder.DropTable(
                name: "permission");

            migrationBuilder.DropTable(
                name: "component");

            migrationBuilder.DropTable(
                name: "maintenance_ticket");

            migrationBuilder.DropTable(
                name: "type_component");

            migrationBuilder.DropTable(
                name: "schedule_service");

            migrationBuilder.DropTable(
                name: "vehicle_checkin");

            migrationBuilder.DropTable(
                name: "maintenance_request");

            migrationBuilder.DropTable(
                name: "car");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "vehicle_type");

            migrationBuilder.DropTable(
                name: "address");

            migrationBuilder.DropTable(
                name: "branch");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "status_lookup");

            migrationBuilder.DropTable(
                name: "ward");

            migrationBuilder.DropTable(
                name: "province");
        }
    }
}
