import { HTMLAttributes } from 'react';
import { clsx } from 'clsx';

export interface CardProps extends HTMLAttributes<HTMLDivElement> {
    variant?: 'default' | 'elevated' | 'bordered';
    padding?: 'none' | 'sm' | 'md' | 'lg';
    hoverable?: boolean;
}

export default function Card({
    className,
    variant = 'default',
    padding = 'md',
    hoverable = false,
    children,
    ...props
}: CardProps) {
    const baseStyles = 'bg-white rounded-[var(--radius-xl)]';

    const variants = {
        default: 'border border-[var(--color-neutral-200)]',
        elevated: 'shadow-md hover:shadow-lg transition-shadow',
        bordered: 'border-2 border-[var(--color-neutral-200)]',
    };

    const paddings = {
        none: '',
        sm: 'p-3',
        md: 'p-4',
        lg: 'p-6',
    };

    const hoverStyles = hoverable
        ? 'transition-all duration-200 hover:shadow-lg hover:border-sky-200 hover:-translate-y-0.5 cursor-pointer'
        : '';

    return (
        <div
            className={clsx(baseStyles, variants[variant], paddings[padding], hoverStyles, className)}
            {...props}
        >
            {children}
        </div>
    );
}

// Card sub-components
export function CardHeader({
    className,
    ...props
}: HTMLAttributes<HTMLDivElement>) {
    return (
        <div
            className={clsx('flex items-center justify-between pb-4 border-b border-[var(--color-neutral-200)]', className)}
            {...props}
        />
    );
}

export function CardTitle({
    className,
    ...props
}: HTMLAttributes<HTMLHeadingElement>) {
    return (
        <h3 className={clsx('text-lg font-semibold text-[var(--color-neutral-900)]', className)} {...props} />
    );
}

export function CardContent({
    className,
    ...props
}: HTMLAttributes<HTMLDivElement>) {
    return (
        <div className={clsx('pt-4', className)} {...props} />
    );
}
