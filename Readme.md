# dotnet ef migrations add InitCreate --startup-project ..\TodoApp.WebAPI
# dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.3

# 

# Remove-Item -Path "Migrations" -Recurse -Force; 
# dotnet ef migrations add CreateBookGenreTable --startup-project ..\TodoApp.WebAPI

# chay trong infras 
dotnet ef migrations add FixBase --startup-project ..\TodoApp.WebAPI

# drop database 
dotnet ef database drop --startup-project ..\TodoApp.WebAPI
# xoa thu muc migration 
rm -r Migrations
# trong truong hop bao loi nua mà muốn giữ lại dữ liệu ta sẽ vào migration vừa tạo đó xoá câu lệnh các bảng exist đi 
 # example : migrationBuilder.CreateTable(
# name: "Genres",dotnet ef database update --startup-project ..\TodoApp.WebAPI
#   ...
# );

# dotnet ef migrations add DeleteIdUser --startup-project ..\TodoApp.WebAPI 


# // JWT : nuget tu lay thu vien trung cho net 8.0 
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package System.IdentityModel.Tokens.Jwt

dotnet remove package Microsoft.AspNetCore.Authentication.JwtBearer

cho WebAPI
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 8.0.22



| Thư viện                        | Dùng để làm gì            | Bắt buộc không            |
| ------------------------------- | ------------------------- | ------------------------- |
| **FluentValidation**            | Viết rule validate        | ✅ BẮT BUỘC                |
| **FluentValidation.AspNetCore** | Tích hợp với ASP.NET Core | ❌ Chỉ cần khi làm Web API |

Request
  ↓
Middleware
  ↓
Controller / Minimal API
  ↓
Application (Result pattern)
  ↓
Controller map Result → HTTP   ✅
  ↓
Response
dotnet ef migrations add FixBook --startup-project ..\TodoApp.WebAPI
dotnet ef migrations add FixConfigUrlImage --startup-project ..\TodoApp.WebAPI
dotnet ef migrations add FixConfigv2 --startup-project ..\TodoApp.WebAPI
dotnet ef database update --startup-project ..\TodoApp.WebAPI
dotnet ef migrations add JwtProperty --startup-project ..\TodoApp.WebAPI
dotnet ef migrations add addColumnRole --startup-project ..\TodoApp.WebAPI
Tóm tắt luồng hoàn chỉnh:
Client gửi request → POST /api/book với JSON body
Controller nhận CreateBookCommand
MediatR.Send(command) được gọi
🔥 ValidationBehavior chặn request, chạy CreateBookCommandValidator
✅ Nếu valid → tiếp tục
❌ Nếu invalid → throw ValidationException → GlobalExceptionFilter bắt và trả về BadRequest
CreateBookHandle.Handle(...) được gọi
Book.Create(...) tạo entity
Repository lưu vào DB
Response trả về client

Lý do:

Kiến trúc cũ (dùng Service Layer):
Controller → Service → Repository → Database

Kiến trúc mới (CQRS với MediatR - BẠN ĐANG DÙNG):
Controller → MediatR → Handler → Repository → Database

So sánh:
Thành phần	    Kiến trúc cũ (Service)	Kiến trúc mới (CQRS)
Business Logic	   BookService	CreateBookHandle, GetBookByIdHandler
Validation	Trong Service	CreateBookCommandValidator (FluentValidation)
Dependency	IBookService	IMediator
Controller gọi	_bookService.CreateAsync()	_mediator.Send(command)
Kết luận:
✅ XÓA ĐƯỢC:

BookService.cs
GenreService.cs
IBookService.cs
IGenreService.cs
✅ GIỮ LẠI:

BookRepository (interface)
BookRepositoryImpl (implementation)
CreateBookHandle, GetBookByIdHandler, ... (handlers)
CreateBookCommand, GetBookByIdQuery, ... (CQRS)






📱 CLIENT GỬI REQUEST
    ↓
    POST /api/books
    Body: {
      "nameBook": "Book Title",
      "author": "Author Name",
      ...
    }
    ↓
