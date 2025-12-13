'use client';

import { useState, useEffect } from 'react';
import { apiClient } from '@/lib/api-client';
import {
    FileText, Clock, CheckCircle, XCircle,
    RotateCcw, Eye, Search, Filter, Plus
} from 'lucide-react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';

export default function SubmissionList() {
    const router = useRouter();
    const [submissions, setSubmissions] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const [statusFilter, setStatusFilter] = useState('All');

    useEffect(() => {
        loadSubmissions();
    }, []);

    const loadSubmissions = async () => {
        try {
            setLoading(true);
            const response = await apiClient.getMySubmissions();
            if (response.success && response.data) {
                setSubmissions(response.data);
            }
        } catch (error) {
            console.error('Error loading submissions:', error);
        } finally {
            setLoading(false);
        }
    };

    const handleRecall = async (id: number) => {
        const reason = prompt('Vui lòng nhập lý do thu hồi biểu mẫu:');
        if (!reason) return;

        try {
            const response = await apiClient.recallSubmission(id, reason);
            if (response.success) {
                alert('Thu hồi biểu mẫu thành công');
                loadSubmissions(); // Reload list
            } else {
                alert(response.message || 'Không thể thu hồi biểu mẫu');
            }
        } catch (error) {
            console.error('Error recalling submission:', error);
            alert('Có lỗi xảy ra khi thu hồi biểu mẫu');
        }
    };

    const getStatusBadge = (status: string) => {
        const styles: any = {
            Submitted: 'bg-blue-100 text-blue-800 border-blue-200',
            Approved: 'bg-green-100 text-green-800 border-green-200',
            Rejected: 'bg-red-100 text-red-800 border-red-200',
            Recalled: 'bg-orange-100 text-orange-800 border-orange-200',
        };

        const icons: any = {
            Submitted: Clock,
            Approved: CheckCircle,
            Rejected: XCircle,
            Recalled: RotateCcw,
        };

        const style = styles[status] || 'bg-gray-100 text-gray-800 border-gray-200';
        const Icon = icons[status] || FileText;

        return (
            <span className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-medium border ${style}`}>
                <Icon className="w-3.5 h-3.5" />
                {status}
            </span>
        );
    };

    const filteredSubmissions = submissions.filter(sub => {
        const matchesSearch = sub.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
            sub.procedureName.toLowerCase().includes(searchTerm.toLowerCase());
        const matchesStatus = statusFilter === 'All' || sub.status === statusFilter;
        return matchesSearch && matchesStatus;
    });

    if (loading) {
        return (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {[1, 2, 3, 4, 5, 6].map((i) => (
                    <div key={i} className="bg-white rounded-xl shadow-sm border p-6 animate-pulse">
                        <div className="h-4 bg-gray-200 rounded w-1/4 mb-4"></div>
                        <div className="h-6 bg-gray-200 rounded w-3/4 mb-4"></div>
                        <div className="h-4 bg-gray-200 rounded w-1/2"></div>
                    </div>
                ))}
            </div>
        );
    }

    return (
        <div className="space-y-6">
            <div className="flex flex-col md:flex-row gap-4 justify-between items-center">
                <div className="relative w-full md:w-96">
                    <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                    <input
                        type="text"
                        placeholder="Tìm kiếm biểu mẫu..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="w-full pl-10 pr-4 py-2 rounded-lg border border-gray-200 focus:border-blue-500 focus:ring-2 focus:ring-blue-100 outline-none"
                    />
                </div>

                <div className="flex items-center gap-3 w-full md:w-auto">
                    <div className="flex items-center gap-2 px-3 py-2 bg-white border border-gray-200 rounded-lg">
                        <Filter className="w-4 h-4 text-gray-500" />
                        <select
                            value={statusFilter}
                            onChange={(e) => setStatusFilter(e.target.value)}
                            className="border-none bg-transparent text-sm focus:ring-0 cursor-pointer"
                        >
                            <option value="All">Tất cả trạng thái</option>
                            <option value="Submitted">Đã nộp</option>
                            <option value="Approved">Đã duyệt</option>
                            <option value="Rejected">Từ chối</option>
                            <option value="Recalled">Đã thu hồi</option>
                        </select>
                    </div>

                    <Link
                        href="/dashboard/submissions/new"
                        className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors whitespace-nowrap"
                    >
                        <Plus className="w-4 h-4" />
                        <span>Nộp mới</span>
                    </Link>
                </div>
            </div>

            {filteredSubmissions.length === 0 ? (
                <div className="text-center py-12 bg-white rounded-xl border border-dashed border-gray-300">
                    <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mx-auto mb-4">
                        <FileText className="w-8 h-8 text-gray-400" />
                    </div>
                    <h3 className="text-lg font-medium text-gray-900 mb-1">Chưa có biểu mẫu nào</h3>
                    <p className="text-gray-500 mb-4">Bạn chưa nộp biểu mẫu nào hoặc không tìm thấy kết quả phù hợp</p>
                    <Link
                        href="/dashboard/submissions/new"
                        className="inline-flex items-center gap-2 text-blue-600 hover:text-blue-700 font-medium"
                    >
                        Nộp biểu mẫu ngay
                    </Link>
                </div>
            ) : (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                    {filteredSubmissions.map((submission) => (
                        <div
                            key={submission.id}
                            className="group bg-white rounded-xl shadow-sm border border-gray-200 hover:shadow-md hover:border-blue-200 transition-all duration-200 overflow-hidden flex flex-col"
                        >
                            <div className="p-5 flex-1">
                                <div className="flex items-start justify-between mb-4">
                                    <div className="w-10 h-10 rounded-lg bg-blue-50 flex items-center justify-center text-blue-600 font-bold text-sm">
                                        {submission.procedureCode || 'F'}
                                    </div>
                                    {getStatusBadge(submission.status)}
                                </div>

                                <h3 className="text-base font-semibold text-gray-900 mb-2 line-clamp-2 group-hover:text-blue-600 transition-colors">
                                    {submission.title}
                                </h3>

                                <div className="space-y-2 text-sm text-gray-500 mb-4">
                                    <div className="flex items-center gap-2">
                                        <FileText className="w-4 h-4" />
                                        <span className="truncate">{submission.procedureName}</span>
                                    </div>
                                    <div className="flex items-center gap-2">
                                        <Clock className="w-4 h-4" />
                                        <span>{new Date(submission.submittedAt).toLocaleString('vi-VN')}</span>
                                    </div>
                                </div>

                                {submission.recallReason && (
                                    <div className="bg-orange-50 p-3 rounded-lg text-xs text-orange-800 mb-2">
                                        <strong>Lý do thu hồi:</strong> {submission.recallReason}
                                    </div>
                                )}
                            </div>

                            <div className="px-5 py-4 border-t border-gray-100 bg-gray-50/50 flex items-center justify-between">
                                <button
                                    onClick={() => router.push(`/dashboard/submissions/${submission.id}`)}
                                    className="text-sm font-medium text-gray-600 hover:text-blue-600 transition-colors flex items-center gap-1"
                                >
                                    <Eye className="w-4 h-4" />
                                    Chi tiết
                                </button>

                                {submission.canRecall && (
                                    <button
                                        onClick={() => handleRecall(submission.id)}
                                        className="text-sm font-medium text-orange-600 hover:text-orange-700 transition-colors flex items-center gap-1"
                                    >
                                        <RotateCcw className="w-4 h-4" />
                                        Thu hồi
                                    </button>
                                )}
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}
