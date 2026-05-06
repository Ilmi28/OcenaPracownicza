import { useEffect, useState } from "react";
import {
    Box,
    Typography,
    Paper,
    Button,
    TextField,
    Grid,
    CircularProgress,
    Alert
} from "@mui/material";
import axiosClient from "../services/axiosClient";
import { useAuth } from "../hooks/AuthProvider";
import { AdminView } from "../utils/types";


interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}

const AdminProfile = () => {
    const { user, loading: authLoading } = useAuth();

    const [admin, setAdmin] = useState<AdminView | null>(null);
    const [editMode, setEditMode] = useState(false);
    const [newAdmin, setNewAdmin] = useState<AdminView | null>(null);
    const [saving, setSaving] = useState(false);

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchData = async () => {
        try {
            setLoading(true);
            const [adminResp] = await Promise.all([
                axiosClient.get<ApiResponse<AdminView>>(`/admin/me`)
            ]);

            setAdmin(adminResp.data.data);


        } catch (err: any) {
            setError(err?.response?.data?.message || err?.message || "Błąd pobierania danych");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (!authLoading && user) {
            fetchData();
        } else if (!authLoading && !user) {
            setLoading(false);
        }
    }, [user, authLoading]);

    const handleSaveProfile = async () => {
        if (!newAdmin || !admin?.id) return;
        setSaving(true);
        try {
            const payload = {
                userName: newAdmin.userName || "",
                email: newAdmin.email || "",
                firstName: newAdmin.firstName,
                lastName: newAdmin.lastName,
            };
            await axiosClient.put(`/admin/${admin.id}`, payload);
            await fetchData();
            setEditMode(false);
        } catch (err: any) {
            setError("Błąd podczas zapisywania profilu");
        } finally {
            setSaving(false);
        }
    };

    if (authLoading || loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" height="60vh">
                <CircularProgress />
            </Box>
        );
    }

    const currentAdmin = editMode && newAdmin ? newAdmin : admin;

    return (
        <Box sx={{ maxWidth: 1000, mx: "auto", py: 4 }}>
            <Typography variant="h5" fontWeight="600" gutterBottom>
                Panel Administratora
            </Typography>

            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

            <Paper sx={{ p: 3, mb: 4 }}>
                <Box sx={{ display: "flex", justifyContent: "space-between", mb: 3 }}>
                    <Typography variant="subtitle1" fontWeight="600">Dane osobowe</Typography>
                    {!editMode && (
                        <Button variant="outlined" size="small" onClick={() => { setNewAdmin(admin); setEditMode(true); }}>
                            Edytuj Profil
                        </Button>
                    )}
                </Box>

                <Grid container spacing={3}>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField fullWidth label="Nazwa użytkownika" value={currentAdmin?.userName ?? ""} disabled={!editMode} size="small"
                            onChange={(e) => setNewAdmin(prev => prev ? { ...prev, userName: e.target.value } : null)} />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField fullWidth label="E-mail" value={currentAdmin?.email ?? ""} disabled={!editMode} size="small"
                            onChange={(e) => setNewAdmin(prev => prev ? { ...prev, email: e.target.value } : null)} />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField fullWidth label="Imię" value={currentAdmin?.firstName ?? ""} disabled={!editMode} size="small"
                            onChange={(e) => setNewAdmin(prev => prev ? { ...prev, firstName: e.target.value } : null)} />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField fullWidth label="Nazwisko" value={currentAdmin?.lastName ?? ""} disabled={!editMode} size="small"
                            onChange={(e) => setNewAdmin(prev => prev ? { ...prev, lastName: e.target.value } : null)} />
                    </Grid>

                    {editMode && (
                        <Grid size={{ xs: 12 }} sx={{ display: "flex", justifyContent: "flex-end", gap: 2, mt: 1 }}>
                            <Button variant="outlined" onClick={() => setEditMode(false)}>Anuluj</Button>
                            <Button variant="contained" onClick={handleSaveProfile} disabled={saving}>Zapisz</Button>
                        </Grid>
                    )}
                </Grid>
            </Paper>
        </Box>
    );
};

export default AdminProfile;