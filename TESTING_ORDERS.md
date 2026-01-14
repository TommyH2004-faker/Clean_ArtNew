# 🧪 HƯỚNG DẪN TEST ORDER API

## **📋 SETUP**

### **1. Đăng ký Repository trong Program.cs**
```csharp
builder.Services.AddScoped<IOrderRepository, OrderRepositoryImpl>();
```

### **2. Build và Run**
```bash
cd TodoApp.WebAPI
dotnet build
dotnet run
```

---

## **🎯 TEST SCENARIOS**

### **SCENARIO 1: TẠO ĐỚN HÀNG MỚI**

#### **Request:**
```http
POST https://localhost:7xxx/api/orders
Content-Type: application/json

{
  "idUser": 1,
  "note": "Giao trong giờ hành chính",
  "items": [
    {
      "idBook": 1,
      "quantity": 2,
      "price": 350000
    },
    {
      "idBook": 2,
      "quantity": 1,
      "price": 250000
    }
  ]
}
```

#### **Expected Response (201 Created):**
```json
{
  "message": "Order created successfully",
  "data": {
    "idOrder": 1,
    "idUser": 1,
    "totalPrice": 950000,
    "status": "Pending",
    "note": "Giao trong giờ hành chính",
    "createdAt": "2026-01-14T10:30:00Z",
    "updatedAt": null,
    "orderDetails": [
      {
        "idOrderDetail": 1,
        "idBook": 1,
        "bookName": null,
        "quantity": 2,
        "price": 350000,
        "subtotal": 700000
      },
      {
        "idOrderDetail": 2,
        "idBook": 2,
        "bookName": null,
        "quantity": 1,
        "price": 250000,
        "subtotal": 250000
      }
    ]
  }
}
```

#### **Events Triggered:**
```
✅ [ORDER] Order #1 created by User #1 - 2 items, 3 total quantity
   📦 Book #1: 2 x ₫350,000.00 = ₫700,000.00
   📦 Book #2: 1 x ₫250,000.00 = ₫250,000.00
🗑️ [CACHE] Clearing order cache after creation
📝 [AUDIT] Recording CREATE for Order #1
📧 [NOTIFICATION] Sending email for new order #1
```

---

### **SCENARIO 2: LẤY THÔNG TIN ĐỚN HÀNG**

#### **Request:**
```http
GET https://localhost:7xxx/api/orders/1
```

#### **Expected Response (200 OK):**
```json
{
  "message": "Success",
  "data": {
    "idOrder": 1,
    "idUser": 1,
    "totalPrice": 950000,
    "status": "Pending",
    "note": "Giao trong giờ hành chính",
    "createdAt": "2026-01-14T10:30:00Z",
    "updatedAt": null,
    "orderDetails": [
      {
        "idOrderDetail": 1,
        "idBook": 1,
        "bookName": "Harry Potter",
        "quantity": 2,
        "price": 350000,
        "subtotal": 700000
      }
    ]
  }
}
```

---

### **SCENARIO 3: XÁC NHẬN ĐỚN HÀNG**

#### **Request:**
```http
POST https://localhost:7xxx/api/orders/1/confirm
```

#### **Expected Response (200 OK):**
```json
{
  "message": "Order #1 confirmed successfully"
}
```

#### **Events Triggered:**
```
✅ [ORDER] Order #1 confirmed at 2026-01-14T10:35:00Z
🗑️ [CACHE] Clearing order cache after confirmation
📝 [AUDIT] Recording CONFIRM for Order #1
📧 [NOTIFICATION] Sending email for order confirmation
```

---

### **SCENARIO 4: BẮT ĐẦU GIAO HÀNG**

#### **Request:**
```http
POST https://localhost:7xxx/api/orders/1/ship
Content-Type: application/json

{
  "trackingNumber": "VNP123456789"
}
```

#### **Expected Response (200 OK):**
```json
{
  "message": "Order #1 shipped successfully"
}
```

