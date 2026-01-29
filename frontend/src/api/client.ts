import axios, { AxiosError, AxiosResponse } from 'axios';

// Types
export interface User {
  id: number;
  userName: string;
  fullName: string;
  email: string;
  role: string;
  roleId?: number; // Added
  unitId: number;
  unitName: string;
  isActive?: boolean; // Added
}

export interface Role {
  id: number;
  name: string;
  code: string;
  description?: string;
  isSystemRole: boolean;
  createdAt?: string;
  updatedAt?: string;
}

export interface Permission {
  id: number;
  name: string;
  code: string;
  module: string;
  description?: string;
}

export interface PermissionGroup {
  module: string;
  permissions: Permission[];
}

export interface Unit {
  id: number;
  name: string;
  type: string;
}

export interface UnitApi {
  id: number;
  unitName?: string;
  unitType?: string;
  UnitName?: string;
  UnitType?: string;
}

export interface Procedure {
  id: number;
  code: string;
  name: string;
  version: string;
  state: string;
  description?: string;
  ownerUserId?: number;
  ownerUserName?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface Template {
  id: number;
  procedureId: number;
  templateKey?: string;
  templateNo?: string;
  name: string;
  templateType: string;
  state: string;
  fileName?: string;
}

export interface Submission {
  id: number;
  submissionCode: string;
  procedureId: number;
  procedureCode?: string;
  templateId: number;
  templateName?: string;
  sendingUnitId: number;
  sendingUnitName?: string;
  senderUserId: number;
  senderUserName?: string;
  state: string;
  submittedAt?: string;
  dueDate?: string;
  note?: string;
}

export interface Approval {
  id: number;
  submissionId: number;
  submissionCode?: string;
  templateName?: string;
  procedureCode?: string;
  senderUnitName?: string;
  state: string;
  submittedAt?: string;
}

export interface AuditLog {
  id: number;
  time: string;
  userId?: number;
  userName: string;
  action: string;
  target: string;
  targetType?: string;
  targetName?: string;
  detail?: string;
}

export interface AuditLogApi {
  id: number;
  userId?: number;
  userName?: string;
  action: string;
  targetType?: string;
  targetId?: number;
  targetName?: string;
  detail?: string;
  actionTime: string;
}

export interface AuditLogPagedResult {
  items: AuditLogApi[];
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface DashboardStats {
  totalProcedures: number;
  totalTemplates: number;
  totalSubmissions: number;
  pendingApprovals: number;
  approvedSubmissions: number;
  rejectedSubmissions: number;
  totalUsers: number;
  totalUnits: number;
  recentActivities: AuditLog[];
}

export interface ApiResponse<T> {
  success: boolean;
  data?: T;
  message?: string;
}

// API Client
const api = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request interceptor - add auth token
api.interceptors.request.use((config) => {
  // Try Zustand persist store first, fallback to direct localStorage
  let token = null;
  const authStorage = localStorage.getItem('ssms-auth');
  if (authStorage) {
    try {
      const parsed = JSON.parse(authStorage);
      token = parsed.state?.token;
    } catch {
      // fallback
    }
  }
  if (!token) {
    token = localStorage.getItem('token');
  }
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Response interceptor - handle errors
api.interceptors.response.use(
  (response: AxiosResponse) => response,
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// Helper to wrap API calls
async function apiCall<T>(promise: Promise<AxiosResponse<{ success: boolean; data?: T; message?: string }>>): Promise<ApiResponse<T>> {
  try {
    const response = await promise;
    const apiResponse = response.data;
    return {
      success: apiResponse.success,
      data: apiResponse.data,
      message: apiResponse.message
    };
  } catch (error) {
    const err = error as AxiosError<{ success?: boolean; message?: string; error?: string }>
    return {
      success: false,
      message: err.response?.data?.message || err.response?.data?.error || err.message || 'An error occurred',
    };
  }
}

// API Methods
export const apiClient = {
  // Auth
  login: (loginName: string, password: string) =>
    apiCall<{ token: string; user: User }>(api.post('/auth/login', { loginName, password })),
  logout: () => apiCall<void>(api.post('/auth/logout')),

  // Dashboard
  getDashboardStats: () => apiCall<DashboardStats>(api.get('/dashboard/stats')),

  // Units
  getUnits: async () => {
    const response = await apiCall<UnitApi[]>(api.get('/units'));
    if (response.success && response.data) {
      const items = Array.isArray(response.data) ? response.data : [];
      return {
        ...response,
        data: items.map((item) => ({
          id: item.id,
          name: item.unitName || item.UnitName || 'Không rõ',
          type: item.unitType || item.UnitType || 'Khác'
        }))
      };
    }
    return response as { success: boolean; data?: Unit[]; message?: string };
  },

  // Procedures
  getProcedures: () => apiCall<Procedure[]>(api.get('/procedures')),
  getProcedureById: (id: number) => apiCall<Procedure>(api.get(`/procedures/${id}`)),
  createProcedure: (data: Partial<Procedure>) => apiCall<Procedure>(api.post('/procedures', data)),
  updateProcedure: (id: number, data: Partial<Procedure>) =>
    apiCall<Procedure>(api.put(`/procedures/${id}`, data)),
  deleteProcedure: (id: number) => apiCall<void>(api.delete(`/procedures/${id}`)),

  // Templates
  getTemplates: () => apiCall<Template[]>(api.get('/templates')),
  getTemplatesByProcedure: (procedureId: number) =>
    apiCall<Template[]>(api.get(`/templates/procedure/${procedureId}`)),
  createTemplate: (data: FormData) =>
    apiCall<Template>(api.post('/templates', data, {
      headers: { 'Content-Type': 'multipart/form-data' }
    })),
  updateTemplate: (id: number, data: Partial<Template>) =>
    apiCall<Template>(api.put(`/templates/${id}`, data)),
  deleteTemplate: (id: number) => apiCall<void>(api.delete(`/templates/${id}`)),

  // Submissions
  getSubmissions: () => apiCall<Submission[]>(api.get('/submissions/my')),
  getSubmissionById: (id: number) => apiCall<Submission>(api.get(`/submissions/${id}`)),
  createSubmission: (data: Partial<Submission>) =>
    apiCall<Submission>(api.post('/submissions', data)),
  updateSubmission: (id: number, data: Partial<Submission>) =>
    apiCall<Submission>(api.put(`/submissions/${id}`, data)),
  submitForApproval: (id: number) => apiCall<void>(api.post(`/submissions/${id}/submit`)),
  withdrawSubmission: (id: number) => apiCall<void>(api.post(`/submissions/${id}/recall`)),

  // Approvals
  getApprovals: () => apiCall<Approval[]>(api.get('/approvals/pending')),
  getApprovalById: (id: number) => apiCall<Approval>(api.get(`/approvals/${id}`)),
  approveSubmission: (id: number, comment?: string) =>
    apiCall<void>(api.post(`/approvals/${id}/approve`, { note: comment })),
  rejectSubmission: (id: number, comment?: string) =>
    apiCall<void>(api.post(`/approvals/${id}/reject`, { note: comment })),

  // Audit Log
  getAuditLogs: async () => {
    const response = await apiCall<AuditLogPagedResult>(api.get('/auditlogs'));
    if (response.success && response.data) {
      const items = Array.isArray(response.data.items) ? response.data.items : [];
      return {
        ...response,
        data: items.map((item) => ({
          id: item.id,
          time: item.actionTime,
          userId: item.userId,
          userName: item.userName || 'Unknown',
          action: item.action,
          target: item.targetName || item.targetType || 'Unknown',
          targetType: item.targetType,
          targetName: item.targetName,
          detail: item.detail
        }))
      };
    }
    return response as { success: boolean; data?: AuditLog[]; message?: string };
  },

  // Roles (Phase 5)
  getRoles: () => apiCall<Role[]>(api.get('/roles')),
  getRoleById: (id: number) => apiCall<Role>(api.get(`/roles/${id}`)),
  createRole: (data: Partial<Role>) => apiCall<Role>(api.post('/roles', data)),
  updateRole: (id: number, data: Partial<Role>) => apiCall<Role>(api.put(`/roles/${id}`, data)),
  deleteRole: (id: number) => apiCall<void>(api.delete(`/roles/${id}`)),
  getRolePermissions: (id: number) => apiCall<Permission[]>(api.get(`/roles/${id}/permissions`)),
  assignPermissions: (id: number, permissionIds: number[]) => 
    apiCall<void>(api.post(`/roles/${id}/permissions`, { permissionIds })),

  // Permissions (Phase 5)
  getPermissions: () => apiCall<Permission[]>(api.get('/permissions')),
  getPermissionsGrouped: () => apiCall<PermissionGroup[]>(api.get('/permissions/grouped')),

  // Users (Enhanced)
  getUsers: () => apiCall<User[]>(api.get('/users/all')),
  createUser: (data: any) => apiCall<User>(api.post('/users', data)),
  updateUser: (id: number, data: any) => apiCall<User>(api.put(`/users/${id}`, data)),
  deleteUser: (id: number) => apiCall<void>(api.delete(`/users/${id}`)),

  // File Downloads
  downloadProcedureDocument: async (documentId: number, fileName: string) => {
    try {
      const response = await api.get(`/procedures/documents/${documentId}`, { 
        responseType: 'blob' 
      });
      const url = URL.createObjectURL(response.data);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
      return { success: true };
    } catch (error) {
      return { success: false, message: 'Tải xuống thất bại' };
    }
  },

  downloadTemplate: async (templateId: number, fileName: string) => {
    try {
      const response = await api.get(`/templates/${templateId}/download`, { 
        responseType: 'blob' 
      });
      const url = URL.createObjectURL(response.data);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
      return { success: true };
    } catch (error) {
      return { success: false, message: 'Tải xuống thất bại' };
    }
  },

  downloadSubmissionFile: async (fileId: number, fileName: string) => {
    try {
      const response = await api.get(`/submissions/files/${fileId}`, { 
        responseType: 'blob' 
      });
      const url = URL.createObjectURL(response.data);
      const link = document.createElement('a');
      link.href = url;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
      return { success: true };
    } catch (error) {
      return { success: false, message: 'Tải xuống thất bại' };
    }
  },
};
