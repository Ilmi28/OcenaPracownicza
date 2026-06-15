import { useCallback, useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import {
    Alert,
    Box,
    Button,
    CircularProgress,
    Grid,
    Paper,
    TextField,
    Typography,
    Chip,
    IconButton,
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import SaveIcon from "@mui/icons-material/Save";
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
    6: "Oczekuje do poprawy",
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
            setError(err?.response?.data?.message ?? "Nie udało się pobrać szczegółów.");
        } finally {
            setLoading(false);
        }
    }, [achievementId]);

    useEffect(() => {
        load();
    }, [load]);

    const onUpdateScore = async () => {
        if (!achievementId || !data) return;
        setSaving(true);
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
            setError(err?.response?.data?.message ?? "Błąd aktualizacji punktacji.");
        } finally {
            setSaving(false);
        }
    };

    const handleAction = async (action: () => Promise<Stage2ReviewDetailsView>, errorMessage: string, requireComment = false) => {
        if (requireComment && (!comment || comment.trim() === "")) {
            setError("Komentarz jest wymagany dla tej akcji.");
            return;
        }

        setSaving(true);
        setError(null);
        try {
            const updated = await action();
            setData(updated);
        } catch (err: any) {
            setError(err?.response?.data?.message ?? errorMessage);
        } finally {
            setSaving(false);
        }
    };

    if (loading) return <Box display="flex" justifyContent="center" mt={8}><CircularProgress /></Box>;
    if (!data) return <Alert severity="success">Pomyślnie zmieniono rekord.</Alert>

    const canDecide = data.stage2Status === 1;
    const isAdmin = user?.role === "Admin";
    const canClose = isAdmin && (data.stage2Status === 2 || data.stage2Status === 3);

    return (
        <Box>
            <Box display="flex" justifyContent="space-between" alignItems="center" mb={2}>
                <Typography variant="h5" fontWeight={600}>Weryfikacja etapu 2</Typography>
                <Button variant="outlined" onClick={() => navigate("/evaluation/stage2")}>Wróć do kolejki</Button>
            </Box>
            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
            
            <Paper sx={{ p: 3, mb: 3 }}>
                <Grid container spacing={2}>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <Typography variant="body2" color="text.secondary">Pracownik</Typography>
                        <Typography>{data.fullName}</Typography>
                    </Grid>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <Typography variant="body2" color="text.secondary">Osiągnięcie</Typography>
                        <Typography>{data.achievementName}</Typography>
                    </Grid>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <Typography variant="body2" color="text.secondary">Status</Typography>
                        <Chip 
                            label={STATUS_LABELS[data.stage2Status]} 
                            color={data.stage2Status === 2 ? "success" : data.stage2Status === 6 ? "warning" : "default"} 
                            variant="outlined" 
                        />
                    </Grid>
                    <Grid size={{ xs: 12, md: 6 }}>
                        <Typography variant="body2" color="text.secondary">Wynik (Punkty)</Typography>
                        {isEditingScore ? (
                            <Box display="flex" alignItems="center" gap={1}>
                                <TextField size="small" type="number" value={score} onChange={(e) => setScore(e.target.value)} sx={{ width: 100 }} />
                                <IconButton color="success" onClick={onUpdateScore}><SaveIcon /></IconButton>
                            </Box>
                        ) : (
                            <Typography>{data.finalScore} {canDecide && <IconButton size="small" onClick={() => setIsEditingScore(true)}><EditIcon fontSize="small" /></IconButton>}</Typography>
                        )}
                    </Grid>
                </Grid>
            </Paper>

            <Paper sx={{ p: 3, mb: 3 }}>
                <Typography variant="subtitle1" fontWeight={600} mb={2}>Decyzja komisji</Typography>
                <TextField 
                    fullWidth 
                    multiline 
                    minRows={3} 
                    label="Komentarz" 
                    value={comment} 
                    onChange={(e) => setComment(e.target.value)} 
                    sx={{ mb: 2 }} 
                />
                <Box display="flex" gap={1} justifyContent="flex-end">
                    <Button 
                        variant="contained" 
                        color="warning" 
                        onClick={() => handleAction(() => evaluationService.returnForCorrection(achievementId!, comment), "Błąd zwracania do poprawy.", true)} 
                        disabled={saving || !canDecide}
                    >
                        Do poprawy
                    </Button>
                    <Button 
                        variant="outlined" 
                        color="error" 
                        onClick={() => handleAction(() => evaluationService.reject(achievementId!, comment), "Błąd odrzucenia.", true)} 
                        disabled={saving || !canDecide}
                    >
                        Odrzuć
                    </Button>
                    <Button 
                        variant="contained" 
                        color="success" 
                        onClick={() => handleAction(() => evaluationService.approve(achievementId!, comment), "Błąd zatwierdzania.")} 
                        disabled={saving || !canDecide}
                    >
                        Zatwierdź
                    </Button>
                    <Button 
                        variant="outlined" 
                        onClick={() => handleAction(() => evaluationService.close(achievementId!), "Błąd zamykania.")} 
                        disabled={saving || !canClose}
                    >
                        Zamknij
                    </Button>
                </Box>
            </Paper>
        </Box>
    );
}