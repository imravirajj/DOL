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

export interface User {
  id: string;
  fullName: string;
  firstName?: string;
  lastName?: string;
  email: string;
  phoneNumber?: string;
  status?: string;
  roles?: string[];
  companyId?: string | null;
  branchId?: string | null;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt?: string;
  user?: User;
}
