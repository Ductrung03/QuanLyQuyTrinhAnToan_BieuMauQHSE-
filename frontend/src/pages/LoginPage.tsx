
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { motion } from 'framer-motion';
import { Mail, Lock, Anchor, ArrowRight, ShieldCheck, Eye, EyeOff } from 'lucide-react';
import { Button, Input } from '@/components/ui';
import { useAuthStore } from '@/stores/authStore';
import { apiClient } from '@/api/client';
import { useToast } from '@/components/ui/Toast';

export default function LoginPage() {
    const navigate = useNavigate();
    const { login } = useAuthStore();
    const toast = useToast();
    const [formData, setFormData] = useState({ loginName: '', password: '' });
    const [loading, setLoading] = useState(false);
    const [errors, setErrors] = useState<{ loginName?: string; password?: string }>({});
    const [showPassword, setShowPassword] = useState(false);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        // Validate
        const newErrors: typeof errors = {};
        if (!formData.loginName) newErrors.loginName = 'Vui lòng nhập tên đăng nhập';
        if (!formData.password) newErrors.password = 'Vui lòng nhập mật khẩu';
        if (Object.keys(newErrors).length > 0) {
            setErrors(newErrors);
            return;
        }

        setLoading(true);
        setErrors({});

        try {
            const response = await apiClient.login(formData.loginName, formData.password);

            if (response.success && response.data) {
                login(response.data.token, response.data.user);
                toast.success('Đăng nhập thành công');
                navigate('/');
            } else {
                toast.error('Đăng nhập thất bại', response.message || 'Sai tên đăng nhập hoặc mật khẩu');
            }
        } catch {
            toast.error('Lỗi kết nối', 'Không thể kết nối đến server');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="w-full min-h-screen flex overflow-hidden">
            {/* Left Side - Image Banner (Full Height) */}
            <div className="hidden lg:block lg:w-[60%] relative bg-slate-900">
                {/* High Quality Maritime Image */}
                <img
                    src="https://images.unsplash.com/photo-1542300058-b94b8ab7411b?q=80&w=2669&auto=format&fit=crop"
                    alt="Maritime Background"
                    className="absolute inset-0 w-full h-full object-cover opacity-90"
                />

                {/* Subtle Gradient Overlay for Text Readability */}
                <div className="absolute inset-0 bg-gradient-to-r from-blue-900/80 to-slate-900/40 mix-blend-multiply" />
                <div className="absolute inset-0 bg-gradient-to-t from-slate-900 via-transparent to-transparent" />

                {/* Brand Content */}
                <div className="absolute bottom-0 left-0 p-16 pb-20 w-full text-white">
                    <motion.div
                        initial={{ opacity: 0, y: 20 }}
                        animate={{ opacity: 1, y: 0 }}
                        transition={{ delay: 0.2, duration: 0.8 }}
                    >
                        <div className="flex items-center gap-3 mb-6">
                            <div className="p-2 bg-white/10 backdrop-blur-sm rounded-lg border border-white/20">
                                <Anchor className="w-6 h-6 text-sky-400" />
                            </div>
                    <span className="text-lg font-semibold tracking-wide text-sky-100">Hệ thống SSMS QHSE</span>
                        </div>

                        <h1 className="text-5xl font-bold leading-tight mb-6 max-w-2xl">
                            Quản lý An toàn & <br />
                            Vận hành Đội tàu
                        </h1>

                        <p className="text-lg text-slate-300 max-w-xl leading-relaxed mb-8">
                            Nền tảng số hóa toàn diện giúp tối ưu hóa quy trình QHSE,
                            đảm bảo tuân thủ tiêu chuẩn hàng hải quốc tế.
                        </p>

                        <div className="flex items-center gap-4 text-sm font-medium text-slate-400">
                            <div className="flex items-center gap-2">
                                <ShieldCheck className="w-5 h-5 text-emerald-400" />
                                <span>ISO 9001:2015</span>
                            </div>
                            <div className="w-1.5 h-1.5 rounded-full bg-slate-600" />
                            <span>Security Certified</span>
                        </div>
                    </motion.div>
                </div>
            </div>

            {/* Right Side - Login Form (Clean White) */}
            <div className="w-full lg:w-[40%] bg-white flex flex-col justify-center px-8 sm:px-12 lg:px-20 xl:px-24 py-12 relative shadow-2xl shadow-slate-200 z-10">

                <div className="max-w-[420px] w-full mx-auto space-y-10">
                    <div className="space-y-2">
                        <div className="w-12 h-12 bg-blue-600 rounded-xl flex items-center justify-center mb-6 shadow-lg shadow-blue-600/20 lg:hidden">
                            <Anchor className="w-6 h-6 text-white" />
                        </div>
                        <h2 className="text-3xl font-bold text-slate-900">Đăng nhập</h2>
                        <p className="text-slate-500">Nhập thông tin tài khoản để tiếp tục</p>
                    </div>

                    <form onSubmit={handleSubmit} className="space-y-6">
                        <div className="space-y-5">
                            <Input
                                label="Tên tài khoản"
                                placeholder="example@domain.com"
                                type="text"
                                className="h-12 bg-slate-50 border-slate-200 focus:bg-white focus:border-blue-500 transition-all font-medium"
                                icon={<Mail className="w-5 h-5 text-slate-400" />}
                                value={formData.loginName}
                                onChange={(e) => setFormData({ ...formData, loginName: e.target.value })}
                                error={errors.loginName}
                                disabled={loading}
                            />

                            <div className="space-y-1.5">
                            <Input
                                label="Mật khẩu"
                                placeholder="••••••••"
                                type={showPassword ? 'text' : 'password'}
                                className="h-12 bg-slate-50 border-slate-200 focus:bg-white focus:border-blue-500 transition-all font-medium"
                                icon={<Lock className="w-5 h-5 text-slate-400" />}
                                rightIcon={showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                                onRightIconClick={() => setShowPassword((prev) => !prev)}
                                rightIconAriaLabel={showPassword ? 'Ẩn mật khẩu' : 'Hiện mật khẩu'}
                                value={formData.password}
                                onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                                error={errors.password}
                                disabled={loading}
                            />
                                <div className="flex justify-end">
                                    <a href="#" className="text-sm font-semibold text-blue-600 hover:text-blue-700 transition-colors">
                                        Quên mật khẩu?
                                    </a>
                                </div>
                            </div>
                        </div>

                        <Button
                            type="submit"
                            size="lg"
                            className="w-full h-12 bg-blue-600 hover:bg-blue-700 text-white font-semibold text-base shadow-lg shadow-blue-600/20 hover:shadow-blue-600/30 transition-all"
                            loading={loading}
                        >
                            <span>Truy cập hệ thống</span>
                            <ArrowRight className="w-5 h-5 ml-2" />
                        </Button>
                    </form>

                    <div className="pt-6 border-t border-slate-100">
                        <p className="text-center text-sm text-slate-500">
                            Cần hỗ trợ kỹ thuật? <a href="#" className="text-blue-700 font-semibold hover:underline">Liên hệ IT</a>
                        </p>
                    </div>
                </div>

                <div className="absolute bottom-6 left-0 w-full text-center">
                <p className="text-xs text-slate-400 font-medium">© 2024 SSMS QHSE v3.4.2</p>
                </div>
            </div>
        </div>
    );
}
