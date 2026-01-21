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
```dotnet ef migrations add AddNotification --startup-project ..\TodoApp.WebAPI
dotnet ef migrations add LoadDatabaseNew --startup-project ..\TodoApp.WebAPI
### **Áp dụng Migration vào Database**
```bash
dotnet ef database update --startup-project ..\TodoApp.WebAPI
```
dotnet ef migrations remove --startup-project ..\TodoApp.WebAPI

### **Xóa Database (Cẩn thận!)**
```bash
# Xóa database
dotnet ef database drop --startup-project ..\TodoApp.WebAPI

# Xóa thư mục Migrations (nếu cần reset)
Remove-Item -Path "Migrations" -Recurse -Force
# Hoặc (Linux/Mac):
rm -r Migrations
```
dotnet ef migrations add FixAddLoadUser --startup-project ..\TodoApp.WebAPI
### **Lịch sử Migrations đã tạo**
```bash
dotnet ef migrations add ReloadUser --startup-project ..\TodoApp.WebAPI
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
dotnet ef migrations add AddEntitiesPJ --startup-project ..\TodoApp.WebAPI
```
dotnet ef migrations add AuditCode --startup-project ..\TodoApp.WebAPI
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

**3. Infrastructure Layer - Auto-Discovery với DomainEventDispatcher**
```csharp
// TodoApp.Infrastructure/Persistence/TodoAppDbContext.cs
// ✅ Không cần pattern matching thủ công!
await _eventDispatcher.DispatchAllAsync(domainEvents, cancellationToken);
```

---

### **🔄 IDomainEventDispatcher - Auto-Discovery Pattern**

#### **❓ Vấn đề với cách cũ (Pattern Matching)**

```csharp
// ❌ CŨ: Phải khai báo thủ công TỪNG event type
public class TodoAppDbContext : DbContext {
    private readonly IMediator _mediator;  // Inject trực tiếp
    
    public override async Task<int> SaveChangesAsync(...) {
        var notification = domainEvent switch {
            GenreEvents.GenreCreated e => new GenreCreatedEvent(e),
            GenreEvents.GenreUpdated e => new GenreUpdatedEvent(e),
            GenreEvents.GenreDeleted e => new GenreDeletedEvent(e),
            // ❌ Thêm BookEvents → phải sửa file này!
            // ❌ Thêm UserEvents → phải sửa file này!
            // ❌ Vi phạm Open/Closed Principle
            _ => null
        };
        await _mediator.Publish(notification);
    }
}
```

**Vấn đề:**
1. ❌ Mỗi lần thêm entity mới (Book, User) → phải sửa DbContext
2. ❌ Pattern matching list ngày càng dài
3. ❌ Vi phạm **Open/Closed Principle** (OCP)
4. ❌ DbContext biết quá nhiều về event types (tight coupling)

---

#### **✅ Giải pháp: IDomainEventDispatcher**

```csharp
// ✅ MỚI: DbContext không cần biết về event types
public class TodoAppDbContext : DbContext {
    private readonly IDomainEventDispatcher _eventDispatcher;  // Abstraction
    
    public override async Task<int> SaveChangesAsync(...) {
        // ✅ Chỉ 1 dòng, không cần biết chi tiết!
        await _eventDispatcher.DispatchAllAsync(domainEvents);
    }
}
```

---

#### **📁 Cấu trúc files**

```
TodoApp.Application/
└── Events/
    ├── IDomainEventWrapper.cs         ← Interface marker cho auto-discovery
    ├── GenreCreatedEvent.cs           ← Implement IDomainEventWrapper<T>
    ├── GenreUpdatedEvent.cs
    └── GenreDeletedEvent.cs

TodoApp.Infrastructure/
└── Services/
    └── DomainEventDispatcher.cs       ← Auto-discovery engine
```

---

#### **⚙️ Cách hoạt động**

```
┌──────────────────────────────────────────────────────────────┐
│ 1️⃣ DbContext gọi Dispatcher                                  │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  await _eventDispatcher.DispatchAllAsync(domainEvents);     │
│  // domainEvents = [GenreEvents.GenreCreated, ...]          │
└──────────────────┬───────────────────────────────────────────┘
                   ↓
┌──────────────────────────────────────────────────────────────┐
│ 2️⃣ DomainEventDispatcher - Auto Discovery (Reflection)       │
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  foreach (var domainEvent in domainEvents) {                │
│      // Lấy type của domain event                           │
│      var eventType = domainEvent.GetType();                 │
│      // → GenreEvents.GenreCreated                          │
│                                                              │
│      // Tìm wrapper implement IDomainEventWrapper<T>        │
│      var wrapperType = FindWrapperType(eventType);          │
│      // → GenreCreatedEvent                                 │
│                                                              │
│      // Tạo instance bằng reflection                        │
│      var notification = Activator.CreateInstance(           │
│          wrapperType, domainEvent);                         │
│      // → new GenreCreatedEvent(domainEvent)                │
│                                                              │
│      // Publish qua MediatR                                  │
│      await _mediator.Publish(notification);                 │
│  }                                                           │
└──────────────────────────────────────────────────────────────┘
```

---

#### **📊 So sánh 2 cách tiếp cận**

| Tiêu chí | IMediator trực tiếp | IDomainEventDispatcher |
|----------|---------------------|------------------------|
| **Thêm event mới** | ❌ Sửa DbContext | ✅ Chỉ tạo wrapper class |
| **DbContext code** | ❌ Phình to theo event count | ✅ Giữ nguyên mãi |
| **Auto-discovery** | ❌ Không | ✅ Có (Reflection) |
| **Open/Closed Principle** | ❌ Vi phạm | ✅ Tuân thủ |
| **Single Responsibility** | ❌ DbContext làm quá nhiều | ✅ Dispatcher chuyên biệt |
| **Testability** | ❌ Mock IMediator phức tạp | ✅ Mock IDomainEventDispatcher đơn giản |
| **Performance** | ✅ Nhanh hơn (no reflection) | ⚠️ Chậm hơn một chút (có cache) |

---

#### **🔧 Convention để Auto-Discovery hoạt động**

Để DomainEventDispatcher tự động tìm wrapper, bạn cần tuân thủ convention:

**1. Wrapper phải implement `IDomainEventWrapper<TDomainEvent>`**
```csharp
// ✅ ĐÚNG
public class GenreCreatedEvent : IDomainEventWrapper<GenreEvents.GenreCreated> {
    public GenreEvents.GenreCreated DomainEvent { get; }
    
