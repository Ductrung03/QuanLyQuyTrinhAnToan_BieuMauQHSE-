/* eslint-disable @typescript-eslint/no-explicit-any */
import { clsx } from 'clsx';
import { motion } from 'framer-motion';

interface Column<T> {
    key: string;
    header: string;
    render?: (item: T) => React.ReactNode;
    className?: string;
}

interface TableProps<T> {
    columns: Column<T>[];
    data: T[];
    keyField: keyof T;
    onRowClick?: (item: T) => void;
    selectedId?: string | number | null;
    emptyMessage?: string;
    loading?: boolean;
}

export default function Table<T>({
    columns,
    data,
    keyField,
    onRowClick,
    selectedId,
    emptyMessage = 'Không có dữ liệu',
    loading = false,
}: TableProps<T>) {
    if (loading) {
        return (
            <div className="w-full overflow-x-auto">
                <table className="w-full">
                    <thead className="bg-[var(--color-neutral-50)] border-b border-[var(--color-neutral-200)]">
                        <tr>
                            {columns.map((col) => (
                                <th
                                    key={col.key}
                                    className={clsx(
                                        'px-4 py-3 text-left text-xs font-medium text-[var(--color-neutral-500)] uppercase tracking-wider',
                                        col.className
                                    )}
                                >
                                    {col.header}
                                </th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {[1, 2, 3, 4, 5].map((i) => (
                            <tr key={i} className="border-b border-[var(--color-neutral-100)]">
                                {columns.map((col) => (
                                    <td key={col.key} className="px-4 py-3">
                                        <div className="h-4 skeleton rounded w-3/4"></div>
                                    </td>
                                ))}
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        );
    }

    if (data.length === 0) {
        return (
            <div className="text-center py-12 text-[var(--color-neutral-500)]">
                {emptyMessage}
            </div>
        );
    }

    return (
        <div className="w-full overflow-x-auto">
            <table className="w-full">
                <thead className="bg-[var(--color-neutral-50)] border-b border-[var(--color-neutral-200)]">
                    <tr>
                        {columns.map((col) => (
                            <th
                                key={col.key}
                                className={clsx(
                                    'px-4 py-3 text-left text-xs font-medium text-[var(--color-neutral-500)] uppercase tracking-wider',
                                    col.className
                                )}
                            >
                                {col.header}
                            </th>
                        ))}
                    </tr>
                </thead>
                <tbody className="divide-y divide-[var(--color-neutral-100)]">
                    {data.map((item, index) => {
                        const id = (item as any)[keyField] as string | number;
                        const isSelected = selectedId === id;

                        return (
                            <motion.tr
                                key={id}
                                initial={{ opacity: 0, y: 5 }}
                                animate={{ opacity: 1, y: 0 }}
                                transition={{ delay: index * 0.03 }}
                                onClick={() => onRowClick?.(item)}
                                className={clsx(
                                    'transition-colors duration-150',
                                    onRowClick && 'cursor-pointer hover:bg-[var(--color-neutral-50)]',
                                    isSelected && 'bg-[var(--color-primary-50)]'
                                )}
                            >
                                {columns.map((col) => (
                                    <td key={col.key} className={clsx('px-4 py-3 text-sm', col.className)}>
                                        {col.render
                                            ? col.render(item)
                                            : ((item as any)[col.key] as React.ReactNode) ?? '-'}
                                    </td>
                                ))}
                            </motion.tr>
                        );
                    })}
                </tbody>
            </table>
        </div>
    );
}
