import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor để thêm token
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

// Response interceptor để xử lý errors
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
      window.location.href = '/login'
    }
    return Promise.reject(error)
  }
)

// Auth API
export const authAPI = {
  login: (email: string, password: string) =>
    api.post('/api/auth/login', { email, password }),
}

// Orders API
export const ordersAPI = {
  getAll: () => api.get('/api/orders'),
  getById: (id: number) => api.get(`/api/orders/${id}`),
  create: (data: unknown) => api.post('/api/orders', data),
  confirm: (id: number) => api.post(`/api/orders/${id}/confirm`),
  ship: (id: number, trackingNumber?: string) =>
    api.post(`/api/orders/${id}/ship`, { trackingNumber }),
  cancel: (id: number, reason: string) =>
    api.post(`/api/orders/${id}/cancel`, { reason }),
}
