import React, { useState } from "react";
import { Box, Typography, TextField, Button } from "@mui/material";
import axios from "axios";
import { authService } from "../services/authService";

const Settings: React.FC = () => {
    const [oldPassword, setOldPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleChangePassword = async () => {
        setMessage(""); // wyczyść komunikat

        try {
            await authService.changePassword(oldPassword, newPassword);

            setMessage("Hasło zostało zmienione.");
            setOldPassword("");
            setNewPassword("");
        } catch (error: unknown) {
            console.error(error);

            let backendMessage = "Wystąpił błąd podczas zmiany hasła.";

            //  Typ bezpieczny - tylko jeśli to AxiosError
            if (axios.isAxiosError(error)) {
                const data = error.response?.data;

                if (typeof data === "string" && data.trim()) {
                    backendMessage = data.trim();
                } else if (
                    typeof data === "object" &&
                    data !== null &&
                    "message" in data &&
                    typeof (data as { message?: unknown }).message === "string"
                ) {
                    backendMessage = (data as { message: string }).message;
                }
            }

            setMessage(backendMessage);
        }
    };

    return (
        <Box sx={{ maxWidth: 400 }}>
            <Typography variant="h5" sx={{ mb: 3 }}>
                Zmień hasło
            </Typography>

            <TextField
                label="Stare hasło"
                type="password"
                fullWidth
                sx={{ mb: 2 }}
                value={oldPassword}
                onChange={(e) => setOldPassword(e.target.value)}
            />

            <TextField
                label="Nowe hasło"
                type="password"
                fullWidth
                sx={{ mb: 2 }}
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
            />

            <Button variant="contained" fullWidth onClick={handleChangePassword}>
                Zmień hasło
            </Button>

            {message && (
                <Typography sx={{ mt: 2 }}>
                    {message}
                </Typography>
            )}
        </Box>
    );
};

export default Settings;
