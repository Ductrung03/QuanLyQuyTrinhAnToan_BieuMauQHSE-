'use client';

interface SkeletonProps {
    className?: string;
    width?: string;
    height?: string;
}

export function Skeleton({ className = '', width, height }: SkeletonProps) {
    return (
        <div
            className={`animate-pulse bg-gray-200 rounded ${className}`}
            style={{ width, height }}
        />
    );
}

export function SkeletonText({ lines = 3, className = '' }: { lines?: number; className?: string }) {
    return (
        <div className={`space-y-2 ${className}`}>
            {Array.from({ length: lines }).map((_, i) => (
                <Skeleton
                    key={i}
                    className="h-4"
                    width={i === lines - 1 ? '70%' : '100%'}
                />
            ))}
        </div>
    );
}

export function SkeletonCard({ className = '' }: { className?: string }) {
    return (
        <div className={`bg-white rounded-xl border border-gray-200 p-6 ${className}`}>
            <div className="flex items-center gap-4 mb-4">
                <Skeleton className="w-12 h-12 rounded-full" />
                <div className="flex-1">
                    <Skeleton className="h-5 w-3/4 mb-2" />
                    <Skeleton className="h-4 w-1/2" />
                </div>
            </div>
            <SkeletonText lines={2} />
        </div>
    );
}

export function SkeletonTable({
    rows = 5,
    cols = 4,
    className = '',
}: {
    rows?: number;
    cols?: number;
    className?: string;
}) {
    return (
        <div className={`bg-white rounded-xl border border-gray-200 overflow-hidden ${className}`}>
            {/* Header */}
            <div className="flex gap-4 p-4 bg-gray-50 border-b border-gray-200">
                {Array.from({ length: cols }).map((_, i) => (
                    <Skeleton key={i} className="h-4 flex-1" />
                ))}
            </div>
            {/* Rows */}
            {Array.from({ length: rows }).map((_, rowIdx) => (
                <div
                    key={rowIdx}
                    className="flex gap-4 p-4 border-b border-gray-100 last:border-b-0"
                >
                    {Array.from({ length: cols }).map((_, colIdx) => (
                        <Skeleton key={colIdx} className="h-4 flex-1" />
                    ))}
                </div>
            ))}
        </div>
    );
}

export function PageLoading({ message = 'Đang tải...' }: { message?: string }) {
    return (
        <div className="flex flex-col items-center justify-center min-h-[400px] gap-4">
            <div className="w-10 h-10 border-4 border-blue-600 border-t-transparent rounded-full animate-spin" />
            <p className="text-gray-500 text-sm">{message}</p>
        </div>
    );
}

export function LoadingSpinner({ size = 'md' }: { size?: 'sm' | 'md' | 'lg' }) {
    const sizeClasses = {
        sm: 'w-4 h-4 border-2',
        md: 'w-6 h-6 border-2',
        lg: 'w-8 h-8 border-4',
    };

    return (
        <div
            className={`${sizeClasses[size]} border-blue-600 border-t-transparent rounded-full animate-spin`}
        />
    );
}
