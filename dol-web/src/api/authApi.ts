import apiClient from "./apiClient";
import type {
  LoginRequest,
  RegisterRequest,
  AuthResponse,
  User,
} from "../types/auth";

interface BackendUserDto {
  id: string;
  firstName?: string;
  lastName?: string;
  fullName?: string;
  email: string;
  phoneNumber?: string;
  status?: string;
  roles?: string[];
  companyId?: string | null;
  branchId?: string | null;
  accessScope?: string | null;
}

interface AuthResponsePayload {
  accessToken?: string;
  refreshToken?: string;
  expiresAt?: string;
  AccessToken?: string;
  RefreshToken?: string;
  ExpiresAt?: string;
  tokens?: {
    accessToken?: string;
    refreshToken?: string;
    expiresAt?: string;
    AccessToken?: string;
    RefreshToken?: string;
    ExpiresAt?: string;
  };
  Tokens?: {
    accessToken?: string;
    refreshToken?: string;
    expiresAt?: string;
    AccessToken?: string;
    RefreshToken?: string;
    ExpiresAt?: string;
  };
  user?: BackendUserDto;
  User?: BackendUserDto;
}

const mapUserDto = (dto?: BackendUserDto): User | undefined => {
  if (!dto) return undefined;
  const fullName =
    dto.fullName ||
    [dto.firstName, dto.lastName].filter(Boolean).join(" ") ||
    dto.email;
  return {
    id: dto.id,
    fullName,
    firstName: dto.firstName,
    lastName: dto.lastName,
    email: dto.email,
    roles: dto.roles || [],
    companyId: dto.companyId,
    branchId: dto.branchId,
    status: dto.status,
    phoneNumber: dto.phoneNumber,
  };
};

const toAuthResponse = (
  payload: AuthResponsePayload
): AuthResponse => {
  const tokenObj = payload.tokens ?? payload.Tokens;
  const rawUser = payload.user ?? payload.User;

  return {
    accessToken:
      payload.accessToken ??
      payload.AccessToken ??
      tokenObj?.accessToken ??
      tokenObj?.AccessToken ??
      "",
    refreshToken:
      payload.refreshToken ??
      payload.RefreshToken ??
      tokenObj?.refreshToken ??
      tokenObj?.RefreshToken ??
      "",
    expiresAt:
      payload.expiresAt ??
      payload.ExpiresAt ??
      tokenObj?.expiresAt ??
      tokenObj?.ExpiresAt,
    user: mapUserDto(rawUser),
  };
};

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
  const names = (request.fullName || "").trim().split(/\s+/);
  const firstName = names[0] || "User";
  const lastName = names.slice(1).join(" ") || "Customer";

  const payload = {
    firstName,
    lastName,
    email: request.email,
    phoneNumber: request.phoneNumber || "0000000000",
    password: request.password,
    role: request.roleName || "Buyer",
  };

  const response = await apiClient.post<AuthResponsePayload>(
    "/auth/register",
    payload
  );

  return toAuthResponse(response.data);
};

export const getCurrentUser = async (
  suppressGlobalError = false
): Promise<User> => {
  try {
    const response = await apiClient.get<BackendUserDto>("/user/profile", {
      suppressGlobalError,
    });
    const mapped = mapUserDto(response.data);
    if (mapped) return mapped;
  } catch {
    // fallback in case of route variation
    const response = await apiClient.get<BackendUserDto>("/users/me", {
      suppressGlobalError,
    });
    const mapped = mapUserDto(response.data);
    if (mapped) return mapped;
  }

  throw new Error("Unable to fetch user profile.");
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
