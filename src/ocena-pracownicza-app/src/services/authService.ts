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


