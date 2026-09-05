import type { AxiosError } from "axios";

const statusMessages: Record<number, string> = {
  400: "Please check the submitted details and try again.",
  401: "Please check your credentials and try again.",
  403: "You do not have permission to perform this action.",
  404: "The requested resource was not found.",
  409: "This request conflicts with existing data.",
  422: "Please fix the highlighted details and try again.",
  500: "Something went wrong on the server. Please try again later.",
};

const getValidationMessage = (
  errors: any
) => {
  if (!errors) {
    return undefined;
  }

  if (Array.isArray(errors)) {
    return errors[0];
  }

  if (typeof errors === "string") {
    return errors;
  }

  const firstError = Object.values(errors)[0];

  if (Array.isArray(firstError)) {
    return firstError[0];
  }

  return firstError as string;
};

export const getApiErrorMessage = (
  error: AxiosError<any>
) => {
  if (!error.response) {
    return "Unable to connect to the server. Please check your connection.";
  }

  const payload = error.response.data;
  const message =
    payload?.error ||
    payload?.message ||
    getValidationMessage(payload?.errors) ||
    payload?.title ||
    statusMessages[error.response.status];

  return message || "Something went wrong. Please try again.";
};

export const isInvalidCredentialsError = (
  error: AxiosError<any>
) =>
  error.response?.status === 401 &&
  error.config?.url?.includes("/auth/login");