    public GenreCreatedEvent(GenreEvents.GenreCreated domainEvent) {
        DomainEvent = domainEvent;
    }
}
```

**2. Wrapper phải có constructor nhận domain event**
```csharp
// Constructor signature phải match
public GenreCreatedEvent(GenreEvents.GenreCreated domainEvent)
```

**3. Wrapper phải nằm trong Application assembly**
```csharp
// DomainEventDispatcher scan assembly này
var applicationAssembly = typeof(IDomainEventWrapper).Assembly;
```

---

#### **🚀 Khi thêm BookEvents - Không cần sửa DbContext!**

**Bước 1: Tạo Domain Events (Domain Layer)**
```csharp
// TodoApp.Domain/Events/BookEvents.cs
public static class BookEvents {
    public record BookCreated : DomainEventBase {
        public int BookId { get; init; }
        public string BookName { get; init; }
        public BookCreated(int bookId, string bookName) {
            BookId = bookId;
            BookName = bookName;
        }
    }
}
```

**Bước 2: Tạo Wrapper (Application Layer)**
```csharp
// TodoApp.Application/Events/BookCreatedEvent.cs
public class BookCreatedEvent : IDomainEventWrapper<BookEvents.BookCreated> {
    public BookEvents.BookCreated DomainEvent { get; }
    IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
    
    public int BookId => DomainEvent.BookId;
    public string BookName => DomainEvent.BookName;
    
    public BookCreatedEvent(BookEvents.BookCreated domainEvent) {
        DomainEvent = domainEvent;
    }
}
```

**Bước 3: Tạo Handler (Application Layer)**
```csharp
// TodoApp.Application/Features/BookHandle/EventHandlers/BookCreatedEventHandler.cs
public class BookCreatedEventHandler : INotificationHandler<BookCreatedEvent> {
    private readonly ILogger<BookCreatedEventHandler> _logger;
    
    public async Task Handle(BookCreatedEvent @event, CancellationToken cancel) {
        _logger.LogInformation($"📚 Book created: {@event.BookName}");
    }
}
```

**✅ KHÔNG CẦN SỬA GÌ TRONG:**
- ❌ TodoAppDbContext.cs
- ❌ DomainEventDispatcher.cs
- ❌ Program.cs (MediatR tự scan handlers)

---

#### **🎯 Tóm tắt IDomainEventDispatcher**

```
┌─────────────────────────────────────────────────────────────┐
│ IDomainEventDispatcher                                       │
├─────────────────────────────────────────────────────────────┤
│ 📍 Vị trí: TodoApp.Infrastructure/Services/                 │
│                                                              │
│ 🎯 Mục đích:                                                 │
│ ├─ Tự động tìm wrapper cho mỗi domain event type            │
│ ├─ Convert Domain Events → MediatR Notifications            │
│ └─ Dispatch events qua MediatR                              │
│                                                              │
│ ✅ Lợi ích:                                                  │
│ ├─ DbContext sạch sẽ, không biết về event types             │
│ ├─ Thêm event mới không cần sửa code cũ (OCP)               │
│ ├─ Dễ test (mock interface)                                 │
│ └─ Tách biệt concerns (SRP)                                 │
│                                                              │
│ ⚠️ Trade-off:                                                │
│ └─ Dùng Reflection (có cache để optimize)                   │
└─────────────────────────────────────────────────────────────┘
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
No EVENTS
Request
 → MediatR.Send
 → CommandHandler
 → Domain Entity
 → Repository
 → DbContext.SaveChanges
 → Log
 → Clear Cache
 → Response
EVENT
Request
 → MediatR.Send
 → CommandHandler
 → Domain Entity (raise event)
 → Repository
 → DbContext.SaveChanges
 → MediatR.Publish(Event)
 → EventHandler(s)
 → Response

Made with ❤️ using Clean Architecture + DDD + CQRS + Event-Driven Architecture

---

## 📝 **HƯỚNG DẪN: TRIỂN KHAI EVENT-DRIVEN CHO GENRE (LEVEL 5)**

### **🎯 Mục tiêu**
Xây dựng Event-Driven Architecture cho Genre entity từ đầu, bao gồm:
- Domain Events (pure POCOs)
- Event Handlers (side effects)
- Auto-discovery Dispatcher
- Clean Architecture compliance

---

### **📋 BƯỚC 1: TẠO DOMAIN EVENT INFRASTRUCTURE (Common)**

#### **1.1. Tạo IDomainEvent.cs**
📍 `TodoApp.Domain/Common/IDomainEvent.cs`

```csharp
namespace TodoApp.Domain.Common
{
    /// <summary>
    /// Base interface cho tất cả Domain Events.
    /// Domain Events là pure POCOs, không phụ thuộc infrastructure.
    /// </summary>
    public interface IDomainEvent
    {
        /// <summary>
        /// Thời điểm event xảy ra
        /// </summary>
        DateTime OccurredOn { get; }
    }
}
```

**Lý do:** Interface để marking tất cả domain events.

---

#### **1.2. Tạo DomainEventBase.cs**
📍 `TodoApp.Domain/Common/DomainEventBase.cs`

