// src/components/users/UserList.tsx
import React, { useState } from "react";
import {
    Paper,
    Table,
    TableHead,
    TableRow,
    TableCell,
    TableBody,
    IconButton,
    Typography,
    CircularProgress,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import DeleteIcon from "@mui/icons-material/Delete";
import PasswordIcon from "@mui/icons-material/LockReset";
import { userService } from "../../services/userService";
import { IUser } from "../../models/User";

type Props = {
    users: IUser[];
    loading: boolean;
    onReload: () => void;
    onEdit: (user: IUser) => void;
};

const UserList: React.FC<Props> = ({ users, loading, onReload, onEdit }) => {
    const [deleting, setDeleting] = useState<string | null>(null);
    const [confirmDelete, setConfirmDelete] = useState<IUser | null>(null);
    const [resetting, setResetting] = useState<IUser | null>(null);

    const handleDelete = async (id: string) => {
        setDeleting(id);
        try {
            await userService.delete(id);
            onReload();
        } catch (e) {
            console.error(e);
            alert("B³¹d podczas usuwania");
        } finally {
            setDeleting(null);
            setConfirmDelete(null);
        }
    };

    const handleReset = async (user: IUser) => {
        // adminResetPassword mo¿e nie istnieæ - obs³u¿ b³¹d
        try {
            await userService.adminResetPassword(user.id);
            alert("Has³o zosta³o zresetowane (sprawdŸ e-mail u¿ytkownika).");
            onReload();
        } catch (e) {
            console.error(e);
            alert("Reset has³a nie jest dostêpny lub wyst¹pi³ b³¹d.");
        } finally {
            setResetting(null);
        }
    };

    if (loading) return <CircularProgress />;

    return (
        <Paper sx={{ p: 2 }}>
            <Typography variant="h6" gutterBottom>U¿ytkownicy</Typography>
            <Table>
                <TableHead>
                    <TableRow>
                        <TableCell>Imiê</TableCell>
                        <TableCell>Nazwisko</TableCell>
                        <TableCell>Email</TableCell>
                        <TableCell>Rola</TableCell>
                        <TableCell align="right">Akcje</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {users.map(u => (
                        <TableRow key={u.id}>
                            <TableCell>{u.firstName ?? "-"}</TableCell>
                            <TableCell>{u.lastName ?? "-"}</TableCell>
                            <TableCell>{u.email}</TableCell>
                            <TableCell>{(u.roles || []).join(", ")}</TableCell>
                            <TableCell align="right">
                                <IconButton onClick={() => onEdit(u)} size="small"><EditIcon /></IconButton>
                                <IconButton onClick={() => setConfirmDelete(u)} size="small"><DeleteIcon color="error" /></IconButton>
                                <IconButton onClick={() => setResetting(u)} size="small"><PasswordIcon /></IconButton>
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>

            {/* Delete confirm */}
            <Dialog open={!!confirmDelete} onClose={() => setConfirmDelete(null)}>
                <DialogTitle>Usuñ u¿ytkownika</DialogTitle>
                <DialogContent>
                    Czy na pewno chcesz usun¹æ <b>{confirmDelete?.email}</b>?
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setConfirmDelete(null)}>Anuluj</Button>
                    <Button color="error" onClick={() => handleDelete(confirmDelete!.id)} disabled={deleting === confirmDelete?.id}>
                        {deleting === confirmDelete?.id ? "Usuwanie..." : "Usuñ"}
                    </Button>
                </DialogActions>
            </Dialog>

            {/* Reset confirm */}
            <Dialog open={!!resetting} onClose={() => setResetting(null)}>
                <DialogTitle>Reset has³a</DialogTitle>
                <DialogContent>
                    Czy chcesz zresetowaæ has³o u¿ytkownika <b>{resetting?.email}</b>? (jeœli endpoint istnieje, nowe has³o zostanie wys³ane e-mail)
                </DialogContent>
                <DialogActions>
                    <Button onClick={() => setResetting(null)}>Anuluj</Button>
                    <Button onClick={() => handleReset(resetting!)}>Resetuj</Button>
                </DialogActions>
            </Dialog>
        </Paper>
    );
};

export default UserList;
