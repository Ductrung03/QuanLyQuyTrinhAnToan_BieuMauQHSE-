import { useState } from 'react';
import { motion } from 'framer-motion';
import { Settings as SettingsIcon, Database, Download, Upload, RefreshCw, Shield, Users } from 'lucide-react';
import { Card, CardHeader, CardTitle, CardContent, Button } from '@/components/ui';
import { useToast } from '@/components/ui/Toast';
import { useAuthStore } from '@/stores/authStore';

export default function SettingsPage() {
    const toast = useToast();
    const { user } = useAuthStore();
    const [exporting, setExporting] = useState(false);

    const handleExportData = async () => {
        setExporting(true);
        // Simulate export
        await new Promise((resolve) => setTimeout(resolve, 1500));
        toast.success('Xuất dữ liệu thành công', 'File đã được tải xuống');
        setExporting(false);
    };

    const handleImportData = () => {
        toast.info('Chức năng đang phát triển', 'Tính năng import sẽ sớm được cập nhật');
    };

    const handleResetDemo = () => {
        if (confirm('Bạn có chắc muốn reset dữ liệu demo? Tất cả thay đổi sẽ bị mất.')) {
            localStorage.clear();
            window.location.reload();
        }
    };

    return (
        <div className="space-y-6">
            {/* Header */}
            <motion.div
                initial={{ opacity: 0, y: -10 }}
                animate={{ opacity: 1, y: 0 }}
            >
                <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Cài đặt</h1>
                <p className="text-[var(--color-neutral-500)] mt-1">
                    Quản lý cấu hình và dữ liệu hệ thống
                </p>
            </motion.div>

            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                {/* User Info */}
                <motion.div
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.1 }}
                >
                    <Card>
                        <CardHeader>
                            <CardTitle className="flex items-center gap-2">
                                <Shield className="w-5 h-5 text-[var(--color-primary-600)]" />
                                Thông tin tài khoản
                            </CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className="space-y-4">
                                <div className="flex items-center gap-4">
                                    <div className="w-16 h-16 bg-[var(--color-primary-100)] rounded-full flex items-center justify-center">
                                        <Users className="w-8 h-8 text-[var(--color-primary-600)]" />
                                    </div>
                                    <div>
                                        <h3 className="font-semibold text-lg">{user?.fullName}</h3>
                                        <p className="text-sm text-[var(--color-neutral-500)]">{user?.email}</p>
                                    </div>
                                </div>
                                <div className="grid grid-cols-2 gap-4 pt-4 border-t border-[var(--color-neutral-200)]">
                                    <div>
                                        <label className="text-xs text-[var(--color-neutral-500)]">Vai trò</label>
                                        <p className="font-medium">{user?.role}</p>
                                    </div>
                                    <div>
                                        <label className="text-xs text-[var(--color-neutral-500)]">Đơn vị</label>
                                        <p className="font-medium">{user?.unitName}</p>
                                    </div>
                                </div>
                            </div>
                        </CardContent>
                    </Card>
                </motion.div>

                {/* Data Management */}
                <motion.div
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.2 }}
                >
                    <Card>
                        <CardHeader>
                            <CardTitle className="flex items-center gap-2">
                                <Database className="w-5 h-5 text-[var(--color-primary-600)]" />
                                Quản lý dữ liệu
                            </CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className="space-y-3">
                                <Button
                                    variant="secondary"
                                    className="w-full justify-start"
                                    icon={<Download className="w-4 h-4" />}
                                    onClick={handleExportData}
                                    loading={exporting}
                                >
                                    Xuất dữ liệu (JSON)
                                </Button>
                                <Button
                                    variant="secondary"
                                    className="w-full justify-start"
                                    icon={<Upload className="w-4 h-4" />}
                                    onClick={handleImportData}
                                >
                                    Nhập dữ liệu
                                </Button>
                                <Button
                                    variant="danger"
                                    className="w-full justify-start"
                                    icon={<RefreshCw className="w-4 h-4" />}
                                    onClick={handleResetDemo}
                                >
                                    Reset dữ liệu Demo
                                </Button>
                            </div>
                        </CardContent>
                    </Card>
                </motion.div>

                {/* System Info */}
                <motion.div
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: 0.3 }}
                    className="lg:col-span-2"
                >
                    <Card>
                        <CardHeader>
                            <CardTitle className="flex items-center gap-2">
                                <SettingsIcon className="w-5 h-5 text-[var(--color-primary-600)]" />
                                Thông tin hệ thống
                            </CardTitle>
                        </CardHeader>
                        <CardContent>
                            <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
                                <div>
                                    <label className="text-xs text-[var(--color-neutral-500)]">Phiên bản</label>
                                    <p className="font-medium">v3.4.2</p>
                                </div>
                                <div>
                                    <label className="text-xs text-[var(--color-neutral-500)]">Frontend</label>
                                    <p className="font-medium">React 18 + Vite</p>
                                </div>
                                <div>
                                    <label className="text-xs text-[var(--color-neutral-500)]">Backend</label>
                                    <p className="font-medium">C# .NET (Planned)</p>
                                </div>
                                <div>
                                    <label className="text-xs text-[var(--color-neutral-500)]">Database</label>
                                    <p className="font-medium">MS SQL Server</p>
                                </div>
                            </div>
                        </CardContent>
                    </Card>
                </motion.div>
            </div>
        </div>
    );
}