```csharp
namespace TodoApp.Domain.Common
{
    /// <summary>
    /// Base record cho Domain Events.
    /// Sử dụng C# record để đảm bảo immutability.
    /// </summary>
    public abstract record DomainEventBase : IDomainEvent
    {
        public DateTime OccurredOn { get; init; }
        
        protected DomainEventBase()
        {
            OccurredOn = DateTime.UtcNow;
        }
    }
}
```

**Lý do:** Base class với auto-set timestamp.

---

#### **1.3. Tạo IHasDomainEvents.cs**
📍 `TodoApp.Domain/Common/IHasDomainEvents.cs`

```csharp
namespace TodoApp.Domain.Common
{
    /// <summary>
    /// Interface cho Aggregate Roots có thể raise Domain Events.
    /// Entities implement interface này sẽ có collection _domainEvents.
    /// </summary>
    public interface IHasDomainEvents
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void AddDomainEvent(IDomainEvent domainEvent);
        void RemoveDomainEvent(IDomainEvent domainEvent);
        void ClearDomainEvents();
    }
}
```

**Lý do:** Contract cho entities có thể raise events.

---

### **📋 BƯỚC 2: TẠO GENRE DOMAIN EVENTS**

#### **2.1. Tạo GenreEvents.cs**
📍 `TodoApp.Domain/Events/GenreEvents.cs`

```csharp
using TodoApp.Domain.Common;

namespace TodoApp.Domain.Events
{
    /// <summary>
    /// Domain Events cho Genre aggregate.
    /// Static class chứa các nested records.
    /// </summary>
    public static class GenreEvents
    {
        /// <summary>
        /// Event: Genre mới được tạo
        /// </summary>
        public record GenreCreated : DomainEventBase
        {
            public int GenreId { get; init; }
            public string GenreName { get; init; }
            
            public GenreCreated(int genreId, string genreName)
            {
                GenreId = genreId;
                GenreName = genreName;
            }
        }

        /// <summary>
        /// Event: Genre được cập nhật
        /// </summary>
        public record GenreUpdated : DomainEventBase
        {
            public int GenreId { get; init; }
            public string OldName { get; init; }
            public string NewName { get; init; }
            
            public GenreUpdated(int genreId, string oldName, string newName)
            {
                GenreId = genreId;
                OldName = oldName;
                NewName = newName;
            }
        }

        /// <summary>
        /// Event: Genre bị xóa
        /// </summary>
        public record GenreDeleted : DomainEventBase
        {
            public int GenreId { get; init; }
            public string GenreName { get; init; }
            
            public GenreDeleted(int genreId, string genreName)
            {
                GenreId = genreId;
                GenreName = genreName;
            }
        }
    }
}
```

**✅ Checkpoint:** Domain Events hoàn toàn PURE, không phụ thuộc gì!

---

### **📋 BƯỚC 3: CẬP NHẬT GENRE ENTITY**

#### **3.1. Implement IHasDomainEvents**
📍 `TodoApp.Domain/Entities/Genre.cs`

```csharp
public class Genre : IHasDomainEvents  // ← Implement interface
{
    // ... existing properties ...
    
    // Domain Events Support
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
    
    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
```

---

#### **3.2. Thêm RaiseCreatedEvent() method**

```csharp
public static Genre Create(string nameGenre)
{
    if (string.IsNullOrWhiteSpace(nameGenre))
        throw new ArgumentException("NameGenre cannot be empty");

    var genre = new Genre
    {
        NameGenre = nameGenre,
        CreatedAt = DateTime.UtcNow
    };
    
    // KHÔNG raise event ở đây vì IdGenre = 0
    return genre;
}

/// <summary>
/// Raise Created event SAU khi entity đã được save vào DB.
/// Lúc này IdGenre đã có giá trị thật từ database.
/// </summary>
public void RaiseCreatedEvent()
{
    AddDomainEvent(new GenreEvents.GenreCreated(this.IdGenre, this.NameGenre));
}
```

**⚠️ QUAN TRỌNG:** Event được raise SAU khi save, để có ID thật từ DB!

---

#### **3.3. Update() method raise event**

```csharp
public void Update(string nameGenre)
{
    if (string.IsNullOrWhiteSpace(nameGenre))
        throw new ArgumentException("NameGenre cannot be empty");

    var oldName = this.NameGenre;
    this.NameGenre = nameGenre;
    this.UpdatedAt = DateTime.UtcNow;

    // Raise Domain Event
    AddDomainEvent(new GenreEvents.GenreUpdated(this.IdGenre, oldName, nameGenre));
}
```

---

#### **3.4. MarkForDeletion() method raise event**

```csharp
public void MarkForDeletion()
{
    ValidateForDeletion();  // Business rule validation
    
    // Raise Domain Event
    AddDomainEvent(new GenreEvents.GenreDeleted(this.IdGenre, this.NameGenre));
}
```

---

### **📋 BƯỚC 4: TẠO APPLICATION EVENT WRAPPERS**

#### **4.1. Tạo IDomainEventWrapper.cs**
📍 `TodoApp.Application/Events/IDomainEventWrapper.cs`

```csharp
using MediatR;
using TodoApp.Domain.Common;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// Interface marker cho Domain Event wrappers.
    /// Cho phép auto-discovery.
    /// </summary>
    public interface IDomainEventWrapper : INotification
    {
        IDomainEvent DomainEvent { get; }
    }

    /// <summary>
    /// Generic wrapper interface cho type-safe conversion
    /// </summary>
    public interface IDomainEventWrapper<TDomainEvent> : IDomainEventWrapper
        where TDomainEvent : IDomainEvent
    {
        new TDomainEvent DomainEvent { get; }
    }
}
```

---

#### **4.2. Tạo GenreCreatedEvent.cs (wrapper)**
📍 `TodoApp.Application/Events/GenreCreatedEvent.cs`

