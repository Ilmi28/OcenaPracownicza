import { useEffect, useState } from 'react';
import { 
    Box, Typography, CircularProgress, Alert, 
    TextField,
    Paper,
    Grid,
    Button
} from '@mui/material';
import axiosClient from '../services/axiosClient';
import { useAuth } from '../hooks/AuthProvider';

interface EmployeeView {
    id?: string;
    firstName: string;
    lastName: string;
    position: string;
    period: string;
    finalScore: string;
    achievementsSummary: string;
}

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}


export default function EmployeeDashboard() {
    const { user, loading: authLoading } = useAuth();
    const [employee, setEmployee] = useState<EmployeeView | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [editMode, setEditMode] = useState<boolean>(false);
    const [newEmployee, setNewEmployee] = useState<EmployeeView | null>(null);
    const [saving, setSaving] = useState<boolean>(false);

    useEffect(() => {
        const fetchEmployeeData = async () => {
            try {
                const resp = await axiosClient.get<ApiResponse<EmployeeView>>('/employee/me');
                setEmployee(resp.data.data);
            } catch (err: any) {
                const msg = err?.response?.data?.message || err?.message || 'Błąd pobierania danych';
                setError(msg);
            } finally {
                setLoading(false);
            }
        };

        if (!authLoading && user) {
            fetchEmployeeData();
        } else if (!authLoading && !user) {
            setLoading(false);
        }
    }, [user, authLoading]);

    if (authLoading || loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" height="60vh">
                <CircularProgress />
            </Box>
        );
    }

    if (!user) return <Box m={2}><Alert severity="warning">Zaloguj się, aby zobaczyć swoje dane.</Alert></Box>;
    if (error) return <Box m={2}><Alert severity="error">Błąd: {error}</Alert></Box>;
    if (!employee) return <Box m={2}><Typography>Nie znaleziono danych użytkownika.</Typography></Box>;

    const handleSave = async () => {
        if (!newEmployee || !employee?.id) {
            setError('Brak danych do zapisania');
            return;
        }

        setSaving(true);
        setError(null);

        try {
            const payload = {
                userName: user?.email || '',
                email: user?.email || '',
                firstName: newEmployee.firstName,
                lastName: newEmployee.lastName,
                position: newEmployee.position,
                period: newEmployee.period,
                finalScore: newEmployee.finalScore,
                achievementsSummary: newEmployee.achievementsSummary,
            };
            await axiosClient.put(`/employee/${employee.id}`, payload);

            const resp = await axiosClient.get<ApiResponse<EmployeeView>>('/employee/me');
            setEmployee(resp.data.data);
            setEditMode(false);
            setNewEmployee(null);
        } catch (err: any) {
            const msg = err?.response?.data?.message || err?.message || 'Błąd podczas zapisywania danych';
            setError(msg);
        } finally {
            setSaving(false);
        }
    }


    const currentEmployee = editMode && newEmployee ? newEmployee : employee;

    return (
        <Box>
            <Typography variant="h5" fontWeight="600">Mój Profil</Typography>
            <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
                Zarządzaj swoimi danymi osobowymi
            </Typography>

            {error && (
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                </Alert>
            )}

            <Paper sx={{ p: 3, mb: 3 }}>
                <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                    <Typography variant="subtitle1" fontWeight="600">
                        Dane osobowe
                    </Typography>
                    {!editMode && (
                        <Button 
                            variant="outlined" 
                            size="small"
                            onClick={() => {
                                setNewEmployee(employee);
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
                            label="Imię" 
                            value={currentEmployee.firstName} 
                            disabled={!editMode}
                            size="small"
                            onChange={(e) =>
                                setNewEmployee((prev) =>
                                    prev ? { ...prev, firstName: e.target.value } : prev
                                )
                            }
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField 
                            fullWidth 
                            label="Nazwisko" 
                            value={currentEmployee.lastName} 
                            disabled={!editMode}
                            size="small"
                            onChange={(e) =>
                                setNewEmployee((prev) =>
                                    prev ? { ...prev, lastName: e.target.value } : prev
                                )
                            }
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField 
                            fullWidth 
                            label="Stanowisko" 
                            value={currentEmployee.position} 
                            disabled={!editMode}
                            size="small"
                            onChange={(e) =>
                                setNewEmployee((prev) =>
                                    prev ? { ...prev, position: e.target.value } : prev
                                )
                            }
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField 
                            fullWidth 
                            label="Okres oceny" 
                            value={currentEmployee.period} 
                            disabled={!editMode}
                            size="small"
                            onChange={(e) =>
                                setNewEmployee((prev) =>
                                    prev ? { ...prev, period: e.target.value } : prev
                                )
                            }
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField 
                            fullWidth 
                            label="Wynik końcowy" 
                            value={currentEmployee.finalScore} 
                            disabled={!editMode}
                            size="small"
                            onChange={(e) =>
                                setNewEmployee((prev) =>
                                    prev ? { ...prev, finalScore: e.target.value } : prev
                                )
                            }
                        />
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                        <TextField 
                            fullWidth 
                            label="Podsumowanie osiągnięć" 
                            value={currentEmployee.achievementsSummary} 
                            disabled={!editMode}
                            size="small"
                            multiline
                            rows={4}
                            onChange={(e) =>
                                setNewEmployee((prev) =>
                                    prev ? { ...prev, achievementsSummary: e.target.value } : prev
                                )
                            }
                        />
                    </Grid>
                </Grid>

                {editMode && (
                    <Box mt={3} display="flex" justifyContent="flex-end" gap={2}>
                        <Button 
                            variant="outlined" 
                            onClick={() => {
                                setEditMode(false);
                                setNewEmployee(null);
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
                            {saving ? 'Zapisywanie...' : 'Zapisz'}
                        </Button>
                    </Box>
                )}
            </Paper>
        </Box>
    );
}