export type ToastType = "error" | "success" | "warning" | "info";

export interface Toast {
  id: number;
  type: ToastType;
  message: string;
}

type ToastListener = (toast: Toast) => void;

let nextToastId = 1;
let lastToast:
  | {
      key: string;
      shownAt: number;
    }
  | undefined;

const listeners = new Set<ToastListener>();
const duplicateWindowMs = 2500;

export const subscribeToToasts = (listener: ToastListener) => {
  listeners.add(listener);

  return () => {
    listeners.delete(listener);
  };
};

export const showToast = (
  message: string,
  type: ToastType = "error"
) => {
  const normalizedMessage = message.trim();

  if (!normalizedMessage) {
    return;
  }

  const key = `${type}:${normalizedMessage}`;
  const now = Date.now();

  if (
    lastToast?.key === key &&
    now - lastToast.shownAt < duplicateWindowMs
  ) {
    return;
  }

  lastToast = {
    key,
    shownAt: now,
  };

  const toast: Toast = {
    id: nextToastId,
    type,
    message: normalizedMessage,
  };

  nextToastId += 1;

  listeners.forEach((listener) => listener(toast));
};

export const showSuccessToast = (message: string) => {
  showToast(message, "success");
};

export const showErrorToast = (message: string) => {
  showToast(message, "error");
};

export const showWarningToast = (message: string) => {
  showToast(message, "warning");
};

export const showInfoToast = (message: string) => {
  showToast(message, "info");
};
