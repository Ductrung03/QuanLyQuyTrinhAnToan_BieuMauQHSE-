import { ButtonHTMLAttributes } from 'react';
import { clsx } from 'clsx';
import { Loader2 } from 'lucide-react';

export interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
    variant?: 'primary' | 'secondary' | 'ghost' | 'danger' | 'success';
    size?: 'sm' | 'md' | 'lg';
    loading?: boolean;
    icon?: React.ReactNode;
}

export default function Button({
    className,
    variant = 'primary',
    size = 'md',
    loading,
    icon,
    children,
    disabled,
    ...props
}: ButtonProps) {
    const baseStyles = 'inline-flex items-center justify-center gap-2 font-medium rounded-[var(--radius-lg)] transition-all duration-200 focus:outline-none focus:ring-2 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed active:scale-[0.98]';

    const variants = {
        primary: 'bg-sky-600 text-white hover:bg-sky-700 focus:ring-sky-500 shadow-sm hover:shadow-md',
        secondary: 'bg-white text-[var(--color-neutral-700)] border border-[var(--color-neutral-300)] hover:bg-[var(--color-neutral-50)] focus:ring-[var(--color-neutral-400)]',
        ghost: 'text-[var(--color-neutral-600)] hover:bg-[var(--color-neutral-100)] focus:ring-[var(--color-neutral-400)]',
        danger: 'bg-[var(--color-danger-600)] text-white hover:bg-[var(--color-danger-700)] focus:ring-[var(--color-danger-500)] shadow-sm hover:shadow-md',
        success: 'bg-[var(--color-success-600)] text-white hover:bg-[var(--color-success-700)] focus:ring-[var(--color-success-500)] shadow-sm hover:shadow-md',
    };

    const sizes = {
        sm: 'text-xs px-3 py-1.5',
        md: 'text-sm px-4 py-2',
        lg: 'text-base px-6 py-3',
    };

    return (
        <button
            className={clsx(
                baseStyles,
                variants[variant],
                sizes[size],
                'hover:scale-[1.02] active:scale-[0.98]',
                className
            )}
            disabled={disabled || loading}
            {...props}
        >
            {loading ? (
                <Loader2 className="w-4 h-4 animate-spin" />
            ) : icon ? (
                icon
            ) : null}
            {children}
        </button>
    );
}