#### **Events Triggered:**
```
🚚 [ORDER] Order #1 shipped - Tracking: VNP123456789
🗑️ [CACHE] Clearing order cache after shipping
📝 [AUDIT] Recording SHIP for Order #1
📧 [NOTIFICATION] Sending email with tracking number
```

---

### **SCENARIO 5: HỦY ĐỚN HÀNG**

#### **Request:**
```http
POST https://localhost:7xxx/api/orders/1/cancel
Content-Type: application/json

{
  "reason": "Khách hàng yêu cầu hủy"
}
```

#### **Expected Response (200 OK):**
```json
{
  "message": "Order #1 cancelled successfully"
}
```

#### **Events Triggered:**
```
❌ [ORDER] Order #1 cancelled - Reason: Khách hàng yêu cầu hủy
🗑️ [CACHE] Clearing order cache after cancellation
📝 [AUDIT] Recording CANCELLATION for Order #1
📧 [NOTIFICATION] Sending email about cancellation
```

---

## **❌ ERROR CASES**

### **Case 1: Validation Error - Empty Items**
```http
POST https://localhost:7xxx/api/orders
Content-Type: application/json

{
  "idUser": 1,
  "items": []
}
```

**Response (400 Bad Request):**
```json
{
  "status": 400,
  "message": "Order must have at least one item"
}
```

---

### **Case 2: Not Found Error**
```http
GET https://localhost:7xxx/api/orders/999
```

**Response (404 Not Found):**
```json
{
  "message": "Order #999 not found"
}
```

---

### **Case 3: Business Logic Error - Invalid Status Transition**
```http
POST https://localhost:7xxx/api/orders/1/ship
# (Order vẫn ở status Pending, chưa Confirm)
```

**Response (400 Bad Request):**
```json
{
  "message": "Cannot start shipping for order with status Pending"
}
```

---

## **🔍 KIỂM TRA LOGS**

Sau mỗi thao tác, check logs trong terminal:

```
✅ [ORDER] Order #1 created by User #1 at 2026-01-14T10:30:00Z - 2 items, 3 total quantity
   📦 Book #1: 2 x ₫350,000.00 = ₫700,000.00
   📦 Book #2: 1 x ₫250,000.00 = ₫250,000.00
🗑️ [CACHE] Clearing order cache after creation - Order #1
📝 [AUDIT] Recording CREATE for Order #1
📧 [NOTIFICATION] Sending email for new order #1
```

---

## **🗄️ KIỂM TRA DATABASE**

### **Check Order:**
```sql
SELECT * FROM Orders WHERE IdOrder = 1;
```

### **Check OrderDetails:**
```sql
SELECT * FROM OrderDetails WHERE IdOrder = 1;
```

### **Check AuditLogs:**
```sql
SELECT * FROM AuditLogs WHERE EntityType = 'Orders' ORDER BY Timestamp DESC;
```

---

## **📧 KIỂM TRA EMAIL**

Check email inbox (Gmail: hiept81331@gmail.com) để xem:
- ✅ New Order Created email
- ✅ Order Confirmed email
- ✅ Order Shipped email (với tracking)
- ✅ Order Cancelled email

---

## **🎯 POSTMAN COLLECTION**

Tạo collection với các requests trên, test theo thứ tự:

1. **Create Order** → Lấy `idOrder` từ response
2. **Get Order By ID** → Verify data
3. **Confirm Order** → Check status → "Confirmed"
4. **Ship Order** → Check status → "Shipping"
5. **Get Order By ID** → Verify tracking number
6. **(Optional) Cancel Order** → Nếu muốn test cancellation flow

---

## **✅ SUCCESS CRITERIA**

- ✅ Order được tạo với IdOrder > 0
- ✅ OrderDetails tự động được tạo trong DB
- ✅ TotalPrice tính đúng
- ✅ Status transitions: Pending → Confirmed → Shipping → Delivered
- ✅ Events được dispatch (check logs)
- ✅ Cache được clear (check logs)
- ✅ AuditLogs được tạo
- ✅ Email được gửi
- ✅ Error handling đúng với các edge cases

---

Made with ❤️ using CQRS + Event-Driven Architecture
