# 📚 TodoApp - Clean Architecture with CQRS

> ASP.NET Core 8.0 Web API với Clean Architecture, DDD, CQRS Pattern và JWT Authentication

---

## 🏗️ **KIẾN TRÚC PROJECT**

### **Clean Architecture Layers**

```
┌─────────────────────────────────────────────────────┐
│  Presentation Layer (TodoApp.WebAPI)                │
│  - Controllers                                      │
│  - Filters (GlobalExceptionFilter)                  │
│  - Program.cs (DI Configuration)                    │
└──────────────────┬──────────────────────────────────┘
                   │ depends on
┌──────────────────▼──────────────────────────────────┐
│  Application Layer (TodoApp.Application)            │
│  - Features (CQRS: Commands/Queries)                │
│  - DTOs, Validators, Behaviors                      │
│  - Repository Interfaces                            │
│  - Common (Result Pattern, ErrorType)               │
└──────────────────┬──────────────────────────────────┘
                   │ depends on
┌──────────────────▼──────────────────────────────────┐
│  Domain Layer (TodoApp.Domain)                      │
│  - Entities (Book, User, Genre, BookGenre)          │
│  - Domain Logic (Factory Methods, Business Rules)   │
│  - No Dependencies!                                 │
└─────────────────────────────────────────────────────┘
         ▲
         │ implements
┌────────┴─────────────────────────────────────────────┐
│  Infrastructure Layer (TodoApp.Infrastructure)       │
│  - DbContext (EF Core)                               │
│  - Repository Implementations                        │
│  - External Services (JwtService)                    │
└──────────────────────────────────────────────────────┘
```

---

## 🛠️ **TECH STACK**

| Công nghệ | Version | Mục đích |
|-----------|---------|----------|
| **.NET** | 8.0 | Framework chính |
| **EF Core** | 8.0 | ORM - Database access |
| **MySQL** | 8.0.29 | Database |
| **MediatR** | Latest | CQRS Pattern |
| **FluentValidation** | Latest | Input validation |
| **JWT Bearer** | 8.0.22 | Authentication |
| **BCrypt.Net** | Latest | Password hashing |

---

## 📦 **PACKAGE INSTALLATION**

### **TodoApp.WebAPI**
```bash
# JWT Authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.22
dotnet add package System.IdentityModel.Tokens.Jwt

# Entity Framework
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.3
```

### **TodoApp.Application**
```bash
# CQRS & Validation
dotnet add package MediatR
dotnet add package FluentValidation
dotnet add package FluentValidation.AspNetCore

# Password Hashing
dotnet add package BCrypt.Net-Next
```

### **TodoApp.Infrastructure**
```bash
# Database
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.0.2
```

---

## 🗄️ **DATABASE MIGRATIONS**

> **Lưu ý:** Chạy tất cả commands trong thư mục `TodoApp.Infrastructure`

### **Tạo Migration mới**
```bash
dotnet ef migrations add <MigrationName> --startup-project ..\TodoApp.WebAPI
```

### **Áp dụng Migration vào Database**
```bash
dotnet ef database update --startup-project ..\TodoApp.WebAPI
```

### **Xóa Database (Cẩn thận!)**
```bash
# Xóa database
dotnet ef database drop --startup-project ..\TodoApp.WebAPI

# Xóa thư mục Migrations (nếu cần reset)
Remove-Item -Path "Migrations" -Recurse -Force
# Hoặc (Linux/Mac):
rm -r Migrations
```

### **Lịch sử Migrations đã tạo**
```bash
# Initial setup
dotnet ef migrations add InitCreate --startup-project ..\TodoApp.WebAPI

# Book & Genre
dotnet ef migrations add CreateBookGenreTable --startup-project ..\TodoApp.WebAPI
dotnet ef migrations add FixBook --startup-project ..\TodoApp.WebAPI
dotnet ef migrations add FixConfigUrlImage --startup-project ..\TodoApp.WebAPI
dotnet ef migrations add FixConfigv2 --startup-project ..\TodoApp.WebAPI

# User & Authentication
dotnet ef migrations add DeleteIdUser --startup-project ..\TodoApp.WebAPI
dotnet ef migrations add JwtProperty --startup-project ..\TodoApp.WebAPI
dotnet ef migrations add addColumnRole --startup-project ..\TodoApp.WebAPI
```

### **Tips: Xử lý lỗi Migration**
Nếu migration báo lỗi bảng đã tồn tại nhưng muốn giữ lại dữ liệu:
1. Mở file migration vừa tạo (trong `Migrations/`)
2. Xóa các câu lệnh `CreateTable` của bảng đã tồn tại
3. Chạy `dotnet ef database update --startup-project ..\TodoApp.WebAPI`