```csharp
using MediatR;
using TodoApp.Domain.Common;
using static TodoApp.Domain.Events.GenreEvents;

namespace TodoApp.Application.Events
{
    /// <summary>
    /// MediatR Notification wrapper cho Genre Created Domain Event.
    /// Implement IDomainEventWrapper để hỗ trợ auto-discovery.
    /// </summary>
    public class GenreCreatedEvent : IDomainEventWrapper<GenreCreated>
    {
        public GenreCreated DomainEvent { get; }
        
        // Explicit interface implementation
        IDomainEvent IDomainEventWrapper.DomainEvent => DomainEvent;
        
        public int GenreId => DomainEvent.GenreId;
        public string GenreName => DomainEvent.GenreName;
        public DateTime OccurredOn => DomainEvent.OccurredOn;

        public GenreCreatedEvent(GenreCreated domainEvent)
        {
            DomainEvent = domainEvent;
        }
    }
}
```

**Tương tự:** Tạo `GenreUpdatedEvent.cs` và `GenreDeletedEvent.cs`

---

### **📋 BƯỚC 5: TẠO AUTO-DISCOVERY DISPATCHER**

#### **5.1. Tạo DomainEventDispatcher.cs**
📍 `TodoApp.Infrastructure/Services/DomainEventDispatcher.cs`

```csharp
using System.Collections.Concurrent;
using System.Reflection;
using MediatR;
using TodoApp.Application.Events;
using TodoApp.Domain.Common;

namespace TodoApp.Infrastructure.Services
{
    /// <summary>
    /// Service tự động convert Domain Events → MediatR Notifications.
    /// Sử dụng reflection để auto-discover event wrappers.
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IMediator _mediator;
        private static readonly ConcurrentDictionary<Type, Type?> _eventWrapperCache = new();
        private static readonly ConcurrentDictionary<Type, ConstructorInfo?> _constructorCache = new();

        public DomainEventDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
        {
            var notification = CreateNotification(domainEvent);
            
            if (notification != null)
            {
                await _mediator.Publish(notification, cancellationToken);
            }
        }

        public async Task DispatchAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                await DispatchAsync(domainEvent, cancellationToken);
            }
        }

        private INotification? CreateNotification(IDomainEvent domainEvent)
        {
            var domainEventType = domainEvent.GetType();
            var wrapperType = _eventWrapperCache.GetOrAdd(domainEventType, FindWrapperType);
            
            if (wrapperType == null) return null;

            var constructor = _constructorCache.GetOrAdd(wrapperType, t => 
                t.GetConstructor(new[] { domainEventType }));
            
            if (constructor == null) return null;

            return constructor.Invoke(new object[] { domainEvent }) as INotification;
        }

        private static Type? FindWrapperType(Type domainEventType)
        {
            var targetInterface = typeof(IDomainEventWrapper<>).MakeGenericType(domainEventType);
            var applicationAssembly = typeof(IDomainEventWrapper).Assembly;
            
            return applicationAssembly.GetTypes()
                .FirstOrDefault(t => 
                    !t.IsAbstract && 
                    !t.IsInterface && 
                    targetInterface.IsAssignableFrom(t));
        }
    }

    public interface IDomainEventDispatcher
    {
        Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
        Task DispatchAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
```

**✨ Magic:** Tự động tìm wrapper cho mỗi domain event type!

---

### **📋 BƯỚC 6: CẬP NHẬT DBCONTEXT**

#### **6.1. Sửa TodoAppDbContext.cs**
📍 `TodoApp.Infrastructure/Persistence/TodoAppDbContext.cs`

```csharp
public class TodoAppDbContext : DbContext
{
    private readonly IDomainEventDispatcher _eventDispatcher;  // ← Inject dispatcher

    public TodoAppDbContext(
        DbContextOptions<TodoAppDbContext> options, 
        IDomainEventDispatcher eventDispatcher) 
        : base(options)
    {
        _eventDispatcher = eventDispatcher;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // 1. Lấy entities có Domain Events
        var entitiesWithEvents = ChangeTracker.Entries<IHasDomainEvents>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        // 2. Lấy events
        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // 3. Clear events
        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        // 4. Save TRƯỚC
        var result = await base.SaveChangesAsync(cancellationToken);

        // 5. Dispatch events SAU (tự động tìm wrapper)
        await _eventDispatcher.DispatchAllAsync(domainEvents, cancellationToken);

        return result;
    }
}
```

**✅ Lợi ích:** Không cần pattern matching, tự động dispatch!

---

### **📋 BƯỚC 7: TẠO EVENT HANDLERS**

#### **7.1. GenreCreatedEventHandler.cs (Logging)**
📍 `TodoApp.Application/Features/GenreHandle/EventHandlers/`

```csharp
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.GenreHandle.EventHandlers
{
    public class GenreCreatedEventHandler : INotificationHandler<GenreCreatedEvent>
    {
        private readonly ILogger<GenreCreatedEventHandler> _logger;

        public GenreCreatedEventHandler(ILogger<GenreCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(GenreCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "✅ Domain Event: Genre '{GenreName}' (ID: {GenreId}) was created at {Time}",
                notification.GenreName,
                notification.GenreId,
                notification.OccurredOn);

            return Task.CompletedTask;
        }
    }
}
```

---

#### **7.2. GenreCacheInvalidationHandler.cs**

```csharp
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;

namespace TodoApp.Application.Features.GenreHandle.EventHandlers
{
    public class GenreCacheInvalidationHandler :
        INotificationHandler<GenreCreatedEvent>,
        INotificationHandler<GenreUpdatedEvent>,
        INotificationHandler<GenreDeletedEvent>
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<GenreCacheInvalidationHandler> _logger;
        private const string ALL_GENRES_CACHE_KEY = "genres:all";

        public GenreCacheInvalidationHandler(IMemoryCache cache, ILogger<GenreCacheInvalidationHandler> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task Handle(GenreCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(" [CACHE] Clearing cache after Genre creation");
            _cache.Remove(ALL_GENRES_CACHE_KEY);
            return Task.CompletedTask;
        }

        public Task Handle(GenreUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(" [CACHE] Clearing cache after Genre update");
            _cache.Remove(ALL_GENRES_CACHE_KEY);
            _cache.Remove($"genres:id:{notification.GenreId}");
            return Task.CompletedTask;
        }

        public Task Handle(GenreDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation(" [CACHE] Clearing cache after Genre deletion");
            _cache.Remove(ALL_GENRES_CACHE_KEY);
            return Task.CompletedTask;
        }
    }
}
```

