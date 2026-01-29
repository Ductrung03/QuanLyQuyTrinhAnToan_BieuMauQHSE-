import { useState, useEffect } from 'react';
import { Plus, Edit, Trash2, Shield, Search } from 'lucide-react';
import { apiClient, Role, PermissionGroup, Permission } from '@/api/client';
import { Card, Button, Input, Table, Modal, Badge } from '@/components/ui';
import { useToast } from '@/components/ui/Toast';
import { useForm } from 'react-hook-form';
import { z } from 'zod';
import { zodResolver } from '@hookform/resolvers/zod';

const roleSchema = z.object({
    name: z.string().min(1, 'Tên vai trò là bắt buộc'),
    description: z.string().optional(),
});

type RoleFormData = z.infer<typeof roleSchema>;

export default function RolesPage() {
    const [roles, setRoles] = useState<Role[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingRole, setEditingRole] = useState<Role | null>(null);
    const [permissions, setPermissions] = useState<PermissionGroup[]>([]);
    const [selectedPermissions, setSelectedPermissions] = useState<number[]>([]);
    const [loadingPermissions, setLoadingPermissions] = useState(false);
    
    const toast = useToast();
    
    const { register, handleSubmit, reset, formState: { errors }, setValue } = useForm<RoleFormData>({
        resolver: zodResolver(roleSchema),
    });

    useEffect(() => {
        fetchRoles();
        fetchPermissions();
    }, []);

    const fetchRoles = async () => {
        try {
            setLoading(true);
            const response = await apiClient.getRoles();
            if (response.success && response.data) {
                setRoles(Array.isArray(response.data) ? response.data : []);
            } else {
                setRoles([]);
                toast.error('Lỗi', response.message || 'Không thể tải danh sách vai trò');
            }
        } catch (error) {
            setRoles([]);
            toast.error('Lỗi', 'Lỗi kết nối server');
        } finally {
            setLoading(false);
        }
    };

    const fetchPermissions = async () => {
        try {
            const response = await apiClient.getPermissionsGrouped();
            if (response.success && response.data) {
                setPermissions(Array.isArray(response.data) ? response.data : []);
            } else {
                setPermissions([]);
                toast.error('Lỗi', response.message || 'Không thể tải danh sách quyền');
            }
        } catch (error) {
            setPermissions([]);
            toast.error('Lỗi', 'Không thể tải danh sách quyền');
        }
    };

    const fetchRolePermissions = async (roleId: number) => {
        try {
            setLoadingPermissions(true);
            const response = await apiClient.getRolePermissions(roleId);
            if (response.success && response.data) {
                const permissionIds = Array.isArray(response.data) 
                    ? response.data.map(p => p.id) 
                    : [];
                setSelectedPermissions(permissionIds);
            } else {
                setSelectedPermissions([]);
            }
        } catch (error) {
            setSelectedPermissions([]);
            toast.error('Lỗi', 'Không thể tải quyền của vai trò');
        } finally {
            setLoadingPermissions(false);
        }
    };

    const handleCreate = () => {
        setEditingRole(null);
        setSelectedPermissions([]);
        reset({ name: '', description: '' });
        setIsModalOpen(true);
    };

    const handleEdit = async (role: Role) => {
        setEditingRole(role);
        setValue('name', role.name);
        setValue('description', role.description || '');
        await fetchRolePermissions(role.id);
        setIsModalOpen(true);
    };

    const handleDelete = async (role: Role) => {
        if (role.isSystemRole) {
            toast.error('Lỗi', 'Không thể xóa vai trò hệ thống');
            return;
        }

        if (!window.confirm(`Bạn có chắc chắn muốn xóa vai trò "${role.name}"?`)) return;

        try {
            const response = await apiClient.deleteRole(role.id);
            if (response.success) {
                toast.success('Thành công', 'Đã xóa vai trò');
                fetchRoles();
            } else {
                toast.error('Lỗi', response.message || 'Không thể xóa vai trò');
            }
        } catch (error) {
            toast.error('Lỗi', 'Lỗi khi xóa vai trò');
        }
    };

    const onSubmit = async (data: RoleFormData) => {
        try {
            let response;
            if (editingRole) {
                response = await apiClient.updateRole(editingRole.id, data);
            } else {
                response = await apiClient.createRole(data);
            }

            if (response.success && response.data) {
                const roleId = response.data.id;
                const permissionIds = Array.from(new Set(selectedPermissions))
                    .filter((id) => Number.isInteger(id) && id > 0);
                if (permissionIds.length === 0) {
                    toast.error('Lỗi', 'Vui lòng chọn ít nhất 1 quyền');
                    return;
                }
                const permResponse = await apiClient.assignPermissions(roleId, permissionIds);
                
                if (permResponse.success) {
                    toast.success('Thành công', editingRole ? 'Đã cập nhật vai trò' : 'Đã tạo vai trò mới');
                    fetchRoles();
                    setIsModalOpen(false);
                } else {
                    toast.warning('Cảnh báo', 'Đã lưu vai trò nhưng lỗi khi cập nhật quyền');
                }
            } else {
                toast.error('Lỗi', response.message || 'Có lỗi xảy ra');
            }
        } catch (error) {
            toast.error('Lỗi', 'Lỗi kết nối server');
        }
    };

    const togglePermission = (permissionId: number) => {
        setSelectedPermissions(prev => 
            prev.includes(permissionId)
                ? prev.filter(id => id !== permissionId)
                : [...prev, permissionId]
        );
    };

    const toggleModulePermissions = (modulePermissions: Permission[]) => {
        const modulePermissionIds = modulePermissions.map(p => p.id);
        const allSelected = modulePermissionIds.every(id => selectedPermissions.includes(id));
        
        if (allSelected) {
            setSelectedPermissions(prev => prev.filter(id => !modulePermissionIds.includes(id)));
        } else {
            setSelectedPermissions(prev => [...new Set([...prev, ...modulePermissionIds])]);
        }
    };

    const filteredRoles = roles.filter(role => 
        role.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        role.code.toLowerCase().includes(searchTerm.toLowerCase())
    );

    const columns = [
        { key: 'code', header: 'Mã vai trò', className: 'font-mono' },
        { key: 'name', header: 'Tên vai trò', className: 'font-medium' },
        { key: 'description', header: 'Mô tả' },
        { 
            key: 'isSystemRole', 
            header: 'Loại', 
            render: (role: Role) => (
                role.isSystemRole ? 
                <Badge variant="primary">Hệ thống</Badge> : 
                <Badge variant="default">Tùy chỉnh</Badge>
            )
        },
        {
            key: 'actions',
            header: 'Thao tác',
            className: 'text-right',
            render: (role: Role) => (
                <div className="flex justify-end gap-2">
                    <Button variant="ghost" size="sm" onClick={(e) => { e.stopPropagation(); handleEdit(role); }}>
                        <Edit className="w-4 h-4 text-blue-600" />
                    </Button>
                    {!role.isSystemRole && (
                        <Button variant="ghost" size="sm" onClick={(e) => { e.stopPropagation(); handleDelete(role); }}>
                            <Trash2 className="w-4 h-4 text-red-600" />
                        </Button>
                    )}
                </div>
            )
        }
    ];

    return (
        <div className="space-y-6">
            <div className="flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-bold text-[var(--color-neutral-900)]">Quản lý Vai trò</h1>
                    <p className="text-[var(--color-neutral-500)] mt-1">Phân quyền và quản lý vai trò người dùng</p>
                </div>
                <Button onClick={handleCreate}>
                    <Plus className="w-4 h-4 mr-2" />
                    Thêm vai trò
                </Button>
            </div>

            <Card>
                <div className="p-4 border-b border-[var(--color-neutral-200)]">
                    <div className="relative max-w-md">
                        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-[var(--color-neutral-400)]" />
                        <Input 
                            placeholder="Tìm kiếm vai trò..." 
                            className="pl-9"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                    </div>
                </div>
                <Table 
                    columns={columns} 
                    data={filteredRoles} 
                    keyField="id" 
                    loading={loading}
                    emptyMessage="Chưa có vai trò nào"
                />
            </Card>

            <Modal
                isOpen={isModalOpen}
                onClose={() => setIsModalOpen(false)}
                title={editingRole ? 'Chỉnh sửa Vai trò' : 'Thêm Vai trò mới'}
                size="xl"
                footer={
                    <>
                        <Button variant="secondary" onClick={() => setIsModalOpen(false)}>Hủy</Button>
                        <Button onClick={handleSubmit(onSubmit)}>Lưu</Button>
                    </>
                }
            >
                <form className="space-y-6">
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div className="space-y-2">
                            <label className="text-sm font-medium text-[var(--color-neutral-700)]">
                                Tên vai trò <span className="text-red-500">*</span>
                            </label>
                            <Input {...register('name')} error={errors.name?.message} placeholder="VD: Trưởng phòng" />
                        </div>
                        <div className="col-span-full space-y-2">
                            <label className="text-sm font-medium text-[var(--color-neutral-700)]">Mô tả</label>
                            <Input {...register('description')} placeholder="Mô tả chi tiết về vai trò này" />
                        </div>
                    </div>

                    <div className="space-y-4">
                        <div className="flex items-center gap-2 pb-2 border-b border-[var(--color-neutral-200)]">
                            <Shield className="w-5 h-5 text-[var(--color-primary-600)]" />
                            <h3 className="font-semibold text-[var(--color-neutral-900)]">Phân quyền</h3>
                        </div>
                        
                        {loadingPermissions ? (
                            <div className="text-center py-4 text-[var(--color-neutral-500)]">Đang tải quyền...</div>
                        ) : (
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6 max-h-[400px] overflow-y-auto p-1">
                                {permissions.map((group) => {
                                    const allSelected = group.permissions.every(p => selectedPermissions.includes(p.id));
                                    return (
                                        <div key={group.module} className="bg-[var(--color-neutral-50)] p-4 rounded-lg border border-[var(--color-neutral-200)]">
                                            <div className="flex items-center justify-between mb-3">
                                                <h4 className="font-medium text-[var(--color-neutral-900)]">{group.module}</h4>
                                                <Button 
                                                    type="button" 
                                                    variant="ghost" 
                                                    size="sm"
                                                    onClick={() => toggleModulePermissions(group.permissions)}
                                                    className="text-xs"
                                                >
                                                    {allSelected ? 'Bỏ chọn hết' : 'Chọn hết'}
                                                </Button>
                                            </div>
                                            <div className="space-y-2">
                                                {group.permissions.map((perm) => (
                                                    <label key={perm.id} className="flex items-start gap-2 cursor-pointer group">
                                                        <input
                                                            type="checkbox"
                                                            className="mt-1 rounded border-gray-300 text-[var(--color-primary-600)] focus:ring-[var(--color-primary-500)]"
                                                            checked={selectedPermissions.includes(perm.id)}
                                                            onChange={() => togglePermission(perm.id)}
                                                        />
                                                        <div className="text-sm">
                                                            <div className="font-medium text-[var(--color-neutral-700)] group-hover:text-[var(--color-primary-700)] transition-colors">
                                                                {perm.name}
                                                            </div>
                                                            {perm.description && (
                                                                <div className="text-xs text-[var(--color-neutral-500)]">{perm.description}</div>
                                                            )}
                                                        </div>
                                                    </label>
                                                ))}
                                            </div>
                                        </div>
                                    );
                                })}
                            </div>
                        )}
                    </div>
                </form>
            </Modal>
        </div>
    );
}
