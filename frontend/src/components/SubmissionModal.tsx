import { useState, useEffect } from 'react';
import { Upload, X, FileText } from 'lucide-react';
import Modal from './ui/Modal';
import Input from './ui/Input';
import Select from './ui/Select';
import MultiSelect from './ui/MultiSelect';
import Button from './ui/Button';
import { apiClient, Procedure, Template, Unit } from '../api/client';
import { useToast } from '@/components/ui/Toast';

interface SubmissionModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSuccess: () => void;
}

export default function SubmissionModal({ isOpen, onClose, onSuccess }: SubmissionModalProps) {
    const toast = useToast();
    const [procedures, setProcedures] = useState<Procedure[]>([]);
    const [templates, setTemplates] = useState<Template[]>([]);
    const [units, setUnits] = useState<Unit[]>([]);
    const [loading, setLoading] = useState(false);
    const [submitting, setSubmitting] = useState(false);

    const [formData, setFormData] = useState({
        procedureId: '',
        templateId: '',
        title: '',
        content: '',
        recipientIds: [] as number[],
        dueDate: '',
        note: '',
    });

    const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
    const [errors, setErrors] = useState<Record<string, string>>({});

    useEffect(() => {
        if (isOpen) {
            loadData();
        }
    }, [isOpen]);

    useEffect(() => {
        if (formData.procedureId) {
            setFormData(prev => ({ ...prev, templateId: '' }));
            loadTemplates(Number(formData.procedureId));
        } else {
            setTemplates([]);
            setFormData(prev => ({ ...prev, templateId: '' }));
        }
    }, [formData.procedureId]);

    const loadData = async () => {
        setLoading(true);
        try {
            const [procRes, unitRes] = await Promise.all([
                apiClient.getProcedures(),
                apiClient.getUnits(),
            ]);

            if (procRes.success && procRes.data) {
                setProcedures(procRes.data);
            } else {
                setProcedures([]);
                toast.error('Lỗi', procRes.message || 'Không thể tải danh sách quy trình');
            }

            if (unitRes.success && unitRes.data) {
                setUnits(unitRes.data);
            } else {
                setUnits([]);
                toast.error('Lỗi', unitRes.message || 'Không thể tải danh sách đơn vị');
            }
        } catch {
            setProcedures([]);
            setUnits([]);
            toast.error('Lỗi', 'Không thể tải dữ liệu biểu mẫu');
        } finally {
            setLoading(false);
        }
    };

    const loadTemplates = async (procedureId: number) => {
        try {
            const response = await apiClient.getTemplatesByProcedure(procedureId);
            if (response.success && response.data) {
                setTemplates(response.data);
            } else {
                setTemplates([]);
                toast.error('Lỗi', response.message || 'Không thể tải danh sách mẫu biểu');
            }
        } catch {
            setTemplates([]);
            toast.error('Lỗi', 'Không thể tải danh sách mẫu biểu');
        }
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const files = Array.from(e.target.files || []);
        const validFiles = files.filter(file => {
            const ext = file.name.toLowerCase().split('.').pop();
            const validExts = ['docx', 'xlsx', 'pdf', 'doc', 'xls'];
            if (!validExts.includes(ext || '')) {
                toast.error(`File ${file.name} không hợp lệ. Chỉ chấp nhận .doc, .docx, .xls, .xlsx, .pdf`);
                return false;
            }
            if (file.size > 20 * 1024 * 1024) {
                toast.error(`File ${file.name} vượt quá 20MB`);
                return false;
            }
            return true;
        });

        setSelectedFiles(prev => [...prev, ...validFiles]);
    };

    const removeFile = (index: number) => {
        setSelectedFiles(prev => prev.filter((_, i) => i !== index));
    };

    const validate = (): boolean => {
        const newErrors: Record<string, string> = {};

        if (!formData.procedureId) newErrors.procedureId = 'Vui lòng chọn quy trình';
        if (!formData.templateId) newErrors.templateId = 'Vui lòng chọn mẫu';
        if (!formData.title.trim()) newErrors.title = 'Vui lòng nhập tiêu đề';
        if (!formData.content.trim()) newErrors.content = 'Vui lòng nhập nội dung';
        if (formData.recipientIds.length === 0) newErrors.recipientIds = 'Vui lòng chọn ít nhất 1 người nhận';
        if (selectedFiles.length === 0) newErrors.files = 'Vui lòng đính kèm ít nhất 1 tệp';

        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!validate()) {
            toast.error('Vui lòng kiểm tra lại thông tin');
            return;
        }

        setSubmitting(true);

        try {
            const formDataToSend = new FormData();
            formDataToSend.append('procedureId', formData.procedureId);
            formDataToSend.append('templateId', formData.templateId);
            formDataToSend.append('title', formData.title);
            formDataToSend.append('content', formData.content);
            formData.recipientIds.forEach(id => {
                formDataToSend.append('recipientIds', id.toString());
            });
            if (formData.dueDate) {
                formDataToSend.append('dueDate', formData.dueDate);
            }
            if (formData.note) {
                formDataToSend.append('note', formData.note);
            }
            selectedFiles.forEach(file => {
                formDataToSend.append('files', file);
            });

            const response = await apiClient.createSubmission(formDataToSend as any);

            if (response.success) {
                toast.success('Nộp biểu mẫu thành công');
                resetForm();
                onSuccess();
            } else {
                toast.error(response.message || 'Nộp biểu mẫu thất bại');
            }
        } catch (error) {
            toast.error('Đã xảy ra lỗi khi nộp biểu mẫu');
        } finally {
            setSubmitting(false);
        }
    };

    const resetForm = () => {
        setFormData({
            procedureId: '',
            templateId: '',
            title: '',
            content: '',
            recipientIds: [],
            dueDate: '',
            note: '',
        });
        setSelectedFiles([]);
        setErrors({});
    };

    const handleClose = () => {
        if (!submitting) {
            resetForm();
            onClose();
        }
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={handleClose}
            title="Nộp biểu mẫu mới"
            size="xl"
            footer={
                <>
                    <Button variant="secondary" onClick={handleClose} disabled={submitting}>
                        Hủy
                    </Button>
                    <Button onClick={handleSubmit} loading={submitting}>
                        Nộp biểu mẫu
                    </Button>
                </>
            }
        >
            {loading ? (
                <div className="flex items-center justify-center py-8">
                    <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[var(--color-primary-600)]"></div>
                </div>
            ) : (
                <form onSubmit={handleSubmit} className="space-y-4">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <Select
                            label="Quy trình *"
                            options={procedures.map(p => ({ value: p.id, label: `${p.code} - ${p.name}` }))}
                            value={formData.procedureId}
                            onChange={(e) => setFormData(prev => ({ ...prev, procedureId: e.target.value }))}
                            placeholder="Chọn quy trình"
                            error={errors.procedureId}
                        />

                        <Select
                            label="Chọn mẫu *"
                            options={templates.map(t => ({ value: t.id, label: `${t.templateNo} - ${t.name}` }))}
                            value={formData.templateId}
                            onChange={(e) => setFormData(prev => ({ ...prev, templateId: e.target.value }))}
                            placeholder="Chọn mẫu"
                            disabled={!formData.procedureId}
                            error={errors.templateId}
                        />
                    </div>

                    <Input
                        label="Tiêu đề *"
                        value={formData.title}
                        onChange={(e) => setFormData(prev => ({ ...prev, title: e.target.value }))}
                        placeholder="Nhập tiêu đề biểu mẫu"
                        error={errors.title}
                    />

                    <div>
                        <label className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5">
                            Nội dung *
                        </label>
                        <textarea
                            value={formData.content}
                            onChange={(e) => setFormData(prev => ({ ...prev, content: e.target.value }))}
                            placeholder="Nhập nội dung biểu mẫu"
                            rows={4}
                            className="w-full px-3 py-2 text-sm rounded-lg border border-[var(--color-neutral-300)] hover:border-[var(--color-neutral-400)] focus:outline-none focus:ring-2 focus:ring-[var(--color-primary-500)] focus:border-transparent resize-none"
                        />
                        {errors.content && (
                            <p className="mt-1.5 text-xs text-[var(--color-danger-600)]">{errors.content}</p>
                        )}
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <MultiSelect
                            label="Người nhận *"
                            options={units.map(u => ({ value: u.id, label: u.name }))}
                            selected={formData.recipientIds}
                            onChange={(selected) => setFormData(prev => ({ ...prev, recipientIds: selected as number[] }))}
                            placeholder={units.length > 0 ? 'Chọn người nhận' : 'Chưa có đơn vị'}
                            error={errors.recipientIds}
                        />

                        <Input
                            label="Hạn xử lý"
                            type="date"
                            value={formData.dueDate}
                            onChange={(e) => setFormData(prev => ({ ...prev, dueDate: e.target.value }))}
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5">
                            Tệp đính kèm * (.docx, .xlsx, .pdf)
                        </label>
                        <div className="space-y-2">
                            <label className="flex items-center justify-center gap-2 px-4 py-3 border-2 border-dashed border-[var(--color-neutral-300)] rounded-lg cursor-pointer hover:border-[var(--color-primary-500)] hover:bg-[var(--color-primary-50)] transition-colors">
                                <Upload className="w-5 h-5 text-[var(--color-neutral-400)]" />
                                <span className="text-sm text-[var(--color-neutral-600)]">
                                    Chọn tệp để tải lên
                                </span>
                                <input
                                    type="file"
                                    multiple
                                    accept=".docx,.xlsx,.pdf,.doc,.xls"
                                    onChange={handleFileChange}
                                    className="hidden"
                                />
                            </label>

                            {selectedFiles.length > 0 && (
                                <div className="space-y-2">
                                    {selectedFiles.map((file, index) => (
                                        <div
                                            key={index}
                                            className="flex items-center justify-between px-3 py-2 bg-[var(--color-neutral-50)] rounded-lg"
                                        >
                                            <div className="flex items-center gap-2">
                                                <FileText className="w-4 h-4 text-[var(--color-primary-600)]" />
                                                <span className="text-sm text-[var(--color-neutral-700)]">
                                                    {file.name}
                                                </span>
                                                <span className="text-xs text-[var(--color-neutral-400)]">
                                                    ({(file.size / 1024 / 1024).toFixed(2)} MB)
                                                </span>
                                            </div>
                                            <button
                                                type="button"
                                                onClick={() => removeFile(index)}
                                                className="p-1 hover:bg-[var(--color-neutral-200)] rounded transition-colors"
                                            >
                                                <X className="w-4 h-4 text-[var(--color-neutral-500)]" />
                                            </button>
                                        </div>
                                    ))}
                                </div>
                            )}

                            {errors.files && (
                                <p className="text-xs text-[var(--color-danger-600)]">{errors.files}</p>
                            )}
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5">
                            Ghi chú
                        </label>
                        <textarea
                            value={formData.note}
                            onChange={(e) => setFormData(prev => ({ ...prev, note: e.target.value }))}
                            placeholder="Nhập ghi chú (tùy chọn)"
                            rows={3}
                            className="w-full px-3 py-2 text-sm rounded-lg border border-[var(--color-neutral-300)] hover:border-[var(--color-neutral-400)] focus:outline-none focus:ring-2 focus:ring-[var(--color-primary-500)] focus:border-transparent resize-none"
                        />
                    </div>
                </form>
            )}
        </Modal>
    );
}
