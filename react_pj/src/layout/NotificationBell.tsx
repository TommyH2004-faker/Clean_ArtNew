import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useNotificationStore } from '../hook/useNotification';
import NotificationsIcon from '@mui/icons-material/Notifications';
import { Badge, IconButton, Menu, MenuItem, Typography, Box } from '@mui/material';
import CheckIcon from '@mui/icons-material/Check'
import ClearIcon from '@mui/icons-material/Clear'
import { formatDistanceToNow } from 'date-fns'
import { vi } from 'date-fns/locale'

export default function NotificationBell() {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null)
  const navigate = useNavigate()
  const { notifications, unreadCount, markAsRead, markAllAsRead, clearAll } =
    useNotificationStore()

  const handleClick = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget)
  }

  const handleClose = () => {
    setAnchorEl(null)
  }

  const handleNotificationClick = (notification: any) => {
    markAsRead(notification.id)
    navigate(`/admin/orders/${notification.orderId}`)
    handleClose()
  }

  const handleMarkAllAsRead = () => {
    markAllAsRead()
  }

  return (
    <>
      <IconButton color="inherit" onClick={handleClick}>
        <Badge badgeContent={unreadCount} color="error">
          <NotificationsIcon />
        </Badge>
      </IconButton>

      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleClose}
        PaperProps={{
          style: {
            maxHeight: 500,
            width: 400,
          },
        }}
      >
        <Box sx={{ p: 2, borderBottom: 1, borderColor: 'divider', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h6">🔔 Thông báo ({unreadCount})</Typography>
          <Box>
            {notifications.length > 0 && (
              <>
                <IconButton size="small" onClick={handleMarkAllAsRead} title="Đánh dấu đã đọc tất cả">
                  <CheckIcon fontSize="small" />
                </IconButton>
                <IconButton size="small" onClick={clearAll} title="Xóa tất cả">
                  <ClearIcon fontSize="small" />
                </IconButton>
              </>
            )}
          </Box>
        </Box>

        {notifications.length === 0 ? (
          <MenuItem disabled>
            <Typography variant="body2" color="text.secondary">
              Chưa có thông báo nào
            </Typography>
          </MenuItem>
        ) : (
          notifications.map((notification) => (
            <MenuItem
              key={notification.id}
              onClick={() => handleNotificationClick(notification)}
              sx={{
                backgroundColor: !notification.read ? '#e3f2fd' : 'inherit',
                borderBottom: 1,
                borderColor: 'divider',
                flexDirection: 'column',
                alignItems: 'flex-start',
                '&:hover': {
                  backgroundColor: '#f5f5f5',
                },
              }}
            >
              <Box sx={{ display: 'flex', justifyContent: 'space-between', width: '100%', mb: 0.5 }}>
                <Typography variant="subtitle2" fontWeight="bold">
                  🛒 Đơn hàng #{notification.orderId}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  {formatDistanceToNow(new Date(notification.timestamp), {
                    addSuffix: true,
                    locale: vi,
                  })}
                </Typography>
              </Box>
              <Typography variant="body2" color="text.secondary">
                {notification.message}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                📦 {notification.itemCount} sản phẩm • {notification.totalQuantity} items
              </Typography>
            </MenuItem>
          ))
        )}
      </Menu>
    </>
  )
}
