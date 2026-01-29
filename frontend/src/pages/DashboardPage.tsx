import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import {
    FileText,
    Upload,
    CheckCircle,
    Clock,
    Users,
    Building,
    Activity,
    ArrowRight,
} from 'lucide-react';
import { Card, StatCardSkeleton, Button } from '@/components/ui';
import { apiClient, DashboardStats, AuditLog } from '@/api/client';
import { getActionLabel } from '@/utils/labels';
import { format } from 'date-fns';
import { vi } from 'date-fns/locale';

interface StatCardProps {
    title: string;
    value: number;
    subtitle: string;
    icon: React.ElementType;
    color: string;
    href: string;
    delay?: number;
}

function StatCard({ title, value, subtitle, icon: Icon, color, href, delay = 0 }: StatCardProps) {
    return (
        <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay }}
        >
            <Link to={href}>
                <div className="bg-white rounded-xl p-6 shadow-sm border border-slate-200 hover:shadow-md hover:border-blue-200 transition-all duration-300 group">
                    <div className="flex items-start justify-between">
                        <div>
                            <p className="text-sm font-medium text-slate-500 mb-2">{title}</p>
                            <div className="flex items-baseline gap-2">
                                <p className="text-3xl font-bold text-slate-900 tracking-tight">{value}</p>
                            </div>
                            <p className="text-xs text-slate-400 mt-2 font-medium">{subtitle}</p>
                        </div>
                        <div className={`p-3.5 rounded-xl ${color} shadow-sm group-hover:scale-110 transition-transform duration-300`}>
                            <Icon className="w-5 h-5 text-white" />
                        </div>
                    </div>
                </div>
            </Link>
        </motion.div>
    );
}

function getActionClass(action: string): string {
    const classes: Record<string, string> = {
        Create: 'text-emerald-600 bg-emerald-50 border-emerald-200',
        Edit: 'text-blue-600 bg-blue-50 border-blue-200',
        Update: 'text-blue-600 bg-blue-50 border-blue-200',
        Delete: 'text-red-600 bg-red-50 border-red-200',
        Submit: 'text-indigo-600 bg-indigo-50 border-indigo-200',
        Approve: 'text-emerald-600 bg-emerald-50 border-emerald-200',
        Reject: 'text-amber-600 bg-amber-50 border-amber-200',
        Recall: 'text-slate-600 bg-slate-100 border-slate-200',
        Withdraw: 'text-slate-600 bg-slate-100 border-slate-200',
        Login: 'text-slate-600 bg-slate-50 border-slate-200',
        Logout: 'text-slate-600 bg-slate-50 border-slate-200'
    };
    return classes[action] || 'text-slate-600 bg-slate-50 border-slate-200';
}

