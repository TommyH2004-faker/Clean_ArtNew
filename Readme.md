
# Giải Thích Chi Tiết Project Clean Architecture (TodoApp)
Tài liệu này giải thích các khía cạnh quan trọng của dự án theo yêu cầu, bao gồm cấu trúc, luồng dữ liệu, thư viện và migration.
# 1. Policy + Version (MediatR)
- MediatR: Version 14.0.0 (Cung cấp pattern CQRS: Command Query Responsibility Segregation). =>> đang là bản mới nhất 
- FluentValidation: Version 12.1.1 (Thường đi kèm để validate dữ liệu đầu vào).
=>> bản mới nhất 
## 2. Luồng Của Dữ Liệu (Config, Validate, Flow)
Luồng xử lý một request (Ví dụ: `CreateUser`) diễn ra như sau:

1.  **Config (Khởi động)**:
    - `Program.cs` đăng ký các services: MediatR (`AddMediatR`), Validators (`AddValidatorsFromAssembly`), DBContext.

2.  **Request (API call)**:
    - Client gọi API -> `UserController`.

3.  **MediatR Dispatch**:
    - Controller gọi `_sender.Send(command)`.

4.  **Validate (Kiểm tra dữ liệu)**:
    - **Thực tế trong code hiện tại**: Validation đang được viết **thủ công** bên trong `CreateUserCommandHandler.cs` (`if (string.IsNullOrWhiteSpace...)`).
    - **Lý thuyết chuẩn (Clean Architecture)**: Nên sử dụng **Pipeline Behavior**. Khi `Send(command)` được gọi, MediatR sẽ tự động chạy qua một `ValidationBehavior` (Middleware của MediatR) để gọi `CreateUserCommandValidator` trước khi đến Handler.
    - *Lưu ý*: Hiện tại project đã cài đặt `FluentValidation` và có file `CreateUserCommandValidator.cs` nhưng chưa được cấu hình `AddOpenBehavior` trong `Program.cs`, nên validator này có thể chưa chạy tự động.

5.  **Handler (Xử lý)**:
    - `CreateUserCommandHandler` nhận command hợp lệ -> Gọi Repository -> Lưu xuống DB.

## 3. MediatR - Chức Năng & Cách Hoạt Động Chi Tiết

### MediatR là gì?
**MediatR** là một thư viện .NET triển khai **Mediator Pattern** (Pattern trung gian). Nó đóng vai trò là người **trung gian điều phối** giữa các thành phần trong ứng dụng, giúp chúng giao tiếp mà không cần biết trực tiếp về nhau.

### Chức năng chính

#### 1. **Decoupling (Tách rời phụ thuộc)**
- Controller không cần inject trực tiếp các Service (UserService, TodoService...).
- Thay vào đó, Controller chỉ cần inject `IMediator` và gửi request (Command/Query).
- MediatR sẽ tự động tìm Handler phù hợp và điều hướng request đến đó.

#### 2. **CQRS Pattern (Command Query Responsibility Segregation)**
- **Command**: Thao tác thay đổi dữ liệu (Create, Update, Delete). Trả về `Result` hoặc `void`.
- **Query**: Thao tác đọc dữ liệu (Get, GetAll). Trả về dữ liệu.
- Tách biệt rõ ràng giữa logic đọc và ghi, dễ tối ưu hóa performance.

#### 3. **Pipeline Behaviors (Middleware)**
- Cho phép thêm các xử lý trước/sau khi Handler chạy mà không cần sửa code Handler.
- Ví dụ: Logging, Validation, Transaction, Caching, Authorization.

### Cách hoạt động trong Project

### Luồng hoạt động chi tiết

Client Request
    ↓
Controller (nhận HTTP request)
    ↓
Tạo Command/Query object
    ↓
_mediator.Send(command) ← Gửi đến MediatR
    ↓
MediatR tìm Handler tương ứng (qua DI Container)
    ↓
