# Project Structure & Architecture

## 1. Overview

This project is built using **React + TypeScript**.

It follows a simple **Modular React Architecture** with **Separation of Concerns**.  
The main goal is to keep UI, authentication, API communication, routing, services, and types organized and easy to maintain.

The structure is intentionally simple so the project can be expanded in the future without adding unnecessary complexity.

---

## 2. Project Structure

```text
src/
├── api/
│   ├── apiClient.ts
│   ├── apiErrorHandler.ts
│   └── authApi.ts
│
├── assets/
│
├── components/
│
├── context/
│   └── AuthContext.tsx
│
├── pages/
│   ├── DashboardPage.tsx
│   ├── ForgotPasswordPage.tsx
│   ├── LoginPage.tsx
│   ├── RegisterPage.tsx
│   └── ResetPasswordPage.tsx
│
├── routes/
│   └── AppRoutes.tsx
│
├── services/
│   └── toastService.ts
│
├── types/
│   └── auth.ts
│
├── App.tsx
├── App.css
├── index.css
└── main.tsx
```

---

## 3. Folder Responsibilities

### `api/`

Contains backend API communication and common API-related logic.

- `apiClient.ts` – Central API client and common API configuration.
- `authApi.ts` – Authentication-related API calls such as login, register, logout, refresh token, and current user.
- `apiErrorHandler.ts` – Common API error handling and error identification.

### `components/`

Contains reusable UI components that can be used across multiple pages.

### `context/`

Contains global React Context logic.

- `AuthContext.tsx` – Manages authentication state, current user, login, registration, and logout.

### `pages/`

Contains application-level screens/pages.

Current pages include:

- Login
- Register
- Forgot Password
- Reset Password
- Dashboard

Pages mainly handle UI and user interaction while API/authentication logic is kept in the appropriate modules.

### `routes/`

Contains application routing.

- `AppRoutes.tsx` – Defines application routes and protected routes.

### `services/`

Contains reusable application services.

- `toastService.ts` – Provides a common way to display success/error notifications.

### `types/`

Contains shared TypeScript types and interfaces.

- `auth.ts` – Authentication-related request, response, and user types.

### `assets/`

Contains images and other static resources used by the application.

---

## 4. Architecture

The project uses a **Modular React Architecture with Separation of Concerns**.

Instead of keeping all logic inside page components, responsibilities are separated:

```text
Pages / Components
        ↓
Context / API
        ↓
API Client
        ↓
Interceptor / Error Handler
        ↓
Backend API
        ↓
Response / Error
        ↓
UI / Toast
```

This makes the code easier to understand, maintain, and extend.

---

## 5. Patterns Used

### Hooks Pattern

React Hooks are used for component state and lifecycle-related logic.

Examples:

- `useState`
- `useEffect`
- `useNavigate`

Example:

```tsx
const [loading, setLoading] = useState(false);
```

### Custom Hook Pattern

`useAuth()` is a custom hook used to access authentication functionality.

```tsx
const { login, logout, user } = useAuth();
```

This avoids directly accessing `AuthContext` throughout the application.

### Context / Provider Pattern

`AuthContext` and `AuthProvider` are used to manage authentication state globally.

The provider exposes:

- Current user
- Authentication status
- Loading state
- Login
- Register
- Logout

### Service Layer Pattern

Reusable application functionality is kept in services.

For example:

```text
services/toastService.ts
```

This keeps notification logic separate from UI components.

### API Client / Interceptor Pattern

API communication is centralized through `apiClient.ts`.

This provides a common place for request/response handling and interceptor-related logic such as authentication and common API errors.

### Protected Route / Route Guard Pattern

`ProtectedRoute` checks authentication before allowing access to protected pages.

```text
Authenticated → Dashboard
Not Authenticated → Login
```

### Mapper / Adapter Pattern

`toAuthResponse()` converts the backend authentication response into the application's expected `AuthResponse` format.

This helps keep API response differences isolated from the rest of the application.

---

## 6. Authentication Flow

The authentication flow is approximately:

```text
LoginPage
    ↓
useAuth()
    ↓
AuthContext
    ↓
authApi
    ↓
apiClient
    ↓
Backend API
    ↓
Auth Response
    ↓
Store Auth Tokens
    ↓
Get Current User
    ↓
Update Auth State
    ↓
Dashboard
```

The authentication state is maintained by `AuthContext`.

Authentication tokens are handled through the API client utilities.

---

## 7. Protected Routes

Protected pages are wrapped with `ProtectedRoute`.

The route checks:

1. Whether authentication is still loading.
2. Whether the user is authenticated.
3. Redirects unauthenticated users to `/login`.
4. Allows authenticated users to access the protected page.

Example flow:

```text
/dashboard
     ↓
ProtectedRoute
     ↓
Is Authenticated?
   ↙       ↘
 YES        NO
 ↓          ↓
Dashboard   /login
```

---

## 8. API Error & Toast Handling

API errors are handled through the API error-handling layer.

The project supports displaying errors in different ways depending on the situation.

For example:

- Login credential errors → shown inside the login form.
- Other API errors → shown using the global toast service.
- Successful actions → can show a success toast.

This keeps common API notification behavior consistent across the application.

---

## 9. Scalability

The current architecture is suitable for a small-to-medium React application.

As the application grows, new modules can be added without changing the basic architecture.

For example:

```text
api/
├── authApi.ts
├── userApi.ts
├── dashboardApi.ts
└── productApi.ts
```

The same approach can be followed for pages, components, services, and types.

The goal is to **add complexity only when it is actually needed**.

---

## 10. Development Principles

The project follows these basic principles:

- Keep components simple and focused on UI.
- Keep API calls outside page components.
- Reuse common services instead of duplicating logic.
- Keep authentication state in one central place.
- Keep routing separate from page implementation.
- Use TypeScript types for API and application data.
- Avoid unnecessary abstraction and complexity.
- Keep the structure easy to extend in the future.

---

## 11. Quick Summary

**Architecture:** Modular React Architecture

**Main Principle:** Separation of Concerns

**Patterns Used:**

- Hooks Pattern
- Custom Hook Pattern
- Context / Provider Pattern
- Service Layer Pattern
- API Client / Interceptor Pattern
- Protected Route / Route Guard Pattern
- Mapper / Adapter Pattern

The project is intentionally kept simple, modular, and maintainable so new features can be added easily as the application grows.
