import { useState } from "react";
import { Link, useSearchParams } from "react-router-dom";

export default function ResetPasswordPage() {
  const [searchParams] = useSearchParams();

  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [loading, setLoading] = useState(false);

  const resetToken = searchParams.get("token");

  const handleSubmit = async (
    event: React.FormEvent
  ) => {
    event.preventDefault();

    setError("");
    setIsSubmitted(false);

    if (password.length < 8) {
      setError("Password must be at least 8 characters.");
      return;
    }

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    setLoading(true);

    try {
      await new Promise((resolve) => setTimeout(resolve, 600));
      setIsSubmitted(true);
      setPassword("");
      setConfirmPassword("");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-header">
          <span className="auth-eyebrow">DealerOneLane</span>
          <h1>Reset password</h1>
          <p>Choose a new password for your account.</p>
        </div>

        {!resetToken && (
          <div className="info-message">
            Reset token handling is ready for the future API integration.
          </div>
        )}

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        {isSubmitted && (
          <div className="success-message">
            Your password reset request is ready to submit once the API is
            connected.
          </div>
        )}

        <form onSubmit={handleSubmit} className="auth-form">
          <div className="auth-field">
            <label htmlFor="reset-password">New password</label>

            <input
              id="reset-password"
              type="password"
              value={password}
              onChange={(event) =>
                setPassword(event.target.value)
              }
              placeholder="Enter new password"
              autoComplete="new-password"
              required
            />
          </div>

          <div className="auth-field">
            <label htmlFor="reset-confirm-password">Confirm password</label>

            <input
              id="reset-confirm-password"
              type="password"
              value={confirmPassword}
              onChange={(event) =>
                setConfirmPassword(event.target.value)
              }
              placeholder="Confirm new password"
              autoComplete="new-password"
              required
            />
          </div>

          <button
            type="submit"
            disabled={loading}
          >
            {loading ? "Resetting..." : "Reset password"}
          </button>
        </form>

        <p className="auth-switch">
          Go back to{" "}
          <Link to="/login">
            login
          </Link>
        </p>
      </div>
    </div>
  );
}
