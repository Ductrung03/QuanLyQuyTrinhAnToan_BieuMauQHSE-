import { useState, useEffect } from 'react';
import { Edit, Trash2, Search, UserPlus } from 'lucide-react';
import { apiClient, User, Role, Unit } from '@/api/client';
import { Card, Button, Input, Table, Modal, Select, Badge } from '@/components/ui';
import { useToast } from '@/components/ui/Toast';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';

const userSchema = z.object({
    username: z.string().min(3, 'Tên đăng nhập phải có ít nhất 3 ký tự'),
    password: z.string().min(6, 'Mật khẩu phải có ít nhất 6 ký tự').optional().or(z.literal('')),
    fullName: z.string().min(1, 'Họ tên là bắt buộc'),
    email: z.string().email('Email không hợp lệ'),
    roleId: z.string().min(1, 'Vui lòng chọn vai trò'),
    unitId: z.string().min(1, 'Vui lòng chọn đơn vị'),
    isActive: z.boolean().optional(),
});

type UserFormData = z.infer<typeof userSchema>;

export default function UsersPage() {
    const [users, setUsers] = useState<User[]>([]);
    const [roles, setRoles] = useState<Role[]>([]);
    const [units, setUnits] = useState<Unit[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingUser, setEditingUser] = useState<User | null>(null);
    
    const toast = useToast();
    
    const { register, handleSubmit, reset, formState: { errors }, setValue } = useForm<UserFormData>({
        resolver: zodResolver(userSchema),
    });

    useEffect(() => {
        fetchUsers();
        fetchRoles();
        fetchUnits();
    }, []);

    const fetchUsers = async () => {
        try {
            setLoading(true);
            const response = await apiClient.getUsers();
            if (response.success && response.data) {
                setUsers(Array.isArray(response.data) ? response.data : []);
            } else {
                setUsers([]);
                toast.error('Lỗi', response.message || 'Không thể tải danh sách người dùng');
            }
        } catch (error) {
            setUsers([]);
            toast.error('Lỗi', 'Lỗi kết nối server');
        } finally {
            setLoading(false);
        }
    };

    const fetchRoles = async () => {
        const response = await apiClient.getRoles();
        if (response.success && response.data) {
            setRoles(Array.isArray(response.data) ? response.data : []);
        } else {
            setRoles([]);
            toast.error('Lỗi', response.message || 'Không thể tải danh sách vai trò');
        }
    };

    const fetchUnits = async () => {
        const response = await apiClient.getUnits();
        if (response.success && response.data) {
            setUnits(Array.isArray(response.data) ? response.data : []);
        } else {
            setUnits([]);
            toast.error('Lỗi', response.message || 'Không thể tải danh sách đơn vị');
        }
    };

    const handleCreate = () => {
        setEditingUser(null);
        reset({ username: '', password: '', fullName: '', email: '', roleId: '', unitId: '', isActive: true });
        setIsModalOpen(true);
    };

    const handleEdit = (user: User) => {
        setEditingUser(user);
        setValue('username', user.userName);
        setValue('fullName', user.fullName);
        setValue('email', user.email);
        setValue('roleId', user.roleId?.toString() || '');
        setValue('unitId', user.unitId.toString());
        setValue('isActive', user.isActive ?? true);
        setIsModalOpen(true);
    };

    const handleDelete = async (user: User) => {
        if (!window.confirm(`Bạn có chắc chắn muốn xóa người dùng "${user.userName}"?`)) return;

        try {
            const response = await apiClient.deleteUser(user.id);
            if (response.success) {
                toast.success('Thành công', 'Đã xóa người dùng');
                fetchUsers();
            } else {
                toast.error('Lỗi', response.message || 'Không thể xóa người dùng');
            }
        } catch (error) {
            toast.error('Lỗi', 'Lỗi khi xóa người dùng');
        }
    };

    const onSubmit = async (data: UserFormData) => {
        try {
            const payload = {
                ...data,
                roleId: parseInt(data.roleId),
                unitId: parseInt(data.unitId),
            };

            let response;
            if (editingUser) {
                if (!payload.password) delete payload.password;
                response = await apiClient.updateUser(editingUser.id, payload);
            } else {
                if (!payload.password) {
                    toast.error('Lỗi', 'Mật khẩu là bắt buộc khi tạo mới');
                    return;
                }
                response = await apiClient.createUser(payload);
            }

            if (response.success) {
                toast.success('Thành công', editingUser ? 'Đã cập nhật người dùng' : 'Đã tạo người dùng mới');
                fetchUsers();
                setIsModalOpen(false);
            } else {
                toast.error('Lỗi', response.message || 'Có lỗi xảy ra');
            }
        } catch (error) {
            toast.error('Lỗi', 'Lỗi kết nối server');
        }
    };

    const normalizedSearch = (searchTerm || '').toLowerCase();
    const filteredUsers = users.filter(user => {
        if (!user) return false;
        const userName = String(user.userName ?? '').toLowerCase();
        const fullName = String(user.fullName ?? '').toLowerCase();
        const email = String(user.email ?? '').toLowerCase();
        return (
            userName.includes(normalizedSearch) ||
            fullName.includes(normalizedSearch) ||
            email.includes(normalizedSearch)
        );
    });

    const columns = [
        { key: 'userName', header: 'Tên đăng nhập', className: 'font-medium' },
        { key: 'fullName', header: 'Họ và tên' },
        { key: 'email', header: 'Email' },
        { 
            key: 'role', 
            header: 'Vai trò',
            render: (user: User) => (
                <Badge variant="default">{user.role}</Badge>
            )
        },
        { key: 'unitName', header: 'Đơn vị' },
        {
            key: 'isActive',
            header: 'Trạng thái',
            render: (user: User) => (
                <Badge variant={user.isActive ? 'success' : 'default'}>
                    {user.isActive ? 'Hoạt động' : 'Vô hiệu'}
                </Badge>
            )
        },
        {
            key: 'actions',
            header: 'Thao tác',
            className: 'text-right',
            render: (user: User) => (
                <div className="flex justify-end gap-2">
                    <Button variant="ghost" size="sm" onClick={(e) => { e.stopPropagation(); handleEdit(user); }}>
                        <Edit className="w-4 h-4 text-blue-600" />
                    </Button>
                    <Button variant="ghost" size="sm" onClick={(e) => { e.stopPropagation(); handleDelete(user); }}>
                        <Trash2 className="w-4 h-4 text-red-600" />
                    </Button>
                </div>
            )
        }
    ];

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Quản lý Người dùng</h1>
                    <p className="text-[var(--color-neutral-500)] mt-1">Quản lý tài khoản và thông tin người dùng</p>
                </div>
                <Button onClick={handleCreate}>
                    <UserPlus className="w-4 h-4 mr-2" />
                    Thêm người dùng
                </Button>
            </div>

            <Card>
                <div className="p-4 border-b border-[var(--color-neutral-200)]">
                    <div className="relative max-w-md">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-[var(--color-neutral-400)]" />
                        <Input 
                            placeholder="Tìm kiếm người dùng..." 
                            className="pl-9"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                </div>
                <Table 
                    columns={columns} 
                    data={filteredUsers} 
                    keyField="id" 
                    loading={loading}
                    emptyMessage="Chưa có người dùng nào"
                />
            </Card>

            <Modal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                title={editingUser ? 'Chỉnh sửa Người dùng' : 'Thêm Người dùng mới'}
                size="lg"
                footer={
                    <>
                        <Button variant="secondary" onClick={() => setIsModalOpen(false)}>Hủy</Button>
                        <Button onClick={handleSubmit(onSubmit)}>Lưu</Button>
                    </>
                }
            >
                <form className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="space-y-2">
                        <label className="text-sm font-medium text-[var(--color-neutral-700)]">
                            Tên đăng nhập <span className="text-red-500">*</span>
                        </label>
                        <Input 
                            {...register('username')} 
                            error={errors.username?.message} 
                            placeholder="VD: nguyenvana"
                            disabled={!!editingUser} 
                        />
                    </div>
                    <div className="space-y-2">
                        <label className="text-sm font-medium text-[var(--color-neutral-700)]">
                            Mật khẩu {editingUser ? '(Để trống nếu không đổi)' : <span className="text-red-500">*</span>}
                        </label>
                        <Input 
                            type="password"
                            {...register('password')} 
                            error={errors.password?.message}
                            placeholder="******" 
                        />
                    </div>
                    <div className="space-y-2">
                        <label className="text-sm font-medium text-[var(--color-neutral-700)]">
                            Họ và tên <span className="text-red-500">*</span>
                        </label>
                        <Input {...register('fullName')} error={errors.fullName?.message} placeholder="VD: Nguyễn Văn A" />
                    </div>
                    <div className="space-y-2">
                        <label className="text-sm font-medium text-[var(--color-neutral-700)]">
                            Email <span className="text-red-500">*</span>
                        </label>
                        <Input {...register('email')} error={errors.email?.message} placeholder="email@example.com" />
                    </div>
                    <div className="space-y-2">
                        <label className="text-sm font-medium text-[var(--color-neutral-700)]">
                            Vai trò <span className="text-red-500">*</span>
                        </label>
                        <Select 
                            {...register('roleId')} 
                            error={errors.roleId?.message}
                            options={roles.map(role => ({ value: role.id, label: role.name }))}
                            placeholder="Chọn vai trò"
                        />
                    </div>
                    <div className="space-y-2">
                        <label className="text-sm font-medium text-[var(--color-neutral-700)]">
                            Đơn vị <span className="text-red-500">*</span>
                        </label>
                        <Select 
                            {...register('unitId')} 
                            error={errors.unitId?.message}
                            options={units.map(unit => ({ value: unit.id, label: unit.name }))}
                            placeholder="Chọn đơn vị"
                        />
                    </div>
                    {editingUser && (
                        <div className="col-span-full flex items-center gap-2">
                            <input 
                                type="checkbox" 
                                id="isActive"
                                className="rounded border-gray-300 text-[var(--color-primary-600)] focus:ring-[var(--color-primary-500)]"
                                {...register('isActive')}
                            />
                            <label htmlFor="isActive" className="text-sm font-medium text-[var(--color-neutral-700)]">
                                Đang hoạt động
                            </label>
                        </div>
                    )}
                </form>
            </Modal>
        </div>
    );
}
