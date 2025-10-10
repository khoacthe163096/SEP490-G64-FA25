# Vehicle Check-in API Documentation

## Tổng quan
API này cung cấp các chức năng quản lý check-in xe của khách hàng, bao gồm tạo mới, cập nhật, xem chi tiết và xóa.

## Base URL
```
http://localhost:5000/api/vehiclecheckin
```

## Endpoints

### 1. Tạo mới Vehicle Check-in
**POST** `/api/vehiclecheckin`

Tạo mới một bản ghi check-in xe với thông tin và hình ảnh.

**Request Body:**
```json
{
  "carId": 1,
  "maintenanceRequestId": 1,
  "mileage": 50000,
  "notes": "Xe có tiếng động lạ ở động cơ",
  "imageUrls": [
    "https://example.com/image1.jpg",ac
    "https://example.com/image2.jpg",
    "https://example.com/image3.jpg"
  ]
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "carId": 1,
    "maintenanceRequestId": 1,
    "mileage": 50000,
    "notes": "Xe có tiếng động lạ ở động cơ",
    "createdAt": "2024-01-15T10:30:00Z",
    "carName": "Toyota Camry",
    "carModel": "2020",
    "licensePlate": "30A-12345",
    "color": "Trắng",
    "yearOfManufacture": 2020,
    "customerName": "Nguyễn Văn A",
    "customerPhone": "0123456789",
    "customerEmail": "nguyenvana@email.com",
    "images": [
      {
        "id": 1,
        "imageUrl": "https://example.com/image1.jpg",
        "createdAt": "2024-01-15T10:30:00Z"
      }
    ],
    "maintenanceRequestStatus": "PENDING",
    "requestDate": "2024-01-15T09:00:00Z"
  },
  "message": "Vehicle check-in created successfully"
}
```

### 2. Cập nhật Vehicle Check-in
**PUT** `/api/vehiclecheckin/{id}`

Cập nhật thông tin check-in xe.

**Request Body:**
```json
{
  "id": 1,
  "mileage": 51000,
  "notes": "Đã kiểm tra và phát hiện vấn đề ở bugi",
  "imageUrls": [
    "https://example.com/updated_image1.jpg",
    "https://example.com/updated_image2.jpg"
  ]
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "carId": 1,
    "maintenanceRequestId": 1,
    "mileage": 51000,
    "notes": "Đã kiểm tra và phát hiện vấn đề ở bugi",
    "createdAt": "2024-01-15T10:30:00Z",
    "carName": "Toyota Camry",
    "carModel": "2020",
    "licensePlate": "30A-12345",
    "color": "Trắng",
    "yearOfManufacture": 2020,
    "customerName": "Nguyễn Văn A",
    "customerPhone": "0123456789",
    "customerEmail": "nguyenvana@email.com",
    "images": [
      {
        "id": 2,
        "imageUrl": "https://example.com/updated_image1.jpg",
        "createdAt": "2024-01-15T11:00:00Z"
      }
    ],
    "maintenanceRequestStatus": "PENDING",
    "requestDate": "2024-01-15T09:00:00Z"
  },
  "message": "Vehicle check-in updated successfully"
}
```

### 3. Lấy chi tiết Vehicle Check-in
**GET** `/api/vehiclecheckin/{id}`

Lấy thông tin chi tiết của một check-in xe.

**Response:**
```json
{
  "success": true,
  "data": {
    "id": 1,
    "carId": 1,
    "maintenanceRequestId": 1,
    "mileage": 50000,
    "notes": "Xe có tiếng động lạ ở động cơ",
    "createdAt": "2024-01-15T10:30:00Z",
    "carName": "Toyota Camry",
    "carModel": "2020",
    "licensePlate": "30A-12345",
    "color": "Trắng",
    "yearOfManufacture": 2020,
    "customerName": "Nguyễn Văn A",
    "customerPhone": "0123456789",
    "customerEmail": "nguyenvana@email.com",
    "images": [
      {
        "id": 1,
        "imageUrl": "https://example.com/image1.jpg",
        "createdAt": "2024-01-15T10:30:00Z"
      }
    ],
    "maintenanceRequestStatus": "PENDING",
    "requestDate": "2024-01-15T09:00:00Z"
  }
}
```

### 4. Lấy danh sách Vehicle Check-in
**GET** `/api/vehiclecheckin?page=1&pageSize=10`

Lấy danh sách tất cả check-in xe với phân trang.

**Query Parameters:**
- `page` (optional): Số trang (mặc định: 1)
- `pageSize` (optional): Số lượng item mỗi trang (mặc định: 10)

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "carId": 1,
      "carName": "Toyota Camry",
      "licensePlate": "30A-12345",
      "customerName": "Nguyễn Văn A",
      "mileage": 50000,
      "createdAt": "2024-01-15T10:30:00Z",
      "notes": "Xe có tiếng động lạ ở động cơ",
      "firstImageUrl": "https://example.com/image1.jpg"
    }
  ],
  "page": 1,
  "pageSize": 10
}
```

### 5. Lấy Vehicle Check-in theo Car ID
**GET** `/api/vehiclecheckin/car/{carId}`

Lấy danh sách check-in của một xe cụ thể.

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "carId": 1,
      "carName": "Toyota Camry",
      "licensePlate": "30A-12345",
      "customerName": "Nguyễn Văn A",
      "mileage": 50000,
      "createdAt": "2024-01-15T10:30:00Z",
      "notes": "Xe có tiếng động lạ ở động cơ",
      "firstImageUrl": "https://example.com/image1.jpg"
    }
  ]
}
```

### 6. Lấy Vehicle Check-in theo Maintenance Request ID
**GET** `/api/vehiclecheckin/maintenance-request/{maintenanceRequestId}`

Lấy danh sách check-in của một yêu cầu bảo dưỡng cụ thể.

**Response:**
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "carId": 1,
      "carName": "Toyota Camry",
      "licensePlate": "30A-12345",
      "customerName": "Nguyễn Văn A",
      "mileage": 50000,
      "createdAt": "2024-01-15T10:30:00Z",
      "notes": "Xe có tiếng động lạ ở động cơ",
      "firstImageUrl": "https://example.com/image1.jpg"
    }
  ]
}
```

### 7. Xóa Vehicle Check-in
**DELETE** `/api/vehiclecheckin/{id}`

Xóa một check-in xe.

**Response:**
```json
{
  "success": true,
  "message": "Vehicle check-in deleted successfully"
}
```

## Error Responses

### 400 Bad Request
```json
{
  "success": false,
  "message": "Validation error message"
}
```

### 404 Not Found
```json
{
  "success": false,
  "message": "Vehicle check-in not found"
}
```

### 500 Internal Server Error
```json
{
  "success": false,
  "message": "Internal server error",
  "error": "Detailed error message"
}
```

## Validation Rules

### VehicleCheckinRequestDTO  
- `carId`: Required, must be a valid car ID
- `maintenanceRequestId`: Required, must be a valid maintenance request ID
- `mileage`: Required, must be a positive number
- `notes`: Optional, maximum 1000 characters
- `imageUrls`: Required, must contain at least one image URL

### VehicleCheckinUpdateDTO
- `id`: Required, must match the URL parameter
- `mileage`: Required, must be a positive number
- `notes`: Optional, maximum 1000 characters
- `imageUrls`: Optional, if provided will replace existing images

## Notes
- Tất cả thời gian được trả về theo định dạng ISO 8601 UTC
- Hình ảnh được lưu trữ dưới dạng URL, cần upload trước khi gọi API
- API hỗ trợ phân trang cho danh sách check-in
- Mỗi check-in có thể có nhiều hình ảnh
