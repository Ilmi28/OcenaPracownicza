import React, { useEffect, useState } from "react";
import {
    Paper, Typography, Box, TextField, Button,
    Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
    IconButton, Dialog, DialogTitle, DialogContent, DialogActions,
    CircularProgress, Tooltip
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import axiosClient from "../services/axiosClient";

interface EvaluationPeriod {
    id: string;
    name: string;
    startDate: string;
    endDate: string;
    regulationVersion: string;             
}

const EvaluationPeriodManager: React.FC = () => {
    const [periods, setPeriods] = useState<EvaluationPeriod[]>([]);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingPeriod, setEditingPeriod] = useState<EvaluationPeriod | null>(null);
    const [formData, setFormData] = useState({
        name: "",
        startDate: "",
        endDate: "",
        regulationVersion: ""          
    });

    const fetchPeriods = async () => {
        try {
            const resp = await axiosClient.get("/evaluation-periods");
            setPeriods(resp.data);
        } catch (err) {
            console.error("Błąd pobierania okresów", err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => { fetchPeriods(); }, []);

    const openModal = (period: EvaluationPeriod | null = null) => {
        if (period) {
            setEditingPeriod(period);
            setFormData({
                name: period.name,
                startDate: period.startDate.slice(0, 10),
                endDate: period.endDate.slice(0, 10),
                regulationVersion: period.regulationVersion || ""
            });
        } else {
            setEditingPeriod(null);
            setFormData({ 
                name: "", 
                startDate: "", 
                endDate: "", 
                regulationVersion: "Zarządzanie nr 1..."                
            });
        }
        setIsModalOpen(true);
    };

    const handleSave = async () => {
        setSaving(true);
        try {
            if (editingPeriod) {
                await axiosClient.put(`/evaluation-periods/${editingPeriod.id}`, formData);
            } else {
                await axiosClient.post("/evaluation-periods", formData);
            }
            setIsModalOpen(false);
            fetchPeriods();
        } catch (err: any) {
            alert(err.response?.data?.message || "Błąd zapisu danych");
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async (id: string) => {
        if (!window.confirm("Czy na pewno chcesz usunąć ten okres?")) return;
        try {
            await axiosClient.delete(`/evaluation-periods/${id}`);
            setPeriods(periods.filter(p => p.id !== id));
        } catch (err) {
            alert("Nie można usunąć okresu (prawdopodobnie posiada przypisane osiągnięcia).");
        }
    };

    if (loading) return (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 5 }}>
            <CircularProgress />
        </Box>
    );

    return (
        <Box sx={{ p: 3 }}>
            <Paper sx={{ p: 3 }}>
                <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
                    <Typography variant="h5" fontWeight="700" color="primary.main">
                        Zarządzanie Okresami Ocen
                    </Typography>
                    <Button 
                        variant="contained" 
                        onClick={() => openModal()}
                        sx={{ borderRadius: 2, px: 3 }}
                    >
                        + Dodaj Nowy Okres
                    </Button>
                </Box>

                <TableContainer>
                    <Table size="small">
                        <TableHead sx={{ bgcolor: "grey.100" }}>
                            <TableRow>
                                <TableCell sx={{ fontWeight: 700 }}>Nazwa Okresu</TableCell>
                                <TableCell sx={{ fontWeight: 700 }}>Data Startu</TableCell>
                                <TableCell sx={{ fontWeight: 700 }}>Data Końca</TableCell>
                                <TableCell sx={{ fontWeight: 700 }}>Regulamin</TableCell>
                                <TableCell sx={{ fontWeight: 700 }} align="center">Status</TableCell>
                                <TableCell sx={{ fontWeight: 700 }} align="right">Akcje</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {periods.map((p) => {
                                const isPast = new Date(p.endDate) < new Date();
                                return (
                                    <TableRow key={p.id} hover>
                                        <TableCell sx={{ fontWeight: 600 }}>{p.name}</TableCell>
                                        <TableCell>{new Date(p.startDate).toLocaleDateString()}</TableCell>
                                        <TableCell>{new Date(p.endDate).toLocaleDateString()}</TableCell>
                                        <TableCell>
                                            <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
                                                {p.regulationVersion}
                                            </Box>
                                        </TableCell>
                                        <TableCell align="center">
                                            <Box component="span" sx={{
                                                px: 1.5, py: 0.5, borderRadius: 1, fontSize: '0.75rem', fontWeight: 700,
                                                bgcolor: isPast ? "error.light" : "success.light",
                                                color: isPast ? "error.dark" : "success.dark"
                                            }}>
                                                {isPast ? "ZAKOŃCZONY" : "AKTYWNY"}
                                            </Box>
                                        </TableCell>
                                        <TableCell align="right">
                                            <Tooltip title="Edytuj">
                                                <IconButton onClick={() => openModal(p)} color="primary" size="small" sx={{ mr: 1 }}>
                                                    <EditIcon fontSize="small" />
                                                </IconButton>
                                            </Tooltip>
                                            <Tooltip title="Usuń">
                                                <IconButton onClick={() => handleDelete(p.id)} color="error" size="small">
                                                    <DeleteIcon fontSize="small" />
                                                </IconButton>
                                            </Tooltip>
                                        </TableCell>
                                    </TableRow>
                                );
                            })}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Paper>

            <Dialog open={isModalOpen} onClose={() => setIsModalOpen(false)} fullWidth maxWidth="xs">
                <DialogTitle sx={{ fontWeight: 700 }}>
                    {editingPeriod ? "Edytuj Okres" : "Nowy Okres Oceny"}
                </DialogTitle>
                <DialogContent>
                    <Box sx={{ mt: 1, display: 'flex', flexDirection: 'column', gap: 2 }}>
                        <TextField
                            fullWidth
                            label="Nazwa okresu"
                            value={formData.name}
                            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                        />
                        <TextField
                            fullWidth
                            label="Wersja regulaminu"
                            placeholder="np. 2.0 lub 2024/A"
                            value={formData.regulationVersion}
                            onChange={(e) => setFormData({ ...formData, regulationVersion: e.target.value })}
                        />
                        <TextField
                            fullWidth
                            type="date"
                            label="Data rozpoczęcia"
                            InputLabelProps={{ shrink: true }}
                            value={formData.startDate}
                            onChange={(e) => setFormData({ ...formData, startDate: e.target.value })}
                        />
                        <TextField
                            fullWidth
                            type="date"
                            label="Data zakończenia"
                            InputLabelProps={{ shrink: true }}
                            value={formData.endDate}
                            onChange={(e) => setFormData({ ...formData, endDate: e.target.value })}
                        />
                    </Box>
                </DialogContent>
                <DialogActions sx={{ p: 2, bgcolor: 'grey.50' }}>
                    <Button onClick={() => setIsModalOpen(false)} color="inherit">Anuluj</Button>
                    <Button 
                        onClick={handleSave} 
                        variant="contained" 
                        disabled={saving || !formData.name || !formData.regulationVersion}
                    >
                        {saving ? "Zapisywanie..." : "Zapisz zmiany"}
                    </Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
};

export default EvaluationPeriodManager;