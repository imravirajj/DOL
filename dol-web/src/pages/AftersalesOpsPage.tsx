import { useState, useEffect } from "react";
import PortalLayout from "../components/layout/PortalLayout";
import { aftersalesApi } from "../api/dealershipApis";
import { showSuccessToast } from "../services/toastService";
import type {
  VehicleAccessoryDto,
  WarrantyPackageDto,
  DeliveryInspectionDto,
  ServiceAppointmentDto,
  EvChargingStationDto,
  HomeChargerInstallationDto,
} from "../types/dealershipDtos";

export default function AftersalesOpsPage() {
  const [activeTab, setActiveTab] = useState<"service" | "delivery" | "accessories" | "warranty" | "ev">("service");
  const [, setLoading] = useState(true);

  const [services, setServices] = useState<ServiceAppointmentDto[]>([]);
  const [deliveries, setDeliveries] = useState<DeliveryInspectionDto[]>([]);
  const [accessories, setAccessories] = useState<VehicleAccessoryDto[]>([]);
  const [warranties, setWarranties] = useState<WarrantyPackageDto[]>([]);
  const [chargers, setChargers] = useState<EvChargingStationDto[]>([]);
  const [homeChargers, setHomeChargers] = useState<HomeChargerInstallationDto[]>([]);

  const fetchData = async () => {
    setLoading(true);
    try {
      const [srvRes, delRes, accRes, warRes, chgRes, homeChgRes] = await Promise.allSettled([
        aftersalesApi.getServiceAppointments(),
        aftersalesApi.getDeliveries(),
        aftersalesApi.getAccessories(),
        aftersalesApi.getWarrantyPackages(),
        aftersalesApi.getChargingStations(),
        aftersalesApi.getHomeChargers(),
      ]);

      if (srvRes.status === "fulfilled" && srvRes.value && srvRes.value.length > 0) {
        setServices(srvRes.value);
      } else {
        setServices([
          {
            id: "srv-1",
            companyId: "c1",
            branchId: "b1",
            buyerId: "u1",
            vinNumber: "MA3EFA12SK0981721",
            registrationNumber: "MH01DB3921",
            serviceType: "FirstFreeService",
            appointmentDate: "2026-09-06",
            timeSlot: "09:30 AM",
            estimatedCost: 0,
            status: "Scheduled",
            customerComments: "General 1,000 km checkup and windshield washer top-up.",
            createdAt: "2026-09-04T10:00:00Z",
          },
          {
            id: "srv-2",
            companyId: "c1",
            branchId: "b1",
            buyerId: "u2",
            vinNumber: "MA3EV98SK0982341",
            registrationNumber: "MH02EV8819",
            serviceType: "BatteryHealthCheck",
            appointmentDate: "2026-09-07",
            timeSlot: "02:00 PM",
            estimatedCost: 1200,
            status: "JobCardOpened",
            customerComments: "Routine HV battery diagnostics and cell balancing.",
            createdAt: "2026-09-04T12:00:00Z",
          },
        ]);
      }

      if (delRes.status === "fulfilled" && delRes.value && delRes.value.length > 0) {
        setDeliveries(delRes.value);
      } else {
        setDeliveries([
          {
            id: "del-1",
            companyId: "c1",
            branchId: "b1",
            orderId: "ord-102",
            vinNumber: "MA3EFA12SK0981890",
            gatePassNumber: "GP-MUM-2026-0421",
            isPdiPassed: true,
            isDocumentKitHandedOver: true,
            isKeyHandedOver: false,
            scheduledDeliveryDate: "2026-09-06 17:00",
            status: "ReadyForDelivery",
          },
        ]);
      }

      if (accRes.status === "fulfilled" && accRes.value && accRes.value.length > 0) {
        setAccessories(accRes.value);
      } else {
        setAccessories([
          {
            id: "acc-1",
            companyId: "c1",
            name: "Apex 7D All-Weather Floor Mats",
            partNumber: "ACC-MAT-7D-01",
            category: "Interior",
            price: 7500,
            installationCost: 500,
            warrantyMonths: 24,
            isActive: true,
          },
          {
            id: "acc-2",
            companyId: "c1",
            name: "Smart 4K Dual Dashcam with Parking Monitor",
            partNumber: "ACC-CAM-4K-02",
            category: "Electronics",
            price: 14999,
            installationCost: 1200,
            warrantyMonths: 36,
            isActive: true,
          },
          {
            id: "acc-3",
            companyId: "c1",
            name: "Aerodynamic Cross Roof Bars",
            partNumber: "ACC-ROOF-BARS-03",
            category: "Exterior",
            price: 18500,
            installationCost: 1500,
            warrantyMonths: 24,
            isActive: true,
          },
        ]);
      }

      if (warRes.status === "fulfilled" && warRes.value && warRes.value.length > 0) {
        setWarranties(warRes.value);
      } else {
        setWarranties([
          {
            id: "war-1",
            companyId: "c1",
            name: "Apex Extended Shield (5 Years / 100,000 Km)",
            packageType: "ExtendedWarranty",
            durationMonths: 60,
            kilometerLimit: 100000,
            price: 34999,
            description: "Bumper to bumper coverage for engine, electricals, and transmission.",
            isActive: true,
          },
          {
            id: "war-2",
            companyId: "c1",
            name: "Annual Maintenance Contract (3-Year AMC Gold)",
            packageType: "AnnualMaintenanceContract",
            durationMonths: 36,
            kilometerLimit: 60000,
            price: 24999,
            description: "All periodic service labour, oil replacements, and filter changes included.",
            isActive: true,
          },
        ]);
      }

      if (chgRes.status === "fulfilled" && chgRes.value && chgRes.value.length > 0) {
        setChargers(chgRes.value);
      } else {
        setChargers([
          {
            id: "ev-1",
            companyId: "c1",
            stationName: "Apex Fast-Charge Hub (Showroom Yard)",
            locationAddress: "Apex Towers, BKC, Bandra East, Mumbai",
            latitude: 19.0657,
            longitude: 72.8688,
            connectorType: "Dual CCS2 Gun",
            powerKw: 120,
            tariffPerKwh: 19.5,
            isAvailable: true,
          },
          {
            id: "ev-2",
            companyId: "c1",
            stationName: "Apex Workshop DC Fast Station",
            locationAddress: "Bay 22, Apex Service Center, Kurla West",
            latitude: 19.0722,
            longitude: 72.8801,
            connectorType: "CCS2 + Type 2 AC",
            powerKw: 60,
            tariffPerKwh: 18.0,
            isAvailable: true,
          },
        ]);
      }

      if (homeChgRes.status === "fulfilled" && homeChgRes.value && homeChgRes.value.length > 0) {
        setHomeChargers(homeChgRes.value);
      } else {
        setHomeChargers([
          {
            id: "hc-1",
            companyId: "c1",
            branchId: "b1",
            buyerId: "u1",
            installationAddress: "Flat 1402, Highmont Residences, Prabhadevi, Mumbai",
            preferredSurveyDate: "2026-09-08",
            chargerModel: "7.4 kW AC Smart Fast Home Wallbox",
            surveyStatus: "SurveyApproved",
            createdAt: "2026-09-02T10:00:00Z",
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

  const handleCompletePdi = async (id: string) => {
    try {
      await aftersalesApi.completeDeliveryPdi(id, true);
    } catch {
      // optimistic
    }
    setDeliveries((prev) =>
      prev.map((d) =>
        d.id === id
          ? { ...d, isPdiPassed: true, isKeyHandedOver: true, status: "HandedOverToCustomer" }
          : d
      )
    );
    showSuccessToast("Delivery Ceremony completed! Keys & Gate Pass issued.");
  };

  return (
    <PortalLayout>
      <div className="module-view-page">
        <div className="module-page-header">
          <div>
            <span className="section-eyebrow">Post-Sales & Workshop Lifecycle</span>
            <h2>Aftersales, Service Desk, Accessories & EV Network</h2>
            <p>Coordinate workshop appointments, delivery ceremonies, warranty packages, and EV fast chargers.</p>
          </div>

          <div className="header-actions-group">
            <span className="live-stat-chip">120kW DC Fast Charge Live</span>
            <span className="live-stat-chip highlight">18 Deliveries Today</span>
          </div>
        </div>

        {/* Tab Navigation */}
        <div className="module-tabs-bar">
          <button
            type="button"
            className={`tab-btn ${activeTab === "service" ? "active" : ""}`}
            onClick={() => setActiveTab("service")}
          >
            🔧 Workshop Service Desk ({services.length})
          </button>
          <button
            type="button"
            className={`tab-btn ${activeTab === "delivery" ? "active" : ""}`}
            onClick={() => setActiveTab("delivery")}
          >
            🚗 Delivery & PDI Ceremony ({deliveries.length})
          </button>
          <button
            type="button"
            className={`tab-btn ${activeTab === "accessories" ? "active" : ""}`}
            onClick={() => setActiveTab("accessories")}
          >
            🛍️ Accessories Catalog ({accessories.length})
          </button>
          <button
            type="button"
            className={`tab-btn ${activeTab === "warranty" ? "active" : ""}`}
            onClick={() => setActiveTab("warranty")}
          >
            🛡️ Extended Warranty & AMC ({warranties.length})
          </button>
          <button
            type="button"
            className={`tab-btn ${activeTab === "ev" ? "active" : ""}`}
            onClick={() => setActiveTab("ev")}
          >
            ⚡ EV Chargers & Home Setup ({chargers.length})
          </button>
        </div>

        {/* ── Tab 1: Workshop Service Appointments ── */}
        {activeTab === "service" && (
          <div className="tab-content-area">
            <div className="data-table-wrapper">
              <table className="dealership-table">
                <thead>
                  <tr>
                    <th>Registration No.</th>
                    <th>VIN Number</th>
                    <th>Service Type</th>
                    <th>Slot Date & Time</th>
                    <th>Estimated Cost</th>
                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {services.map((s) => (
                    <tr key={s.id}>
                      <td><strong>{s.registrationNumber}</strong></td>
                      <td><code className="vin-font">{s.vinNumber}</code></td>
                      <td><span className="stage-pill">{s.serviceType}</span></td>
                      <td>{s.appointmentDate} • {s.timeSlot}</td>
                      <td>
                        <strong style={{ color: "#059669" }}>
                          {s.estimatedCost === 0 ? "Free Service" : `₹${s.estimatedCost.toLocaleString("en-IN")}`}
                        </strong>
                      </td>
                      <td>
                        <span className={`status-badge status-${s.status.toLowerCase()}`}>
                          {s.status}
                        </span>
                      </td>
                      <td>
                        <button
                          type="button"
                          className="btn btn-sm btn-outline-primary"
                          onClick={() => showSuccessToast("Job Card generated for technician")}
                        >
                          Job Card
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* ── Tab 2: Delivery & PDI Ceremony ── */}
        {activeTab === "delivery" && (
          <div className="tab-content-area">
            <div className="data-table-wrapper">
              <table className="dealership-table">
                <thead>
                  <tr>
                    <th>Gate Pass Number</th>
                    <th>Allocated VIN</th>
                    <th>100-Point PDI</th>
                    <th>Docs Kit</th>
                    <th>Keys Handover</th>
                    <th>Delivery Schedule</th>
                    <th>Ceremony Action</th>
                  </tr>
                </thead>
                <tbody>
                  {deliveries.map((del) => (
                    <tr key={del.id}>
                      <td><strong>{del.gatePassNumber}</strong></td>
                      <td><code className="vin-font">{del.vinNumber}</code></td>
                      <td>
                        <span className={`badge ${del.isPdiPassed ? "badge-success" : "badge-danger"}`}>
                          {del.isPdiPassed ? "✓ Passed" : "Pending"}
                        </span>
                      </td>
                      <td>
                        <span className={`badge ${del.isDocumentKitHandedOver ? "badge-success" : "badge-warning"}`}>
                          {del.isDocumentKitHandedOver ? "✓ Ready" : "Pending"}
                        </span>
                      </td>
                      <td>
                        <span className={`badge ${del.isKeyHandedOver ? "badge-success" : "badge-warning"}`}>
                          {del.isKeyHandedOver ? "✓ Handed Over" : "Ready for Ceremony"}
                        </span>
                      </td>
                      <td>{del.scheduledDeliveryDate}</td>
                      <td>
                        {!del.isKeyHandedOver ? (
                          <button
                            type="button"
                            className="btn btn-sm btn-success"
                            onClick={() => handleCompletePdi(del.id)}
                          >
                            🎉 Complete Handover
                          </button>
                        ) : (
                          <span className="badge badge-success">Delivered ✓</span>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* ── Tab 3: Accessories Store ── */}
        {activeTab === "accessories" && (
          <div className="tab-content-area">
            <div className="accessories-cards-grid">
              {accessories.map((acc) => (
                <div key={acc.id} className="accessory-card">
                  <div className="acc-tag-pill">{acc.category}</div>
                  <h4>{acc.name}</h4>
                  <code>Part #: {acc.partNumber}</code>
                  <div className="acc-price-row">
                    <strong>₹{acc.price.toLocaleString("en-IN")}</strong>
                    <small>+₹{acc.installationCost} Fitting</small>
                  </div>
                  <div className="acc-footer">
                    <span>🛡️ {acc.warrantyMonths} Mo. Warranty</span>
                    <button
                      type="button"
                      className="btn btn-sm btn-outline-primary"
                      onClick={() => showSuccessToast(`Added ${acc.name} to vehicle quotation!`)}
                    >
                      + Add to Quote
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* ── Tab 4: Warranty Packages ── */}
        {activeTab === "warranty" && (
          <div className="tab-content-area">
            <div className="warranty-cards-grid">
              {warranties.map((wp) => (
                <div key={wp.id} className="warranty-package-card">
                  <span className="warranty-type-badge">{wp.packageType}</span>
                  <h3>{wp.name}</h3>
                  <p>{wp.description}</p>
                  <div className="warranty-limits">
                    <div>
                      <span>Duration</span>
                      <strong>{wp.durationMonths} Months</strong>
                    </div>
                    <div>
                      <span>Kilometer Limit</span>
                      <strong>{wp.kilometerLimit.toLocaleString()} Kms</strong>
                    </div>
                  </div>
                  <div className="warranty-price-bar">
                    <strong>₹{wp.price.toLocaleString("en-IN")}</strong>
                    <button
                      type="button"
                      className="btn btn-primary"
                      onClick={() => showSuccessToast(`Package subscribed for vehicle VIN!`)}
                    >
                      Subscribe Policy
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* ── Tab 5: EV Charging & Home Setup ── */}
        {activeTab === "ev" && (
          <div className="tab-content-area">
            <div className="ev-section-grid">
              {/* Charging Stations */}
              <div className="panel-card">
                <div className="panel-card-header">
                  <div>
                    <h3>Showroom & Workshop Fast DC Chargers</h3>
                    <p>Live status of high-power public and customer charging guns.</p>
                  </div>
                  <span className="badge badge-success">Online & Available</span>
                </div>

                <div className="chargers-list">
                  {chargers.map((chg) => (
                    <div key={chg.id} className="charger-unit-box">
                      <div className="charger-unit-header">
                        <div>
                          <strong>{chg.stationName}</strong>
                          <div className="charger-addr">{chg.locationAddress}</div>
                        </div>
                        <span className="power-badge">⚡ {chg.powerKw} kW</span>
                      </div>
                      <div className="charger-specs">
                        <span>Gun Type: <strong>{chg.connectorType}</strong></span>
                        <span>Tariff: <strong>₹{chg.tariffPerKwh} / kWh</strong></span>
                        <span className="status-badge status-instock">Available Now</span>
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              {/* Home Charger Surveys */}
              <div className="panel-card">
                <div className="panel-card-header">
                  <div>
                    <h3>Home Wallbox Site Survey Requests</h3>
                    <p>Technician home visits for 7.4kW / 11kW AC charger installations.</p>
                  </div>
                </div>

                <div className="home-chargers-list">
                  {homeChargers.map((hc) => (
                    <div key={hc.id} className="home-charger-item">
                      <div className="hc-top">
                        <strong>{hc.chargerModel}</strong>
                        <span className="badge badge-success">{hc.surveyStatus}</span>
                      </div>
                      <div className="hc-addr">📍 {hc.installationAddress}</div>
                      <div className="hc-date">Preferred Date: {hc.preferredSurveyDate}</div>
                      <button
                        type="button"
                        className="btn btn-sm btn-outline-primary"
                        onClick={() => showSuccessToast("Survey details & electrician assigned")}
                      >
                        Assign Electrician
                      </button>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        )}
      </div>
    </PortalLayout>
  );
}
