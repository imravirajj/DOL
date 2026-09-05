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
  const [activeLaneTab, setActiveLaneTab] = useState<"sales" | "menu" | "fast">("sales");

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
    <div className="odl-landing-wrapper">
      {/* ── Ambient Background Lighting ── */}
      <div className="odl-radial-glow-top" />
      <div className="odl-radial-glow-bottom" />

      {/* ── Top Header Bar (Authentic One Dealer Lane Header) ── */}
      <header className="odl-site-header">
        <div className="odl-header-container">
          <Link to="/" className="odl-brand-logo-link">
            <svg xmlns="http://www.w3.org/2000/svg" width="130" height="42" viewBox="0 0 140 51" fill="none" className="odl-logo-svg">
              <path d="M10.2149 46.3325C10.2219 47.15 10.0216 47.9558 9.63303 48.6731C9.23555 49.4046 8.63266 50.0012 7.90018 50.3878C7.03269 50.8307 6.06866 51.046 5.09704 51.0137C4.11663 51.0448 3.14498 50.8188 2.27691 50.3578C1.55773 49.9648 0.967437 49.369 0.578033 48.6431C0.211178 47.9369 0.0195312 47.1514 0.0195313 46.354C0.0195312 45.5565 0.211178 44.771 0.578033 44.0648C0.972725 43.3356 1.57082 42.7394 2.29814 42.3501C3.15914 41.8885 4.12227 41.6554 5.09704 41.6728C5.85514 41.6599 6.60924 41.7863 7.32256 42.0457C7.92125 42.2642 8.46603 42.6107 8.91951 43.0617C9.33573 43.4853 9.65976 43.992 9.87088 44.5492C10.0948 45.1168 10.2115 45.7216 10.2149 46.3325ZM7.44573 46.3325C7.45063 46.0146 7.40032 45.6984 7.29708 45.398C7.20083 45.1239 7.04729 44.8739 6.84688 44.665C6.63762 44.4521 6.38564 44.2869 6.10787 44.1806C5.78639 44.0582 5.44481 43.9985 5.10129 44.0048C4.78962 43.9892 4.47809 44.0382 4.1859 44.1488C3.89372 44.2594 3.62708 44.4292 3.40241 44.6478C3.00133 45.1147 2.78017 45.7117 2.77939 46.3296C2.77861 46.9475 2.99826 47.5451 3.39816 48.0129C3.62268 48.2317 3.8893 48.4017 4.18152 48.5123C4.47374 48.6229 4.78534 48.6718 5.09704 48.656C5.54002 48.6709 5.97919 48.5689 6.37119 48.3602C6.71226 48.1698 6.99003 47.8819 7.16967 47.5328C7.35669 47.1612 7.45143 46.7493 7.44573 46.3325Z" fill="white"/>
              <path d="M22.4795 50.8796H20.0628L15.3909 45.8212V50.8796H12.7109V41.7959H15.1191L19.791 46.8286V41.7959H22.4965L22.4795 50.8796Z" fill="white"/>
              <path d="M32.5221 50.8796H25.7266V41.7959H32.4116V43.6178H28.4575V45.2853H32.2375V47.1115H28.4575V49.0534H32.5263L32.5221 50.8796Z" fill="white"/>
              <path d="M44.5948 46.5034C44.6296 47.283 44.455 48.0577 44.0894 48.7454C43.7819 49.274 43.3767 49.7383 42.8959 50.1129C42.1051 50.668 41.1521 50.9383 40.1905 50.8802H35.3359V41.7965H39.0735C39.5563 41.7913 40.0389 41.8214 40.5175 41.8865C40.8835 41.9379 41.2444 42.0211 41.5963 42.1351C41.8813 42.2257 42.1561 42.3464 42.416 42.4952C42.6284 42.621 42.8302 42.7644 43.0191 42.9239C43.3309 43.1819 43.6028 43.4853 43.8261 43.8241C44.0702 44.1916 44.2564 44.5951 44.3782 45.0201C44.5238 45.5008 44.5968 46.0008 44.5948 46.5034ZM41.8341 46.3833C41.8663 45.7817 41.7183 45.1842 41.4094 44.6686C41.1717 44.2889 40.8185 43.9972 40.4028 43.837C40.0181 43.6995 39.6132 43.6284 39.2051 43.6269H38.0499V48.9983H39.2051C39.8573 49.0158 40.5001 48.8383 41.0527 48.4882C41.5708 48.1481 41.8299 47.4465 41.8299 46.3833H41.8341Z" fill="white"/>
              <path d="M54.0455 50.8796H47.25V41.7959H53.9393V43.6178H49.9809V45.2853H53.7567V47.1115H49.9809V49.0534H54.0497L54.0455 50.8796Z" fill="white"/>
              <path d="M66.7442 50.8796H63.9623L63.2317 49.0406H59.3201L58.5768 50.8796H55.7949L59.7745 41.7959H62.7773L66.7442 50.8796ZM62.6074 47.4502L61.2908 44.1365L59.9572 47.4502H62.6074Z" fill="white"/>
              <path d="M75.7712 50.8796H68.9375V41.7959H71.6515V48.7062H75.7712V50.8796Z" fill="white"/>
              <path d="M85.1314 50.8796H78.3359V41.7959H85.0083V43.6178H81.0499V45.2853H84.8256V47.1115H81.0499V49.0534H85.1187L85.1314 50.8796Z" fill="white"/>
              <path d="M96.6582 50.8797H93.7447L93.0396 49.2679C92.85 48.8154 92.6137 48.3843 92.3346 47.9818C92.1836 47.7562 91.9753 47.5756 91.7315 47.4589C91.4856 47.3742 91.2267 47.335 90.967 47.3431H90.6612V50.8883H87.9473V41.796H92.564C93.205 41.7698 93.84 41.9289 94.3945 42.2547C94.834 42.5214 95.1902 42.9077 95.4223 43.3692C95.6206 43.7552 95.7282 44.182 95.7366 44.6167C95.75 45.0199 95.6608 45.4199 95.4775 45.7784C95.3419 46.0452 95.1524 46.2804 94.9211 46.4686C94.7118 46.6267 94.4918 46.7699 94.2628 46.8973C94.5375 47.0516 94.781 47.2567 94.9806 47.5017C95.2874 47.9457 95.5465 48.4214 95.7536 48.9206L96.6582 50.8797ZM92.9717 44.6681C92.9833 44.5 92.95 44.3317 92.8753 44.1809C92.8006 44.0302 92.6872 43.9023 92.547 43.8108C92.1322 43.6146 91.6745 43.529 91.2176 43.5622H90.6442V45.7527H91.2303C91.6945 45.7886 92.1604 45.7121 92.5894 45.5298C92.7263 45.4363 92.8346 45.3061 92.9021 45.1539C92.9697 45.0017 92.9937 44.8334 92.9717 44.6681Z" fill="white"/>
              <path d="M105.731 50.8796H98.8926V41.7959H101.607V48.7062H105.731V50.8796Z" fill="white"/>
              <path d="M117.996 50.8796H115.218L114.483 49.0406H110.576L109.828 50.8796H107.051L111.03 41.7959H114.029L117.996 50.8796ZM113.859 47.4502L112.551 44.1536L111.217 47.4673L113.859 47.4502Z" fill="white"/>
              <path d="M129.964 50.8796H127.543L122.871 45.8212V50.8796H120.195V41.7959H122.603L127.275 46.8286V41.7959H129.964V50.8796Z" fill="white"/>
              <path d="M140.001 50.8796H133.205V41.7959H139.882V43.6178H135.923V45.2853H139.699V47.1115H135.923V49.0534H139.992L140.001 50.8796Z" fill="white"/>
              <path d="M62.4766 24.2251V10.7174H74.0714C75.0665 10.7065 76.0609 10.7782 77.0444 10.9317C77.5979 10.9993 78.1279 11.197 78.5919 11.509C79.0559 11.821 79.4408 12.2384 79.7159 12.7279C80.1746 13.5595 80.4039 15.1456 80.4039 17.4862C80.4245 18.475 80.3877 19.464 80.2935 20.4484C79.9877 22.5832 78.8919 23.8078 77.0062 24.1222C76.2018 24.222 75.3912 24.2621 74.581 24.2422L62.4766 24.2251ZM66.5624 21.0057H73.4046C73.9612 21.0267 74.5182 20.9734 75.061 20.8471C75.3633 20.7464 75.6315 20.562 75.835 20.3149C76.0385 20.0678 76.1691 19.768 76.212 19.4496C76.3046 18.8024 76.3401 18.1483 76.3181 17.4948C76.3181 16.1616 76.1737 15.2828 75.8934 14.8627C75.5225 14.3283 74.7212 14.0597 73.4895 14.0568H66.5624V21.0057Z" fill="white"/>
              <path d="M89.1914 24.2247V10.717H93.273V20.8853H102.502V24.2247H89.1914Z" fill="white"/>
              <path d="M60.4627 17.4821C60.4753 20.955 59.4665 24.3536 57.564 27.2475C55.6615 30.1413 52.951 32.4002 49.7757 33.7381C46.6003 35.076 43.1031 35.4326 39.7268 34.7629C36.3505 34.0932 33.247 32.4272 30.8095 29.9759C28.3719 27.5247 26.71 24.3984 26.034 20.9931C25.3581 17.5878 25.6986 14.0566 27.0125 10.8468C28.3263 7.63697 30.5544 4.89288 33.4145 2.96206C36.2746 1.03123 39.6381 0.000545238 43.0789 0.000521915C45.3577 -0.00398577 47.615 0.444644 49.722 1.32079C51.8289 2.19693 53.7442 3.48343 55.3585 5.10679C56.9728 6.73016 58.2544 8.6586 59.1302 10.782C60.006 12.9053 60.4588 15.1821 60.4627 17.4821ZM43.0832 4.00439C40.4421 4.01286 37.8628 4.81112 35.671 6.29834C33.4792 7.78556 31.7732 9.89503 30.7686 12.3603C29.7639 14.8256 29.5056 17.5361 30.0263 20.1494C30.547 22.7628 31.8233 25.1618 33.6941 27.0434C35.5648 28.9251 37.9461 30.2049 40.5371 30.7213C43.1281 31.2378 45.8127 30.9676 48.2516 29.9449C50.6906 28.9223 52.7746 27.193 54.2404 24.9756C55.7063 22.7582 56.4881 20.152 56.4873 17.4864C56.4783 13.9053 55.0617 10.4743 52.5486 7.94653C50.0354 5.41879 46.6311 4.00097 43.0832 4.00439Z" fill="white"/>
            </svg>
          </Link>

          <nav className="odl-nav-links">
            <Link to="/" className="odl-nav-link active">Home</Link>
            <div className="odl-nav-dropdown-trigger">
              <span>Products ▾</span>
              <div className="odl-nav-dropdown-menu">
                <div className="dropdown-item orange">
                  <span className="lane-badge-dot orange" />
                  <div>
                    <strong>Sales Lane</strong>
                    <small>Powersports Desking Suite</small>
                  </div>
                </div>
                <div className="dropdown-item purple">
                  <span className="lane-badge-dot purple" />
                  <div>
                    <strong>Menu Lane</strong>
                    <small>Interactive F&I Menu Software</small>
                  </div>
                </div>
                <div className="dropdown-item teal">
                  <span className="lane-badge-dot teal" />
                  <div>
                    <strong>Fast Lane</strong>
                    <small>Digital Credit & Lender Push</small>
                  </div>
                </div>
              </div>
            </div>
            <span className="odl-nav-link">News</span>
            <span className="odl-nav-link">Integrations</span>
            <span className="odl-nav-link">Company</span>
          </nav>

          <div className="odl-header-cta-group">
            <a href="tel:8774210135" className="odl-phone-chip" title="Call Us">
              <span className="phone-icon">📞</span>
              <strong>(877) 421-0135</strong>
            </a>
            <div className="odl-api-status-pill">
              <span className="pulse-dot" />
              <span>144 APIs Live • v1.0.1</span>
            </div>
          </div>
        </div>
      </header>

      {/* ── Main Showcase + Sign-In Area ── */}
      <main className="odl-hero-section">
        <div className="odl-hero-split-grid">
          {/* Left Column: Authentic One Dealer Lane Marketing & Showcase */}
          <div className="odl-hero-left">
            <div className="odl-badge-eyebrow">
              <span className="sparkle-icon">⚡</span>
              POWERSPORTS & AUTOMOTIVE DEALERSHIP DESKING
            </div>

            <h1 className="odl-main-hero-title">
              Close deals <span className="odl-gradient-text">in 20 mins</span>
            </h1>

            <p className="odl-hero-description">
              Unlock a new level of speed and efficiency without changing how your team works.
              Connect showroom desking, F&amp;I menu presentation, lender submission, and back-office accounting into a single unified lane.
            </p>

            {/* 3 Lane Selector Cards */}
            <div className="odl-lanes-tabs-row">
              <button
                type="button"
                className={`odl-lane-card-btn ${activeLaneTab === "sales" ? "active-orange" : ""}`}
                onClick={() => setActiveLaneTab("sales")}
              >
                <div className="lane-card-top">
                  <span className="lane-icon-circle orange">⚡</span>
                  <span className="lane-name">Sales Lane</span>
                </div>
                <p>Digital desking, CRM lead pipeline &amp; multi-unit quotation calculation.</p>
              </button>

              <button
                type="button"
                className={`odl-lane-card-btn ${activeLaneTab === "menu" ? "active-purple" : ""}`}
                onClick={() => setActiveLaneTab("menu")}
              >
                <div className="lane-card-top">
                  <span className="lane-icon-circle purple">📑</span>
                  <span className="lane-name">Menu Lane</span>
                </div>
                <p>Interactive F&amp;I menus, real-time eRating, warranty packages &amp; eSign.</p>
              </button>

              <button
                type="button"
                className={`odl-lane-card-btn ${activeLaneTab === "fast" ? "active-teal" : ""}`}
                onClick={() => setActiveLaneTab("fast")}
              >
                <div className="lane-card-top">
                  <span className="lane-icon-circle teal">🚗</span>
                  <span className="lane-name">Fast Lane</span>
                </div>
                <p>Digital credit application extraction &amp; 1-click multi-lender repopulation.</p>
              </button>
            </div>

            {/* Metric Achievements Tickers */}
            <div className="odl-stats-bar">
              <div className="stat-ticker">
                <strong>78%</strong>
                <span>Faster Deal Turnaround</span>
              </div>
              <div className="stat-divider" />
              <div className="stat-ticker">
                <strong>30+ Mins</strong>
                <span>Saved Per Customer Deal</span>
              </div>
              <div className="stat-divider" />
              <div className="stat-ticker">
                <strong>+27%</strong>
                <span>F&amp;I PVR Profit Growth</span>
              </div>
            </div>

            {/* Certified Integrations Marquee Badges */}
            <div className="odl-integrations-preview">
              <span className="integrations-label">VERIFIED INTEGRATIONS:</span>
              <div className="integrations-pill-row">
                <span className="integ-pill">Lightspeed DMS</span>
                <span className="integ-pill">700Credit</span>
                <span className="integ-pill">Safe-Guard</span>
                <span className="integ-pill">McGraw</span>
                <span className="integ-pill">PEN Network</span>
                <span className="integ-pill cert">SOC 2 Type II</span>
              </div>
            </div>
          </div>

          {/* Right Column: Sleek Dealership Sign In & Role Switcher */}
          <div className="odl-hero-right">
            <div className="odl-sign-in-glass-card">
              <div className="sign-in-card-header">
                <div className="sign-in-title-row">
                  <div>
                    <h2>Dealership Workspace Sign In</h2>
                    <p>Enter credentials or choose an operational role for instant demo.</p>
                  </div>
                  <div className="odl-speed-mark">ODL</div>
                </div>
              </div>

              {/* Persona Selector (Compact High-Tech Segmented Grid) */}
              <div className="odl-role-segmented-box">
                <span className="role-box-label">QUICK OPERATIONAL PERSONA:</span>
                <div className="role-segmented-buttons">
                  {(Object.keys(ROLE_PRESETS) as PresetRole[]).map((key) => {
                    const preset = ROLE_PRESETS[key];
                    const isSelected = selectedRole === key;
                    return (
                      <button
                        key={key}
                        type="button"
                        onClick={() => handleRoleSelect(key)}
                        className={`role-seg-btn ${isSelected ? "selected" : ""}`}
                        style={{
                          borderColor: isSelected ? preset.color : undefined,
                          background: isSelected ? `${preset.color}20` : undefined,
                        }}
                      >
                        <span className="seg-dot" style={{ background: preset.color }} />
                        <span className="seg-name">{preset.name}</span>
                      </button>
                    );
                  })}
                </div>

                <div className="active-role-preview-bar">
                  <div className="active-role-meta">
                    <strong style={{ color: ROLE_PRESETS[selectedRole].color }}>
                      {ROLE_PRESETS[selectedRole].badge}
                    </strong>
                    <span>{ROLE_PRESETS[selectedRole].desc}</span>
                  </div>
                  <button
                    type="button"
                    disabled={loading}
                    onClick={() => handleQuickRoleLogin(selectedRole)}
                    className="instant-login-action-btn"
                    style={{ background: ROLE_PRESETS[selectedRole].color }}
                  >
                    {loading ? "Signing In..." : "Instant Demo Login ➔"}
                  </button>
                </div>
              </div>

              <div className="odl-form-divider">
                <span>OR SIGN IN WITH CREDENTIALS</span>
              </div>

              {error && <div className="odl-auth-error-banner">{error}</div>}

              <form onSubmit={handleSubmit} className="odl-credential-form">
                <div className="odl-input-field">
                  <label htmlFor="login-email">Staff Email Address</label>
                  <input
                    id="login-email"
                    type="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="Enter dealership email"
                    autoComplete="email"
                    required
                  />
                </div>

                <div className="odl-input-field">
                  <div className="field-label-row">
                    <label htmlFor="login-password">Workspace Password</label>
                    <Link to="/forgot-password" className="forgot-link">Forgot password?</Link>
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

                <button type="submit" disabled={loading} className="odl-sign-in-btn">
                  {loading ? "Authenticating Session..." : "Sign In to Connected OS"}
                </button>
              </form>

              <div className="odl-card-footer-meta">
                <span>Direct Access:</span>
                <span className="quick-acc-tag" onClick={() => handleRoleSelect("SuperAdmin")}>SuperAdmin</span>
                <span className="quick-acc-tag" onClick={() => handleRoleSelect("SalesExecutive")}>Sales Desk</span>
                <span className="quick-acc-tag" onClick={() => handleRoleSelect("Buyer")}>Buyer Portal</span>
              </div>
            </div>
          </div>
        </div>
      </main>

      {/* ── Footer ── */}
      <footer className="odl-landing-footer">
        <div className="footer-inner">
          <div className="footer-left">
            <span>© 2026 One Dealer Lane • Enterprise Connected Dealership Operating System</span>
          </div>
          <div className="footer-right">
            <span>SOC 2 Type II Certified</span>
            <span className="sep">•</span>
            <span>Lightspeed DMS Certified</span>
            <span className="sep">•</span>
            <span>144 APIs Online</span>
          </div>
        </div>
      </footer>
    </div>
  );
}

