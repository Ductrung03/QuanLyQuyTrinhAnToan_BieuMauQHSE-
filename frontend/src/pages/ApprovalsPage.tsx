import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { CheckCircle, XCircle, Eye, Clock } from 'lucide-react';
import { Card, Button, Table, StateBadge, EmptyState, Modal } from '@/components/ui';
import { useToast } from '@/components/ui/Toast';
import { apiClient, Approval } from '@/api/client';
import { format } from 'date-fns';
import { vi } from 'date-fns/locale';

export default function ApprovalsPage() {
    const toast = useToast();
    const [approvals, setApprovals] = useState<Approval[]>([]);
    const [loading, setLoading] = useState(true);
    const [selectedApproval, setSelectedApproval] = useState<Approval | null>(null);
    const [showModal, setShowModal] = useState(false);
    const [modalAction, setModalAction] = useState<'approve' | 'reject'>('approve');
    const [comment, setComment] = useState('');
    const [processing, setProcessing] = useState(false);

    useEffect(() => {
        const loadApprovals = async () => {
            const response = await apiClient.getApprovals();
            if (response.success && response.data) {
                setApprovals(Array.isArray(response.data) ? response.data : []);
            } else {
                setApprovals([]);
            }
            setLoading(false);
        };
        loadApprovals();
    }, []);

    const handleApprove = (approval: Approval) => {
        setSelectedApproval(approval);
        setModalAction('approve');
        setComment('');
        setShowModal(true);
    };

    const handleReject = (approval: Approval) => {
        setSelectedApproval(approval);
        setModalAction('reject');
        setComment('');
        setShowModal(true);
    };

    const handleSubmitAction = async () => {
        if (!selectedApproval) return;

        setProcessing(true);
        const response =
            modalAction === 'approve'
                ? await apiClient.approveSubmission(selectedApproval.id, comment)
                : await apiClient.rejectSubmission(selectedApproval.id, comment);

        if (response.success) {
            setApprovals(approvals.filter((a) => a.id !== selectedApproval.id));
            toast.success(
                modalAction === 'approve' ? 'Đã phê duyệt thành công' : 'Đã từ chối biểu mẫu'
            );
            setShowModal(false);
        } else {
            toast.error('Thao tác thất bại', response.message);
        }
        setProcessing(false);
    };

    const pendingApprovals = approvals.filter((a) => a.state === 'Under Review' || a.state === 'Submitted');

    const columns = [
        { key: 'submissionCode', header: 'Mã nộp' },
        { key: 'templateName', header: 'Biểu mẫu' },
        { key: 'procedureCode', header: 'Quy trình' },
        { key: 'senderUnitName', header: 'Đơn vị nộp' },
        {
            key: 'state',
            header: 'Trạng thái',
            render: (item: Approval) => <StateBadge state={item.state} />,
        },
        {
            key: 'submittedAt',
            header: 'Ngày nộp',
            render: (item: Approval) =>
                item.submittedAt ? format(new Date(item.submittedAt), 'dd/MM/yyyy', { locale: vi }) : '-',
        },
        {
            key: 'actions',
            header: 'Thao tác',
            render: (item: Approval) => (
                <div className="flex items-center gap-2">
                    <button className="p-1.5 rounded-lg hover:bg-[var(--color-neutral-100)] text-[var(--color-neutral-500)]">
                        <Eye className="w-4 h-4" />
                    </button>
                    <button
                        onClick={() => handleApprove(item)}
                        className="p-1.5 rounded-lg hover:bg-[var(--color-success-50)] text-[var(--color-success-600)]"
                        title="Phê duyệt"
                    >
                        <CheckCircle className="w-4 h-4" />
                    </button>
                    <button
                        onClick={() => handleReject(item)}
                        className="p-1.5 rounded-lg hover:bg-[var(--color-danger-50)] text-[var(--color-danger-600)]"
                        title="Từ chối"
                    >
                        <XCircle className="w-4 h-4" />
                    </button>
                </div>
            ),
        },
    ];

    return (
        <div className="space-y-6">
            {/* Header */}
            <motion.div
                initial={{ opacity: 0, y: -10 }}
                animate={{ opacity: 1, y: 0 }}
            >
                <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Phê duyệt</h1>
                <p className="text-[var(--color-neutral-500)] mt-1">
                    Xem xét và phê duyệt các biểu mẫu được nộp
                </p>
            </motion.div>

            {/* Stats */}
            <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.1 }}
                className="grid grid-cols-1 sm:grid-cols-3 gap-4"
            >
                <Card className="bg-[var(--color-warning-50)] border-[var(--color-warning-200)]">
                    <div className="flex items-center gap-3">
                        <div className="p-2 bg-[var(--color-warning-500)] rounded-lg">
                            <Clock className="w-5 h-5 text-white" />
                        </div>
                        <div>
                            <p className="text-sm text-[var(--color-warning-700)]">Chờ phê duyệt</p>
                            <p className="text-2xl font-bold text-[var(--color-warning-800)]">{pendingApprovals.length}</p>
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center gap-3">
                        <div className="p-2 bg-[var(--color-success-500)] rounded-lg">
                            <CheckCircle className="w-5 h-5 text-white" />
                        </div>
                        <div>
                            <p className="text-sm text-[var(--color-neutral-500)]">Đã phê duyệt</p>
                            <p className="text-2xl font-bold">{approvals.filter((a) => a.state === 'Approved').length}</p>
                        </div>
                    </div>
                </Card>
                <Card>
                    <div className="flex items-center gap-3">
                        <div className="p-2 bg-[var(--color-danger-500)] rounded-lg">
                            <XCircle className="w-5 h-5 text-white" />
                        </div>
                        <div>
                            <p className="text-sm text-[var(--color-neutral-500)]">Từ chối</p>
                            <p className="text-2xl font-bold">{approvals.filter((a) => a.state === 'Rejected').length}</p>
                        </div>
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
                        <h3 className="font-semibold text-[var(--color-neutral-900)]">
                            Danh sách chờ phê duyệt
                        </h3>
                    </div>

                    {loading ? (
                        <div className="p-8 text-center text-[var(--color-neutral-500)]">Đang tải...</div>
                    ) : pendingApprovals.length > 0 ? (
                        <Table columns={columns} data={pendingApprovals} keyField="id" />
                    ) : (
                        <EmptyState
                            title="Không có mục nào chờ phê duyệt"
                            description="Các biểu mẫu mới sẽ hiển thị ở đây khi được nộp"
                            icon="inbox"
                        />
                    )}
                </Card>
            </motion.div>

            {/* Approval Modal */}
            <Modal
                isOpen={showModal}
                onClose={() => setShowModal(false)}
                title={modalAction === 'approve' ? 'Phê duyệt biểu mẫu' : 'Từ chối biểu mẫu'}
                size="sm"
                footer={
                    <>
                        <Button variant="secondary" onClick={() => setShowModal(false)}>
                            Hủy
                        </Button>
                        <Button
                            variant={modalAction === 'approve' ? 'success' : 'danger'}
                            onClick={handleSubmitAction}
                            loading={processing}
                        >
                            {modalAction === 'approve' ? 'Phê duyệt' : 'Từ chối'}
                        </Button>
                    </>
                }
            >
                <div className="space-y-4">
                    <p className="text-[var(--color-neutral-600)]">
                        {modalAction === 'approve'
                            ? 'Bạn có chắc muốn phê duyệt biểu mẫu này?'
                            : 'Bạn có chắc muốn từ chối biểu mẫu này?'}
                    </p>
                    <div>
                        <label className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5">
                            Ghi chú (tùy chọn)
                        </label>
                        <textarea
                            value={comment}
                            onChange={(e) => setComment(e.target.value)}
                            className="w-full px-3 py-2 text-sm rounded-lg border border-[var(--color-neutral-300)] bg-white min-h-[80px]"
                            placeholder={
                                modalAction === 'approve'
                                    ? 'Nhập ghi chú phê duyệt...'
                                    : 'Nhập lý do từ chối...'
                            }
                        />
                    </div>
                </div>
            </Modal>
        </div>
    );
}
