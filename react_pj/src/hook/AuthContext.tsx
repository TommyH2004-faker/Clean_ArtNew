import { createContext, useState, useEffect } from "react";

interface AuthContextType {
  isLoggedIn: boolean;
  token: string | null;
  userId: number | null;
  role: string | null;
  login: (data: LoginResponse) => void;
  logout: () => void;
}

interface LoginResponse {
  userId: number;
  token: string;
  role: string;
}

// eslint-disable-next-line react-refresh/only-export-components
export const AuthContext = createContext<AuthContextType>({
  isLoggedIn: false,
  token: null,
  userId: null,
  role: null,
  login: () => {},
  logout: () => {},
});

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [userId, setUserId] = useState<number | null>(null);
  const [role, setRole] = useState<string | null>(null);

  // 👉 Load lại state khi refresh trang
  useEffect(() => {
    const savedToken = localStorage.getItem("token");
    const savedUserId = localStorage.getItem("userId");
    const savedRole = localStorage.getItem("role");
    if (savedToken && savedUserId && savedRole) {
      // eslint-disable-next-line react-hooks/set-state-in-effect
      setToken(savedToken);
      setUserId(Number(savedUserId));
      setRole(savedRole);
    }
  }, []);

  const login = (data: LoginResponse) => {
    setToken(data.token);
    setUserId(data.userId);
    setRole(data.role);
    localStorage.setItem("token", data.token);
    localStorage.setItem("userId", data.userId.toString());
    localStorage.setItem("role", data.role);
  };

  const logout = () => {
    setToken(null);
    setUserId(null);
    setRole(null);
    localStorage.removeItem("token");
    localStorage.removeItem("userId");
    localStorage.removeItem("role");
  };

  return (
    <AuthContext.Provider
      value={{
        isLoggedIn: !!token,
        token,
        userId,
        role,
        login,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}
