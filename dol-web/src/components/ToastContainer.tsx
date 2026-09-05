import { useEffect, useState } from "react";
import {
  subscribeToToasts,
  type Toast,
} from "../services/toastService";

const toastDurationMs = 4500;

const toastMeta = {
  error: {
    title: "Error",
    icon: "!",
  },
  success: {
    title: "Success",
    icon: "✓",
  },
  warning: {
    title: "Warning",
    icon: "!",
  },
  info: {
    title: "Info",
    icon: "i",
  },
};

export default function ToastContainer() {
  const [toasts, setToasts] = useState<Toast[]>([]);

  const dismissToast = (toastId: number) => {
    setToasts((currentToasts) =>
      currentToasts.filter((toast) => toast.id !== toastId)
    );
  };

  useEffect(() => {
    return subscribeToToasts((toast) => {
      setToasts((currentToasts) => [
        ...currentToasts,
        toast,
      ]);

      window.setTimeout(() => {
        dismissToast(toast.id);
      }, toastDurationMs);
    });
  }, []);

  if (toasts.length === 0) {
    return null;
  }

  return (
    <div className="toast-container" role="status" aria-live="polite">
      {toasts.map((toast) => (
        <div
          key={toast.id}
          className={`toast toast-${toast.type}`}
        >
          <span className="toast-icon">
            {toastMeta[toast.type].icon}
          </span>
          <div className="toast-content">
            <strong>{toastMeta[toast.type].title}</strong>
            <span>{toast.message}</span>
          </div>
          <button
            type="button"
            className="toast-close"
            aria-label="Dismiss notification"
            onClick={() => dismissToast(toast.id)}
          >
            ×
          </button>
        </div>
      ))}
    </div>
  );
}
