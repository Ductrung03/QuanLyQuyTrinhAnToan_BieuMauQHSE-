'use client';

import { useState, useEffect, useCallback } from 'react';
import { apiClient, Procedure } from '@/lib/api-client';
import { Plus, FileText, Edit, Trash2 } from 'lucide-react';

export default function ProceduresPage() {
    const [procedures, setProcedures] = useState<Procedure[]>([]);
    const [loading, setLoading] = useState(true);
    const [showCreateModal, setShowCreateModal] = useState(false);
    const [selectedProcedure, setSelectedProcedure] = useState<Procedure | null>(null);

    const loadProcedures = useCallback(async () => {
        setLoading(true);
        const response = await apiClient.getProcedures();
        if (response.success && response.data) {
            setProcedures(response.data);
        }
        setLoading(false);
    }, []);

    useEffect(() => {
        loadProcedures();
    }, [loadProcedures]);



    const handleCreate = () => {
        setSelectedProcedure(null);
        setShowCreateModal(true);
    };

    const handleEdit = (procedure: Procedure) => {
        setSelectedProcedure(procedure);
        setShowCreateModal(true);
    };

    const handleDelete = async (id: number) => {
        if (!confirm('Bạn có chắc muốn xóa quy trình này?')) return;

        const response = await apiClient.deleteProcedure(id);
        if (response.success) {
            alert('Xóa thành công!');
            loadProcedures();
        } else {
            alert(response.message || 'Có lỗi xảy ra');
        }
    };

    const getStateColor = (state: string): string => {
        const colors: Record<string, string> = {
            Draft: 'bg-gray-100 text-gray-800',
            Submitted: 'bg-blue-100 text-blue-800',
            Approved: 'bg-green-100 text-green-800',
            Rejected: 'bg-red-100 text-red-800',
        };
        return colors[state] || 'bg-gray-100 text-gray-800';
    };

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-screen">
                <div className="text-gray-600">Đang tải...</div>
            </div>
        );
    }

    return (
        <div className="space-y-6">
            {/* Header */}
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Quản lý Quy trình</h1>
                    <p className="text-gray-600 mt-1">
                        Quản lý các quy trình vận hành và biểu mẫu QHSE
                    </p>
                </div>
                <button
                    onClick={handleCreate}
                    className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                >
                    <Plus className="w-5 h-5" />
                    <span>Tạo quy trình mới</span>
                </button>
            </div>

            {/* Stats */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
                    <div className="text-sm text-gray-600">Tổng số quy trình</div>
                    <div className="text-2xl font-bold text-gray-900 mt-1">
                        {procedures.length}
                    </div>
                </div>
                <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
                    <div className="text-sm text-gray-600">Đang soạn thảo</div>
                    <div className="text-2xl font-bold text-gray-900 mt-1">
                        {procedures.filter((p) => p.state === 'Draft').length}
                    </div>
                </div>
                <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
                    <div className="text-sm text-gray-600">Đã phê duyệt</div>
                    <div className="text-2xl font-bold text-green-600 mt-1">
                        {procedures.filter((p) => p.state === 'Approved').length}
                    </div>
                </div>
                <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-4">
                    <div className="text-sm text-gray-600">Chờ phê duyệt</div>
                    <div className="text-2xl font-bold text-orange-600 mt-1">
                        {procedures.filter((p) => p.state === 'Submitted').length}
                    </div>
                </div>
            </div>

            {/* Table */}
            <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
                <div className="overflow-x-auto">
                    <table className="w-full">
                        <thead className="bg-gray-50 border-b border-gray-200">
                            <tr>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Mã QT
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Tên quy trình
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Phiên bản
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Trạng thái
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Người chủ trì
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Tài liệu
                                </th>
                                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Biểu mẫu
                                </th>
                                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                                    Thao tác
                                </th>
                            </tr>
                        </thead>
                        <tbody className="bg-white divide-y divide-gray-200">
                            {procedures.map((procedure) => (
                                <tr key={procedure.id} className="hover:bg-gray-50">
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <div className="flex items-center gap-2">
                                            <FileText className="w-4 h-4 text-blue-600" />
                                            <span className="font-medium text-gray-900">
                                                {procedure.code}
                                            </span>
                                        </div>
                                    </td>
                                    <td className="px-6 py-4">
                                        <div className="text-sm text-gray-900">{procedure.name}</div>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <span className="text-sm text-gray-600">
                                            {procedure.version || '-'}
                                        </span>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <span
                                            className={`px-2 py-1 text-xs font-medium rounded-full ${getStateColor(
                                                procedure.state
                                            )}`}
                                        >
                                            {procedure.state}
                                        </span>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <span className="text-sm text-gray-600">
                                            {procedure.ownerUserName || '-'}
                                        </span>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <span className="text-sm text-gray-600">
                                            {procedure.documentCount || 0}
                                        </span>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap">
                                        <span className="text-sm text-gray-600">
                                            {procedure.templateCount || 0}
                                        </span>
                                    </td>
                                    <td className="px-6 py-4 whitespace-nowrap text-right">
                                        <div className="flex items-center justify-end gap-2">
                                            <button
                                                onClick={() => handleEdit(procedure)}
                                                className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                                                title="Sửa"
                                            >
                                                <Edit className="w-4 h-4" />
                                            </button>
                                            <button
                                                onClick={() => handleDelete(procedure.id)}
                                                className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                                                title="Xóa"
                                            >
                                                <Trash2 className="w-4 h-4" />
                                            </button>
                                        </div>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>

                {procedures.length === 0 && (
                    <div className="text-center py-12">
                        <FileText className="w-12 h-12 text-gray-400 mx-auto mb-3" />
                        <p className="text-gray-600">Chưa có quy trình nào</p>
                        <button
                            onClick={handleCreate}
                            className="mt-4 text-blue-600 hover:text-blue-700 font-medium"
                        >
                            Tạo quy trình đầu tiên
                        </button>
                    </div>
                )}
            </div>

            {/* Create/Edit Modal */}
            {showCreateModal && (
                <ProcedureModal
                    procedure={selectedProcedure}
                    onClose={() => setShowCreateModal(false)}
                    onSuccess={() => {
                        setShowCreateModal(false);
                        loadProcedures();
                    }}
                />
            )}
        </div>
    );
}

// Modal Component
function ProcedureModal({
    procedure,
    onClose,
    onSuccess,
}: {
    procedure: Procedure | null;
    onClose: () => void;
    onSuccess: () => void;
}) {
    const [formData, setFormData] = useState({
        code: procedure?.code || '',
        name: procedure?.name || '',
        version: procedure?.version || '1.0',
        description: procedure?.description || '',
    });
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);

        const response = procedure
            ? await apiClient.updateProcedure(procedure.id, formData)
            : await apiClient.createProcedure(formData);

        if (response.success) {
            alert(procedure ? 'Cập nhật thành công!' : 'Tạo mới thành công!');
            onSuccess();
        } else {
            alert(response.message || 'Có lỗi xảy ra');
        }

        setLoading(false);
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-xl shadow-2xl w-full max-w-2xl max-h-[90vh] overflow-y-auto">
                <div className="p-6 border-b border-gray-200">
                    <h2 className="text-xl font-bold text-gray-900">
                        {procedure ? 'Cập nhật quy trình' : 'Tạo quy trình mới'}
                    </h2>
                </div>

                <form onSubmit={handleSubmit} className="p-6 space-y-4">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Mã quy trình *
                        </label>
                        <input
                            type="text"
                            value={formData.code}
                            onChange={(e) => setFormData({ ...formData, code: e.target.value })}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                            placeholder="VD: OPS-01"
                            required
                            disabled={!!procedure}
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Tên quy trình *
                        </label>
                        <input
                            type="text"
                            value={formData.name}
                            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                            placeholder="Nhập tên quy trình"
                            required
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Phiên bản
                        </label>
                        <input
                            type="text"
                            value={formData.version}
                            onChange={(e) => setFormData({ ...formData, version: e.target.value })}
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                            placeholder="VD: 1.0"
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-2">
                            Mô tả
                        </label>
                        <textarea
                            value={formData.description}
                            onChange={(e) =>
                                setFormData({ ...formData, description: e.target.value })
                            }
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                            rows={4}
                            placeholder="Nhập mô tả quy trình"
                        />
                    </div>

                    <div className="flex gap-3 pt-4">
                        <button
                            type="button"
                            onClick={onClose}
                            className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors"
                            disabled={loading}
                        >
                            Hủy
                        </button>
                        <button
                            type="submit"
                            className="flex-1 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
                            disabled={loading}
                        >
                            {loading ? 'Đang xử lý...' : procedure ? 'Cập nhật' : 'Tạo mới'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
