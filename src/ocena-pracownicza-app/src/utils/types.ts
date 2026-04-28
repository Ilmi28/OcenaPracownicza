export interface User {
    userId?: string;
    email?: string;
    userName: string;
    role: string;
    roles?: string[];
}

export interface EmployeeView {
    id?: string;
    email: string;
    userName: string;
    firstName: string;
    lastName: string;
    position: string;
    userId: string;
}

export interface Stage2ReviewItemView {
    achievementId: string;
    employeeId: string;
    fullName: string;
    position: string;
    achievementName: string;
    period: string;
    finalScore: string;
    stage2Status: number;
}

export interface Stage2HistoryItemView {
    achievementId: string;
    employeeId: string;
    fullName: string;
    position: string;
    achievementName: string;
    period: string;
    finalScore: string;
    stage2Status: number;
    date: string;
    stage2ReviewedAtUtc?: string | null;
}

export interface AchievementStage2View {
    id: string;
    name: string;
    description: string;
    date: string;
    category: number;
    period: string;
    finalScore: string;
    achievementsSummary: string;
    stage2Status: number;
    stage2Comment?: string | null;
    stage2ReviewedByUserId?: string | null;
    stage2ReviewedAtUtc?: string | null;
}

export interface Stage2ReviewDetailsView {
    achievementId: string;
    employeeId: string;
    fullName: string;
    position: string;
    achievementName: string;
    period: string;
    finalScore: string;
    achievementsSummary: string;
    stage2Status: number;
    stage2Comment?: string | null;
    stage2ReviewedByUserId?: string | null;
    stage2ReviewedAtUtc?: string | null;
    achievements: AchievementStage2View[];
}

export interface ManagerView {
    id: string;
    email: string;
    userName: string;
    firstName: string;
    lastName: string;
    achievementsSummary: string;
}

export interface AdminView {
    id: string;
    email: string;
    userName: string;
    firstName: string;
    lastName: string;
}

export interface RegisterRequest {
    userName: string;
    email: string;
    password: string;
}
