// src/pages/ChangePassword.tsx
import React, { useState } from "react";
import { Box, TextField, Button, Typography } from "@mui/material";
import { userService }  from "../services/userService";

const ChangePassword: React.FC = () => {
    const [current, setCurrent] = useState("");
    const [newPass, setNewPass] = useState("");
    const [confirm, setConfirm] = useState("");
    const [loading, setLoading] = useState(false);

    const submit = async () => {
        if (newPass !== confirm) {
            alert("Nowe has³o i potwierdzenie nie s¹ takie same");
            return;
        }
        setLoading(true);
        try {
            await userService.changePassword(current, newPass);
            alert("Has³o zmienione pomyœlnie");
            setCurrent(""); setNewPass(""); setConfirm("");
        } catch (e: unknown) {
            console.error(e);
            alert("B³¹d podczas zmiany has³a");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box sx={{ p: 3, maxWidth: 480 }}>
            <Typography variant="h5">Zmieñ has³o</Typography>
            <TextField label="Obecne has³o" type="password" value={current} onChange={(e) => setCurrent(e.target.value)} fullWidth sx={{ mt: 2 }} />
            <TextField label="Nowe has³o" type="password" value={newPass} onChange={(e) => setNewPass(e.target.value)} fullWidth sx={{ mt: 2 }} />
            <TextField label="Powtórz nowe has³o" type="password" value={confirm} onChange={(e) => setConfirm(e.target.value)} fullWidth sx={{ mt: 2 }} />
            <Button variant="contained" sx={{ mt: 2 }} onClick={submit} disabled={loading}>Zmieñ has³o</Button>
        </Box>
    );
};

export default ChangePassword;
