import { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { authAPI } from '../Api/api';
import toast from 'react-hot-toast';
import { Box, Container, Typography, CircularProgress, Button } from '@mui/material';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import ErrorIcon from '@mui/icons-material/Error';

export default function ActivatePage() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [status, setStatus] = useState<'loading' | 'success' | 'error'>('loading');
  const [message, setMessage] = useState('');

  useEffect(() => {
    const activateAccount = async () => {
      const code = searchParams.get('code');
      const userId = searchParams.get('userId');

      if (!code || !userId) {
        setStatus('error');
        setMessage('Link kích hoạt không hợp lệ');
        return;
      }

      try {
        const response = await authAPI.activate({
          userId: parseInt(userId),
          activationCode: code,
        });

        setStatus('success');
        setMessage(response.data.message || 'Kích hoạt tài khoản thành công!');
        toast.success('Kích hoạt thành công! Chuyển đến trang đăng nhập...');

        // Redirect sau 3 giây
        setTimeout(() => {
          navigate('/login');
        }, 3000);
      } catch (error: unknown) {
        setStatus('error');
        if (error instanceof Error) {
          setMessage(error.message);
        } else {
          setMessage('Kích hoạt thất bại. Vui lòng thử lại.');
        }
        toast.error('Kích hoạt thất bại');
      }
    };

    activateAccount();
  }, [searchParams, navigate]);

  return (
    <Container maxWidth="sm">
      <Box
        sx={{
          minHeight: '100vh',
          display: 'flex',
          alignItems: 'center',
          justifyContent: 'center',
        }}
      >
        <Box
          sx={{
            textAlign: 'center',
            p: 4,
            borderRadius: 2,
            boxShadow: 3,
            bgcolor: 'background.paper',
          }}
        >
          {status === 'loading' && (
            <>
              <CircularProgress size={60} />
              <Typography variant="h5" sx={{ mt: 3 }}>
                Đang kích hoạt tài khoản...
              </Typography>
            </>
          )}

          {status === 'success' && (
            <>
              <CheckCircleIcon
                sx={{ fontSize: 80, color: 'success.main', mb: 2 }}
              />
              <Typography variant="h4" gutterBottom color="success.main">
                Thành công!
              </Typography>
              <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
                {message}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                Đang chuyển đến trang đăng nhập...
              </Typography>
            </>
          )}

          {status === 'error' && (
            <>
              <ErrorIcon sx={{ fontSize: 80, color: 'error.main', mb: 2 }} />
              <Typography variant="h4" gutterBottom color="error.main">
                Lỗi kích hoạt
              </Typography>
              <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
                {message}
              </Typography>
              <Button
                variant="contained"
                onClick={() => navigate('/login')}
                sx={{ mt: 2 }}
              >
                Về trang đăng nhập
              </Button>
            </>
          )}
        </Box>
      </Box>
    </Container>
  );
}
