import { clsx } from 'clsx';

interface SkeletonProps {
    className?: string;
    variant?: 'text' | 'rectangular' | 'circular';
    width?: string | number;
    height?: string | number;
}

export default function Skeleton({ className, variant = 'text', width, height }: SkeletonProps) {
    const baseStyles = 'skeleton bg-[var(--color-neutral-200)]';

    const variants = {
        text: 'rounded',
        rectangular: 'rounded-lg',
        circular: 'rounded-full',
    };

    const defaultSizes = {
        text: { height: '1rem', width: '100%' },
        rectangular: { height: '4rem', width: '100%' },
        circular: { height: '2.5rem', width: '2.5rem' },
    };

    const style = {
        width: width ?? defaultSizes[variant].width,
        height: height ?? defaultSizes[variant].height,
    };

    return <div className={clsx(baseStyles, variants[variant], className)} style={style} />;
}

// Pre-built skeleton patterns
export function CardSkeleton() {
    return (
        <div className="bg-white rounded-xl border border-[var(--color-neutral-200)] p-4 space-y-3">
            <Skeleton variant="text" width="60%" />
            <Skeleton variant="text" width="80%" />
            <Skeleton variant="text" width="40%" />
        </div>
    );
}

export function TableRowSkeleton({ columns = 4 }: { columns?: number }) {
    return (
        <tr className="border-b border-[var(--color-neutral-100)]">
            {Array.from({ length: columns }).map((_, i) => (
                <td key={i} className="px-4 py-3">
                    <Skeleton variant="text" width={i === 0 ? '30%' : '70%'} />
                </td>
            ))}
        </tr>
    );
}

export function StatCardSkeleton() {
    return (
        <div className="bg-white rounded-xl border border-[var(--color-neutral-200)] p-6 space-y-3">
            <div className="flex items-center justify-between">
                <Skeleton variant="text" width="60%" />
                <Skeleton variant="circular" width={40} height={40} />
            </div>
            <Skeleton variant="text" width="40%" height="2rem" />
            <Skeleton variant="text" width="50%" height="0.75rem" />
        </div>
    );
}
