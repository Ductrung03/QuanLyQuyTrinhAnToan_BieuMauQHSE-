'use client';

import { useState, useEffect, useCallback } from 'react';
import { apiClient, AuditLog, AuditLogFilter, AuditLogPagedResult } from '@/lib/api-client';
import {
    Clock, User, Filter,
    ChevronLeft, ChevronRight, Activity,
    Download
} from 'lucide-react';
import { format } from 'date-fns';
import { vi } from 'date-fns/locale';

export default function AuditLogPage() {
    const [logs, setLogs] = useState<AuditLog[]>([]);
    const [loading, setLoading] = useState(true);
    const [totalCount, setTotalCount] = useState(0);
    const [totalPages, setTotalPages] = useState(0);
    const [actionTypes, setActionTypes] = useState<string[]>([]);
    const [targetTypes, setTargetTypes] = useState<string[]>([]);

    const [filter, setFilter] = useState<AuditLogFilter>({
        page: 1,
        pageSize: 20,
    });

    const loadLogs = useCallback(async () => {
        try {
            setLoading(true);
            const response = await apiClient.getAuditLogs(filter);
            if (response.success && response.data) {
                const result = response.data as AuditLogPagedResult;
                setLogs(result.items || []);
                setTotalCount(result.totalCount);
                setTotalPages(result.totalPages);
            }
        } catch (error) {
            console.error('Error loading audit logs:', error);
        } finally {
            setLoading(false);
        }
    }, [filter]);

    const loadFilterOptions = useCallback(async () => {
        try {
            const [actionsRes, targetsRes] = await Promise.all([
                apiClient.getAuditActionTypes(),
                apiClient.getAuditTargetTypes()
            ]);
            if (actionsRes.success && actionsRes.data) {
                setActionTypes(actionsRes.data);
            }
            if (targetsRes.success && targetsRes.data) {
                setTargetTypes(targetsRes.data);
            }
        } catch (error) {
            console.error('Error loading filter options:', error);
        }
    }, []);

    useEffect(() => {
        loadLogs();
    }, [loadLogs]);

    useEffect(() => {
        loadFilterOptions();
    }, [loadFilterOptions]);

    const handlePageChange = (newPage: number) => {
        setFilter(prev => ({ ...prev, page: newPage }));
    };

    const handleFilterChange = (key: keyof AuditLogFilter, value: string | number | undefined) => {
        setFilter(prev => ({ ...prev, [key]: value || undefined, page: 1 }));
    };

    const getActionColor = (action: string): string => {
        const colors: Record<string, string> = {
            Create: 'bg-green-100 text-green-800 border-green-200',
            Edit: 'bg-blue-100 text-blue-800 border-blue-200',
            Delete: 'bg-red-100 text-red-800 border-red-200',
            Submit: 'bg-indigo-100 text-indigo-800 border-indigo-200',
            Approve: 'bg-emerald-100 text-emerald-800 border-emerald-200',
            Reject: 'bg-orange-100 text-orange-800 border-orange-200',
            Recall: 'bg-yellow-100 text-yellow-800 border-yellow-200',
            Login: 'bg-purple-100 text-purple-800 border-purple-200',
            Logout: 'bg-gray-100 text-gray-800 border-gray-200',
        };
        return colors[action] || 'bg-gray-100 text-gray-800 border-gray-200';
    };

    const exportToJSON = () => {
        const dataStr = JSON.stringify(logs, null, 2);
        const dataBlob = new Blob([dataStr], { type: 'application/json' });
        const url = URL.createObjectURL(dataBlob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `audit_log_${format(new Date(), 'yyyy-MM-dd_HHmmss')}.json`;
        link.click();
        URL.revokeObjectURL(url);
    };

    if (loading && logs.length === 0) {
        return (
            <div className="space-y-6">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Nhật ký hệ thống</h1>
                    <p className="text-gray-600 mt-1">Đang tải...</p>
                </div>
                <div className="bg-white rounded-lg shadow-sm border p-8">
                    <div className="animate-pulse space-y-4">
                        {[1, 2, 3, 4, 5].map(i => (
                            <div key={i} className="h-12 bg-gray-200 rounded"></div>
                        ))}
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Nhật ký hệ thống</h1>
                    <p className="text-gray-600 mt-1">
                        Theo dõi tất cả hoạt động trong hệ thống ({totalCount} bản ghi)
                    </p>
                </div>
                <button
                    onClick={exportToJSON}
                    className="flex items-center gap-2 px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 transition-colors"
                >
                    <Download className="w-4 h-4" />
                    Xuất JSON
                </button>
            </div>

            {/* Filters */}
            <div className="bg-white rounded-lg shadow-sm border p-4">
                <div className="flex flex-wrap gap-4">
                    <div className="flex items-center gap-2">
                        <Filter className="w-4 h-4 text-gray-500" />
                        <span className="text-sm font-medium text-gray-700">Bộ lọc:</span>
                    </div>

                    <select
                        value={filter.action || ''}
                        onChange={(e) => handleFilterChange('action', e.target.value)}
                        className="px-3 py-2 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                    >
                        <option value="">-- Tất cả hành động --</option>
                        {actionTypes.map(action => (
                            <option key={action} value={action}>{action}</option>
                        ))}
                    </select>

                    <select
                        value={filter.targetType || ''}
                        onChange={(e) => handleFilterChange('targetType', e.target.value)}
                        className="px-3 py-2 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                    >
                        <option value="">-- Tất cả đối tượng --</option>
                        {targetTypes.map(type => (
                            <option key={type} value={type}>{type}</option>
                        ))}
                    </select>

                    <input
                        type="date"
                        value={filter.fromDate || ''}
                        onChange={(e) => handleFilterChange('fromDate', e.target.value)}
                        className="px-3 py-2 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                        placeholder="Từ ngày"
                    />

                    <input
                        type="date"
                        value={filter.toDate || ''}
                        onChange={(e) => handleFilterChange('toDate', e.target.value)}
                        className="px-3 py-2 border border-gray-200 rounded-lg text-sm focus:ring-2 focus:ring-blue-100 focus:border-blue-500"
                        placeholder="Đến ngày"
                    />

                    <button
                        onClick={() => setFilter({ page: 1, pageSize: 20 })}
                        className="px-3 py-2 text-sm text-gray-600 hover:text-gray-900"
                    >
                        Xóa bộ lọc
                    </button>
                </div>
            </div>

            {/* Table */}
            <div className="bg-white rounded-lg shadow-sm border overflow-hidden">
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50 border-b border-gray-200">
                            <tr>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Thời gian
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Người dùng
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Hành động
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Đối tượng
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Chi tiết
                                </th>
                            </tr>
                        </thead>
                        <tbody className="bg-white divide-y divide-gray-200">
                            {logs.length === 0 ? (
                                <tr>
                                    <td colSpan={5} className="px-6 py-12 text-center">
                                        <Activity className="w-12 h-12 text-gray-400 mx-auto mb-3" />
                                        <p className="text-gray-600">Chưa có nhật ký nào</p>
                                    </td>
                                </tr>
                            ) : (
                                logs.map((log) => (
                                    <tr key={log.id} className="hover:bg-gray-50">
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <div className="flex items-center gap-2 text-sm text-gray-600">
                                                <Clock className="w-4 h-4" />
                                                {format(new Date(log.actionTime), 'dd/MM/yyyy HH:mm:ss', { locale: vi })}
                                            </div>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <div className="flex items-center gap-2">
                                                <div className="w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center">
                                                    <User className="w-4 h-4 text-blue-600" />
                                                </div>
                                                <span className="text-sm text-gray-900">
                                                    {log.userName || 'Unknown'}
                                                </span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4 whitespace-nowrap">
                                            <span className={`px-2 py-1 text-xs font-medium rounded-full border ${getActionColor(log.action)}`}>
                                                {log.action}
                                            </span>
                                        </td>
                                        <td className="px-6 py-4">
                                            <div className="text-sm">
                                                {log.targetType && (
                                                    <span className="text-gray-500">[{log.targetType}] </span>
                                                )}
                                                <span className="text-gray-900">{log.targetName || '-'}</span>
                                            </div>
                                        </td>
                                        <td className="px-6 py-4">
                                            <span className="text-sm text-gray-600">
                                                {log.detail || '-'}
                                            </span>
                                        </td>
                                    </tr>
                                ))
                            )}
                        </tbody>
                    </table>
                </div>

                {/* Pagination */}
                {totalPages > 1 && (
                    <div className="px-6 py-4 border-t border-gray-200 flex items-center justify-between">
                        <div className="text-sm text-gray-600">
                            Trang {filter.page} / {totalPages} ({totalCount} bản ghi)
                        </div>
                        <div className="flex items-center gap-2">
                            <button
                                onClick={() => handlePageChange((filter.page || 1) - 1)}
                                disabled={(filter.page || 1) <= 1}
                                className="p-2 rounded-lg border border-gray-200 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                <ChevronLeft className="w-4 h-4" />
                            </button>

                            {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                                const startPage = Math.max(1, (filter.page || 1) - 2);
                                const pageNum = startPage + i;
                                if (pageNum > totalPages) return null;
                                return (
                                    <button
                                        key={pageNum}
                                        onClick={() => handlePageChange(pageNum)}
                                        className={`px-3 py-1 rounded-lg text-sm ${pageNum === (filter.page || 1)
                                            ? 'bg-blue-600 text-white'
                                            : 'border border-gray-200 hover:bg-gray-50'
                                            }`}
                                    >
                                        {pageNum}
                                    </button>
                                );
                            })}

                            <button
                                onClick={() => handlePageChange((filter.page || 1) + 1)}
                                disabled={(filter.page || 1) >= totalPages}
                                className="p-2 rounded-lg border border-gray-200 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                            >
                                <ChevronRight className="w-4 h-4" />
                            </button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}