---

#### **7.3. GenreAuditLogHandler.cs**

```csharp
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Repository;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Features.GenreHandle.EventHandlers
{
    public class GenreAuditLogHandler :
        INotificationHandler<GenreCreatedEvent>,
        INotificationHandler<GenreUpdatedEvent>,
        INotificationHandler<GenreDeletedEvent>
    {
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<GenreAuditLogHandler> _logger;

        public GenreAuditLogHandler(IAuditLogRepository auditLogRepository, ILogger<GenreAuditLogHandler> logger)
        {
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task Handle(GenreCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 [AUDIT] Recording CREATE for Genre ID: {GenreId}", notification.GenreId);

            var newValues = JsonSerializer.Serialize(new { notification.GenreId, notification.GenreName });
            var auditLog = AuditLog.Create("CREATE", "Genre", notification.GenreId.ToString(), null, newValues, "System");
            
            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task Handle(GenreUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 [AUDIT] Recording UPDATE for Genre ID: {GenreId}", notification.GenreId);

            var oldValues = JsonSerializer.Serialize(new { GenreName = notification.OldName });
            var newValues = JsonSerializer.Serialize(new { GenreName = notification.NewName });
            var auditLog = AuditLog.Create("UPDATE", "Genre", notification.GenreId.ToString(), oldValues, newValues, "System");
            
            await _auditLogRepository.AddAsync(auditLog);
        }

        public async Task Handle(GenreDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📝 [AUDIT] Recording DELETE for Genre ID: {GenreId}", notification.GenreId);

            var oldValues = JsonSerializer.Serialize(new { notification.GenreId, notification.GenreName });
            var auditLog = AuditLog.Create("DELETE", "Genre", notification.GenreId.ToString(), oldValues, null, "System");
            
            await _auditLogRepository.AddAsync(auditLog);
        }
    }
}
```

---

#### **7.4. GenreNotificationHandler.cs**

```csharp
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Events;
using TodoApp.Application.Service;

namespace TodoApp.Application.Features.GenreHandle.EventHandlers
{
    public class GenreNotificationHandler :
        INotificationHandler<GenreCreatedEvent>,
        INotificationHandler<GenreUpdatedEvent>,
        INotificationHandler<GenreDeletedEvent>
    {
        private readonly ILogger<GenreNotificationHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly string[] _adminEmails;

        public GenreNotificationHandler(
            ILogger<GenreNotificationHandler> logger,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _logger = logger;
            _emailService = emailService;
            _adminEmails = configuration.GetSection("AdminEmails").Get<string[]>() ?? new[] { "admin@example.com" };
        }

        public async Task Handle(GenreCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [NOTIFICATION] Sending email for new Genre: {GenreName}", notification.GenreName);

            var subject = $"🎉 New Genre Created: {notification.GenreName}";
            var body = $@"
                <h2>Genre Created</h2>
                <p><strong>ID:</strong> {notification.GenreId}</p>
                <p><strong>Name:</strong> {notification.GenreName}</p>
                <p><strong>Time:</strong> {notification.OccurredOn:yyyy-MM-dd HH:mm:ss}</p>";

            foreach (var email in _adminEmails)
            {
                await _emailService.SendEmailAsync(email, subject, body, isHtml: true);
            }
        }

        public async Task Handle(GenreUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [NOTIFICATION] Sending email for Genre update");
            // Similar implementation...
        }

        public async Task Handle(GenreDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("📧 [NOTIFICATION] Sending email for Genre deletion");
            // Similar implementation...
        }
    }
}
```

---

### **📋 BƯỚC 8: CẬP NHẬT COMMAND HANDLER**

#### **8.1. Sửa CreateGenreCommandHandler.cs**
📍 `TodoApp.Application/Features/GenreHandle/Command/Create/`

```csharp
public async Task<Result<GenreResponseDTO>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
{
    // 1. Business validation
    var existingGenre = await _genreRepository.GetNameGenreAsync(request.NameGenre);
    if (existingGenre != null)
    {
        return Result<GenreResponseDTO>.Failure(ErrorType.Conflict, "Genre đã tồn tại");
    }

    // 2. Tạo Genre (chưa có event)
    var newGenre = Genre.Create(request.NameGenre);

    // 3. Save để có ID
    await _genreRepository.AddGenreAsync(newGenre);

    // 4. Raise event SAU khi có ID thật
    newGenre.RaiseCreatedEvent();
    await _genreRepository.SaveChangesAsync();  // ← Events được dispatch ở đây

    // 5. Return DTO
    return Result<GenreResponseDTO>.Success(new GenreResponseDTO { ... });
}
```

**⚠️ KEY POINT:** Raise event SAU khi save, để có ID thật!

---

### **📋 BƯỚC 9: ĐĂNG KÝ DEPENDENCY INJECTION**

#### **9.1. Cập nhật Program.cs**
📍 `TodoApp.WebAPI/Program.cs`

```csharp
// Register Repositories
builder.Services.AddScoped<IGenreRepository, GenreRepositoryImpl>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepositoryImpl>();

// Register Domain Event Dispatcher (Auto-discovery)
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

// Register Memory Cache
builder.Services.AddMemoryCache();

// Register Email Service
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// Register MediatR (auto-scan event handlers)
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(CreateGenreCommand).Assembly);
});
```

