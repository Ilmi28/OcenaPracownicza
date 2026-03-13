import { useEffect, useState } from "react";
import {
    Box,
    Typography,
    Paper,
    CircularProgress,
    Alert,
    TextField,
    Grid,
    Button,
} from "@mui/material";
import axiosClient from "../services/axiosClient";
import { useAuth } from "../hooks/AuthProvider";
import { ManagerView } from "../utils/types";

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}

export default function ManagerDashboard() {
    const { user, loading: authLoading } = useAuth();

    const [manager, setManager] = useState<ManagerView | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [editMode, setEditMode] = useState(false);
    const [newManager, setNewManager] = useState<ManagerView | null>(null);
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        const fetchManagerData = async () => {
            try {
                const resp =
                    await axiosClient.get<ApiResponse<ManagerView>>(
                        "/manager/me",
                    );
                setManager(resp.data.data);
            } catch (err: any) {
                const msg =
                    err?.response?.data?.message ||
                    err?.message ||
                    "Błąd pobierania danych";
                setError(msg);
            } finally {
                setLoading(false);
            }
        };

        if (!authLoading && user) {
            fetchManagerData();
        } else if (!authLoading && !user) {
            setLoading(false);
        }
    }, [user, authLoading]);

    const handleSave = async () => {
        if (!newManager || !manager?.id) {
            setError("Brak danych do zapisania");
            return;
        }

        setSaving(true);
        setError(null);

        try {
            const payload = {
                userName: newManager.userName || "",
                email: newManager.email || "",
                firstName: newManager.firstName,
                lastName: newManager.lastName,
                achievementSummary: newManager.achievementsSummary,
            };

            await axiosClient.put(`/manager/${manager.id}`, payload);

            const resp =
                await axiosClient.get<ApiResponse<ManagerView>>("/manager/me");
            setManager(resp.data.data);

            setEditMode(false);
            setNewManager(null);
        } catch (err: any) {
            const msg =
                err?.response?.data?.message ||
                err?.message ||
                "Błąd podczas zapisywania danych";
            setError(msg);
        } finally {
            setSaving(false);
        }
    };

    if (authLoading || loading) {
        return (
            <Box
                display="flex"
                justifyContent="center"
                alignItems="center"
                height="60vh"
            >
                <CircularProgress />
            </Box>
        );
    }

    if (!user) {
        return (
            <Box m={2}>
                <Alert severity="warning">
                    Zaloguj się, aby zobaczyć swoje dane.
                </Alert>
            </Box>
        );
    }

    if (error) {
        return (
            <Box m={2}>
                <Alert severity="error">Błąd: {error}</Alert>
            </Box>
        );
    }

    if (!manager) {
        return (
            <Box m={2}>
                <Typography>Nie znaleziono danych użytkownika.</Typography>
            </Box>
        );
    }

    const currentManager = editMode && newManager ? newManager : manager;

    return (
        <Box>
            <Typography variant="h5" fontWeight="600">
                Mój Profil
            </Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
                Zarządzaj swoimi danymi osobowymi
            </Typography>

            {error && (
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                </Alert>
            )}

            <Paper sx={{ p: 3, mb: 3 }}>
                <Box
                    sx={{
                        display: "flex",
                        justifyContent: "space-between",
                        alignItems: "center",
                        mb: 2,
                    }}
                >
                    <Typography variant="subtitle1" fontWeight="600">
                        Dane osobowe
                    </Typography>

                    {!editMode && (
                        <Button
                            variant="outlined"
                            size="small"
                            onClick={() => {
                                setNewManager(manager);
                                setEditMode(true);
                                setError(null);
                            }}
                        >
                            Edytuj
                        </Button>
                    )}
                </Box>

                <Grid container spacing={3}>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField
                            fullWidth
                            label="Nazwa użytkownika"
                            value={currentManager.userName ?? ""}
                            disabled={!editMode}
                            size="small"
                            onChange={(e) =>
                                setNewManager((prev) =>
                                    prev
                                        ? { ...prev, userName: e.target.value }
                                        : prev,
                                )
                            }
                        />
                    </Grid>

                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField
                            fullWidth
                            label="E-mail"
                            value={currentManager.email ?? ""}
                            disabled={!editMode}
                            size="small"
                            onChange={(e) =>
                                setNewManager((prev) =>
                                    prev
                                        ? { ...prev, email: e.target.value }
                                        : prev,
                                )
                            }
                        />
                    </Grid>

                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField
                            fullWidth
                            label="Imię"
                            value={currentManager.firstName}
                            disabled={!editMode}
                            size="small"
                            onChange={(e) =>
                                setNewManager((prev) =>
                                    prev
                                        ? { ...prev, firstName: e.target.value }
                                        : prev,
                                )
                            }
                        />
                    </Grid>

                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField
                            fullWidth
                            label="Nazwisko"
                            value={currentManager.lastName}
                            disabled={!editMode}
                            size="small"
                            onChange={(e) =>
                                setNewManager((prev) =>
                                    prev
                                        ? { ...prev, lastName: e.target.value }
                                        : prev,
                                )
                            }
                        />
                    </Grid>

                    <Grid size={{ xs: 12 }}>
                        <TextField
                            fullWidth
                            label="Podsumowanie osiągnięć"
                            value={currentManager.achievementsSummary ?? ""}
                            disabled={!editMode}
                            size="small"
                            multiline
                            rows={4}
                            onChange={(e) =>
                                setNewManager((prev) =>
                                    prev
                                        ? {
                                              ...prev,
                                              achievementsSummary:
                                                  e.target.value,
                                          }
                                        : prev,
                                )
                            }
                        />
                    </Grid>
                </Grid>

                {editMode && (
                    <Box
                        mt={3}
                        display="flex"
                        justifyContent="flex-end"
                        gap={2}
                    >
                        <Button
                            variant="outlined"
                            onClick={() => {
                                setEditMode(false);
                                setNewManager(null);
                                setError(null);
                            }}
                            disabled={saving}
                        >
                            Anuluj
                        </Button>

                        <Button
                            variant="contained"
                            onClick={handleSave}
                            disabled={saving}
                        >
                            {saving ? "Zapisywanie..." : "Zapisz"}
                        </Button>
                    </Box>
                )}
            </Paper>
        </Box>
    );
}
