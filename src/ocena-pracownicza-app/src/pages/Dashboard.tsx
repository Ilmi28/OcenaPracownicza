import { useCallback, useEffect, useMemo, useState } from "react";
import {
    Alert,
    Box,
    Button,
    Chip,
    CircularProgress,
    Grid,
    Paper,
    Stack,
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableRow,
    Typography,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/AuthProvider";
import axiosClient from "../services/axiosClient";
import { evaluationService } from "../services/evaluationService";
import { Stage2HistoryItemView, Stage2ReviewItemView } from "../utils/types";

interface ApiResponse<T> {
    success: boolean;
    message: string;
    data: T;
}

interface CategoryProgressView {
    categoryId: number;
    categoryName: string;
    currentCount: number;
    requiredCount: number;
}

interface EmployeeProgressView {
    totalAchievements: number;
    metCategoriesCount: number;
    totalRequiredCategories: number;
    progressPercentage: number;
    categories: CategoryProgressView[];
}

interface BasicPersonView {
    id: string;
}

interface EvaluationPeriod {
    id?: number;
    name: string;
    startDate: string;
    endDate: string;
}

const STATUS_LABELS: Record<number, string> = {
    0: "Szkic",
    1: "Oczekuje na etap 2",
    2: "Zatwierdzona",
    3: "Odrzucona",
    4: "Zamknięta",
    5: "Zarchiwizowana",
};

const statusChipColor = (status: number) =>
    status === 2 ? "success" : status === 3 ? "error" : status >= 4 ? "default" : "warning";

const isApiResponse = <T,>(payload: unknown): payload is ApiResponse<T> =>
    typeof payload === "object" && payload !== null && "data" in payload;

const unwrapData = <T,>(payload: T | ApiResponse<T>): T =>
    isApiResponse<T>(payload) ? payload.data : payload;

const formatDate = (isoDate: string) => new Date(isoDate).toLocaleDateString("pl-PL");

const MetricCard = ({ title, value, subtitle }: { title: string; value: string | number; subtitle?: string }) => (
    <Paper sx={{ p: 2.5 }}>
        <Typography variant="body2" color="text.secondary">
            {title}
        </Typography>
        <Typography variant="h5" mt={0.5}>
            {value}
        </Typography>
        {subtitle && (
            <Typography variant="body2" color="text.secondary" mt={0.5}>
                {subtitle}
            </Typography>
        )}
    </Paper>
);

export default function Dashboard() {
    const { user, loading: authLoading } = useAuth();
    const navigate = useNavigate();

    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const [employeeProgress, setEmployeeProgress] = useState<EmployeeProgressView | null>(null);
    const [employeeHistory, setEmployeeHistory] = useState<Stage2HistoryItemView[]>([]);

    const [pendingReviews, setPendingReviews] = useState<Stage2ReviewItemView[]>([]);
    const [stage2History, setStage2History] = useState<Stage2HistoryItemView[]>([]);

    const [employeesCount, setEmployeesCount] = useState<number>(0);
    const [managersCount, setManagersCount] = useState<number>(0);
    const [adminsCount, setAdminsCount] = useState<number>(0);
    const [activePeriods, setActivePeriods] = useState<EvaluationPeriod[]>([]);

    const loadData = useCallback(async () => {
        if (!user) {
            setLoading(false);
            return;
        }

        setLoading(true);
        setError(null);

        try {
            if (user.role === "Employee") {
                const [progressResp, historyResp] = await Promise.all([
                    axiosClient.get<ApiResponse<EmployeeProgressView>>("/progress/me"),
                    evaluationService.getMyStage2History(),
                ]);

                setEmployeeProgress(progressResp.data.data);
                setEmployeeHistory(historyResp);
                return;
            }

            if (user.role === "Manager") {
                const [pendingResp, historyResp, employeesResp] = await Promise.all([
                    evaluationService.getPending(),
                    evaluationService.getStage2History(),
                    axiosClient.get<ApiResponse<BasicPersonView[]>>("/employee"),
                ]);

                setPendingReviews(pendingResp);
                setStage2History(historyResp);
                setEmployeesCount(employeesResp.data.data.length);
                return;
            }

            if (user.role === "Admin") {
                const [pendingResp, historyResp, employeesResp, managersResp, adminsResp, periodsResp] =
                    await Promise.all([
                        evaluationService.getPending(),
                        evaluationService.getStage2History(),
                        axiosClient.get<ApiResponse<BasicPersonView[]>>("/employee"),
                        axiosClient.get<ApiResponse<BasicPersonView[]>>("/manager"),
                        axiosClient.get<ApiResponse<BasicPersonView[]>>("/admin"),
                        axiosClient.get<EvaluationPeriod[] | ApiResponse<EvaluationPeriod[]>>("/evaluation-periods"),
                    ]);

                const periods = unwrapData<EvaluationPeriod[]>(periodsResp.data);
                const now = new Date();

                setPendingReviews(pendingResp);
                setStage2History(historyResp);
                setEmployeesCount(employeesResp.data.data.length);
                setManagersCount(managersResp.data.data.length);
                setAdminsCount(adminsResp.data.data.length);
                setActivePeriods(
                    periods.filter((p) => new Date(p.startDate) <= now && new Date(p.endDate) >= now),
                );
            }
        } catch (err: unknown) {
            const message =
                err instanceof Error ? err.message : "Nie udało się pobrać danych dashboardu.";
            setError(message);
        } finally {
            setLoading(false);
        }
    }, [user]);

    useEffect(() => {
        if (!authLoading) {
            loadData();
        }
    }, [authLoading, loadData]);

    const reviewedCount = useMemo(
        () =>
            stage2History.filter((item) =>
                [2, 3, 4, 5].includes(item.stage2Status),
            ).length,
        [stage2History],
    );

    const recentHistory = useMemo(
        () => (user?.role === "Employee" ? employeeHistory : stage2History).slice(0, 5),
        [employeeHistory, stage2History, user?.role],
    );

    if (authLoading || loading) {
        return (
            <Box display="flex" justifyContent="center" mt={8}>
                <CircularProgress />
            </Box>
        );
    }

    if (!user) {
        return <Alert severity="warning">Zaloguj się, aby zobaczyć dashboard.</Alert>;
    }

    return (
        <Box>
            <Typography variant="h5" fontWeight={700} mb={0.5}>
                Dashboard
            </Typography>
            <Typography variant="body2" color="text.secondary" mb={3}>
                {user.role === "Employee"
                    ? "Podsumowanie Twoich postępów i ostatnich ocen."
                    : user.role === "Manager"
                      ? "Najważniejsze informacje o procesie ocen i zadaniach do weryfikacji."
                      : "Przegląd systemu ocen i kluczowych danych administracyjnych."}
            </Typography>

            {error && (
                <Alert severity="error" sx={{ mb: 2 }}>
                    {error}
                </Alert>
            )}

            {user.role === "Employee" && employeeProgress && (
                <>
                    <Grid container spacing={2} mb={3}>
                        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                            <MetricCard
                                title="Wszystkie osiągnięcia"
                                value={employeeProgress.totalAchievements}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                            <MetricCard
                                title="Spełnione kategorie"
                                value={`${employeeProgress.metCategoriesCount}/${employeeProgress.totalRequiredCategories}`}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                            <MetricCard
                                title="Postęp wymagań"
                                value={`${employeeProgress.progressPercentage}%`}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                            <MetricCard
                                title="Rekordy historii ocen"
                                value={employeeHistory.length}
                            />
                        </Grid>
                    </Grid>

                    <Paper sx={{ p: 2.5, mb: 3 }}>
                        <Typography variant="subtitle1" fontWeight={700} mb={2}>
                            Postęp w kategoriach
                        </Typography>
                        <Stack direction="row" useFlexGap flexWrap="wrap" gap={1}>
                            {employeeProgress.categories.map((category) => (
                                <Chip
                                    key={category.categoryId}
                                    label={`${category.categoryName}: ${category.currentCount}/${category.requiredCount}`}
                                    color={category.currentCount >= category.requiredCount ? "success" : "default"}
                                    variant="outlined"
                                />
                            ))}
                        </Stack>
                    </Paper>
                </>
            )}

            {user.role === "Manager" && (
                <Grid container spacing={2} mb={3}>
                    <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                        <MetricCard title="Oczekujące oceny etap 2" value={pendingReviews.length} />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                        <MetricCard title="Ocenione rekordy" value={reviewedCount} />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                        <MetricCard title="Wszystkie rekordy historii" value={stage2History.length} />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                        <MetricCard title="Liczba pracowników" value={employeesCount} />
                    </Grid>
                </Grid>
            )}

            {user.role === "Admin" && (
                <>
                    <Grid container spacing={2} mb={3}>
                        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                            <MetricCard title="Pracownicy" value={employeesCount} />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                            <MetricCard title="Menedżerowie" value={managersCount} />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                            <MetricCard title="Administratorzy" value={adminsCount} />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                            <MetricCard title="Oczekujące oceny etap 2" value={pendingReviews.length} />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                            <MetricCard
                                title="Aktywne okresy oceny"
                                value={activePeriods.length}
                            />
                        </Grid>
                    </Grid>

                    {activePeriods.length > 0 && (
                        <Paper sx={{ p: 2.5, mb: 3 }}>
                            <Typography variant="subtitle1" fontWeight={700} mb={2}>
                                Aktywne okresy oceny
                            </Typography>
                            <Stack direction="row" useFlexGap flexWrap="wrap" gap={1}>
                                {activePeriods.map((period) => (
                                    <Chip
                                        key={period.id ?? period.name}
                                        label={`${period.name} (${formatDate(period.startDate)} - ${formatDate(period.endDate)})`}
                                        color="primary"
                                        variant="outlined"
                                    />
                                ))}
                            </Stack>
                        </Paper>
                    )}
                </>
            )}

            <Paper sx={{ p: 2.5, mb: 3 }}>
                <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
                    <Typography variant="subtitle1" fontWeight={700}>
                        Ostatnie rekordy ocen
                    </Typography>
                    <Button
                        size="small"
                        variant="outlined"
                        onClick={() => navigate("/evaluation/history")}
                    >
                        Zobacz historię
                    </Button>
                </Box>

                <Table size="small">
                    <TableHead>
                        <TableRow>
                            {user.role !== "Employee" && <TableCell>Pracownik</TableCell>}
                            <TableCell>Osiągnięcie</TableCell>
                            <TableCell>Okres</TableCell>
                            <TableCell>Wynik</TableCell>
                            <TableCell>Status</TableCell>
                            <TableCell>Data</TableCell>
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {recentHistory.length > 0 ? (
                            recentHistory.map((item) => (
                                <TableRow key={item.achievementId}>
                                    {user.role !== "Employee" && <TableCell>{item.fullName}</TableCell>}
                                    <TableCell>{item.achievementName}</TableCell>
                                    <TableCell>{item.period}</TableCell>
                                    <TableCell>{item.finalScore}</TableCell>
                                    <TableCell>
                                        <Chip
                                            size="small"
                                            label={STATUS_LABELS[item.stage2Status] ?? "Nieznany"}
                                            color={statusChipColor(item.stage2Status)}
                                            variant="outlined"
                                        />
                                    </TableCell>
                                    <TableCell>{formatDate(item.date)}</TableCell>
                                </TableRow>
                            ))
                        ) : (
                            <TableRow>
                                <TableCell colSpan={user.role === "Employee" ? 5 : 6} align="center">
                                    Brak danych do wyświetlenia.
                                </TableCell>
                            </TableRow>
                        )}
                    </TableBody>
                </Table>
            </Paper>

            <Paper sx={{ p: 2.5 }}>
                <Typography variant="subtitle1" fontWeight={700} mb={2}>
                    Szybkie akcje
                </Typography>
                <Stack direction="row" useFlexGap flexWrap="wrap" gap={1.5}>
                    <Button variant="contained" onClick={() => navigate("/settings")}>
                        Ustawienia konta
                    </Button>
                    {user.role === "Employee" && (
                        <>
                            <Button variant="outlined" onClick={() => navigate("/achievement/add")}>
                                Dodaj osiągnięcie
                            </Button>
                            <Button variant="outlined" onClick={() => navigate("/achievements")}>
                                Moje osiągnięcia
                            </Button>
                            <Button variant="outlined" onClick={() => navigate("/employee")}>
                                Mój profil
                            </Button>
                        </>
                    )}
                    {user.role === "Manager" && (
                        <>
                            <Button variant="outlined" onClick={() => navigate("/evaluation/stage2")}>
                                Kolejka etapu 2
                            </Button>
                            <Button variant="outlined" onClick={() => navigate("/users")}>
                                Użytkownicy
                            </Button>
                            <Button variant="outlined" onClick={() => navigate("/manager")}>
                                Mój profil
                            </Button>
                        </>
                    )}
                    {user.role === "Admin" && (
                        <>
                            <Button variant="outlined" onClick={() => navigate("/users")}>
                                Zarządzaj użytkownikami
                            </Button>
                            <Button variant="outlined" onClick={() => navigate("/evaluation/stage2")}>
                                Etap 2
                            </Button>
                            <Button variant="outlined" onClick={() => navigate("/admin")}>
                                Panel administratora
                            </Button>
                        </>
                    )}
                </Stack>
            </Paper>
        </Box>
    );
}
