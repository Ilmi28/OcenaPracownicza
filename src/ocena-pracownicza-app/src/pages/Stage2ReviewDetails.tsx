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
    IconButton,
} from "@mui/material";
import DownloadIcon from "@mui/icons-material/Download";
import EditIcon from "@mui/icons-material/Edit";
import SaveIcon from "@mui/icons-material/Save";
import CancelIcon from "@mui/icons-material/Cancel";
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
    const { achievementId } = useParams<{ achievementId: string }>();
    const navigate = useNavigate();
    const { user } = useAuth();
    const [data, setData] = useState<Stage2ReviewDetailsView | null>(null);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [comment, setComment] = useState("");
    
    const [score, setScore] = useState("");
    const [isEditingScore, setIsEditingScore] = useState(false);

    const [error, setError] = useState<string | null>(null);
    const [downloadingId, setDownloadingId] = useState<string | null>(null);

    const load = useCallback(async () => {
        if (!achievementId) return;
        setLoading(true);
        setError(null);
        try {
            const response = await evaluationService.getDetails(achievementId);
            setData(response);
            setComment(response.stage2Comment ?? "");
            setScore(response.finalScore ?? "0");          
        } catch (err: any) {
            const msg =
                err?.response?.data?.message ??
                err?.message ??
                "Nie udało się pobrać szczegółów.";
            setError(msg);
        } finally {
            setLoading(false);
        }
    }, [achievementId]);

    useEffect(() => {
        load();
    }, [load]);

    const handleDownloadAttachment = async (attachmentId: string, originalFileName: string) => {
        try {
            setDownloadingId(attachmentId);
            await evaluationService.downloadAttachment(attachmentId, originalFileName);
        } catch (err: any) {
            alert("Nie udało się pobrać pliku. Upewnij się, że masz odpowiednie uprawnienia.");
        } finally {
            setDownloadingId(null);
        }
    };

    const onUpdateScore = async () => {
        if (!achievementId || !data) return;
        setSaving(true);
        setError(null);
        try {
            const updated = await evaluationService.updateAchievement(achievementId, {
                finalScore: score.trim(),
                name: data.achievementName,
                description: data.achievementsSummary
            });
            setData(updated);
            setScore(updated.finalScore ?? "0");
            setIsEditingScore(false);
        } catch (err: any) {
            setError(err?.response?.data?.message ?? "Nie udało się zaktualizować punktacji.");
        } finally {
            setSaving(false);
        }
    };

    const onApprove = async () => {
        if (!achievementId) return;
        setSaving(true);
        setError(null);
        try {
            const updated = await evaluationService.approve(achievementId, comment);
            setData(updated);
        } catch (err: any) {
            setError(err?.response?.data?.message ?? "Nie udało się zatwierdzić.");
        } finally {
            setSaving(false);
        }
    };

    const onReject = async () => {
        if (!achievementId) return;
        setSaving(true);
        setError(null);
        try {
            const updated = await evaluationService.reject(achievementId, comment);
            setData(updated);
        } catch (err: any) {
            setError(err?.response?.data?.message ?? "Nie udało się odrzucić.");
        } finally {
            setSaving(false);
        }
    };

    const onClose = async () => {
        if (!achievementId) return;
        setSaving(true);
        setError(null);
        try {
            const updated = await evaluationService.close(achievementId);
            setData(updated);
        } catch (err: any) {
            setError(err?.response?.data?.message ?? "Nie udało się zamknąć oceny.");
        } finally {
            setSaving(false);
        }
    };

    const onArchive = async () => {
        if (!achievementId) return;
        setSaving(true);
        setError(null);
        try {
            const updated = await evaluationService.archive(achievementId);
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

                    {                              }
                    <Grid size={{ xs: 12, md: 6 }}>
                        <Typography variant="body2" color="text.secondary">
                            Wynik końcowy (Punkty)
                        </Typography>
                        {isEditingScore ? (
                            <Box display="flex" alignItems="center" gap={1} mt={0.5}>
                                <TextField
                                    size="small"
                                    type="number"
                                    value={score}
                                    onChange={(e) => setScore(e.target.value)}
                                    disabled={saving}
                                    sx={{ width: 120 }}
                                />
                                <IconButton color="success" onClick={onUpdateScore} disabled={saving}>
                                    <SaveIcon />
                                </IconButton>
                                <IconButton color="error" onClick={() => { setIsEditingScore(false); setScore(data.finalScore); }} disabled={saving}>
                                    <CancelIcon />
                                </IconButton>
                            </Box>
                        ) : (
                            <Box display="flex" alignItems="center" gap={1}>
                                <Typography fontWeight={700}>{data.finalScore}</Typography>
                                {canDecide && (
                                    <IconButton size="small" color="primary" onClick={() => setIsEditingScore(true)}>
                                        <EditIcon fontSize="small" />
                                    </IconButton>
                                )}
                            </Box>
                        )}
                    </Grid>

                    <Grid size={{ xs: 12, md: 6 }}>
                        <Typography variant="body2" color="text.secondary">
                            Załącznik
                        </Typography>
                        {data.attachmentId ? (
                            <Box display="flex" alignItems="center" mt={0.5}>
                                <Button
                                    size="small"
                                    variant="outlined"
                                    startIcon={downloadingId === data.attachmentId ? <CircularProgress size={16} /> : <DownloadIcon />}
                                    disabled={downloadingId !== null}
                                    onClick={() => handleDownloadAttachment(data.attachmentId!, data.attachmentFileName ?? `Zalacznik_${data.achievementName}`)}
                                >
                                    {downloadingId === data.attachmentId ? "Pobieranie..." : "Pobierz plik"}
                                </Button>
                            </Box>
                        ) : (
                            <Typography variant="body2" color="text.disabled" sx={{ mt: 0.5 }}>
                                Brak załącznika
                            </Typography>
                        )}
                    </Grid>

                    <Grid size={{ xs: 12 }}>
                        <Typography variant="body2" color="text.secondary">
                            Podsumowanie osiągnięć
                        </Typography>
                        <Typography>{data.achievementsSummary}</Typography>
                    </Grid>
                </Grid>
            </Paper>
            <Paper sx={{ p: 3, mb: 3}}>
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
                    disabled={saving || canArchive || canClose}
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
                            <TableCell align="center">Załącznik</TableCell>
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
                                <TableCell align="center">
                                    {item.attachmentId ? (
                                        <Button
                                            size="small"
                                            variant="text"
                                            startIcon={downloadingId === item.attachmentId ? <CircularProgress size={16} /> : <DownloadIcon />}
                                            disabled={downloadingId !== null}
                                            onClick={() => handleDownloadAttachment(item.attachmentId!, item.attachmentFileName ?? `Zalacznik_${item.name}`)}
                                        >
                                            {downloadingId === item.attachmentId ? "Pobieranie..." : "Pobierz"}
                                        </Button>
                                    ) : (
                                        <Typography variant="caption" color="text.disabled">
                                            Brak
                                        </Typography>
                                    )}
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </Paper>


        </Box>
    );
}