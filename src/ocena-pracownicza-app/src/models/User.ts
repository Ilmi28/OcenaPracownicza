// src/models/User.ts

export interface IUser {
    id: string;
    firstName?: string;
    lastName?: string;
    email: string;
    roles?: string[];
    isActive?: boolean;

}

export interface IUserCreate {
    firstName?: string;
    lastName?: string;
    email: string;
    password?: string; // przy tworzeniu u¿ytkownika (admin mo¿e ustawiæ)
    roles?: string[];
}
