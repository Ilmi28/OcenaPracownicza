import React, { useState } from "react";
import { Box, Typography, TextField, Button } from "@mui/material";
import axios from "axios";
import { authService } from "../services/authService";

const Settings: React.FC = () => {
    const [oldPassword, setOldPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [message, setMessage] = useState("");

    const handleChangePassword = async () => {
        setMessage(""); // wyczyœæ komunikat

        try {
            await authService.changePassword(oldPassword, newPassword);

            setMessage("Has³o zosta³o zmienione.");
            setOldPassword("");
            setNewPassword("");
        } catch (error: unknown) {
            console.error(error);

            let backendMessage = "Wyst¹pi³ b³¹d podczas zmiany has³a.";

            //  Typ bezpieczny - tylko jeœli to AxiosError
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
                Zmieñ has³o
            </Typography>

            <TextField
                label="Stare has³o"
                type="password"
                fullWidth
                sx={{ mb: 2 }}
                value={oldPassword}
                onChange={(e) => setOldPassword(e.target.value)}
            />

            <TextField
                label="Nowe has³o"
                type="password"
                fullWidth
                sx={{ mb: 2 }}
                value={newPassword}
                onChange={(e) => setNewPassword(e.target.value)}
            />

            <Button variant="contained" fullWidth onClick={handleChangePassword}>
                Zmieñ has³o
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
