import axios from "axios";
import type {
  AxiosRequestConfig,
  AxiosError,
  InternalAxiosRequestConfig,
} from "axios";
import {
  getApiErrorMessage,
  isInvalidCredentialsError,
} from "./apiErrorHandler";
import { showToast } from "../services/toastService";

declare module "axios" {
  interface AxiosRequestConfig {
    suppressGlobalError?: boolean;
  }
}

interface RetriableRequestConfig
  extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

interface AuthTokenPayload {
  accessToken?: string;
  refreshToken?: string;
  AccessToken?: string;
  RefreshToken?: string;
}

const ACCESS_TOKEN_KEY = "accessToken";
const REFRESH_TOKEN_KEY = "refreshToken";

export const getAccessToken = () =>
  localStorage.getItem(ACCESS_TOKEN_KEY);

export const getRefreshToken = () =>
  localStorage.getItem(REFRESH_TOKEN_KEY);

export const setAuthTokens = (
  accessToken: string,
  refreshToken: string
) => {
  if (!accessToken || !refreshToken) {
    clearAuthTokens();
    throw new Error("Authentication response did not include tokens.");
  }

  localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
};

export const clearAuthTokens = () => {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
};

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

const shouldShowGlobalError = (
  error: AxiosError,
  config?: AxiosRequestConfig
) => {
  if (config?.suppressGlobalError) {
    return false;
  }

  if (isInvalidCredentialsError(error)) {
    return false;
  }

  if (
    error.response?.status === 401 &&
    !config?.url?.includes("/auth/")
  ) {
    return false;
  }

  return true;
};

apiClient.interceptors.request.use((config) => {
  const token = getAccessToken();

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const originalRequest = error.config as
      | RetriableRequestConfig
      | undefined;
    const accessToken = getAccessToken();
    const refreshToken = getRefreshToken();

    if (
      error.response?.status !== 401 ||
      !originalRequest ||
      originalRequest._retry ||
      originalRequest.url?.includes("/auth/")
    ) {
      if (shouldShowGlobalError(error, originalRequest)) {
        showToast(getApiErrorMessage(error));
      }

      return Promise.reject(error);
    }

    if (!accessToken || !refreshToken) {
      clearAuthTokens();
      showToast("Your session has expired. Please log in again.");
      return Promise.reject(error);
    }

    originalRequest._retry = true;

    try {
      const response = await axios.post(
        `${import.meta.env.VITE_API_BASE_URL}/auth/refresh-token`,
        {
          accessToken,
          refreshToken,
        },
        {
          headers: {
            "Content-Type": "application/json",
          },
        }
      );

      const tokens = response.data as AuthTokenPayload;
      const newAccessToken =
        tokens.accessToken ?? tokens.AccessToken;
      const newRefreshToken =
        tokens.refreshToken ?? tokens.RefreshToken;

      setAuthTokens(newAccessToken ?? "", newRefreshToken ?? "");

      originalRequest.headers.Authorization =
        `Bearer ${newAccessToken}`;

      return apiClient(originalRequest);
    } catch (refreshError) {
      clearAuthTokens();
      showToast("Your session has expired. Please log in again.");
      return Promise.reject(refreshError);
    }
  }
);

export default apiClient;
