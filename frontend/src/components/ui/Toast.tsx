import { create } from 'zustand';
import { motion, AnimatePresence } from 'framer-motion';
import { CheckCircle2, XCircle, AlertCircle, Info, X } from 'lucide-react';
import { clsx } from 'clsx';
import { useEffect } from 'react';

// Types
export interface Toast {
    id: string;
    type: 'success' | 'error' | 'warning' | 'info';
    title: string;
    message?: string;
    duration?: number;
}

// Store
interface ToastStore {
    toasts: Toast[];
    addToast: (toast: Omit<Toast, 'id'>) => void;
    removeToast: (id: string) => void;
}

// eslint-disable-next-line react-refresh/only-export-components
export const useToastStore = create<ToastStore>((set) => ({
    toasts: [],
    addToast: (toast) =>
        set((state) => ({
            toasts: [
                ...state.toasts,
                { ...toast, id: Math.random().toString(36).substr(2, 9) },
            ],
        })),
    removeToast: (id) =>
        set((state) => ({
            toasts: state.toasts.filter((t) => t.id !== id),
        })),
}));

// Hook
// eslint-disable-next-line react-refresh/only-export-components
export function useToast() {
    const { addToast } = useToastStore();

    return {
        success: (title: string, message?: string) =>
            addToast({ type: 'success', title, message, duration: 4000 }),
        error: (title: string, message?: string) =>
            addToast({ type: 'error', title, message, duration: 5000 }),
        warning: (title: string, message?: string) =>
            addToast({ type: 'warning', title, message, duration: 4000 }),
        info: (title: string, message?: string) =>
            addToast({ type: 'info', title, message, duration: 4000 }),
    };
}

// Toast Item Component
function ToastItem({ toast }: { toast: Toast }) {
    const { removeToast } = useToastStore();

    useEffect(() => {
        if (toast.duration) {
            const timer = setTimeout(() => removeToast(toast.id), toast.duration);
            return () => clearTimeout(timer);
        }
    }, [toast.id, toast.duration, removeToast]);

    const icons = {
        success: <CheckCircle2 className="w-5 h-5 text-[var(--color-success-500)]" />,
        error: <XCircle className="w-5 h-5 text-[var(--color-danger-500)]" />,
        warning: <AlertCircle className="w-5 h-5 text-[var(--color-warning-500)]" />,
        info: <Info className="w-5 h-5 text-[var(--color-primary-500)]" />,
    };

    const backgrounds = {
        success: 'border-l-[var(--color-success-500)]',
        error: 'border-l-[var(--color-danger-500)]',
        warning: 'border-l-[var(--color-warning-500)]',
        info: 'border-l-[var(--color-primary-500)]',
    };

    return (
        <motion.div
            initial={{ opacity: 0, x: 100, scale: 0.9 }}
            animate={{ opacity: 1, x: 0, scale: 1 }}
            exit={{ opacity: 0, x: 100, scale: 0.9 }}
            transition={{ duration: 0.2 }}
            className={clsx(
                'flex items-start gap-3 p-4 bg-white rounded-xl border border-[var(--color-neutral-200)] border-l-4',
                backgrounds[toast.type]
            )}
        >
            {icons[toast.type]}
            <div className="flex-1 min-w-0">
                <h4 className="text-sm font-medium text-[var(--color-neutral-900)]">{toast.title}</h4>
                {toast.message && (
                    <p className="mt-1 text-sm text-[var(--color-neutral-500)]">{toast.message}</p>
                )}
            </div>
            <button
                onClick={() => removeToast(toast.id)}
                className="text-[var(--color-neutral-400)] hover:text-[var(--color-neutral-600)] transition-colors"
            >
                <X className="w-4 h-4" />
            </button>
        </motion.div>
    );
}

// Toast Container Component
export function ToastContainer() {
    const { toasts } = useToastStore();

    return (
        <div className="fixed bottom-4 right-4 z-50 flex flex-col gap-2 max-w-sm w-full">
            <AnimatePresence mode="popLayout">
                {toasts.map((toast) => (
                    <ToastItem key={toast.id} toast={toast} />
                ))}
            </AnimatePresence>
        </div>
    );
}
