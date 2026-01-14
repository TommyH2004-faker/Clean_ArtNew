import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { ordersAPI } from '../Api/api';
import toast from 'react-hot-toast';
import type { Order } from '../Model/Order';
import { format } from 'date-fns';
import { vi } from 'date-fns/locale';
import {
  Container,
  Box,
  Typography,
  Button,
  TextField,
  Chip,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  CircularProgress,
  Tabs,
  Tab,
} from '@mui/material';
import VisibilityIcon from '@mui/icons-material/Visibility';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import LocalShippingIcon from '@mui/icons-material/LocalShipping';
import CancelIcon from '@mui/icons-material/Cancel';
import RefreshIcon from '@mui/icons-material/Refresh';
import SearchIcon from '@mui/icons-material/Search';

export default function OrdersPage() {
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [filter, setFilter] = useState<string>('all');
  const [searchTerm, setSearchTerm] = useState('');
  const navigate = useNavigate();

  const fetchOrders = async () => {
    try {
      setLoading(true);
      const response = await ordersAPI.getAll();
      setOrders(response.data.data || response.data);
    } catch (error) {
      console.error('Failed to fetch orders:', error);
      toast.error('Không thể tải danh sách đơn hàng');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchOrders();
  }, []);

  const handleConfirm = async (id: number) => {
    try {
      await ordersAPI.confirm(id);
      toast.success(`Đơn hàng #${id} đã được xác nhận`);
      fetchOrders();
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Xác nhận thất bại';
      toast.error(message);
    }
  };

  const handleShip = async (id: number) => {
    const trackingNumber = prompt('Nhập mã vận đơn (optional):');
    try {
      await ordersAPI.ship(id, trackingNumber || undefined);
      toast.success(`Đơn hàng #${id} đã chuyển sang giao hàng`);
      fetchOrders();
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Cập nhật thất bại';
      toast.error(message);
    }
  };

  const handleCancel = async (id: number) => {
    const reason = prompt('Lý do hủy đơn:');
    if (!reason) return;

    try {
      await ordersAPI.cancel(id, reason);
      toast.success(`Đơn hàng #${id} đã được hủy`);
      fetchOrders();
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Hủy đơn thất bại';
      toast.error(message);
    }
  };

  const getStatusColor = (status: Order['status']): 'warning' | 'info' | 'secondary' | 'success' | 'error' | 'default' => {
    const colors: Record<Order['status'], 'warning' | 'info' | 'secondary' | 'success' | 'error'> = {
      Pending: 'warning',
      Confirmed: 'info',
      Shipping: 'secondary',
      Delivered: 'success',
      Cancelled: 'error',
    };
    return colors[status] || 'default';
  };

  const getStatusText = (status: Order['status']) => {
    const texts = {
      Pending: 'Chờ xác nhận',
      Confirmed: 'Đã xác nhận',
      Shipping: 'Đang giao',
      Delivered: 'Hoàn thành',
      Cancelled: 'Đã hủy',
    };
    return texts[status] || status;
  };

  const filteredOrders = orders
    .filter((order) => filter === 'all' || order.status === filter)
    .filter(
      (order) =>
        order.idOrder.toString().includes(searchTerm) ||
        order.idUser.toString().includes(searchTerm)
    );

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box>
          <Typography variant="h4" gutterBottom fontWeight="bold">
            Quản lý đơn hàng
          </Typography>
          <Typography variant="body1" color="text.secondary">
            Tổng số: {filteredOrders.length} đơn hàng
          </Typography>
        </Box>
        <Button
          variant="outlined"
          startIcon={<RefreshIcon />}
          onClick={fetchOrders}
        >
          Làm mới
        </Button>
      </Box>

      {/* Filters */}
      <Paper sx={{ p: 2, mb: 3 }}>
        <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap', alignItems: 'center' }}>
          <TextField
            placeholder="Tìm theo mã đơn hoặc mã khách hàng..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            size="small"
            sx={{ flexGrow: 1, minWidth: 300 }}
            InputProps={{
              startAdornment: <SearchIcon sx={{ mr: 1, color: 'text.secondary' }} />,
            }}
          />
          <Tabs value={filter} onChange={(_, v) => setFilter(v)}>
            <Tab label="Tất cả" value="all" />
            <Tab label="Chờ xác nhận" value="Pending" />
            <Tab label="Đã xác nhận" value="Confirmed" />
            <Tab label="Đang giao" value="Shipping" />
            <Tab label="Hoàn thành" value="Delivered" />
            <Tab label="Đã hủy" value="Cancelled" />
          </Tabs>
        </Box>
      </Paper>

      {/* Orders Table */}
      <Paper>
        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 8 }}>
            <CircularProgress />
          </Box>
        ) : filteredOrders.length === 0 ? (
          <Box sx={{ p: 8, textAlign: 'center' }}>
            <Typography color="text.secondary">Không có đơn hàng nào</Typography>
          </Box>
        ) : (
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Mã đơn</TableCell>
                  <TableCell>Khách hàng</TableCell>
                  <TableCell align="right">Tổng tiền</TableCell>
                  <TableCell>Trạng thái</TableCell>
                  <TableCell>Ngày tạo</TableCell>
                  <TableCell align="center">Hành động</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {filteredOrders.map((order) => (
                  <TableRow key={order.idOrder} hover>
                    <TableCell>
                      <Typography fontWeight="bold">#{order.idOrder}</Typography>
                    </TableCell>
                    <TableCell>User #{order.idUser}</TableCell>
                    <TableCell align="right">
                      <Typography fontWeight="bold">
                        {order.totalPrice.toLocaleString('vi-VN')}đ
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={getStatusText(order.status)}
                        color={getStatusColor(order.status)}
                        size="small"
                      />
                    </TableCell>
                    <TableCell>
                      {format(new Date(order.createdAt), 'dd/MM/yyyy HH:mm', { locale: vi })}
                    </TableCell>
                    <TableCell align="center">
                      <Box sx={{ display: 'flex', gap: 1, justifyContent: 'center' }}>
                        <IconButton
                          size="small"
                          color="primary"
                          onClick={() => navigate(`/admin/orders/${order.idOrder}`)}
                          title="Xem chi tiết"
                        >
                          <VisibilityIcon />
                        </IconButton>
                        {order.status === 'Pending' && (
                          <>
                            <IconButton
                              size="small"
                              color="success"
                              onClick={() => handleConfirm(order.idOrder)}
                              title="Xác nhận"
                            >
                              <CheckCircleIcon />
                            </IconButton>
                            <IconButton
                              size="small"
                              color="error"
                              onClick={() => handleCancel(order.idOrder)}
                              title="Hủy đơn"
                            >
                              <CancelIcon />
                            </IconButton>
                          </>
                        )}
                        {order.status === 'Confirmed' && (
                          <>
                            <IconButton
                              size="small"
                              color="secondary"
                              onClick={() => handleShip(order.idOrder)}
                              title="Giao hàng"
                            >
                              <LocalShippingIcon />
                            </IconButton>
                            <IconButton
                              size="small"
                              color="error"
                              onClick={() => handleCancel(order.idOrder)}
                              title="Hủy đơn"
                            >
                              <CancelIcon />
                            </IconButton>
                          </>
                        )}
                      </Box>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>
    </Container>
  );
}
