'use client';

import { useEffect, useState } from 'react';
import { FileText, Upload, CheckCircle, Clock } from 'lucide-react';

interface DashboardStats {
    totalProcedures: number;
    totalSubmissions: number;
    pendingApprovals: number;
    recentActivities: number;
}

export default function DashboardPage() {
    const [stats, setStats] = useState<DashboardStats>({
        totalProcedures: 0,
        totalSubmissions: 0,
        pendingApprovals: 0,
        recentActivities: 0,
    });

    useEffect(() => {
        // TODO: Load stats from API
        setStats({
            totalProcedures: 5,
            totalSubmissions: 12,
            pendingApprovals: 3,
            recentActivities: 8,
        });
    }, []);

    const statCards = [
        {
            title: 'Quy trình',
            value: stats.totalProcedures,
            icon: FileText,
            color: 'bg-blue-500',
            bgColor: 'bg-blue-50',
            textColor: 'text-blue-600',
        },
        {
            title: 'Biểu mẫu đã nộp',
            value: stats.totalSubmissions,
            icon: Upload,
            color: 'bg-green-500',
            bgColor: 'bg-green-50',
            textColor: 'text-green-600',
        },
        {
            title: 'Chờ phê duyệt',
            value: stats.pendingApprovals,
            icon: Clock,
            color: 'bg-orange-500',
            bgColor: 'bg-orange-50',
            textColor: 'text-orange-600',
        },
        {
            title: 'Hoạt động gần đây',
            value: stats.recentActivities,
            icon: CheckCircle,
            color: 'bg-purple-500',
            bgColor: 'bg-purple-50',
            textColor: 'text-purple-600',
        },
    ];

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
                        <div
                            key={card.title}
                            className="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow"
                        >
                            <div className="flex items-center justify-between">
                                <div>
                                    <p className="text-sm text-gray-600 mb-1">{card.title}</p>
                                    <p className="text-3xl font-bold text-gray-900">
                                        {card.value}
                                    </p>
                                </div>
                                <div className={`${card.bgColor} p-3 rounded-lg`}>
                                    <Icon className={`w-6 h-6 ${card.textColor}`} />
                                </div>
                            </div>
                        </div>
                    );
                })}
            </div>

            {/* Welcome Card */}
            <div className="bg-gradient-to-r from-blue-500 to-indigo-600 rounded-xl shadow-lg p-8 text-white">
                <h2 className="text-2xl font-bold mb-2">
                    Hệ thống Quản lý Quy trình An toàn & Biểu mẫu QHSE
                </h2>
                <p className="text-blue-100 mb-6">
                    Ship Safety Management System - QHSE Process & Forms Management
                </p>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <div className="bg-white/10 backdrop-blur-sm rounded-lg p-4">
                        <h3 className="font-semibold mb-1">Quản lý Quy trình</h3>
                        <p className="text-sm text-blue-100">
                            Tạo và quản lý các quy trình QHSE
                        </p>
                    </div>
                    <div className="bg-white/10 backdrop-blur-sm rounded-lg p-4">
                        <h3 className="font-semibold mb-1">Nộp Biểu mẫu</h3>
                        <p className="text-sm text-blue-100">
                            Nộp và theo dõi biểu mẫu an toàn
                        </p>
                    </div>
                    <div className="bg-white/10 backdrop-blur-sm rounded-lg p-4">
                        <h3 className="font-semibold mb-1">Phê duyệt</h3>
                        <p className="text-sm text-blue-100">
                            Xem xét và phê duyệt biểu mẫu
                        </p>
                    </div>
                </div>
            </div>

            {/* Quick Actions */}
            <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h2 className="text-lg font-semibold text-gray-900 mb-4">
                    Thao tác nhanh
                </h2>
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <button className="flex items-center gap-3 p-4 border-2 border-gray-200 rounded-lg hover:border-blue-500 hover:bg-blue-50 transition-all">
                        <FileText className="w-5 h-5 text-blue-600" />
                        <span className="font-medium text-gray-700">Xem Quy trình</span>
                    </button>
                    <button className="flex items-center gap-3 p-4 border-2 border-gray-200 rounded-lg hover:border-green-500 hover:bg-green-50 transition-all">
                        <Upload className="w-5 h-5 text-green-600" />
                        <span className="font-medium text-gray-700">Nộp Biểu mẫu</span>
                    </button>
                    <button className="flex items-center gap-3 p-4 border-2 border-gray-200 rounded-lg hover:border-orange-500 hover:bg-orange-50 transition-all">
                        <CheckCircle className="w-5 h-5 text-orange-600" />
                        <span className="font-medium text-gray-700">Phê duyệt</span>
                    </button>
                </div>
            </div>
        </div>
    );
}
