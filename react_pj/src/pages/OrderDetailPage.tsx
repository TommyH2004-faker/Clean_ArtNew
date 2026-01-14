import { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
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
  Chip,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Grid,
  Card,
  CardContent,
  CircularProgress,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import LocalShippingIcon from '@mui/icons-material/LocalShipping';
import CancelIcon from '@mui/icons-material/Cancel';
import PersonIcon from '@mui/icons-material/Person';
import CalendarTodayIcon from '@mui/icons-material/CalendarToday';
import AttachMoneyIcon from '@mui/icons-material/AttachMoney';
import InventoryIcon from '@mui/icons-material/Inventory';

export default function OrderDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [order, setOrder] = useState<Order | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchOrder = useCallback(async () => {
    try {
      setLoading(true);
      const response = await ordersAPI.getById(Number(id));
      setOrder(response.data.data);
    } catch (error) {
      console.error('Failed to fetch order:', error);
      toast.error('Không thể tải thông tin đơn hàng');
      navigate('/admin/orders');
    } finally {
      setLoading(false);
    }
  }, [id, navigate]);

  useEffect(() => {
    fetchOrder();
  }, [fetchOrder]);

  const handleConfirm = async () => {
    try {
      await ordersAPI.confirm(Number(id));
      toast.success('Đơn hàng đã được xác nhận');
      fetchOrder();
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Xác nhận thất bại';
      toast.error(message);
    }
  };

  const handleShip = async () => {
    const trackingNumber = prompt('Nhập mã vận đơn (optional):');
    try {
      await ordersAPI.ship(Number(id), trackingNumber || undefined);
      toast.success('Đơn hàng đã chuyển sang giao hàng');
      fetchOrder();
    } catch (error) {
      const message = error instanceof Error ? error.message : 'Cập nhật thất bại';
      toast.error(message);
    }
  };

  const handleCancel = async () => {
    const reason = prompt('Lý do hủy đơn:');
    if (!reason) return;

    try {
      await ordersAPI.cancel(Number(id), reason);
      toast.success('Đơn hàng đã được hủy');
      fetchOrder();
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

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: 400 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!order) {
    return null;
  }

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 3 }}>
        <Button
          variant="outlined"
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/admin/orders')}
        >
          Quay lại
        </Button>
        <Box sx={{ flexGrow: 1 }}>
          <Typography variant="h4" fontWeight="bold">
            Đơn hàng #{order.idOrder}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            Tạo lúc {format(new Date(order.createdAt), 'dd/MM/yyyy HH:mm', { locale: vi })}
          </Typography>
        </Box>
        <Chip label={getStatusText(order.status)} color={getStatusColor(order.status)} />
      </Box>

      {/* Actions */}
      {order.status === 'Pending' && (
        <Paper sx={{ p: 2, mb: 3 }}>
          <Box sx={{ display: 'flex', gap: 2 }}>
            <Button
              variant="contained"
              color="success"
              startIcon={<CheckCircleIcon />}
              onClick={handleConfirm}
              fullWidth
            >
              Xác nhận đơn hàng
            </Button>
            <Button
              variant="contained"
              color="error"
              startIcon={<CancelIcon />}
              onClick={handleCancel}
              fullWidth
            >
              Hủy đơn hàng
            </Button>
          </Box>
        </Paper>
      )}

      {order.status === 'Confirmed' && (
        <Paper sx={{ p: 2, mb: 3 }}>
          <Box sx={{ display: 'flex', gap: 2 }}>
            <Button
              variant="contained"
              color="primary"
              startIcon={<LocalShippingIcon />}
              onClick={handleShip}
              fullWidth
            >
              Giao hàng
            </Button>
            <Button
              variant="contained"
              color="error"
              startIcon={<CancelIcon />}
              onClick={handleCancel}
              fullWidth
            >
              Hủy đơn hàng
            </Button>
          </Box>
        </Paper>
      )}

      {/* Order Info */}
      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                <PersonIcon color="action" />
                <Box>
                  <Typography variant="body2" color="text.secondary">
                    Khách hàng
                  </Typography>
                  <Typography variant="h6">User #{order.idUser}</Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                <AttachMoneyIcon color="action" />
                <Box>
                  <Typography variant="body2" color="text.secondary">
                    Tổng tiền
                  </Typography>
                  <Typography variant="h6" fontWeight="bold">
                    {order.totalPrice.toLocaleString('vi-VN')}đ
                  </Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                <CalendarTodayIcon color="action" />
                <Box>
                  <Typography variant="body2" color="text.secondary">
                    Ngày tạo
                  </Typography>
                  <Typography variant="h6">
                    {format(new Date(order.createdAt), 'dd/MM/yyyy', { locale: vi })}
                  </Typography>
                </Box>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Note */}
      {order.note && (
        <Paper sx={{ p: 2, mb: 3 }}>
          <Typography variant="h6" gutterBottom>
            Ghi chú
          </Typography>
          <Typography variant="body1" color="text.secondary">
            {order.note}
          </Typography>
        </Paper>
      )}

      {/* Order Items */}
      <Paper>
        <Box sx={{ p: 2 }}>
          <Typography variant="h6" gutterBottom>
            Chi tiết đơn hàng ({order.orderDetails.length} sản phẩm)
          </Typography>
        </Box>
        <TableContainer>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>Sản phẩm</TableCell>
                <TableCell align="center">Số lượng</TableCell>
                <TableCell align="right">Đơn giá</TableCell>
                <TableCell align="right">Thành tiền</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {order.orderDetails.map((item) => (
                <TableRow key={item.idOrderDetail} hover>
                  <TableCell>
                    <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
                      <InventoryIcon color="action" />
                      <Box>
                        <Typography fontWeight="medium">
                          {item.bookName || `Book #${item.idBook}`}
                        </Typography>
                        <Typography variant="caption" color="text.secondary">
                          ID: {item.idBook}
                        </Typography>
                      </Box>
                    </Box>
                  </TableCell>
                  <TableCell align="center">
                    <Typography fontWeight="medium">{item.quantity}</Typography>
                  </TableCell>
                  <TableCell align="right">
                    {item.price.toLocaleString('vi-VN')}đ
                  </TableCell>
                  <TableCell align="right">
                    <Typography fontWeight="bold">
                      {item.subtotal.toLocaleString('vi-VN')}đ
                    </Typography>
                  </TableCell>
                </TableRow>
              ))}
              <TableRow>
                <TableCell colSpan={3} align="right">
                  <Typography variant="h6" fontWeight="bold">
                    Tổng cộng:
                  </Typography>
                </TableCell>
                <TableCell align="right">
                  <Typography variant="h6" fontWeight="bold" color="primary">
                    {order.totalPrice.toLocaleString('vi-VN')}đ
                  </Typography>
                </TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>
    </Container>
  );
}
