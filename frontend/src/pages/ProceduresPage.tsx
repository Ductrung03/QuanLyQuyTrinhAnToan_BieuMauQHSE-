import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import {
    Plus,
    Search,
    FileText,
    Clock,
    FileCheck,
    Trash2,
    Edit,
    Download,
    Eye,
    ChevronRight,
} from 'lucide-react';
import {
    Card,
    Button,
    Input,
    Modal,
    Table,
    StateBadge,
    EmptyState,
    CardSkeleton,
} from '@/components/ui';
import { useToast } from '@/components/ui/Toast';
import { apiClient, Procedure, Template } from '@/api/client';
import TemplateModal from '@/components/TemplateModal';
import { format } from 'date-fns';
import { vi } from 'date-fns/locale';

type TabType = 'list' | 'info' | 'templates' | 'changelog';

export default function ProceduresPage() {
    const toast = useToast();
    const [procedures, setProcedures] = useState<Procedure[]>([]);
    const [templates, setTemplates] = useState<Template[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchQuery, setSearchQuery] = useState('');
    const [selectedProcedure, setSelectedProcedure] = useState<Procedure | null>(null);
    const [activeTab, setActiveTab] = useState<TabType>('list');
    const [showModal, setShowModal] = useState(false);
    const [editingProcedure, setEditingProcedure] = useState<Procedure | null>(null);
    const [showTemplateModal, setShowTemplateModal] = useState(false);


    // Load procedures
    useEffect(() => {
        const loadProcedures = async () => {
            try {
                const response = await apiClient.getProcedures();
                if (response.success && response.data) {
                    setProcedures(Array.isArray(response.data) ? response.data : []);
                } else {
                    setProcedures([]);
                    toast.error('Lỗi', response.message || 'Không thể tải danh sách quy trình');
                }
            } catch {
                setProcedures([]);
                toast.error('Lỗi', 'Không thể tải danh sách quy trình');
            } finally {
                setLoading(false);
            }
        };
        loadProcedures();
    }, []);

    // Load templates when procedure is selected
    useEffect(() => {
        if (selectedProcedure) {
            const loadTemplates = async () => {
                const response = await apiClient.getTemplatesByProcedure(selectedProcedure.id);
                if (response.success && response.data) {
                    setTemplates(Array.isArray(response.data) ? response.data : []);
                } else {
                    setTemplates([]);
                }
            };
            loadTemplates();
        }
    }, [selectedProcedure]);

    const normalizedSearch = searchQuery.toLowerCase();
    const filteredProcedures = procedures.filter(
        (p) =>
            (p.code ?? '').toLowerCase().includes(normalizedSearch) ||
            (p.name ?? '').toLowerCase().includes(normalizedSearch)
    );

    const handleCreateProcedure = () => {
        setEditingProcedure(null);
        setShowModal(true);
    };

    const handleEditProcedure = (procedure: Procedure) => {
        setEditingProcedure(procedure);
        setShowModal(true);
    };

    const handleDeleteProcedure = async (id: number) => {
        if (!confirm('Bạn có chắc muốn xóa quy trình này?')) return;
        const response = await apiClient.deleteProcedure(id);
        if (response.success) {
            setProcedures(procedures.filter((p) => p.id !== id));
            if (selectedProcedure?.id === id) {
                setSelectedProcedure(null);
                setActiveTab('list');
            }
            toast.success('Đã xóa quy trình');
        } else {
            toast.error('Không thể xóa', response.message);
        }
    };

    const handleSelectProcedure = (procedure: Procedure) => {
        setSelectedProcedure(procedure);
        setActiveTab('info');
    };

    const handleCreateTemplate = () => {
        if (!selectedProcedure) {
            toast.error('Chưa chọn quy trình', 'Vui lòng chọn quy trình trước khi thêm biểu mẫu');
            return;
        }
        setShowTemplateModal(true);
    };

    const tabs: { id: TabType; label: string; icon: React.ElementType; disabled?: boolean }[] = [
        { id: 'list', label: 'Danh sách quy trình', icon: FileText },
        { id: 'info', label: 'Thông tin chung', icon: FileCheck, disabled: !selectedProcedure },
        { id: 'templates', label: 'Danh sách mẫu', icon: FileText, disabled: !selectedProcedure },
        { id: 'changelog', label: 'Nhật ký thay đổi', icon: Clock, disabled: !selectedProcedure },
    ];

    const procedureColumns = [
        { key: 'code', header: 'Mã quy trình' },
        { key: 'name', header: 'Tên quy trình' },
        { key: 'version', header: 'Phiên bản' },
        {
            key: 'state',
            header: 'Trạng thái',
            render: (item: Procedure) => <StateBadge state={item.state} />,
        },
        {
            key: 'createdAt',
            header: 'Ngày tạo',
            render: (item: Procedure) =>
                format(new Date(item.createdAt), 'dd/MM/yyyy', { locale: vi }),
        },
        {
            key: 'actions',
            header: '',
            render: (item: Procedure) => (
                <div className="flex items-center gap-2">
                    <button
                        onClick={(e) => {
                            e.stopPropagation();
                            handleEditProcedure(item);
                        }}
                        className="p-1.5 rounded-lg hover:bg-[var(--color-neutral-100)] text-[var(--color-neutral-500)] hover:text-[var(--color-primary-600)]"
                    >
                        <Edit className="w-4 h-4" />
                    </button>
                    <button
                        onClick={(e) => {
                            e.stopPropagation();
                            handleDeleteProcedure(item.id);
                        }}
                        className="p-1.5 rounded-lg hover:bg-[var(--color-neutral-100)] text-[var(--color-neutral-500)] hover:text-[var(--color-danger-600)]"
                    >
                        <Trash2 className="w-4 h-4" />
                    </button>
                </div>
            ),
        },
    ];

    const templateColumns = [
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
                <div className="flex items-center gap-2">
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

    if (loading) {
        return (
            <div className="space-y-6">
                <div className="flex items-center justify-between">
                    <div>
                        <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Quản lý Quy trình</h1>
                        <p className="text-[var(--color-neutral-500)] mt-1">Đang tải...</p>
                    </div>
                </div>
                <div className="grid grid-cols-1 gap-6">
                    {[1, 2, 3].map((i) => <CardSkeleton key={i} />)}
                </div>
            </div>
        );
    }

    return (
        <div className="space-y-6">
            {/* Header */}
            <motion.div
                initial={{ opacity: 0, y: -10 }}
                animate={{ opacity: 1, y: 0 }}
                className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4"
            >
                <div>
                    <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Quản lý Quy trình</h1>
                    <p className="text-[var(--color-neutral-500)] mt-1">
                        Quản lý các quy trình an toàn và vận hành
                    </p>
                </div>
                <Button icon={<Plus className="w-4 h-4" />} onClick={handleCreateProcedure}>
                    Thêm quy trình
                </Button>
            </motion.div>

            {/* Tabs */}
            <motion.div
                initial={{ opacity: 0, y: 10 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.1 }}
            >
                <Card padding="none">
                    <div className="border-b border-[var(--color-neutral-200)]">
                        <div className="flex overflow-x-auto">
                            {tabs.map((tab) => {
                                const Icon = tab.icon;
                                return (
                                    <button
                                        key={tab.id}
                                        onClick={() => !tab.disabled && setActiveTab(tab.id)}
                                        disabled={tab.disabled}
                                        className={`relative flex items-center gap-2 px-6 py-4 text-sm font-medium whitespace-nowrap transition-colors ${activeTab === tab.id
                                            ? 'text-[var(--color-primary-600)]'
                                            : tab.disabled
                                                ? 'text-[var(--color-neutral-300)] cursor-not-allowed'
                                                : 'text-[var(--color-neutral-600)] hover:text-[var(--color-neutral-900)]'
                                            }`}
                                    >
                                        <Icon className="w-4 h-4" />
                                        {tab.label}
                                        {activeTab === tab.id && (
                                            <motion.div
                                                layoutId="tab-indicator"
                                                className="absolute bottom-0 left-0 right-0 h-0.5 bg-[var(--color-primary-600)]"
                                            />
                                        )}
                                    </button>
                                );
                            })}
                        </div>
                    </div>

                    <div className="p-6">
                        <AnimatePresence mode="wait">
                            {/* List Tab */}
                            {activeTab === 'list' && (
                                <motion.div
                                    key="list"
                                    initial={{ opacity: 0, x: -10 }}
                                    animate={{ opacity: 1, x: 0 }}
                                    exit={{ opacity: 0, x: 10 }}
                                >
                                    <div className="mb-4">
                                        <Input
                                            placeholder="Tìm kiếm theo mã hoặc tên quy trình..."
                                            icon={<Search className="w-4 h-4" />}
                                            value={searchQuery}
                                            onChange={(e) => setSearchQuery(e.target.value)}
                                            className="max-w-md"
                                        />
                                    </div>

                                    {filteredProcedures.length > 0 ? (
                                        <Table
                                            columns={procedureColumns}
                                            data={filteredProcedures}
                                            keyField="id"
                                            onRowClick={handleSelectProcedure}
                                            selectedId={selectedProcedure?.id}
                                        />
                                    ) : (
                                        <EmptyState
                                            title="Chưa có quy trình nào"
                                            description="Tạo quy trình đầu tiên để bắt đầu"
                                            action={{ label: 'Thêm quy trình', onClick: handleCreateProcedure }}
                                        />
                                    )}
                                </motion.div>
                            )}

                            {/* Info Tab */}
                            {activeTab === 'info' && selectedProcedure && (
                                <motion.div
                                    key="info"
                                    initial={{ opacity: 0, x: -10 }}
                                    animate={{ opacity: 1, x: 0 }}
                                    exit={{ opacity: 0, x: 10 }}
                                    className="space-y-4"
                                >
                                    <div className="flex items-center gap-2 text-sm text-[var(--color-neutral-500)] mb-4">
                                        <span>Quy trình</span>
                                        <ChevronRight className="w-4 h-4" />
                                        <span className="text-[var(--color-neutral-900)] font-medium">
                                            {selectedProcedure.code}
                                        </span>
                                    </div>

                                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                        <div className="space-y-4">
                                            <div>
                                                <label className="text-sm text-[var(--color-neutral-500)]">Mã quy trình</label>
                                                <p className="font-medium">{selectedProcedure.code}</p>
                                            </div>
                                            <div>
                                                <label className="text-sm text-[var(--color-neutral-500)]">Tên quy trình</label>
                                                <p className="font-medium">{selectedProcedure.name}</p>
                                            </div>
                                            <div>
                                                <label className="text-sm text-[var(--color-neutral-500)]">Phiên bản</label>
                                                <p className="font-medium">{selectedProcedure.version}</p>
                                            </div>
                                        </div>
                                        <div className="space-y-4">
                                            <div>
                                                <label className="text-sm text-[var(--color-neutral-500)]">Trạng thái</label>
                                                <div className="mt-1">
                                                    <StateBadge state={selectedProcedure.state} />
                                                </div>
                                            </div>
                                            <div>
                                                <label className="text-sm text-[var(--color-neutral-500)]">Người tạo</label>
                                                <p className="font-medium">{selectedProcedure.ownerUserName || '-'}</p>
                                            </div>
                                            <div>
                                                <label className="text-sm text-[var(--color-neutral-500)]">Ngày tạo</label>
                                                <p className="font-medium">
                                                    {format(new Date(selectedProcedure.createdAt), 'dd/MM/yyyy HH:mm', { locale: vi })}
                                                </p>
                                            </div>
                                        </div>
                                    </div>

                                    {selectedProcedure.description && (
                                        <div>
                                            <label className="text-sm text-[var(--color-neutral-500)]">Mô tả</label>
                                            <p className="mt-1 p-3 bg-[var(--color-neutral-50)] rounded-lg text-sm whitespace-pre-wrap">
                                                {selectedProcedure.description}
                                            </p>
                                        </div>
                                    )}

                                    <div className="flex gap-3 pt-4">
                                        <Button variant="secondary" onClick={() => handleEditProcedure(selectedProcedure)}>
                                            <Edit className="w-4 h-4 mr-2" />
                                            Chỉnh sửa
                                        </Button>
                                        <Button variant="danger" onClick={() => handleDeleteProcedure(selectedProcedure.id)}>
                                            <Trash2 className="w-4 h-4 mr-2" />
                                            Xóa
                                        </Button>
                                    </div>
                                </motion.div>
                            )}

                            {/* Templates Tab */}
                            {activeTab === 'templates' && selectedProcedure && (
                                <motion.div
                                    key="templates"
                                    initial={{ opacity: 0, x: -10 }}
                                    animate={{ opacity: 1, x: 0 }}
                                    exit={{ opacity: 0, x: 10 }}
                                >
                                    <div className="flex items-center justify-between mb-4">
                                        <h3 className="text-lg font-semibold">
                                            Biểu mẫu của quy trình {selectedProcedure.code}
                                        </h3>
                                        <Button size="sm" icon={<Plus className="w-4 h-4" />} onClick={handleCreateTemplate}>
                                            Thêm biểu mẫu
                                        </Button>
                                    </div>

                                    {templates.length > 0 ? (
                                        <Table
                                            columns={templateColumns}
                                            data={templates}
                                            keyField="id"
                                        />
                                    ) : (
                                        <EmptyState
                                            title="Chưa có biểu mẫu nào"
                                            description="Thêm biểu mẫu cho quy trình này"
                                            icon="file"
                                        />
                                    )}
                                </motion.div>
                            )}

                            {/* Changelog Tab */}
                            {activeTab === 'changelog' && selectedProcedure && (
                                <motion.div
                                    key="changelog"
                                    initial={{ opacity: 0, x: -10 }}
                                    animate={{ opacity: 1, x: 0 }}
                                    exit={{ opacity: 0, x: 10 }}
                                >
                                    <EmptyState
                                        title="Nhật ký thay đổi"
                                        description="Lịch sử thay đổi của quy trình sẽ được hiển thị ở đây"
                                    />
                                </motion.div>
                            )}
                        </AnimatePresence>
                    </div>
                </Card>
            </motion.div>

            {/* Procedure Modal - dùng key để reset form khi procedure thay đổi */}
            <ProcedureModal
                key={editingProcedure?.id || 'new'}
                isOpen={showModal}
                onClose={() => setShowModal(false)}
                procedure={editingProcedure}
                onSave={async (data) => {
                    if (editingProcedure) {
                        const response = await apiClient.updateProcedure(editingProcedure.id, data);
                        if (response.success && response.data) {
                            setProcedures(procedures.map((p) => p.id === editingProcedure.id ? response.data! : p));
                            toast.success('Đã cập nhật quy trình');
                            setShowModal(false);
                        } else {
                            toast.error('Không thể cập nhật', response.message || 'Có lỗi xảy ra');
                        }
                    } else {
                        const response = await apiClient.createProcedure(data);
                        if (response.success && response.data) {
                            setProcedures([response.data, ...procedures]);
                            toast.success('Đã thêm quy trình mới');
                            setShowModal(false);
                        } else {
                            toast.error('Không thể tạo mới', response.message || 'Có lỗi xảy ra');
                        }
                    }
                }}
            />

            <TemplateModal
                isOpen={showTemplateModal}
                onClose={() => setShowTemplateModal(false)}
                defaultProcedureId={selectedProcedure?.id}
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

// Procedure Modal Component
interface ProcedureModalProps {
    isOpen: boolean;
    onClose: () => void;
    procedure: Procedure | null;
    onSave: (data: Partial<Procedure>) => Promise<void>;
}

function ProcedureModal({ isOpen, onClose, procedure, onSave }: ProcedureModalProps) {
    // Khởi tạo formData trực tiếp từ procedure (component sẽ được remount nhờ key)
    const [formData, setFormData] = useState({
        name: procedure?.name || '',
        version: procedure?.version || '1.0',
        state: procedure?.state || 'Draft',
        description: procedure?.description || '',
    });

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        await onSave(formData);
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title={procedure ? 'Chỉnh sửa Quy trình' : 'Thêm Quy trình mới'}
            size="md"
            footer={
                <>
                    <Button variant="secondary" onClick={onClose}>
                        Hủy
                    </Button>
                    <Button onClick={handleSubmit}>
                        {procedure ? 'Cập nhật' : 'Tạo mới'}
                    </Button>
                </>
            }
        >
            <form onSubmit={handleSubmit} className="space-y-4">
                <Input
                    label="Tên quy trình"
                    placeholder="Nhập tên quy trình"
                    value={formData.name}
                    onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                    required
                />
                <div className="grid grid-cols-2 gap-4">
                    <Input
                        label="Phiên bản"
                        placeholder="1.0"
                        value={formData.version}
                        onChange={(e) => setFormData({ ...formData, version: e.target.value })}
                    />
                    <div>
                        <label className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5">
                            Trạng thái
                        </label>
                        <select
                            value={formData.state}
                            onChange={(e) => setFormData({ ...formData, state: e.target.value })}
                            className="w-full px-3 py-2 text-sm rounded-lg border border-[var(--color-neutral-300)] bg-white"
                        >
                                                <option value="Draft">Nháp</option>
                                                <option value="Submitted">Đã gửi</option>
                                                <option value="Approved">Đã phê duyệt</option>
                        </select>
                    </div>
                </div>
                <div>
                    <label className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5">
                        Mô tả
                    </label>
                    <textarea
                        value={formData.description}
                        onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                        className="w-full px-3 py-2 text-sm rounded-lg border border-[var(--color-neutral-300)] bg-white min-h-[100px]"
                        placeholder="Nhập mô tả quy trình..."
                    />
                </div>
            </form>
        </Modal>
    );
}
