import apiClient from "./apiClient";
import type {
  UserManagementDto,
  CreateUserPayload,
  UpdateUserPayload,
  UserFilter,
  PagedResult,
  OEMOption,
  DealerOption,
} from "../types/userManagement";

export const fetchUsers = async (
  filter: UserFilter
): Promise<PagedResult<UserManagementDto>> => {
  const params: Record<string, string | number | boolean> = {
    pageNumber: filter.pageNumber,
    pageSize: filter.pageSize,
  };

  if (filter.searchTerm && filter.searchTerm.trim()) {
    params.searchTerm = filter.searchTerm.trim();
  }

  if (filter.role && filter.role !== "ALL") {
    params.role = filter.role;
  }

  if (filter.isActive !== undefined) {
    params.isActive = filter.isActive;
  }

  const response = await apiClient.get<PagedResult<UserManagementDto>>(
    "/users",
    { params }
  );

  return response.data;
};

export const fetchUserById = async (
  id: string
): Promise<UserManagementDto> => {
  const response = await apiClient.get<UserManagementDto>(`/users/${id}`);
  return response.data;
};

export const createUser = async (
  payload: CreateUserPayload
): Promise<{ id: string }> => {
  const response = await apiClient.post<{ id: string }>("/users", payload);
  return response.data;
};

export const updateUser = async (
  id: string,
  payload: UpdateUserPayload
): Promise<void> => {
  await apiClient.put(`/users/${id}`, payload);
};

export const setUserStatus = async (
  id: string,
  isActive: boolean
): Promise<void> => {
  await apiClient.patch(`/users/${id}/status`, { isActive });
};

export const deleteUser = async (id: string): Promise<void> => {
  await apiClient.delete(`/users/${id}`);
};

export const fetchOEMs = async (): Promise<OEMOption[]> => {
  try {
    const response = await apiClient.get<any>("/oem");
    const data = response.data;
    if (Array.isArray(data)) return data;
    if (Array.isArray(data?.value)) return data.value;
    return [];
  } catch (err) {
    console.error("Failed to load OEMs", err);
    return [];
  }
};

export const fetchDealers = async (
  oemId?: string
): Promise<DealerOption[]> => {
  try {
    const params = oemId ? { oemId } : undefined;
    const response = await apiClient.get<any>("/dealer", { params });
    const data = response.data;
    if (Array.isArray(data)) return data;
    if (Array.isArray(data?.value)) return data.value;
    return [];
  } catch (err) {
    console.error("Failed to load Dealers", err);
    return [];
  }
};

export const fetchDealerAdmins = async (): Promise<UserManagementDto[]> => {
  try {
    const response = await fetchUsers({
      role: "DealerAdmin",
      pageNumber: 1,
      pageSize: 100,
    });
    return response.items || [];
  } catch (err) {
    console.error("Failed to load DealerAdmins", err);
    return [];
  }
};
