import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import PortalLayout from "../components/layout/PortalLayout";
import { analyticsApi, feedbackApi } from "../api/dealershipApis";
import { showSuccessToast } from "../services/toastService";
import type {
  SalesFunnelDto,
  StockAgingDto,
  RevenueAnalyticsDto,
  CustomerNotificationDto,
} from "../types/dealershipDtos";

interface ActiveDealDeskItem {
  id: string;
  dealNumber: string;
  customerName: string;
  customerPhone: string;
  unitTitle: string;
  unitVin: string;
  salesRep: string;
  stage: "Quoting" | "Credit Pulled" | "Desking" | "F&I Menu" | "Approved" | "Delivery Ready";
  elapsedMinutes: number;
  grossMargin: number;
  paymentMonthly: number;
  lane: "Sales" | "Fast" | "Menu";
}

const INITIAL_DESK_DEALS: ActiveDealDeskItem[] = [
  {
    id: "deal-1",
    dealNumber: "DL-8421",
    customerName: "Rahul Sharma",
    customerPhone: "+91 98201 44821",
    unitTitle: "2024 Yamaha YZF-R1M",
    unitVin: "JYARN20E7PA004128",
    salesRep: "Arjun Verma",
    stage: "Desking",
    elapsedMinutes: 14,
    grossMargin: 185000,
    paymentMonthly: 38200,
    lane: "Sales",
  },
  {
    id: "deal-2",
    dealNumber: "DL-8422",
    customerName: "Vikram Malhotra",
    customerPhone: "+91 98110 59281",
    unitTitle: "2024 Polaris RZR XP 1000 Sport",
    unitVin: "4XARZR145RP901243",
    salesRep: "Neha Kapoor",
    stage: "F&I Menu",
    elapsedMinutes: 17,
    grossMargin: 245000,
    paymentMonthly: 49800,
    lane: "Menu",
  },
  {
    id: "deal-3",
    dealNumber: "DL-8423",
    customerName: "Ananya Sen",
    customerPhone: "+91 98300 12948",
    unitTitle: "2024 Kawasaki Ninja ZX-10R KRT",
    unitVin: "JKAZX10R8PA771029",
    salesRep: "Rohan Das",
    stage: "Credit Pulled",
    elapsedMinutes: 8,
    grossMargin: 140000,
    paymentMonthly: 29500,
    lane: "Fast",
  },
  {
    id: "deal-4",
    dealNumber: "DL-8424",
    customerName: "Priya Nair",
    customerPhone: "+91 98450 67341",
    unitTitle: "2024 Can-Am Maverick R X RS",
    unitVin: "2BXCANM88RA009381",
    salesRep: "Sanjay Singhania",
    stage: "Approved",
    elapsedMinutes: 19,
    grossMargin: 310000,
    paymentMonthly: 62000,
    lane: "Fast",
  },
  {
    id: "deal-5",
    dealNumber: "DL-8425",
    customerName: "Kabir Mehta",
    customerPhone: "+91 98212 90124",
    unitTitle: "2024 Ducati Panigale V4 S",
    unitVin: "ZDM14B1W3PB001928",
    salesRep: "Arjun Verma",
    stage: "Delivery Ready",
    elapsedMinutes: 21,
    grossMargin: 290000,
    paymentMonthly: 54000,
    lane: "Sales",
  },
];