[Pipeline Behaviors] ← Validation, Logging (nếu có)
    ↓
Handler.Handle(command) ← Xử lý logic
    ↓
Trả về Result
    ↓
Controller nhận Result
    ↓
Trả về HTTP Response cho Client
```

### So sánh: Không dùng MediatR vs Dùng MediatR
| Không MediatR                   | Có MediatR      |
| ------------------------------- | --------------- |
| Gọi handler trực tiếp           | `Send(command)` |
| Explicit                        | Abstract        |
| Dễ debug                        | Khó trace       |
| Ít magic                        | Nhiều pipeline  |
| Controller inject nhiều handler | Controller gọn  |
#### **Không dùng MediatR (Truyền thống)**
```csharp
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly ILogger _logger;

    // Phải inject nhiều service
    public UserController(IUserService userService, IEmailService emailService, ILogger logger)
    {
        _userService = userService;
        _emailService = emailService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        // Validation, logging phải viết ở đây
        _logger.LogInformation("Creating user...");
        var result = await _userService.CreateUserAsync(request);
        await _emailService.SendWelcomeEmail(result.Email);
        return Ok(result);
    }
}
```
 **Nhược điểm:**
- Controller phụ thuộc nhiều vào Service → khó test.
- Mỗi endpoint phải inject các service khác nhau → code dài.
- Logic cross-cutting (log, validate) lặp lại ở mọi nơi.

 **Dùng MediatR**
```csharp
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    // Chỉ cần inject 1 dependency duy nhất
    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(request.Username, request.Email);
        var result = await _mediator.Send(command);  // Gửi và quên
        return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
    }
}
```
 **Ưu điểm:**
- Controller nhẹ nhàng, chỉ lo routing.
- Logic xử lý tập trung ở Handler (Single Responsibility).
- Dễ test: Mock `IMediator` thay vì mock nhiều service.
- Thêm Validation/Logging bằng Pipeline Behavior mà không sửa Controller.

### Lợi ích thực tế trong Project
1. **Dễ bảo trì**: Mỗi Handler chỉ làm 1 việc cụ thể.
2. **Dễ mở rộng**: Thêm Command/Query mới không ảnh hưởng code cũ.
3. **Tách biệt layer**: Controller không biết gì về Repository/Database.
4. **Testable**: Test từng Handler riêng lẻ, không cần chạy toàn bộ app.

### FluentValidation
Thư viện bổ trợ cho MediatR, giúp viết logic kiểm tra dữ liệu bằng cú pháp mạch lạc (Fluent Interface), tách biệt logic validate khỏi logic nghiệp vụ.

---
## 4. Tại Sao Lại Sử Dụng `ToDoItemConfiguration.cs`?

Đây là file config cho **Entity Framework Core** (nam trong folder `Infrastructure/Configurations`).

- **Mục đích**: Tách biệt logic cấu hình Database (độ dài cột, tên bảng, khóa ngoại, quan hệ) ra khỏi class Entity (`TodoItem.cs`).
- **Lợi ích**:
  - Giữ cho Domain Entity (`TodoItem`) sạch sẽ ("Pure"), không dính dáng đến các attribute của database (`[MaxLength]`, `[Table]`).
  - Dễ quản lý và đọc code cấu hình ở một nơi tập trung.
  - Sử dụng Fluent API mạnh mẽ hơn Data Annotations.

---
config dùng để mô tả model 

## 5. Khi Nào Dùng Command Validate, Vì Sao Chạy Được & Chạy Như Thế Nào?

### Khi nào dùng?
Dùng để kiểm tra tính đúng đắn của dữ liệu đầu vào từ người dùng (Format email, độ dài chuỗi, bắt buộc nhập...) **trước khi** đi vào xử lý nghiệp vụ chính.

### Vì sao chạy được?
Nó chạy được nhờ cơ chế **Dependency Injection (DI)** và **Reflection**:
1. `builder.Services.AddValidatorsFromAssembly(...)` trong `Program.cs` sẽ quét toàn bộ Project Application để tìm các class kế thừa `AbstractValidator<T>`.
2. Nó đăng ký các validator này vào container của DI.

### Chạy như thế nào?
Có 2 cách:
1.  **Thủ công (Hiện tại có thể đang dùng cách này hoặc chưa dùng)**: Inject `IValidator<CreateUserCommand>` vào Handler và gọi `validator.ValidateAsync(command)`.
2.  **Tự động (Khuyên dùng)**: Sử dụng `IPipelineBehavior` của MediatR. Khi đó, mỗi khi có request, Behavior sẽ chặn lại, tìm validator tương ứng trong DI, validate xong mới cho đi tiếp vào Handler.

---

## 6. Tại Sao Cần Sử Dụng Method Ở Mỗi Đầu API (Attributes)

Ví dụ: `[HttpGet]`, `[HttpPost("login")]`, `[Authorize]`.

- **Mục đích**:
  - **Routing**: Định nghĩa đường dẫn URL và phương thức HTTP (GET, POST, PUT, DELETE) cho action đó. Nếu không có, .NET sẽ không biết request nào vào hàm nào.
  - **Metadata**: Cung cấp thông tin thêm cho Swagger để sinh tài liệu API.
  - **Middleware/Filter**: Các attribute như `[Authorize]` kích hoạt middleware kiểm tra quyền truy cập trước khi hàm chạy.

- **Tự xây attribute**: Bạn có thể tạo các `ActionFilterAttribute` để tự động log, xử lý transaction, hoặc validate header cho riêng một số API. (Ví dụ trong code đã có `GlobalExceptionFilter` nhưng đó là filter toàn cục).

---

## 7. Lưu Ý Thư Viện Khi Cài Mọi Thư Viện, Xem Nó Được Cài Ở Đâu

Khi cài đặt thư viện (NuGet Package), cần chú ý cài **đúng Layer**:

- **Domain**: Hầu như **KHÔNG** cài thư viện (Nên là Pure C#).
- **Application**: Cài `MediatR`, `FluentValidation`, `AutoMapper`. (Chứa logic ứng dụng).
- **Infrastructure**: Cài `Microsoft.EntityFrameworkCore.SqlServer/PostgreSQL`, các thư viện giao tiếp bên thứ 3 (Email, Payment).
- **WebAPI**: Cài `MediatR` (để đăng ký), `Swashbuckle` (Swagger).

Cách kiểm tra: Mở file `.csproj` của từng project hoặc nhìn vào `Dependencies/Packages` trong Solution Explorer.

---

## 8. Tạo Migration & Kiểm Tra File DB

### Lệnh tạo Migration
```bash
dotnet ef migrations add <TenMigration> --project TodoApp.Infrastructure --startup-project TodoApp.WebAPI
```

### Xem kỹ file DB truyền vào
Khi mở file migration sinh ra (trong folder `Migrations`), cần kiểm tra:
1.  **Phương thức `Up()`**: Xem các lệnh `CreateTable`, `AddColumn` có đúng dự kiến không.
2.  **Thuộc tính ID**:
    - Trong code (`User.cs`): `public int UserId { get; private set; }`.
    - Trong migration (`...InitialCreate.cs`):
      ```csharp
      UserId = table.Column<int>(type: "integer", nullable: false)
          .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
      ```
    - Dòng `IdentityByDefaultColumn` (với PostgreSQL) hoặc `SqlServer:Identity` (với SQL Server) chứng tỏ DB sẽ tự động tăng ID (Auto Increment). Điều này xác nhận ID được cấu hình đúng.

### Kiểm tra Migration đã chạy chưa
1.  Kiểm tra trong Database thực tế (bảng `__EFMigrationsHistory` lưu danh sách các migration đã chạy).
2.  Chạy lệnh `dotnet ef database update` để apply các migration chưa chạy.



