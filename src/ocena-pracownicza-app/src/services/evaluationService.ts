import axiosClient from "./axiosClient";
import {
    Stage2HistoryItemView,
    Stage2ReviewDetailsView,
    Stage2ReviewItemView,
} from "../utils/types";

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}

interface UpdateAchievementPayload {
    name: string;
    description: string;
    finalScore: string;
    stage2Comment?: string | null;
}

export const evaluationService = {
    getStage2History: async () => {
        const res = await axiosClient.get<ApiResponse<Stage2HistoryItemView[]>>(
            "/evaluation/stage2/history",
        );
        return res.data.data;
    },
    getMyStage2History: async () => {
        const res = await axiosClient.get<ApiResponse<Stage2HistoryItemView[]>>(
            "/evaluation/stage2/history/me",
        );
        return res.data.data;
    },
    getPending: async () => {
        const res = await axiosClient.get<ApiResponse<Stage2ReviewItemView[]>>(
            "/evaluation/stage2/pending",
        );
        return res.data.data;
    },
    getApproved: async () => {
        const res = await axiosClient.get<ApiResponse<Stage2ReviewItemView[]>>(
            "/evaluation/stage2/approved",
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
    getMyDetails: async (achievementId: string) => {
        const res = await axiosClient.get<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/history/me/${achievementId}`,
        );
        return res.data.data;
    },

    updateAchievement: async (achievementId: string, payload: UpdateAchievementPayload) => {
        const res = await axiosClient.put<ApiResponse<Stage2ReviewDetailsView>>(
            `/evaluation/stage2/${achievementId}`,
            payload
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

    downloadAttachment: async (attachmentId: string, fileName: string): Promise<void> => {
        const res = await axiosClient.get(`/Achievement/attachments/${attachmentId}`, {
            responseType: "blob",
        });

        const url = window.URL.createObjectURL(new Blob([res.data]));
        const link = document.createElement("a");
        link.href = url;
        
        link.setAttribute("download", fileName); 
        document.body.appendChild(link);
        link.click();

        link.parentNode?.removeChild(link);
        window.URL.revokeObjectURL(url);
    },
};