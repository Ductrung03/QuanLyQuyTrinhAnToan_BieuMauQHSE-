'use client';

import SubmissionForm from '@/components/business/submissions/SubmissionForm'; import { ArrowLeft } from 'lucide-react';
import Link from 'next/link';

export default function NewSubmissionPage() {
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
                    <h1 className="text-2xl font-bold text-gray-900">Nộp biểu mẫu mới</h1>
                    <p className="text-gray-500">Điền thông tin và đính kèm tài liệu để nộp biểu mẫu</p>
                </div>
            </div>

            <SubmissionForm />
        </div>
    );
}
