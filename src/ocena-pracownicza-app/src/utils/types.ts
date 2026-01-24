export interface User {
    userName: string;
    role: string;
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
    userId: string;
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
