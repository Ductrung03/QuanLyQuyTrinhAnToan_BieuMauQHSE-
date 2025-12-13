'use client';

import { createContext, useContext, useState, useCallback, ReactNode } from 'react';
import { CheckCircle, XCircle, AlertCircle, X, Info } from 'lucide-react';

type ToastType = 'success' | 'error' | 'warning' | 'info';

interface Toast {
    id: string;
    message: string;
    type: ToastType;
    duration?: number;
}

interface ToastContextType {
    showToast: (message: string, type?: ToastType, duration?: number) => void;
    success: (message: string) => void;
    error: (message: string) => void;
    warning: (message: string) => void;
    info: (message: string) => void;
}

const ToastContext = createContext<ToastContextType | undefined>(undefined);

export function useToast() {
    const context = useContext(ToastContext);
    if (!context) {
        throw new Error('useToast must be used within a ToastProvider');
    }
    return context;
}

const toastStyles: Record<ToastType, { bg: string; icon: typeof CheckCircle; iconColor: string }> = {
    success: { bg: 'bg-green-50 border-green-200', icon: CheckCircle, iconColor: 'text-green-500' },
    error: { bg: 'bg-red-50 border-red-200', icon: XCircle, iconColor: 'text-red-500' },
    warning: { bg: 'bg-yellow-50 border-yellow-200', icon: AlertCircle, iconColor: 'text-yellow-500' },
    info: { bg: 'bg-blue-50 border-blue-200', icon: Info, iconColor: 'text-blue-500' },
};

function ToastItem({ toast, onClose }: { toast: Toast; onClose: () => void }) {
    const style = toastStyles[toast.type];
    const Icon = style.icon;

    return (
        <div
            className={`flex items-center gap-3 px-4 py-3 rounded-lg border shadow-lg ${style.bg} animate-slide-in`}
            role="alert"
        >
            <Icon className={`w-5 h-5 flex-shrink-0 ${style.iconColor}`} />
            <p className="text-sm text-gray-800 flex-1">{toast.message}</p>
            <button
                onClick={onClose}
                className="p-1 hover:bg-black/5 rounded transition-colors"
                aria-label="Đóng thông báo"
            >
                <X className="w-4 h-4 text-gray-500" />
            </button>
        </div>
    );
}

export function ToastProvider({ children }: { children: ReactNode }) {
    const [toasts, setToasts] = useState<Toast[]>([]);

    const removeToast = useCallback((id: string) => {
        setToasts((prev) => prev.filter((t) => t.id !== id));
    }, []);

    const showToast = useCallback(
        (message: string, type: ToastType = 'info', duration: number = 4000) => {
            const id = `toast-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
            const newToast: Toast = { id, message, type, duration };

            setToasts((prev) => [...prev, newToast]);

            if (duration > 0) {
                setTimeout(() => {
                    removeToast(id);
                }, duration);
            }
        },
        [removeToast]
    );

    const success = useCallback((message: string) => showToast(message, 'success'), [showToast]);
    const error = useCallback((message: string) => showToast(message, 'error', 6000), [showToast]);
    const warning = useCallback((message: string) => showToast(message, 'warning'), [showToast]);
    const info = useCallback((message: string) => showToast(message, 'info'), [showToast]);

    return (
        <ToastContext.Provider value={{ showToast, success, error, warning, info }}>
            {children}
            {/* Toast Container */}
            <div
                className="fixed top-4 right-4 z-50 flex flex-col gap-2 max-w-sm w-full pointer-events-none"
                aria-live="polite"
            >
                {toasts.map((toast) => (
                    <div key={toast.id} className="pointer-events-auto">
                        <ToastItem toast={toast} onClose={() => removeToast(toast.id)} />
                    </div>
                ))}
            </div>
        </ToastContext.Provider>
    );
}