---

## 🎯 **KIẾN TRÚC SO SÁNH**

### **Kiến trúc cũ (Service Layer) ❌**
```
Controller → Service → Repository → Database
```

### **Kiến trúc mới (CQRS với MediatR) ✅**
```
Controller → MediatR → Handler → Repository → Database
```

### **Bảng so sánh chi tiết**

| Thành phần | Service Pattern (Cũ) | CQRS Pattern (Mới) |
|------------|---------------------|-------------------|
| **Business Logic** | `BookService` | `CreateBookHandler`, `GetBookByIdHandler` |
| **Validation** | Trong Service | `CreateBookCommandValidator` (FluentValidation) |
| **Dependency Injection** | `IBookService` | `IMediator` |
| **Controller gọi** | `_bookService.CreateAsync()` | `_mediator.Send(command)` |
| **Tách biệt Read/Write** | ❌ Không | ✅ Có (Command/Query) |

### **Kết luận Migration**

✅ **XÓA ĐƯỢC** (không dùng nữa):
- `BookService.cs`
- `GenreService.cs`
- `IBookService.cs`
- `IGenreService.cs`

✅ **GIỮ LẠI** (vẫn cần):
- `BookRepository` (interface)
- `BookRepositoryImpl` (implementation)
- `CreateBookHandler`, `GetBookByIdHandler`, ... (handlers)
- `CreateBookCommand`, `GetBookByIdQuery`, ... (CQRS)

---

## 🔄 **REQUEST FLOW - HAPPY PATH**

### **Scenario: Tạo Book thành công**

```
📱 CLIENT GỬI REQUEST
    ↓
    POST /api/books
    Body: {
      "nameBook": "Harry Potter",
      "author": "J.K. Rowling",
      "description": "Magic book",
      "listPrice": 350000,
      "quantity": 100
    }
    ↓
┌───────────────────────────────────────────────────────┐
│ 1️⃣ ASP.NET CORE PIPELINE                             │
│    BookController.CreateBook()                        │
│    ↓                                                  │
│    var result = await _mediator.Send(command);       │
└──────────────────┬────────────────────────────────────┘
                   ↓
┌───────────────────────────────────────────────────────┐
│ 2️⃣ MEDIATR PIPELINE                                   │
│                                                        │
│    ValidationBehavior (Automatic)                     │
│    ├─ Tìm IValidator<CreateBookCommand>              │
│    │  → CreateBookCommandValidator                    │
│    │                                                  │
│    ├─ Chạy validation rules:                          │
│    │  ✅ NameBook: NotEmpty, MaxLength(200)          │
│    │  ✅ Author: NotEmpty, MaxLength(100)            │
│    │  ✅ Description: NotEmpty                        │
│    │  ✅ ListPrice: >= 0                              │
│    │  ✅ Quantity: >= 0                               │
│    │                                                  │
│    └─ PASS → Tiếp tục đến Handler                   │
└──────────────────┬────────────────────────────────────┘
                   ↓
┌───────────────────────────────────────────────────────┐
│ 3️⃣ HANDLER: CreateBookHandler.Handle()               │
│                                                        │
│    ├─ Kiểm tra tên sách trùng (Business logic)       │
│    ├─ Book.Create(...) → Domain Factory Method       │
│    ├─ _bookRepository.AddBookAsync(book)             │
│    └─ return Result<BookResponseDTO>.Success(...)    │
└──────────────────┬────────────────────────────────────┘
                   ↓
┌───────────────────────────────────────────────────────┐
│ 4️⃣ CONTROLLER RESPONSE                                │
│                                                        │
│    return CreatedAtAction(...)                        │
│    Status: 201 Created                                │
│    Body: {                                            │
│      "message": "Tạo sách thành công",               │
│      "data": {                                        │
│        "idBook": 123,                                 │
│        "nameBook": "Harry Potter",                    │
│        "author": "J.K. Rowling",                      │
│        ...                                            │
│      }                                                │
│    }                                                  │
└───────────────────────────────────────────────────────┘
```

---

## ❌ **REQUEST FLOW - VALIDATION ERROR**

### **Scenario: Dữ liệu không hợp lệ**

