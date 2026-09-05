import { useState, useEffect } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth, ROLE_PRESETS, type PresetRole } from "../../context/AuthContext";
import { feedbackApi } from "../../api/dealershipApis";
import { showSuccessToast } from "../../services/toastService";

interface PortalLayoutProps {
  children: React.ReactNode;
}

export default function PortalLayout({ children }: PortalLayoutProps) {
  const navigate = useNavigate();
  const { user, logout, isSuperAdmin, isCompanyAdmin, isBuyer, primaryRole, loginWithRolePreset } = useAuth();

  const [notificationsCount, setNotificationsCount] = useState(3);
  const [switchingRole, setSwitchingRole] = useState(false);

  useEffect(() => {
    feedbackApi.getNotifications()
      .then((items) => {
        const unread = items.filter((n) => !n.isRead).length;
        setNotificationsCount(unread || 0);
      })
      .catch(() => setNotificationsCount(2));
  }, []);

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  const handleQuickSwitch = async (role: PresetRole) => {
    setSwitchingRole(true);
    try {
      await loginWithRolePreset(role);
      showSuccessToast(`Switched view to ${ROLE_PRESETS[role].badge}`);
      navigate("/dashboard");
    } catch (err) {
      console.error(err);
    } finally {
      setSwitchingRole(false);
    }
  };

  interface NavItem {
    path: string;
    label: string;
    icon: string;
    badge?: string;
    count?: string;
  }

  interface NavGroup {
    group: string;
    items: NavItem[];
  }

  const navGroups: NavGroup[] = [
    {
      group: "Core Operations",
      items: [
        { path: "/dashboard", label: "Analytics & Executive", icon: "📊", badge: "Live" },
        { path: "/crm-sales", label: "Sales & CRM Pipeline", icon: "🎯", count: "128" },
        { path: "/inventory", label: "Catalog & Yard Stock", icon: "🚗", count: "316" },
        { path: "/orders", label: "Orders & KYC Vault", icon: "📑", count: "42" },
      ],
    },
    {
      group: "Financial & Desk",
      items: [
        { path: "/finance", label: "Finance & Payments", icon: "💳", badge: "Ledger" },
        { path: "/aftersales", label: "Aftersales, Service & EV", icon: "⚡", badge: "6 Ops" },
      ],
    },
    ...((isSuperAdmin || isCompanyAdmin)
      ? [
          {
            group: "Administration",
            items: [
              { path: "/admin-setup", label: "Admin Masters & Tenancy", icon: "🏢", badge: "Config" },
              { path: "/users", label: "User Access & RBAC", icon: "👥" },
            ],
          },
        ]
      : []),
  ];

  return (
    <div className="portal-container">
      {/* ── Sidebar ── */}
      <aside className="portal-sidebar">
        <div className="portal-brand">
          <div className="brand-logo-icon">DOL</div>
          <div className="brand-text">
            <strong>DealerOneLane</strong>
            <span>Enterprise Cloud</span>
          </div>
        </div>

        {/* Current Active Tenant Badge */}
        <div className="tenant-badge-card">
          <div className="tenant-status-dot" />
          <div className="tenant-meta">
            <span className="tenant-title">Apex Motors India</span>
            <small className="tenant-sub">Mumbai Flagship HQ (MUM-BKC-01)</small>
          </div>
        </div>

        {/* Navigation Sections */}
        <nav className="portal-nav">
          {navGroups.map((grp) => (
            <div key={grp.group} className="nav-group">
              <span className="nav-group-title">{grp.group}</span>
              {grp.items.map((item) => (
                <NavLink
                  key={item.path}
                  to={item.path}
                  className={({ isActive }) => `portal-nav-item ${isActive ? "active" : ""}`}
                >
                  <span className="nav-icon">{item.icon}</span>
                  <span className="nav-label">{item.label}</span>
                  {item.badge && <span className="nav-badge">{item.badge}</span>}
                  {item.count && <span className="nav-count">{item.count}</span>}
                </NavLink>
              ))}
            </div>
          ))}
        </nav>

        {/* User Card & Logout */}
        <div className="portal-user-profile">
          <div className="user-avatar-circle">
            {user?.fullName?.charAt(0) || "U"}
          </div>
          <div className="user-profile-details">
            <strong>{user?.fullName || "Staff Member"}</strong>
            <span className="user-role-tag">{primaryRole}</span>
          </div>
          <button type="button" onClick={handleLogout} className="logout-icon-btn" title="Sign Out">
            🚪
          </button>
        </div>
      </aside>

      {/* ── Main Area ── */}
      <div className="portal-body">
        {/* Top Header */}
        <header className="portal-topbar">
          <div className="topbar-left">
            <div className="breadcrumb-trail">
              <span>Dealership Operations</span>
              <span className="sep">/</span>
              <strong className="current-view">{primaryRole} Workspace</strong>
            </div>
            <div className="live-api-badge">
              <span className="pulse-dot" />
              <strong>144 APIs Live</strong>
              <small>v1.0.1</small>
            </div>
          </div>

          <div className="topbar-right">
            {/* Quick Role Switcher for rapid demo testing */}
            <div className="topbar-role-switcher">
              <span className="switcher-label">Switch Role:</span>
              <select
                disabled={switchingRole}
                value={isSuperAdmin ? "SuperAdmin" : isCompanyAdmin ? "CompanyAdmin" : isBuyer ? "Buyer" : "SalesExecutive"}
                onChange={(e) => handleQuickSwitch(e.target.value as PresetRole)}
                className="role-select-box"
              >
                {(Object.keys(ROLE_PRESETS) as PresetRole[]).map((r) => (
                  <option key={r} value={r}>
                    {ROLE_PRESETS[r].badge} ({ROLE_PRESETS[r].name})
                  </option>
                ))}
              </select>
            </div>

            {/* Notification Bell */}
            <div className="topbar-notification" title="Alerts & Customer Inquiries">
              🔔
              {notificationsCount > 0 && <span className="notif-count">{notificationsCount}</span>}
            </div>

            {/* User Chip */}
            <div className="topbar-user-pill">
              <span className="user-pill-avatar">{user?.fullName?.charAt(0) || "U"}</span>
              <span className="user-pill-email">{user?.email}</span>
            </div>
          </div>
        </header>

        {/* Page View Viewport */}
        <main className="portal-content-scrollable">{children}</main>
      </div>
    </div>
  );
}
