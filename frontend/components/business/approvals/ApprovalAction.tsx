"use client";

import { useState } from "react";
import { apiClient } from "@/lib/api-client";
import { Check, X, Loader2 } from "lucide-react";
import { useRouter } from "next/navigation";
import { useToast } from "@/components/ui/Toast";

interface ApprovalActionProps {
    submissionId: number;
    onSuccess?: () => void;
}

export default function ApprovalAction({ submissionId, onSuccess }: ApprovalActionProps) {
    const router = useRouter();
    const toast = useToast();
    const [loading, setLoading] = useState(false);
    const [showModal, setShowModal] = useState(false);
    const [actionType, setActionType] = useState<"approve" | "reject" | null>(null);
    const [note, setNote] = useState("");

    const handleActionClick = (type: "approve" | "reject") => {
        setActionType(type);
        setShowModal(true);
        setNote("");
    };

    const calculateActionLabel = () => {
        return actionType === "approve" ? "Phê duyệt" : "Từ chối";
    };

    const handleSubmit = async () => {
        if (!actionType) return;

        // Validate note for rejection
        if (actionType === "reject" && !note.trim()) {
            toast.warning("Vui lòng nhập lý do từ chối");
            return;
        }

        try {
            setLoading(true);
            const res = await (actionType === "approve"
                ? apiClient.approveSubmission(submissionId, note)
                : apiClient.rejectSubmission(submissionId, note));

            if (res.success) {
                toast.success(actionType === "approve" ? "Đã phê duyệt thành công" : "Đã từ chối thành công");
                setShowModal(false);
                if (onSuccess) {
                    onSuccess();
                } else {
                    router.refresh();
                }
            } else {
                toast.error(res.message || "Có lỗi xảy ra");
            }
        } catch (error) {
            console.error(error);
            toast.error("Lỗi kết nối server");
        } finally {
            setLoading(false);
        }
    };

    return (
        <>
            <div className="flex gap-2">
                <button
                    onClick={() => handleActionClick("approve")}
                    className="flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded-lg hover:bg-green-700 transition"
                >
                    <Check className="w-4 h-4" />
                    Phê duyệt
                </button>
                <button
                    onClick={() => handleActionClick("reject")}
                    className="flex items-center gap-2 px-4 py-2 bg-red-600 text-white rounded-lg hover:bg-red-700 transition"
                >
                    <X className="w-4 h-4" />
                    Từ chối
                </button>
            </div>

            {showModal && (
                <div className="fixed inset-0 bg-black/50 z-50 flex items-center justify-center p-4">
                    <div className="bg-white rounded-xl shadow-xl max-w-md w-full p-6 animate-in fade-in zoom-in-95 duration-200">
                        <h3 className={`text-lg font-bold mb-4 ${actionType === 'approve' ? 'text-green-700' : 'text-red-700'}`}>
                            Xác nhận {calculateActionLabel()}
                        </h3>

                        <div className="mb-4">
                            <label className="block text-sm font-medium text-gray-700 mb-1">
                                {actionType === 'approve' ? 'Ghi chú (tùy chọn)' : 'Lý do từ chối (*)'}
                            </label>
                            <textarea
                                value={note}
                                onChange={(e) => setNote(e.target.value)}
                                className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500"
                                rows={3}
                                placeholder={actionType === 'approve' ? 'Nhập ghi chú...' : 'Nhập lý do từ chối...'}
                            />
                        </div>

                        <div className="flex justify-end gap-2">
                            <button
                                onClick={() => setShowModal(false)}
                                className="px-4 py-2 text-gray-600 hover:bg-gray-100 rounded-lg"
                                disabled={loading}
                            >
                                Hủy
                            </button>
                            <button
                                onClick={handleSubmit}
                                className={`px-4 py-2 text-white rounded-lg flex items-center gap-2 ${actionType === 'approve'
                                    ? 'bg-green-600 hover:bg-green-700'
                                    : 'bg-red-600 hover:bg-red-700'
                                    }`}
                                disabled={loading}
                            >
                                {loading && <Loader2 className="w-4 h-4 animate-spin" />}
                                Xác nhận
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </>
    );
}
