'use client';

import { useEffect, useState, useMemo, useCallback } from 'react';
import { useRouter, usePathname } from 'next/navigation';
import Link from 'next/link';
import {
    LayoutDashboard,
    FileText,
    Upload,
    CheckCircle,
    Menu,
    X,
    LogOut,
    User,
    Clock,
    Settings
} from 'lucide-react';

interface UserData {
    fullName: string;
    role: string;
    unitName: string;
    email: string;
}

function parseUserData(data: string | null): UserData | null {
    if (!data) return null;
    try {
        return JSON.parse(data) as UserData;
    } catch {
        return null;
    }
}

export default function DashboardLayout({
    children,
}: {
    children: React.ReactNode;
}) {
    const router = useRouter();
    const pathname = usePathname();
    const [user, setUser] = useState<UserData | null>(null);
    const [sidebarOpen, setSidebarOpen] = useState(true);
    const [isInitialized, setIsInitialized] = useState(false);

    useEffect(() => {
        // Kiểm tra authentication
        const token = localStorage.getItem('token');
        const userData = localStorage.getItem('user');

        if (!token || !userData) {
            router.push('/login');
            return;
        }

        const parsedUser = parseUserData(userData);
        if (parsedUser) {
            setUser(parsedUser);
        } else {
            router.push('/login');
            return;
        }
        setIsInitialized(true);
    }, [router]);

    const handleLogout = useCallback(() => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        router.push('/login');
    }, [router]);

    const menuItems = useMemo(() => [
        {
            title: 'Tổng quan',
            icon: LayoutDashboard,
            href: '/dashboard',
            roles: ['Admin', 'Manager', 'User'],
        },
        {
            title: 'Quản lý Quy trình',
            icon: FileText,
            href: '/dashboard/procedures',
            roles: ['Admin', 'Manager'],
        },
        {
            title: 'Nộp Biểu mẫu',
            icon: Upload,
            href: '/dashboard/submissions',
            roles: ['Admin', 'Manager', 'User'],
        },
        {
            title: 'Phê duyệt',
            icon: CheckCircle,
            href: '/dashboard/approvals',
            roles: ['Admin', 'Manager'],
        },
        {
            title: 'Nhật ký',
            icon: Clock,
            href: '/dashboard/audit',
            roles: ['Admin', 'Manager'],
        },
        {
            title: 'Cài đặt',
            icon: Settings,
            href: '/dashboard/settings',
            roles: ['Admin'],
        },
    ], []);

    const filteredMenuItems = useMemo(() => {
        if (!user) return [];
        return menuItems.filter((item) => item.roles.includes(user.role));
    }, [menuItems, user]);

    if (!isInitialized || !user) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-gray-600">Đang tải...</div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Sidebar */}
            <aside
                className={`fixed top-0 left-0 z-40 h-screen transition-transform ${sidebarOpen ? 'translate-x-0' : '-translate-x-full'
                    } bg-white border-r border-gray-200 w-64`}
            >
                <div className="h-full flex flex-col">
                    {/* Logo */}
                    <div className="p-6 border-b border-gray-200">
                        <h1 className="text-xl font-bold text-gray-800">SSMS QHSE</h1>
                        <p className="text-sm text-gray-600 mt-1">Quản lý An toàn</p>
                    </div>

                    {/* Navigation */}
                    <nav className="flex-1 overflow-y-auto p-4">
                        <ul className="space-y-2">
                            {filteredMenuItems.map((item) => {
                                const Icon = item.icon;
                                const isActive = pathname === item.href;

                                return (
                                    <li key={item.href}>
                                        <Link
                                            href={item.href}
                                            className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${isActive
                                                ? 'bg-blue-50 text-blue-600 font-medium'
                                                : 'text-gray-700 hover:bg-gray-50'
                                                }`}
                                        >
                                            <Icon className="w-5 h-5" />
                                            <span>{item.title}</span>
                                        </Link>
                                    </li>
                                );
                            })}
                        </ul>
                    </nav>

                    {/* User Info */}
                    <div className="p-4 border-t border-gray-200">
                        <div className="flex items-center gap-3 mb-3">
                            <div className="w-10 h-10 bg-blue-100 rounded-full flex items-center justify-center">
                                <User className="w-5 h-5 text-blue-600" />
                            </div>
                            <div className="flex-1 min-w-0">
                                <p className="text-sm font-medium text-gray-900 truncate">
                                    {user.fullName}
                                </p>
                                <p className="text-xs text-gray-500 truncate">
                                    {user.role} - {user.unitName}
                                </p>
                            </div>
                        </div>
                        <button
                            onClick={handleLogout}
                            className="w-full flex items-center justify-center gap-2 px-4 py-2 text-sm text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                        >
                            <LogOut className="w-4 h-4" />
                            <span>Đăng xuất</span>
                        </button>
                    </div>
                </div>
            </aside>

            {/* Main Content */}
            <div className={`${sidebarOpen ? 'ml-64' : 'ml-0'} transition-all`}>
                {/* Header */}
                <header className="bg-white border-b border-gray-200 sticky top-0 z-30">
                    <div className="px-6 py-4 flex items-center justify-between">
                        <button
                            onClick={() => setSidebarOpen(!sidebarOpen)}
                            className="p-2 rounded-lg hover:bg-gray-100 transition-colors"
                        >
                            {sidebarOpen ? (
                                <X className="w-6 h-6 text-gray-600" />
                            ) : (
                                <Menu className="w-6 h-6 text-gray-600" />
                            )}
                        </button>

                        <div className="flex items-center gap-4">
                            <div className="text-right">
                                <p className="text-sm font-medium text-gray-900">
                                    {user.fullName}
                                </p>
                                <p className="text-xs text-gray-500">{user.email}</p>
                            </div>
                        </div>
                    </div>
                </header>

                {/* Page Content */}
                <main className="p-6">{children}</main>
            </div>
        </div>
    );
}
