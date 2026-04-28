import { useCallback, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
    Alert,
    Box,
    Button,
    Chip,
    CircularProgress,
    Grid,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableRow,
    Typography,
} from "@mui/material";
import { Stage2ReviewDetailsView } from "../utils/types";
import { evaluationService } from "../services/evaluationService";

const STATUS_LABELS: Record<number, string> = {
    0: "Szkic",
    1: "Oczekuje na etap 2",
    2: "Zatwierdzona",
    3: "Odrzucona",
    4: "Zamknięta",
    5: "Zarchiwizowana",
};

export default function EmployeeEvaluationHistoryDetails() {
    const { achievementId } = useParams<{ achievementId: string }>();
    const navigate = useNavigate();
    const [data, setData] = useState<Stage2ReviewDetailsView | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const load = useCallback(async () => {
        if (!achievementId) return;

        setLoading(true);
        setError(null);
        try {
            const response = await evaluationService.getDetails(achievementId);
            setData(response);
        } catch (err: any) {
            const msg =
                err?.response?.data?.message ??
                err?.message ??
                "Nie udało się pobrać szczegółów historii oceny.";
            setError(msg);
        } finally {
            setLoading(false);
        }
    }, [achievementId]);

    useEffect(() => {
        load();
    }, [load]);

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

    return (
        <Box>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
                <Typography variant="h5" fontWeight={600}>
                    Szczegóły historii oceny
                </Typography>
                <Button variant="outlined" onClick={() => navigate("/evaluation/history")}>
                    Wróć do historii
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
                            Osiągnięcie
                        </Typography>
                        <Typography>{data.achievementName}</Typography>
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
                            <TableCell>Okres</TableCell>
                            <TableCell>Wynik</TableCell>
                            <TableCell>Status</TableCell>
                            <TableCell>Komentarz</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {data.achievements.map((item) => (
                            <TableRow key={item.id}>
                                <TableCell>{new Date(item.date).toLocaleDateString("pl-PL")}</TableCell>
                                <TableCell>{item.name}</TableCell>
                                <TableCell>{item.description}</TableCell>
                                <TableCell>{item.period}</TableCell>
                                <TableCell>{item.finalScore}</TableCell>
                                <TableCell>{STATUS_LABELS[item.stage2Status] ?? "Nieznany"}</TableCell>
                                <TableCell>{item.stage2Comment ?? "-"}</TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </Paper>

            <Paper sx={{ p: 3 }}>
                <Typography variant="subtitle1" fontWeight={600} mb={2}>
                    Informacje o decyzji
                </Typography>
                <Typography variant="body2" color="text.secondary">
                    Komentarz
                </Typography>
                <Typography mb={2}>{data.stage2Comment ?? "-"}</Typography>
                <Typography variant="body2" color="text.secondary">
                    Data oceny
                </Typography>
                <Typography>
                    {data.stage2ReviewedAtUtc
                        ? new Date(data.stage2ReviewedAtUtc).toLocaleString("pl-PL")
                        : "-"}
                </Typography>
            </Paper>
        </Box>
    );
}
