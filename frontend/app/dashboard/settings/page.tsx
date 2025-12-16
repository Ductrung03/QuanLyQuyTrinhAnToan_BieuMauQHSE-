'use client';

import { useState, useCallback } from 'react';
import { apiClient } from '@/lib/api-client';
import {
    Download, Upload, Database,
    Settings as SettingsIcon, Shield, Users,
    CheckCircle, AlertCircle, Info
} from 'lucide-react';
import { format } from 'date-fns';

export default function SettingsPage() {
    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState<{ type: 'success' | 'error' | 'info'; text: string } | null>(null);

    // Export all data to JSON
    const handleExportData = useCallback(async () => {
        try {
            setLoading(true);
            setMessage({ type: 'info', text: 'Đang xuất dữ liệu...' });

            // Fetch all data from APIs
            const [proceduresRes, templatesRes, submissionsRes, approvalsRes, auditLogsRes] = await Promise.all([
                apiClient.getProcedures(),
                apiClient.getTemplates(),
                apiClient.getMySubmissions(),
                apiClient.getPendingApprovals(),
                apiClient.getAuditLogs({ page: 1, pageSize: 1000 })
            ]);

            const exportData = {
                exportedAt: new Date().toISOString(),
                version: '1.0',
                data: {
                    procedures: proceduresRes.success ? proceduresRes.data : [],
                    templates: templatesRes.success ? templatesRes.data : [],
                    submissions: submissionsRes.success ? submissionsRes.data : [],
                    approvals: approvalsRes.success ? approvalsRes.data : [],
                    auditLogs: auditLogsRes.success ? auditLogsRes.data?.items : []
                }
            };

            const dataStr = JSON.stringify(exportData, null, 2);
            const dataBlob = new Blob([dataStr], { type: 'application/json' });
            const url = URL.createObjectURL(dataBlob);
            const link = document.createElement('a');
            link.href = url;
            link.download = `SSMS_backup_${format(new Date(), 'yyyy-MM-dd_HHmmss')}.json`;
            link.click();
            URL.revokeObjectURL(url);

            setMessage({ type: 'success', text: 'Xuất dữ liệu thành công!' });
        } catch (error) {
            console.error('Export error:', error);
            setMessage({ type: 'error', text: 'Lỗi khi xuất dữ liệu' });
        } finally {
            setLoading(false);
        }
    }, []);

    // Import data from JSON file
    const handleImportData = useCallback((event: React.ChangeEvent<HTMLInputElement>) => {
        const file = event.target.files?.[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = () => {
            try {
                const data = JSON.parse(reader.result as string);
                console.log('Imported data:', data);
                setMessage({
                    type: 'info',
                    text: 'Tính năng nhập dữ liệu đang được phát triển. Dữ liệu đã được đọc thành công.'
                });
            } catch (_error) {
                setMessage({ type: 'error', text: 'File JSON không hợp lệ' });
            }
        };
        reader.readAsText(file);
        event.target.value = '';
    }, []);

    const MessageComponent = () => {
        if (!message) return null;

        const icons = {
            success: <CheckCircle className="w-5 h-5 text-green-600" />,
            error: <AlertCircle className="w-5 h-5 text-red-600" />,
            info: <Info className="w-5 h-5 text-blue-600" />
        };

        const colors = {
            success: 'bg-green-50 border-green-200 text-green-800',
            error: 'bg-red-50 border-red-200 text-red-800',
            info: 'bg-blue-50 border-blue-200 text-blue-800'
        };

        return (
            <div className={`flex items-center gap-3 p-4 rounded-lg border ${colors[message.type]}`}>
                {icons[message.type]}
                <span>{message.text}</span>
                <button
                    onClick={() => setMessage(null)}
                    className="ml-auto text-gray-500 hover:text-gray-700"
                >
                    ×
                </button>
            </div>
        );
    };

    return (
        <div className="space-y-6">
            {/* Header */}
            <div>
                <h1 className="text-2xl font-bold text-gray-900">Cài đặt & Sao lưu</h1>
                <p className="text-gray-600 mt-1">
                    Quản lý cấu hình hệ thống và sao lưu dữ liệu
                </p>
            </div>

            {/* Message */}
            <MessageComponent />

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* Backup & Restore */}
                <div className="bg-white rounded-lg shadow-sm border p-6">
                    <div className="flex items-center gap-3 mb-4">
                        <div className="p-2 bg-blue-100 rounded-lg">
                            <Database className="w-5 h-5 text-blue-600" />
                        </div>
                        <h2 className="text-lg font-semibold text-gray-900">Sao lưu & Khôi phục</h2>
                    </div>

                    <p className="text-sm text-gray-600 mb-6">
                        Xuất toàn bộ dữ liệu ra file JSON để sao lưu hoặc di chuyển dữ liệu.
                    </p>

                    <div className="space-y-4">
                        <button
                            onClick={handleExportData}
                            disabled={loading}
                            className="w-full flex items-center justify-center gap-2 px-4 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
                        >
                            <Download className="w-5 h-5" />
                            {loading ? 'Đang xuất...' : 'Xuất dữ liệu (JSON)'}
                        </button>

                        <label className="w-full flex items-center justify-center gap-2 px-4 py-3 border-2 border-dashed border-gray-300 text-gray-600 rounded-lg hover:border-blue-500 hover:bg-blue-50 transition-colors cursor-pointer">
                            <Upload className="w-5 h-5" />
                            <span>Nhập dữ liệu (JSON)</span>
                            <input
                                type="file"
                                accept=".json,application/json"
                                onChange={handleImportData}
                                className="hidden"
                            />
                        </label>
                    </div>

                    <div className="mt-6 p-4 bg-gray-50 rounded-lg">
                        <h4 className="text-sm font-medium text-gray-700 mb-2">Lưu ý:</h4>
                        <ul className="text-sm text-gray-600 space-y-1 list-disc list-inside">
                            <li>Dữ liệu được lưu trữ trên server database</li>
                            <li>Xuất JSON để sao lưu/di chuyển máy</li>
                            <li>Khôi phục cần quyền Admin</li>
                        </ul>
                    </div>
                </div>

                {/* System Info */}
                <div className="bg-white rounded-lg shadow-sm border p-6">
                    <div className="flex items-center gap-3 mb-4">
                        <div className="p-2 bg-purple-100 rounded-lg">
                            <SettingsIcon className="w-5 h-5 text-purple-600" />
                        </div>
                        <h2 className="text-lg font-semibold text-gray-900">Thông tin hệ thống</h2>
                    </div>

                    <div className="space-y-4">
                        <div className="flex justify-between py-3 border-b border-gray-100">
                            <span className="text-gray-600">Phiên bản</span>
                            <span className="font-medium text-gray-900">v3.4.2</span>
                        </div>
                        <div className="flex justify-between py-3 border-b border-gray-100">
                            <span className="text-gray-600">Tên hệ thống</span>
                            <span className="font-medium text-gray-900">SSMS QHSE</span>
                        </div>
                        <div className="flex justify-between py-3 border-b border-gray-100">
                            <span className="text-gray-600">Cơ sở dữ liệu</span>
                            <span className="font-medium text-gray-900">MS SQL Server</span>
                        </div>
                        <div className="flex justify-between py-3 border-b border-gray-100">
                            <span className="text-gray-600">API Server</span>
                            <span className="font-medium text-green-600">Đang hoạt động</span>
                        </div>
                        <div className="flex justify-between py-3">
                            <span className="text-gray-600">Ngôn ngữ</span>
                            <span className="font-medium text-gray-900">Tiếng Việt</span>
                        </div>
                    </div>
                </div>

                {/* Security */}
                <div className="bg-white rounded-lg shadow-sm border p-6">
                    <div className="flex items-center gap-3 mb-4">
                        <div className="p-2 bg-green-100 rounded-lg">
                            <Shield className="w-5 h-5 text-green-600" />
                        </div>
                        <h2 className="text-lg font-semibold text-gray-900">Bảo mật</h2>
                    </div>

                    <div className="space-y-4">
                        <div className="flex items-center justify-between py-3 border-b border-gray-100">
                            <div>
                                <p className="font-medium text-gray-900">Xác thực JWT</p>
                                <p className="text-sm text-gray-500">Token-based authentication</p>
                            </div>
                            <span className="px-2 py-1 bg-green-100 text-green-700 text-xs font-medium rounded-full">
                                Đang bật
                            </span>
                        </div>
                        <div className="flex items-center justify-between py-3 border-b border-gray-100">
                            <div>
                                <p className="font-medium text-gray-900">Phân quyền RBAC</p>
                                <p className="text-sm text-gray-500">Role-based access control</p>
                            </div>
                            <span className="px-2 py-1 bg-green-100 text-green-700 text-xs font-medium rounded-full">
                                Đang bật
                            </span>
                        </div>
                        <div className="flex items-center justify-between py-3">
                            <div>
                                <p className="font-medium text-gray-900">Audit Log</p>
                                <p className="text-sm text-gray-500">Ghi nhật ký hệ thống</p>
                            </div>
                            <span className="px-2 py-1 bg-green-100 text-green-700 text-xs font-medium rounded-full">
                                Đang bật
                            </span>
                        </div>
                    </div>
                </div>

                {/* User Management Preview */}
                <div className="bg-white rounded-lg shadow-sm border p-6">
                    <div className="flex items-center gap-3 mb-4">
                        <div className="p-2 bg-orange-100 rounded-lg">
                            <Users className="w-5 h-5 text-orange-600" />
                        </div>
                        <h2 className="text-lg font-semibold text-gray-900">Quản lý người dùng</h2>
                    </div>

                    <p className="text-sm text-gray-600 mb-4">
                        Quản lý tài khoản người dùng và phân quyền truy cập.
                    </p>

                    <div className="p-4 bg-orange-50 rounded-lg border border-orange-200">
                        <p className="text-sm text-orange-800">
                            Tính năng quản lý người dùng đang được phát triển.
                            Vui lòng liên hệ Admin để thay đổi quyền truy cập.
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}
