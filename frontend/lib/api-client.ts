const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5265/api';

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

  // Procedures endpoints
  async getProcedures(): Promise<ApiResponse<any[]>> {
    return this.get<any[]>('/procedures');
  }

  async getProcedureById(id: number): Promise<ApiResponse<any>> {
    return this.get<any>(`/procedures/${id}`);
  }

  async createProcedure(data: any): Promise<ApiResponse<any>> {
    return this.post<any>('/procedures', data);
  }

  async updateProcedure(id: number, data: any): Promise<ApiResponse<any>> {
    return this.put<any>(`/procedures/${id}`, data);
  }

  async deleteProcedure(id: number): Promise<ApiResponse<any>> {
    return this.delete<any>(`/procedures/${id}`);
  }

  async uploadProcedureDocument(procedureId: number, file: File, docVersion?: string): Promise<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('file', file);
    if (docVersion) {
      formData.append('docVersion', docVersion);
    }

    try {
      const response = await fetch(`${this.baseUrl}/procedures/${procedureId}/documents`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${this.getToken()}`,
        },
        body: formData,
      });

      return await response.json();
    } catch (error) {
      console.error('API Upload Error:', error);
      return {
        success: false,
        message: 'Lỗi khi upload tài liệu',
      };
    }
  }

  async deleteProcedureDocument(documentId: number): Promise<ApiResponse<any>> {
    return this.delete<any>(`/procedures/documents/${documentId}`);
  }

  // Templates endpoints
  async getTemplates(): Promise<ApiResponse<any[]>> {
    return this.get<any[]>('/templates');
  }

  async getTemplatesByProcedure(procedureId: number): Promise<ApiResponse<any[]>> {
    return this.get<any[]>(`/templates/procedure/${procedureId}`);
  }

  async getTemplateById(id: number): Promise<ApiResponse<any>> {
    return this.get<any>(`/templates/${id}`);
  }

  async createTemplate(data: any, file?: File): Promise<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('procedureId', data.procedureId.toString());
    formData.append('name', data.name);
    formData.append('templateType', data.templateType);
    if (data.templateKey) formData.append('templateKey', data.templateKey);
    if (data.templateNo) formData.append('templateNo', data.templateNo);
    if (file) formData.append('file', file);

    try {
      const response = await fetch(`${this.baseUrl}/templates`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${this.getToken()}`,
        },
        body: formData,
      });

      return await response.json();
    } catch (error) {
      console.error('API Create Template Error:', error);
      return {
        success: false,
        message: 'Lỗi khi tạo biểu mẫu',
      };
    }
  }

  async updateTemplate(id: number, data: any): Promise<ApiResponse<any>> {
    return this.put<any>(`/templates/${id}`, data);
  }

  async deleteTemplate(id: number): Promise<ApiResponse<any>> {
    return this.delete<any>(`/templates/${id}`);
  }

  async uploadTemplateFile(templateId: number, file: File): Promise<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('file', file);

    try {
      const response = await fetch(`${this.baseUrl}/templates/${templateId}/upload`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${this.getToken()}`,
        },
        body: formData,
      });

      return await response.json();
    } catch (error) {
      console.error('API Upload Template Error:', error);
      return {
        success: false,
        message: 'Lỗi khi upload file biểu mẫu',
      };
    }
  }

  // Submissions endpoints
  async getMySubmissions(): Promise<ApiResponse<any[]>> {
    return this.get<any[]>('/submissions/my');
  }

  async getSubmissionById(id: number): Promise<ApiResponse<any>> {
    return this.get<any>(`/submissions/${id}`);
  }

  async createSubmission(data: any, files: File[]): Promise<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('procedureId', data.procedureId.toString());
    formData.append('title', data.title);
    if (data.templateId) formData.append('templateId', data.templateId.toString());
    if (data.content) formData.append('content', data.content);
    
    // Add recipients if any
    if (data.recipientUserIds && data.recipientUserIds.length > 0) {
      data.recipientUserIds.forEach((id: number) => {
        formData.append('recipientUserIds', id.toString());
      });
    }
    
    files.forEach((file) => {
      formData.append('files', file);
    });

    try {
      const response = await fetch(`${this.baseUrl}/submissions`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${this.getToken()}`,
        },
        body: formData,
      });
      return await response.json();
    } catch (error) {
      console.error('API Create Submission Error:', error);
      return { success: false, message: 'Lỗi khi nộp biểu mẫu' };
    }
  }

  async recallSubmission(id: number, reason: string): Promise<ApiResponse<any>> {
    return this.post<any>(`/submissions/${id}/recall`, { reason });
  }

  async canRecallSubmission(id: number): Promise<ApiResponse<boolean>> {
    return this.get<boolean>(`/submissions/${id}/can-recall`);
  }

  // Approvals endpoints
  async getPendingApprovals(): Promise<ApiResponse<any[]>> {
    return this.get<any[]>('/approvals/pending');
  }

  async approveSubmission(id: number, note?: string): Promise<ApiResponse<any>> {
    return this.post<any>(`/approvals/${id}/approve`, { note });
  }

  async rejectSubmission(id: number, note?: string): Promise<ApiResponse<any>> {
    return this.post<any>(`/approvals/${id}/reject`, { note });
  }
}

export const apiClient = new ApiClient();
