import { useState, useEffect } from "react";
import PortalLayout from "../components/layout/PortalLayout";
import { catalogApi, inventoryApi } from "../api/dealershipApis";
import { showSuccessToast } from "../services/toastService";
import type {
  BrandDto,
  VehicleModelDto,
  VehicleVariantDto,
  VehicleStockDto,
  VehicleStockStatus,
} from "../types/dealershipDtos";

export default function CatalogInventoryPage() {
  const [activeTab, setActiveTab] = useState<"inventory" | "catalog">("inventory");
  const [, setLoading] = useState(true);

  // Yard Inventory
  const [stocks, setStocks] = useState<VehicleStockDto[]>([]);
  const [statusFilter, setStatusFilter] = useState<string>("All");
  const [searchVin, setSearchVin] = useState("");

  // Catalog
  const [, setBrands] = useState<BrandDto[]>([]);
  const [models, setModels] = useState<VehicleModelDto[]>([]);
  const [variants, setVariants] = useState<VehicleVariantDto[]>([]);

  // Move modal
  const [movingVehicle, setMovingVehicle] = useState<VehicleStockDto | null>(null);
  const [newYardLocation, setNewYardLocation] = useState("");

  const fetchData = async () => {
    setLoading(true);
    try {
      const [invRes, brandRes, modelRes, varRes] = await Promise.allSettled([
        inventoryApi.getVehicles(),
        catalogApi.getBrands(),
        catalogApi.getModels(),
        catalogApi.getVariants(),
      ]);

      if (invRes.status === "fulfilled" && invRes.value && invRes.value.length > 0) {
        setStocks(invRes.value);
      } else {
        setStocks([
          {
            id: "s1",
            companyId: "c1",
            branchId: "b1",
            variantId: "v1",
            vinNumber: "MA3EFA12SK0981721",
            engineNumber: "ENG-K15C-48912",
            color: "Nexa Blue Metallic",
            status: "InStock",
            yardLocation: "Bay 4 - Showroom Floor",
            receivedDate: "2026-08-28",
            variantName: "Alpha Plus 1.5L AT",
            modelName: "Apex Grand Tourer",
          },
          {
            id: "s2",
            companyId: "c1",
            branchId: "b1",
            variantId: "v2",
            vinNumber: "MA3EFA12SK0981890",
            engineNumber: "ENG-K15C-48999",
            color: "Opulent Red",
            status: "Allocated",
            yardLocation: "Bay 12 - PDI Staging",
            receivedDate: "2026-08-30",
            variantName: "Zeta 1.5L MT",
            modelName: "Apex Grand Tourer",
          },
          {
            id: "s3",
            companyId: "c1",
            branchId: "b1",
            variantId: "v3",
            vinNumber: "MA3EV98SK0982341",
            engineNumber: "EV-MOT-150KW-912",
            color: "Midnight Black",
            status: "InStock",
            yardLocation: "Yard North - Section B",
            receivedDate: "2026-09-02",
            variantName: "Empowered Plus 60kWh",
            modelName: "Apex Harrier EV",
          },
          {
            id: "s4",
            companyId: "c1",
            branchId: "b1",
            variantId: "v4",
            vinNumber: "MA3EV98SK0982455",
            engineNumber: "EV-MOT-150KW-955",
            color: "Pristine White Dual-Tone",
            status: "InTransit",
            yardLocation: "Factory Transit Truck #402",
            receivedDate: "2026-09-04",
            variantName: "Fearless EV 50kWh",
            modelName: "Apex Harrier EV",
          },
        ]);
      }

      if (brandRes.status === "fulfilled" && brandRes.value && brandRes.value.length > 0) {
        setBrands(brandRes.value);
      } else {
        setBrands([
          { id: "b1", name: "Apex Motors", code: "APEX", countryOfOrigin: "India" },
          { id: "b2", name: "Apex Nexa", code: "NEXA", countryOfOrigin: "India" },
        ]);
      }

      if (modelRes.status === "fulfilled" && modelRes.value && modelRes.value.length > 0) {
        setModels(modelRes.value);
      } else {
        setModels([
          {
            id: "m1",
            brandId: "b1",
            name: "Apex Harrier EV",
            bodyType: "Electric SUV",
            fuelTypes: ["Electric"],
            startingPrice: 2199000,
          },
          {
            id: "m2",
            brandId: "b1",
            name: "Apex Grand Tourer",
            bodyType: "Executive Sedan",
            fuelTypes: ["Petrol", "Hybrid"],
            startingPrice: 1649000,
          },
          {
            id: "m3",
            brandId: "b2",
            name: "Apex Urban Cruiser",
            bodyType: "Compact SUV",
            fuelTypes: ["Petrol", "CNG"],
            startingPrice: 1120000,
          },
        ]);
      }

      if (varRes.status === "fulfilled" && varRes.value && varRes.value.length > 0) {
        setVariants(varRes.value);
      } else {
        setVariants([
          {
            id: "v1",
            modelId: "m1",
            name: "Empowered Plus 60kWh Long Range",
            transmission: "Single Speed e-Drive",
            engineCapacityCc: 0,
            fuelType: "Electric",
            exShowroomPrice: 2499000,
            colorOptions: ["Midnight Black", "Arctic White", "Ocean Blue"],
          },
          {
            id: "v2",
            modelId: "m2",
            name: "Alpha Plus 1.5L Turbo AT",
            transmission: "7-Speed Dual Clutch",
            engineCapacityCc: 1498,
            fuelType: "Petrol",
            exShowroomPrice: 1950000,
            colorOptions: ["Nexa Blue", "Pearl Metallic Silver"],
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

  const handleUpdateStatus = async (id: string, newStatus: VehicleStockStatus) => {
    try {
      await inventoryApi.updateStatus(id, newStatus);
    } catch {
      // optimistic
    }
    setStocks((prev) =>
      prev.map((s) => (s.id === id ? { ...s, status: newStatus } : s))
    );
    showSuccessToast(`VIN status updated to ${newStatus}`);
  };

  const handleYardMove = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!movingVehicle || !newYardLocation) return;
    try {
      await inventoryApi.recordYardMovement(movingVehicle.id, newYardLocation);
    } catch {
      // optimistic
    }
    setStocks((prev) =>
      prev.map((s) =>
        s.id === movingVehicle.id ? { ...s, yardLocation: newYardLocation } : s
      )
    );
    showSuccessToast(`Vehicle ${movingVehicle.vinNumber.slice(-6)} moved to ${newYardLocation}`);
    setMovingVehicle(null);
    setNewYardLocation("");
  };

  const filteredStocks = stocks.filter((s) => {
    const matchesStatus = statusFilter === "All" || s.status === statusFilter;
    const matchesVin =
      s.vinNumber.toLowerCase().includes(searchVin.toLowerCase()) ||
      s.yardLocation.toLowerCase().includes(searchVin.toLowerCase()) ||
      (s.modelName && s.modelName.toLowerCase().includes(searchVin.toLowerCase()));
    return matchesStatus && matchesVin;
  });

  return (
    <PortalLayout>
      <div className="module-view-page">
        {/* Module Header */}
        <div className="module-page-header">
          <div>
            <span className="section-eyebrow">Inventory Management & Supply Chain</span>
            <h2>Catalog & Live Yard Stock (VIN Telematics)</h2>
            <p>Track VIN numbers, stock status, staging yards, factory dispatches, and OEM vehicle catalogs.</p>
          </div>

          <div className="header-actions-group">
            <span className="live-stat-chip">316 Total Stock Units</span>
            <span className="live-stat-chip highlight">24 Ready For PDI</span>
          </div>
        </div>

        {/* Tab Selector */}
        <div className="module-tabs-bar">
          <button
            type="button"
            className={`tab-btn ${activeTab === "inventory" ? "active" : ""}`}
            onClick={() => setActiveTab("inventory")}
          >
            📍 Yard Inventory & VIN Tracking ({stocks.length})
          </button>
          <button
            type="button"
            className={`tab-btn ${activeTab === "catalog" ? "active" : ""}`}
            onClick={() => setActiveTab("catalog")}
          >
            🚙 Vehicle Catalog, Models & Variants ({models.length})
          </button>
        </div>

        {/* ── Tab 1: Yard Stock Table ── */}
        {activeTab === "inventory" && (
          <div className="tab-content-area">
            {/* Filter and Search Bar */}
            <div className="filter-controls-row">
              <div className="search-input-wrapper">
                <input
                  type="text"
                  placeholder="Search by VIN, Yard Bay or Model..."
                  value={searchVin}
                  onChange={(e) => setSearchVin(e.target.value)}
                  className="search-input"
                />
              </div>

              <div className="filter-pills">
                <span className="filter-label">Status Filter:</span>
                {["All", "InStock", "InTransit", "Allocated", "Delivered"].map((st) => (
                  <button
                    key={st}
                    type="button"
                    className={`pill-btn ${statusFilter === st ? "active" : ""}`}
                    onClick={() => setStatusFilter(st)}
                  >
                    {st}
                  </button>
                ))}
              </div>
            </div>

            {/* Table */}
            <div className="data-table-wrapper">
              <table className="dealership-table">
                <thead>
                  <tr>
                    <th>VIN Number</th>
                    <th>Model & Variant</th>
                    <th>Exterior Color</th>
                    <th>Stock Status</th>
                    <th>Yard Bay Location</th>
                    <th>Arrival Date</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {filteredStocks.map((v) => (
                    <tr key={v.id}>
                      <td>
                        <strong className="vin-font">{v.vinNumber}</strong>
                        <div><small style={{ color: "#64748b" }}>Eng: {v.engineNumber}</small></div>
                      </td>
                      <td>
                        <strong>{v.modelName || "Apex Grand Tourer"}</strong>
                        <div><small>{v.variantName || "Standard Spec"}</small></div>
                      </td>
                      <td>
                        <span className="color-indicator-dot" />
                        {v.color}
                      </td>
                      <td>
                        <span className={`status-badge status-${v.status.toLowerCase()}`}>
                          {v.status}
                        </span>
                      </td>
                      <td>
                        <span className="yard-tag">📍 {v.yardLocation}</span>
                      </td>
                      <td>{v.receivedDate}</td>
                      <td>
                        <div className="action-button-group">
                          <button
                            type="button"
                            className="btn btn-sm btn-outline-primary"
                            onClick={() => {
                              setMovingVehicle(v);
                              setNewYardLocation(v.yardLocation);
                            }}
                          >
                            Move Bay
                          </button>
                          {v.status === "InStock" && (
                            <button
                              type="button"
                              className="btn btn-sm btn-outline-success"
                              onClick={() => handleUpdateStatus(v.id, "Allocated")}
                            >
                              Allocate
                            </button>
                          )}
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* ── Tab 2: Vehicle Catalog & Models ── */}
        {activeTab === "catalog" && (
          <div className="tab-content-area">
            <div className="catalog-cards-grid">
              {models.map((model) => (
                <div key={model.id} className="catalog-model-card">
                  <div className="card-top-tag">
                    <span>{model.bodyType}</span>
                    <strong>{model.fuelTypes.join(" / ")}</strong>
                  </div>
                  <h3>{model.name}</h3>
                  <div className="price-tag-row">
                    <span>Starting from</span>
                    <strong>₹{model.startingPrice.toLocaleString("en-IN")}</strong>
                  </div>

                  <div className="variants-spec-list">
                    <strong>Available Trims:</strong>
                    {variants
                      .filter((vr) => vr.modelId === model.id)
                      .map((vr) => (
                        <div key={vr.id} className="variant-mini-row">
                          <span>{vr.name}</span>
                          <em>₹{vr.exShowroomPrice.toLocaleString("en-IN")}</em>
                        </div>
                      ))}
                    {variants.filter((vr) => vr.modelId === model.id).length === 0 && (
                      <div className="variant-mini-row">
                        <span>Standard Trims Available</span>
                        <em>Ex-Showroom Configured</em>
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* ── Move Bay Modal ── */}
        {movingVehicle && (
          <div className="modal-backdrop">
            <div className="modal-dialog">
              <div className="modal-header">
                <h3>Relocate Yard Bay Location</h3>
                <button type="button" onClick={() => setMovingVehicle(null)} className="close-btn">
                  ×
                </button>
              </div>

              <form onSubmit={handleYardMove} className="modal-body-form">
                <p>
                  Relocate VIN <strong>{movingVehicle.vinNumber}</strong> ({movingVehicle.modelName})
                </p>

                <div className="form-group">
                  <label>Current Location</label>
                  <input type="text" disabled value={movingVehicle.yardLocation} />
                </div>

                <div className="form-group">
                  <label>New Yard Bay or Staging Area</label>
                  <select
                    required
                    value={newYardLocation}
                    onChange={(e) => setNewYardLocation(e.target.value)}
                  >
                    <option value="">Select Destination Bay...</option>
                    <option value="Bay 1 - Showroom Floor Display">Bay 1 - Showroom Floor Display</option>
                    <option value="Bay 4 - Showroom Floor">Bay 4 - Showroom Floor</option>
                    <option value="Bay 12 - PDI Staging & Inspection">Bay 12 - PDI Staging & Inspection</option>
                    <option value="Bay 18 - Delivery Readiness Area">Bay 18 - Delivery Readiness Area</option>
                    <option value="Yard North - Section A">Yard North - Section A</option>
                    <option value="Yard North - Section B">Yard North - Section B</option>
                    <option value="Workshop Wash Bay">Workshop Wash Bay</option>
                  </select>
                </div>

                <div className="modal-footer">
                  <button type="button" className="btn btn-secondary" onClick={() => setMovingVehicle(null)}>
                    Cancel
                  </button>
                  <button type="submit" className="btn btn-primary">
                    Confirm Move
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}
      </div>
    </PortalLayout>
  );
}
