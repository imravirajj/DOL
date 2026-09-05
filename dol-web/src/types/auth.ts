export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  password: string;
  phoneNumber: string;
  roleName?: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt?: string;
}

export interface User {
  id: string;
  fullName: string;
  email: string;
  roles?: string[];
}
