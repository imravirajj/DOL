import { useState } from "react";
import axios from "axios";
import { Link, useNavigate } from "react-router-dom";
import { useAuth, ROLE_PRESETS, type PresetRole } from "../context/AuthContext";
import { isInvalidCredentialsError, getApiErrorMessage } from "../api/apiErrorHandler";
import { showSuccessToast, showErrorToast } from "../services/toastService";

interface LoginPageProps {
  initialMode?: "signin" | "signup";
}

export default function LoginPage({ initialMode = "signin" }: LoginPageProps) {
  const navigate = useNavigate();
  const { login, register, loginWithRolePreset } = useAuth();

  const [authMode, setAuthMode] = useState<"signin" | "signup">(initialMode);

  // Sign In state
  const [email, setEmail] = useState("admin@dol.com");
  const [password, setPassword] = useState("Admin@123");
  const [selectedRole, setSelectedRole] = useState<PresetRole>("SuperAdmin");

  // Sign Up state
  const [registerFullName, setRegisterFullName] = useState("");
  const [registerEmail, setRegisterEmail] = useState("");
  const [registerPhone, setRegisterPhone] = useState("");
  const [registerPassword, setRegisterPassword] = useState("");

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleRoleSelect = (roleKey: PresetRole) => {
    setSelectedRole(roleKey);
    setEmail(ROLE_PRESETS[roleKey].email);
    setPassword("Admin@123");
    setError("");
  };

  const handleQuickRoleLogin = async (roleKey: PresetRole) => {
    setError("");
    setLoading(true);
    setSelectedRole(roleKey);
    try {
      await loginWithRolePreset(roleKey);
      showSuccessToast(`Logged in as ${ROLE_PRESETS[roleKey].badge}`);
      navigate("/dashboard");
    } catch (err) {
      console.error(err);
      setError("Failed to login with role preset. Please verify backend is active.");
      showErrorToast("Role login failed. Verify backend services.");
    } finally {
      setLoading(false);
    }
  };

  const handleSignInSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    setLoading(true);

    try {
      await login({ email, password });
      showSuccessToast("Login successful");
      navigate("/dashboard");
    } catch (err) {
      console.error(err);
      if (axios.isAxiosError(err)) {
        if (isInvalidCredentialsError(err)) {
          setError("Invalid email or password.");
        } else {
          setError(getApiErrorMessage(err));
        }
      } else {
        setError("An unexpected error occurred. Please try again.");
      }
    } finally {
      setLoading(false);
    }
  };

  const handleSignUpSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError("");
    setLoading(true);

    try {
      await register({
        fullName: registerFullName,
        email: registerEmail,
        password: registerPassword,
        phoneNumber: registerPhone,
      });
      showSuccessToast("Account created successfully!");
      navigate("/dashboard");
    } catch (err) {
      console.error(err);
      if (axios.isAxiosError(err)) {
        setError(getApiErrorMessage(err));
      } else {
        setError("Registration failed. Please try again.");
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="dol-split-auth-page">
      {/* Background ambient lighting effects */}
      <div className="dol-split-bg-glow left" />
      <div className="dol-split-bg-glow right" />

      <div className="dol-split-auth-container">
        {/* ═══════════════════════════════════════════════════════════ */}
        {/* LEFT COLUMN: DEALER ONE LANE (DOL) BRAND & SYSTEM SHOWCASE */}
        {/* ═══════════════════════════════════════════════════════════ */}
        <div className="dol-split-left-brand">
          <div className="dol-brand-topbar">
            <Link to="/" className="dol-brand-link" title="Dealer One Lane Homepage">
              <svg xmlns="http://www.w3.org/2000/svg" width="160" height="48" viewBox="0 0 136 42" fill="none" className="dol-split-logo-svg">
                <g transform="translate(44, 2)">
                  <rect x="0" y="0" width="48" height="23" rx="11.5" stroke="#ffffff" strokeWidth="2.2" fill="none" />
                  <circle cx="14" cy="11.5" r="7.5" stroke="#f26522" strokeWidth="2.2" fill="none" />
                  <circle cx="34" cy="11.5" r="7.5" stroke="#ffffff" strokeWidth="2.2" fill="none" />
                  <text x="14" y="15.5" fill="#f26522" fontFamily="'Outfit', sans-serif" fontWeight="900" fontSize="10" textAnchor="middle">1</text>
                  <text x="34" y="15.5" fill="#ffffff" fontFamily="'Outfit', sans-serif" fontWeight="900" fontSize="9.5" textAnchor="middle">DOL</text>
                </g>
                <text x="68" y="36" fill="#ffffff" fontFamily="'Outfit', sans-serif" fontWeight="800" fontSize="7.8" letterSpacing="2.6" textAnchor="middle">DEALER ONE LANE</text>
              </svg>
            </Link>
            <span className="dol-version-pill">v1.0.1 ENTERPRISE</span>
          </div>

          <div className="dol-brand-hero-text">
            <span className="dol-eyebrow-accent">CLOUD DEALERSHIP OPERATING SYSTEM</span>
            <h1 className="dol-brand-title">
              Multiple stages.<br />
              <span className="dol-gradient-text">One connected experience.</span>
            </h1>
            <p className="dol-brand-lead">
              Unify showroom desking, real-time vehicle inventory, F&amp;I interactive menus, and lender digital contracts in a single cohesive cloud workspace.
            </p>
          </div>

          {/* Key Value Cards */}
          <div className="dol-feature-cards-grid">
            <div className="dol-feature-mini-card">
              <div className="mini-card-icon orange">⚡</div>
              <div className="mini-card-text">
                <strong>20-Minute Deal Closing</strong>
                <span>From desking calculations to instant digital lender contracts.</span>
              </div>
            </div>

            <div className="dol-feature-mini-card">
              <div className="mini-card-icon teal">🔄</div>
              <div className="mini-card-text">
                <strong>Synchronized Fabric</strong>
                <span>Real-time sync between Showroom CRM, DMS, and Vault.</span>
              </div>
            </div>

            <div className="dol-feature-mini-card">
              <div className="mini-card-icon purple">🛡️</div>
              <div className="mini-card-text">
                <strong>Granular Enterprise RBAC</strong>
                <span>Tailored dashboards for SuperAdmin, Owners, Managers &amp; Sales.</span>
              </div>
            </div>

            <div className="dol-feature-mini-card">
              <div className="mini-card-icon amber">🌐</div>
              <div className="mini-card-text">
                <strong>144+ Production APIs</strong>
                <span>Pre-integrated microservices mesh &amp; verified endpoints.</span>
              </div>
            </div>
          </div>

          {/* Telematics & Navigation Footer */}
          <div className="dol-brand-footer">
            <div className="dol-telemetry-badge">
              <span className="pulse-dot" />
              <span>Identity Service Live (Port 5065) • All Systems Operational</span>
            </div>
            <Link to="/" className="dol-back-home-link">
              &larr; Back to Dealer One Lane Homepage
            </Link>
          </div>
        </div>

        {/* ═══════════════════════════════════════════════════════════ */}
        {/* RIGHT COLUMN: SIGN IN / SIGN UP AUTHENTICATION CARD         */}
        {/* ═══════════════════════════════════════════════════════════ */}
        <div className="dol-split-right-auth">
          <div className="dol-auth-box-card">
            {/* Segmented Auth Toggle Switcher */}
            <div className="dol-auth-segment-switch">
              <button
                type="button"
                className={`segment-btn ${authMode === "signin" ? "active" : ""}`}
                onClick={() => { setAuthMode("signin"); setError(""); }}
              >
                Sign In
              </button>
              <button
                type="button"
                className={`segment-btn ${authMode === "signup" ? "active" : ""}`}
                onClick={() => { setAuthMode("signup"); setError(""); }}
              >
                Sign Up
              </button>
            </div>

            {/* Header for Active Mode */}
            <div className="dol-auth-card-header">
              <h2>{authMode === "signin" ? "Dealership Workspace Sign In" : "Create Dealership Account"}</h2>
              <p>
                {authMode === "signin"
                  ? "Select a 1-click test role preset or enter your credentials."
                  : "Join Dealer One Lane with your employee or customer details."}
              </p>
            </div>

            {error && (
              <div className="odl-alert-box error">
                <span className="alert-icon">⚠️</span>
                <span>{error}</span>
              </div>
            )}

            {authMode === "signin" ? (
              /* ── SIGN IN TAB CONTENT ── */
              <div className="dol-auth-flow-content">
                {/* 1-Click Role Presets */}
                <div className="odl-role-presets-container">
                  <span className="preset-group-title">⚡ 1-CLICK ROLE PRESETS:</span>
                  <div className="odl-role-presets-grid compact">
                    {(Object.keys(ROLE_PRESETS) as PresetRole[]).map((roleKey) => {
                      const preset = ROLE_PRESETS[roleKey];
                      const isSelected = selectedRole === roleKey;
                      return (
                        <button
                          key={roleKey}
                          type="button"
                          disabled={loading}
                          className={`odl-role-chip ${isSelected ? "selected" : ""}`}
                          onClick={() => handleRoleSelect(roleKey)}
                          onDoubleClick={() => handleQuickRoleLogin(roleKey)}
                          title="Click to select, Double-click to instant login"
                        >
                          <div className="role-chip-top">
                            <strong className="role-name">{preset.badge}</strong>
                          </div>
                          <small className="role-desc">{preset.desc}</small>
                        </button>
                      );
                    })}
                  </div>
                </div>

                {/* Form */}
                <form onSubmit={handleSignInSubmit} className="odl-login-form">
                  <div className="form-group">
                    <label htmlFor="login-email">Work Email</label>
                    <input
                      id="login-email"
                      type="email"
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      placeholder="admin@dol.com"
                      required
                    />
                  </div>

                  <div className="form-group">
                    <div className="label-with-link">
                      <label htmlFor="login-password">Password</label>
                      <Link to="/forgot-password" className="subtle-link">
                        Forgot?
                      </Link>
                    </div>
                    <input
                      id="login-password"
                      type="password"
                      value={password}
                      onChange={(e) => setPassword(e.target.value)}
                      placeholder="••••••••"
                      required
                    />
                  </div>

                  <div className="form-actions-row">
                    <button
                      type="button"
                      className="odl-btn-secondary"
                      disabled={loading}
                      onClick={() => handleQuickRoleLogin(selectedRole)}
                    >
                      ⚡ Instant ({ROLE_PRESETS[selectedRole].badge})
                    </button>
                    <button
                      type="submit"
                      className="odl-btn-primary"
                      disabled={loading}
                    >
                      {loading ? "Authenticating..." : "Enter Workspace →"}
                    </button>
                  </div>
                </form>

                <div className="dol-card-bottom-switch">
                  <span>Don't have an account yet?</span>{" "}
                  <button
                    type="button"
                    className="inline-link-btn"
                    onClick={() => { setAuthMode("signup"); setError(""); }}
                  >
                    Create an account &rarr;
                  </button>
                </div>
              </div>
            ) : (
              /* ── SIGN UP TAB CONTENT ── */
              <div className="dol-auth-flow-content">
                <form onSubmit={handleSignUpSubmit} className="odl-login-form">
                  <div className="form-group">
                    <label htmlFor="signup-fullname">Full Name</label>
                    <input
                      id="signup-fullname"
                      type="text"
                      value={registerFullName}
                      onChange={(e) => setRegisterFullName(e.target.value)}
                      placeholder="e.g. Alex Morgan"
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="signup-email">Work Email</label>
                    <input
                      id="signup-email"
                      type="email"
                      value={registerEmail}
                      onChange={(e) => setRegisterEmail(e.target.value)}
                      placeholder="e.g. alex@dealership.com"
                      required
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="signup-phone">Phone Number</label>
                    <input
                      id="signup-phone"
                      type="tel"
                      value={registerPhone}
                      onChange={(e) => setRegisterPhone(e.target.value)}
                      placeholder="+1 (555) 000-0000"
                    />
                  </div>

                  <div className="form-group">
                    <label htmlFor="signup-password">Password</label>
                    <input
                      id="signup-password"
                      type="password"
                      value={registerPassword}
                      onChange={(e) => setRegisterPassword(e.target.value)}
                      placeholder="Min 6 characters with mixed case & symbol"
                      required
                    />
                  </div>

                  <button
                    type="submit"
                    className="odl-btn-primary w-full"
                    disabled={loading}
                  >
                    {loading ? "Creating Account..." : "Create Account & Enter Workspace →"}
                  </button>
                </form>

                <div className="dol-card-bottom-switch">
                  <span>Already have an account?</span>{" "}
                  <button
                    type="button"
                    className="inline-link-btn"
                    onClick={() => { setAuthMode("signin"); setError(""); }}
                  >
                    Sign in to workspace &rarr;
                  </button>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
