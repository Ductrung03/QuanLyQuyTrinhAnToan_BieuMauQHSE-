'use client';

import { ToastProvider } from '@/components/ui/Toast';
import { ErrorBoundary } from '@/components/ui/ErrorBoundary';

export default function ClientProviders({ children }: { children: React.ReactNode }) {
    return (
        <ErrorBoundary>
            <ToastProvider>
                {children}
            </ToastProvider>
        </ErrorBoundary>
    );
}
