import axiosClient from "./axiosClient";
import { Stage2ReviewDetailsView, Stage2ReviewItemView } from "../utils/types";

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}

export const evaluationService = {
    getPending: async () => {
        const res = await axiosClient.get<ApiResponse<Stage2ReviewItemView[]>>(
            "/evaluation/stage2/pending",
        );
        return res.data.data;
    },
    getArchived: async () => {
        const res = await axiosClient.get<ApiResponse<Stage2ReviewItemView[]>>(
            "/evaluation/stage2/archived",
        );
        return res.data.data;
    },
    getDetails: async (employeeId: string) => {
        const res = await axiosClient.get<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/${employeeId}`,
        );
        return res.data.data;
    },
    approve: async (employeeId: string, comment?: string) => {
        const res = await axiosClient.post<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/${employeeId}/approve`,
            { comment: comment ?? null },
        );
        return res.data.data;
    },
    reject: async (employeeId: string, comment: string) => {
        const res = await axiosClient.post<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/${employeeId}/reject`,
            { comment },
        );
        return res.data.data;
    },
    close: async (employeeId: string) => {
        const res = await axiosClient.post<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/${employeeId}/close`,
        );
        return res.data.data;
    },
    archive: async (employeeId: string) => {
        const res = await axiosClient.post<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/${employeeId}/archive`,
        );
        return res.data.data;
    },
};
