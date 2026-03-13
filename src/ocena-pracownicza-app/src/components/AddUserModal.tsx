import React, { useState, useEffect } from 'react';
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    TextField,
    Grid,
    Typography
} from '@mui/material';
import axiosClient from '../services/axiosClient';

interface AddUserModalProps {
    open: boolean;
    onClose: () => void;
    activeTab: number; // 0: Pracownik, 1: Manager, 2: Admin
    onSuccess: () => void;
}

const AddUserModal: React.FC<AddUserModalProps> = ({ open, onClose, activeTab, onSuccess }) => {
    const initialState = {
        userName: '',
        email: '',
        password: '',
        firstName: '',
        lastName: '',
        position: '',
        period: '2025',
        finalScore: '0',
        achievementsSummary: ''
    };

    const [formData, setFormData] = useState(initialState);

    // Resetuj formularz przy każdym otwarciu
    useEffect(() => {
        if (open) setFormData(initialState);
    }, [open, activeTab]);

    const handleSubmit = async () => {
        const endpoints = ['/employee', '/manager', '/admin'];
        try {
            // Przygotowanie payloadu zgodnie z klasami Create...Request w C#
            let payload: any = {
                userName: formData.userName,
                email: formData.email,
                password: formData.password,
                firstName: formData.firstName,
                lastName: formData.lastName,
            };

            if (activeTab === 0) { // CreateEmployeeRequest
                payload = {
                    ...payload,
                    position: formData.position,
                    period: formData.period,
                    finalScore: formData.finalScore,
                    achievementsSummary: formData.achievementsSummary
                };
            } else if (activeTab === 1) { // CreateManagerRequest
                payload = {
                    ...payload,
                    achievementsSummary: formData.achievementsSummary
                };
            }

            await axiosClient.post(endpoints[activeTab], payload);
            onSuccess();
            onClose();
        } catch (error) {
            console.error(error);
            alert("Błąd podczas dodawania użytkownika. Sprawdź czy wszystkie dane są poprawne.");
        }
    };

    const getTitle = () => {
        if (activeTab === 0) return 'Dodaj Pracownika';
        if (activeTab === 1) return 'Dodaj Menedżera';
        return 'Dodaj Administratora';
    };

    return (
        <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
            <DialogTitle sx={{ fontWeight: 600 }}>{getTitle()}</DialogTitle>
            <DialogContent dividers>
                <Grid container spacing={2} sx={{ mt: 0.5 }}>
                    <Grid size={{ xs: 6 }}>
                        <TextField fullWidth label="Imię" size="small" value={formData.firstName} onChange={e => setFormData({...formData, firstName: e.target.value})} />
                    </Grid>
                    <Grid size={{ xs: 6 }}>
                        <TextField fullWidth label="Nazwisko" size="small" value={formData.lastName} onChange={e => setFormData({...formData, lastName: e.target.value})} />
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                        <TextField fullWidth label="Nazwa użytkownika" size="small" value={formData.userName} onChange={e => setFormData({...formData, userName: e.target.value})} />
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                        <TextField fullWidth label="Email" size="small" value={formData.email} onChange={e => setFormData({...formData, email: e.target.value})} />
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                        <TextField fullWidth label="Hasło" type="password" size="small" value={formData.password} onChange={e => setFormData({...formData, password: e.target.value})} />
                    </Grid>

                    {/* Pola dla Pracownika (Employee) */}
                    {activeTab === 0 && (
                        <>
                            <Grid size={{ xs: 12 }}><Typography variant="subtitle2" color="primary">Dane stanowiska</Typography></Grid>
                            <Grid size={{ xs: 6 }}>
                                <TextField fullWidth label="Stanowisko" size="small" value={formData.position} onChange={e => setFormData({...formData, position: e.target.value})} />
                            </Grid>
                            <Grid size={{ xs: 6 }}>
                                <TextField fullWidth label="Okres" size="small" value={formData.period} onChange={e => setFormData({...formData, period: e.target.value})} />
                            </Grid>
                        </>
                    )}

                    {/* Pole Achievements (Employee i Manager) */}
                    {(activeTab === 0 || activeTab === 1) && (
                        <Grid size={{ xs: 12 }}>
                            <TextField
                                fullWidth
                                multiline
                                rows={2}
                                label="Osiągnięcia / Podsumowanie"
                                size="small"
                                value={formData.achievementsSummary}
                                onChange={e => setFormData({...formData, achievementsSummary: e.target.value})}
                            />
                        </Grid>
                    )}
                </Grid>
            </DialogContent>
            <DialogActions sx={{ p: 2.5 }}>
                <Button onClick={onClose} color="inherit">Anuluj</Button>
                <Button onClick={handleSubmit} variant="contained" sx={{ px: 4 }}>Zapisz</Button>
            </DialogActions>
        </Dialog>
    );
};

export default AddUserModal;