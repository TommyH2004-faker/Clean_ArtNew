# 📧 Cấu hình SMTP Email Service

## 🔧 **Cấu hình cho Gmail**

### **Bước 1: Bật "App Password"**

1. Truy cập: https://myaccount.google.com/security
2. Tìm mục **"2-Step Verification"** → Bật nếu chưa có
3. Quay lại Security → Tìm **"App passwords"**
4. Chọn **"Mail"** và **"Other"** (đặt tên "TodoApp")
5. Copy **16-ký-tự App Password** (ví dụ: `abcd efgh ijkl mnop`)

### **Bước 2: Cập nhật appsettings.json**

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "Username": "your-email@gmail.com",
    "Password": "abcdefghijklmnop",  // ← 16 ký tự (không có khoảng trắng)
    "FromEmail": "your-email@gmail.com",
    "FromName": "TodoApp Notification"
  },
  "AdminEmails": [
    "admin@gmail.com"  // Email nhận notification
  ]
}
```

---

## 📮 **Cấu hình cho Outlook/Hotmail**

```json
{
  "Smtp": {
    "Host": "smtp.office365.com",
    "Port": "587",
    "Username": "your-email@outlook.com",
    "Password": "your-password",
    "FromEmail": "your-email@outlook.com",
    "FromName": "TodoApp Notification"
  }
}
```

---

## 🧪 **Test Email**

Khi tạo Genre mới, `GenreNotificationHandler` sẽ tự động gửi email:

```
POST /api/genres
{
  "nameGenre": "Test Email Event"
}
```

**Email sẽ được gửi với:**
- **To:** admin@gmail.com (từ config)
- **Subject:** New Genre Created
- **Body:** Genre 'Test Email Event' (ID: 5) was created at 01/13/2026 14:30

---

## 🔍 **Troubleshooting**

### **Lỗi: "SMTP authentication failed"**
- ✅ Kiểm tra Username/Password
- ✅ Dùng **App Password** (không phải password thường)
- ✅ Bật 2-Step Verification

### **Lỗi: "SMTP server requires a secure connection"**
- ✅ Kiểm tra `EnableSsl = true`
- ✅ Port phải là **587** (hoặc 465 cho SSL)

### **Email không gửi được**
1. Check logs trong console
2. Verify Gmail/Outlook account không bị khóa
3. Test với email command line: `telnet smtp.gmail.com 587`

---

## 📊 **Giới hạn gửi email**

| Provider | Giới hạn | Ghi chú |
|----------|----------|---------|
| **Gmail** | 500 email/ngày | Tài khoản free |
| **Gmail (Workspace)** | 2000 email/ngày | Tài khoản trả phí |
| **Outlook** | 300 email/ngày | Tài khoản free |

---

Made with ❤️ using SMTP Email Service
