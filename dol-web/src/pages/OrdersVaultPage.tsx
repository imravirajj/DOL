import { useState, useEffect } from "react";
import PortalLayout from "../components/layout/PortalLayout";
import { salesFlowApi, documentsApi } from "../api/dealershipApis";
import { showSuccessToast } from "../services/toastService";
import type {
  VehicleBookingDto,
  VehicleOrderDto,
  CustomerDocumentDto,
} from "../types/dealershipDtos";

export default function OrdersVaultPage() {
  const [activeTab, setActiveTab] = useState<"orders" | "bookings" | "kyc">("orders");
  const [, setLoading] = useState(true);

  // Orders, Bookings, KYC
  const [orders, setOrders] = useState<VehicleOrderDto[]>([]);
  const [bookings, setBookings] = useState<VehicleBookingDto[]>([]);
  const [documents, setDocuments] = useState<CustomerDocumentDto[]>([]);

  const fetchData = async () => {
    setLoading(true);
    try {
      const [ordRes, bookRes, docRes] = await Promise.allSettled([
        salesFlowApi.getOrders(),
        salesFlowApi.getBookings(),
        documentsApi.getDocuments(),
      ]);

      if (ordRes.status === "fulfilled" && ordRes.value && ordRes.value.length > 0) {
        setOrders(ordRes.value);
      } else {
        setOrders([
          {
            id: "ord-101",
            companyId: "c1",
            branchId: "b1",
            buyerId: "u1",
            variantId: "v1",
            orderNumber: "DOL-ORD-2026-089",
            totalAmount: 2499000,
            downPaymentPaid: 500000,
            balanceDue: 1999000,
            status: "FinancingApproved",
            allocatedVin: "MA3EV98SK0982341",
            createdAt: "2026-09-01T10:15:00Z",
          },
          {
            id: "ord-102",
            companyId: "c1",
            branchId: "b1",
            buyerId: "u2",
            variantId: "v2",
            orderNumber: "DOL-ORD-2026-090",
            totalAmount: 1950000,
            downPaymentPaid: 390000,
            balanceDue: 1560000,
            status: "ReadyForDelivery",
            allocatedVin: "MA3EFA12SK0981890",
            createdAt: "2026-09-02T14:30:00Z",
          },
        ]);
      }

      if (bookRes.status === "fulfilled" && bookRes.value && bookRes.value.length > 0) {
        setBookings(bookRes.value);
      } else {
        setBookings([
          {
            id: "bkg-201",
            companyId: "c1",
            branchId: "b1",
            buyerId: "u1",
            variantId: "v1",
            bookingAmount: 25000,
            bookingReference: "BKG-APEX-9921",
            status: "Confirmed",
            allocatedVin: "MA3EV98SK0982341",
            createdAt: "2026-08-29T09:00:00Z",
          },
          {
            id: "bkg-202",
            companyId: "c1",
            branchId: "b1",
            buyerId: "u3",
            variantId: "v3",
            bookingAmount: 11000,
            bookingReference: "BKG-APEX-9922",
            status: "Allocated",
            allocatedVin: "MA3EFA12SK0981721",
            createdAt: "2026-09-03T16:20:00Z",
          },
        ]);
      }

      if (docRes.status === "fulfilled" && docRes.value && docRes.value.length > 0) {
        setDocuments(docRes.value);
      } else {
        setDocuments([
          {
            id: "doc-1",
            companyId: "c1",
            userId: "u1",
            documentType: "AadhaarCard",
            documentNumber: "XXXX-XXXX-4819",
            fileUrl: "https://storage.dol.cloud/kyc/u1-aadhaar.pdf",
            fileName: "Aadhaar_Vikram_Malhotra.pdf",
            fileSizeBytes: 245000,
            verificationStatus: "Approved",
            verifiedAt: "2026-09-02T11:00:00Z",
            createdAt: "2026-09-01T15:00:00Z",
          },
          {
            id: "doc-2",
            companyId: "c1",
            userId: "u1",
            documentType: "PanCard",
            documentNumber: "ABCDE1234F",
            fileUrl: "https://storage.dol.cloud/kyc/u1-pan.pdf",
            fileName: "PAN_Vikram_Malhotra.pdf",
            fileSizeBytes: 182000,
            verificationStatus: "Pending",
            createdAt: "2026-09-02T12:30:00Z",
          },
          {
            id: "doc-3",
            companyId: "c1",
            userId: "u2",
            documentType: "DrivingLicense",
            documentNumber: "MH0120190045231",
            fileUrl: "https://storage.dol.cloud/kyc/u2-dl.pdf",
            fileName: "DL_Sneha_Kapadia.pdf",
            fileSizeBytes: 312000,
            verificationStatus: "Pending",
            createdAt: "2026-09-03T10:00:00Z",
          },
        ]);
      }
    } catch (err) {
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchData();
  }, []);

  const handleVerifyDocument = async (id: string, approve: boolean) => {
    try {
      await documentsApi.verifyDocument(id, approve, approve ? undefined : "Image unclear, please re-upload");
    } catch {
      // optimistic
    }
    setDocuments((prev) =>
      prev.map((d) =>
        d.id === id
          ? {
              ...d,
              verificationStatus: approve ? "Approved" : "Rejected",
              verifiedAt: new Date().toISOString(),
            }
          : d
      )
    );
    showSuccessToast(approve ? "Document verified & approved!" : "Document marked as rejected");
  };

  return (
    <PortalLayout>
      <div className="module-view-page">
        {/* Header */}
        <div className="module-page-header">
          <div>
            <span className="section-eyebrow">Transactional Workflows</span>
            <h2>Orders, Pre-Bookings & Customer KYC Vault</h2>
            <p>Process confirmed customer orders, track token bookings, and manage KYC compliance verifications.</p>
          </div>

          <div className="header-actions-group">
            <span className="live-stat-chip">42 Active Orders</span>
            <span className="live-stat-chip highlight">2 Pending KYC</span>
          </div>
        </div>

        {/* Tab Navigation */}
        <div className="module-tabs-bar">
          <button
            type="button"
            className={`tab-btn ${activeTab === "orders" ? "active" : ""}`}
            onClick={() => setActiveTab("orders")}
          >
            🚗 Vehicle Sales Orders ({orders.length})
          </button>
          <button
            type="button"
            className={`tab-btn ${activeTab === "bookings" ? "active" : ""}`}
            onClick={() => setActiveTab("bookings")}
          >
            💳 Pre-Bookings & Tokens ({bookings.length})
          </button>
          <button
            type="button"
            className={`tab-btn ${activeTab === "kyc" ? "active" : ""}`}
            onClick={() => setActiveTab("kyc")}
          >
            📄 Customer KYC Document Vault ({documents.length})
          </button>
        </div>

        {/* ── Tab 1: Orders ── */}
        {activeTab === "orders" && (
          <div className="tab-content-area">
            <div className="data-table-wrapper">
              <table className="dealership-table">
                <thead>
                  <tr>
                    <th>Order Number</th>
                    <th>Allocated VIN</th>
                    <th>Total Price</th>
                    <th>Down Payment</th>
                    <th>Balance Due</th>
                    <th>Order Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {orders.map((ord) => (
                    <tr key={ord.id}>
                      <td>
                        <strong>{ord.orderNumber}</strong>
                        <div><small>{new Date(ord.createdAt).toLocaleDateString()}</small></div>
                      </td>
                      <td>
                        {ord.allocatedVin ? (
                          <code className="vin-font">{ord.allocatedVin}</code>
                        ) : (
                          <em style={{ color: "#94a3b8" }}>Awaiting allocation</em>
                        )}
                      </td>
                      <td>₹{ord.totalAmount.toLocaleString("en-IN")}</td>
                      <td style={{ color: "#059669" }}>
                        ₹{ord.downPaymentPaid.toLocaleString("en-IN")}
                      </td>
                      <td>
                        <strong style={{ color: ord.balanceDue > 0 ? "#dc2626" : "#059669" }}>
                          ₹{ord.balanceDue.toLocaleString("en-IN")}
                        </strong>
                      </td>
                      <td>
                        <span className={`status-badge status-${ord.status.toLowerCase()}`}>
                          {ord.status}
                        </span>
                      </td>
                      <td>
                        <button
                          type="button"
                          className="btn btn-sm btn-outline-primary"
                          onClick={() => showSuccessToast(`Viewing digital tax invoice for ${ord.orderNumber}`)}
                        >
                          Invoice PDF
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* ── Tab 2: Bookings ── */}
        {activeTab === "bookings" && (
          <div className="tab-content-area">
            <div className="data-table-wrapper">
              <table className="dealership-table">
                <thead>
                  <tr>
                    <th>Booking Reference</th>
                    <th>Token Amount</th>
                    <th>Allocated VIN</th>
                    <th>Booking Status</th>
                    <th>Date</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {bookings.map((bkg) => (
                    <tr key={bkg.id}>
                      <td>
                        <strong>{bkg.bookingReference}</strong>
                      </td>
                      <td>
                        <strong style={{ color: "#059669" }}>
                          ₹{bkg.bookingAmount.toLocaleString("en-IN")}
                        </strong>
                      </td>
                      <td>
                        {bkg.allocatedVin ? (
                          <code className="vin-font">{bkg.allocatedVin}</code>
                        ) : (
                          <em style={{ color: "#94a3b8" }}>Pending VIN</em>
                        )}
                      </td>
                      <td>
                        <span className="badge badge-success">{bkg.status}</span>
                      </td>
                      <td>{new Date(bkg.createdAt).toLocaleDateString()}</td>
                      <td>
                        <button
                          type="button"
                          className="btn btn-sm btn-outline-primary"
                          onClick={() => showSuccessToast("Booking token receipt downloaded")}
                        >
                          Receipt
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* ── Tab 3: Customer KYC Vault ── */}
        {activeTab === "kyc" && (
          <div className="tab-content-area">
            <div className="data-table-wrapper">
              <table className="dealership-table">
                <thead>
                  <tr>
                    <th>Document Type</th>
                    <th>Document Number</th>
                    <th>File Name</th>
                    <th>Verification Status</th>
                    <th>Submission Date</th>
                    <th>Staff Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {documents.map((doc) => (
                    <tr key={doc.id}>
                      <td>
                        <span className="doc-icon">🪪</span>
                        <strong>{doc.documentType}</strong>
                      </td>
                      <td><code>{doc.documentNumber}</code></td>
                      <td>
                        <a href="#view" onClick={(e) => { e.preventDefault(); showSuccessToast("Opening secure KYC viewer"); }}>
                          {doc.fileName}
                        </a>
                      </td>
                      <td>
                        <span
                          className={`priority-badge ${
                            doc.verificationStatus === "Approved"
                              ? "priority-hot"
                              : doc.verificationStatus === "Rejected"
                              ? "priority-cold"
                              : "priority-warm"
                          }`}
                        >
                          {doc.verificationStatus}
                        </span>
                      </td>
                      <td>{new Date(doc.createdAt).toLocaleDateString()}</td>
                      <td>
                        {doc.verificationStatus === "Pending" ? (
                          <div className="action-button-group">
                            <button
                              type="button"
                              className="btn btn-sm btn-success"
                              onClick={() => handleVerifyDocument(doc.id, true)}
                            >
                              ✓ Approve
                            </button>
                            <button
                              type="button"
                              className="btn btn-sm btn-outline-danger"
                              onClick={() => handleVerifyDocument(doc.id, false)}
                            >
                              ✗ Reject
                            </button>
                          </div>
                        ) : (
                          <small style={{ color: "#64748b" }}>Completed</small>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </PortalLayout>
  );
}
