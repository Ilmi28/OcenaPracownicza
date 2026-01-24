import { jwtDecode } from "jwt-decode";
import axiosClient from "./axiosClient";

interface LoginCredentials {
    userNameEmail: string;
    password: string;
}

export const authService = {
    login: async (loginCredentials: LoginCredentials) => {
        const res = await axiosClient.post("/auth/login", {
            userNameEmail: loginCredentials.userNameEmail,
            password: loginCredentials.password,
        });
        return res.data;
    },

    logout: async () => {
        await axiosClient.get("/auth/logout");
    },

    isLoggedIn: async () => {
        const res = await axiosClient.get("/auth/me");

        if (res.status === 401) return false;
        return true;
    },

    getUser: async () => {
        const res = await axiosClient.get("/auth/me");

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
