import { Link, useLocation } from 'react-router-dom';
import { clsx } from 'clsx';
import { motion } from 'framer-motion';
import {
    LayoutDashboard,
    FileText,
    Upload,
    CheckCircle,
    Clock,
    Settings,
    ClipboardList,
    LogOut,
    User,
    Ship,
    ChevronDown,
    Users,
    Shield,
} from 'lucide-react';
import { useAuthStore } from '@/stores/authStore';
import { getUnitTypeLabel } from '@/utils/labels';
import { useState } from 'react';

interface MenuItem {
    title: string;
    icon: React.ElementType;
    href: string;
    roles: string[];
}

const menuItems: MenuItem[] = [
    { title: 'Tổng quan', icon: LayoutDashboard, href: '/', roles: ['Admin', 'Manager', 'User'] },
    { title: 'Quy trình', icon: FileText, href: '/procedures', roles: ['Admin', 'Manager'] },
    { title: 'Biểu mẫu', icon: ClipboardList, href: '/templates', roles: ['Admin', 'Manager', 'User'] },
    { title: 'Nộp biểu mẫu', icon: Upload, href: '/submissions', roles: ['Admin', 'Manager', 'User'] },
    { title: 'Phê duyệt', icon: CheckCircle, href: '/approvals', roles: ['Admin', 'Manager'] },
    { title: 'Nhật ký', icon: Clock, href: '/audit', roles: ['Admin', 'Manager'] },
    { title: 'Người dùng', icon: Users, href: '/users', roles: ['Admin', 'Manager'] },
    { title: 'Vai trò', icon: Shield, href: '/roles', roles: ['Admin'] },
    { title: 'Cài đặt', icon: Settings, href: '/settings', roles: ['Admin'] },
];

interface SidebarProps {
    units?: { id: number; name: string; type: string }[];
}

