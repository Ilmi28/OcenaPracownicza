import React, { useState, useEffect } from "react";
import { authService } from "../services/authService";
import { Props, AuthContext } from "./AuthProvider";


export const AuthProvider: React.FC<Props> = ({ children }) => {
    const [user, setUser] = useState<boolean | null>(null);
    const [loading, setLoading] = useState(true);

    const refresh = async () => {
        setLoading(true);
        try {
            await authService.check();
            setUser(true);
        } catch {
            setUser(false);
        } finally {
            setLoading(false);
        }
    };

    const login = async (userNameEmail: string, password: string) => {
        await authService.login(userNameEmail, password);
        await refresh();
    };

    useEffect(() => {
        refresh();
    }, []);

    return (
        <AuthContext.Provider value={{ user, loading, login, refresh }}>
            {children}
        </AuthContext.Provider>
    );
};
