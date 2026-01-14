import * as signalR from '@microsoft/signalr'
import toast from 'react-hot-toast'

const SIGNALR_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

let connection: signalR.HubConnection | null = null

export const initializeSignalR = (onNotification: (data: unknown) => void) => {
  const token = localStorage.getItem('token')

  connection = new signalR.HubConnectionBuilder()
    .withUrl(`${SIGNALR_URL}/notificationHub`, {
      accessTokenFactory: () => token || '',
    })
    .withAutomaticReconnect()
    .build()

  // Handle new order notification
  connection.on('NewOrderNotification', (data) => {
    console.log('📦 New order notification:', data)
    
    onNotification(data)

    // Show toast
    toast.success(
      `🛒 Đơn hàng mới #${data.orderId} - ${data.totalAmount.toLocaleString('vi-VN')}đ`,
      {
        duration: 5000,
        icon: '🔔',
      }
    )

    // Play notification sound
    playNotificationSound()
  })

  // Connection events
  connection.onreconnecting(() => {
    console.log('🔄 SignalR reconnecting...')
  })

  connection.onreconnected(() => {
    console.log('✅ SignalR reconnected')
  })

  connection.onclose(() => {
    console.log('❌ SignalR connection closed')
  })

  // Start connection
  connection
    .start()
    .then(() => {
      console.log('✅ SignalR connected')
    })
    .catch((err) => {
      console.error('❌ SignalR connection error:', err)
    })

  return connection
}

export const disconnectSignalR = () => {
  if (connection) {
    connection.stop()
    connection = null
  }
}

function playNotificationSound() {
  const audio = new Audio(
    'data:audio/wav;base64,UklGRnoGAABXQVZFZm10IBAAAAABAAEAQB8AAEAfAAABAAgAZGF0YQoGAACBhYqFbF1fdJivrJBhNjVgodDbq2EcBj+a2/LDciUFLIHO8tiJNwgZaLvt559NEAxQp+PwtmMcBjiR1/LMeSwFJHfH8N2QQAoUXrTp66hVFApGn+DyvmwhBSuIzvLZiTYJHmS36+mhTgwOUqng7rhkHAU0j9nyzn0xCCF9zPDadkUIDmaL'
  )
  audio.play().catch(() => {
    // Ignore errors if autoplay is blocked
  })
}
