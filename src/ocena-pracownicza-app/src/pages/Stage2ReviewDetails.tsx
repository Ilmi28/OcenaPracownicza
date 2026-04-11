import { useCallback, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
    Alert,
    Box,
    Button,
    CircularProgress,
    Grid,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableRow,
    TextField,
    Typography,
    Chip,
} from "@mui/material";
import { Stage2ReviewDetailsView } from "../utils/types";
import { evaluationService } from "../services/evaluationService";
import { useAuth } from "../hooks/AuthProvider";

const STATUS_LABELS: Record<number, string> = {
    0: "Szkic",
    1: "Oczekuje na etap 2",
    2: "Zatwierdzona",
    3: "Odrzucona",
    4: "Zamknięta",
    5: "Zarchiwizowana",
};

export default function Stage2ReviewDetails() {
    const { employeeId } = useParams<{ employeeId: string }>();
    const navigate = useNavigate();
    const { user } = useAuth();
    const [data, setData] = useState<Stage2ReviewDetailsView | null>(null);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [comment, setComment] = useState("");
    const [error, setError] = useState<string | null>(null);

    const load = useCallback(async () => {
        if (!employeeId) return;
        setLoading(true);
        setError(null);
        try {
            const response = await evaluationService.getDetails(employeeId);
            setData(response);
            setComment(response.stage2Comment ?? "");
        } catch (err: any) {
            const msg =
                err?.response?.data?.message ??
                err?.message ??
                "Nie udało się pobrać szczegółów.";
            setError(msg);
        } finally {
            setLoading(false);
        }
    }, [employeeId]);

    useEffect(() => {
        load();
    }, [load]);

    const onApprove = async () => {
        if (!employeeId) return;
        setSaving(true);
        setError(null);
        try {
            const updated = await evaluationService.approve(employeeId, comment);
            setData(updated);
        } catch (err: any) {
            setError(err?.response?.data?.message ?? "Nie udało się zatwierdzić.");
        } finally {
            setSaving(false);
        }
    };

    const onReject = async () => {
        if (!employeeId) return;
        setSaving(true);
        setError(null);
        try {
            const updated = await evaluationService.reject(employeeId, comment);
            setData(updated);
        } catch (err: any) {
            setError(err?.response?.data?.message ?? "Nie udało się odrzucić.");
        } finally {
            setSaving(false);
        }
    };

    const onClose = async () => {
        if (!employeeId) return;
        setSaving(true);
        setError(null);
        try {
            const updated = await evaluationService.close(employeeId);
            setData(updated);
        } catch (err: any) {
            setError(err?.response?.data?.message ?? "Nie udało się zamknąć oceny.");
        } finally {
            setSaving(false);
        }
    };

    const onArchive = async () => {
        if (!employeeId) return;
        setSaving(true);
        setError(null);
        try {
            const updated = await evaluationService.archive(employeeId);
            setData(updated);
        } catch (err: any) {
            setError(err?.response?.data?.message ?? "Nie udało się zarchiwizować oceny.");
        } finally {
            setSaving(false);
        }
    };

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" mt={8}>
                <CircularProgress />
            </Box>
        );
    }

    if (!data) {
        return <Alert severity="warning">Brak danych rekordu.</Alert>;
    }

    const canDecide = data.stage2Status === 1;
    const isAdmin = user?.role === "Admin";
    const canClose = isAdmin && (data.stage2Status === 2 || data.stage2Status === 3);
    const canArchive = isAdmin && data.stage2Status === 4;

    return (
        <Box>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
                <Typography variant="h5" fontWeight={600}>
                    Weryfikacja etapu 2
                </Typography>
                <Button variant="outlined" onClick={() => navigate("/evaluation/stage2")}>
                    Wróć do kolejki
                </Button>
            </Box>
            {error && (
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                </Alert>
            )}
            <Paper sx={{ p: 3, mb: 3 }}>
                <Grid container spacing={2}>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <Typography variant="body2" color="text.secondary">
                            Pracownik
                        </Typography>
                        <Typography>{data.fullName}</Typography>
                    </Grid>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <Typography variant="body2" color="text.secondary">
                            Status
                        </Typography>
                        <Chip
                            label={STATUS_LABELS[data.stage2Status] ?? "Nieznany"}
                            color={data.stage2Status === 2 ? "success" : data.stage2Status === 3 ? "error" : "warning"}
                            variant="outlined"
                            size="small"
                        />
                    </Grid>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <Typography variant="body2" color="text.secondary">
                            Okres
                        </Typography>
                        <Typography>{data.period}</Typography>
                    </Grid>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <Typography variant="body2" color="text.secondary">
                            Wynik końcowy
                        </Typography>
                        <Typography>{data.finalScore}</Typography>
                    </Grid>
                    <Grid size={{ xs: 12 }}>
                        <Typography variant="body2" color="text.secondary">
                            Podsumowanie osiągnięć
                        </Typography>
                        <Typography>{data.achievementsSummary}</Typography>
                    </Grid>
                </Grid>
            </Paper>

            <Paper sx={{ p: 3, mb: 3 }}>
                <Typography variant="subtitle1" fontWeight={600} mb={2}>
                    Osiągnięcia
                </Typography>
                <Table size="small">
                    <TableHead>
                        <TableRow>
                            <TableCell>Data</TableCell>
                            <TableCell>Nazwa</TableCell>
                            <TableCell>Opis</TableCell>
                            <TableCell>Status</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {data.achievements.map((item) => (
                            <TableRow key={item.id}>
                                <TableCell>{new Date(item.date).toLocaleDateString("pl-PL")}</TableCell>
                                <TableCell>{item.name}</TableCell>
                                <TableCell>{item.description}</TableCell>
                                <TableCell>{STATUS_LABELS[item.stage2Status] ?? "Nieznany"}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </Paper>

            <Paper sx={{ p: 3 }}>
                <Typography variant="subtitle1" fontWeight={600} mb={2}>
                    Decyzja komisji
                </Typography>
                <TextField
                    fullWidth
                    multiline
                    minRows={3}
                    label="Komentarz (wymagany przy odrzuceniu)"
                    value={comment}
                    onChange={(e) => setComment(e.target.value)}
                    disabled={saving || !canDecide}
                    sx={{ mb: 2 }}
                />
                <Box display="flex" gap={2} justifyContent="flex-end">
                    <Button variant="outlined" color="error" disabled={saving || !canDecide} onClick={onReject}>
                        Odrzuć
                    </Button>
                    <Button variant="contained" disabled={saving || !canDecide} onClick={onApprove}>
                        Zatwierdź
                    </Button>
                    <Button variant="outlined" disabled={saving || !canClose} onClick={onClose}>
                        Zamknij ocenę
                    </Button>
                    <Button variant="contained" color="secondary" disabled={saving || !canArchive} onClick={onArchive}>
                        Archiwizuj ocenę
                    </Button>
                </Box>
            </Paper>
        </Box>
    );
}
