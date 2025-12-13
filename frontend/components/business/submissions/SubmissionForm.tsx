'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { apiClient } from '@/lib/api-client';
import { Upload, X, FileText, Send, UserPlus, File, CheckCircle } from 'lucide-react';

export default function SubmissionForm() {
    const router = useRouter();
    const [loading, setLoading] = useState(false);
    const [procedures, setProcedures] = useState<any[]>([]);
    const [templates, setTemplates] = useState<any[]>([]);
    const [users, setUsers] = useState<any[]>([]);

    const [formData, setFormData] = useState({
        procedureId: 0,
        templateId: 0,
        title: '',
        content: '',
        recipientUserIds: [] as number[],
    });

    const [files, setFiles] = useState<File[]>([]);

    useEffect(() => {
        loadInitialData();
    }, []);

    const loadInitialData = async () => {
        try {
            const [proceduresRes, usersRes] = await Promise.all([
                apiClient.getProcedures(),
                apiClient.getAvailableUsers()
            ]);

            if (proceduresRes.success && proceduresRes.data) {
                setProcedures(proceduresRes.data);
            }

            if (usersRes.success && usersRes.data) {
                setUsers(usersRes.data);
            }
        } catch (error) {
            console.error('Error loading initial data:', error);
        }
    };

    const handleProcedureChange = async (procedureId: number) => {
        setFormData(prev => ({ ...prev, procedureId, templateId: 0 }));
        if (procedureId) {
            const response = await apiClient.getTemplatesByProcedure(procedureId);
            if (response.success && response.data) {
                setTemplates(response.data);
            }
        } else {
            setTemplates([]);
        }
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        if (e.target.files) {
            const newFiles = Array.from(e.target.files);
            setFiles(prev => [...prev, ...newFiles]);
        }
    };

    const removeFile = (index: number) => {
        setFiles(prev => prev.filter((_, i) => i !== index));
    };

    const toggleRecipient = (userId: number) => {
        setFormData(prev => {
            const current = prev.recipientUserIds;
            if (current.includes(userId)) {
                return { ...prev, recipientUserIds: current.filter(id => id !== userId) };
            } else {
                return { ...prev, recipientUserIds: [...current, userId] };
            }
        });
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);

        try {
            const response = await apiClient.createSubmission(formData, files);

            if (response.success) {
                alert('Nộp biểu mẫu thành công!');
                router.push('/dashboard/submissions');
            } else {
                alert(response.message || 'Có lỗi xảy ra khi nộp biểu mẫu');
            }
        } catch (error) {
            console.error('Error submitting form:', error);
            alert('Có lỗi xảy ra khi nộp biểu mẫu');
        } finally {
            setLoading(false);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="space-y-8 animate-in fade-in duration-500">
            <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
                <div className="p-6 border-b border-gray-100 bg-gray-50/50">
                    <h2 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
                        <FileText className="w-5 h-5 text-blue-600" />
                        Thông tin chung
                    </h2>
                </div>

                <div className="p-6 grid grid-cols-1 md:grid-cols-2 gap-6">
                    <div className="space-y-2">
                        <label className="text-sm font-medium text-gray-700">Quy trình <span className="text-red-500">*</span></label>
                        <select
                            required
                            value={formData.procedureId}
                            onChange={(e) => handleProcedureChange(Number(e.target.value))}
                            className="w-full px-4 py-2.5 rounded-lg border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none transition-all bg-white"
                        >
                            <option value="">-- Chọn quy trình --</option>
                            {procedures.map((p) => (
                                <option key={p.id} value={p.id}>
                                    {p.code} - {p.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="space-y-2">
                        <label className="text-sm font-medium text-gray-700">Biểu mẫu đính kèm (Template)</label>
                        <select
                            value={formData.templateId}
                            onChange={(e) => setFormData({ ...formData, templateId: Number(e.target.value) })}
                            className="w-full px-4 py-2.5 rounded-lg border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none transition-all bg-white disabled:bg-gray-50 disabled:text-gray-400"
                            disabled={!formData.procedureId || templates.length === 0}
                        >
                            <option value="0">-- Không sử dụng template --</option>
                            {templates.map((t) => (
                                <option key={t.id} value={t.id}>
                                    {t.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="md:col-span-2 space-y-2">
                        <label className="text-sm font-medium text-gray-700">Tiêu đề <span className="text-red-500">*</span></label>
                        <input
                            required
                            type="text"
                            value={formData.title}
                            onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                            placeholder="Nhập tiêu đề cho biểu mẫu này..."
                            className="w-full px-4 py-2.5 rounded-lg border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none transition-all"
                        />
                    </div>

                    <div className="md:col-span-2 space-y-2">
                        <label className="text-sm font-medium text-gray-700">Nội dung / Ghi chú</label>
                        <textarea
                            rows={4}
                            value={formData.content}
                            onChange={(e) => setFormData({ ...formData, content: e.target.value })}
                            placeholder="Nhập nội dung chi tiết..."
                            className="w-full px-4 py-2.5 rounded-lg border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-200 outline-none transition-all resize-none"
                        />
                    </div>
                </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
                {/* Recipient Selection */}
                <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
                    <div className="p-6 border-b border-gray-100 bg-gray-50/50">
                        <h2 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
                            <UserPlus className="w-5 h-5 text-blue-600" />
                            Người nhận (CC)
                        </h2>
                    </div>
                    <div className="p-6">
                        <div className="max-h-60 overflow-y-auto space-y-2 pr-2 custom-scrollbar">
                            {users.map((user) => (
                                <div
                                    key={user.id}
                                    onClick={() => toggleRecipient(user.id)}
                                    className={`flex items-center justify-between p-3 rounded-lg border cursor-pointer transition-all ${formData.recipientUserIds.includes(user.id)
                                            ? 'border-blue-500 bg-blue-50'
                                            : 'border-gray-200 hover:border-blue-300'
                                        }`}
                                >
                                    <div className="flex items-center gap-3">
                                        <div className={`w-8 h-8 rounded-full flex items-center justify-center text-sm font-medium ${formData.recipientUserIds.includes(user.id) ? 'bg-blue-200 text-blue-700' : 'bg-gray-100 text-gray-600'
                                            }`}>
                                            {user.fullName.charAt(0)}
                                        </div>
                                        <div>
                                            <p className="text-sm font-medium text-gray-900">{user.fullName}</p>
                                            <p className="text-xs text-gray-500">{user.email}</p>
                                        </div>
                                    </div>
                                    {formData.recipientUserIds.includes(user.id) && (
                                        <CheckCircle className="w-5 h-5 text-blue-600" />
                                    )}
                                </div>
                            ))}
                            {users.length === 0 && (
                                <p className="text-gray-500 text-center py-4">Không có người dùng nào để chọn</p>
                            )}
                        </div>
                    </div>
                </div>

                {/* File Upload */}
                <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
                    <div className="p-6 border-b border-gray-100 bg-gray-50/50">
                        <h2 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
                            <Upload className="w-5 h-5 text-blue-600" />
                            Tài liệu đính kèm
                        </h2>
                    </div>
                    <div className="p-6 space-y-4">
                        <div className="relative">
                            <input
                                type="file"
                                multiple
                                onChange={handleFileChange}
                                className="absolute inset-0 w-full h-full opacity-0 cursor-pointer"
                            />
                            <div className="border-2 border-dashed border-gray-300 rounded-xl p-8 text-center hover:border-blue-500 hover:bg-blue-50 transition-all cursor-pointer">
                                <div className="w-12 h-12 bg-blue-100 rounded-full flex items-center justify-center mx-auto mb-3">
                                    <Upload className="w-6 h-6 text-blue-600" />
                                </div>
                                <p className="text-sm font-medium text-gray-900">Click hoặc kéo thả file vào đây</p>
                                <p className="text-xs text-gray-500 mt-1">Hỗ trợ nhiều định dạng file</p>
                            </div>
                        </div>

                        {files.length > 0 && (
                            <div className="space-y-2 max-h-48 overflow-y-auto custom-scrollbar">
                                {files.map((file, index) => (
                                    <div key={index} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg group">
                                        <div className="flex items-center gap-3 overflow-hidden">
                                            <File className="w-5 h-5 text-gray-400 flex-shrink-0" />
                                            <div className="min-w-0">
                                                <p className="text-sm font-medium text-gray-700 truncate">{file.name}</p>
                                                <p className="text-xs text-gray-500">{(file.size / 1024 / 1024).toFixed(2)} MB</p>
                                            </div>
                                        </div>
                                        <button
                                            type="button"
                                            onClick={() => removeFile(index)}
                                            className="p-1 hover:bg-red-100 rounded text-gray-400 hover:text-red-500 transition-colors"
                                        >
                                            <X className="w-4 h-4" />
                                        </button>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </div>
            </div>

            <div className="flex justify-end gap-4 pt-4">
                <button
                    type="button"
                    onClick={() => router.back()}
                    className="px-6 py-2.5 rounded-lg border border-gray-300 text-gray-700 font-medium hover:bg-gray-50 transition-colors"
                >
                    Hủy bỏ
                </button>
                <button
                    type="submit"
                    disabled={loading}
                    className="px-6 py-2.5 rounded-lg bg-blue-600 text-white font-medium hover:bg-blue-700 focus:ring-4 focus:ring-blue-100 transition-all flex items-center gap-2 disabled:opacity-70 disabled:cursor-not-allowed"
                >
                    {loading ? (
                        <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
                    ) : (
                        <Send className="w-4 h-4" />
                    )}
                    {loading ? 'Đang xử lý...' : 'Nộp biểu mẫu'}
                </button>
            </div>
        </form>
    );
}
