'use client';

import { Component, ReactNode } from 'react';
import { AlertTriangle, RefreshCw } from 'lucide-react';

interface Props {
    children: ReactNode;
    fallback?: ReactNode;
}

interface State {
    hasError: boolean;
    error: Error | null;
}

export class ErrorBoundary extends Component<Props, State> {
    constructor(props: Props) {
        super(props);
        this.state = { hasError: false, error: null };
    }

    static getDerivedStateFromError(error: Error): State {
        return { hasError: true, error };
    }

    componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
        console.error('ErrorBoundary caught an error:', error, errorInfo);
    }

    handleRetry = () => {
        this.setState({ hasError: false, error: null });
    };

    render() {
        if (this.state.hasError) {
            if (this.props.fallback) {
                return this.props.fallback;
            }

            return (
                <div className="min-h-[300px] flex flex-col items-center justify-center p-8 text-center">
                    <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mb-4">
                        <AlertTriangle className="w-8 h-8 text-red-500" />
                    </div>
                    <h2 className="text-xl font-semibold text-gray-900 mb-2">
                        Đã xảy ra lỗi
                    </h2>
                    <p className="text-gray-600 mb-6 max-w-md">
                        Có lỗi xảy ra khi tải trang này. Vui lòng thử lại hoặc liên hệ hỗ trợ
                        nếu vấn đề tiếp tục.
                    </p>
                    <button
                        onClick={this.handleRetry}
                        className="flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors"
                    >
                        <RefreshCw className="w-4 h-4" />
                        Thử lại
                    </button>
                    {process.env.NODE_ENV === 'development' && this.state.error && (
                        <details className="mt-6 text-left max-w-lg">
                            <summary className="cursor-pointer text-sm text-gray-500 hover:text-gray-700">
                                Chi tiết lỗi (Development)
                            </summary>
                            <pre className="mt-2 p-4 bg-gray-100 rounded-lg text-xs overflow-auto max-h-48">
                                {this.state.error.toString()}
                                {'\n\n'}
                                {this.state.error.stack}
                            </pre>
                        </details>
                    )}
                </div>
            );
        }

        return this.props.children;
    }
}

// HOC for functional components
export function withErrorBoundary<P extends object>(
    WrappedComponent: React.ComponentType<P>,
    fallback?: ReactNode
) {
    return function WithErrorBoundary(props: P) {
        return (
            <ErrorBoundary fallback={fallback}>
                <WrappedComponent {...props} />
            </ErrorBoundary>
        );
    };
}
