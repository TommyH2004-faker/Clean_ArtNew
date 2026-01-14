import React from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Container,
  Grid,
  Card,
  CardContent,
  Typography,
  Button,
} from '@mui/material';
import ShoppingCartIcon from '@mui/icons-material/ShoppingCart';
import PackageIcon from '@mui/icons-material/Inventory';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import TrendingUpIcon from '@mui/icons-material/TrendingUp';

const AdminPage: React.FC = () => {
  const navigate = useNavigate();

  const stats = [
    {
      title: 'Tổng đơn hàng',
      value: '156',
      change: '+12%',
      icon: ShoppingCartIcon,
      color: '#2196f3',
    },
    {
      title: 'Chờ xác nhận',
      value: '23',
      change: '+5',
      icon: PackageIcon,
      color: '#ff9800',
    },
    {
      title: 'Hoàn thành',
      value: '98',
      change: '+18%',
      icon: CheckCircleIcon,
      color: '#4caf50',
    },
    {
      title: 'Doanh thu',
      value: '45.2M',
      change: '+23%',
      icon: TrendingUpIcon,
      color: '#9c27b0',
    },
  ];

  return (
    <Container maxWidth="xl" sx={{ mt: 4, mb: 4 }}>
      <Box sx={{ mb: 4 }}>
        <Typography variant="h4" gutterBottom fontWeight="bold">
          Dashboard
        </Typography>
        <Typography variant="body1" color="text.secondary">
          Tổng quan hệ thống quản lý đơn hàng
        </Typography>
      </Box>

      {/* Stats Grid */}
      <Grid container spacing={3} sx={{ mb: 4 }}>
        {stats.map((stat) => (
          <Grid item xs={12} sm={6} md={3} key={stat.title}>
            <Card>
              <CardContent>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
                  <Box>
                    <Typography variant="body2" color="text.secondary" gutterBottom>
                      {stat.title}
                    </Typography>
                    <Typography variant="h4" fontWeight="bold" gutterBottom>
                      {stat.value}
                    </Typography>
                    <Typography variant="body2" color="success.main">
                      {stat.change}
                    </Typography>
                  </Box>
                  <Box
                    sx={{
                      backgroundColor: stat.color,
                      borderRadius: 2,
                      p: 1.5,
                    }}
                  >
                    <stat.icon sx={{ color: 'white', fontSize: 32 }} />
                  </Box>
                </Box>
              </CardContent>
            </Card>
          </Grid>
        ))}
      </Grid>

      {/* Quick Actions */}
      <Card sx={{ mb: 4 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom fontWeight="bold">
            Thao tác nhanh
          </Typography>
          <Grid container spacing={2} sx={{ mt: 1 }}>
            <Grid item xs={12} md={4}>
              <Button
                variant="contained"
                fullWidth
                startIcon={<ShoppingCartIcon />}
                onClick={() => navigate('/admin/orders')}
              >
                Xem đơn hàng
              </Button>
            </Grid>
            <Grid item xs={12} md={4}>
              <Button variant="outlined" fullWidth startIcon={<PackageIcon />}>
                Quản lý sản phẩm
              </Button>
            </Grid>
            <Grid item xs={12} md={4}>
              <Button variant="outlined" fullWidth startIcon={<TrendingUpIcon />}>
                Xem báo cáo
              </Button>
            </Grid>
          </Grid>
        </CardContent>
      </Card>

      {/* Recent Activity */}
      <Card>
        <CardContent>
          <Typography variant="h6" gutterBottom fontWeight="bold">
            Hoạt động gần đây
          </Typography>
          <Box sx={{ mt: 2 }}>
            {[1, 2, 3, 4].map((i) => (
              <Box
                key={i}
                sx={{
                  display: 'flex',
                  alignItems: 'center',
                  gap: 2,
                  p: 1.5,
                  mb: 1,
                  backgroundColor: '#f5f5f5',
                  borderRadius: 1,
                }}
              >
                <Box
                  sx={{
                    width: 8,
                    height: 8,
                    borderRadius: '50%',
                    backgroundColor: '#2196f3',
                  }}
                />
                <Box sx={{ flexGrow: 1 }}>
                  <Typography variant="body2" fontWeight="medium">
                    Đơn hàng #{1000 + i} đã được tạo
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    {i} phút trước
                  </Typography>
                </Box>
              </Box>
            ))}
          </Box>
        </CardContent>
      </Card>
    </Container>
  );
};

export default AdminPage;
