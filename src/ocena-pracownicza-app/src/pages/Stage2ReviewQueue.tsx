import { useEffect, useState } from "react";
import {
    Alert,
    Box,
    CircularProgress,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableRow,
    Typography,
    Button,
    Chip,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { Stage2ReviewItemView } from "../utils/types";
import { evaluationService } from "../services/evaluationService";

const STATUS_LABELS: Record<number, string> = {
    0: "Szkic",
    1: "Oczekuje na etap 2",
    2: "Zatwierdzona",
    3: "Odrzucona",
    4: "Zamknięta",
    5: "Zarchiwizowana",
};

export default function Stage2ReviewQueue() {
    const navigate = useNavigate();
    const [data, setData] = useState<Stage2ReviewItemView[]>([]);
    const [showArchived, setShowArchived] = useState(false);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const load = async () => {
        setLoading(true);
        setError(null);
        try {
            const response = showArchived
                ? await evaluationService.getArchived()
                : await evaluationService.getPending();
            setData(response);
        } catch (err: any) {
            const msg =
                err?.response?.data?.message ??
                err?.message ??
                "Nie udało się pobrać listy ocen.";
            setError(msg);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        load();
    }, [showArchived]);

    if (loading) {
        return (
            <Box display="flex" justifyContent="center" mt={8}>
                <CircularProgress />
            </Box>
        );
    }

    return (
        <Box>
            <Typography variant="h5" fontWeight={600} mb={1}>
                {showArchived ? "Archiwum ocen" : "Kolejka etapu 2"}
            </Typography>
            <Typography variant="body2" color="text.secondary" mb={3}>
                {showArchived
                    ? "Zarchiwizowane oceny dostępne do podglądu."
                    : "Weryfikacja oceny i osiągnięć przez Manager/Admin."}
            </Typography>
            {error && (
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                </Alert>
            )}
            <Paper>
                <Box display="flex" justifyContent="flex-end" gap={1} p={2}>
                    <Button
                        variant={showArchived ? "contained" : "outlined"}
                        onClick={() => setShowArchived((prev) => !prev)}
                    >
                        {showArchived ? "Pokaż kolejkę" : "Pokaż archiwum"}
                    </Button>
                    <Button variant="outlined" onClick={load}>
                        Odśwież
                    </Button>
                </Box>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>Pracownik</TableCell>
                            <TableCell>Stanowisko</TableCell>
                            <TableCell>Okres</TableCell>
                            <TableCell>Wynik</TableCell>
                            <TableCell>Status</TableCell>
                            <TableCell>Osiągnięcia</TableCell>
                            <TableCell align="right">Akcje</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {data.map((item) => (
                            <TableRow key={item.employeeId}>
                                <TableCell>{item.fullName}</TableCell>
                                <TableCell>{item.position}</TableCell>
                                <TableCell>{item.period}</TableCell>
                                <TableCell>{item.finalScore}</TableCell>
                                <TableCell>
                                    <Chip
                                        size="small"
                                        label={
                                            STATUS_LABELS[item.stage2Status] ??
                                            "Nieznany"
                                        }
                                        color={
                                            item.stage2Status === 2
                                                ? "success"
                                                : item.stage2Status === 3
                                                  ? "error"
                                                  : item.stage2Status >= 4
                                                    ? "default"
                                                    : "warning"
                                        }
                                        variant="outlined"
                                    />
                                </TableCell>
                                <TableCell>{item.achievementsCount}</TableCell>
                                <TableCell align="right">
                                    <Button
                                        size="small"
                                        variant="contained"
                                        onClick={() =>
                                            navigate(
                                                `/evaluation/stage2/${item.employeeId}`,
                                            )
                                        }
                                    >
                                        Szczegóły
                                    </Button>
                                </TableCell>
                            </TableRow>
                        ))}
                        {data.length === 0 && (
                            <TableRow>
                                <TableCell colSpan={7} align="center">
                                    {showArchived
                                        ? "Brak zarchiwizowanych ocen."
                                        : "Brak rekordów oczekujących na etap 2."}
                                </TableCell>
                            </TableRow>
                        )}
                    </TableBody>
                </Table>
            </Paper>
        </Box>
    );
}
