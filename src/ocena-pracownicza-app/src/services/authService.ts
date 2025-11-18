import axiosClient from "./axiosClient";

export const authService = {
    login: async (username: string, password: string) => {
        const res = await axiosClient.post("/auth/login", {
            username,
            password,
        });
        return res.data;
    },

    check: async () => {
        const res = await axiosClient.get("/auth/secure");
        return res.data;
    },
};
