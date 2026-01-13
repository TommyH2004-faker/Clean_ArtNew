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
dotnet ef migrations add AddTimeGenres --startup-project ..\TodoApp.WebAPI
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

## 🎓 **CÁC CẤP ĐỘ KIẾN TRÚC - ARCHITECTURE MATURITY LEVELS**

### **📊 Tổng quan các Level**

| Level | Tên | Mô tả | Status trong Project |
|-------|-----|-------|---------------------|
| **Level 1** | Basic CRUD | Controller → Service → Repository | ❌ Đã nâng cấp |
| **Level 2** | Clean Architecture | 4 layers phân tầng rõ ràng | ✅ Hoàn thành 100% |
| **Level 3** | CQRS Pattern | Tách biệt Command/Query với MediatR | ✅ Hoàn thành 100% |
| **Level 4** | DDD Tactical Patterns | Encapsulation, Factory Methods, Aggregates | ✅ Hoàn thành 100% |
| **Level 5** | Event-Driven Architecture | Domain Events + Event Handlers | ✅ Hoàn thành cho Genre |

---

## 🏆 **LEVEL 4: DOMAIN-DRIVEN DESIGN (DDD) - HOÀN THÀNH**

### **✨ Các Pattern đã áp dụng cho Genre Entity**

#### **1. Encapsulation (Đóng gói)**
```csharp
public class Genre {
    // ❌ TRƯỚC: Public setters - ai cũng sửa được
    public int IdGenre { get; set; }
    public string NameGenre { get; set; }
    
    // ✅ SAU: Private setters - chỉ Domain kiểm soát
    public int IdGenre { get; private set; }
    public string NameGenre { get; private set; }
}
```

#### **2. Factory Methods (Phương thức tạo)**
```csharp
// ❌ TRƯỚC: Tạo trực tiếp với new
var genre = new Genre { NameGenre = "Sci-Fi" };

// ✅ SAU: Tạo qua Factory Method
var genre = Genre.Create("Sci-Fi");  // Business logic bên trong
```

#### **3. Domain Methods (Phương thức nghiệp vụ)**
```csharp
// ❌ TRƯỚC: Logic nằm trong Service/Handler
genre.NameGenre = newName;  // Không kiểm tra gì

// ✅ SAU: Logic nằm trong Entity
genre.Update(newName);  // Entity tự validate, tự quản lý trạng thái
```

#### **4. Business Rules Validation**
```csharp
public void MarkForDeletion() {
    if (_bookGenres.Any()) {
        throw new InvalidOperationException(
            $"Cannot delete Genre '{NameGenre}'. It has {_bookGenres.Count} books."
        );
    }
    AddDomainEvent(new GenreEvents.GenreDeleted(IdGenre, NameGenre));
}
```

#### **5. Aggregate Root Pattern**
```csharp
// Genre là Aggregate Root, quản lý BookGenre
public IReadOnlyCollection<BookGenre> BookGenres => _bookGenres.AsReadOnly();

public void AddBookGenre(int bookId) {
    if (_bookGenres.Any(bg => bg.BookId == bookId)) {
        throw new InvalidOperationException("Book already in this genre");
    }
    _bookGenres.Add(new BookGenre(bookId, IdGenre));
}
```

---

## 🚀 **LEVEL 5: EVENT-DRIVEN ARCHITECTURE - MỚI HOÀN THÀNH**

### **🎯 Tại sao cần Domain Events?**

**Vấn đề:** Khi Genre được tạo/sửa/xóa, cần thực hiện nhiều side effects:
- ✅ Ghi log để audit
- ✅ Clear cache
- ✅ Gửi notification
- ✅ Sync với external systems

**Giải pháp cũ (Level 4):**
```csharp
// ❌ Handler phải biết tất cả side effects
public async Task<Result<GenreResponseDTO>> Handle(...) {
    var genre = Genre.Create(request.NameGenre);
    await _repository.AddGenreAsync(genre);
    
    // Phải gọi thủ công - dễ quên
    _logger.LogInformation("Genre created");
    await _cacheService.ClearCache();
    await _notificationService.Send();
}
```

**Giải pháp mới (Level 5 - Event-Driven):**
```csharp
// ✅ Handler chỉ lo business logic
public async Task<Result<GenreResponseDTO>> Handle(...) {
    var genre = Genre.Create(request.NameGenre);  // Tự động raise Event!
    await _repository.AddGenreAsync(genre);
    // Event handlers tự động xử lý side effects
}
```

