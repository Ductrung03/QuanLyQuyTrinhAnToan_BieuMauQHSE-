'use client';

import { useState, useEffect, useMemo } from 'react';
import { FileText, Upload, CheckCircle, Clock, Users, Building, Activity } from 'lucide-react';
import { apiClient, DashboardStats } from '@/lib/api-client';
import { format } from 'date-fns';
import { vi } from 'date-fns/locale';
import Link from 'next/link';

export default function DashboardPage() {
    const [stats, setStats] = useState<DashboardStats | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const loadStats = async () => {
            try {
                setLoading(true);
                const response = await apiClient.getDashboardStats();
                if (response.success && response.data) {
                    setStats(response.data);
                } else {
                    setError(response.message || 'Không thể tải dữ liệu');
                }
            } catch (err) {
                console.error('Error loading dashboard stats:', err);
                setError('Lỗi kết nối server');
            } finally {
                setLoading(false);
            }
        };

        loadStats();
    }, []);

    const statCards = useMemo(() => {
        if (!stats) return [];
        return [
            {
                title: 'Tổng số Quy trình',
                value: stats.totalProcedures,
                subtitle: 'đang quản lý',
                icon: FileText,
                color: 'bg-blue-500',
                bgColor: 'bg-blue-50',
                textColor: 'text-blue-600',
                href: '/dashboard/procedures'
            },
            {
                title: 'Tổng số Biểu mẫu',
                value: stats.totalTemplates,
                subtitle: 'form & checklist',
                icon: Upload,
                color: 'bg-green-500',
                bgColor: 'bg-green-50',
                textColor: 'text-green-600',
                href: '/dashboard/submissions'
            },
            {
                title: 'Chờ phê duyệt',
                value: stats.pendingApprovals,
                subtitle: 'mục cần xử lý',
                icon: Clock,
                color: 'bg-orange-500',
                bgColor: 'bg-orange-50',
                textColor: 'text-orange-600',
                href: '/dashboard/approvals'
            },
            {
                title: 'Đã phê duyệt',
                value: stats.approvedSubmissions,
                subtitle: 'biểu mẫu hoàn thành',
                icon: CheckCircle,
                color: 'bg-emerald-500',
                bgColor: 'bg-emerald-50',
                textColor: 'text-emerald-600',
                href: '/dashboard/submissions'
            },
        ];
    }, [stats]);

    const getActionColor = (action: string): string => {
        const colors: Record<string, string> = {
            Create: 'text-green-600',
            Edit: 'text-blue-600',
            Delete: 'text-red-600',
            Submit: 'text-indigo-600',
            Approve: 'text-emerald-600',
            Reject: 'text-orange-600',
            Recall: 'text-yellow-600',
        };
        return colors[action] || 'text-gray-600';
    };

    if (loading) {
        return (
            <div className="space-y-6">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Tổng quan</h1>
                    <p className="text-gray-600 mt-1">Đang tải dữ liệu...</p>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                    {[1, 2, 3, 4].map((i) => (
                        <div key={i} className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 animate-pulse">
                            <div className="h-4 bg-gray-200 rounded w-1/2 mb-3"></div>
                            <div className="h-8 bg-gray-200 rounded w-1/3 mb-2"></div>
                            <div className="h-3 bg-gray-200 rounded w-1/4"></div>
                        </div>
                    ))}
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="space-y-6">
                <div>
                    <h1 className="text-2xl font-bold text-gray-900">Tổng quan</h1>
                    <p className="text-red-600 mt-1">{error}</p>
                </div>
                <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-8 text-center">
                    <Activity className="w-12 h-12 text-gray-400 mx-auto mb-4" />
                    <p className="text-gray-600 mb-4">
                        Không thể tải dữ liệu từ server. Vui lòng kiểm tra kết nối và thử lại.
                    </p>
                    <button
                        onClick={() => {
                            setError(null);
                            setLoading(true);
                            apiClient.getDashboardStats().then(response => {
                                if (response.success && response.data) {
                                    setStats(response.data);
                                } else {
                                    setError(response.message || 'Không thể tải dữ liệu');
                                }
                                setLoading(false);
                            });
                        }}
                        className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                    >
                        Thử lại
                    </button>
                </div>

                {/* Show quick links even when error */}
                <div className="bg-gradient-to-r from-blue-600 to-indigo-700 rounded-xl shadow-lg p-8 text-white">
                    <h2 className="text-2xl font-bold mb-2">
                        Hệ thống Quản lý Quy trình An toàn & Biểu mẫu QHSE
                    </h2>
                    <p className="text-blue-100 mb-6">
                        Ship Safety Management System - QHSE Process & Forms Management (v3.4.2)
                    </p>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <Link href="/dashboard/procedures" className="bg-white/10 backdrop-blur-sm rounded-lg p-4 hover:bg-white/20 transition-all">
                            <h3 className="font-semibold mb-1">Quản lý Quy trình</h3>
                            <p className="text-sm text-blue-100">Tạo và quản lý các quy trình QHSE</p>
                        </Link>
                        <Link href="/dashboard/submissions" className="bg-white/10 backdrop-blur-sm rounded-lg p-4 hover:bg-white/20 transition-all">
                            <h3 className="font-semibold mb-1">Nộp Biểu mẫu</h3>
                            <p className="text-sm text-blue-100">Nộp và theo dõi biểu mẫu an toàn</p>
                        </Link>
                        <Link href="/dashboard/approvals" className="bg-white/10 backdrop-blur-sm rounded-lg p-4 hover:bg-white/20 transition-all">
                            <h3 className="font-semibold mb-1">Phê duyệt</h3>
                            <p className="text-sm text-blue-100">Xem xét và phê duyệt biểu mẫu</p>
                        </Link>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="space-y-6">
            <div>
                <h1 className="text-2xl font-bold text-gray-900">Tổng quan</h1>
                <p className="text-gray-600 mt-1">
                    Chào mừng bạn đến với Hệ thống Quản lý QHSE
                </p>
            </div>

            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                {statCards.map((card) => {
                    const Icon = card.icon;
                    return (
                        <Link
                            key={card.title}
                            href={card.href}
                            className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md hover:border-blue-200 transition-all"
                        >
                            <div className="flex items-center justify-between">
                                <div>
                                    <p className="text-sm text-gray-600 mb-1">{card.title}</p>
                                    <p className="text-3xl font-bold text-gray-900">
                                        {card.value}
                                    </p>
                                    <p className="text-xs text-gray-500 mt-1">{card.subtitle}</p>
                                </div>
                                <div className={`${card.bgColor} p-3 rounded-lg`}>
                                    <Icon className={`w-6 h-6 ${card.textColor}`} />
                                </div>
                            </div>
                        </Link>
                    );
                })}
            </div>

            {/* Additional Stats Row */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                    <div className="flex items-center gap-3 mb-3">
                        <div className="p-2 bg-purple-50 rounded-lg">
                            <Users className="w-5 h-5 text-purple-600" />
                        </div>
                        <span className="text-sm font-medium text-gray-600">Người dùng</span>
                    </div>
                    <p className="text-2xl font-bold text-gray-900">{stats?.totalUsers || 0}</p>
                </div>
                <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                    <div className="flex items-center gap-3 mb-3">
                        <div className="p-2 bg-indigo-50 rounded-lg">
                            <Building className="w-5 h-5 text-indigo-600" />
                        </div>
                        <span className="text-sm font-medium text-gray-600">Đơn vị</span>
                    </div>
                    <p className="text-2xl font-bold text-gray-900">{stats?.totalUnits || 0}</p>
                </div>
                <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                    <div className="flex items-center gap-3 mb-3">
                        <div className="p-2 bg-red-50 rounded-lg">
                            <FileText className="w-5 h-5 text-red-600" />
                        </div>
                        <span className="text-sm font-medium text-gray-600">Từ chối</span>
                    </div>
                    <p className="text-2xl font-bold text-gray-900">{stats?.rejectedSubmissions || 0}</p>
                </div>
            </div>

            {/* Welcome Card */}
            <div className="bg-gradient-to-r from-blue-600 to-indigo-700 rounded-xl shadow-lg p-8 text-white">
                <h2 className="text-2xl font-bold mb-2">
                    Hệ thống Quản lý Quy trình An toàn & Biểu mẫu QHSE
                </h2>
                <p className="text-blue-100 mb-6">
                    Ship Safety Management System - QHSE Process & Forms Management (v3.4.2)
                </p>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <Link href="/dashboard/procedures" className="bg-white/10 backdrop-blur-sm rounded-lg p-4 hover:bg-white/20 transition-all">
                        <h3 className="font-semibold mb-1">Quản lý Quy trình</h3>
                        <p className="text-sm text-blue-100">
                            Tạo và quản lý các quy trình QHSE
                        </p>
                    </Link>
                    <Link href="/dashboard/submissions" className="bg-white/10 backdrop-blur-sm rounded-lg p-4 hover:bg-white/20 transition-all">
                        <h3 className="font-semibold mb-1">Nộp Biểu mẫu</h3>
                        <p className="text-sm text-blue-100">
                            Nộp và theo dõi biểu mẫu an toàn
                        </p>
                    </Link>
                    <Link href="/dashboard/approvals" className="bg-white/10 backdrop-blur-sm rounded-lg p-4 hover:bg-white/20 transition-all">
                        <h3 className="font-semibold mb-1">Phê duyệt</h3>
                        <p className="text-sm text-blue-100">
                            Xem xét và phê duyệt biểu mẫu
                        </p>
                    </Link>
                </div>
            </div>

            {/* Recent Activities */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <div className="flex items-center justify-between mb-4">
                    <div className="flex items-center gap-3">
                        <Activity className="w-5 h-5 text-gray-600" />
                        <h2 className="text-lg font-semibold text-gray-900">
                            Hoạt động gần đây
                        </h2>
                    </div>
                    <Link href="/dashboard/audit" className="text-sm text-blue-600 hover:text-blue-700">
                        Xem tất cả →
                    </Link>
                </div>

                {stats?.recentActivities && stats.recentActivities.length > 0 ? (
                    <div className="overflow-x-auto">
                        <table className="w-full">
                            <thead className="border-b border-gray-200">
                                <tr>
                                    <th className="text-left py-3 text-xs font-medium text-gray-500 uppercase">Thời gian</th>
                                    <th className="text-left py-3 text-xs font-medium text-gray-500 uppercase">Người dùng</th>
                                    <th className="text-left py-3 text-xs font-medium text-gray-500 uppercase">Hành động</th>
                                    <th className="text-left py-3 text-xs font-medium text-gray-500 uppercase">Đối tượng</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-gray-100">
                                {stats.recentActivities.slice(0, 6).map((activity, index) => (
                                    <tr key={index} className="hover:bg-gray-50">
                                        <td className="py-3 text-sm text-gray-600">
                                            {format(new Date(activity.time), 'dd/MM/yyyy HH:mm', { locale: vi })}
                                        </td>
                                        <td className="py-3 text-sm text-gray-900">{activity.userName}</td>
                                        <td className="py-3">
                                            <span className={`text-sm font-medium ${getActionColor(activity.action)}`}>
                                                {activity.action}
                                            </span>
                                        </td>
                                        <td className="py-3 text-sm text-gray-600">{activity.target}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                ) : (
                    <div className="text-center py-8 text-gray-500">
                        <Activity className="w-12 h-12 mx-auto mb-3 opacity-50" />
                        <p>Chưa có hoạt động nào được ghi nhận</p>
                    </div>
                )}
            </div>

            {/* Quick Actions */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">
                    Thao tác nhanh
                </h2>
                <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                    <Link
                        href="/dashboard/procedures"
                        className="flex items-center gap-3 p-4 border-2 border-gray-200 rounded-lg hover:border-blue-500 hover:bg-blue-50 transition-all"
                    >
                        <FileText className="w-5 h-5 text-blue-600" />
                        <span className="font-medium text-gray-700">Xem Quy trình</span>
                    </Link>
                    <Link
                        href="/dashboard/submissions/new"
                        className="flex items-center gap-3 p-4 border-2 border-gray-200 rounded-lg hover:border-green-500 hover:bg-green-50 transition-all"
                    >
                        <Upload className="w-5 h-5 text-green-600" />
                        <span className="font-medium text-gray-700">Nộp Biểu mẫu</span>
                    </Link>
                    <Link
                        href="/dashboard/approvals"
                        className="flex items-center gap-3 p-4 border-2 border-gray-200 rounded-lg hover:border-orange-500 hover:bg-orange-50 transition-all"
                    >
                        <CheckCircle className="w-5 h-5 text-orange-600" />
                        <span className="font-medium text-gray-700">Phê duyệt</span>
                    </Link>
                    <Link
                        href="/dashboard/audit"
                        className="flex items-center gap-3 p-4 border-2 border-gray-200 rounded-lg hover:border-purple-500 hover:bg-purple-50 transition-all"
                    >
                        <Activity className="w-5 h-5 text-purple-600" />
                        <span className="font-medium text-gray-700">Nhật ký</span>
                    </Link>
                </div>
            </div>
        </div>
    );
}