export default function Sidebar({ units = [] }: SidebarProps) {
    const location = useLocation();
    const { user, currentUnitId, setCurrentUnit, logout } = useAuthStore();
    const [showUnitDropdown, setShowUnitDropdown] = useState(false);

    const filteredMenuItems = menuItems.filter((item) =>
        user?.role ? item.roles.includes(user.role) : false
    );

    const currentUnit = units.find((u) => u.id === currentUnitId);

    const handleUnitChange = (unitId: number) => {
        setCurrentUnit(unitId);
        setShowUnitDropdown(false);
        window.location.reload();
    };

    return (
        <>
            <aside className="h-full w-64 bg-slate-900 text-white border-r border-slate-800 shadow-xl overflow-hidden">
                <div className="h-full flex flex-col overflow-y-auto">
                    {/* Logo */}
                    <div className="p-6 border-b border-slate-800 flex items-center gap-3 flex-shrink-0">
                        <div className="w-8 h-8 rounded-lg bg-gradient-to-br from-sky-500 to-sky-700 flex items-center justify-center shadow-lg shadow-sky-900/50">
                            <Ship className="w-5 h-5 text-white" />
                        </div>
                        <div>
                            <h1 className="text-lg font-bold tracking-tight text-white drop-shadow-sm">SSMS QHSE</h1>
                            <p className="text-xs text-slate-100 font-medium tracking-wide drop-shadow-sm">AN TOÀN HÀNG HẢI</p>
                        </div>
                    </div>

                {/* Unit Selector for Admin */}
                {user?.role === 'Admin' && (
                    <div className="px-4 py-4 border-b border-slate-800 flex-shrink-0">
                        <div className="relative">
                            <button
                                onClick={() => units.length > 0 && setShowUnitDropdown(!showUnitDropdown)}
                                disabled={units.length === 0}
                                className="w-full flex items-center justify-between gap-2 px-3 py-3 md:py-2.5 bg-slate-800/50 border border-slate-700/50 rounded-lg hover:bg-slate-800 transition-all text-sm group touch-manipulation disabled:opacity-70 disabled:cursor-not-allowed"
                            >
                                <div className="flex items-center gap-3 truncate">
                                    <div className="w-6 h-6 rounded bg-slate-700 flex items-center justify-center text-slate-300 group-hover:text-white transition-colors">
                                        <Ship className="w-3.5 h-3.5" />
                                    </div>
                                    <span className="text-slate-100 group-hover:text-white truncate font-medium">
                                        {currentUnit?.name || (units.length > 0 ? 'Chọn đơn vị' : 'Chưa có đơn vị')}
                                    </span>
                                </div>
                                <ChevronDown className={clsx('w-4 h-4 text-slate-500 group-hover:text-white transition-all', showUnitDropdown && 'rotate-180')} />
                            </button>

                            {showUnitDropdown && (
                                <motion.div
                                    initial={{ opacity: 0, y: -5 }}
                                    animate={{ opacity: 1, y: 0 }}
                                    className="absolute top-full left-0 right-0 mt-2 bg-slate-800 border border-slate-700 rounded-xl shadow-2xl z-50 max-h-60 overflow-y-auto overflow-x-hidden"
                                >
                                    {units.length === 0 ? (
                                        <div className="px-4 py-3 text-sm text-slate-200">Chưa có đơn vị</div>
                                    ) : (
                                        units.map((unit) => (
                                            <button
                                                key={unit.id}
                                                onClick={() => handleUnitChange(unit.id)}
                                                className={clsx(
                                                    'w-full text-left px-4 py-3 text-sm hover:bg-slate-700/50 transition-colors border-b border-slate-700/50 last:border-0',
                                                    currentUnitId === unit.id ? 'bg-sky-600/10 border-l-2 border-sky-500 pl-[14px] text-slate-100' : 'text-slate-200 hover:text-white'
                                                )}
                                            >
                                                <div className={clsx("font-medium", currentUnitId === unit.id ? "text-sky-300" : "text-slate-100")}>{unit.name}</div>
                                                <div className="text-xs text-slate-300 mt-0.5">{getUnitTypeLabel(unit.type)}</div>
                                            </button>
                                        ))
                                    )}
                                </motion.div>
                            )}
                        </div>
                    </div>
                )}

                {/* Navigation */}
                <nav className="flex-1 overflow-y-auto p-4 space-y-1">
                    {filteredMenuItems.map((item) => {
                        const Icon = item.icon;
                        const isActive = location.pathname === item.href ||
                            (item.href !== '/' && location.pathname.startsWith(item.href));

                        return (
                            <div key={item.href}>
                                <Link
                                    to={item.href}
                                    className={clsx(
                                        'group flex items-center gap-3 px-4 py-3.5 md:py-3 rounded-lg transition-all duration-200 relative overflow-hidden touch-manipulation',
                                        isActive
                                            ? 'bg-sky-600 text-white shadow-lg shadow-sky-900/30'
                                            : 'text-slate-400 hover:bg-slate-800 hover:text-white'
                                    )}
                                >
                                    {isActive && (
                                        <motion.div
                                            layoutId="active-bg"
                                            className="absolute inset-0 bg-sky-600 z-0 rounded-lg"
                                            initial={false}
                                            transition={{ type: "spring", stiffness: 300, damping: 30 }}
                                        />
                                    )}
                                    <Icon className={clsx("w-5 h-5 relative z-10 transition-transform group-hover:scale-110", isActive ? "text-white" : "text-slate-400 group-hover:text-white")} />
                                    <span className="relative z-10 font-medium">{item.title}</span>
                                </Link>
                            </div>
                        );
                    })}
                </nav>

                {/* User Info */}
                <div className="p-4 border-t border-slate-800 bg-slate-900/50 flex-shrink-0">
                    <div className="flex items-center gap-3 mb-4 px-2">
                        <div className="w-10 h-10 bg-slate-800 rounded-full flex items-center justify-center border border-slate-700">
                            <User className="w-5 h-5 text-slate-300" />
                        </div>
                        <div className="flex-1 min-w-0">
                            <p className="text-sm font-semibold text-white truncate">
                                {user?.fullName}
                            </p>
                            <p className="text-xs text-slate-400 truncate">
                                {user?.role} - {user?.unitName}
                            </p>
                        </div>
                    </div>
                    <button
                        onClick={() => void logout()}
                        className="w-full flex items-center justify-center gap-2 px-4 py-3 md:py-2.5 text-sm font-medium text-red-400 hover:text-red-300 hover:bg-red-500/10 rounded-lg transition-all border border-transparent hover:border-red-500/20 touch-manipulation"
                    >
                        <LogOut className="w-4 h-4" />
                        <span>Đăng xuất</span>
                    </button>
                </div>
            </div>

            {/* Overlay for dropdown */}
            {showUnitDropdown && (
                <div
                    className="fixed inset-0 z-30 bg-black/20 backdrop-blur-[1px]"
                    onClick={() => setShowUnitDropdown(false)}
                />
            )}
        </aside>
        </>
    );
}
