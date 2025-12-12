// src/hooks/useUsers.ts
import { useState, useEffect, useCallback } from "react";
import { userService } from "../services/userService";
import { IUser } from "../models/User";

export default function useUsers() {
    const [users, setUsers] = useState<IUser[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const load = useCallback(async () => {
        setLoading(true);
        setError(null);

        try {
            const data = await userService.getAll();
            setUsers(data);

        } catch (err: unknown) {

            if (err instanceof Error) {
                setError(err.message);
            } else {
                setError("B³¹d ³adowania u¿ytkowników");
            }

        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        load();
    }, [load]);

    return { users, loading, error, reload: load, setUsers };
}
