import { useEffect, useState } from 'react';
import {
    Box, Typography, Paper, CircularProgress, Alert,
    Table, TableBody, TableCell, TableContainer, TableRow
} from '@mui/material';
import axiosClient from '../services/axiosClient';
import { useAuth } from '../hooks/AuthProvider';

interface ManagerView {
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

export default function Managerashboard() {
    const { user, loading: authLoading } = useAuth();
    const [manager, setManager] = useState<ManagerView | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const fetchManagerData = async () => {
            try {
                const resp = await axiosClient.get<ApiResponse<ManagerView>>('/manager/me');
                setManager(resp.data.data);
            } catch (err: any) {
                const msg = err?.response?.data?.message || err?.message || 'B³¹d pobierania danych';
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

    if (authLoading || loading) {
        return (
            <Box display="flex" justifyContent="center" alignItems="center" height="60vh">
                <CircularProgress />
            </Box>
        );
    }

    if (!user) return <Box m={2}><Alert severity="warning">Zaloguj siê, aby zobaczyæ swoje dane.</Alert></Box>;
    if (error) return <Box m={2}><Alert severity="error">B³¹d: {error}</Alert></Box>;
    if (!manager) return <Box m={2}><Typography>Nie znaleziono danych u¿ytkownika.</Typography></Box>;

    const rows = [
        { label: 'Imiê i Nazwisko', value: `${manager.firstName} ${manager.lastName}` },
        { label: 'Stanowisko', value: manager.position },
        { label: 'Okres oceny', value: manager.period },
        { label: 'Wynik koñcowy', value: manager.finalScore },
        { label: 'Podsumowanie osi¹gniêæ', value: manager.achievementsSummary },
    ];

    return (
        <Box p={3} maxWidth="800px" mx="auto">
            <Typography variant="h4" gutterBottom sx={{ fontWeight: 'bold', mb: 3 }}>
                Informacje
            </Typography>

            <TableContainer component={Paper} elevation={3}>
                <Table>
                    <TableBody>
                        {rows.map((row) => (
                            <TableRow key={row.label} sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>
                                <TableCell component="th" scope="row" sx={{ fontWeight: 'bold', width: '30%', backgroundColor: '#f5f5f5' }}>
                                    {row.label}
                                </TableCell>
                                <TableCell sx={{ whiteSpace: 'pre-line' }}>
                                    {row.value}
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </TableContainer>
        </Box>
    );
}