'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { apiClient, UserInfo } from '@/lib/api-client';

export default function LoginPage() {
    const router = useRouter();
    const [users, setUsers] = useState<UserInfo[]>([]);
    const [selectedUserId, setSelectedUserId] = useState<number | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        loadUsers();
    }, []);

    const loadUsers = async () => {
        const response = await apiClient.getAvailableUsers();
        if (response.success && response.data) {
            setUsers(response.data);
        } else {
            setError(response.message || 'Không thể tải danh sách người dùng');
        }
    };

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!selectedUserId) {
            setError('Vui lòng chọn người dùng');
            return;
        }

        setLoading(true);
        setError(null);

        const response = await apiClient.login({ userId: selectedUserId });

        if (response.success && response.data) {
            // Lưu thông tin user vào localStorage
            localStorage.setItem('user', JSON.stringify(response.data));
            router.push('/dashboard');
        } else {
            setError(response.message || 'Đăng nhập thất bại');
        }

        setLoading(false);
    };

    return (
        <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-blue-50 to-indigo-100">
            <div className="bg-white p-8 rounded-2xl shadow-2xl w-full max-w-md">
                <div className="text-center mb-8">
                    <h1 className="text-3xl font-bold text-gray-800 mb-2">
                        SSMS - Hệ thống QHSE
                    </h1>
                    <p className="text-gray-600">
                        Quản lý Quy trình An toàn & Biểu mẫu
                    </p>
                </div>

                <form onSubmit={handleLogin} className="space-y-6">
                    <div>
                        <label htmlFor="user-select" className="block text-sm font-medium text-gray-700 mb-2">
                            Chọn người dùng
                        </label>
                        <select
                            id="user-select"
                            value={selectedUserId || ''}
                            onChange={(e) => setSelectedUserId(Number(e.target.value))}
                            className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all"
                            disabled={loading}
                        >
                            <option value="">-- Chọn tài khoản --</option>
                            {users.map((user) => (
                                <option key={user.id} value={user.id}>
                                    {user.fullName} ({user.role}) - {user.unitName}
                                </option>
                            ))}
                        </select>
                    </div>

                    {error && (
                        <div className="bg-red-50 border border-red-200 text-red-700 px-4 py-3 rounded-lg">
                            {error}
                        </div>
                    )}

                    <button
                        type="submit"
                        disabled={loading || !selectedUserId}
                        className="w-full bg-blue-600 text-white py-3 px-4 rounded-lg font-medium hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-all"
                    >
                        {loading ? 'Đang đăng nhập...' : 'Đăng nhập'}
                    </button>
                </form>

                <div className="mt-6 text-center text-sm text-gray-500">
                    <p>Mock Authentication - Development Mode</p>
                </div>
            </div>
        </div>
    );
}
