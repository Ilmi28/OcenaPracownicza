import React, { createContext, useContext } from "react";

interface AuthContextType {
    user: boolean | null;
    loading: boolean;
    login: (userNameEmail: string, password: string) => Promise<void>;
    refresh: () => Promise<void>;
}

export interface Props {
    children: React.ReactNode;
}

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = (): AuthContextType => {
    const ctx = useContext(AuthContext);
    if (!ctx) {
        throw new Error("useAuth must be used inside <AuthProvider>");
    }
    return ctx;
};
