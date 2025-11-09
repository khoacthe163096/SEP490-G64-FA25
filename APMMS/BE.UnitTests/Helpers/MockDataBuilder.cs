using BE.vn.fpt.edu.models;
using BE.vn.fpt.edu.DTOs.VehicleCheckin;

namespace BE.UnitTests.Helpers
{
    /// <summary>
    /// Helper class để tạo mock data cho Unit Tests
    /// </summary>
    public static class MockDataBuilder
    {
        /// <summary>
        /// Tạo mock VehicleCheckinRequestDto
        /// </summary>
        public static VehicleCheckinRequestDto CreateVehicleCheckinRequestDto(
            long carId = 1,
            long branchId = 1,
            int mileage = 10000,
            string? notes = "Test notes",
            List<string>? imageUrls = null)
        {
            return new VehicleCheckinRequestDto
            {
                CarId = carId,
                BranchId = branchId,
                Mileage = mileage,
                Notes = notes,
                ImageUrls = imageUrls ?? new List<string> { "https://example.com/image1.jpg" },
                MaintenanceRequestId = null
            };
        }

        /// <summary>
        /// Tạo mock VehicleCheckin entity
        /// </summary>
        public static VehicleCheckin CreateVehicleCheckin(
            long id = 1,
            long carId = 1,
            long branchId = 1,
            int mileage = 10000,
            string? code = "VCI-12345",
            string statusCode = "PENDING")
        {
            return new VehicleCheckin
            {
                Id = id,
                CarId = carId,
                BranchId = branchId,
                Mileage = mileage,
                Code = code,
                StatusCode = statusCode,
                Notes = "Test notes",
                CreatedAt = DateTime.UtcNow,
                VehicleCheckinImages = new List<VehicleCheckinImage>
                {
                    new VehicleCheckinImage
                    {
                        Id = 1,
                        VehicleCheckinId = id,
                        ImageUrl = "https://example.com/image1.jpg",
                        CreatedAt = DateTime.UtcNow
                    }
                }
            };
        }

        /// <summary>
        /// Tạo mock Car entity
        /// </summary>
        public static Car CreateCar(
            long id = 1,
            string carName = "Test Car",
            string licensePlate = "51A-12345",
            string vinNumber = "VIN123456789",
            long userId = 1,
            long branchId = 1)
        {
            return new Car
            {
                Id = id,
                CarName = carName,
                LicensePlate = licensePlate,
                VinNumber = vinNumber,
                CarModel = "Toyota Camry",
                YearOfManufacture = 2020,
                Color = "White",
                UserId = userId,
                BranchId = branchId,
                User = new User
                {
                    Id = userId,
                    Username = "testuser",
                    Password = "TestPassword123",
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@test.com",
                    Phone = "0987654321"
                },
                Branch = new Branch
                {
                    Id = branchId,
                    Name = "Test Branch",
                    Address = "123 Test Street",
                    Phone = "0123456789"
                }
            };
        }

        /// <summary>
        /// Tạo mock Branch entity
        /// </summary>
        public static Branch CreateBranch(long id = 1, string name = "Test Branch", string? address = "123 Test Street")
        {
            return new Branch
            {
                Id = id,
                Name = name,
                Address = address,
                Phone = "0123456789"
            };
        }

        /// <summary>
        /// Tạo mock User entity
        /// </summary>
        public static User CreateUser(
            long id = 1,
            string firstName = "Test",
            string lastName = "User",
            string email = "test@test.com",
            string phone = "0987654321",
            string username = "testuser",
            string password = "TestPassword123")
        {
            return new User
            {
                Id = id,
                Username = username,
                Password = password,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone
            };
        }
    }
}

