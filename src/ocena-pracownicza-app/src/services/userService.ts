// src/services/userService.ts
import axiosClient from "./axiosClient";
import { IUser, IUserCreate } from "../models/User";

const API_URL = "/api/user";

async function getAll(): Promise<IUser[]> {
    const res = await axiosClient.get(`${API_URL}`);
    return res.data;
}

async function getById(id: string): Promise<IUser> {
    const res = await axiosClient.get(`${API_URL}/${id}`);
    return res.data;
}

async function create(payload: IUserCreate): Promise<IUser> {
    const res = await axiosClient.post(`${API_URL}`, payload);
    return res.data;
}

async function update(id: string, payload: Partial<IUserCreate>): Promise<IUser> {
    const res = await axiosClient.put(`${API_URL}/${id}`, payload);
    return res.data;
}

async function deleteUser(id: string): Promise<void> {
    await axiosClient.delete(`${API_URL}/${id}`);
}

async function adminResetPassword(id: string): Promise<void> {
    await axiosClient.post(`${API_URL}/reset-password/${id}`);
}

async function changePassword(currentPassword: string, newPassword: string): Promise<void> {
    await axiosClient.post(`${API_URL}/change-password`, {
        currentPassword,
        newPassword,
    });
}

export const userService = {
    getAll,
    getById,
    create,
    update,
    delete: deleteUser,
    adminResetPassword,
    changePassword,
};
