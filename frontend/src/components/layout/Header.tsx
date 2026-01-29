import { Menu, X, Ship } from 'lucide-react';
import { useAuthStore } from '@/stores/authStore';

interface HeaderProps {
    sidebarOpen: boolean;
    onToggleSidebar: () => void;
    currentUnitName?: string;
}

export default function Header({ sidebarOpen, onToggleSidebar, currentUnitName }: HeaderProps) {
    const { user } = useAuthStore();

    return (
        <header className="bg-white border-b border-[var(--color-neutral-200)] sticky top-0 z-30">
            <div className="px-6 py-4 flex items-center justify-between">
                <div className="flex items-center gap-4">
                    <button
                        onClick={onToggleSidebar}
                        className="p-2 rounded-lg hover:bg-[var(--color-neutral-100)] transition-colors"
                    >
                        {sidebarOpen ? (
                            <X className="w-6 h-6 text-[var(--color-neutral-600)]" />
                        ) : (
                            <Menu className="w-6 h-6 text-[var(--color-neutral-600)]" />
                        )}
                    </button>

                    {currentUnitName && (
                        <div className="hidden md:flex items-center gap-2 px-3 py-1 bg-sky-50 text-sky-700 rounded-full text-sm">
                            <Ship className="w-4 h-4" />
                            <span>{currentUnitName}</span>
                        </div>
                    )}
                </div>

                <div className="flex items-center gap-4">
                    <div className="text-right">
                        <p className="text-sm font-medium text-[var(--color-neutral-900)]">{user?.fullName}</p>
                        <p className="text-xs text-[var(--color-neutral-500)]">{user?.email}</p>
                    </div>
                </div>
            </div>
        </header>
    );
}
