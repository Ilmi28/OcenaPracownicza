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
    MenuItem,       
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { evaluationService } from "../services/evaluationService";
import axiosClient from "../services/axiosClient"; 
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

interface EvaluationPeriod {
    id: string;
    name: string;
}

export default function EmployeeEvaluationHistory() {
    const navigate = useNavigate();
    const { user } = useAuth();
    const [data, setData] = useState<Stage2HistoryItemView[]>([]);
    const [periods, setPeriods] = useState<EvaluationPeriod[]>([]);          
    const [employeeFilter, setEmployeeFilter] = useState("");
    const [positionFilter, setPositionFilter] = useState("");
    const [periodFilter, setPeriodFilter] = useState("all");          
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const fetchPeriods = async () => {
        try {
            const resp = await axiosClient.get("/evaluation-periods");
            setPeriods(resp.data);
        } catch (err) {
            console.error("Błąd pobierania okresów", err);
        }
    };

    const load = useCallback(async () => {
        setLoading(true);
        setError(null);
        try {
            const response =
                user?.role === "Employee"
                    ? await evaluationService.getMyStage2History()
                    : await evaluationService.getStage2History();
            setData(response);
            await fetchPeriods();                   
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
            const matchesPeriod = 
                periodFilter === "all" || 
                item.period === periodFilter;

            return matchesEmployee && matchesPosition && matchesPeriod;
        });
    }, [data, employeeFilter, positionFilter, periodFilter]);

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
                <Box display="flex" gap={2} p={2} flexWrap="wrap" alignItems="center">
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
                    
                    {            }
                    <TextField
                        select
                        size="small"
                        label="Okres"
                        value={periodFilter}
                        onChange={(e) => setPeriodFilter(e.target.value)}
                        sx={{ minWidth: 200 }}
                    >
                        <MenuItem value="all">Wszystkie okresy</MenuItem>
                        {periods.map((p) => (
                            <MenuItem key={p.id} value={p.name}>
                                {p.name}
                            </MenuItem>
                        ))}
                    </TextField>

                    <Box ml="auto">
                        <Button variant="outlined" onClick={load}>
                            Odśwież
                        </Button>
                    </Box>
                </Box>
                
                <Table>
                    <TableHead>
                        <TableRow sx={{ bgcolor: "grey.50" }}>
                            {user?.role !== "Employee" && <TableCell sx={{ fontWeight: 700 }}>Pracownik</TableCell>}
                            {user?.role !== "Employee" && <TableCell sx={{ fontWeight: 700 }}>Stanowisko</TableCell>}
                            <TableCell sx={{ fontWeight: 700 }}>Osiągnięcie</TableCell>
                            <TableCell sx={{ fontWeight: 700 }}>Okres</TableCell>
                            <TableCell sx={{ fontWeight: 700 }}>Wynik</TableCell>
                            <TableCell sx={{ fontWeight: 700 }}>Data</TableCell>
                            <TableCell sx={{ fontWeight: 700 }}>Status</TableCell>
                            <TableCell sx={{ fontWeight: 700 }} align="right">Akcje</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {filteredData.map((item) => (
                            <TableRow key={item.achievementId} hover>
                                {user?.role !== "Employee" && <TableCell>{item.fullName}</TableCell>}
                                {user?.role !== "Employee" && <TableCell>{item.position}</TableCell>}
                                <TableCell sx={{ fontWeight: 500 }}>{item.achievementName}</TableCell>
                                <TableCell>{item.period}</TableCell>
                                <TableCell>{item.finalScore}</TableCell>
                                <TableCell>
                                    {new Date(item.date).toLocaleDateString("pl-PL")}
                                </TableCell>
                                <TableCell>
                                    <Chip
                                        size="small"
                                        label={STATUS_LABELS[item.stage2Status] ?? "Nieznany"}
                                        color={
                                            item.stage2Status === 2 ? "success" : 
                                            item.stage2Status === 3 ? "error" : 
                                            item.stage2Status >= 4 ? "default" : "warning"
                                        }
                                        variant="outlined"
                                        sx={{ fontWeight: 600 }}
                                    />
                                </TableCell>
                                <TableCell align="right">
                                    <Button
                                        size="small"
                                        variant="contained"
                                        onClick={() => navigate(`/evaluation/history/${item.achievementId}`)}
                                    >
                                        Szczegóły
                                    </Button>
                                </TableCell>
                            </TableRow>
                        ))}
                        {filteredData.length === 0 && (
                            <TableRow>
                                <TableCell colSpan={user?.role === "Employee" ? 6 : 8} align="center" sx={{ py: 3 }}>
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