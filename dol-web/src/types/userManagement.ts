export const ROLES = {
  SuperAdmin: "SuperAdmin",
  OEMAdmin: "OEMAdmin",
  DealerAdmin: "DealerAdmin",
  DealerSalesExecutive: "DealerSalesExecutive",
  Customer: "Customer",
  BankOfficer: "BankOfficer",
  InsuranceOfficer: "InsuranceOfficer",
  RTOOfficer: "RTOOfficer",
  DeliveryExecutive: "DeliveryExecutive",
} as const;

export type RoleType = (typeof ROLES)[keyof typeof ROLES];

export interface UserManagementDto {
  id: string;
  email: string;
  fullName: string;
  phoneNumber?: string;
  isActive: boolean;
  roles: string[];
  createdAt: string;

  // Address Details
  address?: string;
  city?: string;
  state?: string;
  pincode?: string;

  // Role-Specific Details
  oemId?: string;
  dealerId?: string;
  reportingAdminId?: string;
  reportingAdminName?: string;
  bankName?: string;
  branchName?: string;
  employeeId?: string;
  insuranceCompanyName?: string;
  rtoOffice?: string;
  region?: string;
  officerId?: string;
  organizationName?: string;
}

export interface CreateUserPayload {
  fullName: string;
  email: string;
  phoneNumber: string;
  password: string;
  roleName: string;
  isActive: boolean;

  // Address Details
  address?: string;
  city?: string;
  state?: string;
  pincode?: string;

  // Role-Specific Details
  oemId?: string;
  dealerId?: string;
  reportingAdminId?: string;
  bankName?: string;
  branchName?: string;
  employeeId?: string;
  insuranceCompanyName?: string;
  rtoOffice?: string;
  region?: string;
  officerId?: string;
  organizationName?: string;
}

export interface UpdateUserPayload {
  fullName: string;
  phoneNumber: string;
  password?: string;
  roleName: string;
  isActive: boolean;

  // Address Details
  address?: string;
  city?: string;
  state?: string;
  pincode?: string;

  // Role-Specific Details
  oemId?: string;
  dealerId?: string;
  reportingAdminId?: string;
  bankName?: string;
  branchName?: string;
  employeeId?: string;
  insuranceCompanyName?: string;
  rtoOffice?: string;
  region?: string;
  officerId?: string;
  organizationName?: string;
}

export interface UserFilter {
  searchTerm?: string;
  role?: string;
  isActive?: boolean;
  pageNumber: number;
  pageSize: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface OEMOption {
  id: string;
  name: string;
  code: string;
}

export interface DealerOption {
  id: string;
  oemId: string;
  name: string;
  dealerCode: string;
  city?: string;
  state?: string;
}
