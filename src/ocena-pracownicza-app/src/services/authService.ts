import axiosClient from "./axiosClient";

export const authService = {
    // --- LOGIN ---
    login: async (username: string, password: string) => {
        const res = await axiosClient.post("/auth/login", {
            username,
            password,
        });
        return res.data;
    },

    // --- CHECK IF USER IS LOGGED IN ---
    check: async () => {
        const res = await axiosClient.get("/auth/secure");
        return res.data;
    },

    // --- RESET PASSWORD (STEP 1) ---
    resetPassword: async (email: string): Promise<{ message: string }> => {
        const res = await axiosClient.post("/auth/reset-password", { email });
        return res.data; // { message: "..." }
    },

    // --- CONFIRM RESET PASSWORD (STEP 2) ---
    confirmResetPassword: async (dto: {
        token: string;
        newPassword: string;
    }): Promise<{ message: string }> => {
        const res = await axiosClient.post("/auth/confirm-reset-password", dto);
        return res.data; // { message: "..." }
    },

};

export const changePassword = async (oldPassword: string, newPassword: string) => {
    return axiosClient.post("/user/change-password", {
        oldPassword,
        newPassword
    });
};