export default function DashboardPage() {
    const [stats, setStats] = useState<DashboardStats | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const loadStats = async () => {
            try {
                const response = await apiClient.getDashboardStats();
                if (response.success && response.data) {
                    // Ensure recentActivities is always an array
                    const sanitizedData = {
                        ...response.data,
                        recentActivities: Array.isArray(response.data.recentActivities) 
                            ? response.data.recentActivities 
                            : []
                    };
                    setStats(sanitizedData);
                } else {
                    setError(response.message || 'Không thể tải dữ liệu');
                }
            } catch {
                setError('Lỗi kết nối server');
            } finally {
                setLoading(false);
            }
        };

        loadStats();
    }, []);

    if (loading) {
        return (
            <div className="space-y-6">
                <div>
                    <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Tổng quan</h1>
                    <p className="text-[var(--color-neutral-500)] mt-1">Đang tải dữ liệu...</p>
                </div>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                    {[1, 2, 3, 4].map((i) => <StatCardSkeleton key={i} />)}
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="space-y-6">
                <div>
                    <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Tổng quan</h1>
                    <p className="text-[var(--color-danger-600)] mt-1">{error}</p>
                </div>
                <Card padding="lg" className="text-center">
                    <Activity className="w-12 h-12 text-[var(--color-neutral-300)] mx-auto mb-4" />
                    <p className="text-[var(--color-neutral-500)] mb-4">
                        Không thể tải dữ liệu từ server. Vui lòng kiểm tra kết nối và thử lại.
                    </p>
                    <Button onClick={() => window.location.reload()}>Thử lại</Button>
                </Card>
            </div>
        );
    }

    const statCards = [
        {
            title: 'Tổng số Quy trình',
            value: stats?.totalProcedures || 0,
            subtitle: 'đang quản lý',
            icon: FileText,
            color: 'bg-[var(--color-primary-500)]',
            href: '/procedures',
        },
        {
            title: 'Tổng số Biểu mẫu',
            value: stats?.totalTemplates || 0,
            subtitle: 'form & checklist',
            icon: Upload,
            color: 'bg-[var(--color-success-500)]',
            href: '/submissions',
        },
        {
            title: 'Chờ phê duyệt',
            value: stats?.pendingApprovals || 0,
            subtitle: 'mục cần xử lý',
            icon: Clock,
            color: 'bg-[var(--color-warning-500)]',
            href: '/approvals',
        },
        {
            title: 'Đã phê duyệt',
            value: stats?.approvedSubmissions || 0,
            subtitle: 'biểu mẫu hoàn thành',
            icon: CheckCircle,
            color: 'bg-[var(--color-success-600)]',
            href: '/submissions',
        },
    ];

    return (
        <div className="space-y-6">
            {/* Header */}
            <motion.div
                initial={{ opacity: 0, y: -10 }}
                animate={{ opacity: 1, y: 0 }}
            >
                <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Tổng quan</h1>
                <p className="text-[var(--color-neutral-500)] mt-1">
                    Chào mừng bạn đến với Hệ thống Quản lý QHSE
                </p>
            </motion.div>

            {/* Stats Grid */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                {statCards.map((card, i) => (
                    <StatCard key={card.title} {...card} delay={i * 0.1} />
                ))}
            </div>

            {/* Secondary Stats */}
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.4 }}>
                    <Card>
                        <div className="flex items-center gap-3 mb-3">
                            <div className="p-2 bg-purple-50 rounded-lg">
                                <Users className="w-5 h-5 text-purple-600" />
                            </div>
                            <span className="text-sm font-medium text-[var(--color-neutral-500)]">Người dùng</span>
                        </div>
                        <p className="text-2xl font-bold text-[var(--color-neutral-900)]">{stats?.totalUsers || 0}</p>
                    </Card>
                </motion.div>
                <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.5 }}>
                    <Card>
                        <div className="flex items-center gap-3 mb-3">
                            <div className="p-2 bg-indigo-50 rounded-lg">
                                <Building className="w-5 h-5 text-indigo-600" />
                            </div>
                            <span className="text-sm font-medium text-[var(--color-neutral-500)]">Đơn vị</span>
                        </div>
                        <p className="text-2xl font-bold text-[var(--color-neutral-900)]">{stats?.totalUnits || 0}</p>
                    </Card>
                </motion.div>
                <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.6 }}>
                    <Card>
                        <div className="flex items-center gap-3 mb-3">
                            <div className="p-2 bg-red-50 rounded-lg">
                                <FileText className="w-5 h-5 text-red-600" />
                            </div>
                            <span className="text-sm font-medium text-[var(--color-neutral-500)]">Từ chối</span>
                        </div>
                        <p className="text-2xl font-bold text-[var(--color-neutral-900)]">{stats?.rejectedSubmissions || 0}</p>
                    </Card>
                </motion.div>
            </div>

            {/* Hero Card */}
            <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.7 }}
            >
                <div className="relative overflow-hidden rounded-2xl shadow-xl bg-slate-900 text-white p-8">
                    {/* Background Pattern */}
                    <div className="absolute top-0 right-0 w-1/2 h-full bg-gradient-to-l from-blue-900/20 to-transparent pointer-events-none" />
                    <div className="absolute -bottom-24 -right-24 w-64 h-64 bg-blue-500/10 rounded-full blur-3xl pointer-events-none" />

                    <div className="relative z-10">
                        <div className="flex flex-col md:flex-row justify-between items-start md:items-end gap-6 mb-8">
                            <div>
                                <h2 className="text-2xl font-bold mb-2 text-white tracking-tight">
                                    Hệ thống Quản lý Quy trình An toàn & Biểu mẫu QHSE
                                </h2>
                                <p className="text-slate-100 max-w-2xl">
                                    Hệ thống quản lý an toàn tàu - Quy trình & Biểu mẫu QHSE (v3.4.2)
                                </p>
                            </div>
                            <div className="px-4 py-1.5 rounded-full bg-blue-500/20 border border-blue-500/30 text-white text-sm font-medium">
                                Phiên bản Doanh nghiệp
                            </div>
                        </div>

                        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                            <Link
                                to="/procedures"
                                className="group relative bg-slate-800/50 hover:bg-slate-800 border border-slate-700 hover:border-blue-500/50 rounded-xl p-5 transition-all duration-300"
                            >
                                <div className="absolute inset-0 bg-blue-500/5 opacity-0 group-hover:opacity-100 rounded-xl transition-opacity" />
                                <div className="flex items-center gap-3 mb-2">
                                    <div className="p-2 rounded-lg bg-blue-500/10 text-blue-400 group-hover:bg-blue-500 group-hover:text-white transition-colors">
                                        <FileText className="w-5 h-5" />
                                    </div>
                                    <h3 className="font-semibold text-white transition-colors">Quản lý Quy trình</h3>
                                </div>
                                <p className="text-sm text-slate-200 pl-[52px]">Tạo và quản lý các quy trình QHSE</p>
                                <ArrowRight className="absolute top-5 right-5 w-4 h-4 text-slate-200 group-hover:text-blue-200 -translate-x-2 opacity-0 group-hover:opacity-100 group-hover:translate-x-0 transition-all" />
                            </Link>

                            <Link
                                to="/submissions"
                                className="group relative bg-slate-800/50 hover:bg-slate-800 border border-slate-700 hover:border-emerald-500/50 rounded-xl p-5 transition-all duration-300"
                            >
                                <div className="absolute inset-0 bg-emerald-500/5 opacity-0 group-hover:opacity-100 rounded-xl transition-opacity" />
                                <div className="flex items-center gap-3 mb-2">
                                    <div className="p-2 rounded-lg bg-emerald-500/10 text-emerald-400 group-hover:bg-emerald-500 group-hover:text-white transition-colors">
                                        <Upload className="w-5 h-5" />
                                    </div>
                                    <h3 className="font-semibold text-white transition-colors">Nộp Biểu mẫu</h3>
                                </div>
                                <p className="text-sm text-slate-200 pl-[52px]">Nộp và theo dõi biểu mẫu an toàn</p>
                                <ArrowRight className="absolute top-5 right-5 w-4 h-4 text-slate-200 group-hover:text-emerald-200 -translate-x-2 opacity-0 group-hover:opacity-100 group-hover:translate-x-0 transition-all" />
                            </Link>

                            <Link
                                to="/approvals"
                                className="group relative bg-slate-800/50 hover:bg-slate-800 border border-slate-700 hover:border-amber-500/50 rounded-xl p-5 transition-all duration-300"
                            >
                                <div className="absolute inset-0 bg-amber-500/5 opacity-0 group-hover:opacity-100 rounded-xl transition-opacity" />
                                <div className="flex items-center gap-3 mb-2">
                                    <div className="p-2 rounded-lg bg-amber-500/10 text-amber-400 group-hover:bg-amber-500 group-hover:text-white transition-colors">
                                        <CheckCircle className="w-5 h-5" />
                                    </div>
                                    <h3 className="font-semibold text-white transition-colors">Phê duyệt</h3>
                                </div>
                                <p className="text-sm text-slate-200 pl-[52px]">Xem xét và phê duyệt biểu mẫu</p>
                                <ArrowRight className="absolute top-5 right-5 w-4 h-4 text-slate-200 group-hover:text-amber-200 -translate-x-2 opacity-0 group-hover:opacity-100 group-hover:translate-x-0 transition-all" />
                            </Link>
                        </div>
                    </div>
                </div>
            </motion.div>

            {/* Recent Activities */}
            <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 0.8 }}
            >
                <div className="bg-white rounded-xl shadow-sm border border-slate-200 overflow-hidden">
                    <div className="p-6 border-b border-slate-100 flex items-center justify-between">
                        <div className="flex items-center gap-3">
                            <div className="p-2 bg-slate-100 rounded-lg">
                                <Activity className="w-5 h-5 text-slate-600" />
                            </div>
                            <h2 className="text-lg font-bold text-slate-800">
                                Hoạt động gần đây
                            </h2>
                        </div>
                        <Link to="/audit" className="text-sm font-medium text-blue-600 hover:text-blue-700 hover:underline">
                            Xem tất cả →
                        </Link>
                    </div>

                    {stats?.recentActivities && stats.recentActivities.length > 0 ? (
                        <div className="overflow-x-auto">
                            <table className="w-full">
                                <thead className="bg-slate-50 border-b border-slate-200">
                                    <tr>
                                        <th className="text-left py-4 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Thời gian</th>
                                        <th className="text-left py-4 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Người dùng</th>
                                        <th className="text-left py-4 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Hành động</th>
                                        <th className="text-left py-4 px-6 text-xs font-semibold text-slate-500 uppercase tracking-wider">Đối tượng</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-slate-100">
                                    {stats.recentActivities.slice(0, 6).map((activity: AuditLog, index: number) => (
                                        <motion.tr
                                            key={index}
                                            initial={{ opacity: 0, x: -10 }}
                                            animate={{ opacity: 1, x: 0 }}
                                            transition={{ delay: 0.9 + index * 0.05 }}
                                            className="hover:bg-slate-50/80 transition-colors cursor-default"
                                        >
                                            <td className="py-4 px-6 text-sm font-medium text-slate-500 tabular-nums">
                                                {format(new Date(activity.time), 'dd/MM/yyyy HH:mm', { locale: vi })}
                                            </td>
                                            <td className="py-4 px-6">
                                                <div className="flex items-center gap-2">
                                                    <div className="w-6 h-6 rounded-full bg-slate-200 flex items-center justify-center text-xs font-bold text-slate-500">
                                                        {activity.userName.charAt(0)}
                                                    </div>
                                                    <span className="text-sm font-medium text-slate-700">{activity.userName}</span>
                                                </div>
                                            </td>
                                            <td className="py-4 px-6">
                                                <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border ${getActionClass(activity.action)}`}>
                                                    {getActionLabel(activity.action)}
                                                </span>
                                            </td>
                                            <td className="py-4 px-6 text-sm text-slate-600">{activity.target}</td>
                                        </motion.tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    ) : (
                        <div className="text-center py-12">
                            <div className="w-16 h-16 bg-slate-50 rounded-full flex items-center justify-center mx-auto mb-4">
                                <Activity className="w-8 h-8 text-slate-300" />
                            </div>
                            <h3 className="text-slate-900 font-medium mb-1">Chưa có hoạt động</h3>
                            <p className="text-slate-500 text-sm">Các hoạt động mới sẽ xuất hiện tại đây</p>
                        </div>
                    )}
                </div>
            </motion.div>

            {/* Quick Actions */}
            {/* Quick Actions */}
            <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: 1 }}
            >
                <div className="bg-white rounded-xl shadow-sm border border-slate-200 p-6">
                    <h2 className="text-lg font-bold text-slate-800 mb-4">
                        Thao tác nhanh
                    </h2>
                    <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                        {[
                            { icon: FileText, label: 'Xem Quy trình', href: '/procedures', color: 'bg-blue-50 text-blue-600 border-blue-200 hover:border-blue-400 hover:shadow-md' },
                            { icon: Upload, label: 'Nộp Biểu mẫu', href: '/submissions/new', color: 'bg-emerald-50 text-emerald-600 border-emerald-200 hover:border-emerald-400 hover:shadow-md' },
                            { icon: CheckCircle, label: 'Phê duyệt', href: '/approvals', color: 'bg-amber-50 text-amber-600 border-amber-200 hover:border-amber-400 hover:shadow-md' },
                            { icon: Activity, label: 'Nhật ký', href: '/audit', color: 'bg-purple-50 text-purple-600 border-purple-200 hover:border-purple-400 hover:shadow-md' },
                        ].map((action) => (
                            <motion.div
                                key={action.href}
                                whileHover={{ scale: 1.02 }}
                                whileTap={{ scale: 0.98 }}
                            >
                                <Link
                                    to={action.href}
                                    className={`flex items-center gap-3 p-4 border rounded-xl transition-all duration-300 ${action.color}`}
                                >
                                    <div className="p-1.5 bg-white/60 rounded-lg shadow-sm">
                                        <action.icon className="w-5 h-5" />
                                    </div>
                                    <span className="font-semibold">{action.label}</span>
                                </Link>
                            </motion.div>
                        ))}
                    </div>
                </div>
            </motion.div>
        </div>
    );
}
