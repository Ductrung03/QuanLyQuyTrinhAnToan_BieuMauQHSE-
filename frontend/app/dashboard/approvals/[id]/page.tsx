'use client';

import { useState, useEffect } from 'react';
import { apiClient } from '@/lib/api-client';
import { useRouter, useParams } from 'next/navigation';
import {
    ArrowLeft, FileText, Clock, CheckCircle,
    XCircle, RotateCcw, File
} from 'lucide-react';
import Link from 'next/link';
import ApprovalAction from '@/components/business/approvals/ApprovalAction';

export default function ApprovalDetailPage() {
    // Handling async params in Next.js 15: use wrapping in useEffect or use useParams()
    // Since this is a client component, useParams is the cleaner hook way
    const params = useParams();
    const router = useRouter();
    const [submission, setSubmission] = useState<any>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (params?.id) {
            fetchSubmission(Number(params.id));
        }
    }, [params]);

    const fetchSubmission = async (id: number) => {
        try {
            setLoading(true);
            const response = await apiClient.getSubmissionById(id);
            if (response.success && response.data) {
                setSubmission(response.data);
            } else {
                alert('Không tìm thấy biểu mẫu');
                router.push('/dashboard/approvals');
            }
        } catch (error) {
            console.error('Error loading submission:', error);
        } finally {
            setLoading(false);
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

    const handleApprovalSuccess = () => {
        router.push('/dashboard/approvals');
        router.refresh();
    };

    if (loading) {
        return (
            <div className="flex items-center justify-center min-h-[400px]">
                <div className="w-8 h-8 border-4 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
            </div>
        );
    }

    if (!submission) return null;

    return (
        <div className="space-y-6">
            <div className="flex items-center gap-4">
                <Link
                    href="/dashboard/approvals"
                    className="p-2 hover:bg-gray-100 rounded-lg text-gray-500 transition-colors"
                >
                    <ArrowLeft className="w-5 h-5" />
                </Link>
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Chi tiết phê duyệt</h1>
                    <p className="text-gray-500 text-sm">ID: #{submission.id}</p>
                </div>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className="lg:col-span-2 space-y-6">
                    {/* Approval Action Section */}
                    {submission.canApprove && (
                        <div className="bg-blue-50/50 rounded-xl shadow-sm border border-blue-100 overflow-hidden">
                            <div className="p-6">
                                <h2 className="text-lg font-bold text-gray-900 mb-2">Hành động phê duyệt</h2>
                                <p className="text-gray-600 mb-4 text-sm">
                                    Vui lòng xem xét kỹ nội dung bên dưới trước khi đưa ra quyết định.
                                </p>
                                <ApprovalAction
                                    submissionId={submission.id}
                                    onSuccess={handleApprovalSuccess}
                                />
                            </div>
                        </div>
                    )}

                    {/* Main Info */}
                    <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
                        <div className="p-6 border-b border-gray-100 bg-gray-50/50 flex justify-between items-center">
                            <h2 className="text-lg font-semibold text-gray-800">Thông tin biểu mẫu</h2>
                            {getStatusBadge(submission.status)}
                        </div>
                        <div className="p-6 space-y-4">
                            <div>
                                <label className="text-sm font-medium text-gray-500">Tiêu đề</label>
                                <p className="text-gray-900 font-medium text-lg mt-1">{submission.title}</p>
                            </div>

                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                <div>
                                    <label className="text-sm font-medium text-gray-500">Quy trình</label>
                                    <p className="text-gray-900 mt-1">{submission.procedureName}</p>
                                </div>
                                <div>
                                    <label className="text-sm font-medium text-gray-500">Mẫu áp dụng</label>
                                    <p className="text-gray-900 mt-1">{submission.templateName || 'Không sử dụng'}</p>
                                </div>
                            </div>

                            <div>
                                <label className="text-sm font-medium text-gray-500">Nội dung / Ghi chú</label>
                                <div className="mt-2 p-4 bg-gray-50 rounded-lg border border-gray-100 text-gray-700 min-h-[100px] whitespace-pre-wrap">
                                    {submission.content || 'Không có nội dung'}
                                </div>
                            </div>
                        </div>
                    </div>

                    {/* Files */}
                    <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
                        <div className="p-6 border-b border-gray-100 bg-gray-50/50">
                            <h2 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
                                <File className="w-5 h-5 text-gray-600" />
                                Tài liệu đính kèm
                            </h2>
                        </div>
                        <div className="p-6">
                            {submission.files && submission.files.length > 0 ? (
                                <div className="space-y-3">
                                    {submission.files.map((file: any) => (
                                        <div key={file.id} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg border border-gray-100 hover:border-blue-200 transition-colors">
                                            <div className="flex items-center gap-3">
                                                <FileText className="w-5 h-5 text-blue-600" />
                                                <div>
                                                    <p className="text-sm font-medium text-gray-900">{file.fileName}</p>
                                                    <p className="text-xs text-gray-500">{(file.fileSize / 1024 / 1024).toFixed(2)} MB</p>
                                                </div>
                                            </div>
                                            <a
                                                href={`${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5265/api'}/${file.filePath}`}
                                                target="_blank"
                                                rel="noopener noreferrer"
                                                className="text-sm font-medium text-blue-600 hover:text-blue-700 hover:underline"
                                            >
                                                Tải về
                                            </a>
                                        </div>
                                    ))}
                                </div>
                            ) : (
                                <p className="text-gray-500 text-center py-4">Không có tài liệu đính kèm</p>
                            )}
                        </div>
                    </div>
                </div>

                <div className="space-y-6">
                    {/* Timeline / Metadata */}
                    <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
                        <div className="p-6 border-b border-gray-100 bg-gray-50/50">
                            <h2 className="text-lg font-semibold text-gray-800">Thông tin nộp</h2>
                        </div>
                        <div className="p-6 space-y-4">
                            <div className="flex items-start gap-3">
                                <div className="w-8 h-8 rounded-full bg-blue-100 flex items-center justify-center text-blue-600 font-bold text-xs mt-0.5">
                                    {submission.submittedByUserName?.charAt(0)}
                                </div>
                                <div>
                                    <p className="text-sm font-medium text-gray-900">{submission.submittedByUserName}</p>
                                    <p className="text-xs text-gray-500">Người nộp</p>
                                </div>
                            </div>

                            <div className="pt-4 border-t border-gray-100 space-y-3">
                                <div className="flex justify-between items-center text-sm">
                                    <span className="text-gray-500">Ngày nộp:</span>
                                    <span className="font-medium text-gray-900">
                                        {new Date(submission.submittedAt).toLocaleString('vi-VN')}
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
