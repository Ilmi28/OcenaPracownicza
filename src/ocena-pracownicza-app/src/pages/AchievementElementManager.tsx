import React, { useEffect, useState } from "react";
import {
    Paper, Typography, Box, TextField, Button,
    Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
    IconButton, Dialog, DialogTitle, DialogContent, DialogActions,
    CircularProgress, Tooltip, FormControl, InputLabel, Select, MenuItem, FormControlLabel, Switch
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import axiosClient from "../services/axiosClient";

export interface EvaluationPeriod {
    id: string;
    name: string;
    isClosed: boolean;
}

export interface AchievementElementListItem {
    id: string;
    code: string;
    activity: number;
    activityName: string;
    department: number;
    departmentName: string;
    category: number;
    categoryName: string;
    name: string;
    basePoints: number;
    isStackable: boolean;
    evaluationPeriodId: string;
    evaluationPeriodName: string;
}

export interface DictItem {
    id: number;
    label: string;
}

const AchievementElementManager: React.FC = () => {
    const [dictionaries, setDictionaries] = useState<{
        activities: DictItem[],
        departments: DictItem[],
        categories: DictItem[]
    }>({ activities: [], departments: [], categories: [] });

    const [periods, setPeriods] = useState<EvaluationPeriod[]>([]);
    const [selectedPeriodId, setSelectedPeriodId] = useState<string>("");
    const [elements, setElements] = useState<AchievementElementListItem[]>([]);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editingElement, setEditingElement] = useState<AchievementElementListItem | null>(null);
    const [formData, setFormData] = useState({
        code: "",
        activity: 0,
        department: 0,
        category: 0,
        name: "",
        basePoints: 0,
        isStackable: false
    });

    const fetchData = async () => {
        try {
            setLoading(true);
            const dictResp = await axiosClient.get("/achievementelement/dictionary");
            setDictionaries(dictResp.data);

            const periodResp = await axiosClient.get("/evaluation-periods");
            setPeriods(periodResp.data);
            
            if (periodResp.data.length > 0) {
                const active = periodResp.data.find((p: EvaluationPeriod) => !p.isClosed) || periodResp.data[0];
                setSelectedPeriodId(active.id);
            }
        } catch (err) {
            console.error("Błąd inicjalizacji danych", err);
        } finally {
            setLoading(false);
        }
    };

    const fetchElements = async () => {
        if (!selectedPeriodId) return;
        try {
            const resp = await axiosClient.get("/achievementelement");
            const filtered = resp.data.filter((e: AchievementElementListItem) => e.evaluationPeriodId === selectedPeriodId);
            setElements(filtered);
        } catch (err) {
            console.error("Błąd pobierania szablonów", err);
        }
    };

    useEffect(() => { fetchData(); }, []);
    useEffect(() => { fetchElements(); }, [selectedPeriodId]);

    const getLabel = (list: DictItem[], id: number) => list.find(x => x.id === id)?.label || "Nieznane";

    const openModal = (element: AchievementElementListItem | null = null) => {
        if (element) {
            setEditingElement(element);
            setFormData({
                code: element.code,
                activity: element.activity,
                department: element.department,
                category: element.category,
                name: element.name,
                basePoints: element.basePoints,
                isStackable: element.isStackable
            });
        } else {
            setEditingElement(null);
            setFormData({ code: "", activity: 0, department: 0, category: 0, name: "", basePoints: 0, isStackable: false });
        }
        setIsModalOpen(true);
    };

    const handleSave = async () => {
        setSaving(true);
        try {
            const payload = { ...formData, evaluationPeriodId: selectedPeriodId };
            if (editingElement) {
                await axiosClient.put(`/achievementelement/${editingElement.id}`, payload);
            } else {
                await axiosClient.post("/achievementelement", payload);
            }
            setIsModalOpen(false);
            fetchElements();
        } catch (err: any) {
            alert(err.response?.data?.message || "Błąd zapisu danych");
        } finally {
            setSaving(false);
        }
    };

    const handleDelete = async (id: string) => {
        if (!window.confirm("Czy na pewno chcesz usunąć ten szablon?")) return;
        try {
            await axiosClient.delete(`/achievementelement/${id}`);
            fetchElements();
        } catch (err) {
            alert("Nie można usunąć szablonu.");
        }
    };

    if (loading) return <Box sx={{ display: 'flex', justifyContent: 'center', p: 5 }}><CircularProgress /></Box>;

    return (
        <Box sx={{ p: 3 }}>
            <Paper sx={{ p: 3 }}>
                <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 3 }}>
                    <Typography variant="h5" fontWeight="700">Zarządzanie Szablonami</Typography>
                    <FormControl size="small" sx={{ minWidth: 220, ml: 'auto', mr: 2 }}>
                        <InputLabel id="period-select-label">Okres ocen</InputLabel>
                        <Select
                            labelId="period-select-label"
                            value={selectedPeriodId}
                            label="Okres ocen"
                            onChange={(e) => setSelectedPeriodId(e.target.value)}
                        >
                            {periods.map((p) => (
                                <MenuItem key={p.id} value={p.id}>{p.name}</MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                    <Button variant="contained" onClick={() => openModal()} disabled={!selectedPeriodId}>+ Dodaj</Button>
                </Box>

                <TableContainer>
                    <Table size="small">
                        <TableHead>
                            <TableRow>
                                <TableCell sx={{ fontWeight: 700 }}>Kod</TableCell>
                                <TableCell sx={{ fontWeight: 700 }}>Nazwa</TableCell>
                                <TableCell sx={{ fontWeight: 700 }}>Działalność</TableCell>
                                <TableCell sx={{ fontWeight: 700 }}>Dział</TableCell>
                                <TableCell sx={{ fontWeight: 700 }}>Kategoria</TableCell>
                                <TableCell sx={{ fontWeight: 700 }} align="center">Punkty</TableCell>
                                <TableCell sx={{ fontWeight: 700 }} align="center">Krotność</TableCell>
                                <TableCell sx={{ fontWeight: 700 }} align="right">Akcje</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {elements.length === 0 ? (
                                <TableRow>
                                    <TableCell colSpan={8} align="center" sx={{ p: 3, color: 'text.secondary' }}>
                                        Brak zdefiniowanych szablonów dla wybranego okresu ocen.
                                    </TableCell>
                                </TableRow>
                            ) : (
                                elements.map((item) => (
                                    <TableRow key={item.id} hover>
                                        <TableCell sx={{ fontWeight: 700 }}>{item.code}</TableCell>
                                        <TableCell sx={{ maxWidth: '300px' }}>{item.name}</TableCell>
                                        <TableCell>{getLabel(dictionaries.activities, item.activity)}</TableCell>
                                        <TableCell>{getLabel(dictionaries.departments, item.department)}</TableCell>
                                        <TableCell>{getLabel(dictionaries.categories, item.category)}</TableCell>
                                        <TableCell align="center" sx={{ fontWeight: 600 }}>{item.basePoints}</TableCell>
                                        <TableCell align="center">{item.isStackable ? "TAK" : "NIE"}</TableCell>
                                        <TableCell align="right">
                                            <Tooltip title="Edytuj">
                                                <IconButton onClick={() => openModal(item)} color="primary" size="small" sx={{ mr: 1 }}>
                                                    <EditIcon fontSize="small" />
                                                </IconButton>
                                            </Tooltip>
                                            <Tooltip title="Usuń">
                                                <IconButton onClick={() => handleDelete(item.id)} color="error" size="small">
                                                    <DeleteIcon fontSize="small" />
                                                </IconButton>
                                            </Tooltip>
                                        </TableCell>
                                    </TableRow>
                                ))
                            )}
                        </TableBody>
                    </Table>
                </TableContainer>
            </Paper>

            <Dialog open={isModalOpen} onClose={() => setIsModalOpen(false)} fullWidth maxWidth="sm">
                <DialogTitle sx={{ fontWeight: 700 }}>{editingElement ? "Edytuj Szablon" : "Nowy Szablon"}</DialogTitle>
                <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2.5, pt: 2 }}>
                    <Box sx={{ display: 'flex', gap: 2, mt: 1 }}>
                        <TextField 
                            label="Kod elementu" 
                            value={formData.code} 
                            fullWidth 
                            onChange={(e) => setFormData({...formData, code: e.target.value})} 
                        />
                        <TextField
                            label="Punkty bazowe"
                            type="number"
                            inputProps={{ step: "0.01" }}
                            value={formData.basePoints}
                            fullWidth
                            onChange={(e) => setFormData({ ...formData, basePoints: parseFloat(e.target.value) || 0 })}
                        />
                    </Box>
                    
                    <FormControl fullWidth>
                        <InputLabel>Działalność</InputLabel>
                        <Select value={formData.activity} label="Działalność" onChange={(e) => setFormData({...formData, activity: Number(e.target.value)})}>
                            {dictionaries.activities.map(a => <MenuItem key={a.id} value={a.id}>{a.label}</MenuItem>)}
                        </Select>
                    </FormControl>

                    <FormControl fullWidth>
                        <InputLabel>Dział weryfikujący</InputLabel>
                        <Select value={formData.department} label="Dział weryfikujący" onChange={(e) => setFormData({...formData, department: Number(e.target.value)})}>
                            {dictionaries.departments.map(d => <MenuItem key={d.id} value={d.id}>{d.label}</MenuItem>)}
                        </Select>
                    </FormControl>

                    <FormControl fullWidth>
                        <InputLabel>Kategoria</InputLabel>
                        <Select value={formData.category} label="Kategoria" onChange={(e) => setFormData({...formData, category: Number(e.target.value)})}>
                            {dictionaries.categories.map(c => <MenuItem key={c.id} value={c.id}>{c.label}</MenuItem>)}
                        </Select>
                    </FormControl>

                    <FormControlLabel
                        control={
                            <Switch
                                checked={formData.isStackable}
                                onChange={(e) => setFormData({ ...formData, isStackable: e.target.checked })}
                                color="primary"
                            />
                        }
                        label="Możliwość wielokrotnego rozliczania (Krotność)"
                    />

                    <TextField 
                        label="Nazwa kryterium osiągnięcia" 
                        value={formData.name} 
                        fullWidth 
                        multiline
                        rows={3}
                        onChange={(e) => setFormData({...formData, name: e.target.value})} 
                    />
                </DialogContent>
                <DialogActions sx={{ p: 2, bgcolor: 'grey.50' }}>
                    <Button onClick={() => setIsModalOpen(false)} color="inherit">Anuluj</Button>
                    <Button onClick={handleSave} variant="contained" disabled={saving || !formData.code || !formData.name}>
                        {saving ? "Zapisywanie..." : "Zapisz"}
                    </Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
};

export default AchievementElementManager;