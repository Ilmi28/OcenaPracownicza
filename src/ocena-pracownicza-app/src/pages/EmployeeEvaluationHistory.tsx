import { useCallback, useEffect, useMemo, useState } from "react";
import {
    Alert,
    Box,
    Button,
    Chip,
    CircularProgress,
    Paper,
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableRow,
    TextField,
    Typography,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { evaluationService } from "../services/evaluationService";
import { Stage2HistoryItemView } from "../utils/types";
import { useAuth } from "../hooks/AuthProvider";

const STATUS_LABELS: Record<number, string> = {
    0: "Szkic",
    1: "Oczekuje na etap 2",
    2: "Zatwierdzona",
    3: "Odrzucona",
    4: "Zamknięta",
    5: "Zarchiwizowana",
};

export default function EmployeeEvaluationHistory() {
    const navigate = useNavigate();
    const { user } = useAuth();
    const [data, setData] = useState<Stage2HistoryItemView[]>([]);
    const [employeeFilter, setEmployeeFilter] = useState("");
    const [positionFilter, setPositionFilter] = useState("");
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const filteredData = useMemo(() => {
        const normalizedEmployee = employeeFilter.trim().toLowerCase();
        const normalizedPosition = positionFilter.trim().toLowerCase();

        return data.filter((item) => {
            const matchesEmployee =
                normalizedEmployee.length === 0 ||
                item.fullName.toLowerCase().includes(normalizedEmployee);
            const matchesPosition =
                normalizedPosition.length === 0 ||
                item.position.toLowerCase().includes(normalizedPosition);

            return matchesEmployee && matchesPosition;
        });
    }, [data, employeeFilter, positionFilter]);

    const load = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const response =
                user?.role === "Employee"
                    ? await evaluationService.getMyStage2History()
                    : await evaluationService.getStage2History();
            setData(response);
        } catch (err: any) {
            const msg =
                err?.response?.data?.message ??
                err?.message ??
                "Nie udało się pobrać historii ocen.";
            setError(msg);
        } finally {
            setLoading(false);
        }
    }, [user?.role]);

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

    return (
        <Box>
            <Typography variant="h5" fontWeight={600} mb={1}>
                {user?.role === "Employee"
                    ? "Moja historia ocen"
                    : "Historia ocen pracowników"}
            </Typography>
            <Typography variant="body2" color="text.secondary" mb={3}>
                {user?.role === "Employee"
                    ? "Przeglądaj historię swoich ocen wraz z punktacją i komentarzami."
                    : "Lista wszystkich rekordów ocen dostępnych do wglądu na etapie 2."}
            </Typography>

            {error && (
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                </Alert>
            )}

            <Paper>
                <Box display="flex" gap={2} p={2} flexWrap="wrap">
                    {user?.role !== "Employee" && (
                        <TextField
                            size="small"
                            label="Pracownik"
                            value={employeeFilter}
                            onChange={(e) => setEmployeeFilter(e.target.value)}
                        />
                    )}
                    {user?.role !== "Employee" && (
                        <TextField
                            size="small"
                            label="Stanowisko"
                            value={positionFilter}
                            onChange={(e) => setPositionFilter(e.target.value)}
                        />
                    )}
                    <Box ml="auto">
                        <Button variant="outlined" onClick={load}>
                            Odśwież
                        </Button>
                    </Box>
                </Box>
                <Table>
                    <TableHead>
                        <TableRow>
                            {user?.role !== "Employee" && <TableCell>Pracownik</TableCell>}
                            {user?.role !== "Employee" && <TableCell>Stanowisko</TableCell>}
                            <TableCell>Osiągnięcie</TableCell>
                            <TableCell>Okres</TableCell>
                            <TableCell>Wynik</TableCell>
                            <TableCell>Data</TableCell>
                            <TableCell>Status</TableCell>
                            <TableCell align="right">Akcje</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {filteredData.map((item) => (
                            <TableRow key={item.achievementId}>
                                {user?.role !== "Employee" && <TableCell>{item.fullName}</TableCell>}
                                {user?.role !== "Employee" && <TableCell>{item.position}</TableCell>}
                                <TableCell>{item.achievementName}</TableCell>
                                <TableCell>{item.period}</TableCell>
                                <TableCell>{item.finalScore}</TableCell>
                                <TableCell>
                                    {new Date(item.date).toLocaleDateString("pl-PL")}
                                </TableCell>
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
                                <TableCell align="right">
                                    <Button
                                        size="small"
                                        variant="contained"
                                        onClick={() =>
                                            navigate(
                                                `/evaluation/history/${item.achievementId}`,
                                            )
                                        }
                                    >
                                        Szczegóły
                                    </Button>
                                </TableCell>
                            </TableRow>
                        ))}
                        {filteredData.length === 0 && (
                            <TableRow>
                                <TableCell colSpan={user?.role === "Employee" ? 6 : 8} align="center">
                                    {data.length === 0
                                        ? "Brak rekordów historii ocen."
                                        : "Brak rekordów spełniających kryteria filtrowania."}
                                </TableCell>
                            </TableRow>
                        )}
                    </TableBody>
                </Table>
            </Paper>
        </Box>
    );
}
