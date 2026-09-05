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
      <div className="auth-card auth-card-role-enhanced">
        <div className="auth-header">
          <div style={{ display: "flex", justifyContent: "space-between", alignItems: "center" }}>
            <span className="auth-eyebrow">DealerOneLane Enterprise</span>
            <span style={{ fontSize: "11px", background: "#e0f2fe", color: "#0369a1", padding: "2px 8px", borderRadius: "12px", fontWeight: "600" }}>
              v1.0.1 • 144 APIs
            </span>
          </div>
          <h1>Dealership Sign In</h1>
          <p>Select your operational role below for instant demo login, or enter credentials.</p>
        </div>

        {/* ── Role Preset Selection Bar ── */}
        <div className="role-preset-section">
          <label className="role-preset-label">Quick Role Login (Click to Switch):</label>
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
                  style={{ borderColor: isSelected ? preset.color : undefined }}
                >
                  <span className="role-badge-pill" style={{ background: preset.color }}>
                    {preset.badge.split(" ")[0]}
                  </span>
                  <div className="role-info">
                    <strong style={{ color: isSelected ? preset.color : "#1e293b" }}>{preset.name}</strong>
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
            style={{ background: ROLE_PRESETS[selectedRole].color }}
          >
            {loading ? "Authenticating..." : `⚡ Instant One-Click Login as ${ROLE_PRESETS[selectedRole].badge}`}
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
