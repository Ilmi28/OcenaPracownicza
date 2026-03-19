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
    period: string;
    finalScore: string;
    achievementsSummary: string;
    stage2Status?: number;
    stage2Comment?: string | null;
    stage2ReviewedByUserId?: string | null;
    stage2ReviewedAtUtc?: string | null;
    userId: string;
}

export interface Stage2ReviewItemView {
    employeeId: string;
    fullName: string;
    position: string;
    period: string;
    finalScore: string;
    stage2Status: number;
    achievementsCount: number;
}

export interface AchievementStage2View {
    id: string;
    name: string;
    description: string;
    date: string;
    category: number;
    stage2Status: number;
    stage2Comment?: string | null;
}

export interface Stage2ReviewDetailsView {
    employeeId: string;
    fullName: string;
    position: string;
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
