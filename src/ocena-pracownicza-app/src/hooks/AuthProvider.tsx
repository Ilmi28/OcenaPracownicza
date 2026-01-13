import React, { createContext, useContext, useEffect, useState } from "react";
import { authService } from "../services/authService";
import { UserFromJwt } from "../services/authService";

interface User {
    id: string;
    email?: string;
}


interface User extends UserFromJwt {
    surname?: string;
    job?: string;
}
interface AuthContextType {
    user: User | null;
    loading: boolean;
    login: (email: string, password: string) => Promise<void>;
    logout: () => void;
    refresh: () => Promise<void>;

}

interface Props {
    children: React.ReactNode;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<Props> = ({ children }) => {
    const [user, setUser] = useState<User | null>(null);
    const [loading, setLoading] = useState(true);

    const logout = () => {
        authService.removeToken();
        setUser(null);
    };

    const refresh = async () => {
        setLoading(true);
        try {
            const token = authService.getToken();
            if (!token) {
                setUser(null);
                return;
            }
            debugger;
            const decodedUser = authService.getDecodedUser();

            if (decodedUser) {
                const fullUser: User = {
                    ...decodedUser,
                    surname: "Brak danych z serwera",
                    job: "Brak danych z serwera"
                };
                setUser(fullUser);
            } else {
                logout();
            }
        } catch (error) {
            console.error("Refresh error:", error);
            logout();
        } finally {
            setLoading(false);
        }
    };

    const login = async (username: string, password: string) => {
        await authService.login({
            userNameEmail: username,
            password: password,
        });
        await refresh();
    };

    useEffect(() => {
        refresh();
    }, []);

    return (
        <AuthContext.Provider value={{ user, loading, login, logout, refresh }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = (): AuthContextType => {
    const ctx = useContext(AuthContext);
    if (!ctx) {
        throw new Error("useAuth must be used inside <AuthProvider>");
    }
    return ctx;
};
