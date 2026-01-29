import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { apiClient } from '@/api/client';

interface User {
  id: number;
  userName: string;
  fullName: string;
  email: string;
  role: string;
  unitId: number;
  unitName: string;
}

interface AuthState {
  token: string | null;
  user: User | null;
  currentUnitId: number | null;
  isAuthenticated: boolean;
  login: (token: string, user: User) => void;
  logout: () => Promise<void>;
  setCurrentUnit: (unitId: number) => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      user: null,
      currentUnitId: null,
      isAuthenticated: false,

      login: (token: string, user: User) => {
        set({
          token,
          user,
          currentUnitId: user.unitId,
          isAuthenticated: true,
        });
      },

      logout: async () => {
        try {
          await apiClient.logout();
        } catch {
          // Ignore logout API errors
        } finally {
          set({
            token: null,
            user: null,
            currentUnitId: null,
            isAuthenticated: false,
          });
        }
      },

      setCurrentUnit: (unitId: number) => {
        set({ currentUnitId: unitId });
      },
    }),
    {
      name: 'ssms-auth',
      partialize: (state) => ({
        token: state.token,
        user: state.user,
        currentUnitId: state.currentUnitId,
        isAuthenticated: state.isAuthenticated,
      }),
    }
  )
);