---

### **🏗️ Kiến trúc Domain Events**

#### **📁 Cấu trúc thư mục**

```
TodoApp.Domain/
├── Common/
│   ├── IDomainEvent.cs            ← Interface không dependency
│   ├── IHasDomainEvents.cs        ← Interface cho Aggregate Root
│   └── DomainEventBase.cs         ← Base record cho events
├── Events/
│   └── GenreEvents.cs             ← Pure domain events (POCO)
└── Entities/
    └── Genre.cs                   ← Aggregate Root + IHasDomainEvents

TodoApp.Application/
├── Events/
│   ├── GenreCreatedEvent.cs       ← MediatR INotification wrapper
│   ├── GenreUpdatedEvent.cs       ← MediatR INotification wrapper
│   └── GenreDeletedEvent.cs       ← MediatR INotification wrapper
└── Features/GenreHandle/
    └── EventHandlers/
        ├── GenreCreatedEventHandler.cs
        ├── GenreUpdatedEventHandler.cs
        └── GenreDeletedEventHandler.cs

TodoApp.Infrastructure/
└── Persistence/
    └── TodoAppDbContext.cs        ← Dispatch events sau SaveChanges
```

---

### **⚙️ Luồng hoạt động Event-Driven**

```
┌──────────────────────────────────────────────────────────────┐
│ 1️⃣ DOMAIN LAYER - Raise Event                                │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  public static Genre Create(string nameGenre) {             │
│      var genre = new Genre(nameGenre);                      │
│      genre.AddDomainEvent(                                  │
│          new GenreEvents.GenreCreated(genre.IdGenre,        │
│                                       genre.NameGenre)      │
│      );  ← Event được thêm vào _domainEvents collection     │
│      return genre;                                          │
│  }                                                           │
└──────────────────┬───────────────────────────────────────────┘
                   ↓
┌──────────────────────────────────────────────────────────────┐
│ 2️⃣ HANDLER - Lưu Entity                                      │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  var genre = Genre.Create(request.NameGenre);               │
│  await _repository.AddGenreAsync(genre);                    │
│  // Genre có _domainEvents = [GenreCreated]                 │
└──────────────────┬───────────────────────────────────────────┘
                   ↓
┌──────────────────────────────────────────────────────────────┐
│ 3️⃣ DBCONTEXT - SaveChangesAsync() Override                   │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  public override async Task<int> SaveChangesAsync(...) {    │
│      // Trích xuất events từ tracked entities               │
│      var entities = ChangeTracker.Entries<IHasDomainEvents>()│
│                     .Where(e => e.Entity.DomainEvents.Any)  │
│                     .Select(e => e.Entity).ToList();        │
│                                                              │
│      var events = entities.SelectMany(e => e.DomainEvents)  │
│                           .ToList();                         │
│                                                              │
│      // Clear events trước khi save                          │
│      foreach (var entity in entities) {                     │
│          entity.ClearDomainEvents();                        │
│      }                                                       │
│                                                              │
│      // ⚠️ CRITICAL: Save TRƯỚC khi dispatch events          │
│      var result = await base.SaveChangesAsync(cancel...);  │
│                                                              │
│      // Convert Domain Events → MediatR Notifications        │
│      foreach (var domainEvent in events) {                  │
│          var notification = domainEvent switch {            │
│              GenreEvents.GenreCreated e =>                  │
│                  new GenreCreatedEvent(e),                  │
│              GenreEvents.GenreUpdated e =>                  │
│                  new GenreUpdatedEvent(e),                  │
│              GenreEvents.GenreDeleted e =>                  │
│                  new GenreDeletedEvent(e),                  │
│              _ => null                                      │
│          };                                                  │
│          if (notification != null) {                        │
│              await _mediator.Publish(notification, ...);    │
│          }                                                   │
│      }                                                       │
│      return result;                                          │
│  }                                                           │
└──────────────────┬───────────────────────────────────────────┘
                   ↓
┌──────────────────────────────────────────────────────────────┐
│ 4️⃣ EVENT HANDLERS - Xử lý Side Effects                       │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  GenreCreatedEventHandler:                                  │
│  ├─ ✅ Ghi log: "Genre created: Sci-Fi"                     │
│  ├─ ✅ Clear cache: InvalidateCache("genres")              │
│  └─ ✅ Gửi notification: NotifyAdmins()                     │
│                                                              │
│  GenreUpdatedEventHandler:                                  │
│  ├─ 🔄 Ghi log: "Genre updated: Fantasy → Sci-Fi"          │
│  └─ 🔄 Sync search index                                    │
│                                                              │
│  GenreDeletedEventHandler:                                  │
│  ├─ ❌ Ghi log: "Genre deleted: Horror"                     │
│  └─ ❌ Archive data                                         │
└──────────────────────────────────────────────────────────────┘
```

