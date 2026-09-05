import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from "react";

import {
  login as loginApi,
  register as registerApi,
  getCurrentUser,
  logout as logoutApi,
} from "../api/authApi";
import {
  clearAuthTokens,
  getAccessToken,
  getRefreshToken,
  setAuthTokens,
} from "../api/apiClient";

import type {
  LoginRequest,
  RegisterRequest,
  User,
} from "../types/auth";

export type PresetRole =
  | "SuperAdmin"
  | "CompanyAdmin"
  | "BranchManager"
  | "SalesExecutive"
  | "Buyer";

export interface RolePresetInfo {
  roleKey: PresetRole;
  email: string;
  name: string;
  badge: string;
  desc: string;
  color: string;
}

export const ROLE_PRESETS: Record<PresetRole, RolePresetInfo> = {
  SuperAdmin: {
    roleKey: "SuperAdmin",
    email: "admin@dol.com",
    name: "Platform SuperAdmin",
    badge: "👑 SuperAdmin",
    desc: "All Tenants & Global Master Setup",
    color: "#7c3aed",
  },
  CompanyAdmin: {
    roleKey: "CompanyAdmin",
    email: "companyadmin@dol.com",
    name: "Apex Motors Owner",
    badge: "🏢 CompanyAdmin",
    desc: "Multi-Branch Dealership Group Oversight",
    color: "#2563eb",
  },
  BranchManager: {
    roleKey: "BranchManager",
    email: "branchmanager@dol.com",
    name: "Mumbai HQ Manager",
    badge: "🏬 BranchManager",
    desc: "Showroom Yard & Team Operations",
    color: "#0891b2",
  },
  SalesExecutive: {
    roleKey: "SalesExecutive",
    email: "sales@dol.com",
    name: "Senior Sales Consultant",
    badge: "💼 SalesExecutive",
    desc: "Leads, Quotes, Test Drives & KYC",
    color: "#059669",
  },
  Buyer: {
    roleKey: "Buyer",
    email: "buyer@dol.com",
    name: "John Car Buyer",
    badge: "🚗 Customer",
    desc: "My Orders, KYC Uploads & Service",
    color: "#d97706",
  },
};

interface AuthContextType {
  user: User | null;
  isAuthenticated: boolean;
  isLoading: boolean;

  login: (request: LoginRequest) => Promise<void>;
  loginWithRolePreset: (role: PresetRole) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  logout: () => Promise<void>;

  // Role helpers
  isSuperAdmin: boolean;
  isCompanyAdmin: boolean;
  isBranchManager: boolean;
  isSalesExecutive: boolean;
  isBuyer: boolean;
  primaryRole: string;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isAuthenticated, setIsAuthenticated] = useState(
    () => !!getAccessToken()
  );

  useEffect(() => {
    const loadCurrentUser = async () => {
      const token = getAccessToken();

      if (!token) {
        setIsAuthenticated(false);
        setIsLoading(false);
        return;
      }

      try {
        const currentUser = await getCurrentUser(true);
        setUser(currentUser);
        setIsAuthenticated(true);
      } catch (error) {
        console.error("Failed to load current user", error);
        clearAuthTokens();
        setUser(null);
        setIsAuthenticated(false);
      } finally {
        setIsLoading(false);
      }
    };

    loadCurrentUser();
  }, []);

  const login = async (request: LoginRequest) => {
    const response = await loginApi(request);
    setAuthTokens(response.accessToken, response.refreshToken);
    if (response.user) {
      setUser(response.user);
    } else {
      const currentUser = await getCurrentUser();
      setUser(currentUser);
    }
    setIsAuthenticated(true);
  };

  const loginWithRolePreset = async (role: PresetRole) => {
    const preset = ROLE_PRESETS[role];
    await login({
      email: preset.email,
      password: "Admin@123",
    });
  };

  const register = async (request: RegisterRequest) => {
    const response = await registerApi({
      ...request,
      roleName: request.roleName || "Buyer",
    });
    setAuthTokens(response.accessToken, response.refreshToken);
    if (response.user) {
      setUser(response.user);
    } else {
      const currentUser = await getCurrentUser();
      setUser(currentUser);
    }
    setIsAuthenticated(true);
  };

  const logout = async () => {
    const token = getRefreshToken();
    if (token) {
      try {
        await logoutApi(token);
      } catch (error) {
        console.error("Failed to revoke refresh token on logout", error);
      }
    }
    clearAuthTokens();
    setUser(null);
    setIsAuthenticated(false);
  };

  const roles = user?.roles || [];
  const isSuperAdmin =
    roles.includes("Admin") ||
    roles.includes("SuperAdmin") ||
    roles.includes("GlobalAdmin");
  const isCompanyAdmin = roles.includes("CompanyAdmin");
  const isBranchManager = roles.includes("BranchManager");
  const isSalesExecutive =
    roles.includes("BranchStaff") || roles.includes("SalesExecutive");
  const isBuyer =
    roles.includes("Buyer") ||
    roles.includes("Customer") ||
    roles.includes("Dealer");

  const primaryRole = isSuperAdmin
    ? "SuperAdmin"
    : isCompanyAdmin
    ? "CompanyAdmin"
    : isBranchManager
    ? "BranchManager"
    : isSalesExecutive
    ? "SalesExecutive"
    : isBuyer
    ? "Customer"
    : roles[0] || "User";

  return (
    <AuthContext.Provider
      value={{
        user,
        isAuthenticated,
        isLoading,
        login,
        loginWithRolePreset,
        register,
        logout,
        isSuperAdmin,
        isCompanyAdmin,
        isBranchManager,
        isSalesExecutive,
        isBuyer,
        primaryRole,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used inside AuthProvider");
  }
  return context;
}
