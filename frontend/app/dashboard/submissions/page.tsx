'use client';

import SubmissionList from '@/components/business/submissions/SubmissionList';

export default function SubmissionsPage() {
    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-2xl font-bold text-gray-900">Biểu mẫu đã nộp</h1>
                <p className="text-gray-500">Quản lý và theo dõi trạng thái các biểu mẫu của bạn</p>
            </div>

            <SubmissionList />
        </div>
    );
}
