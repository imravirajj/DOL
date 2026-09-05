import apiClient from "./apiClient";
import type {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  User,
} from "../types/auth";

interface AuthResponsePayload {
  accessToken?: string;
  refreshToken?: string;
  expiresAt?: string;
  AccessToken?: string;
  RefreshToken?: string;
  ExpiresAt?: string;
}

const toAuthResponse = (
  payload: AuthResponsePayload
): AuthResponse => ({
  accessToken: payload.accessToken ?? payload.AccessToken ?? "",
  refreshToken: payload.refreshToken ?? payload.RefreshToken ?? "",
  expiresAt: payload.expiresAt ?? payload.ExpiresAt,
});

export const login = async (
  request: LoginRequest
): Promise<AuthResponse> => {
  const response = await apiClient.post<AuthResponsePayload>(
    "/auth/login",
    request
  );

  return toAuthResponse(response.data);
};

export const register = async (
  request: RegisterRequest
): Promise<AuthResponse> => {
  const response = await apiClient.post<AuthResponsePayload>(
    "/auth/register",
    request
  );

  return toAuthResponse(response.data);
};

export const getCurrentUser = async (
  suppressGlobalError = false
): Promise<User> => {
  const response = await apiClient.get<User>("/users/me", {
    suppressGlobalError,
  });

  return response.data;
};

export const refreshToken = async (
  accessToken: string,
  token: string
): Promise<AuthResponse> => {
  const response = await apiClient.post<AuthResponsePayload>(
    "/auth/refresh-token",
    {
      accessToken,
      refreshToken: token,
    }
  );

  return toAuthResponse(response.data);
};

export const logout = async (token: string): Promise<void> => {
  await apiClient.post(
    "/auth/logout",
    {
      refreshToken: token,
    },
    {
      suppressGlobalError: true,
    }
  );
};
