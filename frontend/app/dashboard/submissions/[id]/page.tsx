'use client';

import { useState, useEffect } from 'react';
import { apiClient } from '@/lib/api-client';
import { useRouter } from 'next/navigation';
import {
    ArrowLeft, FileText, Clock, CheckCircle,
    XCircle, RotateCcw, File, User
} from 'lucide-react';
import Link from 'next/link';

export default function SubmissionDetailPage({ params }: { params: { id: string } }) {
    const router = useRouter();
    const [submission, setSubmission] = useState<any>(null);
    const [loading, setLoading] = useState(true);

    // Unwrap params using React.use() or await if dealing with Next.js 15 async params
    // But for client components in Next 15, we might need to await params if it's passed as a prop,
    // or use the hook `useParams`. Let's stick to standard `useParams` hook pattern for client components.
    // Wait, the props pattern `params` is for Server Components. For Client Components, use `useParams`.
    // Actually, Next.js 15 App Router passes params to page components (Server by default, but this file has 'use client').
    // In Next.js 15, params is a Promise.

    useEffect(() => {
        // Handling async params in Next.js 15
        const loadData = async () => {
            try {
                // Resolve params if it's a promise (Next.js 15 change)
                const resolvedParams = await Promise.resolve(params);
                if (resolvedParams?.id) {
                    fetchSubmission(Number(resolvedParams.id));
                }
            } catch (error) {
                console.error("Error resolving params", error);
            }
        };
        loadData();
    }, [params]);

    const fetchSubmission = async (id: number) => {
        try {
            setLoading(true);
            const response = await apiClient.getSubmissionById(id);
            if (response.success && response.data) {
                setSubmission(response.data);
            } else {
                alert('Không tìm thấy biểu mẫu');
                router.push('/dashboard/submissions');
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
                    href="/dashboard/submissions"
                    className="p-2 hover:bg-gray-100 rounded-lg text-gray-500 transition-colors"
                >
                    <ArrowLeft className="w-5 h-5" />
                </Link>
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Chi tiết biểu mẫu</h1>
                    <p className="text-gray-500 text-sm">ID: #{submission.id}</p>
                </div>
            </div>

            <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                <div className="lg:col-span-2 space-y-6">
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
                                                href={`${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000'}/${file.filePath}`}
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
                                {submission.recalledAt && (
                                    <div className="flex justify-between items-center text-sm">
                                        <span className="text-orange-600">Ngày thu hồi:</span>
                                        <span className="font-medium text-gray-900">
                                            {new Date(submission.recalledAt).toLocaleString('vi-VN')}
                                        </span>
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>

                    {/* Recipients */}
                    <div className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden">
                        <div className="p-6 border-b border-gray-100 bg-gray-50/50">
                            <h2 className="text-lg font-semibold text-gray-800">Người nhận (CC)</h2>
                        </div>
                        <div className="p-6">
                            {submission.recipients && submission.recipients.length > 0 ? (
                                <div className="space-y-3">
                                    {submission.recipients.map((recipient: any) => (
                                        <div key={recipient.id} className="flex items-center gap-3">
                                            <div className="w-8 h-8 rounded-full bg-gray-100 flex items-center justify-center text-gray-600 font-bold text-xs">
                                                {recipient.recipientUserName?.charAt(0)}
                                            </div>
                                            <p className="text-sm text-gray-700">{recipient.recipientUserName}</p>
                                        </div>
                                    ))}
                                </div>
                            ) : (
                                <p className="text-gray-500 text-sm">Không có người nhận thêm</p>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