---

### **🔑 Nguyên tắc Clean Architecture - ZERO Dependencies**

#### **❌ VẤN ĐỀ BAN ĐẦU:**
```csharp
// TodoApp.Domain/Events/GenreCreatedEvent.cs
using MediatR;  // ❌ DOMAIN phụ thuộc Infrastructure!

public class GenreCreatedEvent : INotification {
    public int GenreId { get; }
    public string GenreName { get; }
}
```

**Tại sao sai?**
- Domain Layer không được phụ thuộc vào bất kỳ thư viện nào (MediatR, EF Core, etc.)
- Vi phạm nguyên tắc Dependency Inversion (DIP)
- Domain phải là "lõi sạch" chỉ chứa business logic

#### **✅ GIẢI PHÁP ĐÚNG:**

**1. Domain Layer - Pure POCOs (No dependencies)**
```csharp
// TodoApp.Domain/Common/IDomainEvent.cs
public interface IDomainEvent {
    DateTime OccurredOn { get; }
}

// TodoApp.Domain/Common/DomainEventBase.cs
public abstract record DomainEventBase : IDomainEvent {
    public DateTime OccurredOn { get; init; }
    protected DomainEventBase() => OccurredOn = DateTime.UtcNow;
}

// TodoApp.Domain/Events/GenreEvents.cs
public static class GenreEvents {
    public record GenreCreated : DomainEventBase {
        public int GenreId { get; init; }
        public string GenreName { get; init; }
        
        public GenreCreated(int genreId, string genreName) {
            GenreId = genreId;
            GenreName = genreName;
        }
    }
}
```

**2. Application Layer - MediatR Wrappers**
```csharp
// TodoApp.Application/Events/GenreCreatedEvent.cs
using MediatR;  // ✅ Application có thể dùng MediatR
using TodoApp.Domain.Events;

public class GenreCreatedEvent : INotification {
    public GenreEvents.GenreCreated DomainEvent { get; }
    
    public int GenreId => DomainEvent.GenreId;
    public string GenreName => DomainEvent.GenreName;
    
    public GenreCreatedEvent(GenreEvents.GenreCreated domainEvent) {
        DomainEvent = domainEvent;
    }
}
```

**3. Infrastructure Layer - Pattern Matching Conversion**
```csharp
// TodoApp.Infrastructure/Persistence/TodoAppDbContext.cs
var notification = domainEvent switch {
    GenreEvents.GenreCreated e => new GenreCreatedEvent(e),
    GenreEvents.GenreUpdated e => new GenreUpdatedEvent(e),
    GenreEvents.GenreDeleted e => new GenreDeletedEvent(e),
    _ => null
};
await _mediator.Publish(notification, cancellationToken);
```

---

### **📊 Dependency Graph - Đúng chuẩn Clean Architecture**

```
┌─────────────────────────────────────────────────────────┐
│ TodoApp.Domain (CORE - No Dependencies)                 │
│ ├── IDomainEvent.cs           (interface)               │
│ ├── DomainEventBase.cs        (abstract record)         │
│ └── GenreEvents.cs            (pure POCOs)              │
└─────────────────────────────────────────────────────────┘
            ▲                           ▲
            │                           │
            │ uses                      │ uses
            │                           │
┌───────────┴──────────┐   ┌────────────┴──────────────┐
│ TodoApp.Application  │   │ TodoApp.Infrastructure    │
│ ├── GenreCreatedEvent│   │ ├── TodoAppDbContext      │
│ │   (INotification)  │   │ │   (Pattern Matching)    │
│ └── GenreCreatedEvent│   │ └── Dispatch via MediatR  │
│     Handler          │   │                            │
└──────────────────────┘   └────────────────────────────┘
        uses MediatR              uses MediatR
```

**✅ Kết quả:**
- Domain có ZERO dependencies
- Application và Infrastructure tùy ý dùng MediatR
- Dependency chảy từ ngoài vào trong (Clean Architecture đúng chuẩn)