```
📱 CLIENT GỬI REQUEST (DỮ LIỆU SAI)
    ↓
    POST /api/books
    Body: {
      "nameBook": "",  ❌ Empty
      "author": "Very long author name exceeding 100 characters...",  ❌
      ...
    }
    ↓
1️⃣ BookController.CreateBook()
    ↓
    _mediator.Send(command)
    ↓
┌───────────────────────────────────────────────────────┐
│ 2️⃣ ValidationBehavior                                 │
│                                                        │
│    Chạy CreateBookCommandValidator                    │
│    ├─ NameBook: FAIL - "NameBook is required"        │
│    ├─ Author: FAIL - "Must not exceed 100 chars"     │
│    │                                                  │
│    └─ throw ValidationException(failures) 🔥         │
└──────────────────┬────────────────────────────────────┘
                   ↓
┌───────────────────────────────────────────────────────┐
│ 3️⃣ GlobalExceptionFilter (BẮT LỖI)                   │
│                                                        │
│    if (exception is ValidationException)              │
│    ├─ Chuyển errors thành Dictionary                 │
│    ├─ Format response chuẩn                           │
│    └─ return BadRequest(...)                         │
└──────────────────┬────────────────────────────────────┘
                   ↓
📱 CLIENT NHẬN RESPONSE
    Status: 400 Bad Request
    Body: {
      "status": 400,
      "errorCode": "VALIDATION_ERROR",
      "message": "Dữ liệu không hợp lệ",
      "errors": {
        "NameBook": ["NameBook is required."],
        "Author": ["Author must not exceed 100 characters."]
      },
      "traceId": "0HNIHGU1PQJ8H:00000002"
    }
```

---

## 🔐 **JWT AUTHENTICATION FLOW**

```
1️⃣ User Login:
   POST /api/auth/login
   { "email": "user@example.com", "password": "Pass123" }
   ↓
   Response: {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "refreshToken": "BtZvbjcLpyZq+zZ1TNb3uprtthghrT9k...",
     "expiresAt": "2026-01-12T15:30:00Z"
   }

2️⃣ Access Protected Endpoint:
   GET /api/books
   Headers: {
     "Authorization": "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
   }
   ↓
   ASP.NET Core Middleware validates JWT
   ↓
   ✅ Valid → Allow access
   ❌ Invalid/Expired → 401 Unauthorized

3️⃣ Refresh Token (khi JWT hết hạn):
   POST /api/auth/refresh
   { "refreshToken": "BtZvbjcLpyZq+zZ1TNb3uprtthghrT9k..." }
   ↓
   Response: New JWT + New RefreshToken
```

---

## 📝 **VALIDATION LIBRARIES**

| Thư viện | Mục đích | Bắt buộc? |
|----------|----------|-----------|
| **FluentValidation** | Viết validation rules | ✅ BẮT BUỘC |
| **FluentValidation.AspNetCore** | Tích hợp với ASP.NET Core | ❌ Optional (chỉ khi cần auto-validation) |

**Lưu ý:** Project này dùng Manual Validation qua `ValidationBehavior`, không cần `FluentValidation.AspNetCore`.

---

## 🚀 **CHẠY PROJECT**

### **1. Cấu hình Database**
Chỉnh `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=mysqlart;user=root;password=yourpassword;"
  }
}
```

### **2. Chạy Migrations**
```bash
cd TodoApp.Infrastructure
dotnet ef database update --startup-project ..\TodoApp.WebAPI
```

### **3. Chạy Application**
```bash
cd TodoApp.WebAPI
dotnet run
```

### **4. Test API**
- Swagger UI: `https://localhost:7xxx/swagger`
- API Base URL: `https://localhost:7xxx/api`

---

## 📚 **API ENDPOINTS**

### **Auth**
- `POST /api/auth/register` - Đăng ký
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/refresh` - Refresh token

### **Books**
- `GET /api/books` - Lấy danh sách (filter, pagination)
- `GET /api/books/{id}` - Lấy chi tiết
- `POST /api/books` - Tạo mới (Admin only)
- `PUT /api/books/{id}` - Cập nhật (Admin only)
- `DELETE /api/books/{id}` - Xóa (Admin only)

---

## 👨‍💻 **DEVELOPMENT NOTES**

- **Result Pattern**: Tất cả handlers trả về `Result<T>` thay vì throw exceptions
- **GlobalExceptionFilter**: Xử lý tập trung ValidationException, BusinessLogicException, UnauthorizedException
- **Domain-Driven Design**: Entity có private constructor, chỉ tạo qua Factory Method
- **CQRS**: Tách biệt Command (write) và Query (read)
- **DTO Mapping**: Query trả về DTO, Command trả về DTO sau khi tạo/update

---

Made with ❤️ using Clean Architecture + DDD + CQRS