export default function DashboardPage() {
  const { user, primaryRole, isSuperAdmin, isCompanyAdmin } = useAuth();

  const [, setLoading] = useState(true);
  const [deskDeals, setDeskDeals] = useState<ActiveDealDeskItem[]>(INITIAL_DESK_DEALS);
  const [selectedDeal, setSelectedDeal] = useState<ActiveDealDeskItem | null>(null);

  const [funnel, setFunnel] = useState<SalesFunnelDto>({
    totalQuotations: 128,
    totalOrders: 42,
    pendingLoans: 27,
    approvedLoans: 24,
    completedDeliveries: 18,
    leadToOrderConversionPct: 32.8,
    orderToDeliveryConversionPct: 42.8,
  });

  const [stockAging, setStockAging] = useState<StockAgingDto>({
    totalVehiclesInStock: 316,
    under30Days: 194,
    between31And60Days: 78,
    between61And90Days: 32,
    over90Days: 12,
    totalYardInventoryValue: 425000000,
  });

  const [revenue, setRevenue] = useState<RevenueAnalyticsDto>({
    totalOrderValue: 84000000,
    totalBookingAmountCollected: 2100000,
    totalDownPaymentCollected: 16800000,
    totalLoanDisbursed: 58000000,
    totalAccessoriesRevenue: 1450000,
    totalServiceRevenue: 890000,
  });

  const [, setNotifications] = useState<CustomerNotificationDto[]>([]);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const [f, s, r, n] = await Promise.allSettled([
          analyticsApi.getDashboardFunnel(),
          analyticsApi.getStockAging(),
          analyticsApi.getRevenueAnalytics(),
          feedbackApi.getNotifications(),
        ]);

        if (f.status === "fulfilled" && f.value) setFunnel(f.value);
        if (s.status === "fulfilled" && s.value) setStockAging(s.value);
        if (r.status === "fulfilled" && r.value) setRevenue(r.value);
        if (n.status === "fulfilled" && n.value) setNotifications(n.value);
      } catch (err) {
        console.error("Using baseline metrics for dashboard preview:", err);
      } finally {
        setLoading(false);
      }
    };

    fetchStats();
  }, []);

  const formatCurrency = (amount: number) =>
    new Intl.NumberFormat("en-IN", {
      style: "currency",
      currency: "INR",
      maximumFractionDigits: 0,
    }).format(amount);

  const handleAdvanceStage = (dealId: string) => {
    setDeskDeals((prev) =>
      prev.map((deal) => {
        if (deal.id !== dealId) return deal;
        const stages: ActiveDealDeskItem["stage"][] = [
          "Quoting",
          "Credit Pulled",
          "Desking",
          "F&I Menu",
          "Approved",
          "Delivery Ready",
        ];
        const nextIdx = Math.min(stages.indexOf(deal.stage) + 1, stages.length - 1);
        return { ...deal, stage: stages[nextIdx] };
      })
    );
    showSuccessToast("Deal stage advanced successfully!");
  };

  return (
    <PortalLayout>
      <div className="dashboard-view-container">
        {/* ── Context & Top Operations Header ── */}
        <div className="odl-desking-header-card">
          <div className="header-card-left">
            <div className="brand-eyebrow-row">
              <span className="live-pulse-badge">
                <span className="pulse-dot green" />
                DOL • 144 APIs LIVE • v1.0.1
              </span>
              <span className="campus-badge">
                Apex Powersports Group • Flagship Campus (MUM-BKC-01)
              </span>
            </div>
            <h1 className="desking-main-title">
              Dealer One Lane (DOL) Operating System
            </h1>
            <p className="desking-meta-desc">
              Logged in as <strong>{user?.fullName || "Staff Member"}</strong> ({primaryRole}). Real-time 20-minute deal turnaround desk across Sales Lane, Menu Lane, Fast Lane, and Stock Yard.
            </p>
          </div>

          <div className="header-card-actions">
            <div className="turnaround-target-box">
              <span className="target-label">20-MIN DEAL CLOSE TARGET</span>
              <div className="target-metric-row">
                <strong className="time-val">18.4 min</strong>
                <span className="badge-pill success">94.2% On-Time</span>
              </div>
            </div>
            <Link to="/crm-sales" className="btn btn-primary">
              ⚡ New Deal (+ Lead)
            </Link>
            <Link to="/inventory" className="btn btn-secondary">
              📦 Yard VIN Lookup
            </Link>
          </div>
        </div>

        {/* ── Top Executive KPI Bar (Same as One Dealer Lane Desking Metrics) ── */}
        <section className="stats-kpi-grid">
          <div className="kpi-card highlight-orange">
            <div className="kpi-top">
              <span className="kpi-label">Sales Lane • Active Desk Deals</span>
              <span className="kpi-icon">🎯</span>
            </div>
            <strong className="kpi-value">{funnel.totalQuotations} Deals</strong>
            <small className="kpi-subtext">Avg Turnaround: <strong>18.4 mins</strong> (Target &lt; 20m)</small>
          </div>

          <div className="kpi-card highlight-purple">
            <div className="kpi-top">
              <span className="kpi-label">Menu Lane • F&amp;I PVR Average Gross</span>
              <span className="kpi-icon">💳</span>
            </div>
            <strong className="kpi-value">₹2,34,500</strong>
            <small className="kpi-subtext"><strong>+27%</strong> higher profit margin per customer</small>
          </div>

          <div className="kpi-card highlight-green">
            <div className="kpi-top">
              <span className="kpi-label">Stock Lane • Yard Unit Inventory</span>
              <span className="kpi-icon">🚗</span>
            </div>
            <strong className="kpi-value">{stockAging.totalVehiclesInStock} Units</strong>
            <small className="kpi-subtext">Valued at {formatCurrency(stockAging.totalYardInventoryValue)}</small>
          </div>

          <div className="kpi-card highlight-cyan">
            <div className="kpi-top">
              <span className="kpi-label">Fast Lane • Lender Approvals</span>
              <span className="kpi-icon">📑</span>
            </div>
            <strong className="kpi-value">{funnel.approvedLoans} Approved</strong>
            <small className="kpi-subtext">Disbursed: {formatCurrency(revenue.totalLoanDisbursed)} (88.9%)</small>
          </div>
        </section>

        {/* ── Signature Connected Lanes Pipeline Matrix ── */}
        <div className="odl-stages-pipeline-card">
          <div className="pipeline-header-row">
            <div>
              <h3>One Connected Experience • Deal Stages</h3>
              <p>Continuous data flow across showroom desking, credit bureau, lenders, and F&amp;I warranty menus.</p>
            </div>
            <span className="connected-badge">⚡ Zero Double Entry</span>
          </div>

          <div className="connected-stages-flow">
            <div className="stage-step-item">
              <div className="stage-icon-circle purple">
                <span>📦</span>
              </div>
              <div className="stage-text-block">
                <strong>1. Inventory</strong>
                <small>{stockAging.totalVehiclesInStock} Units on Lot</small>
              </div>
              <div className="stage-arrow">&rarr;</div>
            </div>

            <div className="stage-step-item">
              <div className="stage-icon-circle amber">
                <span>🎯</span>
              </div>
              <div className="stage-text-block">
                <strong>2. CRM &amp; Walk-In</strong>
                <small>{funnel.totalQuotations} Active Quotes</small>
              </div>
              <div className="stage-arrow">&rarr;</div>
            </div>

            <div className="stage-step-item">
              <div className="stage-icon-circle purple">
                <span>📑</span>
              </div>
              <div className="stage-text-block">
                <strong>3. Credit &amp; ID</strong>
                <small>700Credit Bureau Check</small>
              </div>
              <div className="stage-arrow">&rarr;</div>
            </div>

            <div className="stage-step-item">
              <div className="stage-icon-circle amber">
                <span>🚗</span>
              </div>
              <div className="stage-text-block">
                <strong>4. Fast Lane Lenders</strong>
                <small>Multi-Lender Repopulate</small>
              </div>
              <div className="stage-arrow">&rarr;</div>
            </div>

            <div className="stage-step-item">
              <div className="stage-icon-circle purple">
                <span>💳</span>
              </div>
              <div className="stage-text-block">
                <strong>5. Menu Lane F&amp;I</strong>
                <small>eRating &amp; eContracting</small>
              </div>
              <div className="stage-arrow">&rarr;</div>
            </div>

            <div className="stage-step-item">
              <div className="stage-icon-circle green">
                <span>🏁</span>
              </div>
              <div className="stage-text-block">
                <strong>6. Gate Pass</strong>
                <small>{funnel.completedDeliveries} Deliveries Done</small>
              </div>
            </div>
          </div>
        </div>

        {/* ── Active Deal Desk Board (Live 20-Min Deal Timers) ── */}
        <div className="panel-card">
          <div className="panel-card-header">
            <div>
              <h3>Live Deal Desk Floor • Real-Time Deal Tracker</h3>
              <p>Monitor deal desks in progress against the 20-minute turnaround target.</p>
            </div>
            <div className="desk-legend-tags">
              <span className="legend-tag green">⏱️ &lt; 15 mins (Speed Track)</span>
              <span className="legend-tag amber">⏱️ 15-20 mins (Target Zone)</span>
              <span className="legend-tag red">⏱️ &gt; 20 mins (Manager Override)</span>
            </div>
          </div>

          <div className="table-responsive">
            <table className="portal-table">
              <thead>
                <tr>
                  <th>Deal #</th>
                  <th>Customer &amp; Phone</th>
                  <th>Unit &amp; VIN</th>
                  <th>Desk Rep</th>
                  <th>Active Stage</th>
                  <th>Desk Clock</th>
                  <th>Front Gross</th>
                  <th>Monthly Payment</th>
                  <th>Desk Action</th>
                </tr>
              </thead>
              <tbody>
                {deskDeals.map((deal) => {
                  const isOverTarget = deal.elapsedMinutes > 20;
                  const isNearTarget = deal.elapsedMinutes >= 15 && deal.elapsedMinutes <= 20;
                  return (
                    <tr key={deal.id}>
                      <td>
                        <strong className="text-white">{deal.dealNumber}</strong>
                      </td>
                      <td>
                        <div className="cell-primary">{deal.customerName}</div>
                        <small className="cell-secondary">{deal.customerPhone}</small>
                      </td>
                      <td>
                        <div className="cell-primary">{deal.unitTitle}</div>
                        <small className="cell-secondary font-mono">{deal.unitVin}</small>
                      </td>
                      <td>{deal.salesRep}</td>
                      <td>
                        <span className={`status-pill ${
                          deal.stage === "Delivery Ready" || deal.stage === "Approved" 
                            ? "status-success" 
                            : deal.stage === "F&I Menu"
                            ? "status-purple"
                            : "status-warning"
                        }`}>
                          {deal.stage}
                        </span>
                      </td>
                      <td>
                        <span className={`timer-badge ${
                          isOverTarget ? "timer-red" : isNearTarget ? "timer-amber" : "timer-green"
                        }`}>
                          ⏱️ {deal.elapsedMinutes} mins
                        </span>
                      </td>
                      <td>
                        <strong className="text-emerald-400">{formatCurrency(deal.grossMargin)}</strong>
                      </td>
                      <td>
                        <strong>{formatCurrency(deal.paymentMonthly)}/mo</strong>
                      </td>
                      <td>
                        <div className="btn-group-row">
                          <button
                            type="button"
                            className="btn btn-sm btn-primary"
                            onClick={() => handleAdvanceStage(deal.id)}
                            title="Advance to next connected stage"
                          >
                            Advance &rarr;
                          </button>
                          <button
                            type="button"
                            className="btn btn-sm btn-secondary"
                            onClick={() => {
                              setSelectedDeal(deal);
                              showSuccessToast(`Viewing ${deal.dealNumber} details`);
                            }}
                          >
                            Desk 🔍
                          </button>
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
        </div>

        {/* ── Mid Section: Conversion Funnel & Stock Aging Velocity ── */}
        <div className="dashboard-two-col-grid">
          {/* Sales Conversion Funnel */}
          <div className="panel-card">
            <div className="panel-card-header">
              <div>
                <h3>Sales Lane Conversion Telematics</h3>
                <p>Live progression from walk-in quotation to vehicle delivery ceremony.</p>
              </div>
              <span className="live-stat-chip highlight">Auto-Sync</span>
            </div>

            <div className="pipeline-bars-container">
              <div className="pipeline-stage-item">
                <div className="stage-meta">
                  <span>1. On-Road Quotations</span>
                  <strong>{funnel.totalQuotations} Quotes</strong>
                </div>
                <div className="progress-bar-bg">
                  <div className="progress-bar-fill fill-blue" style={{ width: "100%" }} />
                </div>
              </div>

              <div className="pipeline-stage-item">
                <div className="stage-meta">
                  <span>2. Fast Lane Bookings</span>
                  <strong>{funnel.totalOrders} Bookings</strong>
                </div>
                <div className="progress-bar-bg">
                  <div className="progress-bar-fill fill-cyan" style={{ width: "68%" }} />
                </div>
              </div>

              <div className="pipeline-stage-item">
                <div className="stage-meta">
                  <span>3. Menu Lane Underwriting</span>
                  <strong>{funnel.pendingLoans} In-Review ({funnel.approvedLoans} Approved)</strong>
                </div>
                <div className="progress-bar-bg">
                  <div className="progress-bar-fill fill-purple" style={{ width: "52%" }} />
                </div>
              </div>

              <div className="pipeline-stage-item">
                <div className="stage-meta">
                  <span>4. Gate Pass &amp; Customer Handover</span>
                  <strong>{funnel.completedDeliveries} Completed</strong>
                </div>
                <div className="progress-bar-bg">
                  <div className="progress-bar-fill fill-green" style={{ width: "38%" }} />
                </div>
              </div>
            </div>
          </div>

          {/* Yard Inventory Aging Velocity */}
          <div className="panel-card">
            <div className="panel-card-header">
              <div>
                <h3>Stock Lane Yard Aging (VINs)</h3>
                <p>Velocity of units on showroom floor or holding yard.</p>
              </div>
              <span className="live-stat-chip">316 Total Units</span>
            </div>

            <div className="stock-aging-grid">
              <div className="aging-box box-green">
                <strong>{stockAging.under30Days}</strong>
                <span>&lt; 30 Days</span>
                <small>Fast Velocity (61%)</small>
              </div>

              <div className="aging-box box-blue">
                <strong>{stockAging.between31And60Days}</strong>
                <span>31 - 60 Days</span>
                <small>Normal (25%)</small>
              </div>

              <div className="aging-box box-yellow">
                <strong>{stockAging.between61And90Days}</strong>
                <span>61 - 90 Days</span>
                <small>Attention (10%)</small>
              </div>

              <div className="aging-box box-red">
                <strong>{stockAging.over90Days}</strong>
                <span>&gt; 90 Days</span>
                <small>Action Required (4%)</small>
              </div>
            </div>

            <div className="aging-footer-actions">
              <Link to="/inventory" className="text-link">
                View Stock Yard VIN Tracking &rarr;
              </Link>
            </div>
          </div>
        </div>

        {/* ── Bottom Section: Quick Access to 26 Connected Modules ── */}
        <div className="panel-card">
          <div className="panel-card-header">
            <div>
              <h3>Connected Dealership Lanes (26 Integrated Endpoints • 144 APIs)</h3>
              <p>Instant lane dispatch across your cloud operating system.</p>
            </div>
          </div>

          <div className="quick-access-modules-grid">
            <Link to="/crm-sales" className="quick-module-tile orange">
              <div className="tile-icon">🎯</div>
              <div className="tile-content">
                <strong>Sales Lane &amp; CRM</strong>
                <span>Leads, quotes, test drives &amp; desking pipeline</span>
              </div>
              <span className="tile-arrow">&rarr;</span>
            </Link>

            <Link to="/inventory" className="quick-module-tile blue">
              <div className="tile-icon">📦</div>
              <div className="tile-content">
                <strong>Stock Lane &amp; Yard Bay</strong>
                <span>VIN status, staging bays, aging &amp; pricing</span>
              </div>
              <span className="tile-arrow">&rarr;</span>
            </Link>

            <Link to="/orders" className="quick-module-tile cyan">
              <div className="tile-icon">📑</div>
              <div className="tile-content">
                <strong>Fast Lane &amp; Digital Vault</strong>
                <span>Bookings, KYC documents &amp; trade-in valuations</span>
              </div>
              <span className="tile-arrow">&rarr;</span>
            </Link>

            <Link to="/finance" className="quick-module-tile purple">
              <div className="tile-icon">💳</div>
              <div className="tile-content">
                <strong>Menu Lane &amp; F&amp;I</strong>
                <span>Lender disbursements, ledger &amp; warranty rating</span>
              </div>
              <span className="tile-arrow">&rarr;</span>
            </Link>

            <Link to="/aftersales" className="quick-module-tile green">
              <div className="tile-icon">🛠️</div>
              <div className="tile-content">
                <strong>Service Lane &amp; Gate Pass</strong>
                <span>EV batteries, gate passes, complaints &amp; work orders</span>
              </div>
              <span className="tile-arrow">&rarr;</span>
            </Link>

            {(isSuperAdmin || isCompanyAdmin) && (
              <Link to="/admin-setup" className="quick-module-tile slate">
                <div className="tile-icon">🏢</div>
                <div className="tile-content">
                  <strong>Enterprise Tenancy &amp; Masters</strong>
                  <span>Multi-organization setup, branches, RBAC &amp; audit</span>
                </div>
                <span className="tile-arrow">&rarr;</span>
              </Link>
            )}
          </div>
        </div>

        {/* ── Deal Desk Quick Details Modal ── */}
        {selectedDeal && (
          <div className="odl-modal-overlay" onClick={() => setSelectedDeal(null)}>
            <div className="odl-modal-card sm" onClick={(e) => e.stopPropagation()}>
              <button
                type="button"
                className="odl-modal-close-btn"
                onClick={() => setSelectedDeal(null)}
              >
                ✕
              </button>

              <div className="odl-modal-header">
                <span className="modal-badge-eyebrow">DEAL DESK INSPECTOR</span>
                <h2>{selectedDeal.dealNumber} • {selectedDeal.customerName}</h2>
                <p>{selectedDeal.unitTitle} ({selectedDeal.unitVin})</p>
              </div>

              <div className="desking-breakdown-list">
                <div className="breakdown-row">
                  <span>Sales Executive</span>
                  <strong>{selectedDeal.salesRep}</strong>
                </div>
                <div className="breakdown-row">
                  <span>Active Stage</span>
                  <span className="status-pill status-warning">{selectedDeal.stage}</span>
                </div>
                <div className="breakdown-row">
                  <span>Elapsed Turnaround</span>
                  <strong className="text-amber-400">{selectedDeal.elapsedMinutes} mins elapsed</strong>
                </div>
                <div className="breakdown-row">
                  <span>Front Gross Profit</span>
                  <strong className="text-emerald-400">{formatCurrency(selectedDeal.grossMargin)}</strong>
                </div>
                <div className="breakdown-row">
                  <span>Monthly Payment Target</span>
                  <strong>{formatCurrency(selectedDeal.paymentMonthly)}/mo</strong>
                </div>
              </div>

              <div className="form-actions-row mt-4">
                <button
                  type="button"
                  className="odl-btn-primary w-full"
                  onClick={() => {
                    handleAdvanceStage(selectedDeal.id);
                    setSelectedDeal(null);
                  }}
                >
                  Advance Deal Stage &rarr;
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </PortalLayout>
  );
}
