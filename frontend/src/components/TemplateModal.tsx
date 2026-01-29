import { useEffect, useState } from 'react';
import { Modal, Button, Input } from '@/components/ui';

export interface ProcedureOption {
    id: number;
    code: string;
    name: string;
}

interface TemplateModalProps {
    isOpen: boolean;
    onClose: () => void;
    onSave: (data: FormData) => Promise<void>;
    procedureOptions: ProcedureOption[];
    defaultProcedureId?: number | null;
}

export default function TemplateModal({
    isOpen,
    onClose,
    onSave,
    procedureOptions,
    defaultProcedureId,
}: TemplateModalProps) {
    const [formState, setFormState] = useState({
        procedureId: defaultProcedureId ? String(defaultProcedureId) : '',
        name: '',
        templateType: 'Form',
        templateNo: '',
        templateKey: '',
    });
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [errors, setErrors] = useState<{ procedureId?: string; name?: string }>({});

    useEffect(() => {
        if (isOpen) {
            setFormState({
                procedureId: defaultProcedureId ? String(defaultProcedureId) : '',
                name: '',
                templateType: 'Form',
                templateNo: '',
                templateKey: '',
            });
            setSelectedFile(null);
            setErrors({});
        }
    }, [isOpen, defaultProcedureId]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        const nextErrors: { procedureId?: string; name?: string } = {};
        if (!formState.procedureId) nextErrors.procedureId = 'Vui lòng chọn quy trình';
        if (!formState.name.trim()) nextErrors.name = 'Vui lòng nhập tên biểu mẫu';
        setErrors(nextErrors);

        if (Object.keys(nextErrors).length > 0) return;

        const payload = new FormData();
        payload.append('ProcedureId', formState.procedureId);
        payload.append('Name', formState.name.trim());
        payload.append('TemplateType', formState.templateType);
        if (formState.templateNo.trim()) payload.append('TemplateNo', formState.templateNo.trim());
        if (formState.templateKey.trim()) payload.append('TemplateKey', formState.templateKey.trim());
        if (selectedFile) payload.append('file', selectedFile);

        await onSave(payload);
    };

    return (
        <Modal
            isOpen={isOpen}
            onClose={onClose}
            title="Thêm biểu mẫu"
            size="md"
            footer={
                <>
                    <Button variant="secondary" onClick={onClose}>
                        Hủy
                    </Button>
                    <Button onClick={handleSubmit}>Tạo biểu mẫu</Button>
                </>
            }
        >
            <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                    <label className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5">
                        Quy trình *
                    </label>
                    <select
                        value={formState.procedureId}
                        onChange={(e) => setFormState((prev) => ({ ...prev, procedureId: e.target.value }))}
                        className="w-full px-3 py-2 text-sm rounded-lg border border-[var(--color-neutral-300)] bg-white"
                    >
                        <option value="">Chọn quy trình</option>
                        {procedureOptions.map((proc) => (
                            <option key={proc.id} value={proc.id}>
                                {proc.code} - {proc.name}
                            </option>
                        ))}
                    </select>
                    {errors.procedureId && (
                        <p className="mt-1.5 text-xs text-[var(--color-danger-600)]">{errors.procedureId}</p>
                    )}
                </div>

                <Input
                    label="Tên biểu mẫu *"
                    placeholder="Nhập tên biểu mẫu"
                    value={formState.name}
                    onChange={(e) => setFormState((prev) => ({ ...prev, name: e.target.value }))}
                    error={errors.name}
                    required
                />

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <Input
                        label="Số hiệu"
                        placeholder="FM-OPS-01-01"
                        value={formState.templateNo}
                        onChange={(e) => setFormState((prev) => ({ ...prev, templateNo: e.target.value }))}
                    />
                    <div>
                        <label className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5">
                            Loại biểu mẫu
                        </label>
                        <select
                            value={formState.templateType}
                            onChange={(e) => setFormState((prev) => ({ ...prev, templateType: e.target.value }))}
                            className="w-full px-3 py-2 text-sm rounded-lg border border-[var(--color-neutral-300)] bg-white"
                        >
                            <option value="Form">Form</option>
                            <option value="Checklist">Checklist</option>
                            <option value="Report">Report</option>
                            <option value="Other">Other</option>
                        </select>
                    </div>
                </div>

                <Input
                    label="Template Key"
                    placeholder="T1"
                    value={formState.templateKey}
                    onChange={(e) => setFormState((prev) => ({ ...prev, templateKey: e.target.value }))}
                />

                <div>
                    <label className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5">
                        File mẫu (tùy chọn)
                    </label>
                    <input
                        type="file"
                        accept=".doc,.docx,.xls,.xlsx,.pdf"
                        onChange={(e) => setSelectedFile(e.target.files?.[0] ?? null)}
                        className="w-full text-sm"
                    />
                </div>
            </form>
        </Modal>
    );
}
