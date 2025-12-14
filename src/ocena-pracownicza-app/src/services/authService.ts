import axiosClient from "./axiosClient";
import jwtDecode from 'jwt-decode';

interface LoginCredentials {
    userNameEmail: string;
    password: string;
}
interface JwtTokenPayload {
    sub: string;
    unique_name: string;
    email: string;
    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
    exp: number;
}
export interface UserFromJwt {
    id: string;
    name: string;
    email: string;
    role: string;
}
interface LoginResponse {
    token: string;
}

const TOKEN_KEY = "auth_token";

export const authService = {
    getToken: (): string | null => localStorage.getItem(TOKEN_KEY),
    setToken: (token: string): void => localStorage.setItem(TOKEN_KEY, token),
    removeToken: (): void => localStorage.removeItem(TOKEN_KEY),
    getDecodedUser: (): UserFromJwt | null => {
        const token = authService.getToken();
        if (!token) return null;
        try {
            debugger;
            const decoded = jwtDecode<JwtTokenPayload>(token);

            const user: UserFromJwt = {
                id: decoded.sub,
                name: decoded.unique_name,
                email: decoded.email,
                role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
            };

            if (decoded.exp * 1000 < Date.now()) {
                console.error("Token has expired.");
                authService.removeToken();
                return null;
            }
            return user;
        } catch (error) {
            console.error("Error decoding token:", error);
            authService.removeToken();
            return null;
        }
    },

    login: async (loginCredentials: LoginCredentials): Promise<LoginResponse> => {
        const res = await axiosClient.post("/auth/login", {
            userNameEmail: loginCredentials.userNameEmail,
            password: loginCredentials.password,
        });
        debugger;
        const token = res.data.data.token;
        if (token) {
            authService.setToken(token);
        } else {
            throw new Error("Token not returned by server!");
        }
        return res.data;
    },

    check: async () => {
        const res = await axiosClient.get("/auth/secure");
        return res.data;
    },

    changePassword: async (oldPassword: string, newPassword: string) => {
        return axiosClient.post("/user/change-password", {
            oldPassword,
            newPassword,
        });
    },
};


