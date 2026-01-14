import React, { useEffect } from 'react';
import { Routes, Route } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import Header from './layout/Header';
import LoginPage from './pages/LoginPage';
import AdminPage from './pages/AdminPage';
import OrdersPage from './pages/OrdersPage';
import OrderDetailPage from './pages/OrderDetailPage';
import { initializeSignalR, disconnectSignalR } from './Api/signalr';
import { useNotificationStore } from './hook/useNotification';
import type { Notification } from './Model/Order';
import './App.css';

const Home: React.FC = () => (
  <div className="flex flex-col items-center justify-center min-h-screen">
    <h1 className="text-3xl font-bold mb-4">Welcome to BookStore</h1>
    <p className="text-gray-600">Quản lý cửa hàng sách của bạn!</p>
  </div>
);

function App() {
  const { addNotification } = useNotificationStore();

  useEffect(() => {
    const token = localStorage.getItem('token');
    const user = localStorage.getItem('user');
    
    if (token && user) {
      const userData = JSON.parse(user);
      if (userData.role === 'Admin') {
        // Initialize SignalR for admin users
        initializeSignalR((data) => {
          addNotification(data as Omit<Notification, "id" | "read">);
        });

        return () => {
          disconnectSignalR();
        };
      }
    }
  }, [addNotification]);

  return (
    <>
      <Header />
      <Routes>
        <Route path="/admin" element={<AdminPage />} />
        <Route path="/admin/orders" element={<OrdersPage />} />
        <Route path="/admin/orders/:id" element={<OrderDetailPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/" element={<Home />} />
      </Routes>
      <Toaster
        position="top-right"
        toastOptions={{
          duration: 4000,
          style: {
            background: '#333',
            color: '#fff',
          },
        }}
      />
    </>
  );
}

export default App;
