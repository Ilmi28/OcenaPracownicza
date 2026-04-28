import React, { useState, useEffect } from "react";
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    TextField,
    Grid,
    Typography,
} from "@mui/material";
import axiosClient from "../services/axiosClient";

interface AddUserModalProps {
    open: boolean;
    onClose: () => void;
    activeTab: number; // 0: Pracownik, 1: Manager, 2: Admin
    onSuccess: () => void;
    mode?: "create" | "edit";
    userToEdit?: {
        id: string;
        firstName: string;
        lastName: string;
        email: string;
        userName: string;
        position?: string;
        achievementsSummary?: string;
    } | null;
}

const AddUserModal: React.FC<AddUserModalProps> = ({
    open,
    onClose,
    activeTab,
    onSuccess,
    mode = "create",
    userToEdit = null,
}) => {
    const initialState = {
        userName: "",
        email: "",
        password: "",
        firstName: "",
        lastName: "",
        position: "",
        achievementsSummary: "",
    };

    const [formData, setFormData] = useState(initialState);

    useEffect(() => {
        if (!open) return;
        if (mode === "edit" && userToEdit) {
            setFormData({
                userName: userToEdit.userName ?? "",
                email: userToEdit.email ?? "",
                password: "",
                firstName: userToEdit.firstName ?? "",
                lastName: userToEdit.lastName ?? "",
                position: userToEdit.position ?? "",
                achievementsSummary: userToEdit.achievementsSummary ?? "",
            });
            return;
        }

        setFormData(initialState);
    }, [open, activeTab, mode, userToEdit]);

    const handleSubmit = async () => {
        const endpoints = ["/employee", "/manager", "/admin"];
        const endpoint = endpoints[activeTab];

        try {
            if (mode === "edit" && !userToEdit?.id) {
                alert("Brak użytkownika do edycji.");
                return;
            }

            const basePayload = {
                userName: formData.userName,
                email: formData.email,
                firstName: formData.firstName,
                lastName: formData.lastName,
            };
            const createPayload = {
                ...basePayload,
                password: formData.password,
            };

            if (activeTab === 0) {
                if (mode === "edit") {
                    await axiosClient.put(`${endpoint}/${userToEdit?.id}`, {
                        ...basePayload,
                        position: formData.position,
                    });
                } else {
                    await axiosClient.post(endpoint, {
                        ...createPayload,
                        position: formData.position,
                    });
                }
            } else if (activeTab === 1) {
                if (mode === "edit") {
                    await axiosClient.put(`${endpoint}/${userToEdit?.id}`, {
                        ...basePayload,
                        achievementsSummary: formData.achievementsSummary,
                    });
                } else {
                    await axiosClient.post(endpoint, {
                        ...createPayload,
                        achievementsSummary: formData.achievementsSummary,
                    });
                }
            } else if (mode === "edit") {
                await axiosClient.put(`${endpoint}/${userToEdit?.id}`, basePayload);
            } else {
                await axiosClient.post(endpoint, createPayload);
            }

            onSuccess();
            onClose();
        } catch (error) {
            console.error(error);
            alert(
                mode === "edit"
                    ? "Błąd podczas edycji użytkownika. Sprawdź czy wszystkie dane są poprawne."
                    : "Błąd podczas dodawania użytkownika. Sprawdź czy wszystkie dane są poprawne.",
            );
        }
    };

    const getTitle = () => {
        if (mode === "edit") {
            if (activeTab === 0) return "Edytuj Pracownika";
            if (activeTab === 1) return "Edytuj Menedżera";
            return "Edytuj Administratora";
        }

        if (activeTab === 0) return "Dodaj Pracownika";
        if (activeTab === 1) return "Dodaj Menedżera";
        return "Dodaj Administratora";
    };

    return (
        <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
            <DialogTitle sx={{ fontWeight: 600 }}>{getTitle()}</DialogTitle>
            <DialogContent dividers>
                <Grid container spacing={2} sx={{ mt: 0.5 }}>
                    <Grid size={{ xs: 6 }}>
                        <TextField fullWidth label="Imię" size="small" value={formData.firstName} onChange={(e) => setFormData({ ...formData, firstName: e.target.value })} />
                    </Grid>
                    <Grid size={{ xs: 6 }}>
                        <TextField fullWidth label="Nazwisko" size="small" value={formData.lastName} onChange={(e) => setFormData({ ...formData, lastName: e.target.value })} />
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                        <TextField fullWidth label="Nazwa użytkownika" size="small" value={formData.userName} onChange={(e) => setFormData({ ...formData, userName: e.target.value })} />
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                        <TextField fullWidth label="Email" size="small" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} />
                    </Grid>
                    {mode === "create" && (
                        <Grid size={{ xs: 12 }}>
                            <TextField fullWidth label="Hasło" type="password" size="small" value={formData.password} onChange={(e) => setFormData({ ...formData, password: e.target.value })} />
                        </Grid>
                    )}
                    {activeTab === 0 && (
                        <>
                            <Grid size={{ xs: 12 }}><Typography variant="subtitle2" color="primary">Dane stanowiska</Typography></Grid>
                            <Grid size={{ xs: 6 }}>
                                <TextField fullWidth label="Stanowisko" size="small" value={formData.position} onChange={(e) => setFormData({ ...formData, position: e.target.value })} />
                            </Grid>
                        </>
                    )}

                    {activeTab === 1 && (
                        <Grid size={{ xs: 12 }}>
                            <TextField
                                fullWidth
                                multiline
                                rows={2}
                                label="Osiągnięcia / Podsumowanie"
                                size="small"
                                value={formData.achievementsSummary}
                                onChange={(e) => setFormData({ ...formData, achievementsSummary: e.target.value })}
                            />
                        </Grid>
                    )}
                </Grid>
            </DialogContent>
            <DialogActions sx={{ p: 2.5 }}>
                <Button onClick={onClose} color="inherit">Anuluj</Button>
                <Button onClick={handleSubmit} variant="contained" sx={{ px: 4 }}>
                    {mode === "edit" ? "Zapisz zmiany" : "Zapisz"}
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default AddUserModal;
