import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Search, Plus, Download, Eye } from 'lucide-react';
import { Card, Button, Input, Table, StateBadge, EmptyState } from '@/components/ui';
import { useToast } from '@/components/ui/Toast';
import { apiClient, Template, Procedure } from '@/api/client';
import TemplateModal from '@/components/TemplateModal';

export default function TemplatesPage() {
    const toast = useToast();
    const [templates, setTemplates] = useState<Template[]>([]);
    const [procedures, setProcedures] = useState<Procedure[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchQuery, setSearchQuery] = useState('');
    const [showTemplateModal, setShowTemplateModal] = useState(false);

    useEffect(() => {
        const loadTemplates = async () => {
            const response = await apiClient.getTemplates();
            if (response.success && response.data) {
                setTemplates(Array.isArray(response.data) ? response.data : []);
            } else {
                setTemplates([]);
            }
            setLoading(false);
        };
        loadTemplates();
    }, []);

    useEffect(() => {
        const loadProcedures = async () => {
            const response = await apiClient.getProcedures();
            if (response.success && response.data) {
                setProcedures(Array.isArray(response.data) ? response.data : []);
            } else {
                setProcedures([]);
            }
        };
        loadProcedures();
    }, []);

    const filteredTemplates = templates.filter(
        (t) =>
            t.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
            (t.templateNo?.toLowerCase().includes(searchQuery.toLowerCase()) ?? false)
    );

    const columns = [
        { key: 'templateNo', header: 'Số hiệu' },
        { key: 'name', header: 'Tên biểu mẫu' },
        { key: 'templateType', header: 'Loại' },
        {
            key: 'state',
            header: 'Trạng thái',
            render: (item: Template) => <StateBadge state={item.state} />,
        },
        {
            key: 'actions',
            header: '',
            render: (item: Template) => (
                <div className="flex items-center gap-1">
                    <button className="p-1.5 rounded-lg hover:bg-[var(--color-neutral-100)] text-[var(--color-neutral-500)]" title="Xem">
                        <Eye className="w-4 h-4" />
                    </button>
                    <button 
                        className="p-1.5 rounded-lg hover:bg-[var(--color-neutral-100)] text-[var(--color-neutral-500)]"
                        onClick={() => item.fileName && apiClient.downloadTemplate(item.id, item.fileName)}
                        title="Tải xuống"
                    >
                        <Download className="w-4 h-4" />
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
                className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4"
            >
                <div>
                    <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Biểu mẫu</h1>
                    <p className="text-[var(--color-neutral-500)] mt-1">
                        Quản lý các biểu mẫu và checklist
                    </p>
                </div>
                <Button icon={<Plus className="w-4 h-4" />} onClick={() => setShowTemplateModal(true)}>
                    Thêm biểu mẫu
                </Button>
            </motion.div>

            {/* Search */}
            <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.1 }}
            >
                <Card>
                    <Input
                        placeholder="Tìm kiếm theo số hiệu hoặc tên..."
                        icon={<Search className="w-4 h-4" />}
                        value={searchQuery}
                        onChange={(e) => setSearchQuery(e.target.value)}
                        className="max-w-md"
                    />
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
                            <h3 className="font-semibold text-[var(--color-neutral-900)]">Danh sách biểu mẫu</h3>
                            <span className="text-sm text-[var(--color-neutral-500)]">
                                {filteredTemplates.length} mục
                            </span>
                        </div>
                    </div>

                    {loading ? (
                        <div className="p-8 text-center text-[var(--color-neutral-500)]">Đang tải...</div>
                    ) : filteredTemplates.length > 0 ? (
                        <Table columns={columns} data={filteredTemplates} keyField="id" />
                    ) : (
                        <EmptyState
                            title="Chưa có biểu mẫu nào"
                            description="Thêm biểu mẫu để bắt đầu"
                            icon="file"
                        />
                    )}
                </Card>
            </motion.div>

            <TemplateModal
                isOpen={showTemplateModal}
                onClose={() => setShowTemplateModal(false)}
                procedureOptions={procedures.map((p) => ({ id: p.id, code: p.code, name: p.name }))}
                onSave={async (payload) => {
                    const response = await apiClient.createTemplate(payload);
                    if (response.success && response.data) {
                        setTemplates([response.data, ...templates]);
                        toast.success('Đã thêm biểu mẫu mới');
                        setShowTemplateModal(false);
                    } else {
                        toast.error('Không thể tạo biểu mẫu', response.message || 'Có lỗi xảy ra');
                    }
                }}
            />
        </div>
    );
}
