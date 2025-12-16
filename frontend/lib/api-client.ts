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

export interface LoginCredentialsRequest {
  emailOrUsername: string;
  password: string;
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

export interface Procedure {
  id: number;
  code: string;
  name: string;
  version?: string;
  state: string;
  description?: string;
  ownerUserId?: number;
  ownerUserName?: string;
  documentCount?: number;
  templateCount?: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface ProcedureFormData {
  code: string;
  name: string;
  version?: string;
  description?: string;
}

export interface Template {
  id: number;
  procedureId: number;
  name: string;
  templateType: string;
  templateKey?: string;
  templateNo?: string;
  filePath?: string;
  createdAt?: string;
}

export interface Submission {
  id: number;
  procedureId: number;
  templateId?: number;
  title: string;
  content?: string;
  state: string;
  submittedByUserId: number;
  submittedByUserName?: string;
  submittedAt: string;
  createdAt?: string;
}

export interface Approval {
  id: number;
  submissionId: number;
  submissionTitle?: string;
  approverUserId: number;
  approverUserName?: string;
  status: string;
  note?: string;
  createdAt?: string;
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

      // Check if response is OK and has content
      if (!response.ok) {
        if (response.status === 401) {
          return {
            success: false,
            message: 'Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.',
          };
        }
        return {
          success: false,
          message: `Lỗi ${response.status}: ${response.statusText}`,
        };
      }

      // Check if response has content before parsing JSON
      const text = await response.text();
      if (!text) {
        return {
          success: false,
          message: 'Server trả về response trống',
        };
      }

      try {
        return JSON.parse(text);
      } catch {
        return {
          success: false,
          message: 'Server trả về dữ liệu không hợp lệ',
        };
      }
    } catch (error) {
      console.error('API GET Error:', error);
      return {
        success: false,
        message: 'Lỗi kết nối đến server',
      };
    }
  }

  async post<T>(endpoint: string, data?: unknown): Promise<ApiResponse<T>> {
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

  async put<T>(endpoint: string, data?: unknown): Promise<ApiResponse<T>> {
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

  async loginWithCredentials(request: LoginCredentialsRequest): Promise<ApiResponse<LoginResponse>> {
    const response = await this.post<LoginResponse>('/auth/login-credentials', request);
    if (response.success && response.data?.token) {
      this.setToken(response.data.token);
    }
    return response;
  }

  async getCurrentUser(): Promise<ApiResponse<UserInfo>> {
    return this.get<UserInfo>('/auth/me');
  }

  logout() {
    this.removeToken();
  }

  // Procedures endpoints
  async getProcedures(): Promise<ApiResponse<Procedure[]>> {
    return this.get<Procedure[]>('/procedures');
  }

  async getProcedureById(id: number): Promise<ApiResponse<Procedure>> {
    return this.get<Procedure>(`/procedures/${id}`);
  }

  async createProcedure(data: ProcedureFormData): Promise<ApiResponse<Procedure>> {
    return this.post<Procedure>('/procedures', data);
  }

  async updateProcedure(id: number, data: ProcedureFormData): Promise<ApiResponse<Procedure>> {
    return this.put<Procedure>(`/procedures/${id}`, data);
  }

  async deleteProcedure(id: number): Promise<ApiResponse<void>> {
    return this.delete<void>(`/procedures/${id}`);
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

  // Dashboard endpoints
  async getDashboardStats(): Promise<ApiResponse<DashboardStats>> {
    return this.get<DashboardStats>('/dashboard/stats');
  }

  async getRecentActivities(count: number = 10): Promise<ApiResponse<RecentActivity[]>> {
    return this.get<RecentActivity[]>(`/dashboard/recent-activities?count=${count}`);
  }

  // Audit Log endpoints
  async getAuditLogs(filter: AuditLogFilter): Promise<ApiResponse<AuditLogPagedResult>> {
    const params = new URLSearchParams();
    if (filter.userId) params.append('userId', filter.userId.toString());
    if (filter.action) params.append('action', filter.action);
    if (filter.targetType) params.append('targetType', filter.targetType);
    if (filter.fromDate) params.append('fromDate', filter.fromDate);
    if (filter.toDate) params.append('toDate', filter.toDate);
    params.append('page', (filter.page || 1).toString());
    params.append('pageSize', (filter.pageSize || 50).toString());
    
    return this.get<AuditLogPagedResult>(`/auditlogs?${params.toString()}`);
  }

  async getAuditActionTypes(): Promise<ApiResponse<string[]>> {
    return this.get<string[]>('/auditlogs/action-types');
  }

  async getAuditTargetTypes(): Promise<ApiResponse<string[]>> {
    return this.get<string[]>('/auditlogs/target-types');
  }

  // Units endpoints
  async getUnits(): Promise<ApiResponse<Unit[]>> {
    return this.get<Unit[]>('/auth/units');
  }
}

// Additional types
export interface DashboardStats {
  totalProcedures: number;
  totalTemplates: number;
  totalSubmissions: number;
  pendingApprovals: number;
  approvedSubmissions: number;
  rejectedSubmissions: number;
  totalUsers: number;
  totalUnits: number;
  draftProcedures: number;
  submittedProcedures: number;
  approvedProcedures: number;
  recentActivities: RecentActivity[];
}

export interface RecentActivity {
  time: string;
  userName: string;
  action: string;
  target: string;
  detail?: string;
}

export interface AuditLogFilter {
  userId?: number;
  action?: string;
  targetType?: string;
  fromDate?: string;
  toDate?: string;
  page?: number;
  pageSize?: number;
}

export interface AuditLog {
  id: number;
  userId?: number;
  userName?: string;
  action: string;
  targetType?: string;
  targetId?: number;
  targetName?: string;
  detail?: string;
  ipAddress?: string;
  actionTime: string;
}

export interface AuditLogPagedResult {
  items: AuditLog[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface Unit {
  id: number;
  code: string;
  name: string;
  type: string;
}

export const apiClient = new ApiClient();

