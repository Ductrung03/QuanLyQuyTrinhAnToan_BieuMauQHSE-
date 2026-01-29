import { clsx } from 'clsx';
import { FileBox, Inbox, X } from 'lucide-react';
import Button from './Button';

interface EmptyStateProps {
    title: string;
    description?: string;
    icon?: 'inbox' | 'file' | 'custom';
    customIcon?: React.ReactNode;
    action?: {
        label: string;
        onClick: () => void;
    };
    className?: string;
}

export default function EmptyState({
    title,
    description,
    icon = 'inbox',
    customIcon,
    action,
    className,
}: EmptyStateProps) {
    const icons = {
        inbox: <Inbox className="w-12 h-12" />,
        file: <FileBox className="w-12 h-12" />,
        custom: customIcon || <X className="w-12 h-12" />,
    };

    return (
        <div className={clsx('flex flex-col items-center justify-center py-12 px-4', className)}>
            <div className="text-[var(--color-neutral-300)] mb-4">
                {icons[icon]}
            </div>
            <h3 className="text-lg font-medium text-[var(--color-neutral-700)] mb-1">{title}</h3>
            {description && (
                <p className="text-sm text-[var(--color-neutral-500)] text-center max-w-sm mb-4">
                    {description}
                </p>
            )}
            {action && (
                <Button variant="primary" size="sm" onClick={action.onClick}>
                    {action.label}
                </Button>
            )}
        </div>
    );
}
