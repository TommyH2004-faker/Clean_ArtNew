# 🔔 HƯỚNG DẪN SETUP REALTIME NOTIFICATION VỚI SIGNALR

## **1. ĐÃ SETUP GÌ?**

### **Backend (C#/.NET):**
✅ Tạo `NotificationHub.cs` - SignalR Hub cho realtime communication
✅ Cập nhật `OrderNotificationHandler.cs` - Gửi cả Email + SignalR
✅ Đăng ký SignalR trong `Program.cs`
✅ Map endpoint `/notificationHub`

### **Frontend (Demo):**
✅ Tạo `admin-realtime-demo.html` - Demo admin dashboard với chuông notification

---

## **2. CÁCH HOẠT ĐỘNG**

```mermaid
User tạo đơn hàng
    ↓
OrderCreatedEvent được raise
    ↓
OrderNotificationHandler xử lý
    ↓
1. SignalR → Gửi realtime đến Admin đang online (Chuông 🔔)
2. Email → Gửi backup email đến admin list
```

---

## **3. NOTIFICATION DATA**

Khi có đơn hàng mới, admin nhận được:

```javascript
{
  Type: "ORDER_CREATED",
  OrderId: 1,
  UserId: 5,
  TotalAmount: 1440000,
  ItemCount: 3,
  TotalQuantity: 12,
  Timestamp: "2026-01-14T10:30:00Z",
  Message: "Đơn hàng mới #1 - ₫1,440,000.00",
  Url: "/admin/orders/1" // Click để xem chi tiết
}
```

---

## **4. FRONTEND INTEGRATION**

### **A. Admin Dashboard (React/Vue/Angular)**

```javascript
// 1. Install SignalR Client
npm install @microsoft/signalr

// 2. Create SignalR Connection
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:7xxx/notificationHub", {
    accessTokenFactory: () => localStorage.getItem("token") // JWT Auth
  })
  .withAutomaticReconnect()
  .build();

// 3. Handle New Order Notification
connection.on("NewOrderNotification", (data) => {
  // Update notification badge
  setUnreadCount(prev => prev + 1);
  
  // Show toast
  toast.success(`Đơn hàng mới #${data.OrderId} - ${data.TotalAmount}đ`);
  
  // Add to notification list
  addNotification({
    id: data.OrderId,
    message: data.Message,
    timestamp: data.Timestamp,
    url: data.Url,
    read: false
  });
  
  // Play sound
  new Audio('/notification.mp3').play();
});

// 4. Start Connection
connection.start()
  .then(() => console.log("✅ SignalR Connected"))
  .catch(err => console.error("❌ Connection Error:", err));
```

### **B. Notification Bell Component (React)**

```jsx
import { useState, useEffect } from 'react';
import * as signalR from '@microsoft/signalr';

function NotificationBell() {
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);
  const [showDropdown, setShowDropdown] = useState(false);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7xxx/notificationHub")
      .withAutomaticReconnect()
      .build();

    connection.on("NewOrderNotification", (data) => {
      setNotifications(prev => [data, ...prev]);
      setUnreadCount(prev => prev + 1);
      
      // Show toast notification
      alert(`🔔 ${data.Message}`);
    });

    connection.start();

    return () => connection.stop();
  }, []);

  return (
    <div className="notification-bell">
      <button onClick={() => setShowDropdown(!showDropdown)}>
        🔔
        {unreadCount > 0 && <span className="badge">{unreadCount}</span>}
      </button>
      
      {showDropdown && (
        <div className="dropdown">
          {notifications.map(notif => (
            <div key={notif.OrderId} onClick={() => window.location.href = notif.Url}>
              <strong>Đơn hàng #{notif.OrderId}</strong>
              <p>{notif.Message}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
```

---

## **5. TEST REALTIME NOTIFICATION**

### **Bước 1: Chạy Backend**
```bash
cd TodoApp.WebAPI
dotnet run
```

### **Bước 2: Mở Admin Dashboard**
- Mở file `admin-realtime-demo.html` trong browser
- Hoặc integrate vào React/Vue app của bạn
- Chờ kết nối SignalR (status sẽ hiện "🟢 Connected")

### **Bước 3: Tạo đơn hàng mới**
```bash
POST https://localhost:7xxx/api/orders
{
  "idUser": 1,
  "items": [
    { "idBook": 1, "quantity": 2 }
  ],
  "note": "Test notification"
}
```

### **Bước 4: Xem kết quả**
✅ Admin dashboard hiện notification realtime
✅ Chuông đỏ hiện số lượng thông báo chưa đọc
✅ Toast popup hiện thông báo
✅ Click notification để xem chi tiết đơn hàng
✅ Email backup được gửi đến admin

---

## **6. CẤU HÌNH ADMIN EMAILS**

Trong `appsettings.json`:

```json
{
  "AdminEmails": [
    "admin1@example.com",
    "admin2@example.com"
  ]
}
```

---

## **7. PRODUCTION CHECKLIST**

✅ Add authentication cho SignalR Hub
✅ Group admins theo role (Admin, SuperAdmin)
✅ Persist notifications vào database
✅ Add "mark as read" functionality
✅ Add pagination cho notification list
✅ Setup CORS cho production domain
✅ Add retry logic cho failed connections
✅ Monitor SignalR connection health

---

## **8. NEXT FEATURES**

- [ ] OrderConfirmedEvent notification
- [ ] OrderShippedEvent notification (với tracking)
- [ ] OrderCancelledEvent notification
- [ ] UserRegisteredEvent notification
- [ ] LowStockAlert notification (sách sắp hết)
- [ ] Push notifications (Firebase/OneSignal)

Made with ❤️ using SignalR + Event-Driven Architecture