┌───────────────────────────────────────────────────────┐
│ 🌐 ASP.NET CORE PIPELINE                             │
├───────────────────────────────────────────────────────┤
│                                                        │
│  1️⃣ BookController.CreateBook()                       │
│     ↓                                                  │
│     var result = await _mediator.Send(command);       │
│                                                        │
└───────────────────────────────────────────────────────┘
    ↓
┌───────────────────────────────────────────────────────┐
│ 🔧 MEDIATR PIPELINE                                   │
├───────────────────────────────────────────────────────┤
│                                                        │
│  2️⃣ ValidationBehavior (PIPELINE BEHAVIOR)            │
│     ├─ Tìm tất cả IValidator<CreateBookCommand>      │
│     │  → Tìm thấy: CreateBookValidation              │
│     │                                                  │
│     ├─ Chạy validation:                               │
│     │  ✅ NameBook: NotEmpty, MaxLength(200)         │
│     │  ✅ NameBook: MustAsync (check trùng DB)       │
│     │  ✅ Author: NotEmpty, MaxLength(100)           │
│     │  ✅ Description: NotEmpty                       │
│     │  ✅ ListPrice: >= 0                             │
│     │  ✅ Quantity: >= 0                              │
│     │                                                  │
│     └─ KẾT QUẢ:                                       │
│        ✅ PASS → Tiếp tục đến Handler                │
│        ❌ FAIL → throw ValidationException           │
│                                                        │
└───────────────────────────────────────────────────────┘
    ↓ (Nếu PASS)
┌───────────────────────────────────────────────────────┐
│ 📦 HANDLER                                            │
├───────────────────────────────────────────────────────┤
│                                                        │
│  3️⃣ CreateBookHandle.Handle()                         │
│     ├─ Book.Create(...)                               │
│     ├─ _bookRepository.AddBookAsync(book)            │
│     └─ return Result<int>.Success(book.IdBook)       │
│                                                        │
└───────────────────────────────────────────────────────┘
    ↓
┌───────────────────────────────────────────────────────┐
│ 🎯 CONTROLLER RESPONSE                                │
├───────────────────────────────────────────────────────┤
│                                                        │
│  4️⃣ return Created(...)                               │
│     Status: 201 Created                               │
│     Body: {                                           │
│       "message": "Tạo sách thành công",              │
│       "bookId": 123                                   │
│     }                                                  │
│                                                        │
└───────────────────────────────────────────────────────┘


📱 CLIENT GỬI REQUEST (DỮ LIỆU SAI)
    ↓
    POST /api/books
    Body: {
      "nameBook": "",  ❌ Empty
      "author": "Very long author name exceeding 100 characters...",  ❌ Quá dài
      ...
    }
    ↓
1️⃣ BookController.CreateBook()
    ↓
2️⃣ ValidationBehavior
    ├─ Chạy CreateBookValidation
    ├─ Phát hiện lỗi:
    │  • NameBook: "NameBook is required."
    │  • Author: "Author must not exceed 100 characters."
    │
    └─ throw new ValidationException(failures)  🔥
    ↓
┌───────────────────────────────────────────────────────┐
│ 🛡️ GLOBAL EXCEPTION FILTER (BẮT LỖI)                │
├───────────────────────────────────────────────────────┤
│                                                        │
│  3️⃣ GlobalExceptionFilter.OnException()               │
│     ├─ if (ValidationException)                       │
│     │  ├─ Chuyển lỗi thành Dictionary:                │
│     │  │  {                                            │
│     │  │    "NameBook": ["NameBook is required."],   │
│     │  │    "Author": ["Author must not exceed..."]  │
│     │  │  }                                            │
│     │  │                                               │
│     │  └─ return BadRequest với format chuẩn         │
│     │                                                  │
│     └─ context.ExceptionHandled = true                │
│                                                        │
└───────────────────────────────────────────────────────┘
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
      "traceId": "..."
    }