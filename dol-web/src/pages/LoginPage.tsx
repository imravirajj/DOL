import { useState } from "react";
import axios from "axios";
import { Link, useNavigate } from "react-router-dom";
import { useAuth, ROLE_PRESETS, type PresetRole } from "../context/AuthContext";
import { isInvalidCredentialsError, getApiErrorMessage } from "../api/apiErrorHandler";
import { showSuccessToast } from "../services/toastService";

export default function LoginPage() {
  const navigate = useNavigate();
  const { login, loginWithRolePreset } = useAuth();

  const [email, setEmail] = useState("admin@dol.com");
  const [password, setPassword] = useState("Admin@123");
  const [selectedRole, setSelectedRole] = useState<PresetRole>("SuperAdmin");

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
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = async (event: React.FormEvent) => {
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

  return (
    <div className="auth-page">
      <div className="auth-page-glow-1" />
      <div className="auth-page-glow-2" />

      <div className="auth-card auth-card-role-enhanced">
        <div className="auth-header">
          <div className="odl-auth-brand-row">
            <div className="brand-logo-icon">
              <span className="brand-logo-glow" />
              <span className="brand-logo-text">ODL</span>
            </div>
            <div className="odl-brand-title-wrap">
              <span className="odl-brand-title">ONE DEALER LANE</span>
              <span className="odl-brand-sub">ENTERPRISE AUTOMOTIVE CLOUD</span>
            </div>
            <span className="odl-version-badge">
              <span className="pulse-dot" />
              v1.0.1 • 144 APIs
            </span>
          </div>

          <h1>Dealership Sign In</h1>
          <p>Accelerating showroom performance, digital desking, and real-time inventory.</p>
        </div>

        {/* ── Role Preset Selection Bar ── */}
        <div className="role-preset-section">
          <div className="role-preset-label-row">
            <label className="role-preset-label">SELECT OPERATIONAL ROLE FOR INSTANT DEMO:</label>
            <span className="role-preset-hint">5 Pre-configured Personas</span>
          </div>

          <div className="role-preset-grid">
            {(Object.keys(ROLE_PRESETS) as PresetRole[]).map((key) => {
              const preset = ROLE_PRESETS[key];
              const isSelected = selectedRole === key;
              return (
                <button
                  key={key}
                  type="button"
                  onClick={() => handleRoleSelect(key)}
                  className={`role-preset-btn ${isSelected ? "active" : ""}`}
                >
                  <div className="role-preset-header">
                    <span className="role-badge-pill" style={{ background: preset.color }}>
                      {preset.badge}
                    </span>
                    {isSelected && <span className="role-active-check">✓</span>}
                  </div>
                  <div className="role-info">
                    <strong>{preset.name}</strong>
                    <small>{preset.desc}</small>
                  </div>
                </button>
              );
            })}
          </div>

          <button
            type="button"
            className="quick-role-submit-btn"
            disabled={loading}
            onClick={() => handleQuickRoleLogin(selectedRole)}
          >
            {loading ? "Authenticating Session..." : `⚡ Instant One-Click Sign In as ${ROLE_PRESETS[selectedRole].badge}`}
          </button>
        </div>

        <div className="auth-divider">
          <span>Or sign in with custom credentials</span>
        </div>

        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit} className="auth-form">
          <div className="auth-field">
            <label htmlFor="login-email">Email Address</label>
            <input
              id="login-email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Enter email"
              autoComplete="email"
              required
            />
          </div>

          <div className="auth-field">
            <div className="auth-label-row">
              <label htmlFor="login-password">Password</label>
              <Link to="/forgot-password">Forgot password?</Link>
            </div>
            <input
              id="login-password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="Enter password"
              autoComplete="current-password"
              required
            />
          </div>

          <button type="submit" disabled={loading} className="login-submit-btn">
            {loading ? "Signing In..." : "Sign In to Workspace"}
          </button>
        </form>

        <p className="auth-switch">
          Need a customer account? <Link to="/register">Register Buyer</Link>
        </p>
      </div>
    </div>
  );
}
