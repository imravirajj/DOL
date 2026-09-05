import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useAuth, ROLE_PRESETS, type PresetRole } from "../context/AuthContext";
import { showSuccessToast } from "../services/toastService";

export default function LandingPage() {
  const navigate = useNavigate();
  const { loginWithRolePreset } = useAuth();

  const [activeLaneTab, setActiveLaneTab] = useState<"sales" | "menu" | "fast">("sales");
  const [isDemoModalOpen, setIsDemoModalOpen] = useState(false);
  const [isTestimonialsModalOpen, setIsTestimonialsModalOpen] = useState(false);
  const [isAuthModalOpen, setIsAuthModalOpen] = useState(false);
  const [selectedRole, setSelectedRole] = useState<PresetRole>("SuperAdmin");
  const [loading, setLoading] = useState(false);
  const [demoSubmitted, setDemoSubmitted] = useState(false);

  const handleQuickRoleLogin = async (roleKey: PresetRole) => {
    setLoading(true);
    setSelectedRole(roleKey);
    try {
      await loginWithRolePreset(roleKey);
      showSuccessToast(`Logged in as ${ROLE_PRESETS[roleKey].badge}`);
      setIsAuthModalOpen(false);
      navigate("/dashboard");
    } catch (err) {
      console.error(err);
      showSuccessToast("Navigating to login...");
      navigate("/login");
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
      {/* ── Top Header Bar (Same to Same as Screenshot) ── */}
      <header className="odl-site-header">
        <div className="odl-header-container">
          <Link to="/" className="odl-brand-logo-link" title="Dealer One Lane (DOL)">
            <svg xmlns="http://www.w3.org/2000/svg" width="136" height="42" viewBox="0 0 136 42" fill="none" className="odl-logo-svg">
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
              <svg width="15" height="15" viewBox="0 0 24 24" fill="currentColor">
                <path d="M6.62 10.79a15.053 15.053 0 006.59 6.59l2.2-2.2a1 1 0 011.01-.24c1.12.37 2.33.57 3.58.57a1 1 0 011 1V20a1 1 0 01-1 1A17 17 0 013 4a1 1 0 011-1h3.5a1 1 0 011 1c0 1.25.2 2.46.57 3.58a1 1 0 01-.25 1.01l-2.2 2.2z" />
              </svg>
            </button>
            <button 
              type="button" 
              className="odl-btn-coral-pill sm"
              onClick={() => setIsTestimonialsModalOpen(true)}
            >
              Testimonials
            </button>
            <Link 
              to="/login"
              className="odl-header-signin-link"
              title="Sign in to Dealership Workspace"
            >
              Sign In
            </Link>
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

          {/* Action Button Row - Only Book a Demo */}
          <div className="odl-center-cta-row">
            <button
              type="button"
              className="odl-btn-coral-pill lg"
              onClick={() => setIsAuthModalOpen(true)}
            >
              Book a Demo
            </button>
          </div>

          {/* ── Visual Mindmap / Stages Infographic ── */}
          <div className="odl-mindmap-stage-wrapper">
            {/* SVG Solid Curved Connector Tracks linking center card to the 4 nodes */}
            <svg className="odl-connectors-svg" viewBox="0 0 1000 440" fill="none" preserveAspectRatio="xMidYMid meet">
              {/* Left Branch - solid dark gray lines */}
              <path
                d="M 440 310 L 440 215 M 440 215 L 315 215 M 440 215 L 440 140 C 440 85, 430 75, 315 75"
                stroke="#2a3348"
                strokeWidth="2.8"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              {/* Right Branch - solid dark gray lines */}
              <path
                d="M 560 310 L 560 215 M 560 215 L 685 215 M 560 215 L 560 140 C 560 85, 570 75, 685 75"
                stroke="#2a3348"
                strokeWidth="2.8"
                strokeLinecap="round"
                strokeLinejoin="round"
              />
              {/* Bottom branching tracks peeking down */}
              <path
                d="M 440 380 L 440 425 C 440 440, 420 445, 315 445"
                stroke="#222b3d"
                strokeWidth="2.8"
                strokeLinecap="round"
              />
              <path
                d="M 560 380 L 560 425 C 560 440, 580 445, 685 445"
                stroke="#222b3d"
                strokeWidth="2.8"
                strokeLinecap="round"
              />
            </svg>

            {/* Left Node 1: Inventory */}
            <div 
              className="odl-node-pill left-top purple-glow"
              onClick={() => setIsAuthModalOpen(true)}
            >
              <div className="node-icon-box purple">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="white">
                  <rect x="3" y="3" width="7.5" height="7.5" rx="1.8" />
                  <rect x="13.5" y="3" width="7.5" height="7.5" rx="1.8" />
                  <rect x="3" y="13.5" width="7.5" height="7.5" rx="1.8" />
                  <rect x="13.5" y="13.5" width="7.5" height="7.5" rx="1.8" />
                </svg>
              </div>
              <span className="node-label">Inventory</span>
            </div>

            {/* Left Node 2: CRM */}
            <div 
              className="odl-node-pill left-bottom amber-glow"
              onClick={() => setIsAuthModalOpen(true)}
            >
              <div className="node-icon-box amber">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="white">
                  <rect x="4" y="13" width="4" height="8" rx="1.5" />
                  <rect x="10" y="8" width="4" height="13" rx="1.5" />
                  <rect x="16" y="3" width="4" height="18" rx="1.5" />
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
              onClick={() => setIsAuthModalOpen(true)}
            >
              <div className="node-icon-box purple">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="white">
                  <path d="M4 8C4 6.89543 4.89543 6 6 6H18C19.1046 6 20 6.89543 20 8V18C20 19.1046 19.1046 20 18 20H6C4.89543 20 4 19.1046 4 18V8Z" />
                  <path d="M9 6V4.5C9 3.67157 9.67157 3 10.5 3H13.5C14.3284 3 15 3.67157 15 4.5V6" stroke="white" strokeWidth="2" strokeLinecap="round" />
                  <rect x="10" y="11" width="4" height="3" rx="0.5" fill="#a855f7" />
                </svg>
              </div>
              <span className="node-label">Credit &amp;ID</span>
            </div>

            {/* Right Node 2: Lenders */}
            <div 
              className="odl-node-pill right-bottom amber-glow"
              onClick={() => setIsAuthModalOpen(true)}
            >
              <div className="node-icon-box amber">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="white">
                  <circle cx="16" cy="6" r="3.5" fill="white" />
                  <text x="16" y="8.5" fill="#f59e0b" fontSize="7" fontWeight="900" textAnchor="middle">$</text>
                  <path d="M3 16C3 15 4 14 6 14H11L14.5 16H19V19H14L11 17.5H6C4.5 17.5 3 18 3 19V16Z" fill="white" />
                </svg>
              </div>
              <span className="node-label">Lenders</span>
            </div>

            {/* Subtle peeking lower nodes */}
            <div className="odl-node-pill left-bottom-peek teal-glow">
              <div className="node-icon-box teal">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="white">
                  <path d="M12 2L2 7L12 12L22 7L12 2Z" />
                  <path d="M2 17L12 22L22 17" stroke="white" strokeWidth="2" />
                  <path d="M2 12L12 17L22 12" stroke="white" strokeWidth="2" />
                </svg>
              </div>
              <span className="node-label">DMS Sync</span>
            </div>

            <div className="odl-node-pill right-bottom-peek teal-glow">
              <div className="node-icon-box teal">
                <svg width="18" height="18" viewBox="0 0 24 24" fill="white">
                  <path d="M14 2H6a2 2 0 00-2 2v16a2 2 0 002 2h12a2 2 0 002-2V8l-6-6z" />
                  <path d="M14 2v6h6" fill="#06b6d4" />
                </svg>
              </div>
              <span className="node-label">eContracting</span>
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

      {/* ── Floating Right-Edge "Contact Us" Tab ── */}
      <button 
        type="button" 
        className="odl-floating-contact-tab" 
        onClick={() => setIsDemoModalOpen(true)}
        title="Contact Dealership Solutions"
      >
        <span>Contact Us</span>
      </button>

      {/* ── Bottom Right Cookie Badge ── */}
      <button 
        type="button" 
        className="odl-floating-cookie-btn" 
        onClick={() => showSuccessToast("Cookie Preferences: Strictly Necessary & Performance Cookies Enabled")}
        title="Cookie Settings"
      >
        <span>🍪</span>
      </button>

      {/* ── Role Preset Access / Demo Modal ── */}
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
              <p>Select a 1-click role preset to explore the live dealership software or proceed to sign in.</p>
            </div>

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
                      disabled={loading}
                      className={`odl-role-chip ${isSelected ? "selected" : ""}`}
                      onClick={() => handleQuickRoleLogin(roleKey)}
                    >
                      <div className="role-chip-top">
                        <strong className="role-name">{preset.badge}</strong>
                      </div>
                      <small className="role-desc">{preset.desc}</small>
                      <span className="role-action-hint">
                        {loading && selectedRole === roleKey ? "Launching..." : "Click to launch workspace →"}
                      </span>
                    </button>
                  );
                })}
              </div>
            </div>

            <div className="form-actions-row">
              <Link to="/login" className="odl-btn-secondary text-center w-full">
                Custom Email/Password Login &rarr;
              </Link>
            </div>
          </div>
        </div>
      )}

      {/* ── Book a Demo Modal ── */}
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

      {/* ── Testimonials Modal ── */}
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
                  navigate("/login");
                }}
              >
                Proceed to Login &rarr;
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
