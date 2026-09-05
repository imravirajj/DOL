import { useState } from "react";
import { Link } from "react-router-dom";

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState("");
  const [error, setError] = useState("");
  const [isSubmitted, setIsSubmitted] = useState(false);
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (
    event: React.FormEvent
  ) => {
    event.preventDefault();

    const trimmedEmail = email.trim();

    setError("");
    setIsSubmitted(false);

    if (!emailPattern.test(trimmedEmail)) {
      setError("Please enter a valid email address.");
      return;
    }

    setLoading(true);

    try {
      await new Promise((resolve) => setTimeout(resolve, 600));
      setIsSubmitted(true);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-header">
          <span className="auth-eyebrow">DealerOneLane</span>
          <h1>Forgot password</h1>
          <p>
            Enter your email and we will send reset instructions if an account
            exists.
          </p>
        </div>

        {error && (
          <div className="error-message">
            {error}
          </div>
        )}

        {isSubmitted && (
          <div className="success-message">
            Reset instructions are ready to be sent once the API is connected.
          </div>
        )}

        <form onSubmit={handleSubmit} className="auth-form">
          <div className="auth-field">
            <label htmlFor="forgot-email">Email</label>

            <input
              id="forgot-email"
              type="email"
              value={email}
              onChange={(event) =>
                setEmail(event.target.value)
              }
              placeholder="Enter your email"
              autoComplete="email"
              required
            />
          </div>

          <button
            type="submit"
            disabled={loading}
          >
            {loading ? "Preparing..." : "Send reset instructions"}
          </button>
        </form>

        <p className="auth-switch">
          Remember your password?{" "}
          <Link to="/login">
            Back to login
          </Link>
        </p>
      </div>
    </div>
  );
}
