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
    <div className="odl-landing-wrapper flex-center">
      <div className="login-dedicated-container">
        {/* Top Branding */}
        <div className="login-brand-header">
          <Link to="/" title="Back to Dealer One Lane Homepage">
            <svg xmlns="http://www.w3.org/2000/svg" width="140" height="44" viewBox="0 0 136 42" fill="none" className="login-logo-svg">
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
          <span className="login-tagline">Cloud Dealership Operating System</span>
        </div>

        {/* Dedicated Sign-In Card */}
        <div className="odl-modal-card dedicated-login-card">
          <div className="odl-modal-header text-center">
            <span className="modal-badge-eyebrow">DEALER ONE LANE (DOL) • v1.0.1</span>
            <h2>Dealership Workspace Sign In</h2>
            <p>Select a 1-click test role preset or enter your employee credentials.</p>
          </div>

          {/* 1-Click Role Presets Grid */}
          <div className="odl-role-presets-container">
            <span className="preset-group-title">⚡ 1-CLICK ROLE PRESETS:</span>
            <div className="odl-role-presets-grid">
              {(Object.keys(ROLE_PRESETS) as PresetRole[]).map((roleKey) => {
                const preset = ROLE_PRESETS[roleKey];
                const isSelected = selectedRole === roleKey;
                return (
                  <button
                    key={roleKey}
                    type="button"
                    className={`odl-role-chip ${isSelected ? "selected" : ""}`}
                    onClick={() => handleRoleSelect(roleKey)}
                    onDoubleClick={() => handleQuickRoleLogin(roleKey)}
                  >
                    <div className="role-chip-top">
                      <strong className="role-name">{preset.badge}</strong>
                    </div>
                    <small className="role-desc">{preset.desc}</small>
                    <span className="role-action-hint">Double-click to instant login &rarr;</span>
                  </button>
                );
              })}
            </div>
          </div>

          {error && (
            <div className="odl-alert-box error">
              <span className="alert-icon">⚠️</span>
              <span>{error}</span>
            </div>
          )}

          {/* Credential Form */}
          <form onSubmit={handleSubmit} className="odl-login-form">
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
              <label htmlFor="login-password">Password</label>
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
                ⚡ Instant Login as {ROLE_PRESETS[selectedRole].badge}
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

          {/* Navigation Links */}
          <div className="login-card-footer">
            <Link to="/" className="text-link">
              &larr; Back to Dealer One Lane Homepage
            </Link>
            <div className="modal-footer-telematics">
              <span className="pulse-dot" />
              <span>Identity Server Live • 144 APIs Connected • v1.0.1</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
