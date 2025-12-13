"use client";

import { useEffect, useState } from "react";
import { apiClient } from "@/lib/api-client";
import { FileText, Calendar, User, CheckCircle2 } from "lucide-react";
import Link from "next/link";
import { format } from "date-fns";
import { vi } from "date-fns/locale";
import { useToast } from "@/components/ui/Toast";
import { SkeletonCard } from "@/components/ui/Skeleton";

export default function ApprovalsPage() {
    const [submissions, setSubmissions] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const toast = useToast();

    useEffect(() => {
        fetchApprovals();
    }, []);

    const fetchApprovals = async () => {
        try {
            setLoading(true);
            const res = await apiClient.getPendingApprovals();
            if (res.success && res.data) {
                setSubmissions(res.data);
            } else {
                toast.error(res.message || "Không thể tải danh sách phê duyệt");
            }
        } catch (error) {
            console.error(error);
            toast.error("Lỗi kết nối server");
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return (
            <div className="space-y-6">
                <div>
                    <h1 className="text-2xl font-bold tracking-tight">Cần phê duyệt</h1>
                    <p className="text-gray-500 mt-1">Đang tải danh sách...</p>
                </div>
                <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                    {[1, 2, 3].map((i) => (
                        <SkeletonCard key={i} />
                    ))}
                </div>
            </div>
        );
    }

    return (
        <div className="space-y-6">
            <div className="flex justify-between items-center">
                <div>
                    <h1 className="text-2xl font-bold tracking-tight">Cần phê duyệt</h1>
                    <p className="text-muted-foreground mt-1">
                        Danh sách các biểu mẫu đang chờ bạn phê duyệt
                    </p>
                </div>
            </div>

            {submissions.length === 0 ? (
                <div className="text-center py-12 bg-gray-50 rounded-xl border border-dashed">
                    <CheckCircle2 className="w-12 h-12 text-green-500 mx-auto mb-4" />
                    <h3 className="text-lg font-medium">Bạn đã hoàn thành hết công việc!</h3>
                    <p className="text-gray-500 mt-1">Không có biểu mẫu nào cần phê duyệt lúc này.</p>
                </div>
            ) : (
                <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                    {submissions.map((item) => (
                        <div
                            key={item.id}
                            className="group bg-white rounded-xl border shadow-sm hover:shadow-md transition-all p-5 flex flex-col"
                        >
                            <div className="flex justify-between items-start mb-3">
                                <div className="p-2 bg-orange-50 text-orange-600 rounded-lg">
                                    <FileText className="w-5 h-5" />
                                </div>
                                <span className="px-2 py-1 text-xs font-medium rounded-full bg-yellow-100 text-yellow-800 border border-yellow-200">
                                    Chờ duyệt
                                </span>
                            </div>

                            <h3 className="font-semibold text-gray-900 mb-1 line-clamp-1 group-hover:text-blue-600 transition-colors">
                                {item.title}
                            </h3>
                            <p className="text-sm text-gray-500 mb-4 line-clamp-2">
                                {item.procedureName}
                            </p>

                            <div className="mt-auto space-y-3 pt-4 border-t">
                                <div className="flex items-center text-sm text-gray-600">
                                    <User className="w-4 h-4 mr-2 text-gray-400" />
                                    <span className="truncate">{item.submittedByUserName}</span>
                                </div>
                                <div className="flex items-center text-sm text-gray-600">
                                    <Calendar className="w-4 h-4 mr-2 text-gray-400" />
                                    <span>
                                        {format(new Date(item.submittedAt), "dd/MM/yyyy HH:mm", { locale: vi })}
                                    </span>
                                </div>

                                <Link
                                    href={`/dashboard/approvals/${item.id}`}
                                    className="block w-full text-center px-4 py-2 mt-2 bg-blue-50 text-blue-600 rounded-lg hover:bg-blue-100 transition font-medium"
                                >
                                    Xem chi tiết & Duyệt
                                </Link>
                            </div>
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}
