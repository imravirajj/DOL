import {
  BrowserRouter,
  Navigate,
  Route,
  Routes,
} from "react-router-dom";

import LandingPage from "../pages/LandingPage";
import LoginPage from "../pages/LoginPage";
import RegisterPage from "../pages/RegisterPage";
import ForgotPasswordPage from "../pages/ForgotPasswordPage";
import ResetPasswordPage from "../pages/ResetPasswordPage";
import DashboardPage from "../pages/DashboardPage";
import CrmSalesPage from "../pages/CrmSalesPage";
import CatalogInventoryPage from "../pages/CatalogInventoryPage";
import OrdersVaultPage from "../pages/OrdersVaultPage";
import FinanceInsurancePage from "../pages/FinanceInsurancePage";
import AftersalesOpsPage from "../pages/AftersalesOpsPage";
import AdminMastersPage from "../pages/AdminMastersPage";
import UserManagementPage from "../pages/UserManagementPage";

import { useAuth } from "../context/AuthContext";

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return <div style={{ padding: "40px", textAlign: "center" }}>Loading workspace...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
}

function SuperAdminRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isLoading, user, isSuperAdmin, isCompanyAdmin } = useAuth();

  if (isLoading) {
    return <div style={{ padding: "40px", textAlign: "center" }}>Checking permissions...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  const hasAccess =
    isSuperAdmin ||
    isCompanyAdmin ||
    user?.roles?.includes("SuperAdmin") ||
    user?.roles?.includes("Admin") ||
    user?.roles?.includes("CompanyAdmin");

  if (!hasAccess) {
    return <Navigate to="/dashboard" replace />;
  }

  return children;
}

export default function AppRoutes() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LandingPage />} />

        {/* Public Auth Routes */}
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/forgot-password" element={<ForgotPasswordPage />} />
        <Route path="/reset-password" element={<ResetPasswordPage />} />

        {/* Protected Operational Domain Views */}
        <Route
          path="/dashboard"
          element={
            <ProtectedRoute>
              <DashboardPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/crm-sales"
          element={
            <ProtectedRoute>
              <CrmSalesPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/inventory"
          element={
            <ProtectedRoute>
              <CatalogInventoryPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/orders"
          element={
            <ProtectedRoute>
              <OrdersVaultPage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/finance"
          element={
            <ProtectedRoute>
              <FinanceInsurancePage />
            </ProtectedRoute>
          }
        />

        <Route
          path="/aftersales"
          element={
            <ProtectedRoute>
              <AftersalesOpsPage />
            </ProtectedRoute>
          }
        />

        {/* Admin & Setup Masters */}
        <Route
          path="/admin-setup"
          element={
            <SuperAdminRoute>
              <AdminMastersPage />
            </SuperAdminRoute>
          }
        />

        <Route
          path="/users"
          element={
            <SuperAdminRoute>
              <UserManagementPage />
            </SuperAdminRoute>
          }
        />

        {/* Fallback */}
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
