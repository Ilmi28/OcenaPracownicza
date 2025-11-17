import { jsx as _jsx } from "react/jsx-runtime";
import { createContext, useContext, useEffect, useState } from "react";
import { authService } from "../services/authService";
const AuthContext = createContext(undefined);
export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);
    const refresh = async () => {
        setLoading(true);
        try {
            await authService.check();
            setUser(true);
        }
        catch {
            setUser(false);
        }
        finally {
            setLoading(false);
        }
    };
    const login = async (username, password) => {
        await authService.login(username, password);
        await refresh();
    };
    useEffect(() => {
        refresh();
    }, []);
    return (_jsx(AuthContext.Provider, { value: { user, loading, login, refresh }, children: children }));
};
export const useAuth = () => {
    const ctx = useContext(AuthContext);
    if (!ctx) {
        throw new Error("useAuth must be used inside <AuthProvider>");
    }
    return ctx;
};
