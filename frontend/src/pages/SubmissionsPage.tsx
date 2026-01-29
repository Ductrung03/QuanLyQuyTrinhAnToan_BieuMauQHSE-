import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Search, Plus, Eye, Send, X, RotateCcw, Loader2 } from 'lucide-react';
import { Card, Button, Input, Table, StateBadge, EmptyState, Select } from '@/components/ui';
import { apiClient, Submission } from '@/api/client';
import { format } from 'date-fns';
import { vi } from 'date-fns/locale';
import SubmissionModal from '@/components/SubmissionModal';
import { useToast } from '@/components/ui/Toast';

export default function SubmissionsPage() {
    const toast = useToast();
    const [submissions, setSubmissions] = useState<Submission[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchQuery, setSearchQuery] = useState('');
    const [stateFilter, setStateFilter] = useState('');
    const [isSubmissionModalOpen, setIsSubmissionModalOpen] = useState(false);
    const [recallingId, setRecallingId] = useState<number | null>(null);

    useEffect(() => {
        loadSubmissions();
    }, []);

    const loadSubmissions = async () => {
        setLoading(true);
        try {
            const response = await apiClient.getSubmissions();
            if (response.success && response.data) {
                setSubmissions(Array.isArray(response.data) ? response.data : []);
            } else {
                setSubmissions([]);
                toast.error('Lỗi', response.message || 'Không thể tải danh sách biểu mẫu');
            }
        } catch {
            setSubmissions([]);
            toast.error('Lỗi', 'Không thể tải danh sách biểu mẫu');
        } finally {
            setLoading(false);
        }
    };

    const handleRecall = async (id: number) => {
        if (!confirm('Bạn có chắc muốn thu hồi biểu mẫu này?')) return;

        setRecallingId(id);
        try {
            const response = await apiClient.withdrawSubmission(id);

            if (response.success) {
                toast.success('Đã thu hồi biểu mẫu');
                loadSubmissions();
            } else {
                toast.error(response.message || 'Thu hồi thất bại');
            }
        } catch {
            toast.error('Lỗi', 'Thu hồi thất bại');
        } finally {
            setRecallingId(null);
        }
    };

    const filteredSubmissions = submissions.filter((s) => {
        const searchValue = searchQuery.toLowerCase();
        const matchesSearch =
            (s.submissionCode ?? '').toLowerCase().includes(searchValue) ||
            (s.templateName ?? '').toLowerCase().includes(searchValue);
        const matchesState = !stateFilter || s.state === stateFilter;
        return matchesSearch && matchesState;
    });

    const columns = [
        { key: 'submissionCode', header: 'Mã nộp' },
        { key: 'templateName', header: 'Tên biểu mẫu' },
        { key: 'procedureCode', header: 'Quy trình' },
        { key: 'sendingUnitName', header: 'Đơn vị nộp' },
        {
            key: 'state',
            header: 'Trạng thái',
            render: (item: Submission) => <StateBadge state={item.state} />,
        },
        {
            key: 'submittedAt',
            header: 'Ngày nộp',
            render: (item: Submission) =>
                item.submittedAt ? format(new Date(item.submittedAt), 'dd/MM/yyyy HH:mm', { locale: vi }) : '-',
        },
        {
            key: 'actions',
            header: '',
            render: (item: Submission) => (
                <div className="flex items-center gap-1">
                    <button className="p-1.5 rounded-lg hover:bg-[var(--color-neutral-100)] text-[var(--color-neutral-500)]" title="Xem chi tiết">
                        <Eye className="w-4 h-4" />
                    </button>
                    {item.state === 'Draft' && (
                        <button className="p-1.5 rounded-lg hover:bg-[var(--color-primary-50)] text-[var(--color-primary-600)]" title="Gửi duyệt">
                            <Send className="w-4 h-4" />
                        </button>
                    )}
                    {item.state === 'Submitted' && (
                        <button
                            className="p-1.5 rounded-lg hover:bg-[var(--color-warning-50)] text-[var(--color-warning-600)]"
                            onClick={() => handleRecall(item.id)}
                            disabled={recallingId === item.id}
                            title="Thu hồi"
                        >
                            {recallingId === item.id ? (
                                <Loader2 className="w-4 h-4 animate-spin" />
                            ) : (
                                <RotateCcw className="w-4 h-4" />
                            )}
                        </button>
                    )}
                </div>
            ),
        },
    ];

    const stateOptions = [
        { value: '', label: 'Tất cả trạng thái' },
        { value: 'Draft', label: 'Nháp' },
        { value: 'Submitted', label: 'Đã gửi' },
        { value: 'Under Review', label: 'Đang xem xét' },
        { value: 'Approved', label: 'Đã phê duyệt' },
        { value: 'Rejected', label: 'Đã từ chối' },
    ];

    return (
        <div className="space-y-6">
            {/* Header */}
            <motion.div
                initial={{ opacity: 0, y: -10 }}
                animate={{ opacity: 1, y: 0 }}
                className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4"
            >
                <div>
                    <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Nộp Biểu mẫu</h1>
                    <p className="text-[var(--color-neutral-500)] mt-1">
                        Quản lý các biểu mẫu đã nộp và theo dõi trạng thái
                    </p>
                </div>
                <Button icon={<Plus className="w-4 h-4" />} onClick={() => setIsSubmissionModalOpen(true)}>
                    Nộp biểu mẫu mới
                </Button>
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
                            placeholder="Tìm kiếm theo mã hoặc tên..."
                            icon={<Search className="w-4 h-4" />}
                            value={searchQuery}
                            onChange={(e) => setSearchQuery(e.target.value)}
                            className="flex-1"
                        />
                        <Select
                            options={stateOptions}
                            value={stateFilter}
                            onChange={(e) => setStateFilter(e.target.value)}
                            className="sm:w-48"
                        />
                        {stateFilter && (
                            <Button variant="ghost" size="sm" onClick={() => setStateFilter('')}>
                                <X className="w-4 h-4 mr-1" />
                                Xóa bộ lọc
                            </Button>
                        )}
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
                            <h3 className="font-semibold text-[var(--color-neutral-900)]">
                                Danh sách biểu mẫu đã nộp
                            </h3>
                            <span className="text-sm text-[var(--color-neutral-500)]">
                                {filteredSubmissions.length} mục
                            </span>
                        </div>
                    </div>

                    {loading ? (
                        <div className="p-8 text-center text-[var(--color-neutral-500)]">Đang tải...</div>
                    ) : filteredSubmissions.length > 0 ? (
                        <Table
                            columns={columns}
                            data={filteredSubmissions}
                            keyField="id"
                        />
                    ) : (
                        <EmptyState
                            title="Chưa có biểu mẫu nào"
                            description="Bắt đầu bằng cách nộp biểu mẫu đầu tiên"
                        />
                    )}
                </Card>
            </motion.div>

            {/* Submission Modal */}
            <SubmissionModal
                isOpen={isSubmissionModalOpen}
                onClose={() => setIsSubmissionModalOpen(false)}
                onSuccess={() => {
                    setIsSubmissionModalOpen(false);
                    loadSubmissions();
                }}
            />
        </div>
    );
}
