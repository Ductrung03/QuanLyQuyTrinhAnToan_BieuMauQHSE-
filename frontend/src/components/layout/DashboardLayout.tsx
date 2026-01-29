import { useState, useEffect } from 'react';
import { Outlet, useNavigate } from 'react-router-dom';
import { clsx } from 'clsx';
import Sidebar from './Sidebar';
import Header from './Header';
import { useAuthStore } from '@/stores/authStore';
import { apiClient, Unit } from '@/api/client';
import { ToastContainer } from '@/components/ui/Toast';

export default function DashboardLayout() {
    const navigate = useNavigate();
    const { isAuthenticated, currentUnitId } = useAuthStore();
    const [sidebarOpen, setSidebarOpen] = useState(true);
    const [isMobile, setIsMobile] = useState(false);
    const [units, setUnits] = useState<Unit[]>([]);

    useEffect(() => {
        const checkMobile = () => {
            const mobile = window.innerWidth < 768;
            setIsMobile(mobile);
            if (mobile) {
                setSidebarOpen(false);
            } else {
                setSidebarOpen(true);
            }
        };
        
        checkMobile();
        window.addEventListener('resize', checkMobile);
        return () => window.removeEventListener('resize', checkMobile);
    }, []);

    // Check authentication
    useEffect(() => {
        if (!isAuthenticated) {
            navigate('/login');
        }
    }, [isAuthenticated, navigate]);

    // Load units
    useEffect(() => {
        const loadUnits = async () => {
            const res = await apiClient.getUnits();
            if (res.success && res.data) {
                setUnits(res.data);
            }
        };
        loadUnits();
    }, []);

    const currentUnitName = units.find((u) => u.id === currentUnitId)?.name;

    if (!isAuthenticated) {
        return (
            <div className="min-h-screen flex items-center justify-center">
                <div className="text-[var(--color-neutral-600)]">Đang tải...</div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-slate-50">
            {/* Mobile Overlay */}
            {isMobile && sidebarOpen && (
                <div
                    className="fixed inset-0 bg-black/50 backdrop-blur-sm z-[35]"
                    onClick={() => setSidebarOpen(false)}
                />
            )}

            {/* Sidebar - Fixed position with z-index */}
            <div className={clsx(
                'fixed top-0 left-0 h-screen w-64 z-40 transition-transform duration-300 ease-in-out',
                !sidebarOpen && '-translate-x-full'
            )}>
                <Sidebar units={units} />
            </div>

            {/* Main Content */}
            <div className={clsx(
                'min-h-screen transition-all duration-300',
                sidebarOpen && !isMobile ? 'ml-64' : 'ml-0'
            )}>
                <Header
                    sidebarOpen={sidebarOpen}
                    onToggleSidebar={() => setSidebarOpen(!sidebarOpen)}
                    currentUnitName={currentUnitName}
                />

                {/* Page Content */}
                <main className="p-4 md:p-6">
                    <Outlet />
                </main>
            </div>

            {/* Toast Container */}
            <ToastContainer />
        </div>
    );
}
