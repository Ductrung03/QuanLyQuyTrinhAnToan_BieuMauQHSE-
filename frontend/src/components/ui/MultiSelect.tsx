import { useState, useRef, useEffect } from 'react';
import { clsx } from 'clsx';
import { ChevronDown, Check, X } from 'lucide-react';

export interface MultiSelectOption {
    value: string | number;
    label: string;
}

export interface MultiSelectProps {
    label?: string;
    options: MultiSelectOption[];
    selected: (string | number)[];
    onChange: (selected: (string | number)[]) => void;
    placeholder?: string;
    error?: string;
    disabled?: boolean;
    className?: string;
}

export default function MultiSelect({
    label,
    options,
    selected,
    onChange,
    placeholder = 'Chọn...',
    error,
    disabled,
    className
}: MultiSelectProps) {
    const [isOpen, setIsOpen] = useState(false);
    const containerRef = useRef<HTMLDivElement>(null);

    // Close dropdown when clicking outside
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        if (isOpen) {
            document.addEventListener('mousedown', handleClickOutside);
        }

        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, [isOpen]);

    const toggleOption = (value: string | number) => {
        if (selected.includes(value)) {
            onChange(selected.filter(v => v !== value));
        } else {
            onChange([...selected, value]);
        }
    };

    const removeOption = (value: string | number, e: React.MouseEvent) => {
        e.stopPropagation();
        onChange(selected.filter(v => v !== value));
    };

    const selectedLabels = selected
        .map(val => options.find(opt => opt.value === val)?.label)
        .filter(Boolean);

    return (
        <div className={clsx('w-full', className)} ref={containerRef}>
            {label && (
                <label className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5">
                    {label}
                </label>
            )}

            <div className="relative">
                {/* Trigger Button */}
                <button
                    type="button"
                    onClick={() => !disabled && setIsOpen(!isOpen)}
                    disabled={disabled}
                    className={clsx(
                        'w-full px-3 py-2 pr-10 text-sm text-left rounded-lg border transition-colors duration-200',
                        'bg-white flex items-center gap-2 flex-wrap min-h-[40px]',
                        'focus:outline-none focus:ring-2 focus:ring-[var(--color-primary-500)] focus:border-transparent',
                        'disabled:bg-[var(--color-neutral-100)] disabled:cursor-not-allowed',
                        error
                            ? 'border-[var(--color-danger-500)] focus:ring-[var(--color-danger-500)]'
                            : 'border-[var(--color-neutral-300)] hover:border-[var(--color-neutral-400)]'
                    )}
                >
                    {selected.length === 0 ? (
                        <span className="text-[var(--color-neutral-600)]">{placeholder}</span>
                    ) : (
                        <>
                            {selectedLabels.map((label, index) => {
                                const value = selected[index];
                                return (
                                    <span
                                        key={value}
                                        className="inline-flex items-center gap-1 px-2 py-0.5 rounded-md bg-[var(--color-primary-50)] text-[var(--color-primary-700)] text-xs"
                                    >
                                        {label}
                                        <button
                                            type="button"
                                            onClick={(e) => removeOption(value, e)}
                                            className="hover:text-[var(--color-primary-900)] transition-colors"
                                        >
                                            <X className="w-3 h-3" />
                                        </button>
                                    </span>
                                );
                            })}
                        </>
                    )}
                    <ChevronDown
                        className={clsx(
                            'absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-[var(--color-neutral-500)] transition-transform duration-200',
                            isOpen && 'rotate-180'
                        )}
                    />
                </button>

                {/* Dropdown */}
                {isOpen && (
                    <div className="absolute z-50 w-full mt-1 bg-white border border-[var(--color-neutral-300)] rounded-lg shadow-lg max-h-60 overflow-auto">
                        {options.length === 0 ? (
                            <div className="px-3 py-2 text-sm text-[var(--color-neutral-600)]">
                                Không có tùy chọn
                            </div>
                        ) : (
                            <div className="py-1">
                                {options.map((option) => {
                                    const isSelected = selected.includes(option.value);
                                    return (
                                        <button
                                            key={option.value}
                                            type="button"
                                            onClick={() => toggleOption(option.value)}
                                            className={clsx(
                                                'w-full px-3 py-2 text-sm text-left flex items-center gap-2 transition-colors duration-150',
                                                isSelected
                                                    ? 'bg-[var(--color-primary-50)] text-[var(--color-primary-700)]'
                                                    : 'hover:bg-[var(--color-neutral-50)] text-[var(--color-neutral-800)]'
                                            )}
                                        >
                                            <div
                                                className={clsx(
                                                    'w-4 h-4 rounded border flex items-center justify-center transition-colors duration-150',
                                                    isSelected
                                                        ? 'bg-[var(--color-primary-600)] border-[var(--color-primary-600)]'
                                                        : 'border-[var(--color-neutral-300)]'
                                                )}
                                            >
                                                {isSelected && (
                                                    <Check className="w-3 h-3 text-white" />
                                                )}
                                            </div>
                                            <span>{option.label}</span>
                                        </button>
                                    );
                                })}
                            </div>
                        )}
                    </div>
                )}
            </div>

            {error && (
                <p className="mt-1.5 text-xs text-[var(--color-danger-600)]">{error}</p>
            )}
        </div>
    );
}
