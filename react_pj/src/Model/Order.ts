export interface OrderDetail {
  idOrderDetail: number
  idBook: number
  bookName: string | null
  quantity: number
  price: number
  subtotal: number
}

export interface Order {
  idOrder: number
  idUser: number
  totalPrice: number
  status: 'Pending' | 'Confirmed' | 'Shipping' | 'Delivered' | 'Cancelled'
  note: string | null
  createdAt: string
  updatedAt: string | null
  orderDetails: OrderDetail[]
}

export interface Notification {
  id: string
  type: string
  orderId: number
  userId: number
  totalAmount: number
  itemCount: number
  totalQuantity: number
  timestamp: string
  message: string
  url: string
  read: boolean
}
