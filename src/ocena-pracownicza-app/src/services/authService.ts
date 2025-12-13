import axiosClient from "./axiosClient";

export const authService = {
    login: async (username: string, password: string) => {
        const res = await axiosClient.post("/auth/login", { username, password });
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


