import axiosClient from "./axiosClient";

export const authService = {
    login: async (emailOrUsername: string, password: string) => {
        const payload = {
            userNameEmail: emailOrUsername,   
            password: password                
        };

        const res = await axiosClient.post("/auth/login", payload, {
            withCredentials: true,
        });

        return res.data;
    },

    check: async () => {
        return await axiosClient.get("/auth/secure", { withCredentials: true });
    },
};
