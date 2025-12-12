/**
 * API Base URL
 */
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5000";

/**
 * Get authentication token from localStorage
 */
function getAuthToken(): string | null {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("token");
}

/**
 * Fetch wrapper with authentication
 */
async function apiFetch<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<T> {
  const token = getAuthToken();

  const headers: HeadersInit = {
    "Content-Type": "application/json",
    ...options.headers,
  };

  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({
      message: "An error occurred",
    }));
    throw new Error(error.message || `HTTP ${response.status}`);
  }

  return response.json();
}

/**
 * API Client
 */
export const api = {
  /**
   * GET request
   */
  get: <T>(endpoint: string): Promise<T> => {
    return apiFetch<T>(endpoint, { method: "GET" });
  },

  /**
   * POST request
   */
  post: <T>(endpoint: string, data: any): Promise<T> => {
    return apiFetch<T>(endpoint, {
      method: "POST",
      body: JSON.stringify(data),
    });
  },

  /**
   * PUT request
   */
  put: <T>(endpoint: string, data: any): Promise<T> => {
    return apiFetch<T>(endpoint, {
      method: "PUT",
      body: JSON.stringify(data),
    });
  },

  /**
   * DELETE request
   */
  delete: <T>(endpoint: string): Promise<T> => {
    return apiFetch<T>(endpoint, { method: "DELETE" });
  },

  /**
   * Upload file with FormData
   */
  upload: async <T>(endpoint: string, formData: FormData): Promise<T> => {
    const token = getAuthToken();

    const headers: HeadersInit = {};
    if (token) {
      headers["Authorization"] = `Bearer ${token}`;
    }

    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
      method: "POST",
      headers,
      body: formData,
    });

    if (!response.ok) {
      const error = await response.json().catch(() => ({
        message: "Upload failed",
      }));
      throw new Error(error.message || `HTTP ${response.status}`);
    }

    return response.json();
  },
};

/**
 * Auth helpers
 */
export const auth = {
  /**
   * Save authentication token
   */
  setToken: (token: string) => {
    localStorage.setItem("token", token);
  },

  /**
   * Remove authentication token
   */
  clearToken: () => {
    localStorage.removeItem("token");
  },

  /**
   * Check if user is authenticated
   */
  isAuthenticated: (): boolean => {
    return getAuthToken() !== null;
  },
};
