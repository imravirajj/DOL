import { useState, useEffect, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import {
  ROLES,
  type UserManagementDto,
  type UserFilter,
  type PagedResult,
} from "../types/userManagement";
import {
  fetchUsers,
  setUserStatus,
  deleteUser,
} from "../api/userManagementApi";
import { showSuccessToast, showErrorToast } from "../services/toastService";
import UserModal from "../components/UserModal";

export default function UserManagementPage() {
  const navigate = useNavigate();
  const { user: currentUser, logout } = useAuth();

  const [loading, setLoading] = useState(true);
  const [usersData, setUsersData] = useState<PagedResult<UserManagementDto>>({
    items: [],
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 1,
    hasNextPage: false,
    hasPreviousPage: false,
  });

  const [filter, setFilter] = useState<UserFilter>({
    searchTerm: "",
    role: "ALL",
    isActive: undefined,
    pageNumber: 1,
    pageSize: 10,
  });

  // Modal states
  const [modalOpen, setModalOpen] = useState(false);
  const [modalMode, setModalMode] = useState<"create" | "edit" | "view">("create");
  const [selectedUser, setSelectedUser] = useState<UserManagementDto | null>(null);

  // Delete confirmation modal
  const [userToDelete, setUserToDelete] = useState<UserManagementDto | null>(null);
  const [deleting, setDeleting] = useState(false);

  const loadUsers = useCallback(async () => {
    setLoading(true);
    try {
      const data = await fetchUsers(filter);
      setUsersData(data);
    } catch (err: any) {
      const msg =
        err?.response?.data?.description ||
        err?.response?.data?.message ||
        "Failed to load users.";
      showErrorToast(msg);
    } finally {
      setLoading(false);
    }
  }, [filter]);

  useEffect(() => {
    loadUsers();
  }, [loadUsers]);

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFilter((prev) => ({
      ...prev,
      searchTerm: e.target.value,
      pageNumber: 1,
    }));
  };

  const handleRoleFilterChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    setFilter((prev) => ({
      ...prev,
      role: e.target.value,
      pageNumber: 1,
    }));
  };

  const handleStatusFilterChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const val = e.target.value;
    setFilter((prev) => ({
      ...prev,
      isActive: val === "ALL" ? undefined : val === "ACTIVE",
      pageNumber: 1,
    }));
  };

  const handlePageChange = (newPage: number) => {
    setFilter((prev) => ({
      ...prev,
      pageNumber: newPage,
    }));
  };

  const handleOpenCreate = () => {
    setSelectedUser(null);
    setModalMode("create");
    setModalOpen(true);
  };

  const handleOpenView = (user: UserManagementDto) => {
    setSelectedUser(user);
    setModalMode("view");
    setModalOpen(true);
  };

  const handleOpenEdit = (user: UserManagementDto) => {
    setSelectedUser(user);
    setModalMode("edit");
    setModalOpen(true);
  };

  const handleToggleStatus = async (user: UserManagementDto) => {
    if (user.email.toLowerCase() === "admin@dol.com") {
      showErrorToast("The seeded SuperAdmin account cannot be deactivated.");
      return;
    }

    try {
      await setUserStatus(user.id, !user.isActive);
      showSuccessToast(
        `User ${user.isActive ? "deactivated" : "activated"} successfully.`
      );
      loadUsers();
    } catch (err: any) {
      const msg =
        err?.response?.data?.description ||
        err?.response?.data?.message ||
        "Failed to update status.";
      showErrorToast(msg);
    }
  };

  const handleDeleteConfirm = async () => {
    if (!userToDelete) return;

    if (userToDelete.email.toLowerCase() === "admin@dol.com") {
      showErrorToast("The seeded SuperAdmin account cannot be deleted.");
      setUserToDelete(null);
      return;
    }

    setDeleting(true);
    try {
      await deleteUser(userToDelete.id);
      showSuccessToast("User deleted successfully.");
      setUserToDelete(null);
      loadUsers();
    } catch (err: any) {
      const msg =
        err?.response?.data?.description ||
        err?.response?.data?.message ||
        "Failed to delete user.";
      showErrorToast(msg);
    } finally {
      setDeleting(false);
    }
  };

  const isProtectedAdmin = (user: UserManagementDto) =>
    user.email.toLowerCase() === "admin@dol.com";

  return (
    <div className="dashboard-page user-management-page">
      <aside className="dashboard-sidebar">
        <div className="dashboard-brand" onClick={() => navigate("/dashboard")} style={{ cursor: "pointer" }}>
          <span className="brand-mark">DOL</span>
          <div>
            <strong>DealerOneLane</strong>
            <span>Admin Console</span>
          </div>
        </div>

        <nav className="dashboard-nav" aria-label="Sidebar navigation">
          <button type="button" onClick={() => navigate("/dashboard")}>
            Dashboard
          </button>
          <button type="button" className="active">
            User Management
          </button>
        </nav>

        <div className="profile-panel">
          <span className="profile-avatar">
            {currentUser?.fullName?.charAt(0) || "A"}
          </span>
          <div>
            <strong>{currentUser?.fullName || "SuperAdmin"}</strong>
            <span>{currentUser?.roles?.join(", ") || "SuperAdmin"}</span>
          </div>
        </div>
      </aside>

      <main className="dashboard-main">
        <header className="dashboard-header">
          <div>
            <span className="section-eyebrow">Identity & Access Management</span>
            <h1>User Management</h1>
            <p>
              Manage multi-role team members, assignments, access statuses, and role-specific configurations.
            </p>
          </div>

          <div className="dashboard-actions">
            <button
              type="button"
              className="btn-create-user"
              onClick={handleOpenCreate}
            >
              + Create New User
            </button>
            <button
              type="button"
              className="btn-logout-secondary"
              onClick={async () => {
                await logout();
                navigate("/login");
              }}
            >
              Logout
            </button>
          </div>
        </header>

        {/* Search & Filter Toolbar */}
        <section className="filter-toolbar">
          <div className="filter-search">
            <input
              type="text"
              placeholder="Search by name, email, or mobile..."
              value={filter.searchTerm || ""}
              onChange={handleSearchChange}
            />
          </div>

          <div className="filter-selects">
            <select value={filter.role || "ALL"} onChange={handleRoleFilterChange}>
              <option value="ALL">All Roles</option>
              <option value={ROLES.SuperAdmin}>SuperAdmin</option>
              <option value={ROLES.OEMAdmin}>OEMAdmin</option>
              <option value={ROLES.DealerAdmin}>DealerAdmin</option>
              <option value={ROLES.DealerSalesExecutive}>DealerSalesExecutive</option>
              <option value={ROLES.BankOfficer}>BankOfficer</option>
              <option value={ROLES.InsuranceOfficer}>InsuranceOfficer</option>
              <option value={ROLES.RTOOfficer}>RTOOfficer</option>
              <option value={ROLES.DeliveryExecutive}>DeliveryExecutive</option>
              <option value={ROLES.Customer}>Customer</option>
            </select>

            <select
              value={
                filter.isActive === undefined
                  ? "ALL"
                  : filter.isActive
                  ? "ACTIVE"
                  : "INACTIVE"
              }
              onChange={handleStatusFilterChange}
            >
              <option value="ALL">All Statuses</option>
              <option value="ACTIVE">Active</option>
              <option value="INACTIVE">Inactive</option>
            </select>
          </div>
        </section>

        {/* User Table Grid */}
        <section className="table-wrapper">
          <table className="user-table">
            <thead>
              <tr>
                <th>User</th>
                <th>Role</th>
                <th>Contact</th>
                <th>Status</th>
                <th>Created</th>
                <th style={{ textAlign: "right" }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {loading ? (
                <tr>
                  <td colSpan={6} className="text-center py-6">
                    <div className="loading-spinner">Loading users...</div>
                  </td>
                </tr>
              ) : usersData.items.length === 0 ? (
                <tr>
                  <td colSpan={6} className="text-center py-6 empty-state">
                    No users found matching the selected criteria.
                  </td>
                </tr>
              ) : (
                usersData.items.map((u) => {
                  const isProtected = isProtectedAdmin(u);
                  return (
                    <tr key={u.id} className={!u.isActive ? "row-inactive" : ""}>
                      <td>
                        <div className="user-cell">
                          <div className="user-cell-avatar">
                            {u.fullName?.charAt(0) || "U"}
                          </div>
                          <div>
                            <strong>{u.fullName}</strong>
                            <span>{u.email}</span>
                          </div>
                        </div>
                      </td>
                      <td>
                        <span className="role-tag">
                          {u.roles?.join(", ") || "No Role"}
                        </span>
                        {isProtected && <span className="seed-badge">Protected</span>}
                      </td>
                      <td>
                        <div className="contact-cell">
                          <span>{u.phoneNumber || "—"}</span>
                          {u.city && <small>{u.city}, {u.state}</small>}
                        </div>
                      </td>
                      <td>
                        <span
                          className={`status-badge ${
                            u.isActive ? "badge-active" : "badge-inactive"
                          }`}
                        >
                          {u.isActive ? "Active" : "Inactive"}
                        </span>
                      </td>
                      <td>
                        {new Date(u.createdAt).toLocaleDateString("en-US", {
                          year: "numeric",
                          month: "short",
                          day: "numeric",
                        })}
                      </td>
                      <td style={{ textAlign: "right" }}>
                        <div className="action-buttons">
                          <button
                            type="button"
                            className="btn-action btn-view"
                            onClick={() => handleOpenView(u)}
                            title="View full profile"
                          >
                            View
                          </button>
                          <button
                            type="button"
                            className="btn-action btn-edit"
                            onClick={() => handleOpenEdit(u)}
                            title="Edit details"
                          >
                            Edit
                          </button>
                          {!isProtected && (
                            <>
                              <button
                                type="button"
                                className={`btn-action ${
                                  u.isActive ? "btn-deactivate" : "btn-activate"
                                }`}
                                onClick={() => handleToggleStatus(u)}
                                title={
                                  u.isActive
                                    ? "Deactivate user"
                                    : "Activate user"
                                }
                              >
                                {u.isActive ? "Deactivate" : "Activate"}
                              </button>
                              <button
                                type="button"
                                className="btn-action btn-delete"
                                onClick={() => setUserToDelete(u)}
                                title="Delete user"
                              >
                                Delete
                              </button>
                            </>
                          )}
                        </div>
                      </td>
                    </tr>
                  );
                })
              )}
            </tbody>
          </table>

          {/* Pagination Controls */}
          {usersData.totalPages > 1 && (
            <div className="pagination-bar">
              <span className="pagination-info">
                Showing {(usersData.pageNumber - 1) * usersData.pageSize + 1} to{" "}
                {Math.min(
                  usersData.pageNumber * usersData.pageSize,
                  usersData.totalCount
                )}{" "}
                of {usersData.totalCount} users
              </span>

              <div className="pagination-buttons">
                <button
                  type="button"
                  disabled={!usersData.hasPreviousPage}
                  onClick={() => handlePageChange(usersData.pageNumber - 1)}
                >
                  &larr; Prev
                </button>
                <span className="page-indicator">
                  Page {usersData.pageNumber} of {usersData.totalPages}
                </span>
                <button
                  type="button"
                  disabled={!usersData.hasNextPage}
                  onClick={() => handlePageChange(usersData.pageNumber + 1)}
                >
                  Next &rarr;
                </button>
              </div>
            </div>
          )}
        </section>
      </main>

      {/* User Create / Edit / View Modal */}
      <UserModal
        isOpen={modalOpen}
        mode={modalMode}
        user={selectedUser}
        onClose={() => setModalOpen(false)}
        onSuccess={loadUsers}
      />

      {/* Delete Confirmation Modal */}
      {userToDelete && (
        <div className="modal-backdrop">
          <div className="modal-container delete-dialog">
            <div className="modal-header">
              <h2>Confirm User Deletion</h2>
              <button
                type="button"
                className="btn-close"
                onClick={() => setUserToDelete(null)}
              >
                &times;
              </button>
            </div>
            <div className="delete-dialog-content">
              <p>
                Are you sure you want to delete user{" "}
                <strong>{userToDelete.fullName}</strong> ({userToDelete.email})?
              </p>
              <p className="delete-warning">
                This will soft-delete the user and deactivate their account access.
              </p>
            </div>
            <div className="modal-actions">
              <button
                type="button"
                className="btn-secondary"
                onClick={() => setUserToDelete(null)}
                disabled={deleting}
              >
                Cancel
              </button>
              <button
                type="button"
                className="btn-danger"
                onClick={handleDeleteConfirm}
                disabled={deleting}
              >
                {deleting ? "Deleting..." : "Confirm Delete"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
