const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
}

export interface UserInfo {
  id: number;
  username: string;
  email: string;
  fullName: string;
  role: string;
  position?: string;
  unitId: number;
  unitCode: string;
  unitName: string;
}

export interface LoginRequest {
  userId: number;
}

export interface LoginResponse {
  token: string;
  userId: number;
  username: string;
  email: string;
  fullName: string;
  role: string;
  unitId: number;
  unitName: string;
  expiresAt: string;
}

class ApiClient {
  private baseUrl: string;

  constructor() {
    this.baseUrl = API_BASE_URL;
  }

  private getHeaders(): HeadersInit {
    const headers: HeadersInit = {
      'Content-Type': 'application/json',
    };

    const token = this.getToken();
    if (token) {
      headers['Authorization'] = `Bearer ${token}`;
    }

    return headers;
  }

  private getToken(): string | null {
    if (typeof window !== 'undefined') {
      return localStorage.getItem('token');
    }
    return null;
  }

  setToken(token: string) {
    if (typeof window !== 'undefined') {
      localStorage.setItem('token', token);
    }
  }

  removeToken() {
    if (typeof window !== 'undefined') {
      localStorage.removeItem('token');
    }
  }

  async get<T>(endpoint: string): Promise<ApiResponse<T>> {
    try {
      const response = await fetch(`${this.baseUrl}${endpoint}`, {
        method: 'GET',
        headers: this.getHeaders(),
      });

      return await response.json();
    } catch (error) {
      console.error('API GET Error:', error);
      return {
        success: false,
        message: 'Lỗi kết nối đến server',
      };
    }
  }

  async post<T>(endpoint: string, data?: any): Promise<ApiResponse<T>> {
    try {
      const response = await fetch(`${this.baseUrl}${endpoint}`, {
        method: 'POST',
        headers: this.getHeaders(),
        body: JSON.stringify(data),
      });

      return await response.json();
    } catch (error) {
      console.error('API POST Error:', error);
      return {
        success: false,
        message: 'Lỗi kết nối đến server',
      };
    }
  }

  async put<T>(endpoint: string, data?: any): Promise<ApiResponse<T>> {
    try {
      const response = await fetch(`${this.baseUrl}${endpoint}`, {
        method: 'PUT',
        headers: this.getHeaders(),
        body: JSON.stringify(data),
      });

      return await response.json();
    } catch (error) {
      console.error('API PUT Error:', error);
      return {
        success: false,
        message: 'Lỗi kết nối đến server',
      };
    }
  }

  async delete<T>(endpoint: string): Promise<ApiResponse<T>> {
    try {
      const response = await fetch(`${this.baseUrl}${endpoint}`, {
        method: 'DELETE',
        headers: this.getHeaders(),
      });

      return await response.json();
    } catch (error) {
      console.error('API DELETE Error:', error);
      return {
        success: false,
        message: 'Lỗi kết nối đến server',
      };
    }
  }

  // Auth endpoints
  async getAvailableUsers(): Promise<ApiResponse<UserInfo[]>> {
    return this.get<UserInfo[]>('/auth/users');
  }

  async login(request: LoginRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await this.post<LoginResponse>('/auth/login', request);
    if (response.success && response.data?.token) {
      this.setToken(response.data.token);
    }
    return response;
  }

  async getCurrentUser(): Promise<ApiResponse<any>> {
    return this.get<any>('/auth/me');
  }

  logout() {
    this.removeToken();
  }
}

export const apiClient = new ApiClient();
