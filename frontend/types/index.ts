/**
 * Common Types
 */

export type State =
  | "Draft"
  | "Submitted"
  | "Approved"
  | "Rejected"
  | "Obsolete"
  | "Archived";

export interface User {
  userId: number;
  userName: string;
  email: string;
  phone?: string;
  loginName: string;
  isActive: boolean;
}

export interface Unit {
  unitId: number;
  unitCode: string;
  unitName: string;
  unitType: "Ship" | "Department" | "Branch";
  isActive: boolean;
}

/**
 * Procedure Types
 */

export interface OpsProcedure {
  procedureId: number;
  code: string;
  name: string;
  version: string;
  state: State;
  ownerUserId: number;
  authorUserId: number;
  approverUserId?: number;
  createdDate: string;
  releasedDate?: string;
  description?: string;
  ownerUser?: User;
  authorUser?: User;
  approverUser?: User;
  documents?: OpsProcedureDocument[];
  templates?: OpsTemplate[];
}

export interface OpsProcedureDocument {
  procedureDocId: number;
  procedureId: number;
  docVersion: string;
  fileName: string;
  filePath: string;
  uploadedAt: string;
}

export interface OpsTemplate {
  templateId: number;
  procedureId: number;
  templateNo: string;
  name: string;
  templateType: "Form" | "Checklist";
  state: State;
  fileName?: string;
  filePath?: string;
}

/**
 * Submission Types
 */

export interface OpsSubmission {
  submissionId: number;
  submissionCode: string;
  procedureId: number;
  templateId?: number;
  unitId: number;
  senderUserId: number;
  sentAt: string;
  dueDate?: string;
  state: State;
  note?: string;
  procedure?: OpsProcedure;
  template?: OpsTemplate;
  unit?: Unit;
  senderUser?: User;
  files?: OpsSubmissionFile[];
  recipients?: OpsSubmissionRecipient[];
  approvals?: OpsApproval[];
}

export interface OpsSubmissionFile {
  submissionFileId: number;
  submissionId: number;
  fileName: string;
  filePath: string;
  uploadedAt: string;
}

export interface OpsSubmissionRecipient {
  submissionId: number;
  unitId: number;
  recipientRole: "To" | "Cc";
  unit?: Unit;
}

/**
 * Approval Types
 */

export interface OpsApproval {
  approvalId: number;
  approvalCode: string;
  submissionId: number;
  state: State;
  approverUserId: number;
  decisionAt?: string;
  comment?: string;
  approverUser?: User;
  submission?: OpsSubmission;
}

/**
 * API Response Types
 */

export interface ApiResponse<T> {
  success: boolean;
  data: T;
  message?: string;
}

export interface PaginatedResponse<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

/**
 * Form Types
 */

export interface ProcedureFormData {
  code: string;
  name: string;
  version: string;
  ownerUserId: number;
  authorUserId: number;
  approverUserId?: number;
  description?: string;
}

export interface SubmissionFormData {
  procedureId: number;
  templateId?: number;
  unitId: number;
  recipientUnits: { unitId: number; recipientRole: "To" | "Cc" }[];
  dueDate?: string;
  note?: string;
  files: File[];
}

export interface ApprovalFormData {
  action: "Approve" | "Reject";
  comment?: string;
}