---

### **📋 BƯỚC 10: TẠO SUPPORTING ENTITIES**

#### **10.1. AuditLog.cs**
📍 `TodoApp.Domain/Entities/AuditLog.cs`

```csharp
namespace TodoApp.Domain.Entities
{
    public class AuditLog
    {
        public int Id { get; private set; }
        public string Action { get; private set; } = null!;
        public string EntityType { get; private set; } = null!;
        public string EntityId { get; private set; } = null!;
        public string? OldValues { get; private set; }
        public string? NewValues { get; private set; }
        public DateTime Timestamp { get; private set; }
        public string? PerformedBy { get; private set; }

        private AuditLog() { }

        public static AuditLog Create(string action, string entityType, string entityId, 
            string? oldValues, string? newValues, string? performedBy)
        {
            return new AuditLog
            {
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                OldValues = oldValues,
                NewValues = newValues,
                Timestamp = DateTime.UtcNow,
                PerformedBy = performedBy
            };
        }
    }
}
```

---

#### **10.2. IAuditLogRepository.cs** + Implementation

Tạo interface và implementation trong Application/Infrastructure layers.

---

### **📋 BƯỚC 11: TẠO DATABASE MIGRATION**

```bash
cd TodoApp.Infrastructure
dotnet ef migrations add AddAuditLogsTable --startup-project ..\TodoApp.WebAPI
dotnet ef database update --startup-project ..\TodoApp.WebAPI
```

---

### **📋 BƯỚC 12: TEST**

#### **12.1. Build project**
```bash
cd TodoApp.WebAPI
dotnet build
```

#### **12.2. Run application**
```bash
dotnet run
```

#### **12.3. Test tạo Genre**
```http
POST https://localhost:7xxx/api/genres
Content-Type: application/json

{
  "nameGenre": "Test Event Domain"
}
```

#### **12.4. Kiểm tra logs**
```
✅ Domain Event: Genre 'Test Event Domain' (ID: 5) created
🗑️ [CACHE] Clearing cache after Genre creation
📝 [AUDIT] Recording CREATE for Genre ID: 5
📧 [NOTIFICATION] Sending email for new Genre
```

---

## ✅ **CHECKLIST HOÀN THÀNH**

| # | Bước | Status |
|---|------|--------|
| 1 | Domain Event Infrastructure | ✅ |
| 2 | Genre Domain Events | ✅ |
| 3 | Genre Entity (IHasDomainEvents) | ✅ |
| 4 | Application Event Wrappers | ✅ |
| 5 | Auto-Discovery Dispatcher | ✅ |
| 6 | DbContext Integration | ✅ |
| 7 | Event Handlers (4 handlers) | ✅ |
| 8 | Command Handler Update | ✅ |
| 9 | DI Registration | ✅ |
| 10 | Supporting Entities | ✅ |
| 11 | Database Migration | ✅ |
| 12 | Testing | ✅ |

---

## 🎯 **LỢI ÍCH ĐẠT ĐƯỢC**

### **1. Separation of Concerns**
- CreateGenreCommandHandler chỉ lo business logic
- Event handlers lo side effects riêng biệt

### **2. Open/Closed Principle**
- Thêm side effect mới → Tạo handler mới
- Không sửa code cũ

### **3. Single Responsibility**
- 1 handler = 1 concern (logging/cache/audit/email)

### **4. Testability**
- Test business logic riêng
- Test side effects riêng
- Mock ít dependencies

### **5. Clean Architecture**
- Domain ZERO dependencies
- Events flow: Domain → Application → Infrastructure

---
📱 Client: POST /api/auth/register
    ↓
RegisterCommandHandler
    ├─ var user = User.Register(...)
    ├─ await AddUserAsync(user)  // IdUser = 42
    ├─ user.RaiseRegisteredEvent()  // _domainEvents = [UserRegistered]
    └─ await SaveChangesAsync()  ← GỌI HÀM NÀY!
        ↓
┌───────────────────────────────────────────────────────┐
│ TodoAppDbContext.SaveChangesAsync()                   │
├───────────────────────────────────────────────────────┤
│                                                       │
│ 1️⃣ Tìm entities có events                            │
│    → [user]                                           │
│                                                       │
│ 2️⃣ Lấy events                                         │
│    → [UserRegistered(42, "john@...", "123456")]      │
│                                                       │
│ 3️⃣ Clear events                                       │
│    user._domainEvents = []                            │
│                                                       │
│ 4️⃣ Save DB                                            │
│    INSERT INTO Users ... → IdUser = 42                │
│                                                       │
│ 5️⃣ Dispatch events                                    │
│    DomainEventDispatcher                              │
│    ├─ Tìm wrapper: UserRegisteredEvent               │
│    └─ MediatR.Publish()                               │
│        ├─ UserNotificationHandler → Gửi email ✉️     │
│        └─ UserAuditLogHandler → Ghi log 📝           │
└───────────────────────────────────────────────────────┘

1. Command → Handler
2. Handler → Business logic
3. Handler → SaveChangesAsync() ← TRIGGER
4. DbContext override → Lấy events
5. DbContext → Commit DB
6. DbContext → EventDispatcher
7. EventDispatcher → Tìm wrapper (reflection)
8. EventDispatcher → MediatR.Publish()
9. MediatR → Tìm handlers
10. MediatR → Task.WhenAll() (parallel)
11. Handlers → Chạy song song
12. Handlers → DONE
13. SaveChangesAsync() → Return
14. Command Handler → Return
15. Controller → Return response
Made with ❤️ using Event-Driven Architecture (Level 5)

🏗️ TỔNG QUAN KIẾN TRÚC DỰ ÁN TODOAPP (BOOKSTORE)
📊 Kiến trúc tổng thể: Clean Architecture + CQRS + Event-Driven

┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│  ┌───────────────┐     ┌──────────────────────────────┐    │
│  │  Controllers  │────▶│  GlobalExceptionFilter       │    │
│  │  - AuthController    │  - Validation Errors          │    │
│  │  - OrdersController  │  - Business Logic Errors      │    │
│  │  - BookController    │  - Unauthorized              │    │
│  └───────┬───────┘     └──────────────────────────────┘    │
│          │ MediatR.Send()                                    │
└──────────┼───────────────────────────────────────────────────┘
           ▼
┌─────────────────────────────────────────────────────────────┐
│                   APPLICATION LAYER                          │
│  ┌──────────────┐  ┌─────────────┐  ┌──────────────────┐  │
│  │  Commands    │  │   Queries   │  │  Event Handlers  │  │
│  │  - Create    │  │  - GetAll   │  │  - Notification  │  │
│  │  - Update    │  │  - GetById  │  │  - AuditLog      │  │
│  │  - Delete    │  │  - Filter   │  │  - Cache         │  │
│  └──────┬───────┘  └──────┬──────┘  └────────▲─────────┘  │
│         │                  │                   │             │
│  ┌──────▼──────────────────▼───────┐    ┌─────┴─────────┐ │
│  │   Command/Query Handlers        │    │ Event Wrappers│ │
│  │  ┌────────────────────────┐     │    │ - UserReg..   │ │
│  │  │ ValidationBehavior     │◀────┼────│ - OrderCre..  │ │
│  │  │ (FluentValidation)     │     │    └───────────────┘ │
│  │  └────────────────────────┘     │                       │
│  └──────┬───────────────────────────┘                       │
│         │ Repository Interfaces                             │
└─────────┼───────────────────────────────────────────────────┘
          ▼
┌─────────────────────────────────────────────────────────────┐
│                  INFRASTRUCTURE LAYER                        │
│  ┌──────────────────────┐    ┌─────────────────────────┐   │
│  │ Repository Impls     │    │  Services               │   │
│  │ - UserRepository     │    │  - EmailService (SMTP)  │   │
│  │ - OrderRepository    │    │  - JwtService           │   │
│  │ - BookRepository     │    │  - DomainEventDispatcher│   │
│  └──────┬───────────────┘    └──────────┬──────────────┘   │
│         │                                │                   │
│  ┌──────▼────────────────────────────────▼──────────────┐  │
│  │          TodoAppDbContext (EF Core)                   │  │
│  │  ┌──────────────────────────────────────────────┐    │  │
│  │  │ Override SaveChangesAsync():                 │    │  │
│  │  │  1. Collect Domain Events                    │    │  │
│  │  │  2. Save to Database                        │    │  │
│  │  │  3. Dispatch Events via DomainEventDispatcher│    │  │
│  │  │  4. MediatR publishes to handlers           │    │  │
│  │  └──────────────────────────────────────────────┘    │  │
│  └───────────────────────────┬───────────────────────────┘  │
│                              │                               │
└──────────────────────────────┼───────────────────────────────┘
                               ▼
                    ┌────────────────────┐
                    │   MySQL Database   │
                    │   - Users          │
                    │   - Orders         │
                    │   - Books          │
                    │   - AuditLogs      │
                    └────────────────────┘


# 🔄 LUỒNG XỬ LÝ REQUEST CHI TIẾT
Ví dụ 1: User đăng ký (POST /api/auth/register)
1️⃣ REQUEST từ Frontend
   POST /api/auth/register
   Body: { username, email, password, confirmPassword }
   ↓
2️⃣ CONTROLLER (AuthController)
   [HttpPost("register")]
   public async Task<IActionResult> Register([FromBody] RegisterCommand command)
   {
       var response = await _mediator.Send(command); // ← Gửi command
       return Ok(response);
   }
   ↓
3️⃣ MEDIATR PIPELINE
   ┌─────────────────────────────────────┐
   │ ValidationBehavior<TRequest>        │
   │ - FluentValidation tự động          │
   │ - RegisterCommandValidator          │
   │   • Username required, 3-50 chars   │
   │   • Email valid format              │
   │   • Password min 6 chars            │
   │   • ConfirmPassword match           │
   └──────────┬──────────────────────────┘
              │ ✅ Valid
   ↓
4️⃣ COMMAND HANDLER (RegisterCommandHandler)
   public async Task<RegisterResponse> Handle(...)
   {
       // Check duplicate
       var existing = await _userRepository.GetUserByEmailAsync(...);
       if (existing != null) throw new InvalidOperationException("Email exists");
       
       // Hash password
       var hashedPassword = BCrypt.HashPassword(password);
       
       // Create entity với Factory Method
       var newUser = User.Register(username, email, hashedPassword, "User");
       
       // Save to DB
       await _userRepository.AddUserAsync(newUser);
       
       // ⭐ RAISE DOMAIN EVENT
       newUser.RaiseRegisteredEvent(); // Add event vào _domainEvents list
       
       // ⭐ SAVE → Trigger event dispatch
       await _userRepository.SaveChangesAsync();
       
       return new RegisterResponse { ... };
   }
   ↓
5️⃣ REPOSITORY LAYER
   public async Task SaveChangesAsync()
   {
       await _context.SaveChangesAsync(); // ← Gọi DbContext
   }
   ↓
6️⃣ DBCONTEXT - SaveChangesAsync() OVERRIDE
   public override async Task<int> SaveChangesAsync(...)
   {
       // 1. Thu thập entities có events
       var entities = ChangeTracker.Entries<IHasDomainEvents>()
           .Where(e => e.Entity.DomainEvents.Any())
           .ToList();
       
       // 2. Lấy events
       var events = entities.SelectMany(e => e.DomainEvents).ToList();
       // → [UserRegistered(IdUser=5, Email="user@mail.com", Code="ABC123")]
       
       // 3. Clear events khỏi entities
       entities.ForEach(e => e.ClearDomainEvents());
       
       // 4. LƯU DATABASE TRƯỚC (Data Consistency)
       var result = await base.SaveChangesAsync();
       // → INSERT INTO Users VALUES (5, 'user@mail.com', ...)
       
       // 5. DISPATCH EVENTS
       await _eventDispatcher.DispatchAllAsync(events);
       
       return result;
   }
   ↓
