import { clsx } from 'clsx';
import { getStateLabel } from '@/utils/labels';

export interface BadgeProps {
    children: React.ReactNode;
    variant?: 'default' | 'success' | 'warning' | 'danger' | 'primary';
    size?: 'sm' | 'md';
    className?: string;
}

export default function Badge({ children, variant = 'default', size = 'sm', className }: BadgeProps) {
    const variants = {
        default: 'bg-[var(--color-neutral-100)] text-[var(--color-neutral-700)]',
        success: 'bg-[var(--color-success-50)] text-[var(--color-success-600)]',
        warning: 'bg-[var(--color-warning-50)] text-[var(--color-warning-600)]',
        danger: 'bg-[var(--color-danger-50)] text-[var(--color-danger-600)]',
        primary: 'bg-sky-50 text-sky-700',
    };

    const sizes = {
        sm: 'text-xs px-2 py-0.5',
        md: 'text-sm px-2.5 py-1',
    };

    return (
        <span className={clsx('inline-flex items-center font-medium rounded-[var(--radius-sm)]', variants[variant], sizes[size], className)}>
            {children}
        </span>
    );
}

// State-specific badge helper
export function StateBadge({ state }: { state: string }) {
    const stateVariants: Record<string, BadgeProps['variant']> = {
        Draft: 'default',
        Submitted: 'primary',
        'Under Review': 'warning',
        Approved: 'success',
        Rejected: 'danger',
    };

    return <Badge variant={stateVariants[state] || 'default'}>{getStateLabel(state)}</Badge>;
}
