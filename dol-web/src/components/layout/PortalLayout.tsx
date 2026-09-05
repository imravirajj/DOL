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
    laneColor?: string;
    badge?: string;
    count?: string;
  }

  interface NavGroup {
    group: string;
    laneTag?: string;
    tagColor?: string;
    items: NavItem[];
  }

  const navGroups: NavGroup[] = [
    {
      group: "Executive & Sales Lane",
      laneTag: "SALES LANE",
      tagColor: "#f26522",
      items: [
        { path: "/dashboard", label: "Executive Analytics", icon: "⚡", badge: "Live", laneColor: "#f26522" },
        { path: "/crm-sales", label: "CRM & Desking Pipeline", icon: "🎯", count: "128", laneColor: "#f26522" },
      ],
    },
    {
      group: "Catalog & Yard Stock",
      laneTag: "STOCK LANE",
      tagColor: "#177ddc",
      items: [
        { path: "/inventory", label: "Model Trims & Yard Bay", icon: "🚗", count: "316", laneColor: "#177ddc" },
      ],
    },
    {
      group: "Fast Lane & Digital Vault",
      laneTag: "FAST LANE",
      tagColor: "#00d2b4",
      items: [
        { path: "/orders", label: "Orders & KYC Vault", icon: "📑", count: "42", laneColor: "#00d2b4" },
      ],
    },
    {
      group: "Menu Lane & F&I",
      laneTag: "MENU LANE",
      tagColor: "#ab7ae0",
      items: [
        { path: "/finance", label: "F&I Ledger & Loans", icon: "💳", badge: "Finance", laneColor: "#ab7ae0" },
        { path: "/aftersales", label: "Service, Warranty & EV", icon: "🛠️", badge: "6 Ops", laneColor: "#10b981" },
      ],
    },
    ...((isSuperAdmin || isCompanyAdmin)
      ? [
          {
            group: "Enterprise & Security",
            laneTag: "ADMIN",
            tagColor: "#94a3b8",
            items: [
              { path: "/admin-setup", label: "Dealership Tenancy & Masters", icon: "🏢", badge: "Multi-Org" },
              { path: "/users", label: "Staff Access & RBAC", icon: "👥" },
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
          <div className="brand-logo-icon">
            <span className="brand-logo-glow" />
            <span className="brand-logo-text">ODL</span>
          </div>
          <div className="brand-text">
            <div className="brand-name-row">
              <strong>ONE DEALER LANE</strong>
            </div>
            <span className="brand-tagline">Connected Dealership OS</span>
          </div>
        </div>

        {/* Current Active Tenant Badge */}
        <div className="tenant-badge-card">
          <div className="tenant-status-dot" />
          <div className="tenant-meta">
            <span className="tenant-title">Apex Powersports Group</span>
            <small className="tenant-sub">Flagship Campus (MUM-BKC-01)</small>
          </div>
          <span className="tenant-chip">HQ</span>
        </div>

        {/* Navigation Sections */}
        <nav className="portal-nav">
          {navGroups.map((grp) => (
            <div key={grp.group} className="nav-group">
              <div className="nav-group-header">
                <span className="nav-group-title">{grp.group}</span>
                {grp.laneTag && (
                  <span className="lane-tag-pill" style={{ color: grp.tagColor, borderColor: `${grp.tagColor}40` }}>
                    {grp.laneTag}
                  </span>
                )}
              </div>
              {grp.items.map((item) => (
                <NavLink
                  key={item.path}
                  to={item.path}
                  className={({ isActive }) => `portal-nav-item ${isActive ? "active" : ""}`}
                >
                  <span className="nav-item-indicator" style={{ background: item.laneColor || "var(--odl-orange)" }} />
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
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
              <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4" />
              <polyline points="16 17 21 12 16 7" />
              <line x1="21" y1="12" x2="9" y2="12" />
            </svg>
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