7️⃣ DOMAIN EVENT DISPATCHER (Auto-discovery)
   public async Task DispatchAsync(IDomainEvent domainEvent, ...)
   {
       // Auto-find wrapper
       // UserRegistered → UserRegisteredEvent (via Reflection)
       var notification = CreateNotification(domainEvent);
       
       // Publish qua MediatR
       await _mediator.Publish(notification); // ← INotification
   }
   ↓
8️⃣ MEDIATR PUBLISH → Gọi TẤT CẢ handlers
   ┌─────────────────────────────────────────┐
   │ UserNotificationHandler                 │
   │  → Gửi email xác thực với code ABC123   │
   │  → SMTP (Gmail/Outlook)                 │
   └─────────────────────────────────────────┘
   ┌─────────────────────────────────────────┐
   │ UserAuditLogHandler                     │
   │  → Ghi log: "User 5 registered"         │
   │  → INSERT INTO AuditLogs                │
   └─────────────────────────────────────────┘
   ┌─────────────────────────────────────────┐
   │ UserCacheInvalidationHandler            │
   │  → Xóa cache users list                 │
   └─────────────────────────────────────────┘
   ↓
9️⃣ RESPONSE trả về Controller
   return Ok({
       userId: 5,
       username: "john",
       email: "user@mail.com",
       message: "Đăng ký thành công! Check email để kích hoạt"
   });
   ↓
🔟 EXCEPTION HANDLING (nếu có lỗi)
   GlobalExceptionFilter:
   - ValidationException → 400 BadRequest
   - InvalidOperationException → 400 (Email exists)
   - UnauthorizedException → 401
   - Unknown → 500 Internal Server Error


# Ví dụ 2: Tạo Order (POST /api/orders)
   1️⃣ REQUEST
   POST /api/orders
   Body: {
     idUser: 5,
     note: "Giao nhanh",
     items: [
       { idBook: 10, quantity: 2 },
       { idBook: 15, quantity: 1 }
     ]
   }
   ↓
2️⃣ CONTROLLER (OrdersController)
   var result = await _mediator.Send(command);
   ↓
3️⃣ VALIDATION BEHAVIOR
   CreateOrderCommandValidator:
   - IdUser required
   - Items not empty
   - Quantity > 0
   ↓
4️⃣ COMMAND HANDLER (CreateOrderCommandHandler)
   // 1. Tạo Order aggregate
   var order = Orders.Create(idUser, note);
   
   // 2. Save để có IdOrder (auto-increment)
   await _orderRepository.AddAsync(order);
   await _orderRepository.SaveChangesAsync();
   
   // 3. Thêm OrderDetails
   foreach (var item in items) {
       var book = await _bookRepository.GetBookByIdAsync(item.IdBook);
       if (book == null) return NotFound;
       
       // Check stock
       if (book.Quantity < item.Quantity) return InsufficientStock;
       
       var price = book.SellPrice > 0 ? book.SellPrice : book.ListPrice;
       var detail = OrderDetails.Create(order.IdOrder, item.IdBook, item.Quantity, price);
       order.AddOrderDetail(detail);
   }
   
   // 4. Recalculate total
   order.RecalculateTotalPrice();
   
   // 5. RAISE EVENT
   order.RaiseCreatedEvent(); // ← OrderCreated event
   
   // 6. SAVE → Dispatch events
   await _orderRepository.SaveChangesAsync();
   ↓
5️⃣ DBCONTEXT SaveChanges()
   // Lưu Order + OrderDetails + Dispatch OrderCreated event
   ↓
6️⃣ EVENT HANDLERS (Parallel execution)
   ┌─────────────────────────────────────┐
   │ OrderNotificationHandler            │
   │  → Gửi email xác nhận đơn hàng      │
   └─────────────────────────────────────┘
   ┌─────────────────────────────────────┐
   │ OrderAuditLogHandler                │
   │  → Log: "Order #123 created"        │
   └─────────────────────────────────────┘
   ┌─────────────────────────────────────┐
   │ OrderCacheInvalidationHandler       │
   │  → Clear cache orders list          │
   └─────────────────────────────────────┘
   ┌─────────────────────────────────────┐
   │ OrderLoggingHandler                 │
   │  → Console log order details        │
   └─────────────────────────────────────┘
   ↓
7️⃣ RESPONSE
   return CreatedAtAction(GetOrderById, 
       new { id = 123 },
       new { message: "Tạo đơn hàng thành công", data: orderDTO }
   );


   # 📈 DATA FLOW SUMMARY
   HTTP Request
    ↓
[Controller] - Minimal logic, chỉ route
    ↓
[MediatR Send/Publish]
    ↓
[Validation Behavior] - FluentValidation
    ↓
[Command/Query Handler] - Business logic
    ↓
[Repository] - Data access
    ↓
[DbContext.SaveChangesAsync()] - Transaction + Event Dispatch
    ↓
[DomainEventDispatcher] - Auto-find wrapper
    ↓
[MediatR.Publish] - Fan-out to handlers
    ↓
[Event Handlers] - Side-effects (Email, Log, Cache...)
    ↓
[Response] - Return DTO to controller
    ↓
HTTP Response (JSON)



# IHasDomainEvents: là interface giúp entity lưu lại các event đã xảy ra trong domain.
# DomainEventBase: là base class dùng để đánh dấu và xác định nơi phát sinh sự kiện trong domain.
# IDomainEvent: là interface dùng để ghi nhận thời gian xảy ra event và các thông tin liên quan đến event đó.