---

### **🐛 Debug Journey - Các lỗi đã fix**

#### **1. CS8864: Records can only inherit from another record**
```csharp
// ❌ SAI
public abstract class DomainEventBase : IDomainEvent { }
public record GenreCreated(...) : DomainEventBase { }  // ERROR!

// ✅ ĐÚNG
public abstract record DomainEventBase : IDomainEvent { }
public record GenreCreated(...) : DomainEventBase { }  // OK!
```

#### **2. CS0272: Property setter inaccessibility**
```csharp
// ❌ SAI
new BookGenre { BookId = 1, GenreId = 2 }  // Private setters!

// ✅ ĐÚNG
new BookGenre(bookId: 1, genreId: 2)  // Public constructor
```

#### **3. CS0311: Type cannot be used as type parameter**
```csharp
// ❌ SAI - Quên implement IRequest
public record CreateGenreCommand { }

// ✅ ĐÚNG
public record CreateGenreCommand : IRequest<Result<GenreResponseDTO>> { }
```

---

## 📈 **SO SÁNH TRƯỚC/SAU EVENT-DRIVEN**

### **Scenario: Tạo Genre mới**

#### **❌ TRƯỚC (Level 4 - Không có Events)**

```csharp
// CreateGenreCommandHandler.cs
public async Task<Result<GenreResponseDTO>> Handle(...) {
    var genre = Genre.Create(request.NameGenre);
    await _repository.AddGenreAsync(genre);
    
    // ❌ Phải gọi thủ công tất cả side effects
    _logger.LogInformation("✅ Genre created");
    await _cacheService.InvalidateCache("genres");
    await _notificationService.NotifyAdmins("New genre created");
    await _searchIndexService.AddToIndex(genre);
    
    // ❌ Nếu cần thêm side effect mới → sửa Handler
    // ❌ Tight coupling giữa business logic và side effects
    // ❌ Khó test (phải mock nhiều services)
    
    return Result<GenreResponseDTO>.Success(...);
}
```

**Vấn đề:**
1. Handler biết quá nhiều chi tiết (logging, cache, notification...)
2. Vi phạm Single Responsibility Principle (SRP)
3. Thêm side effect mới → phải sửa Handler → rủi ro cao
4. Khó test vì phải mock nhiều dependencies

---

#### **✅ SAU (Level 5 - Event-Driven)**

**Handler (Business Logic Only):**
```csharp
// CreateGenreCommandHandler.cs
public async Task<Result<GenreResponseDTO>> Handle(...) {
    var genre = Genre.Create(request.NameGenre);  // ← Event được raise ở đây!
    await _repository.AddGenreAsync(genre);
    // Side effects tự động chạy qua Event Handlers
    return Result<GenreResponseDTO>.Success(...);
}
```

**Event Handlers (Tách biệt từng concern):**
```csharp
// GenreCreatedEventHandler.cs - Logging
public class GenreCreatedEventHandler : INotificationHandler<GenreCreatedEvent> {
    public async Task Handle(GenreCreatedEvent @event, CancellationToken cancel) {
        _logger.LogInformation($"✅ Genre created: {@event.GenreName}");
    }
}

// GenreCacheInvalidationHandler.cs - Caching (Tách riêng!)
public class GenreCacheInvalidationHandler : INotificationHandler<GenreCreatedEvent> {
    public async Task Handle(GenreCreatedEvent @event, CancellationToken cancel) {
        await _cacheService.InvalidateCache("genres");
    }
}

// GenreNotificationHandler.cs - Notifications (Tách riêng!)
public class GenreNotificationHandler : INotificationHandler<GenreCreatedEvent> {
    public async Task Handle(GenreCreatedEvent @event, CancellationToken cancel) {
        await _notificationService.NotifyAdmins($"New genre: {@event.GenreName}");
    }
}
```

**Lợi ích:**
1. ✅ Handler chỉ lo business logic
2. ✅ Mỗi Event Handler có 1 nhiệm vụ duy nhất (SRP)
3. ✅ Thêm side effect mới → chỉ tạo Handler mới, không sửa code cũ (OCP)
4. ✅ Dễ test (mock ít dependencies)
5. ✅ Dễ disable/enable từng side effect (comment registration)

---

### **🧪 Test Case Comparison**

