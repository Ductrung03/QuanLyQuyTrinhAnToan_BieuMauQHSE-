import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Search } from 'lucide-react';
import { Card, Input, Table, Select } from '@/components/ui';
import { apiClient, AuditLog } from '@/api/client';
import { formatAuditSummary, getActionLabel, getTargetLabel } from '@/utils/labels';
import { format } from 'date-fns';
import { vi } from 'date-fns/locale';

export default function AuditPage() {
    const [logs, setLogs] = useState<AuditLog[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchQuery, setSearchQuery] = useState('');
    const [actionFilter, setActionFilter] = useState('');

    useEffect(() => {
        const loadLogs = async () => {
            const response = await apiClient.getAuditLogs();
            if (response.success && response.data) {
                setLogs(Array.isArray(response.data) ? response.data : []);
            } else {
                setLogs([]);
            }
            setLoading(false);
        };
        loadLogs();
    }, []);

    const filteredLogs = logs.filter((log) => {
        const searchValue = searchQuery.toLowerCase();
        const matchesSearch =
            (log.userName ?? '').toLowerCase().includes(searchValue) ||
            (log.target ?? '').toLowerCase().includes(searchValue) ||
            (log.detail?.toLowerCase().includes(searchValue) ?? false);
        const matchesAction = !actionFilter || log.action === actionFilter;
        return matchesSearch && matchesAction;
    });

    const actionOptions = [
        { value: '', label: 'Tất cả hành động' },
        { value: 'Create', label: 'Tạo' },
        { value: 'Update', label: 'Cập nhật' },
        { value: 'Delete', label: 'Xóa' },
        { value: 'Submit', label: 'Nộp' },
        { value: 'Approve', label: 'Phê duyệt' },
        { value: 'Reject', label: 'Từ chối' },
        { value: 'Recall', label: 'Thu hồi' },
        { value: 'Login', label: 'Đăng nhập' },
        { value: 'Logout', label: 'Đăng xuất' },
    ];

    const getActionColor = (action: string) => {
        const colors: Record<string, string> = {
            Create: 'bg-[var(--color-success-50)] text-[var(--color-success-600)]',
            Edit: 'bg-[var(--color-primary-50)] text-[var(--color-primary-600)]',
            Update: 'bg-[var(--color-primary-50)] text-[var(--color-primary-600)]',
            Delete: 'bg-[var(--color-danger-50)] text-[var(--color-danger-600)]',
            Submit: 'bg-purple-50 text-purple-600',
            Approve: 'bg-[var(--color-success-50)] text-[var(--color-success-600)]',
            Reject: 'bg-[var(--color-warning-50)] text-[var(--color-warning-600)]',
            Recall: 'bg-[var(--color-warning-50)] text-[var(--color-warning-600)]',
            Withdraw: 'bg-[var(--color-warning-50)] text-[var(--color-warning-600)]',
            Login: 'bg-[var(--color-neutral-100)] text-[var(--color-neutral-600)]',
            Logout: 'bg-[var(--color-neutral-100)] text-[var(--color-neutral-600)]'
        };
        return colors[action] || 'bg-[var(--color-neutral-100)] text-[var(--color-neutral-600)]';
    };

    const columns = [
        {
            key: 'time',
            header: 'Thời gian',
            render: (item: AuditLog) =>
                format(new Date(item.time), 'dd/MM/yyyy HH:mm:ss', { locale: vi }),
        },
        { key: 'userName', header: 'Người dùng' },
        {
            key: 'action',
            header: 'Hành động',
            render: (item: AuditLog) => (
                <span className={`px-2 py-1 text-xs font-medium rounded-full ${getActionColor(item.action)}`}>
                    {getActionLabel(item.action)}
                </span>
            ),
        },
        {
            key: 'summary',
            header: 'Nội dung',
            render: (item: AuditLog) => (
                <span className="text-sm text-[var(--color-neutral-700)]">
                    {formatAuditSummary(item.action, item.targetType, item.targetName)}
                </span>
            )
        },
        {
            key: 'target',
            header: 'Đối tượng',
            render: (item: AuditLog) => (
                <span className="text-sm text-[var(--color-neutral-600)]">
                    {getTargetLabel(item.targetType, item.targetName)}
                </span>
            )
        },
        { key: 'detail', header: 'Chi tiết', className: 'max-w-xs truncate' },
    ];

    return (
        <div className="space-y-6">
            {/* Header */}
            <motion.div
                initial={{ opacity: 0, y: -10 }}
                animate={{ opacity: 1, y: 0 }}
            >
                <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Nhật ký hoạt động</h1>
                <p className="text-[var(--color-neutral-500)] mt-1">
                    Theo dõi tất cả hoạt động trong hệ thống
                </p>
            </motion.div>

            {/* Filters */}
            <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.1 }}
            >
                <Card>
                    <div className="flex flex-col sm:flex-row gap-4">
                        <Input
                            placeholder="Tìm kiếm theo người dùng, đối tượng..."
                            icon={<Search className="w-4 h-4" />}
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className="flex-1"
                        />
                        <Select
                            options={actionOptions}
                            value={actionFilter}
                            onChange={(e) => setActionFilter(e.target.value)}
                            className="sm:w-48"
                        />
                    </div>
                </Card>
            </motion.div>

            {/* Table */}
            <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.2 }}
            >
                <Card padding="none">
                    <div className="p-4 border-b border-[var(--color-neutral-200)]">
                        <div className="flex items-center justify-between">
                            <h3 className="font-semibold text-[var(--color-neutral-900)]">Lịch sử hoạt động</h3>
                            <span className="text-sm text-[var(--color-neutral-500)]">
                                {filteredLogs.length} mục
                            </span>
                        </div>
                    </div>

                    {loading ? (
                        <div className="p-8 text-center text-[var(--color-neutral-500)]">Đang tải...</div>
                    ) : (
                        <Table columns={columns} data={filteredLogs} keyField="id" />
                    )}
                </Card>
            </motion.div>
        </div>
    );
}
