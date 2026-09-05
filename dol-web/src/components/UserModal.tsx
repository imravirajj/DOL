import React, { useState, useEffect } from "react";
import {
  ROLES,
  type UserManagementDto,
  type CreateUserPayload,
  type UpdateUserPayload,
  type OEMOption,
  type DealerOption,
} from "../types/userManagement";
import {
  createUser,
  updateUser,
  fetchOEMs,
  fetchDealers,
  fetchDealerAdmins,
} from "../api/userManagementApi";
import { showSuccessToast, showErrorToast } from "../services/toastService";

interface UserModalProps {
  isOpen: boolean;
  mode: "create" | "edit" | "view";
  user: UserManagementDto | null;
  onClose: () => void;
  onSuccess: () => void;
}

export default function UserModal({
  isOpen,
  mode,
  user,
  onClose,
  onSuccess,
}: UserModalProps) {
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [password, setPassword] = useState("");
  const [roleName, setRoleName] = useState<string>(ROLES.OEMAdmin);
  const [isActive, setIsActive] = useState(true);

  // Address
  const [address, setAddress] = useState("");
  const [city, setCity] = useState("");
  const [stateName, setStateName] = useState("");
  const [pincode, setPincode] = useState("");

  // Role-Specific Fields
  const [oemId, setOemId] = useState("");
  const [dealerId, setDealerId] = useState("");
  const [reportingAdminId, setReportingAdminId] = useState("");
  const [bankName, setBankName] = useState("");
  const [branchName, setBranchName] = useState("");
  const [employeeId, setEmployeeId] = useState("");
  const [insuranceCompanyName, setInsuranceCompanyName] = useState("");
  const [rtoOffice, setRtoOffice] = useState("");
  const [region, setRegion] = useState("");
  const [officerId, setOfficerId] = useState("");
  const [organizationName, setOrganizationName] = useState("");

  // Dropdown options
  const [oemList, setOemList] = useState<OEMOption[]>([]);
  const [dealerList, setDealerList] = useState<DealerOption[]>([]);
  const [dealerAdminList, setDealerAdminList] = useState<UserManagementDto[]>([]);
  const [submitting, setSubmitting] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const isSeededSuperAdmin = user?.email?.toLowerCase() === "admin@dol.com";

  useEffect(() => {
    if (!isOpen) return;

    // Load reference data
    fetchOEMs().then(setOemList);
    fetchDealers().then(setDealerList);
    fetchDealerAdmins().then(setDealerAdminList);

    if (user && (mode === "edit" || mode === "view")) {
      setFullName(user.fullName || "");
      setEmail(user.email || "");
      setPhoneNumber(user.phoneNumber || "");
      setPassword("");
      setRoleName(user.roles?.[0] || ROLES.Customer);
      setIsActive(user.isActive);

      setAddress(user.address || "");
      setCity(user.city || "");
      setStateName(user.state || "");
      setPincode(user.pincode || "");

      setOemId(user.oemId || "");
      setDealerId(user.dealerId || "");
      setReportingAdminId(user.reportingAdminId || "");
      setBankName(user.bankName || "");
      setBranchName(user.branchName || "");
      setEmployeeId(user.employeeId || "");
      setInsuranceCompanyName(user.insuranceCompanyName || "");
      setRtoOffice(user.rtoOffice || "");
      setRegion(user.region || "");
      setOfficerId(user.officerId || "");
      setOrganizationName(user.organizationName || "");
    } else {
      // Reset form
      setFullName("");
      setEmail("");
      setPhoneNumber("");
      setPassword("");
      setRoleName(ROLES.OEMAdmin);
      setIsActive(true);

      setAddress("");
      setCity("");
      setStateName("");
      setPincode("");

      setOemId("");
      setDealerId("");
      setReportingAdminId("");
      setBankName("");
      setBranchName("");
      setEmployeeId("");
      setInsuranceCompanyName("");
      setRtoOffice("");
      setRegion("");
      setOfficerId("");
      setOrganizationName("");
    }
    setErrors({});
  }, [isOpen, user, mode]);

  if (!isOpen) return null;

  const validateForm = (): boolean => {
    const errs: Record<string, string> = {};

    if (!fullName.trim()) errs.fullName = "Full name is required.";
    if (mode === "create") {
      if (!email.trim()) errs.email = "Email is required.";
      if (!password || password.length < 6)
        errs.password = "Password must be at least 6 characters.";
    }
    if (mode === "edit" && password && password.length < 6) {
      errs.password = "Password must be at least 6 characters.";
    }

    if (!phoneNumber.trim()) errs.phoneNumber = "Phone number is required.";

    // Role-specific validation
    if (!isSeededSuperAdmin) {
      switch (roleName) {
        case ROLES.OEMAdmin:
          if (!oemId && !organizationName.trim()) {
            errs.oemId = "Select an OEM or enter Company Name.";
          }
          break;
        case ROLES.DealerAdmin:
          if (!dealerId) {
            errs.dealerId = "Dealer selection is required.";
          }
          break;
        case ROLES.DealerSalesExecutive:
          if (!dealerId) {
            errs.dealerId = "Dealer selection is required.";
          }
          if (!reportingAdminId) {
            errs.reportingAdminId = "Reporting DealerAdmin is required.";
          }
          break;
        case ROLES.BankOfficer:
          if (!bankName.trim()) errs.bankName = "Bank name is required.";
          if (!branchName.trim()) errs.branchName = "Branch name is required.";
          if (!employeeId.trim()) errs.employeeId = "Employee ID is required.";
          break;
        case ROLES.InsuranceOfficer:
          if (!insuranceCompanyName.trim())
            errs.insuranceCompanyName = "Insurance company name is required.";
          if (!branchName.trim()) errs.branchName = "Branch name is required.";
          if (!employeeId.trim()) errs.employeeId = "Employee ID is required.";
          break;
        case ROLES.RTOOfficer:
          if (!rtoOffice.trim()) errs.rtoOffice = "RTO office is required.";
          if (!region.trim()) errs.region = "Region is required.";
          if (!officerId.trim()) errs.officerId = "Officer ID is required.";
          break;
        case ROLES.DeliveryExecutive:
          if (!employeeId.trim()) errs.employeeId = "Employee ID is required.";
          if (!dealerId && !organizationName.trim()) {
            errs.dealerId = "Dealer or Organization is required.";
          }
          break;
      }
    }

    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (mode === "view") {
      onClose();
      return;
    }

    if (!validateForm()) return;

    setSubmitting(true);
    try {
      if (mode === "create") {
        const payload: CreateUserPayload = {
          fullName: fullName.trim(),
          email: email.trim(),
          phoneNumber: phoneNumber.trim(),
          password,
          roleName,
          isActive,
          address: address.trim() || undefined,
          city: city.trim() || undefined,
          state: stateName.trim() || undefined,
          pincode: pincode.trim() || undefined,
          oemId: oemId || undefined,
          dealerId: dealerId || undefined,
          reportingAdminId: reportingAdminId || undefined,
          bankName: bankName.trim() || undefined,
          branchName: branchName.trim() || undefined,
          employeeId: employeeId.trim() || undefined,
          insuranceCompanyName: insuranceCompanyName.trim() || undefined,
          rtoOffice: rtoOffice.trim() || undefined,
          region: region.trim() || undefined,
          officerId: officerId.trim() || undefined,
          organizationName: organizationName.trim() || undefined,
        };

        await createUser(payload);
        showSuccessToast("User created successfully!");
        onSuccess();
        onClose();
      } else if (mode === "edit" && user) {
        const payload: UpdateUserPayload = {
          fullName: fullName.trim(),
          phoneNumber: phoneNumber.trim(),
          password: password || undefined,
          roleName: isSeededSuperAdmin ? ROLES.SuperAdmin : roleName,
          isActive: isSeededSuperAdmin ? true : isActive,
          address: address.trim() || undefined,
          city: city.trim() || undefined,
          state: stateName.trim() || undefined,
          pincode: pincode.trim() || undefined,
          oemId: oemId || undefined,
          dealerId: dealerId || undefined,
          reportingAdminId: reportingAdminId || undefined,
          bankName: bankName.trim() || undefined,
          branchName: branchName.trim() || undefined,
          employeeId: employeeId.trim() || undefined,
          insuranceCompanyName: insuranceCompanyName.trim() || undefined,
          rtoOffice: rtoOffice.trim() || undefined,
          region: region.trim() || undefined,
          officerId: officerId.trim() || undefined,
          organizationName: organizationName.trim() || undefined,
        };

        await updateUser(user.id, payload);
        showSuccessToast("User updated successfully!");
        onSuccess();
        onClose();
      }
    } catch (err: any) {
      const msg = err?.response?.data?.description || err?.response?.data?.message || err?.message || "Operation failed.";
      showErrorToast(msg);
    } finally {
      setSubmitting(false);
    }
  };

  const filteredDealers = oemId
    ? dealerList.filter((d) => d.oemId === oemId)
    : dealerList;

  return (
    <div className="modal-backdrop">
      <div className="modal-container">
        <div className="modal-header">
          <div>
            <h2>
              {mode === "create" && "Create New User"}
              {mode === "edit" && "Edit User Details"}
              {mode === "view" && "User Profile Details"}
            </h2>
            <p>
              {mode === "create" && "Configure account access and role-specific parameters."}
              {mode === "edit" && "Modify account details, roles, or reset password."}
              {mode === "view" && "Detailed view of user role and assignment profile."}
            </p>
          </div>
          <button type="button" className="btn-close" onClick={onClose}>
            &times;
          </button>
        </div>

        <form onSubmit={handleSubmit} className="modal-form">
          <div className="form-section">
            <h3>1. Core User Information</h3>
            <div className="form-grid">
              <div className="form-group">
                <label>Full Name *</label>
                <input
                  type="text"
                  value={fullName}
                  onChange={(e) => setFullName(e.target.value)}
                  placeholder="e.g. John Doe"
                  disabled={mode === "view"}
                  required
                />
                {errors.fullName && <span className="field-error">{errors.fullName}</span>}
              </div>

              <div className="form-group">
                <label>Email Address *</label>
                <input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  placeholder="e.g. user@dealer.com"
                  disabled={mode !== "create"}
                  required
                />
                {errors.email && <span className="field-error">{errors.email}</span>}
              </div>

              <div className="form-group">
                <label>Phone Number *</label>
                <input
                  type="tel"
                  value={phoneNumber}
                  onChange={(e) => setPhoneNumber(e.target.value)}
                  placeholder="e.g. +91 9876543210"
                  disabled={mode === "view"}
                  required
                />
                {errors.phoneNumber && <span className="field-error">{errors.phoneNumber}</span>}
              </div>

              <div className="form-group">
                <label>
                  {mode === "create" ? "Password *" : "New Password (optional)"}
                </label>
                <input
                  type="password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  placeholder={mode === "create" ? "Min 6 characters" : "Leave blank to keep current"}
                  disabled={mode === "view"}
                  required={mode === "create"}
                />
                {errors.password && <span className="field-error">{errors.password}</span>}
              </div>

              <div className="form-group">
                <label>Role Assignment *</label>
                <select
                  value={roleName}
                  onChange={(e) => setRoleName(e.target.value)}
                  disabled={mode === "view" || isSeededSuperAdmin}
                >
                  {isSeededSuperAdmin ? (
                    <option value={ROLES.SuperAdmin}>{ROLES.SuperAdmin}</option>
                  ) : (
                    <>
                      <option value={ROLES.OEMAdmin}>OEM Admin</option>
                      <option value={ROLES.DealerAdmin}>Dealer Admin</option>
                      <option value={ROLES.DealerSalesExecutive}>Dealer Sales Executive</option>
                      <option value={ROLES.BankOfficer}>Bank Officer</option>
                      <option value={ROLES.InsuranceOfficer}>Insurance Officer</option>
                      <option value={ROLES.RTOOfficer}>RTO Officer</option>
                      <option value={ROLES.DeliveryExecutive}>Delivery Executive</option>
                      <option value={ROLES.Customer}>Customer</option>
                    </>
                  )}
                </select>
              </div>

              <div className="form-group checkbox-group">
                <label className="checkbox-label">
                  <input
                    type="checkbox"
                    checked={isActive}
                    onChange={(e) => setIsActive(e.target.checked)}
                    disabled={mode === "view" || isSeededSuperAdmin}
                  />
                  <span>Active Account Status</span>
                </label>
                {isSeededSuperAdmin && (
                  <small className="field-hint">Seeded SuperAdmin is permanently active.</small>
                )}
              </div>
            </div>
          </div>

          <div className="form-section">
            <h3>2. Address Information</h3>
            <div className="form-grid">
              <div className="form-group full-width">
                <label>Street Address</label>
                <input
                  type="text"
                  value={address}
                  onChange={(e) => setAddress(e.target.value)}
                  placeholder="Street / Office Address"
                  disabled={mode === "view"}
                />
              </div>

              <div className="form-group">
                <label>City</label>
                <input
                  type="text"
                  value={city}
                  onChange={(e) => setCity(e.target.value)}
                  placeholder="City"
                  disabled={mode === "view"}
                />
              </div>

              <div className="form-group">
                <label>State</label>
                <input
                  type="text"
                  value={stateName}
                  onChange={(e) => setStateName(e.target.value)}
                  placeholder="State"
                  disabled={mode === "view"}
                />
              </div>

              <div className="form-group">
                <label>Pincode</label>
                <input
                  type="text"
                  value={pincode}
                  onChange={(e) => setPincode(e.target.value)}
                  placeholder="Pincode"
                  disabled={mode === "view"}
                />
              </div>
            </div>
          </div>

          {/* 3. Dynamic Role-Specific Fields */}
          {!isSeededSuperAdmin && roleName !== ROLES.Customer && (
            <div className="form-section role-specific-section">
              <h3>3. Role-Specific Configuration ({roleName})</h3>

              {roleName === ROLES.OEMAdmin && (
                <div className="form-grid">
                  <div className="form-group">
                    <label>Select OEM *</label>
                    <select
                      value={oemId}
                      onChange={(e) => setOemId(e.target.value)}
                      disabled={mode === "view"}
                    >
                      <option value="">-- Choose OEM --</option>
                      {oemList.map((o) => (
                        <option key={o.id} value={o.id}>
                          {o.name} ({o.code})
                        </option>
                      ))}
                    </select>
                    {errors.oemId && <span className="field-error">{errors.oemId}</span>}
                  </div>

                  <div className="form-group">
                    <label>Company / Organization Name</label>
                    <input
                      type="text"
                      value={organizationName}
                      onChange={(e) => setOrganizationName(e.target.value)}
                      placeholder="Company Name"
                      disabled={mode === "view"}
                    />
                  </div>
                </div>
              )}

              {roleName === ROLES.DealerAdmin && (
                <div className="form-grid">
                  <div className="form-group">
                    <label>Filter by OEM (Optional)</label>
                    <select
                      value={oemId}
                      onChange={(e) => {
                        setOemId(e.target.value);
                        setDealerId("");
                      }}
                      disabled={mode === "view"}
                    >
                      <option value="">-- All OEMs --</option>
                      {oemList.map((o) => (
                        <option key={o.id} value={o.id}>
                          {o.name}
                        </option>
                      ))}
                    </select>
                  </div>

                  <div className="form-group">
                    <label>Assigned Dealership *</label>
                    <select
                      value={dealerId}
                      onChange={(e) => setDealerId(e.target.value)}
                      disabled={mode === "view"}
                      required
                    >
                      <option value="">-- Select Dealership --</option>
                      {filteredDealers.map((d) => (
                        <option key={d.id} value={d.id}>
                          {d.name} ({d.dealerCode}) {d.city ? `- ${d.city}` : ""}
                        </option>
                      ))}
                    </select>
                    {errors.dealerId && <span className="field-error">{errors.dealerId}</span>}
                  </div>
                </div>
              )}

              {roleName === ROLES.DealerSalesExecutive && (
                <div className="form-grid">
                  <div className="form-group">
                    <label>Assigned Dealership *</label>
                    <select
                      value={dealerId}
                      onChange={(e) => setDealerId(e.target.value)}
                      disabled={mode === "view"}
                      required
                    >
                      <option value="">-- Select Dealership --</option>
                      {dealerList.map((d) => (
                        <option key={d.id} value={d.id}>
                          {d.name} ({d.dealerCode})
                        </option>
                      ))}
                    </select>
                    {errors.dealerId && <span className="field-error">{errors.dealerId}</span>}
                  </div>

                  <div className="form-group">
                    <label>Reporting Dealer Admin *</label>
                    <select
                      value={reportingAdminId}
                      onChange={(e) => setReportingAdminId(e.target.value)}
                      disabled={mode === "view"}
                      required
                    >
                      <option value="">-- Select Reporting DealerAdmin --</option>
                      {dealerAdminList.map((admin) => (
                        <option key={admin.id} value={admin.id}>
                          {admin.fullName} ({admin.email})
                        </option>
                      ))}
                    </select>
                    {errors.reportingAdminId && (
                      <span className="field-error">{errors.reportingAdminId}</span>
                    )}
                  </div>
                </div>
              )}

              {roleName === ROLES.BankOfficer && (
                <div className="form-grid">
                  <div className="form-group">
                    <label>Bank Name *</label>
                    <input
                      type="text"
                      value={bankName}
                      onChange={(e) => setBankName(e.target.value)}
                      placeholder="e.g. HDFC Bank, SBI"
                      disabled={mode === "view"}
                      required
                    />
                    {errors.bankName && <span className="field-error">{errors.bankName}</span>}
                  </div>

                  <div className="form-group">
                    <label>Branch Name *</label>
                    <input
                      type="text"
                      value={branchName}
                      onChange={(e) => setBranchName(e.target.value)}
                      placeholder="e.g. Connaught Place Branch"
                      disabled={mode === "view"}
                      required
                    />
                    {errors.branchName && <span className="field-error">{errors.branchName}</span>}
                  </div>

                  <div className="form-group">
                    <label>Employee ID *</label>
                    <input
                      type="text"
                      value={employeeId}
                      onChange={(e) => setEmployeeId(e.target.value)}
                      placeholder="e.g. BNK-1082"
                      disabled={mode === "view"}
                      required
                    />
                    {errors.employeeId && <span className="field-error">{errors.employeeId}</span>}
                  </div>
                </div>
              )}

              {roleName === ROLES.InsuranceOfficer && (
                <div className="form-grid">
                  <div className="form-group">
                    <label>Insurance Company Name *</label>
                    <input
                      type="text"
                      value={insuranceCompanyName}
                      onChange={(e) => setInsuranceCompanyName(e.target.value)}
                      placeholder="e.g. ICICI Lombard, Bajaj Allianz"
                      disabled={mode === "view"}
                      required
                    />
                    {errors.insuranceCompanyName && (
                      <span className="field-error">{errors.insuranceCompanyName}</span>
                    )}
                  </div>

                  <div className="form-group">
                    <label>Branch Name *</label>
                    <input
                      type="text"
                      value={branchName}
                      onChange={(e) => setBranchName(e.target.value)}
                      placeholder="e.g. Metro Regional Office"
                      disabled={mode === "view"}
                      required
                    />
                    {errors.branchName && <span className="field-error">{errors.branchName}</span>}
                  </div>

                  <div className="form-group">
                    <label>Employee ID *</label>
                    <input
                      type="text"
                      value={employeeId}
                      onChange={(e) => setEmployeeId(e.target.value)}
                      placeholder="e.g. INS-4401"
                      disabled={mode === "view"}
                      required
                    />
                    {errors.employeeId && <span className="field-error">{errors.employeeId}</span>}
                  </div>
                </div>
              )}

              {roleName === ROLES.RTOOfficer && (
                <div className="form-grid">
                  <div className="form-group">
                    <label>RTO Office Name *</label>
                    <input
                      type="text"
                      value={rtoOffice}
                      onChange={(e) => setRtoOffice(e.target.value)}
                      placeholder="e.g. MH-02 Mumbai West"
                      disabled={mode === "view"}
                      required
                    />
                    {errors.rtoOffice && <span className="field-error">{errors.rtoOffice}</span>}
                  </div>

                  <div className="form-group">
                    <label>Jurisdiction Region *</label>
                    <input
                      type="text"
                      value={region}
                      onChange={(e) => setRegion(e.target.value)}
                      placeholder="e.g. Western Zone"
                      disabled={mode === "view"}
                      required
                    />
                    {errors.region && <span className="field-error">{errors.region}</span>}
                  </div>

                  <div className="form-group">
                    <label>Officer ID / Badge *</label>
                    <input
                      type="text"
                      value={officerId}
                      onChange={(e) => setOfficerId(e.target.value)}
                      placeholder="e.g. RTO-8890"
                      disabled={mode === "view"}
                      required
                    />
                    {errors.officerId && <span className="field-error">{errors.officerId}</span>}
                  </div>
                </div>
              )}

              {roleName === ROLES.DeliveryExecutive && (
                <div className="form-grid">
                  <div className="form-group">
                    <label>Assigned Dealership</label>
                    <select
                      value={dealerId}
                      onChange={(e) => setDealerId(e.target.value)}
                      disabled={mode === "view"}
                    >
                      <option value="">-- Select Dealership (or enter below) --</option>
                      {dealerList.map((d) => (
                        <option key={d.id} value={d.id}>
                          {d.name}
                        </option>
                      ))}
                    </select>
                    {errors.dealerId && <span className="field-error">{errors.dealerId}</span>}
                  </div>

                  <div className="form-group">
                    <label>Organization / Logistics Partner</label>
                    <input
                      type="text"
                      value={organizationName}
                      onChange={(e) => setOrganizationName(e.target.value)}
                      placeholder="e.g. SwiftLogistics"
                      disabled={mode === "view"}
                    />
                  </div>

                  <div className="form-group">
                    <label>Employee ID *</label>
                    <input
                      type="text"
                      value={employeeId}
                      onChange={(e) => setEmployeeId(e.target.value)}
                      placeholder="e.g. DLV-902"
                      disabled={mode === "view"}
                      required
                    />
                    {errors.employeeId && <span className="field-error">{errors.employeeId}</span>}
                  </div>
                </div>
              )}
            </div>
          )}

          <div className="modal-actions">
            <button type="button" className="btn-secondary" onClick={onClose} disabled={submitting}>
              {mode === "view" ? "Close" : "Cancel"}
            </button>
            {mode !== "view" && (
              <button type="submit" className="btn-primary" disabled={submitting}>
                {submitting ? "Saving..." : mode === "create" ? "Create User" : "Save Changes"}
              </button>
            )}
          </div>
        </form>
      </div>
    </div>
  );
}