#### **Trước (Level 4):**
```csharp
[Fact]
public async Task CreateGenre_Success_Should_LogAndClearCache() {
    // Arrange
    var handler = new CreateGenreCommandHandler(
        _repository,
        _logger,        // ← Phải mock
        _cacheService,  // ← Phải mock
        _notificationService,  // ← Phải mock
        _searchService  // ← Phải mock
    );
    
    // Act
    await handler.Handle(command);
    
    // Assert
    _logger.Verify(x => x.LogInformation(...));
    _cacheService.Verify(x => x.InvalidateCache(...));
    _notificationService.Verify(x => x.NotifyAdmins(...));
}
```

#### **Sau (Level 5):**
```csharp
[Fact]
public async Task CreateGenre_Success_Should_RaiseDomainEvent() {
    // Arrange
    var handler = new CreateGenreCommandHandler(_repository);  // ← Ít dependency
    
    // Act
    var result = await handler.Handle(command);
    
    // Assert - Chỉ verify event được raise
    var genre = await _repository.GetByIdAsync(result.Data.IdGenre);
    Assert.Contains(genre.DomainEvents, e => e is GenreEvents.GenreCreated);
}

// Tách riêng test cho Event Handler
[Fact]
public async Task GenreCreatedEventHandler_Should_LogCorrectly() {
    var handler = new GenreCreatedEventHandler(_logger);
    await handler.Handle(new GenreCreatedEvent(...));
    _logger.Verify(x => x.LogInformation(It.IsAny<string>()));
}
```

---

## 🎯 **KẾT LUẬN**

### **✅ Những gì đã hoàn thành**

| # | Tính năng | Mô tả | Level |
|---|-----------|-------|-------|
| 1 | **Clean Architecture** | 4 layers phân tách rõ ràng, Domain không dependency | Level 2 |
| 2 | **CQRS Pattern** | Commands/Queries với MediatR | Level 3 |
| 3 | **DDD - Genre Entity** | Encapsulation, Factory Methods, Aggregates | Level 4 |
| 4 | **Domain Events** | GenreCreated, GenreUpdated, GenreDeleted | Level 5 |
| 5 | **Event Handlers** | Logging, Caching, Notifications (side effects) | Level 5 |
| 6 | **DbContext Integration** | Auto-dispatch events sau SaveChanges | Level 5 |
| 7 | **Clean Architecture Fix** | Domain ZERO dependencies, MediatR ở Application | Level 5 |
| 8 | **C# Record Syntax** | DomainEventBase là abstract record | Level 5 |

---

### **📚 Kiến thức học được**

1. **Separation of Concerns:**
   - Domain Events = Pure business events (no tech details)
   - Application Events = MediatR notifications (infrastructure adapter)

2. **Dependency Inversion:**
   - Domain định nghĩa interfaces (IDomainEvent)
   - Infrastructure implement chi tiết (DbContext dispatch events)

3. **Open/Closed Principle:**
   - Thêm side effect mới → tạo Event Handler mới
   - Không cần sửa code cũ (CreateGenreCommandHandler giữ nguyên)

4. **Single Responsibility:**
   - 1 Event Handler = 1 concern (logging OR caching OR notification)

5. **Event Sourcing Lite:**
   - Events ghi lại "what happened" (GenreCreated, GenreUpdated)
   - Có thể rebuild state từ events (nếu persist events)

---

### **🚀 Roadmap tiếp theo**

| # | Task | Priority | Estimate |
|---|------|----------|----------|
| 1 | Áp dụng Domain Events cho **Book** entity | High | 2h |
| 2 | Áp dụng Domain Events cho **User** entity | High | 2h |
| 3 | Thêm Event Store (persist events vào DB) | Medium | 4h |
| 4 | Integration Events cho Microservices (nếu cần) | Low | 8h |
| 5 | Outbox Pattern (đảm bảo eventual consistency) | Low | 6h |

---

### **📖 Tài liệu tham khảo**

- [Domain-Driven Design by Eric Evans](https://www.domainlanguage.com/ddd/)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [EF Core - Events and Interceptors](https://learn.microsoft.com/en-us/ef/core/logging-events-diagnostics/events)

---

Clean Architecture  → kiến trúc
DDD                → tư duy thiết kế Domain
CQRS / MediatR     → cách tổ chức luồng xử lý
Domain Events      → xử lý side effects (Level 5)

Made with ❤️ using Clean Architecture + DDD + CQRS + Event-Driven Architecture