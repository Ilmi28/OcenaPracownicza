import { useEffect, useState, useMemo, useCallback } from "react";
import { 
    Box, Paper, Table, TableBody, TableCell, TableHead, TableRow, 
    Typography, Button, MenuItem, TextField, CircularProgress, Chip, 
    IconButton, Alert, Dialog, DialogTitle, DialogContent, DialogActions 
} from "@mui/material";
import DeleteIcon from "@mui/icons-material/Delete";
import EditIcon from "@mui/icons-material/Edit";
import AddIcon from "@mui/icons-material/Add";
import { useAuth } from "../hooks/AuthProvider";
import axiosClient from "../services/axiosClient";

export default function GradeView() {
    const { user } = useAuth();
    const [grades, setGrades] = useState<any[]>([]);
    const [employees, setEmployees] = useState<any[]>([]);
    const [periods, setPeriods] = useState<any[]>([]);
    const [selectedPeriod, setSelectedPeriod] = useState("");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const [openDialog, setOpenDialog] = useState(false);
    const [formData, setFormData] = useState({ id: "", employeeId: "", value: "", comment: "" });
    const [isEdit, setIsEdit] = useState(false);

    const isManagerOrAdmin = user?.role === "Manager" || user?.role === "Admin";

    const fetchData = useCallback(async () => {
        setLoading(true);
        try {
            const [pRes, eRes] = await Promise.all([
                axiosClient.get("/evaluation-periods"),
                isManagerOrAdmin ? axiosClient.get("/employee") : Promise.resolve({ data: [] })
            ]);
            setPeriods(pRes.data);
            setEmployees(Array.isArray(eRes.data) ? eRes.data : (eRes.data?.data || []));
            if (pRes.data?.length > 0 && !selectedPeriod) setSelectedPeriod(pRes.data[0].id);
        } catch (err) { setError("Błąd ładowania słowników."); }
        setLoading(false);
    }, [isManagerOrAdmin, selectedPeriod]);

    useEffect(() => { fetchData(); }, [fetchData]);

    const loadGrades = useCallback(async () => {
        if (!user || !user.userId) return;
        setLoading(true);
        setError(null);
        
        try {
            let res;
            if (user.role === "Employee") {
                res = await axiosClient.get(`/grades/employee/${user.userId}`);
            } else {
                if (!selectedPeriod) return;
                res = await axiosClient.get(`/grades/period/${selectedPeriod}`);
            }
            
            const rawData = res.data?.data || res.data || [];
            
            let filtered = rawData;
            if (user.role === "Employee" && selectedPeriod) {
                const periodName = periods.find(p => p.id === selectedPeriod)?.name;
                filtered = rawData.filter((g: any) => g.periodName === periodName);
            }
            
            setGrades(filtered);
        } catch (err: any) {
            setError("Brak uprawnień lub błąd serwera.");
            setGrades([]);
        } finally {
            setLoading(false);
        }
    }, [selectedPeriod, user, periods]);

    useEffect(() => { loadGrades(); }, [loadGrades]);

    const handleSave = async () => {
        try {
            if (isEdit) {
                await axiosClient.put(`/grades/${formData.id}`, { value: Number(formData.value), comment: formData.comment });
            } else {
                await axiosClient.post("/grades", { ...formData, evaluationPeriodId: selectedPeriod, value: Number(formData.value) });
            }
            loadGrades();
            setOpenDialog(false);
        } catch (err) { alert("Błąd zapisu."); }
    };

    const handleDelete = async (id: string) => {
        if (window.confirm("Na pewno usunąć ocenę?")) {
            await axiosClient.delete(`/grades/${id}`);
            loadGrades();
        }
    };

    const reportData = useMemo(() => {
        if (user?.role === "Employee") {
            return grades.map(g => ({ id: g.id, grade: g }));
        }
        return employees.map(emp => ({ 
            ...emp, 
            grade: grades.find((g: any) => g.employeeId === emp.id) || null 
        }));
    }, [employees, grades, user]);

    return (
        <Box p={3}>
            <Typography variant="h5" mb={3}>Zarządzanie ocenami</Typography>
            {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}

            <TextField select label="Okres" value={selectedPeriod} onChange={(e) => setSelectedPeriod(e.target.value)} sx={{ width: 250, mb: 3 }}>
                {periods.map(p => <MenuItem key={p.id} value={p.id}>{p.name}</MenuItem>)}
            </TextField>

            <Paper>
                <Table>
                    <TableHead>
                        <TableRow>
                            <TableCell>{user?.role === "Employee" ? "Okres" : "Pracownik"}</TableCell>
                            <TableCell>Ocena</TableCell>
                            <TableCell>Komentarz</TableCell>
                            {isManagerOrAdmin && <TableCell align="center">Akcje</TableCell>}
                        </TableRow>
                    </TableHead>
                    <TableBody>
                        {reportData.map((row: any, i: number) => (
                            <TableRow key={row.id || i}>
                                <TableCell>
                                    {user?.role === "Employee" ? (row.grade?.periodName || "Okres") : `${row.firstName || ""} ${row.lastName || ""}`}
                                </TableCell>
                                <TableCell><Chip label={row.grade?.value ?? "Brak"} color={row.grade?.value ? "primary" : "default"} /></TableCell>
                                <TableCell>{row.grade?.comment ?? "-"}</TableCell>
                                {isManagerOrAdmin && (
                                    <TableCell align="center">
                                        {row.grade ? (
                                            <>
                                                <IconButton onClick={() => { setFormData(row.grade); setIsEdit(true); setOpenDialog(true); }}><EditIcon /></IconButton>
                                                <IconButton color="error" onClick={() => handleDelete(row.grade.id)}><DeleteIcon /></IconButton>
                                            </>
                                        ) : (
                                            <Button onClick={() => { setFormData({id:"", employeeId: row.id, value:"", comment:""}); setIsEdit(false); setOpenDialog(true); }}><AddIcon /></Button>
                                        )}
                                    </TableCell>
                                )}
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </Paper>

            <Dialog open={openDialog} onClose={() => setOpenDialog(false)}>
                <DialogTitle>{isEdit ? "Edytuj Ocenę" : "Dodaj Ocenę"}</DialogTitle>
                <DialogContent>
                    <TextField label="Wartość" fullWidth margin="dense" type="number" value={formData.value} onChange={e => setFormData({...formData, value: e.target.value})} />
                    <TextField label="Komentarz" fullWidth margin="dense" value={formData.comment} onChange={e => setFormData({...formData, comment: e.target.value})} />
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setOpenDialog(false)}>Anuluj</Button>
                    <Button variant="contained" onClick={handleSave}>Zapisz</Button>
                </DialogActions>
            </Dialog>
        </Box>
    );
}