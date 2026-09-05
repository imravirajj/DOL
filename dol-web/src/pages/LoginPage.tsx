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
          <Link to="/" className="odl-brand-logo-link">
            <svg xmlns="http://www.w3.org/2000/svg" width="136" height="44" viewBox="0 0 140 51" fill="none" className="odl-logo-svg">
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
            <button type="button" className="odl-nav-link-btn" onClick={() => showSuccessToast("News: One Dealer Lane v1.0.1 Released with 144 Connected APIs!")}>
              News
            </button>
            <button type="button" className="odl-nav-link-btn" onClick={() => showSuccessToast("Certified Integrations: Lightspeed, 700Credit, Safe-Guard, McGraw, CDK")}>
              Integrations
            </button>
            <div className="odl-nav-dropdown-trigger">
              <span>Company ▾</span>
              <div className="odl-nav-dropdown-menu">
                <div className="dropdown-item" onClick={() => setIsDemoModalOpen(true)}>
                  <strong>About One Dealer Lane</strong>
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
              <span className="modal-badge-eyebrow">ONE DEALER LANE DEALERSHIP OS • v1.0.1</span>
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
              <p>See how One Dealer Lane unifies showroom desking, F&amp;I menus, and digital lenders into a seamless workflow.</p>
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
