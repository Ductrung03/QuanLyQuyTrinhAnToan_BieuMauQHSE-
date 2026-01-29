export const stateLabels: Record<string, string> = {
  Draft: 'Nháp',
  Submitted: 'Đã gửi',
  'Under Review': 'Đang xem xét',
  Approved: 'Đã phê duyệt',
  Rejected: 'Đã từ chối'
};

export const actionLabels: Record<string, string> = {
  Create: 'Tạo',
  Update: 'Cập nhật',
  Delete: 'Xóa',
  Submit: 'Nộp',
  Approve: 'Phê duyệt',
  Reject: 'Từ chối',
  Recall: 'Thu hồi',
  Withdraw: 'Thu hồi',
  UploadDocument: 'Tải tài liệu',
  UploadFile: 'Tải tệp',
  Deactivate: 'Vô hiệu hóa',
  Reactivate: 'Kích hoạt lại',
  AssignPermissions: 'Gán quyền',
  ResetPassword: 'Đặt lại mật khẩu',
  ChangePassword: 'Đổi mật khẩu',
  SeedPasswords: 'Thiết lập mật khẩu',
  Login: 'Đăng nhập',
  Logout: 'Đăng xuất'
};

export const targetTypeLabels: Record<string, string> = {
  Procedure: 'Quy trình',
  Template: 'Biểu mẫu',
  Submission: 'Biểu mẫu',
  ProcedureDocument: 'Tài liệu quy trình',
  User: 'Người dùng',
  Unit: 'Đơn vị',
  Role: 'Vai trò',
  Auth: 'Xác thực'
};

export const unitTypeLabels: Record<string, string> = {
  Headquarters: 'Trụ sở',
  Ship: 'Tàu',
  Department: 'Phòng ban',
  Branch: 'Chi nhánh'
};

export const getStateLabel = (state?: string) => (state ? stateLabels[state] || state : '');

export const getActionLabel = (action?: string) => (action ? actionLabels[action] || action : '');

export const getTargetLabel = (targetType?: string, targetName?: string) => {
  const typeLabel = targetType ? targetTypeLabels[targetType] || targetType : '';
  if (targetName && typeLabel) return `${typeLabel}: ${targetName}`;
  if (targetName) return targetName;
  return typeLabel;
};

export const getUnitTypeLabel = (type?: string) => (type ? unitTypeLabels[type] || type : '');

export const formatAuditSummary = (action?: string, targetType?: string, targetName?: string) => {
  const actionLabel = getActionLabel(action);
  const targetLabel = getTargetLabel(targetType, targetName);
  if (actionLabel && targetLabel) return `${actionLabel} ${targetLabel}`;
  return actionLabel || targetLabel || '';
};
