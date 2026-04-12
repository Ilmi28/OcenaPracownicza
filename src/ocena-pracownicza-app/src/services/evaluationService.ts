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
    getDetails: async (achievementId: string) => {
        const res = await axiosClient.get<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/${achievementId}`,
        );
        return res.data.data;
    },
    approve: async (achievementId: string, comment?: string) => {
        const res = await axiosClient.post<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/${achievementId}/approve`,
            { comment: comment ?? null },
        );
        return res.data.data;
    },
    reject: async (achievementId: string, comment: string) => {
        const res = await axiosClient.post<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/${achievementId}/reject`,
            { comment },
        );
        return res.data.data;
    },
    close: async (achievementId: string) => {
        const res = await axiosClient.post<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/${achievementId}/close`,
        );
        return res.data.data;
    },
    archive: async (achievementId: string) => {
        const res = await axiosClient.post<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/${achievementId}/archive`,
        );
        return res.data.data;
    },
};
