import { forwardRef, InputHTMLAttributes } from 'react';
import { clsx } from 'clsx';

export interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
    label?: string;
    error?: string;
    icon?: React.ReactNode;
    rightIcon?: React.ReactNode;
    onRightIconClick?: () => void;
    rightIconAriaLabel?: string;
}

const Input = forwardRef<HTMLInputElement, InputProps>(
    ({ className, label, error, icon, rightIcon, onRightIconClick, rightIconAriaLabel, id, ...props }, ref) => {
        const inputId = id || label?.toLowerCase().replace(/\s/g, '-');

        return (
            <div className="w-full">
                {label && (
                    <label
                        htmlFor={inputId}
                        className="block text-sm font-medium text-[var(--color-neutral-700)] mb-1.5"
                    >
                        {label}
                    </label>
                )}
                <div className="relative">
                    {icon && (
                        <div className="absolute left-3 top-1/2 -translate-y-1/2 text-[var(--color-neutral-400)]">
                            {icon}
                        </div>
                    )}
                    <input
                        ref={ref}
                        id={inputId}
                        className={clsx(
                            'w-full px-3 py-2 text-sm rounded-[var(--radius-md)] border transition-colors duration-200',
                            'bg-white text-[var(--color-neutral-900)]',
                            'placeholder:text-[var(--color-neutral-400)]',
                            'focus:outline-none focus:ring-2 focus:ring-sky-500 focus:border-transparent',
                            'disabled:bg-[var(--color-neutral-100)] disabled:cursor-not-allowed',
                            error
                                ? 'border-[var(--color-danger-500)] focus:ring-[var(--color-danger-500)]'
                                : 'border-[var(--color-neutral-300)] hover:border-[var(--color-neutral-400)]',
                            icon && 'pl-10',
                            rightIcon && 'pr-10',
                            className
                        )}
                        {...props}
                    />
                    {rightIcon && (
                        <button
                            type="button"
                            onClick={onRightIconClick}
                            aria-label={rightIconAriaLabel}
                            className="absolute right-3 top-1/2 -translate-y-1/2 text-[var(--color-neutral-400)] hover:text-[var(--color-neutral-600)] transition-colors"
                            disabled={props.disabled}
                        >
                            {rightIcon}
                        </button>
                    )}
                </div>
                {error && (
                    <p className="mt-1.5 text-xs text-[var(--color-danger-600)]">{error}</p>
                )}
            </div>
        );
    }
);

Input.displayName = 'Input';

export default Input;
