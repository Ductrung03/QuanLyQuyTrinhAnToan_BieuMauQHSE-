'use client';

import { ReactNode } from 'react';
import { Inbox, FileX, Search, AlertCircle } from 'lucide-react';

interface EmptyStateProps {
    icon?: ReactNode;
    title: string;
    description?: string;
    action?: ReactNode;
    variant?: 'default' | 'search' | 'error' | 'inbox';
}

const variantIcons = {
    default: Inbox,
    search: Search,
    error: AlertCircle,
    inbox: FileX,
};

export function EmptyState({
    icon,
    title,
    description,
    action,
    variant = 'default',
}: EmptyStateProps) {
    const Icon = variantIcons[variant];

    return (
        <div className="flex flex-col items-center justify-center py-12 px-4 text-center">
            <div className="w-16 h-16 bg-gray-100 rounded-full flex items-center justify-center mb-4">
                {icon || <Icon className="w-8 h-8 text-gray-400" />}
            </div>
            <h3 className="text-lg font-semibold text-gray-900 mb-1">{title}</h3>
            {description && (
                <p className="text-gray-500 text-sm max-w-sm mb-4">{description}</p>
            )}
            {action && <div className="mt-2">{action}</div>}
        </div>
    );
}

// Confirmation Dialog Component
interface ConfirmDialogProps {
    isOpen: boolean;
    onClose: () => void;
    onConfirm: () => void;
    title: string;
    message: string;
    confirmText?: string;
    cancelText?: string;
    variant?: 'danger' | 'warning' | 'info';
    isLoading?: boolean;
}

export function ConfirmDialog({
    isOpen,
    onClose,
    onConfirm,
    title,
    message,
    confirmText = 'Xác nhận',
    cancelText = 'Hủy',
    variant = 'danger',
    isLoading = false,
}: ConfirmDialogProps) {
    if (!isOpen) return null;

    const variantStyles = {
        danger: 'bg-red-600 hover:bg-red-700',
        warning: 'bg-yellow-600 hover:bg-yellow-700',
        info: 'bg-blue-600 hover:bg-blue-700',
    };

    return (
        <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
            <div className="bg-white rounded-xl shadow-xl max-w-md w-full p-6 animate-in fade-in zoom-in-95 duration-200">
                <h3 className="text-lg font-bold text-gray-900 mb-2">{title}</h3>
                <p className="text-gray-600 mb-6">{message}</p>
                <div className="flex justify-end gap-2">
                    <button
                        onClick={onClose}
                        disabled={isLoading}
                        className="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg disabled:opacity-50"
                    >
                        {cancelText}
                    </button>
                    <button
                        onClick={onConfirm}
                        disabled={isLoading}
                        className={`px-4 py-2 text-white rounded-lg ${variantStyles[variant]} disabled:opacity-50`}
                    >
                        {isLoading ? 'Đang xử lý...' : confirmText}
                    </button>
                </div>
            </div>
        </div>
    );
}

// Badge Component
interface BadgeProps {
    children: ReactNode;
    variant?: 'default' | 'success' | 'warning' | 'danger' | 'info';
    size?: 'sm' | 'md';
}

export function Badge({ children, variant = 'default', size = 'md' }: BadgeProps) {
    const variantStyles = {
        default: 'bg-gray-100 text-gray-700 border-gray-200',
        success: 'bg-green-50 text-green-700 border-green-200',
        warning: 'bg-yellow-50 text-yellow-700 border-yellow-200',
        danger: 'bg-red-50 text-red-700 border-red-200',
        info: 'bg-blue-50 text-blue-700 border-blue-200',
    };

    const sizeStyles = {
        sm: 'px-2 py-0.5 text-xs',
        md: 'px-2.5 py-1 text-xs',
    };

    return (
        <span
            className={`inline-flex items-center rounded-full border font-medium ${variantStyles[variant]} ${sizeStyles[size]}`}
        >
            {children}
        </span>
    );
}

// Status Indicator
interface StatusIndicatorProps {
    status: 'online' | 'offline' | 'busy' | 'away';
    size?: 'sm' | 'md' | 'lg';
}

export function StatusIndicator({ status, size = 'md' }: StatusIndicatorProps) {
    const statusColors = {
        online: 'bg-green-500',
        offline: 'bg-gray-400',
        busy: 'bg-red-500',
        away: 'bg-yellow-500',
    };

    const sizeStyles = {
        sm: 'w-2 h-2',
        md: 'w-3 h-3',
        lg: 'w-4 h-4',
    };

    return (
        <span
            className={`inline-block rounded-full ${statusColors[status]} ${sizeStyles[size]}`}
            aria-label={status}
        />
    );
}
