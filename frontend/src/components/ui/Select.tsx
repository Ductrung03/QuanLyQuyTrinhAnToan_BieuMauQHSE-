import { forwardRef, SelectHTMLAttributes } from 'react';
import { clsx } from 'clsx';
import { ChevronDown } from 'lucide-react';

export interface SelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
    label?: string;
    error?: string;
    options: { value: string | number; label: string }[];
    placeholder?: string;
}

const Select = forwardRef<HTMLSelectElement, SelectProps>(
    ({ className, label, error, options, placeholder, id, ...props }, ref) => {
        const selectId = id || label?.toLowerCase().replace(/\s/g, '-');

        return (
            <div className="w-full">
                {label && (
                    <label
                        htmlFor={selectId}
                        className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5"
                    >
                        {label}
                    </label>
                )}
                <div className="relative">
                    <select
                        ref={ref}
                        id={selectId}
                        className={clsx(
                            'w-full px-3 py-2 pr-10 text-sm rounded-lg border appearance-none cursor-pointer transition-colors duration-200',
                            'bg-white text-[var(--color-neutral-900)]',
                            'focus:outline-none focus:ring-2 focus:ring-[var(--color-primary-500)] focus:border-transparent',
                            'disabled:bg-[var(--color-neutral-100)] disabled:cursor-not-allowed',
                            error
                                ? 'border-[var(--color-danger-500)] focus:ring-[var(--color-danger-500)]'
                                : 'border-[var(--color-neutral-300)] hover:border-[var(--color-neutral-400)]',
                            className
                        )}
                        {...props}
                    >
                        {placeholder && (
                            <option value="" disabled>
                                {placeholder}
                            </option>
                        )}
                        {options.map((option) => (
                            <option key={option.value} value={option.value}>
                                {option.label}
                            </option>
                        ))}
                    </select>
                    <ChevronDown className="absolute right-3 top-1/2 -translate-y-1/2 w-4 h-4 text-[var(--color-neutral-400)] pointer-events-none" />
                </div>
                {error && (
                    <p className="mt-1.5 text-xs text-[var(--color-danger-600)]">{error}</p>
                )}
            </div>
        );
    }
);

Select.displayName = 'Select';

export default Select;
