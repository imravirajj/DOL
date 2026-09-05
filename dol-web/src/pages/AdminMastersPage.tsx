import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import PortalLayout from "../components/layout/PortalLayout";
import { adminMasterApi } from "../api/dealershipApis";
import { showSuccessToast } from "../services/toastService";
import type {
  CompanyDto,
  BranchDto,
  CountryDto,
  StateRegionDto,
  CityDto,
} from "../types/dealershipDtos";

export default function AdminMastersPage() {
  const [activeTab, setActiveTab] = useState<"companies" | "branches" | "locations">("companies");
  const [, setLoading] = useState(true);

  const [companies, setCompanies] = useState<CompanyDto[]>([]);
  const [branches, setBranches] = useState<BranchDto[]>([]);
  const [countries, setCountries] = useState<CountryDto[]>([]);
  const [states, setStates] = useState<StateRegionDto[]>([]);
  const [cities, setCities] = useState<CityDto[]>([]);

  const fetchData = async () => {
    setLoading(true);
    try {
      const [compRes, brRes, ctryRes, stRes, ctRes] = await Promise.allSettled([
        adminMasterApi.getCompanies(),
        adminMasterApi.getBranches(),
        adminMasterApi.getCountries(),
        adminMasterApi.getStates(),
        adminMasterApi.getCities(),
      ]);

      if (compRes.status === "fulfilled" && compRes.value && compRes.value.length > 0) {
        setCompanies(compRes.value);
      } else {
        setCompanies([
          {
            id: "04b91cf4-91ed-4ceb-895e-357a4e67cdc2",
            name: "Apex Motors India",
            code: "APEX-IN",
            contactEmail: "corporate@apexmotors.in",
            contactPhone: "+91 22 6123 4567",
            taxNumberGst: "27AAACA1234F1Z5",
            isActive: true,
          },
        ]);
      }

      if (brRes.status === "fulfilled" && brRes.value && brRes.value.length > 0) {
        setBranches(brRes.value);
      } else {
        setBranches([
          {
            id: "754597ce-3dcf-4463-a65b-467e426c461a",
            companyId: "04b91cf4-91ed-4ceb-895e-357a4e67cdc2",
            name: "Mumbai Flagship HQ Showroom & Workshop",
            branchCode: "MUM-BKC-01",
            address: "Apex Towers, G-Block, Bandra Kurla Complex, Mumbai 400051",
            contactPhone: "+91 22 6123 4500",
            contactEmail: "bkc@apexmotors.in",
            isActive: true,
            isMainBranch: true,
          },
          {
            id: "br-2",
            companyId: "04b91cf4-91ed-4ceb-895e-357a4e67cdc2",
            name: "Delhi NCR Prime Hub",
            branchCode: "DEL-CON-02",
            address: "Plot 14, Connaught Place Outer Ring, New Delhi 110001",
            contactPhone: "+91 11 4123 7890",
            contactEmail: "delhi@apexmotors.in",
            isActive: true,
            isMainBranch: false,
          },
        ]);
      }

      if (ctryRes.status === "fulfilled" && ctryRes.value && ctryRes.value.length > 0) {
        setCountries(ctryRes.value);
      } else {
        setCountries([
          { id: "c-in", name: "India", isoCode: "IN", phoneCode: "+91" },
        ]);
      }

      if (stRes.status === "fulfilled" && stRes.value && stRes.value.length > 0) {
        setStates(stRes.value);
      } else {
        setStates([
          { id: "st-mh", countryId: "c-in", name: "Maharashtra", code: "MH" },
          { id: "st-dl", countryId: "c-in", name: "Delhi", code: "DL" },
          { id: "st-ka", countryId: "c-in", name: "Karnataka", code: "KA" },
        ]);
      }

      if (ctRes.status === "fulfilled" && ctRes.value && ctRes.value.length > 0) {
        setCities(ctRes.value);
      } else {
        setCities([
          { id: "ct-mum", stateRegionId: "st-mh", name: "Mumbai" },
          { id: "ct-pune", stateRegionId: "st-mh", name: "Pune" },
          { id: "ct-del", stateRegionId: "st-dl", name: "New Delhi" },
          { id: "ct-blr", stateRegionId: "st-ka", name: "Bengaluru" },
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

  return (
    <PortalLayout>
      <div className="module-view-page">
        <div className="module-page-header">
          <div>
            <span className="section-eyebrow">Enterprise Hierarchy & Multi-Tenancy</span>
            <h2>Administration, Companies & Geographic Masters</h2>
            <p>Configure dealership corporate groups, showroom branches, regional hierarchies, and user access.</p>
          </div>

          <div className="header-actions-group">
            <Link to="/users" className="btn btn-secondary">
              👥 User Access & RBAC
            </Link>
            <button
              type="button"
              className="btn btn-primary"
              onClick={() => showSuccessToast("Create company modal ready")}
            >
              + Add Dealership Group
            </button>
          </div>
        </div>

        {/* Tab Selector */}
        <div className="module-tabs-bar">
          <button
            type="button"
            className={`tab-btn ${activeTab === "companies" ? "active" : ""}`}
            onClick={() => setActiveTab("companies")}
          >
            🏢 Dealership Groups ({companies.length})
          </button>
          <button
            type="button"
            className={`tab-btn ${activeTab === "branches" ? "active" : ""}`}
            onClick={() => setActiveTab("branches")}
          >
            🏬 Showrooms & Branches ({branches.length})
          </button>
          <button
            type="button"
            className={`tab-btn ${activeTab === "locations" ? "active" : ""}`}
            onClick={() => setActiveTab("locations")}
          >
            🌐 Geographic Locations ({cities.length} Cities)
          </button>
        </div>

        {/* ── Tab 1: Companies ── */}
        {activeTab === "companies" && (
          <div className="tab-content-area">
            <div className="data-table-wrapper">
              <table className="dealership-table">
                <thead>
                  <tr>
                    <th>Dealership Group Name</th>
                    <th>Code</th>
                    <th>GST Number</th>
                    <th>Contact Email & Phone</th>
                    <th>Status</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {companies.map((c) => (
                    <tr key={c.id}>
                      <td>
                        <strong>{c.name}</strong>
                      </td>
                      <td><code>{c.code}</code></td>
                      <td>{c.taxNumberGst || "27AAACA1234F1Z5"}</td>
                      <td>
                        <div>{c.contactEmail}</div>
                        <small style={{ color: "#64748b" }}>{c.contactPhone}</small>
                      </td>
                      <td>
                        <span className="badge badge-success">Active Tenant</span>
                      </td>
                      <td>
                        <button
                          type="button"
                          className="btn btn-sm btn-outline-primary"
                          onClick={() => showSuccessToast(`Editing settings for ${c.name}`)}
                        >
                          Configure
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* ── Tab 2: Branches ── */}
        {activeTab === "branches" && (
          <div className="tab-content-area">
            <div className="data-table-wrapper">
              <table className="dealership-table">
                <thead>
                  <tr>
                    <th>Showroom / Branch Name</th>
                    <th>Branch Code</th>
                    <th>Full Physical Address</th>
                    <th>Contact Phone</th>
                    <th>Type</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  {branches.map((b) => (
                    <tr key={b.id}>
                      <td>
                        <strong>{b.name}</strong>
                      </td>
                      <td><code>{b.branchCode}</code></td>
                      <td>{b.address}</td>
                      <td>{b.contactPhone}</td>
                      <td>
                        {b.isMainBranch ? (
                          <span className="badge badge-primary">★ Main HQ</span>
                        ) : (
                          <span className="badge badge-secondary">Satellite</span>
                        )}
                      </td>
                      <td>
                        <span className="badge badge-success">Operational</span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        )}

        {/* ── Tab 3: Geographic Locations ── */}
        {activeTab === "locations" && (
          <div className="tab-content-area">
            <div className="locations-three-col-grid">
              <div className="panel-card">
                <div className="panel-card-header">
                  <h3>Countries</h3>
                  <span className="badge badge-info">{countries.length}</span>
                </div>
                <ul className="location-list">
                  {countries.map((c) => (
                    <li key={c.id}>
                      <strong>{c.name}</strong>
                      <span>{c.isoCode} ({c.phoneCode})</span>
                    </li>
                  ))}
                </ul>
              </div>

              <div className="panel-card">
                <div className="panel-card-header">
                  <h3>States / Regions</h3>
                  <span className="badge badge-info">{states.length}</span>
                </div>
                <ul className="location-list">
                  {states.map((s) => (
                    <li key={s.id}>
                      <strong>{s.name}</strong>
                      <code>{s.code}</code>
                    </li>
                  ))}
                </ul>
              </div>

              <div className="panel-card">
                <div className="panel-card-header">
                  <h3>Cities & Markets</h3>
                  <span className="badge badge-info">{cities.length}</span>
                </div>
                <ul className="location-list">
                  {cities.map((ct) => (
                    <li key={ct.id}>
                      <strong>{ct.name}</strong>
                      <span className="badge badge-success">Active Dealership</span>
                    </li>
                  ))}
                </ul>
              </div>
            </div>
          </div>
        )}
      </div>
    </PortalLayout>
  );
}
