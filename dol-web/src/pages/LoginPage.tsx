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

  // Modals
  const [isAuthModalOpen, setIsAuthModalOpen] = useState(false);
  const [isDemoModalOpen, setIsDemoModalOpen] = useState(false);
  const [isTestimonialsModalOpen, setIsTestimonialsModalOpen] = useState(false);
  const [activeNodePreview, setActiveNodePreview] = useState<string | null>(null);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [demoSubmitted, setDemoSubmitted] = useState(false);

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
      setIsAuthModalOpen(false);
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
      setIsAuthModalOpen(false);
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

  const handleDemoSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setDemoSubmitted(true);
    setTimeout(() => {
      setDemoSubmitted(false);
      setIsDemoModalOpen(false);
      showSuccessToast("Demo request received! Our sales engineer will reach out.");
    }, 1500);
  };

  return (
    <div className="odl-landing-wrapper">
      {/* ── Top Header Bar (Authentic One Dealer Lane Header) ── */}
      <header className="odl-site-header">
        <div className="odl-header-container">
          <Link to="/" className="odl-brand-logo-link" title="Dealer One Lane (DOL)">
            <svg xmlns="http://www.w3.org/2000/svg" width="180" height="44" viewBox="0 0 180 44" fill="none" className="odl-logo-svg">
              <defs>
                <linearGradient id="dolGrad" x1="0%" y1="0%" x2="100%" y2="100%">
                  <stop offset="0%" stopColor="#ffffff"/>
                  <stop offset="100%" stopColor="#f26522"/>
                </linearGradient>
              </defs>
              <g transform="translate(2, 3)">
                <rect x="0" y="0" width="48" height="38" rx="19" stroke="#ffffff" strokeWidth="3" fill="none" opacity="0.95"/>
                <circle cx="19" cy="19" r="10" stroke="#f26522" strokeWidth="3" fill="none"/>
                <circle cx="29" cy="19" r="10" stroke="#ffffff" strokeWidth="3" fill="none"/>
                <path d="M19 12V26M29 12V26" stroke="#ffffff" strokeWidth="2.5" strokeLinecap="round"/>
                <circle cx="24" cy="19" r="3" fill="#f26522"/>
              </g>
              <g transform="translate(58, 8)">
                <text x="0" y="16" fill="#ffffff" fontFamily="'Outfit', sans-serif" fontWeight="900" fontSize="18" letterSpacing="1.5">DOL</text>
                <text x="0" y="27" fill="#cbd5e1" fontFamily="'Outfit', sans-serif" fontWeight="700" fontSize="7.5" letterSpacing="2.4">DEALER ONE LANE</text>
              </g>
            </svg>
          </Link>

          <nav className="odl-nav-links">
            <Link to="/" className="odl-nav-link active">Home</Link>
            <div className="odl-nav-dropdown-trigger">
              <span>Product ▾</span>
              <div className="odl-nav-dropdown-menu">
                <div className="dropdown-item orange" onClick={() => { setActiveLaneTab("sales"); setIsAuthModalOpen(true); }}>
                  <span className="lane-badge-dot orange" />
                  <div>
                    <strong>Sales Lane</strong>
                    <small>Powersports Desking Suite</small>
                  </div>
                </div>
                <div className="dropdown-item purple" onClick={() => { setActiveLaneTab("menu"); setIsAuthModalOpen(true); }}>
                  <span className="lane-badge-dot purple" />
                  <div>
                    <strong>Menu Lane</strong>
                    <small>Interactive F&amp;I Menu Software</small>
                  </div>
                </div>
                <div className="dropdown-item teal" onClick={() => { setActiveLaneTab("fast"); setIsAuthModalOpen(true); }}>
                  <span className="lane-badge-dot teal" />
                  <div>
                    <strong>Fast Lane</strong>
                    <small>Digital Credit &amp; Lender Push</small>
                  </div>
                </div>
              </div>
            </div>
            <button type="button" className="odl-nav-link-btn" onClick={() => showSuccessToast("News: Dealer One Lane (DOL) v1.0.1 Released with 144 Connected APIs!")}>
              News
            </button>
            <button type="button" className="odl-nav-link-btn" onClick={() => showSuccessToast("Certified Integrations: Lightspeed, 700Credit, Safe-Guard, McGraw, CDK")}>
              Integrations
            </button>
            <div className="odl-nav-dropdown-trigger">
              <span>Company ▾</span>
              <div className="odl-nav-dropdown-menu">
                <div className="dropdown-item" onClick={() => setIsDemoModalOpen(true)}>
                  <strong>About Dealer One Lane (DOL)</strong>
                  <small>Cloud Dealership Operating System</small>
                </div>
                <div className="dropdown-item" onClick={() => setIsTestimonialsModalOpen(true)}>
                  <strong>Customer Testimonials</strong>
                  <small>Dealer Principal Stories</small>
                </div>
              </div>
            </div>
          </nav>

          <div className="odl-header-cta-group">
            <button 
              type="button" 
              className="odl-circle-phone-btn" 
              title="Call Us (877) 421-0135"
              onClick={() => showSuccessToast("Support Desk: (877) 421-0135 • Available 24/7")}
            >
              <span className="phone-icon">📞</span>
            </button>
            <button 
              type="button" 
              className="odl-btn-coral-pill sm"
              onClick={() => setIsTestimonialsModalOpen(true)}
            >
              Testimonials
            </button>
            <button 
              type="button" 
              className="odl-btn-glass-pill sm"
              onClick={() => setIsAuthModalOpen(true)}
            >
              Sign In
            </button>
          </div>
        </div>
      </header>

      {/* ── Main Hero Card Section (Same to Same as Screenshot) ── */}
      <main className="odl-hero-viewport-container">
        <section className="odl-screenshot-hero-card">
          {/* Ambient Warm Amber Radial Vignette Glow behind center mindmap */}
          <div className="odl-hero-ambient-vignette" />

          {/* Centered Headline */}
          <h1 className="odl-center-hero-title">
            Close deals <span className="odl-coral-accent">in 20 mins</span>
          </h1>

          {/* Centered Subtitle */}
          <p className="odl-center-hero-subtitle">
            Unlock a new level of speed and efficiency without changing how your team works.
          </p>

          {/* Action Button Row */}
          <div className="odl-center-cta-row">
            <button
              type="button"
              className="odl-btn-coral-pill lg glow-shadow"
              onClick={() => setIsDemoModalOpen(true)}
            >
              Book a Demo
            </button>
            <button
              type="button"
              className="odl-btn-glass-pill lg"
              onClick={() => setIsAuthModalOpen(true)}
            >
              Launch Dealership OS
            </button>
          </div>

          {/* ── Visual Mindmap / Stages Infographic ── */}
          <div className="odl-mindmap-stage-wrapper">
            {/* SVG Curved Connector Tracks linking center card to the 4 nodes */}
            <svg className="odl-connectors-svg" viewBox="0 0 1000 360" fill="none" preserveAspectRatio="xMidYMid meet">
              <defs>
                <linearGradient id="connectorGlow" x1="0%" y1="0%" x2="100%" y2="0%">
                  <stop offset="0%" stopColor="#38bdf8" stopOpacity="0.6" />
                  <stop offset="50%" stopColor="#f26522" stopOpacity="0.8" />
                  <stop offset="100%" stopColor="#c084fc" stopOpacity="0.6" />
                </linearGradient>
              </defs>

              {/* Left Branch to Inventory (Top Left) */}
              <path
                d="M 440 240 C 370 240, 320 70, 240 70"
                stroke="rgba(100, 116, 139, 0.45)"
                strokeWidth="2.5"
                strokeDasharray="4 2"
              />
              {/* Left Branch to CRM (Bottom Left) */}
              <path
                d="M 440 270 C 370 270, 320 200, 240 200"
                stroke="rgba(100, 116, 139, 0.45)"
                strokeWidth="2.5"
              />

              {/* Right Branch to Credit & ID (Top Right) */}
              <path
                d="M 560 240 C 630 240, 680 70, 760 70"
                stroke="rgba(100, 116, 139, 0.45)"
                strokeWidth="2.5"
                strokeDasharray="4 2"
              />
              {/* Right Branch to Lenders (Bottom Right) */}
              <path
                d="M 560 270 C 630 270, 680 200, 760 200"
                stroke="rgba(100, 116, 139, 0.45)"
                strokeWidth="2.5"
              />
            </svg>

            {/* Left Node 1: Inventory */}
            <div 
              className="odl-node-pill left-top purple-glow"
              onClick={() => {
                setActiveNodePreview("Inventory: Digital Yard Stock, VIN matching & multi-unit pricing margin tracking.");
                setIsAuthModalOpen(true);
              }}
            >
              <div className="node-icon-box purple">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor">
                  <rect x="3" y="3" width="8" height="8" rx="2" />
                  <rect x="13" y="3" width="8" height="8" rx="2" />
                  <rect x="3" y="13" width="8" height="8" rx="2" />
                  <rect x="13" y="13" width="8" height="8" rx="2" />
                </svg>
              </div>
              <span className="node-label">Inventory</span>
            </div>

            {/* Left Node 2: CRM */}
            <div 
              className="odl-node-pill left-bottom amber-glow"
              onClick={() => {
                setActiveNodePreview("CRM: Showroom lead pipeline, trade-in valuations & fast desking quotes.");
                setIsAuthModalOpen(true);
              }}
            >
              <div className="node-icon-box amber">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor">
                  <rect x="4" y="10" width="4" height="10" rx="1" />
                  <rect x="10" y="5" width="4" height="15" rx="1" />
                  <rect x="16" y="2" width="4" height="18" rx="1" />
                </svg>
              </div>
              <span className="node-label">CRM</span>
            </div>

            {/* Center Bottom Card: Multiple stages. One connected experience. */}
            <div className="odl-center-white-card" onClick={() => setIsAuthModalOpen(true)}>
              <span className="center-card-line1">Multiple stages.</span>
              <span className="center-card-line2">
                One connected <span className="coral-bold">experience.</span>
              </span>
            </div>

            {/* Right Node 1: Credit & ID */}
            <div 
              className="odl-node-pill right-top purple-glow"
              onClick={() => {
                setActiveNodePreview("Credit & ID: Digital credit application, 700Credit bureau scores & identity security.");
                setIsAuthModalOpen(true);
              }}
            >
              <div className="node-icon-box purple">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor">
                  <rect x="3" y="6" width="18" height="15" rx="3" />
                  <path d="M8 6V4a2 2 0 0 1 2-2h4a2 2 0 0 1 2 2v2" fill="none" stroke="currentColor" strokeWidth="2" />
                </svg>
              </div>
              <span className="node-label">Credit &amp;ID</span>
            </div>

            {/* Right Node 2: Lenders */}
            <div 
              className="odl-node-pill right-bottom amber-glow"
              onClick={() => {
                setActiveNodePreview("Lenders: 1-Click push to prime & subprime finance tiers, instant rate eRating & eContracting.");
                setIsAuthModalOpen(true);
              }}
            >
              <div className="node-icon-box amber">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="currentColor">
                  <circle cx="12" cy="7" r="4" />
                  <path d="M4 21v-2a4 4 0 0 1 4-4h8a4 4 0 0 1 4 4v2" />
                </svg>
              </div>
              <span className="node-label">Lenders</span>
            </div>
          </div>
        </section>

        {/* ── Three Lanes Breakdown Section ── */}
        <div className="odl-lanes-showcase-grid">
          <div 
            className={`odl-lane-tile ${activeLaneTab === "sales" ? "active-orange" : ""}`}
            onClick={() => setActiveLaneTab("sales")}
          >
            <div className="lane-tile-badge orange">⚡ SALES LANE</div>
            <h3>Powersports Desking Suite</h3>
            <p>Digital desking, CRM lead pipeline &amp; multi-unit quotation calculation. Close in 20 minutes.</p>
            <div className="lane-tile-footer">
              <span>Explore Desking Desk &rarr;</span>
            </div>
          </div>

          <div 
            className={`odl-lane-tile ${activeLaneTab === "menu" ? "active-purple" : ""}`}
            onClick={() => setActiveLaneTab("menu")}
          >
            <div className="lane-tile-badge purple">📑 MENU LANE</div>
            <h3>Interactive F&amp;I Software</h3>
            <p>Interactive F&amp;I menus, real-time warranty packages, gap protection &amp; live e-signature.</p>
            <div className="lane-tile-footer">
              <span>Explore F&amp;I Menus &rarr;</span>
            </div>
          </div>

          <div 
            className={`odl-lane-tile ${activeLaneTab === "fast" ? "active-teal" : ""}`}
            onClick={() => setActiveLaneTab("fast")}
          >
            <div className="lane-tile-badge teal">🚗 FAST LANE</div>
            <h3>Digital Credit &amp; Lender Push</h3>
            <p>Digital credit application extraction &amp; 1-click multi-lender repopulation with certified rating.</p>
            <div className="lane-tile-footer">
              <span>Explore Credit Push &rarr;</span>
            </div>
          </div>
        </div>

        {/* ── Key Metrics Ticker ── */}
        <div className="odl-metric-strip">
          <div className="metric-col">
            <strong>78%</strong>
            <span>Faster Deal Turnaround</span>
          </div>
          <div className="metric-separator" />
          <div className="metric-col">
            <strong>30+ Mins</strong>
            <span>Saved Per Customer Deal</span>
          </div>
          <div className="metric-separator" />
          <div className="metric-col">
            <strong>+27%</strong>
            <span>F&amp;I PVR Profit Growth</span>
          </div>
          <div className="metric-separator" />
          <div className="metric-col">
            <strong>144 APIs</strong>
            <span>Active Enterprise Endpoints</span>
          </div>
        </div>

        {/* ── Certified Integrations Bar ── */}
        <div className="odl-integrations-bar">
          <span className="integ-title">ENTERPRISE INTEGRATIONS:</span>
          <div className="integ-tags">
            <span className="integ-tag">Lightspeed DMS</span>
            <span className="integ-tag">700Credit</span>
            <span className="integ-tag">Safe-Guard</span>
            <span className="integ-tag">McGraw Powersports</span>
            <span className="integ-tag">PEN Network</span>
            <span className="integ-tag cert">SOC 2 Type II Certified</span>
          </div>
        </div>
      </main>

      {/* ── Floating Right-Edge "Contact Us" Tab (Same to Same as Screenshot) ── */}
      <button 
        type="button" 
        className="odl-floating-contact-tab" 
        onClick={() => setIsDemoModalOpen(true)}
        title="Contact Dealership Solutions"
      >
        <span>Contact Us</span>
      </button>

      {/* ── Bottom Right Cookie Badge (Same to Same as Screenshot) ── */}
      <button 
        type="button" 
        className="odl-floating-cookie-btn" 
        onClick={() => showSuccessToast("Cookie Preferences: Strictly Necessary & Performance Cookies Enabled")}
        title="Cookie Settings"
      >
        <span>🍪</span>
      </button>

      {/* ═════════════════════════════════════════════════════════════════ */}
      {/* ── MODAL 1: Role-Based Dealership OS Access & Login Modal ── */}
      {/* ═════════════════════════════════════════════════════════════════ */}
      {isAuthModalOpen && (
        <div className="odl-modal-overlay" onClick={() => setIsAuthModalOpen(false)}>
          <div className="odl-modal-card" onClick={(e) => e.stopPropagation()}>
            <button 
              type="button" 
              className="odl-modal-close-btn" 
              onClick={() => setIsAuthModalOpen(false)}
            >
              ✕
            </button>

            <div className="odl-modal-header">
              <span className="modal-badge-eyebrow">DEALER ONE LANE (DOL) OS • v1.0.1</span>
              <h2>Launch Dealership Workspace</h2>
              <p>
                {activeNodePreview 
                  ? activeNodePreview 
                  : "Select a role preset for instant 1-click preview or sign in with your enterprise credentials."}
              </p>
            </div>

            {/* 1-Click Role Switcher Presets */}
            <div className="odl-role-presets-container">
              <span className="preset-group-title">⚡ 1-CLICK ROLE PRESETS (TEST ENVIRONMENT):</span>
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
                      <span className="role-action-hint">Double click to instant enter &rarr;</span>
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

            {/* Standard Login Form */}
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

            <div className="modal-footer-telematics">
              <span className="pulse-dot" />
              <span>Identity Server Live • 144 Dealership APIs Connected • v1.0.1</span>
            </div>
          </div>
        </div>
      )}

      {/* ═════════════════════════════════════════════════════════════════ */}
      {/* ── MODAL 2: Book a Demo Modal ── */}
      {/* ═════════════════════════════════════════════════════════════════ */}
      {isDemoModalOpen && (
        <div className="odl-modal-overlay" onClick={() => setIsDemoModalOpen(false)}>
          <div className="odl-modal-card sm" onClick={(e) => e.stopPropagation()}>
            <button 
              type="button" 
              className="odl-modal-close-btn" 
              onClick={() => setIsDemoModalOpen(false)}
            >
              ✕
            </button>

            <div className="odl-modal-header">
              <span className="modal-badge-eyebrow">SCHEDULE LIVE DEMO</span>
              <h2>Experience 20-Min Deal Closing</h2>
              <p>See how Dealer One Lane (DOL) unifies showroom desking, F&amp;I menus, and digital lenders into a seamless workflow.</p>
            </div>

            {demoSubmitted ? (
              <div className="odl-demo-success">
                <span className="success-icon">🎉</span>
                <h3>Demo Request Dispatched!</h3>
                <p>A Dealership Solutions specialist will contact you within 15 minutes.</p>
              </div>
            ) : (
              <form onSubmit={handleDemoSubmit} className="odl-demo-form">
                <div className="form-group">
                  <label>Dealership Name</label>
                  <input type="text" placeholder="Apex Powersports Group" required defaultValue="Metro Dealership Group" />
                </div>
                <div className="form-group">
                  <label>Work Email</label>
                  <input type="email" placeholder="principal@dealership.com" required defaultValue="general.manager@powersports.com" />
                </div>
                <div className="form-group">
                  <label>Primary DMS Platform</label>
                  <select defaultValue="Lightspeed">
                    <option value="Lightspeed">Lightspeed EVO / NXT</option>
                    <option value="CDK">CDK Global</option>
                    <option value="Reynolds">Reynolds &amp; Reynolds</option>
                    <option value="DealerTrack">DealerTrack DMS</option>
                  </select>
                </div>
                <div className="form-actions-row">
                  <button type="submit" className="odl-btn-primary w-full">
                    Confirm 20-Min Demo Session &rarr;
                  </button>
                </div>
              </form>
            )}
          </div>
        </div>
      )}

      {/* ═════════════════════════════════════════════════════════════════ */}
      {/* ── MODAL 3: Testimonials & Case Studies Modal ── */}
      {/* ═════════════════════════════════════════════════════════════════ */}
      {isTestimonialsModalOpen && (
        <div className="odl-modal-overlay" onClick={() => setIsTestimonialsModalOpen(false)}>
          <div className="odl-modal-card" onClick={(e) => e.stopPropagation()}>
            <button 
              type="button" 
              className="odl-modal-close-btn" 
              onClick={() => setIsTestimonialsModalOpen(false)}
            >
              ✕
            </button>

            <div className="odl-modal-header">
              <span className="modal-badge-eyebrow">DEALER PRINCIPAL VERIFIED REVIEWS</span>
              <h2>Proven Results Across Top Powersports Dealerships</h2>
              <p>Read how dealerships transformed their floor desk times and back-office F&amp;I margins.</p>
            </div>

            <div className="odl-testimonials-grid">
              <div className="testimonial-card">
                <div className="rating-stars">★★★★★</div>
                <p className="quote">
                  &ldquo;We dropped our average showroom desking time from 58 minutes down to 17 minutes flat. Our F&amp;I product penetration jumped 31% on Menu Lane.&rdquo;
                </p>
                <div className="author-meta">
                  <strong>Marcus Vance</strong>
                  <span>Managing Partner • Rocky Mountain Powersports</span>
                </div>
              </div>

              <div className="testimonial-card">
                <div className="rating-stars">★★★★★</div>
                <p className="quote">
                  &ldquo;The digital lender extraction saves our desk managers at least 30 minutes on every credit pass. It paid for itself in week one.&rdquo;
                </p>
                <div className="author-meta">
                  <strong>Sarah Jenkins</strong>
                  <span>Finance Director • Sunstate Harley &amp; Marine</span>
                </div>
              </div>
            </div>

            <div className="form-actions-row mt-4">
              <button 
                type="button" 
                className="odl-btn-primary w-full"
                onClick={() => {
                  setIsTestimonialsModalOpen(false);
                  setIsAuthModalOpen(true);
                }}
              >
                Launch Workspace &amp; Test Live APIs &rarr;
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
